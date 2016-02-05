using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessPosition.V2.Pieces
{
    public class Knight : ChessPosition.V2.Piece
    {
        public Knight(PlayerEnum p)
            : base(p, Piece.PieceType.Knight)
        {
        }
        public override bool CouldMoveTo(Square source, Square dest, Dictionary<Square, Piece> board, Square epLoc, byte castleRights)
        {
            if (!base.CouldMoveTo(source, dest, board, epLoc, castleRights))
                return false;

            return (Math.Abs(source.rank - dest.rank) + Math.Abs(source.file - dest.file) == 3
                && source.rank != dest.rank && source.file != dest.file);
        }
    }
}
