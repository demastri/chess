using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessPosition
{
    public class Engine
    {
        static Dictionary<Piece, int> ValueMap = new Dictionary<Piece, int>()
        {
            { new Piece( PlayerEnum.White, Piece.PieceType.Pawn), 1 },
            { new Piece( PlayerEnum.White, Piece.PieceType.Rook), 5 },
            { new Piece( PlayerEnum.White, Piece.PieceType.Knight), 3 },
            { new Piece( PlayerEnum.White, Piece.PieceType.Bishop), 3 },
            { new Piece( PlayerEnum.White, Piece.PieceType.Queen), 9 },
            { new Piece( PlayerEnum.White, Piece.PieceType.King), 99 },
            { new Piece( PlayerEnum.Black, Piece.PieceType.Pawn), -1 },
            { new Piece( PlayerEnum.Black, Piece.PieceType.Rook), -5 },
            { new Piece( PlayerEnum.Black, Piece.PieceType.Knight), -3 },
            { new Piece( PlayerEnum.Black, Piece.PieceType.Bishop), -3 },
            { new Piece( PlayerEnum.Black, Piece.PieceType.Queen), -9 },
            { new Piece( PlayerEnum.Black, Piece.PieceType.King), -99 }
        };
        public static Engine InitEngine()
        {
            // ### no idea how to describe this yet
            return new Engine();
        }

        public Analysis GenerateAnalysis(Position p)
        {
            // ### no idea how to instantiate this yet
            return new Analysis();
        }

    }
}
