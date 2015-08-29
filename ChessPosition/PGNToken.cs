using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JPD.Parser;

namespace ChessPosition
{
    public enum PGNTokenType { Tag, MoveNumber, MoveString, Comment, Terminator, Escape, Invalid }; // ### should add support for variations, also
    public class PGNToken
    {
        public PGNTokenType tokenType;
        public string tokenString;
        public int startLocation;

        static public List<PGNToken> TokenFactory(Sentence game, ParseNode rootNode, string rootToken)
        {
            List<ParseNode> candidates = rootNode.FindKids(rootToken);
            return TokenFactory(game, candidates);
        }
        static public List<PGNToken> TokenFactory(Sentence game, List<ParseNode> candidates)
        {
            List<PGNToken> outTokens = new List<PGNToken>();

            PGNMoveString curMove = null;
            foreach (ParseNode candidate in candidates)
            {
                try
                {
                    switch (candidate.name)
                    {
                        case "TagPair*":
                        case "TagPair":
                            outTokens.Add(new PGNTag(game.GetTokenMatch(candidate, "Symbol"), game.GetTokenMatch(candidate, "String")));
                            break;
                        case "MoveText*":
                        case "MoveText":
                            // ### move tokens should treat + and # as objective markers to be included.
                            // The various things we can see at this point are (in no particular order):
                            //  PlyText, Comment, RAVText, Escape
                            foreach (ParseNode moveImpl in candidate.kids)
                            {
                                string moveType = moveImpl.implPattern;
                                try
                                {
                                    switch (moveType)
                                    {
                                        case "PlyText":
                                            List<PGNToken> incrPlyToken = BuildMoveToken(game, moveImpl);
                                            outTokens.AddRange(incrPlyToken);
                                            foreach (PGNToken t in incrPlyToken)
                                                if (t.tokenType == PGNTokenType.MoveString)
                                                    curMove = (PGNMoveString)t;
                                            break;
                                        case "RAVText":
                                            // this opens a variation.  For its direct kide, shey will belong to the move token it "interrupts"
                                            List<PGNToken> curVar = TokenFactory(game, moveImpl.kids);    // ###
                                            curMove.variations.Add(curVar);
                                            break;
                                        case "Escape":
                                            outTokens.Add(new PGNEscape(game.GetTokenMatch(moveImpl, "Escape")));
                                            break;
                                        case "Comment":
                                            outTokens.Add(new PGNComment(game.GetTokenMatch(moveImpl, moveImpl.kids[0].name)));
                                            break;
                                        default:
                                            break;
                                    }
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e.Message);
                                }
                            }
                            break;
                        case "LParen":
                        case "RParen":
                            if (candidate.parent.name == "RAVText")     // save to ignore...
                                continue;
                            break;
                        case "GameTerminator":
                            outTokens.Add(new PGNTerminator(game.GetTokenMatch(candidate, 0)));
                            break;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            return outTokens;
        }
        static private List<PGNToken> BuildMoveToken(Sentence game, ParseNode node)
        {
            List<PGNToken> outTokens = new List<PGNToken>();
            PGNMoveString curMove = null;
            foreach (ParseNode n in node.kids)
            {
                try
                {

                    switch (n.name)
                    {
                        case "_":
                            break;
                        case "MoveNbr":
                            outTokens.Add(new PGNMoveNumber(game.GetTokenMatch(n, "Integer")));
                            break;
                        case "PieceMoveText":
                        case "CastleMoveText":
                            outTokens.Add(curMove = new PGNMoveString(game.GetTokenMatch(n, n.name)));
                            break;
                        case "Annotation":
                            curMove.annotation = game.GetTokenMatch(n, n.name);
                            break;
                        case "NAG":
                            curMove.NAG = Convert.ToInt32(game.GetTokenMatch(n, "Integer"));
                            break;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

            }

            return outTokens;
        }
        static public PGNToken TokenFactory(Token pToken)
        {
            switch (pToken.name)
            {
                /// we actually need the parser to be much smarter
                /// it needs to be able to walk the hierarchy of instantiated sentences
                /// right now, sentences are replaced with sentence/token definitions and these
                /// are ALL ultimately instantiated by tokens - the higher order structure is lost, and all that's
                /// returned is the stream of token (and token names) that satisfies the initial grammar
                /// unfortunately, things like a PGNTag is composed of at least 4 parser tokens
                /// being able to walk through the hierarchy at a higher level, to say:
                /// Game - token 0-120
                ///     |-> WhiteSpace 0-5, TagSection 6-40, WhiteSpace 41-45, MoveText 46-118, WhiteSpace 119, GameTerm 120
                ///                             |-> TagPair 6-12 WhiteSpace 12-13 TagPair 14-20 WhiteSpace 21-22 ...
                ///                                 |-> LBracket 6 WS 7 Symbol 7 WS 9 String 10 WS 11 RBracket 12
                ///  then we can easily walk the sentences without having to reparse the string!
                ///   oh, we're in the tagSection, instantiate all the tagPairs I have, in each get the tag/val, etc
            }
            return null;
        }
    }
    public class PGNTag : PGNToken
    {
        public string key;
        public string value;

        public PGNTag(string k, string v)
        {
            key = k;
            value = v;
        }

        public PGNTag(string s, int offset)
        {
            startLocation = offset;
            tokenType = PGNTokenType.Tag;
            // only two things will interfere here, embedded quoted values and a close ']'
            int lquote = s.IndexOf('\"', offset);
            int rquote = (lquote < 0 ? -1 : s.IndexOf('\"', lquote + 1));
            int rbracket = s.IndexOf(']', offset);
            if (rbracket < lquote)     // no quotes in _this_ bracket set
                lquote = rquote = -1;
            if (rquote > 0 && rbracket < rquote) // current bracket is quoted
                rbracket = s.IndexOf(']', rquote);

            if (rbracket >= 0) // at this point we should have the closest right bracket not embedded in a quote
            {
                tokenString = s.Substring(offset, rbracket - offset + 1);
                if (lquote > 0)    // key/val pair
                {
                    int keyend = s.IndexOf(' ', offset);
                    key = s.Substring(offset + 1, keyend - offset - 1);
                    value = s.Substring(lquote + 1, rquote - lquote - 1);
                }
                else
                {
                    key = "";
                    value = tokenString;
                }
            }
            else
            {
                // error condition
                tokenType = PGNTokenType.Invalid;
                key = value = "";
            }

        }
    }
    public class PGNMoveNumber : PGNToken
    {
        public int value;

        public PGNMoveNumber(string s)
        {
            tokenType = PGNTokenType.MoveNumber;
            tokenString = s;
            value = Convert.ToInt32(s);
        }
        public PGNMoveNumber(string s, int offset)
        {
            tokenType = PGNTokenType.MoveNumber;
            int nbrEnd = s.IndexOf('.', offset);
            if (nbrEnd < 0 || nbrEnd - offset > 5)
                tokenType = PGNTokenType.Invalid;
            else
            {
                startLocation = offset;
                value = Convert.ToInt32(s.Substring(offset, nbrEnd - offset));
                tokenString = s.Substring(offset, nbrEnd - offset + 1);
            }
        }
    }
    public class PGNMoveString : PGNToken
    {
        public string value;
        public bool isCheck;
        public bool isMate;
        public string annotation;
        public int NAG;
        public List<List<PGNToken>> variations;

        public PGNMoveString(string s)
        {
            variations = new List<List<PGNToken>>();
            isCheck = isMate = false;
            tokenType = PGNTokenType.MoveString;
            tokenString = s;
            char lastChar = (s.Length > 0 ? s[s.Length - 1] : '\0');
            if ((isCheck = (lastChar == '+')) || (isMate = (lastChar == '#')))
                s = s.Substring(0, s.Length - 1);
            value = s;
            annotation = "";
            NAG = -1;
        }
        public PGNMoveString(string s, int offset)
        {
            isCheck = isMate = false;
            annotation = "";
            NAG = -1;
            tokenType = PGNTokenType.MoveString;
            int end = offset + 1;
            for (; end < s.Length; end++)
                if (Char.IsWhiteSpace(s[end]))
                    break;

            value = s.Substring(offset, end - offset);
            startLocation = offset;
            tokenString = s.Substring(offset, end - offset + 1);
            annotation = "";
            NAG = -1;
        }
    }

    public class PGNComment : PGNToken
    {
        public string value;

        public PGNComment(string s)
        {
            // first character determines if it's a whole line comment ';' or an inline one {}
            tokenType = PGNTokenType.Comment;
            tokenString = s;
        }
        public PGNComment(string s, int offset)
        {
            startLocation = offset;
            tokenType = PGNTokenType.Comment;
            if (s[offset] == ';')  // whole line comment
            {
                int eol = s.IndexOf(Environment.NewLine, offset);
                if (eol < 0)
                    tokenType = PGNTokenType.Invalid;
                else
                {
                    tokenString = s.Substring(offset, eol - offset);
                    value = s.Substring(offset + 1, eol - offset - 1);   // trim off the ','
                }
            }
            if (s[offset] == '(')  // escaped variation
            {
                int eol = s.IndexOf(')', offset);
                if (eol < 0)
                    tokenType = PGNTokenType.Invalid;
                else
                {
                    tokenString = s.Substring(offset, eol - offset + 1);
                    value = s.Substring(offset + 1, eol - offset - 1);   // trim off the '()'
                }
            }
            if (s[offset] == '{')  // escaped comment
            {
                int eol = s.IndexOf('}', offset);
                if (eol < 0)
                    tokenType = PGNTokenType.Invalid;
                else
                {
                    tokenString = s.Substring(offset, eol - offset + 1);
                    value = s.Substring(offset + 1, eol - offset - 1);   // trim off the '()'
                }
            }
        }
    }

    public class PGNTerminator : PGNToken
    {
        public static List<string> terminators = new List<string>() { "1-0", "0-1", "1/2-1/2", "*" };
        public PGNTerminator(string s)
        {
            tokenType = PGNTokenType.Terminator;
            tokenString = s;
        }
        public PGNTerminator(string s, int offset)
        {
            tokenType = PGNTokenType.Invalid;
            foreach (string t in terminators)
                if (s.Substring(offset).IndexOf(t) == 0)  // ok this is a terminator
                {
                    startLocation = offset;
                    tokenType = PGNTokenType.Terminator;
                    tokenString = t;
                }
        }
    }

    public class PGNEscape : PGNToken
    {
        public PGNEscape(string s)
        {
            tokenType = PGNTokenType.Escape;
            tokenString = s;
        }
    }
}
