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
        public static List<string> Hash = new List<string>(new string[] { "1", "010", "011", "000", "0010", "0011" });
        
        public Piece(PlayerEnum p, PieceType t)
        {
            color = p;
            piece = t;
        }

        private static List<string> Chess7Ref = new List<string>(new string[] { "po", "rt", "nm", "bv", "qw", "kl" });
        public char Chess7Char { get { return Chess7Ref[(int)piece][color == PlayerEnum.White ? 0 : 1]; } }

        public static bool operator ==(Piece lhs, Piece rhs)
        {
            if (object.ReferenceEquals(null, lhs) && object.ReferenceEquals(null, rhs) )
                return true;
            if (object.ReferenceEquals(null, lhs) || object.ReferenceEquals(null, rhs))
                return false;
            return lhs.color == rhs.color && lhs.piece == rhs.piece;
        }
        public static bool operator !=(Piece lhs, Piece rhs)
        {
            return !(lhs == rhs);
        }

        private static Dictionary<Piece.PieceType, string> PieceMapping = new Dictionary<PieceType,string>()
        {
            { Piece.PieceType.Rook, "R" },
            { Piece.PieceType.Knight, "N" },
            { Piece.PieceType.Bishop, "B" },
            { Piece.PieceType.Queen, "Q" },
            { Piece.PieceType.King, "K" },
            { Piece.PieceType.Pawn, "P" }
        };

        override public string ToString()
        {
            return PieceMapping[piece];
        }
    }
}
