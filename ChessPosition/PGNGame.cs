using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ChessPosition
{
    public class PGNGame
    {
        public enum TokenType {Tag, MoveNumber, MoveString, Comment, Terminator, Invalid};
        public class PGNToken
        {
            public TokenType tokenType;
            public string tokenString;
            public int startLocation;
        }
        public class Tag : PGNToken
        {
            public string key;
            public string value;

            public Tag(string s, int offset)
            {
                startLocation = offset;
                tokenType = TokenType.Tag;
                // only two things will interfere here, embedded quoted values and a close ']'
                int lquote = s.IndexOf('\"', offset);
                int rquote = (lquote < 0 ? -1 : s.IndexOf('\"', lquote+1));
                int rbracket = s.IndexOf(']', offset);
                if( rbracket < lquote )     // no quotes in _this_ bracket set
                    lquote = rquote = -1;
                if( rquote > 0 && rbracket<rquote) // current bracket is quoted
                    rbracket = s.IndexOf(']', rquote);

                if( rbracket >= 0 ) // at this point we should have the closest right bracket not embedded in a quote
                {
                    tokenString = s.Substring(offset, rbracket-offset+1);
                    if( lquote > 0 )    // key/val pair
                    {
                        int keyend = s.IndexOf(' ', offset );
                        key = s.Substring(offset+1, keyend-offset-1);
                        value = s.Substring(lquote+1, rquote-lquote-1);
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
                    tokenType = TokenType.Invalid;
                    key = value = "";
                }

            }
        }
        public class MoveNumber : PGNToken
        {
            int value;

            public MoveNumber(string s, int offset)
            {
                tokenType = TokenType.MoveNumber;
                int nbrEnd = s.IndexOf('.', offset);
                if (nbrEnd < 0 || nbrEnd - offset > 5)
                    tokenType = TokenType.Invalid;
                else
                {
                    startLocation = offset;
                    value = Convert.ToInt32(s.Substring(offset, nbrEnd - offset));
                    tokenString = s.Substring(offset, nbrEnd - offset+1);
                }
            }
        }
        public class MoveString : PGNToken
        {
            public string value;

            public MoveString(string s, int offset)
            {
                tokenType = TokenType.MoveString;
                int end = offset + 1;
                for ( ; end < s.Length; end++)
                    if( Char.IsWhiteSpace(s[end]) )
                        break;

                value = s.Substring(offset, end - offset);
                startLocation = offset;
                tokenString = s.Substring(offset, end - offset+1);
            }
        }
        public class Comment : PGNToken
        {
            string value;

            public Comment(string s, int offset)
            {
                startLocation = offset;
                tokenType = TokenType.Comment;
                if (s[offset] == ',')  // whole line comment
                {
                    int eol = s.IndexOf(Environment.NewLine, offset);
                    if (eol < 0)
                        tokenType = TokenType.Invalid;
                    else
                    {
                        tokenString = s.Substring(offset, eol - offset);
                        value = s.Substring(offset+1, eol-offset-1);   // trim off the ','
                    }
                }
                if (s[offset] == '(')  // escaped comment
                {
                    int eol = s.IndexOf(')', offset);
                    if (eol < 0)
                        tokenType = TokenType.Invalid;
                    else
                    {
                        tokenString = s.Substring(offset, eol - offset+1);
                        value = s.Substring(offset+1, eol-offset-1);   // trim off the '()'
                    }
                }
                if (s[offset] == '{')  // escaped annotation
                {
                    int eol = s.IndexOf('}', offset);
                    if (eol < 0)
                        tokenType = TokenType.Invalid;
                    else
                    {
                        tokenString = s.Substring(offset, eol - offset+1);
                        value = s.Substring(offset+1, eol-offset-1);   // trim off the '()'
                    }
                }
            }
        }
        
        private static List<string> terminators = new List<string>() { "1-0", "0-1", "1/2-1/2", "*" };
        public class Terminator : PGNToken
        {
            public Terminator(string s, int offset)
            {
                tokenType = TokenType.Invalid;
                foreach( string t in terminators )
                    if (s.Substring(offset).IndexOf(t) == 0)  // ok this is a terminator
                    {
                        startLocation = offset;
                        tokenType = TokenType.Terminator;
                        tokenString = t;
                    }
            }
        }
        
        public string pgn;
        public List<PGNToken> tokens;
        public PGNGame(StreamReader sr)
        {
            // read and add to pgn until you see a terminator...### could be embedded
            pgn = "";
            bool done = false;
            while (!done && !sr.EndOfStream)
            {
                string line = sr.ReadLine();
                if (line.Trim() != "")
                {
                    pgn += line + " " + Environment.NewLine;
                    foreach (string s in terminators)
                        if (line.IndexOf(s) >= 0 && line.Substring(line.IndexOf(s) + s.Length).Trim() == "")
                            done = true;
                }
            }
            Tokenize();
        }
        public PGNGame(string s)
        {
            pgn = s;
            Tokenize();
        }
        public void Tokenize()
        {
            tokens = new List<PGNToken>();
            for( int i=0; i<pgn.Length; )
            {
                if (char.IsWhiteSpace(pgn[i]))
                    i++;
                else
                {
                    int refi = i;
                    PGNToken pgntoken = BuildToken( ref i );
                    if (pgntoken != null)
                    {
                        tokens.Add(pgntoken);
                    }
                    if (i < refi)
                        i = refi;
                }
            }
        }
        public PGNToken BuildToken( ref int i ) 
        {
            PGNToken outToken = null;
            switch (pgn[i])
            {
                case '[':   // should be a tag value
                    outToken = new Tag(pgn, i);
                    break;
                case '{':   // should be an annotation
                    outToken = new Comment(pgn, i);
                    break;
                case ',':   // should be a comment
                    outToken = new Comment(pgn, i);
                    break;
                case '(':   // should be a comment
                    outToken = new Comment(pgn, i);
                    break;
                default:   // should be a move number or string...
                    outToken = new Terminator(pgn, i);
                    if( outToken.tokenType == TokenType.Invalid )
                        if( char.IsDigit(pgn[i]) )
                            outToken = new MoveNumber(pgn, i);
                        else if (("PRNBKQabcdefghO").IndexOf(pgn[i]) >= 0)
                            outToken = new MoveString(pgn, i);
                    break;
            }
            if (outToken == null || outToken.tokenType == TokenType.Invalid)
                return null;
            i = outToken.startLocation + outToken.tokenString.Length;
            return outToken;
        }

    }
}
