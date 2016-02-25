using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JPD.Utilities;

namespace ChessPosition.V2
{
    public class Position
    {
        #region operator Overrides


        #endregion

        #region enums and static definitions

        public enum CastleRights { KS_White = 0x01, QS_White = 0x02, KS_Black = 0x04, QS_Black = 0x08 };
        public static string startFen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
        public static Position StartPosition { get {return new Position(startFen); } }
        public PositionHash Hash { get { return new PositionHash(this); } }

        #endregion

        #region constructors (empty, copy, from Hash value and FEN)

        public Position()
        {
            ResetStartPosition();
        }
        public Position(Position p) // copy-con
        {
            if (p == null)
                Clone(StartPosition);
            else 
                Clone(p);
        }
        public Position(PositionHash ph)
        {
            Clone(ph);
        }
        public Position(string FEN)
        {
            Clone(FEN);
        }

        #endregion

        #region properties

        // these values need to be managed in any two way hash of the position...
        public PlayerEnum onMove { get; set; }
        public Dictionary<Square, Piece> board { get; set; }
        public byte castleRights { get; set; }   // bits 0-4 -> WK WQ BK BQ
        public Square epLoc { get; set; }        // the ep file is the file, for convenience the rank IDs the square "behind" the P (the capture sq)

        #endregion

        #region string/char conversions

        public string ToFEN(int progress, int moveNbr)
        {
            string outString = "8/8/8/8/8/8/8/8 "
                + (onMove == PlayerEnum.White ? "w " : "b ")
                + (((castleRights & (byte)CastleRights.KS_White) != 0) ? "K" : "")
                + (((castleRights & (byte)CastleRights.QS_White) != 0) ? "Q" : "")
                + (((castleRights & (byte)CastleRights.KS_Black) != 0) ? "k" : "")
                + (((castleRights & (byte)CastleRights.QS_Black) != 0) ? "q" : "")
                + (castleRights == 0 ? "- " : " ")
                + (epLoc.file == Square.File.NONE ? "- " : (epLoc.ToString() + " "))
                + progress.ToString() + " "
                + moveNbr.ToString();

            foreach (Square s in board.Keys)
            {
                Piece pc = board[s];
                // find the spot for the single digit where this to be inserted 
                // n-> aPb, n-> aP, n-> Pb, n-> P
                int strIndex = 0;
                for (Square.Rank i = Square.Rank.R8; i > s.rank; i--)
                    strIndex = outString.IndexOf('/', strIndex) + 1;
                int fileIndex = -1;
                char c;
                strIndex--;
                while (fileIndex < (int)s.file)
                {
                    c = outString[++strIndex];
                    if (!Char.IsDigit(c))
                        ++fileIndex;
                    else
                        fileIndex += (c - '0');
                }
                // at this point the char pointed to by strindex is >= the intended col
                int curWidth = outString[strIndex] - '0';
                int leftPad = (int)s.file - ((fileIndex + 1) - curWidth);
                char thisChar = pc.ToString()[0];
                thisChar = (pc.color == PlayerEnum.White ? Char.ToUpper(thisChar) : Char.ToLower(thisChar));
                int rightPad = fileIndex - (int)s.file;

                outString = JPD.Utilities.Utils.SwapChar(outString, strIndex, thisChar);
                if (leftPad > 0)
                    outString = JPD.Utilities.Utils.InsertChar(outString, strIndex, (char)(leftPad + '0'));
                if (rightPad > 0)
                    outString = JPD.Utilities.Utils.InsertChar(outString, strIndex + (leftPad > 0 ? 2 : 1), (char)(rightPad + '0'));

            }
            return outString;
        }

        private void Clone(Position p)
        {
            board = new Dictionary<Square, Piece>(p.board);
            epLoc = new Square(p.epLoc);
            castleRights = p.castleRights;
            onMove = p.onMove;
        }
        private void Clone(PositionHash ph)
        {
            board.Clear();

            int nextPc = 0;
            Piece curPiece;
            Square curSq;
            while ((curPiece = ph.PieceExists(nextPc++, out curSq)).piece != Piece.PieceType.Invalid)
                board.Add(curSq, curPiece);

            onMove = ph.onMove;
            castleRights = ph.castleRights;
            epLoc = new Square(ph.epLoc);
        }
        private void Clone(string fenString)
        {
            board = new Dictionary<Square, Piece>();

            int curFenStringIndex = 0;
            char thisChar;

            for (Square.Rank rank = Square.Rank.R8; rank >= Square.Rank.R1; rank--)
            {
                Square.File file = Square.File.FA;
                while (file <= Square.File.FH)
                {
                    thisChar = fenString[curFenStringIndex++];
                    if (Char.IsDigit(thisChar))
                        file += (byte)(thisChar - '0');
                    else
                        board.Add(new Square(rank, file++), Piece.FromFENChar(thisChar));
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

        #endregion

        #region domain logic

        private void ResetStartPosition()
        {
            Clone(startFen);
        }

        public Piece PieceAt(Square s)
        {
            return board.Keys.Contains(s) ? board[s] : null;
        }

        public List<Square> FindPieceWithTarget(Piece p, Square target, Square sourceConstraint)
        {
            List<Square> options = new List<Square>();
            if (target.rank == Square.Rank.NONE)
                for (Square.Rank r = Square.Rank.R1; r <= Square.Rank.R8; r++)
                {
                    Square thisTarget = new Square(r, target.file);
                    List<Square> localOptions = FindPieceWithTarget(p, thisTarget, sourceConstraint);
                    options.AddRange(localOptions);
                }
            else
            {
                if (target.file == Square.File.NONE)
                    for (Square.File f = Square.File.FA; f <= Square.File.FH; f++)
                    {
                        Square thisTarget = new Square(target.rank, f);
                        List<Square> localOptions = FindPieceWithTarget(p, thisTarget, sourceConstraint);
                        options.AddRange(localOptions);
                    }
                else
                {

                    foreach (Square source in board.Keys)
                    {
                        if (board[source] == p && p.CouldMoveTo(source, target, board, epLoc, castleRights))
                        {
                            if ((sourceConstraint.rank == Square.Rank.NONE || source.rank == sourceConstraint.rank) &&
                                (sourceConstraint.file == Square.File.NONE || source.file == sourceConstraint.file))
                            {
                                Position possPos = new Position(this);
                                Ply testPly = new Ply(source, target, p);    // i think promo piece is irrelevant here, if the piece is pinned, it doesn't matter what it promotes to...
                                possPos.MakeMove(testPly);
                                if (!possPos.IsStillCheck())
                                    options.Add(source);
                            }
                        }
                    }
                }
            }
            return options;
        }

        public bool IsStillCheck()  // is the just moved King being attacked??
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
                if (board[s].color == onMove && board[s].CouldMoveTo(s, attackSq, board, epLoc, castleRights))
                    return true;
            return false;
        }
        public bool IsCheck()  // is the onMove King being attacked??
        {
            // find the opposing king
            Square attackSq = null;
            foreach (Square s in board.Keys)
                if (board[s].piece == Piece.PieceType.King && board[s].color == onMove)    // got him
                {
                    attackSq = s;
                    break;
                }
            // see if any pieces can move there...
            foreach (Square s in board.Keys)
                if (board[s].color != onMove && board[s].CouldMoveTo(s, attackSq, board, epLoc, castleRights))
                    return true;
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
                if (Math.Abs(p.src.file - p.dest.file) == 2)  // castling...
                {
                    Square.Rank rank = (Square.Rank)p.src.rank;
                    Square.File file = p.dest.file == Square.File.FC ? Square.File.FA : Square.File.FH;
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
                if ((Square.Rank)p.src.rank == rank)
                    if ((Square.File)p.src.file == Square.File.FA)
                        castleRights = (byte)(castleRights & ~(byte)
                            (onMove == PlayerEnum.White ? Position.CastleRights.QS_White : Position.CastleRights.QS_Black));
                    else if ((Square.File)p.src.file == Square.File.FH)
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
                    Square capSq = new Square((epLoc.rank + (onMove == PlayerEnum.White ? -1 : 1)), epLoc.file);
                    if (!board.ContainsKey(capSq))
                        System.Console.WriteLine("Can't find the ep capture piece...");
                    board.Remove(capSq);
                }

                // telegraph any possible ep capture
                if (Math.Abs(p.src.rank - p.dest.rank) == 2)   // it advanced two
                {
                    epLoc.file = p.src.file;
                    epLoc.rank = (p.dest.rank + (onMove == PlayerEnum.White ? -1 : 1));
                }
                else
                {
                    epLoc = Square.None();
                }
            }
            else
            {
                epLoc = Square.None();
            }

            onMove = (onMove == PlayerEnum.White ? PlayerEnum.Black : PlayerEnum.White);
        }

        public List<Ply> CandidateMoves()
        {
            // this does NOT generate all potential promotions, just the fact that a P could have promoted to something...
            List<Ply> candidates = new List<Ply>();

            foreach( Square sq in board.Keys )
                if (board[sq].color == onMove)
                {
                    for( Square.Rank r = Square.Rank.R1; r <= Square.Rank.R8; r++ )
                        for (Square.File f = Square.File.FA; f <= Square.File.FH; f++)
                        {
                            List<Square> dest = FindPieceWithTarget(board[sq], new Square(r, f), sq);
                            foreach( Square d in dest )
                            {
                                Ply possPly = new Ply(sq, d);
                                if( board[sq].piece == Piece.PieceType.Pawn && (d.rank == Square.Rank.R1 || d.rank == Square.Rank.R8) )
                                    possPly.promo = Piece.PieceFactory(onMove, Piece.PieceType.Queen);
                                candidates.Add( possPly );
                            }
                        }
                }

            return candidates;
        }
        #endregion
        
    }
}
