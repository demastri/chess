using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessPosition.V2.Pieces
{
    public class Queen : ChessPosition.V2.Piece
    {
        public Queen(PlayerEnum p)
            : base(p, Piece.PieceType.Queen)
        {
        }
        public override bool CouldMoveTo(Square source, Square dest, Dictionary<Square, Piece> board, Square epLoc, byte castleRights)
        {
            if (!base.CouldMoveTo(source, dest, board, epLoc, castleRights))
                return false;

            return Piece.PieceFactory(color, Piece.PieceType.Rook).CouldMoveTo(source, dest, board, epLoc, castleRights) ||
                Piece.PieceFactory(color, Piece.PieceType.Bishop).CouldMoveTo(source, dest, board, epLoc, castleRights);
        }
    }
}
