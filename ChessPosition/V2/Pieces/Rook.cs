using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessPosition.V2.Pieces
{
    public class Rook : ChessPosition.V2.Piece
    {
        public Rook(PlayerEnum p)
            : base(p, Piece.PieceType.Rook)
        {
        }
        public override bool CouldMoveTo(Square source, Square dest, Dictionary<Square, Piece> board, Square epLoc, byte castleRights)
        {
            if (!base.CouldMoveTo(source, dest, board, epLoc, castleRights))
                return false;

            if (source.rank == dest.rank) // could be, is the path empty?
            {
                Square.File lo = (source.file < dest.file ? source.file : dest.file) + 1;
                Square.File hi = (source.file > dest.file ? source.file : dest.file) - 1;
                Square testSq = new Square(source.rank, lo);
                for (; lo <= hi; testSq.file = ++lo)
                    if (board.ContainsKey(testSq))
                        return false;
                return true;
            }
            else if (source.file == dest.file)  // could be, is the path empty?
            {
                Square.Rank lo = (source.rank < dest.rank ? source.rank : dest.rank) + 1;
                Square.Rank hi = (source.rank > dest.rank ? source.rank : dest.rank) - 1;
                Square testSq = new Square(lo, source.file);
                for (; lo <= hi; testSq.rank = ++lo)
                    if (board.ContainsKey(testSq))
                        return false;
                return true;
            }
            return false;
        }
    }
}
