using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessPosition.V2
{
    public class Comment
    {
        public bool isToEOL;
        public string value;

        public Comment(bool toEOL, string s)
        {
            isToEOL = toEOL;
            value = s;
        }
    }
}
