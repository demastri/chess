using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ChessPosition
{
    public class PGNTokenizer
    {
        private static List<string> terminators = new List<string>() { "1-0", "0-1", "1/2-1/2", "*" };
        public class Terminator : PGNToken
        {
            public Terminator(string s, int offset)
            {
                tokenType = PGNTokenType.Invalid;
                foreach( string t in terminators )
                    if (s.Substring(offset).IndexOf(t) == 0)  // ok this is a terminator
                    {
                        startLocation = offset;
                        tokenType = PGNTokenType.Terminator;
                        tokenString = t;
                    }
            }
        }
        
        public string pgn;
        public List<PGNToken> tokens;
        public PGNTokenizer(StreamReader sr)
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
        public PGNTokenizer(string s)
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
                    outToken = new PGNTag(pgn, i);
                    break;
                case '{':   // should be an annotation
                    outToken = new PGNComment(pgn, i);
                    break;
                case ',':   // should be a comment
                    outToken = new PGNComment(pgn, i);
                    break;
                case '(':   // should be a comment
                    outToken = new PGNComment(pgn, i);
                    break;
                default:   // should be a move number or string...
                    outToken = new Terminator(pgn, i);
                    if( outToken.tokenType == PGNTokenType.Invalid )
                        if( char.IsDigit(pgn[i]) )
                            outToken = new PGNMoveNumber(pgn, i);
                        else if (("PRNBKQabcdefghO").IndexOf(pgn[i]) >= 0)
                            outToken = new PGNMoveString(pgn, i);
                    break;
            }
            if (outToken == null || outToken.tokenType == PGNTokenType.Invalid)
                return null;
            i = outToken.startLocation + outToken.tokenString.Length;
            return outToken;
        }

    }
}
