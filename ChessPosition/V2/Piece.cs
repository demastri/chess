using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessPosition.V2
{
    /// <summary>
    /// This is just a bucket corresponding to a piecetype and color
    /// may want to embed move rules with this class instead of the all encompassing game class...
    /// would likely override for individual pieces in that case...
    /// </summary>
    public class Piece
    {

        #region operator Overrides
        public static bool operator ==(Piece lhs, Piece rhs)
        {
            if (object.ReferenceEquals(null, lhs) && object.ReferenceEquals(null, rhs))
                return true;
            if (object.ReferenceEquals(null, lhs) || object.ReferenceEquals(null, rhs))
                return false;
            return lhs.color == rhs.color && lhs.piece == rhs.piece;
        }
        public static bool operator !=(Piece lhs, Piece rhs)
        {
            return !(lhs == rhs);
        }
        public override bool Equals(object o)
        {
            return (object)this == o;
        }
        public override int GetHashCode()
        {
            return pcValue.GetHashCode();
        }
        #endregion

        #region enums and static definitions
        public enum PieceType { Pawn = 0, Rook = 1, Knight = 2, Bishop = 3, Queen = 4, King = 5, Invalid = 6 }
        public static List<string> Hash = new List<string>(new string[] { "1", "010", "011", "000", "0010", "0011" });

        private static List<string> AsciiRef = new List<string>(new string[] { "Pp", "Rr", "Nn", "Bb", "Qq", "Kk", "Xx" });
        private static List<string> Chess7Ref = new List<string>(new string[] { "po", "rt", "nm", "bv", "qw", "kl", "Xx" });

        public static byte PcTypeMask = 0xf0;
        public static byte PcColorMask = 0x0f;
        public static byte NoPiece = 0x6f;
        #endregion

        #region constructors (empty, copy, and (color/type))
        public static Piece PieceFactory()
        {
            return PieceFactory(PlayerEnum.Unknown, PieceType.Invalid);
        }
        public static Piece PieceFactory(PlayerEnum p, PieceType t)
        {
            switch (t)
            {
                case PieceType.Pawn:
                    return new Pieces.Pawn(p);
                case PieceType.Rook:
                    return new Pieces.Rook(p);
                case PieceType.Knight:
                    return new Pieces.Knight(p);
                case PieceType.Bishop:
                    return new Pieces.Bishop(p);
            }
            return new Pieces.Invalid();
        }
        protected Piece()
        {
            pcValue = NoPiece;
        }
        protected Piece(PlayerEnum p, PieceType t)
        {
            Init(p, t);
        }
        protected virtual void Init(PlayerEnum p, PieceType t)
        {
            pcValue = (byte)(((byte)t & PcColorMask) + (((byte)p << 4) & PcTypeMask));
        }
        #endregion

        #region properties
        private byte pcValue { get; set; }

        public PieceType piece
        {
            get { return (PieceType)((pcValue & PcTypeMask) >> 4); }
        }
        public PlayerEnum color
        {
            get { return (PlayerEnum)(pcValue & PcColorMask); }
        }
        #endregion

        #region string/char conversions
        public char ToChess7Char { get { return Chess7Ref[(int)piece][color == PlayerEnum.Black ? 1 : 0]; } }
        public char ToAscii { get { return AsciiRef[(int)piece][color == PlayerEnum.Black ? 1 : 0]; } }

        override public string ToString()
        {
            return PieceFactory(PlayerEnum.White, piece).ToAscii.ToString();
        }
        public static Piece.PieceType FromChar(char c)
        {
            return FromFENChar(c).piece;
        }
        public static Piece FromFENChar(char c) // FENchar == asciiRef...
        {
            List<string> map = AsciiRef.Where(s => s.IndexOf(c) >= 0).ToList();
            if (map.Count > 0)
            {
                string s = map.First();
                return PieceFactory(s[0] == c ? PlayerEnum.White : PlayerEnum.Black, (PieceType)AsciiRef.IndexOf(s));
            }
            return PieceFactory(PlayerEnum.Unknown, PieceType.Invalid);
        }
        #endregion

        #region domain logic
        public virtual bool CouldMoveTo(Square source, Square dest, Dictionary<Square, Piece> board, Square epLoc, byte castleRights)
        {
            if (source == dest) // can't move to the same square
                return false;
            Piece destPiece = (board.ContainsKey(dest) ? board[dest] : new Piece());
            if (destPiece.piece != PieceType.Invalid && destPiece.color == color)   // or to a square with your piece
                return false;
            return true;
        }
        #endregion

    }
}
