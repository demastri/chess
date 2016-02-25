using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JPD.Parser;

namespace ChessPosition.V2.PGN
{
    public enum PGNTokenType { Tag, MoveNumber, MoveString, Comment, Terminator, Escape, Invalid }; // ### should add support for variations, also
    public class PGNToken
    {
        public PGNTokenType tokenType;
        public string tokenString;
        public int startLocation;
        public string value;

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
                                            List<PGNToken> curVar = TokenFactory(game, moveImpl.kids);    // ###
                                            curMove.variations.Add(curVar); // ### -> this has to make it back up to the ply, not just here...
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
    }

    public class PGNTag : PGNToken
    {
        public string key;
        public string value;

        static public Dictionary<string, string> ToDict(List<PGNTag> tags)
        {
            return tags.ToDictionary(g => g.key, g => g.value);
        }

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
        public override string ToString()
        {
            return "[" + key + ", " + value + "]";
        }
    }
    public class PGNMoveNumber : PGNToken
    {
        new public int value;

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
        public override string ToString()
        {
            return "[" + value+ ".]";
        }
    }
    public class PGNMoveString : PGNToken
    {
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
        public override string ToString()
        {
            return "[" + value + "]";
        }
    }

    public class PGNComment : PGNToken
    {
        public bool isBraceComment;

        public PGNComment(string s)
        {
            // first character determines if it's a whole line comment ';' or an inline one {}
            tokenType = PGNTokenType.Comment;
            tokenString = s;
            if (s[0] == ';')   // whole line comment
            {
                isBraceComment = false;
                value = s.Substring(1);
            }
            if (s[0] == '{' && s[s.Length - 1] == '}')   // embedded comment
            {
                isBraceComment = true;
                value = s.Substring(1, s.Length - 2);
            }

        }
        public override string ToString()
        {
            return "{" + value + "}";
        }
    }

    public class PGNTerminator : PGNToken
    {
        public Game.Terminators terminatorType { get { return (Game.Terminators)Game.terminators.IndexOf(value); } }
        public PGNTerminator(string s)
        {
            tokenType = PGNTokenType.Terminator;
            tokenString = value = s;
        }
        public override string ToString()
        {
            return "<" + value + ">";
        }

    }

    public class PGNEscape : PGNToken
    {
        public PGNEscape(string s)
        {
            tokenType = PGNTokenType.Escape;
            tokenString = s;
            value = s.Substring(1); // split off the escape character
        }
    }
}
