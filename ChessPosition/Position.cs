using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessPosition
{
    public class Position
    {
        public static Position StartPosition = new Position();
        public PositionHash Hash { get { return new PositionHash(this); } }

        // these values are included in the hash
        public Dictionary<Square, Piece> board;
        public byte castleRights;
        public Square epLoc;

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

    }
}
