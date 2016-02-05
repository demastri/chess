using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessPosition.V2.Pieces
{
    public class King : ChessPosition.V2.Piece
    {
        public King(PlayerEnum p)
            : base(p, Piece.PieceType.King)
        {
        }
        public override bool CouldMoveTo(Square source, Square dest, Dictionary<Square, Piece> board, Square epLoc, byte castleRights)
        {
            if (!base.CouldMoveTo(source, dest, board, epLoc, castleRights))
                return false;

            if (Math.Abs(source.rank - dest.rank) <= 1 && Math.Abs(source.file - dest.file) <= 1) // could be, is the path empty?
                return true;
            // castle moves
            // if the right is there, and there are no pieces intervening
            // if castlerights exist, the K and R for that side have to be in the right spot...
            if (color == PlayerEnum.White && (castleRights & (byte)Position.CastleRights.KS_White) != 0 && dest == new Square(Square.Rank.R1, Square.File.FG))
            {
                if (!board.ContainsKey(new Square(Square.Rank.R1, Square.File.FF))
                    && !board.ContainsKey(new Square(Square.Rank.R1, Square.File.FG)))
                    return true;
            }
            if (color == PlayerEnum.White && (castleRights & (byte)Position.CastleRights.QS_White) != 0 && dest == new Square(Square.Rank.R1, Square.File.FC))
            {
                if (!board.ContainsKey(new Square(Square.Rank.R1, Square.File.FD))
                    && !board.ContainsKey(new Square(Square.Rank.R1, Square.File.FC))
                    && !board.ContainsKey(new Square(Square.Rank.R1, Square.File.FB)))
                    return true;
            }
            if (color == PlayerEnum.Black && (castleRights & (byte)Position.CastleRights.KS_Black) != 0 && dest == new Square(Square.Rank.R8, Square.File.FG))
            {
                if (!board.ContainsKey(new Square(Square.Rank.R8, Square.File.FF))
                    && !board.ContainsKey(new Square(Square.Rank.R8, Square.File.FG)))
                    return true;
            }
            if (color == PlayerEnum.Black && (castleRights & (byte)Position.CastleRights.QS_Black) != 0 && dest == new Square(Square.Rank.R8, Square.File.FC))
            {
                if (!board.ContainsKey(new Square(Square.Rank.R8, Square.File.FD))
                    && !board.ContainsKey(new Square(Square.Rank.R8, Square.File.FC))
                    && !board.ContainsKey(new Square(Square.Rank.R8, Square.File.FB)))
                    return true;
            }

            return false;
        }
    }
}
