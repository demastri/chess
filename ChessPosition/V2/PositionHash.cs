using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessPosition.V2
{
    public class PositionHash
    {
        #region operator Overrides

        public override bool Equals(object obj)
        {
            return Equals(obj as PositionHash);
        }
        public bool Equals(PositionHash obj)
        {
            return obj != null && obj.GetHashCode() == this.GetHashCode();
        }
        public static bool operator ==(PositionHash lhs, PositionHash rhs)
        {
            if (object.ReferenceEquals(null, lhs) && object.ReferenceEquals(null, rhs))
                return true;
            if (object.ReferenceEquals(null, lhs) || object.ReferenceEquals(null, rhs))
                return false;
            return lhs.GetHashCode() == rhs.GetHashCode();
        }
        public static bool operator !=(PositionHash lhs, PositionHash rhs)
        {
            return !(lhs == rhs);
        }
        public bool this[int i]
        {
            get
            {
                int charIndex = i / bitsPerChar;
                int bitOffset = i % bitsPerChar;
                return ((hashValue[charIndex] - refZeroVal) & (0x01 << bitOffset)) != 0;
            }
            set
            {
                int charIndex = i / bitsPerChar;
                int bitOffset = i % bitsPerChar;
                if (value)
                    hashValue[charIndex] = (char)(((hashValue[charIndex] - refZeroVal) | (0x01 << bitOffset)) + refZeroVal);
                else
                    hashValue[charIndex] = (char)(((hashValue[charIndex] - refZeroVal) & ~(0x01 << bitOffset)) + refZeroVal);
            }
        }
        public override int GetHashCode()
        {
            string x = "";
            for (int i = 0; i < hashValue.Length; i++)
                x += GetHexString(hashValue[i]);
            int y = x.GetHashCode();
            return y;
        }

        #endregion

        #region enums and static definitions

        private const char refZeroVal = '!';
        private const int hashLength = 50;
        private const int bitsPerChar = 6;

        private const int gridOffset = 0;
        private const int onMoveOffset = 64;
        private const int castleRightsOffset = 65;
        private const int epOffset = 69;
        private const int currentPieceOffset = 77;


        #endregion

        #region constructors (copy, and position)

        public PositionHash(PositionHash ph)
        {
            hashValue = (char[])ph.hashValue.Clone();
        }

        public PositionHash(Position p)
        {
            /// 64 bits for presence / absence of pieces
            /// 1 bit onmove - white=0
            /// 4 bits for castle rights
            /// 8 bits for ep capture square file (file of last p moved, 0xff = no ep)
            /// string of huffman encoded pieces with color bit prepended
            /// typical start position has:
            /// 16 P @ 1 bit each = 16 bits
            /// 4  R @ 3 bit each = 12 bits
            /// 4  N @ 3 bit each = 12 bits
            /// 4  B @ 3 bit each = 12 bits
            /// 2  Q @ 4 bit each =  8 bits
            /// 2  K @ 4 bit each =  8 bits
            /// subtotal of 68 bits for material
            /// additional max 32 color bits 
            /// total of 177 bits overall
            /// => 30 B64 encoded characters
            /// it's possible (but highly unlikely) that promotions without captures could add to this total
            /// theoretically trading every P for a Q would add 3 bits each, or 48 add'l total, 
            /// for a max of 220 bits / 37 B64 char
            /// 
            /// It's phenomenal overkill, but using a 50 char string as the hash leaves no issues, and allows the hash
            /// algorithm to extend for uncluding gamecontext if needed
            /// 
            /// note that this is COMPLETE from a move generation / evaluation perspective,
            /// but INCOMPLETE from a game management perspective because it doesn't 
            /// include 50 move and position repetition info.  
            /// In this model, that's included in the game object, since it's game-specific context


            /// note, there are > 64 printable characters starting at ' ', so use anything close as 0...  '!' is one more and not whitespace
            /// ---------0         1         2         3
            /// ---------0123456789012345678901234567890123456789

            // ### test me...
            hashValue = new char[hashLength + 1];
            for (int i = 0; i < hashLength; i++)
                hashValue[i] = refZeroVal;
            hashValue[hashLength] = '\0';

            /// load the bits for occupied squares
            /// load the castle and ep rights
            /// iterate in order to load the piece squares

            for (int i = 0; i < 64; i++)
                ClearBit(gridOffset + i);
            foreach (Square sq in p.board.Keys)
            {
                Piece pc = p.board[sq];
                int bitLocation = gridOffset + 8 * ((int)sq.rank) + ((int)sq.file);
                SetBit(bitLocation);
            }

            onMove = p.onMove;
            castleRights = p.castleRights;
            epLoc = p.epLoc;

            int thisPieceOffset = currentPieceOffset;

            for (byte i = 0; i < 64; i++)
            {
                if (ReadBit(i))
                {
                    Piece thisPc = p.board[new Square((Square.Rank)(i / 8), (Square.File)(i % 8))];
                    WritePiece(ref thisPieceOffset, thisPc);
                }
            }

        }
        #endregion

        #region properties

        public char[] hashValue { get; set; }

        public byte castleRights
        {
            get { return GetByteFromBitOffset(castleRightsOffset, 4); }
            set { WriteByteToBitOffset(castleRightsOffset, 4, value); }
        }
        public PlayerEnum onMove
        {
            get { return ReadBit(onMoveOffset) ? PlayerEnum.Black : PlayerEnum.White; }
            set { SetBit(onMoveOffset, (value != PlayerEnum.White)); }
        }
        public Square epLoc
        {
            get { return new Square( (Square.Rank)GetByteFromBitOffset(epOffset+4, 4),
                (Square.File)GetByteFromBitOffset(epOffset, 4));

            }
            set
            {
                WriteByteToBitOffset(epOffset, 4, (byte)value.file);
                WriteByteToBitOffset(epOffset + 4, 4, (byte)value.rank);
            }
        }

        #endregion

        #region string/char conversions

        // none...

        #endregion

        #region domain logic

        public Piece PieceExists( int index, out Square curSq )
        {
            int thisPieceOffset = currentPieceOffset;

            for (byte i = 0; i < 64; i++)
            {
                if (ReadBit(i))
                {
                    curSq = new Square((Square.Rank)(i / 8), (Square.File)(i % 8));
                    Piece thisPc = ReadPiece(ref thisPieceOffset);

                    if (index-- == 0)
                        return thisPc;
                }
            }
            curSq = new Square();
            return Piece.PieceFactory(PlayerEnum.Unknown, Piece.PieceType.Invalid);
        }

        private byte GetByteFromBitOffset(int offset, int width)
        {
            byte outByte = 0;
            for (int i = 0; i < width; i++)
                if (ReadBit(offset + i))
                    outByte = (byte)(outByte | (0x01 << i));
                else
                    outByte = (byte)(outByte & ~(0x01 << i));
            return outByte;
        }
        private void WriteByteToBitOffset(int offset, int width, byte value)
        {
            for (int i = 0; i < width; i++)
                SetBit(offset + i, (value & (0x01 << i)) != 0);
        }

        private Piece ReadPiece(ref int offset)
        {
            PlayerEnum plr = (ReadBit(offset++) ? PlayerEnum.Black : PlayerEnum.White);
            string curCode = "";
            while (curCode.Length < hashLength)
            {
                curCode += ReadBit(offset++) ? "1" : "0";
                if (Piece.Hash.Contains(curCode))
                {
                    return Piece.PieceFactory(plr, (Piece.PieceType)Piece.Hash.IndexOf(curCode));
                }
            }
            return null;
        }
        private void WritePiece(ref int offset, Piece thisPc)
        {
            string pcHash = Piece.Hash[(int)thisPc.piece];

            SetBit(offset++, thisPc.color == PlayerEnum.Black);
            for (int j = 0; j < pcHash.Length; j++)
            {
                SetBit(offset++, pcHash[j] == '1');
            }

        }

        private void SetBit(int loc, bool val)
        {
            this[loc] = val;
        }
        private void SetBit(int loc)
        {
            this[loc] = true;
        }
        private void ClearBit(int loc)
        {
            this[loc] = false;
        }
        private bool ReadBit(int loc)
        {
            return this[loc];
        }
        private string GetHexString(char c)
        {
            int lc = ((c & 0x00f0) >> 4);
            int rc = ((c & 0x000f));

            return ((char)(lc < 10 ? lc + '0' : lc + 'A')).ToString() + ((char)(rc < 10 ? rc + '0' : rc + 'A')).ToString();
        }
        #endregion

    }
}
