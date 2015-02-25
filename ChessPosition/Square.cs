using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessPosition
{
    public class Square
    {
        public enum Rank { R1 = 0, R2 = 1, R3 = 2, R4 = 3, R5 = 4, R6 = 5, R7 = 6, R8 = 7, NONE = 0x0f };
        public enum File { FA = 0, FB = 1, FC = 2, FD = 3, FE = 4, FF = 5, FG = 6, FH = 7, NONE = 0x0f };

        public byte loc;
        public byte row
        {
            get { return (byte)((loc >> 4) & 0x0f); }
            set { loc = (byte)((loc & 0x0f) + ((value & 0x0f) << 4)); }
        }
        public byte col
        {
            get { return (byte)(loc & 0x0f); }
            set { loc = (byte)((loc & 0xf0) + (value & 0x0f)); }
        }

        public Square()
        {
            loc = 0xff;
        }
        public Square(byte l)
        {
            loc = l;
        }
        public Square(Square s)
        {
            loc = s.loc;
        }
        public Square(Rank r, File f)
        {
            loc = (byte)((((int)r & 0x0f) << 4) | ((int)f & 0x0f));
        }
        public Square(byte r, byte c)
        {
            loc = (byte)(((r & 0x0f) << 4) | (c & 0x0f));
        }

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
            return loc;
        }
        public override bool Equals(object obj)
        {
            return Equals(obj as Square);
        }
        public bool Equals(Square obj)
        {
            return obj != null && obj.loc == this.loc;
        }
    }
}
