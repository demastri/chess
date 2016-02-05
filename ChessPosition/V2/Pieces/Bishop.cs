using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessPosition.V2.Pieces
{
    public class Bishop : ChessPosition.V2.Piece
    {
        public Bishop(PlayerEnum p)
            : base(p, Piece.PieceType.Bishop)
        {
        }
        public override bool CouldMoveTo(Square source, Square dest, Dictionary<Square, Piece> board, Square epLoc, byte castleRights)
        {
            if (!base.CouldMoveTo(source, dest, board, epLoc, castleRights))
                return false;
            if (Math.Abs(source.rank - dest.rank) == Math.Abs(source.file - dest.file)) // could be, is the path empty?
            {
                int delMax = Math.Abs(source.rank - dest.rank);
                int rInc = source.rank < dest.rank ? 1 : -1;
                int cInc = source.file < dest.file ? 1 : -1;

                for (int sDelta = 1; sDelta < delMax; sDelta++)
                    if (board.ContainsKey(new Square(source.rank + sDelta * rInc, source.file + sDelta * cInc)))
                        return false;
                return true;
            }
            return false;
        }
    }
}
