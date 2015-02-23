using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessPosition
{
    public class Ply
    {
        public int Number;
        public int MoveNumber { get { return Number / 2 + 1; } }


        public Square src;
        public Square dest;

        public Ply( )
        {
        }
    }
}
