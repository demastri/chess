using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessPosition
{
    public class Piece
    {
        public PlayerEnum color;
        public PieceType piece;
        
        public enum PieceType { Pawn=0, Rook=1, Knight=2, Bishop=3, Queen=4, King=5 }
        public static List<string> Hash = new List<string>( new string[] { "1", "010", "011", "000", "0010", "0011" } );


        public Piece(PlayerEnum p, PieceType t)
        {
            color = p;
            piece = t;
        }

        public static bool operator ==(Piece lhs, Piece rhs)
        {
            return (lhs == null && rhs == null) ||
                (lhs != null && rhs != null && lhs.color == rhs.color && lhs.piece == rhs.piece);
        }
        public static bool operator !=(Piece lhs, Piece rhs)
        {
            return !(lhs == rhs);
        }
    }
}
