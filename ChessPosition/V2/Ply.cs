using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessPosition.V2
{
    public class Ply
    {
        #region operator Overrides
        /// none
        #endregion

        #region enums and static definitions
        /// none
        #endregion

        #region constructors (empty, src / dest squares, w/wo promo piece)

        public Ply()
        {
            BaseInit();
        }

        public Ply(Square srcSq, Square destSq)
        {
            BaseInit();

            src = new Square(srcSq);
            dest = new Square(destSq);
        }
        public Ply(Square srcSq, Square destSq, Piece promoPc)
        {
            BaseInit();

            src = new Square(srcSq);
            dest = new Square(destSq);
            promo = Piece.PieceFactory(promoPc.color, promoPc.piece);
        }

        private void BaseInit()
        {
            src = null;
            dest = null;
            promo = null;
            comments = new List<Comment>();
            variations = new List<List<Ply>>();
        }

        #endregion

        #region properties

        public int Number { get; set; } // zero based, ply 0 = first W move
        public int MoveNumber { get { return (Number - 1) / 2 + 1; } }

        public Square src { get; set; }
        public Square dest { get; set; }
        public Piece promo { get; set; }

        public List<Comment> comments { get; set; }
        public List<List<Ply>> variations { get; set; }
        
        #endregion

        #region string/char conversions
        /// none
        #endregion

        #region domain logic
        /// none
        #endregion
    
    }
}
