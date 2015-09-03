using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JPD.Utilities;

namespace ChessPosition
{
    public class Position
    {
        public enum CastleRights { KS_White = 0x01, QS_White = 0x02, KS_Black = 0x04, QS_Black = 0x08 };
        public static Position StartPosition = new Position();
        public PositionHash Hash { get { return new PositionHash(this); } }

        // these values are included in the hash
        public PlayerEnum onMove;
        public Dictionary<Square, Piece> board;
        public byte castleRights;   // bits 0-4 -> WK WQ BK BQ
        public Square epLoc;        // the ep col is the file, for convenience the row is the square "behind" the P (the capture sq)


        public Position()
        {
            board = new Dictionary<Square, Piece>();
            castleRights = 0x0f;
            epLoc = new Square(Square.Rank.NONE, Square.File.NONE);
            onMove = PlayerEnum.White;

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
        public Position(string fenString)
        {
            board = new Dictionary<Square, Piece>();

            int curFenStringIndex = 0;
            char thisChar;

            for (int rank = 7; rank >= 0; rank--)
            {
                byte file = 0;
                while (file < 8)
                {
                    thisChar = fenString[curFenStringIndex++];
                    if (Char.IsDigit(thisChar))
                        file += (byte)(thisChar - '0');
                    else
                        board.Add(new Square((byte)rank, file++), Piece.FromFENChar(thisChar));
                }
                // this had better be the / following the interior file definitions
                if (rank > 0)
                {
                    thisChar = fenString[curFenStringIndex++];
                    if (thisChar != '/')
                        Console.WriteLine("bad fen string...");
                }
            }

            // this had better be the space following the board definitions
            thisChar = fenString[curFenStringIndex++];
            if (thisChar != ' ')
                Console.WriteLine("bad fen string...");
            thisChar = fenString[curFenStringIndex++];
            onMove = (thisChar == 'w' ? PlayerEnum.White : PlayerEnum.Black);

            thisChar = fenString[curFenStringIndex++];
            if (thisChar != ' ')
                Console.WriteLine("bad fen string...");
            if ((thisChar = fenString[curFenStringIndex++]) == '-')
            {
                castleRights = 0x00;
                curFenStringIndex++;    // consume the space...
            }
            else do
                {
                    switch (thisChar)
                    {
                        case 'k': castleRights = (byte)(castleRights | (byte)CastleRights.KS_Black); break;
                        case 'q': castleRights = (byte)(castleRights | (byte)CastleRights.QS_Black); break;
                        case 'K': castleRights = (byte)(castleRights | (byte)CastleRights.KS_White); break;
                        case 'Q': castleRights = (byte)(castleRights | (byte)CastleRights.QS_White); break;
                    }
                } while ((thisChar = fenString[curFenStringIndex++]) != ' ');

            if ((thisChar = fenString[curFenStringIndex++]) == '-')
                epLoc = new Square(Square.Rank.NONE, Square.File.NONE);
            else
            {
                Square.File file = (Square.File)(thisChar - 'a');
                thisChar = fenString[curFenStringIndex++];
                Square.Rank rank = (Square.Rank)(thisChar - '1');
                epLoc = new Square(rank, file);
            }

            // since the porition representation doesn't strictly care about either halfmove click or fullmove number, we're done
            int halfmove = 0;
            while ((thisChar = fenString[++curFenStringIndex]) != ' ')
                halfmove = 10 * halfmove + (thisChar - '0');

            int movenbr = 0;
            while (++curFenStringIndex < fenString.Length)
            {
                thisChar = fenString[curFenStringIndex];
                movenbr = 10 * movenbr + (thisChar - '0');
            }

            if (ToFEN(halfmove, movenbr) != fenString)
                Console.WriteLine("bad fen string...");
        }

        private void Init(Position p)
        {
            board = new Dictionary<Square, Piece>(p.board);
            epLoc = new Square(p.epLoc);
            castleRights = p.castleRights;
            onMove = p.onMove;
        }

        public Piece PieceAt(Square s)
        {
            return board.Keys.Contains(s) ? board[s] : null;
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
                    {
                        Position possPos = new Position(this);
                        Ply testPly = new Ply(ss, s, p);    // i think promo piece is irrelevant here, if the piece is pinned, it doesn't matter what it promotes to...
                        possPos.MakeMove(testPly);
                        if (!possPos.IsCheck())
                            options.Add(ss);
                    }
                }
            }
            return options;
        }

        public bool IsCheck()  // is the onMove King being attacked??
        {
            // find the opposing king
            Square attackSq = null;
            foreach (Square s in board.Keys)
                if (board[s].piece == Piece.PieceType.King && board[s].color != onMove)    // got him
                {
                    attackSq = s;
                    break;
                }
            // see if any pieces can move there...
            foreach (Square s in board.Keys)
                if (board[s].color == onMove && CouldMoveThere(s, attackSq, board[s]))
                    return true;
            return false;
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
                    if (pc.color == PlayerEnum.White && (castleRights & (byte)CastleRights.KS_White) != 0 && dest == new Square(Square.Rank.R1, Square.File.FG))
                    {
                        if (!board.ContainsKey(new Square(Square.Rank.R1, Square.File.FF))
                            && !board.ContainsKey(new Square(Square.Rank.R1, Square.File.FG)))
                            return true;
                    }
                    if (pc.color == PlayerEnum.White && (castleRights & (byte)CastleRights.QS_White) != 0 && dest == new Square(Square.Rank.R1, Square.File.FC))
                    {
                        if (!board.ContainsKey(new Square(Square.Rank.R1, Square.File.FD))
                            && !board.ContainsKey(new Square(Square.Rank.R1, Square.File.FC))
                            && !board.ContainsKey(new Square(Square.Rank.R1, Square.File.FB)))
                            return true;
                    }
                    if (pc.color == PlayerEnum.Black && (castleRights & (byte)CastleRights.KS_Black) != 0 && dest == new Square(Square.Rank.R8, Square.File.FG))
                    {
                        if (!board.ContainsKey(new Square(Square.Rank.R8, Square.File.FF))
                            && !board.ContainsKey(new Square(Square.Rank.R8, Square.File.FG)))
                            return true;
                    }
                    if (pc.color == PlayerEnum.Black && (castleRights & (byte)CastleRights.QS_Black) != 0 && dest == new Square(Square.Rank.R8, Square.File.FC))
                    {
                        if (!board.ContainsKey(new Square(Square.Rank.R8, Square.File.FD))
                            && !board.ContainsKey(new Square(Square.Rank.R8, Square.File.FC))
                            && !board.ContainsKey(new Square(Square.Rank.R8, Square.File.FB)))
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
                        for (; lo <= hi; testSq.col = (byte)++lo)
                            if (board.ContainsKey(testSq))
                                return false;
                        return true;
                    }
                    else if (source.col == dest.col)  // could be, is the path empty?
                    {
                        int lo = source.row < dest.row ? source.row + 1 : dest.row + 1;
                        int hi = source.row > dest.row ? source.row - 1 : dest.row - 1;
                        Square testSq = new Square((Square.Rank)lo, (Square.File)source.col);
                        for (; lo <= hi; testSq.row = (byte)++lo)
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
                        && board.ContainsKey(epTargetSq) && board[epTargetSq].color != pc.color && board[epTargetSq].piece == Piece.PieceType.Pawn)
                        return true;
                    break;
            }
            return false;
        }

        public void MakeMove(Ply p)
        {
            /// advance game state - include castle rights, rep, ep and progress counters
            /// 
            /// in most cases this is simply move the piece from the src sq to the dest sq
            /// if there's a piece on the target square, it needs to be removed
            /// if the piece is a K or R, update castle rights (and castle if appropriate)
            /// if the piece is a P, check for both an ep capture on this move and update ep state - also promotion

            Piece srcPiece = board[p.src];
            Piece destPiece = board.ContainsKey(p.dest) ? board[p.dest] : null;

            if (onMove != srcPiece.color)
                System.Console.WriteLine("Moving the wrong color piece...");
            if (destPiece != null && onMove == destPiece.color)
                System.Console.WriteLine("Taking the wrong color piece...");

            board.Remove(p.src);
            board[p.dest] = srcPiece;

            if (srcPiece.piece == Piece.PieceType.King)
            {
                if (srcPiece.color == PlayerEnum.White)
                {
                    castleRights = (byte)(castleRights & ~(byte)Position.CastleRights.KS_White);
                    castleRights = (byte)(castleRights & ~(byte)Position.CastleRights.QS_White);
                }
                else
                {
                    castleRights = (byte)(castleRights & ~(byte)Position.CastleRights.KS_Black);
                    castleRights = (byte)(castleRights & ~(byte)Position.CastleRights.QS_Black);
                }
                if (Math.Abs(p.src.col - p.dest.col) == 2)  // castling...
                {
                    Square.Rank rank = (Square.Rank)p.src.row;
                    Square.File file = p.dest.col == (byte)Square.File.FC ? Square.File.FA : Square.File.FH;
                    Piece castleRook = null;
                    if (board.ContainsKey(new Square(rank, file)))
                    {
                        castleRook = board[new Square(rank, file)];
                        if (castleRook.color == srcPiece.color && castleRook.piece == Piece.PieceType.Rook)
                        {
                            board.Remove(new Square(rank, file));
                            board[new Square(rank, file == Square.File.FA ? Square.File.FD : Square.File.FF)] = castleRook;
                        }
                        else
                            System.Console.WriteLine("Moving the wrong color piece...");
                    }
                }
            }
            if (srcPiece.piece == Piece.PieceType.Rook) // clear castling rights as appropriate
            {
                Square.Rank rank = (Square.Rank)(onMove == PlayerEnum.White ? Square.Rank.R1 : Square.Rank.R8);
                if ((Square.Rank)p.src.row == rank)
                    if ((Square.File)p.src.col == Square.File.FA)
                        castleRights = (byte)(castleRights & ~(byte)
                            (onMove == PlayerEnum.White ? Position.CastleRights.QS_White : Position.CastleRights.QS_Black));
                    else if ((Square.File)p.src.col == Square.File.FH)
                        castleRights = (byte)(castleRights & ~(byte)
                            (onMove == PlayerEnum.White ? Position.CastleRights.KS_White : Position.CastleRights.KS_Black));

            }
            if (srcPiece.piece == Piece.PieceType.Pawn) // ep and promotion (everything else is just regular...)
            {
                if (p.promo != null)
                {
                    board.Remove(p.dest);
                    board[p.dest] = p.promo;
                }

                // handle this ep capture
                if (p.dest == epLoc)
                {
                    Square capSq = new Square((byte)(epLoc.row + (onMove == PlayerEnum.White ? -1 : 1)), epLoc.col);
                    if (!board.ContainsKey(capSq))
                        System.Console.WriteLine("Can't find the ep capture piece...");
                    board.Remove(capSq);
                }

                // telegraph any possible ep capture
                if (Math.Abs(p.src.row - p.dest.row) == 2)   // it advanced two
                {
                    epLoc.col = p.src.col;
                    epLoc.row = (byte)(p.dest.row + (onMove == PlayerEnum.White ? -1 : 1));
                }
                else
                {
                    epLoc.col = (byte)Square.File.NONE;
                    epLoc.row = (byte)Square.Rank.NONE;
                }
            }
            else
            {
                epLoc.col = (byte)Square.File.NONE;
                epLoc.row = (byte)Square.Rank.NONE;
            }

            onMove = (onMove == PlayerEnum.White ? PlayerEnum.Black : PlayerEnum.White);
        }

        public string ToFEN(int progress, int moveNbr)
        {
            string outString = "8/8/8/8/8/8/8/8 "
                + (onMove == PlayerEnum.White ? "w " : "b ")
                + (((castleRights & (byte)CastleRights.KS_White) != 0) ? "K" : "")
                + (((castleRights & (byte)CastleRights.QS_White) != 0) ? "Q" : "")
                + (((castleRights & (byte)CastleRights.KS_Black) != 0) ? "k" : "")
                + (((castleRights & (byte)CastleRights.QS_Black) != 0) ? "q" : "")
                + (castleRights == 0 ? "- " : " ")
                + (epLoc.col == (byte)Square.File.NONE ? "- " : (epLoc.ToString() + " "))
                + progress.ToString() + " "
                + moveNbr.ToString();

            foreach (Square s in board.Keys)
            {
                Piece pc = board[s];
                // find the spot for the single digit where this to be inserted 
                // n-> aPb, n-> aP, n-> Pb, n-> P
                int strIndex = 0;
                for (int i = 7; i > s.row; i--)
                    strIndex = outString.IndexOf('/', strIndex) + 1;
                int fileIndex = -1;
                char c;
                strIndex--;
                while (fileIndex < s.col)
                {
                    c = outString[++strIndex];
                    if (!Char.IsDigit(c))
                        ++fileIndex;
                    else
                        fileIndex += (c - '0');
                }
                // at this point the char pointed to by strindex is >= the intended col
                int curWidth = outString[strIndex] - '0';
                int leftPad = s.col - ((fileIndex + 1) - curWidth);
                char thisChar = pc.ToString()[0];
                thisChar = (pc.color == PlayerEnum.White ? Char.ToUpper(thisChar) : Char.ToLower(thisChar));
                int rightPad = fileIndex - s.col;

                outString = JPD.Utilities.Utils.SwapChar(outString, strIndex, thisChar);
                if (leftPad > 0)
                    outString = JPD.Utilities.Utils.InsertChar(outString, strIndex, (char)(leftPad + '0'));
                if (rightPad > 0)
                    outString = JPD.Utilities.Utils.InsertChar(outString, strIndex + (leftPad > 0 ? 2 : 1), (char)(rightPad + '0'));

            }
            return outString;
        }

    }
}
