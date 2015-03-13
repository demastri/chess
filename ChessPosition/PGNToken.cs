using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessPosition
{
    public enum PGNTokenType { Tag, MoveNumber, MoveString, Comment, Terminator, Invalid };
    public class PGNToken
    {
        public PGNTokenType tokenType;
        public string tokenString;
        public int startLocation;
    }
    public class PGNTag : PGNToken
    {
        public string key;
        public string value;

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

        public PGNMoveString(string s, int offset)
        {
            tokenType = PGNTokenType.MoveString;
            int end = offset + 1;
            for (; end < s.Length; end++)
                if (Char.IsWhiteSpace(s[end]))
                    break;

            value = s.Substring(offset, end - offset);
            startLocation = offset;
            tokenString = s.Substring(offset, end - offset + 1);
        }
    }
    public class PGNComment : PGNToken
    {
        string value;

        public PGNComment(string s, int offset)
        {
            startLocation = offset;
            tokenType = PGNTokenType.Comment;
            if (s[offset] == ',')  // whole line comment
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
            if (s[offset] == '(')  // escaped comment
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
            if (s[offset] == '{')  // escaped annotation
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
}
