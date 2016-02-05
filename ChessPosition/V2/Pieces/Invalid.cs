using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessPosition.V2.Pieces
{
    public class Invalid : ChessPosition.V2.Piece
    {
        public Invalid()
            : base(PlayerEnum.Unknown, Piece.PieceType.Invalid)
        {
        }
        public override bool CouldMoveTo(Square source, Square dest, Dictionary<Square, Piece> board, Square epLoc, byte castleRights)
        {
            return false;
        }
    }
}
