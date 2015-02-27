using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessPosition
{
    public class PositionHash
    {
        public char[] hashValue;

        char refZeroVal = '!';
        static int hashLength = 40;
        static int bitsPerChar = 6;

        public PositionHash(Position p)
        {
            /// 64 bits for presence / absence of pieces
            /// 4 bits for castle rights
            /// 4 bits for ep file (file of last p moved, 0x0f = no ep)
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
            /// total of 172 bits overall
            /// => 29 B64 encoded characters
            /// it's possible (but highly unlikely) that promotions without captures could add to this total
            /// theoretically trading every P for a Q would add 3 bits each, or 48 add'l total, 
            /// for a max of 220 bits / 37 B64 char
            /// 
            /// It's phenomenal overkill, but using a 40 char string as the hash leaves no issues, and allows the hash
            /// algorithm to extend for uncluding gamecontext if needed
            /// 
            /// note that this is COMPLETE from a move generation / evaluation perspective,
            /// but INCOMPLETE from a game management perspective because it doesn't 
            /// include 50 move and position repetition info.  
            /// In this model, that's included in the game object, since it's game-specific context

            int currentPieceOffset = 72;
            int castleRightsOffset = 64;
            int epOffset = 68;

            /// note, there are > 64 printable characters starting at ' ', so use anything close as 0...  '!' is one more and not whitespace
            /// ---------0         1         2         3
            /// ---------0123456789012345678901234567890123456789
            
            // ### test me...
            hashValue = new char[hashLength+1];
            for (int i = 0; i < hashLength; i++)
                hashValue[i] = refZeroVal;
            hashValue[hashLength] = '\0';

            /// load the bits for occupied squares
            /// load the castle and ep rights
            /// iterate in order to load the piece squares

            for (int i = 0; i < 64; i++)
                ClearBit(i);
            foreach (Square sq in p.board.Keys)
            {
                Piece pc = p.board[sq];
                int bitLocation = 8 * sq.row + sq.col;
                SetBit(bitLocation);
            }

            SetBit(castleRightsOffset + 0, (p.castleRights & 0x01) != 0);
            SetBit(castleRightsOffset + 1, (p.castleRights & 0x02) != 0);
            SetBit(castleRightsOffset + 2, (p.castleRights & 0x04) != 0);
            SetBit(castleRightsOffset + 3, (p.castleRights & 0x08) != 0);

            SetBit(epOffset + 0, (p.epLoc.col & 0x01) != 0);
            SetBit(epOffset + 1, (p.epLoc.col & 0x02) != 0);
            SetBit(epOffset + 2, (p.epLoc.col & 0x04) != 0);
            SetBit(epOffset + 3, (p.epLoc.col & 0x08) != 0);

            for (byte i = 0; i < 64; i++)
            {
                if (ReadBit(i))
                {
                    Piece thisPc = p.board[new Square((Square.Rank)(i / 8), (Square.File)(i % 8))];
                    string pcHash = Piece.Hash[(int)thisPc.piece];

                    SetBit(currentPieceOffset++, thisPc.color == PlayerEnum.Black);
                    for (int j = 0; j < pcHash.Length; j++)
                    {
                        SetBit(currentPieceOffset++, pcHash[0] == '1');
                    }
                }
            }

        }

        public Position Rehydrate()
        {
            // default constructor sets all castle/clears ep rights (has no game context)
            // all that's left to do is repop the board...
            Position outPos = new Position();
            outPos.board.Clear();

            int currentPieceOffset = 72;
            // not used as mentioned above
            //int castleRightsOffset = 64;
            //int epOffset = 68;

            for (byte i = 0; i < 64; i++)
            {
                if (ReadBit(i))
                {
                    Piece thisPc = ReadPiece(ref currentPieceOffset);
                    outPos.board.Add( new Square((Square.Rank)(i / 8), (Square.File)(i % 8)), thisPc );
                }
            }

            return outPos;
        }

        private Piece ReadPiece( ref int offset ) 
        {
            PlayerEnum plr = (ReadBit(offset++) ? PlayerEnum.Black : PlayerEnum.White);
            string curCode = "";
            while (curCode.Length < hashLength)
            {
                curCode += ReadBit(offset++) ? "1" : "0";
                if (Piece.Hash.Contains(curCode))
                {
                    return new Piece( plr, (Piece.PieceType)Piece.Hash.IndexOf(curCode));
                }
            }
            return null;
        }

        private void SetBit(int loc, bool val)
        {
            if (val)
                SetBit(loc);
            else
                ClearBit(loc);
        }
        private void SetBit(int loc)
        {
            int charIndex = loc / bitsPerChar;
            int bitOffset = loc % bitsPerChar;
            hashValue[charIndex] = (char)(((hashValue[charIndex]-refZeroVal) | (0x01 << bitOffset)) + refZeroVal);
        }
        private void ClearBit(int loc)
        {
            int charIndex = loc / bitsPerChar;
            int bitOffset = loc % bitsPerChar;
            hashValue[charIndex] = (char)(((hashValue[charIndex] - refZeroVal) & ~(0x01 << bitOffset)) + refZeroVal);
        }
        private bool ReadBit(int loc)
        {
            int charIndex = loc / bitsPerChar;
            int bitOffset = loc % bitsPerChar;
            return ((hashValue[charIndex] - refZeroVal) & (0x01 << bitOffset)) != 0;
        }

        //public override int GetHashCode()
        //{
        //   return hashValue.ToString();
        //}
        public override bool Equals(object obj)
        {
            return Equals(obj as PositionHash);
        }
        public bool Equals(PositionHash obj)
        {
            return obj != null && obj.hashValue.ToString() == this.hashValue.ToString();
        }

    }
}
