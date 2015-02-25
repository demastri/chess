using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessPosition
{
    public class Position
    {
        public enum CastleRights { KS_White = 0x01, QS_White = 0x02, KS_Black = 0x04, QS_Black = 0x08 };
        public static Position StartPosition = new Position();
        public PositionHash Hash { get { return new PositionHash(this); } }

        // these values are included in the hash
        public Dictionary<Square, Piece> board;
        public byte castleRights;   // bits 0-4 -> WK WQ BK BQ
        public Square epLoc;        // the ep col is the file, for convenience the row is the square "behind" the P (the capture sq)

        // these values are NOT included in the hash
        Dictionary<PositionHash, int> repetitions;
        int progressCounter;

        public Position()
        {
            board = new Dictionary<Square, Piece>();
            castleRights = 0x0f;
            epLoc = new Square(0x0f, 0x0f);
            repetitions = new Dictionary<PositionHash, int>();
            progressCounter = 0;
            board.Add(new Square(Square.Rank.R1, Square.File.FA), new Piece(PlayerEnum.White, Piece.PieceType.Rook));
            board.Add(new Square(Square.Rank.R1, Square.File.FB), new Piece(PlayerEnum.White, Piece.PieceType.Knight));
            board.Add(new Square(Square.Rank.R1, Square.File.FC), new Piece(PlayerEnum.White, Piece.PieceType.Bishop));
            board.Add(new Square(Square.Rank.R1, Square.File.FD), new Piece(PlayerEnum.White, Piece.PieceType.Queen));
            board.Add(new Square(Square.Rank.R1, Square.File.FE), new Piece(PlayerEnum.White, Piece.PieceType.King));
            board.Add(new Square(Square.Rank.R1, Square.File.FF), new Piece(PlayerEnum.White, Piece.PieceType.Bishop));
            board.Add(new Square(Square.Rank.R1, Square.File.FG), new Piece(PlayerEnum.White, Piece.PieceType.Knight));
            board.Add(new Square(Square.Rank.R1, Square.File.FH), new Piece(PlayerEnum.White, Piece.PieceType.Rook));
            board.Add(new Square(Square.Rank.R2, Square.File.FA), new Piece(PlayerEnum.White, Piece.PieceType.Pawn));
            board.Add(new Square(Square.Rank.R2, Square.File.FB), new Piece(PlayerEnum.White, Piece.PieceType.Pawn));
            board.Add(new Square(Square.Rank.R2, Square.File.FC), new Piece(PlayerEnum.White, Piece.PieceType.Pawn));
            board.Add(new Square(Square.Rank.R2, Square.File.FD), new Piece(PlayerEnum.White, Piece.PieceType.Pawn));
            board.Add(new Square(Square.Rank.R2, Square.File.FE), new Piece(PlayerEnum.White, Piece.PieceType.Pawn));
            board.Add(new Square(Square.Rank.R2, Square.File.FF), new Piece(PlayerEnum.White, Piece.PieceType.Pawn));
            board.Add(new Square(Square.Rank.R2, Square.File.FG), new Piece(PlayerEnum.White, Piece.PieceType.Pawn));
            board.Add(new Square(Square.Rank.R2, Square.File.FH), new Piece(PlayerEnum.White, Piece.PieceType.Pawn));
            board.Add(new Square(Square.Rank.R8, Square.File.FA), new Piece(PlayerEnum.Black, Piece.PieceType.Rook));
            board.Add(new Square(Square.Rank.R8, Square.File.FB), new Piece(PlayerEnum.Black, Piece.PieceType.Knight));
            board.Add(new Square(Square.Rank.R8, Square.File.FC), new Piece(PlayerEnum.Black, Piece.PieceType.Bishop));
            board.Add(new Square(Square.Rank.R8, Square.File.FD), new Piece(PlayerEnum.Black, Piece.PieceType.Queen));
            board.Add(new Square(Square.Rank.R8, Square.File.FE), new Piece(PlayerEnum.Black, Piece.PieceType.King));
            board.Add(new Square(Square.Rank.R8, Square.File.FF), new Piece(PlayerEnum.Black, Piece.PieceType.Bishop));
            board.Add(new Square(Square.Rank.R8, Square.File.FG), new Piece(PlayerEnum.Black, Piece.PieceType.Knight));
            board.Add(new Square(Square.Rank.R8, Square.File.FH), new Piece(PlayerEnum.Black, Piece.PieceType.Rook));
            board.Add(new Square(Square.Rank.R7, Square.File.FA), new Piece(PlayerEnum.Black, Piece.PieceType.Pawn));
            board.Add(new Square(Square.Rank.R7, Square.File.FB), new Piece(PlayerEnum.Black, Piece.PieceType.Pawn));
            board.Add(new Square(Square.Rank.R7, Square.File.FC), new Piece(PlayerEnum.Black, Piece.PieceType.Pawn));
            board.Add(new Square(Square.Rank.R7, Square.File.FD), new Piece(PlayerEnum.Black, Piece.PieceType.Pawn));
            board.Add(new Square(Square.Rank.R7, Square.File.FE), new Piece(PlayerEnum.Black, Piece.PieceType.Pawn));
            board.Add(new Square(Square.Rank.R7, Square.File.FF), new Piece(PlayerEnum.Black, Piece.PieceType.Pawn));
            board.Add(new Square(Square.Rank.R7, Square.File.FG), new Piece(PlayerEnum.Black, Piece.PieceType.Pawn));
            board.Add(new Square(Square.Rank.R7, Square.File.FH), new Piece(PlayerEnum.Black, Piece.PieceType.Pawn));
        }
        public Position(Position p) // copy-con
        {
            Init(p);
        }
        public Position(PositionHash ph)
        {
            Init(ph.Rehydrate());
        }

        private void Init(Position p)
        {
            board = new Dictionary<Square, Piece>(p.board);
            epLoc = p.epLoc;
            castleRights = p.castleRights;
            repetitions = new Dictionary<PositionHash, int>(p.repetitions);
            progressCounter = p.progressCounter;
        }
        public List<Square> FindPieceWithTarget(Piece p, Square s, Square.Rank rowConstraint, Square.File colConstraint)
        {
            List<Square> options = new List<Square>();
            foreach (Square ss in board.Keys)
            {
                if (board[ss] == p && CouldMoveThere(ss, s, p))
                {
                    if (((byte)rowConstraint == 0x0f || ss.row == (byte)rowConstraint) &&
                        ((byte)colConstraint == 0x0f || ss.col == (byte)colConstraint))
                        options.Add(ss);
                }
            }
            return options;
        }
        private bool CouldMoveThere(Square source, Square dest, Piece pc)
        {
            if (source == dest) // can't move to the same square
                return false;
            if (board.ContainsKey(dest) && board[dest].color == pc.color)   // or to a square with your piece
                return false;
            switch (pc.piece)
            {
                case Piece.PieceType.King:
                    if (Math.Abs(source.row - dest.row) <= 1 && Math.Abs(source.col - dest.col) <= 1) // could be, is the path empty?
                        return true;
                    // castle moves
                    // if the right is there, and there are no pieces intervening
                    // if castlerights exist, the K and R for that side have to be in the right spot...
                    if (pc.color == PlayerEnum.White && (castleRights & (byte)CastleRights.KS_White) != 0)
                    {
                        if (!board.ContainsKey(new Square(Square.Rank.R1, Square.File.FF))
                            && !board.ContainsKey(new Square(Square.Rank.R1, Square.File.FG)))
                            return true;
                    }
                    if (pc.color == PlayerEnum.White && (castleRights & (byte)CastleRights.QS_White) != 0)
                    {
                        if (!board.ContainsKey(new Square(Square.Rank.R1, Square.File.FD))
                            && !board.ContainsKey(new Square(Square.Rank.R1, Square.File.FC)))
                            return true;
                    }
                    if (pc.color == PlayerEnum.Black && (castleRights & (byte)CastleRights.KS_Black) != 0)
                    {
                        if (!board.ContainsKey(new Square(Square.Rank.R8, Square.File.FF))
                            && !board.ContainsKey(new Square(Square.Rank.R8, Square.File.FG)))
                            return true;
                    }
                    if (pc.color == PlayerEnum.Black && (castleRights & (byte)CastleRights.QS_Black) != 0)
                    {
                        if (!board.ContainsKey(new Square(Square.Rank.R8, Square.File.FD))
                            && !board.ContainsKey(new Square(Square.Rank.R8, Square.File.FC)))
                            return true;
                    }
                    break;
                case Piece.PieceType.Queen:
                    Piece testPc = new Piece(pc.color, Piece.PieceType.Rook);
                    return CouldMoveThere(source, dest, new Piece(pc.color, Piece.PieceType.Rook)) ||
                        CouldMoveThere(source, dest, new Piece(pc.color, Piece.PieceType.Bishop));
                    break;
                case Piece.PieceType.Rook:
                    if (source.row == dest.row) // could be, is the path empty?
                    {
                        int lo = source.col < dest.col ? source.col + 1 : dest.col + 1;
                        int hi = source.col > dest.col ? source.col - 1 : dest.col - 1;
                        Square testSq = new Square((Square.Rank)source.row, (Square.File)lo);
                        for (; lo < hi; testSq.col = (byte)++lo)
                            if (board.ContainsKey(testSq))
                                return false;
                        return true;
                    }
                    else if (source.col == dest.col)  // could be, is the path empty?
                    {
                        int lo = source.row < dest.row ? source.row + 1 : dest.row + 1;
                        int hi = source.row > dest.row ? source.row - 1 : dest.row - 1;
                        Square testSq = new Square((Square.Rank)lo, (Square.File)source.col);
                        for (; lo < hi; testSq.row = (byte)++lo)
                            if (board.ContainsKey(testSq))
                                return false;
                        return true;
                    }
                    break;
                case Piece.PieceType.Bishop:
                    if (Math.Abs(source.row - dest.row) == Math.Abs(source.col - dest.col)) // could be, is the path empty?
                    {
                        int delMax = Math.Abs(source.row - dest.row);
                        int rInc = source.row < dest.row ? 1 : -1;
                        int cInc = source.col < dest.col ? 1 : -1;
                        Square testSq = new Square(source.row, source.col);
                        for (int sDelta = 1; sDelta < delMax; sDelta++)
                            if (board.ContainsKey(new Square((Square.Rank)(source.row + sDelta * rInc), (Square.File)(source.col + sDelta * cInc))))
                                return false;
                        return true;
                    }
                    break;
                case Piece.PieceType.Knight:
                    if (Math.Abs(source.row - dest.row) + Math.Abs(source.col - dest.col) == 3
                        && source.row != dest.row && source.col != dest.col) // could be, is the path empty?
                    {
                        return true;
                    }
                    break;
                case Piece.PieceType.Pawn:
                    int moveDir = pc.color == PlayerEnum.White ? 1 : -1;
                    int startRow = pc.color == PlayerEnum.White ? 1 : 6;
                    // 2xmoves
                    if (source.col == dest.col && source.row + 2 * moveDir == dest.row && source.row == startRow
                        && !board.ContainsKey(new Square((Square.Rank)(source.row + moveDir), (Square.File)(source.col)))
                        && !board.ContainsKey(new Square((Square.Rank)(source.row + 2 * moveDir), (Square.File)(source.col))))
                        return true;
                    // 1xmoves
                    if (source.col == dest.col && source.row + moveDir == dest.row
                        && !board.ContainsKey(new Square((Square.Rank)(source.row + moveDir), (Square.File)(source.col))))
                        return true;
                    // straight capture
                    if (source.row + moveDir == dest.row
                        && (source.col + 1 == dest.col || source.col - 1 == dest.col)
                        && board.ContainsKey(dest) && board[dest].color != pc.color)
                        return true;
                    // ep capture - ep row is set, capture square is one "behind" the advance square
                    // from the capturing Ps perspective, it's a normal capture, 
                    // just that the captured P is one "ahead" of the square
                    Square epTargetSq = new Square((Square.Rank)(source.row), (Square.File)(epLoc.col));
                    if (epLoc.col != 0x0f && source.row + moveDir == dest.row
                        && (source.col + 1 == dest.col || source.col - 1 == dest.col)
                        && dest == epLoc 
                        && !board.ContainsKey(epLoc)
                        && board.ContainsKey(epTargetSq) && board[dest].color != pc.color && board[dest].piece == Piece.PieceType.Pawn)
                        return true;
                    break;
            }
            return false;
        }
    }
}
