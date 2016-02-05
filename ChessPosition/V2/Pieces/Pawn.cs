using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessPosition.V2.Pieces
{
    public class Pawn : ChessPosition.V2.Piece
    {
        public Pawn(PlayerEnum p)
            : base(p, Piece.PieceType.Pawn)
        {
        }
        public override bool CouldMoveTo(Square source, Square dest, Dictionary<Square, Piece> board, Square epLoc, byte castleRights)
        {
            if (!base.CouldMoveTo(source, dest, board, epLoc, castleRights))
                return false;

            int moveDir = color == PlayerEnum.White ? 1 : -1;
            Square.Rank startRow = color == PlayerEnum.White ? Square.Rank.R2 : Square.Rank.R7;
            // 2xmoves
            if (source.file == dest.file && source.rank + 2 * moveDir == dest.rank && source.rank == startRow
                && !board.ContainsKey(new Square((Square.Rank)(source.rank + moveDir), (Square.File)(source.file)))
                && !board.ContainsKey(new Square((Square.Rank)(source.rank + 2 * moveDir), (Square.File)(source.file))))
                return true;
            // 1xmoves
            if (source.file == dest.file && source.rank + moveDir == dest.rank
                && !board.ContainsKey(new Square((Square.Rank)(source.rank + moveDir), (Square.File)(source.file))))
                return true;
            // straight capture
            if (source.rank + moveDir == dest.rank
                && (source.file + 1 == dest.file || source.file - 1 == dest.file)
                && board.ContainsKey(dest) && board[dest].color != color)
                return true;
            // ep capture - ep row is set, capture square is one "behind" the advance square
            // from the capturing Ps perspective, it's a normal capture, 
            // just that the captured P is one "ahead" of the square
            Square epTargetSq = new Square((Square.Rank)(source.rank), (Square.File)(epLoc.file));
            if (epLoc.file != Square.File.NONE && source.rank + moveDir == dest.rank
                && (source.file + 1 == dest.file || source.file - 1 == dest.file)
                && dest == epLoc
                && !board.ContainsKey(epLoc)
                && board.ContainsKey(epTargetSq) && board[epTargetSq].color != color && board[epTargetSq].piece == Piece.PieceType.Pawn)
                return true;

            return false;
        }
    }
}
