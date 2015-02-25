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
        public Piece promo;

        public Ply()
        {
            src = null;
            dest = null;
            promo = null;
        }
        public Ply(Square s, Square d)
        {
            src = new Square(s);
            dest = new Square(d);
            promo = null;
        }
        public Ply(Square s, Square d, Piece p)
        {
            src = new Square(s);
            dest = new Square(d);
            promo = p;
        }
    }
}
