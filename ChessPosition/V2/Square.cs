using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessPosition.V2
{
    /// <summary>
    /// This is just a bucket corresponding to a grid location
    /// </summary>
    public class Square
    {
        #region operator Overrides

        public static bool operator ==(Square lhs, Square rhs)
        {
            if (object.ReferenceEquals(null, lhs) && object.ReferenceEquals(null, rhs))
                return true;
            if (object.ReferenceEquals(null, lhs) || object.ReferenceEquals(null, rhs))
                return false;
            return lhs.loc == rhs.loc;
        }
        public static bool operator !=(Square lhs, Square rhs)
        {
            return !(lhs == rhs);
        }
        public override int GetHashCode()
        {
            return loc.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            return Equals(obj as Square);
        }
        public bool Equals(Square obj)
        {
            return obj != null && obj.loc == this.loc;
        }

        #endregion

        #region enums and static definitions

        public enum Rank { R1 = 0, R2 = 1, R3 = 2, R4 = 3, R5 = 4, R6 = 5, R7 = 6, R8 = 7, NONE = 0x0f };
        public enum File { FA = 0, FB = 1, FC = 2, FD = 3, FE = 4, FF = 5, FG = 6, FH = 7, NONE = 0x0f };
        public static byte RankMask = 0xf0;
        public static byte FileMask = 0x0f;
        public static byte NoLocation = 0xff;
        public static Square None() { return new Square(NoLocation); }

        #endregion

        #region constructors (empty, copy, and (rank/file))

        public Square()
        {
            loc = NoLocation;
        }
        public Square(Square s)
        {
            loc = s.loc;
        }
        private Square(byte thisLoc)
        {
            loc = thisLoc;
        }
        public Square(Rank r, File f)
        {
            rank = r;
            file = f;
        }

        #endregion

        #region properties

        private byte loc { get; set; }
        public Rank rank
        {
            get { return (Rank)((loc & RankMask) >> 4); }
            set { loc = (byte)((loc & FileMask) + (((byte)value << 4) & RankMask)); }
        }
        public File file
        {
            get { return (File)(loc & FileMask); }
            set { loc = (byte)((loc & RankMask) + ((byte)value & FileMask)); }
        }

        #endregion

        #region string/char conversions

        public override string ToString()
        {
            return (loc == NoLocation) ? "??" :
                "fr".Replace('r', (char)(rank + '1')).Replace('f', (char)(file + 'a'));
        }

        #endregion

        #region domain logic
        // none...
        #endregion
        
    }
}
