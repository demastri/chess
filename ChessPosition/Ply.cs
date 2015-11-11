using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessPosition
{
    public class Ply
    {
        public int Number;
        public int MoveNumber { get { return Number / 2 + 1; } }

        public Square src;
        public Square dest;
        public Piece promo;

        public PGNMoveString refToken;
        public List<PGNComment> comments;
        public List<List<Ply>> variation;

        public Ply()
        {
            src = null;
            dest = null;
            promo = null;
            comments = new List<PGNComment>();
        }
        public Ply(string s)    // compact token notation
        {
            int loc = s.IndexOf("Nbr:");
            Number = (loc < 0 ? -1 : Convert.ToInt32(s.Substring(loc + 4).Split()[0]));

            loc = s.IndexOf("Src:");
            src = new Square((byte)(loc < 0 ? 255 : Convert.ToInt32(s.Substring(loc + 4).Split()[0])));

            loc = s.IndexOf("Dest:");
            dest = new Square((byte)(loc < 0 ? 255 : Convert.ToInt32(s.Substring(loc + 5).Split()[0])));

            loc = s.IndexOf("Promo:");
            string thisPcStr = s.Substring(loc + 5).Split()[0];
            if (thisPcStr == "-")
                promo = null;
            else
                promo = new Piece(thisPcStr[0] == Char.ToUpper(thisPcStr[0]) ? PlayerEnum.White : PlayerEnum.Black, Piece.FromChar(Char.ToUpper(thisPcStr[0])));

            int curLoc = loc + "Promo:x ".Length;

            while (curLoc < s.Length)
            {
                // comments
                string nextToken = Game.ReadNextCompactTokenString(s.Substring(curLoc));


                // variations
            }

        }

        public Ply(Square s, Square d)
        {
            src = new Square(s);
            dest = new Square(d);
            promo = null;
            comments = new List<PGNComment>();
        }
        public Ply(Square s, Square d, Piece p)
        {
            src = new Square(s);
            dest = new Square(d);
            promo = p;
            comments = new List<PGNComment>();
        }
        public string GeneratePGNSource(Game refGame, int curPly, int baseStrLen, Game.GameSaveOptions options)
        {
            string outString = "";

            bool WOnMove = (curPly % 2 == 0);
            int curMove = (curPly / 2 + 1);
            if (WOnMove)
            {
                outString += curMove.ToString() + ".";
            }
            refToken.startLocation = baseStrLen + outString.Length;

            outString += refToken.value + " ";

            bool shouldIncludeComments = (((int)options & (int)Game.GameSaveOptions.IncludeComments) != 0);

            foreach (PGNComment comment in comments)
            {
                if (comment.isBraceComment)
                {
                    if (shouldIncludeComments || comment.value.IndexOf("PenaltyDays:") == 0)
                    {
                        outString += "{" + comment.value + "} ";
                    }
                }
                else
                {
                    if (shouldIncludeComments)
                    {
                        outString += "; " + comment.value + Environment.NewLine;
                    }
                }
            }

            if (((int)options & (int)Game.GameSaveOptions.IncludeVariations) != 0)
            {
                if (variation != null)
                    foreach (List<Ply> subVar in variation)  // ### has to actually to be foreach List<Ply> subVar in ply.variations)
                    {
                        string varString = refGame.GeneratePGNSource(subVar, curPly, outString.Length + 1, -1, options).Trim();
                        outString += "(" + varString + ") ";
                    }
            }
            return outString;
        }
        public string GenerateCompactSource()
        {
            string outString =
                "[Ply Nbr:" + Number.ToString() +
                " Src:" + src.loc.ToString() +
                " Dest:" + dest.loc.ToString() +
                " Promo:" + (promo == null ? "-" : (promo.color == PlayerEnum.White ? promo.ToString().ToUpper() : promo.ToString().ToLower())) + " ";

            foreach (PGNComment comment in comments)
            {
                if (comment.isBraceComment)
                    outString += "[BraceComment:" + comment.value + "]";
                else
                    outString += "[LineComment:" + comment.value + "]";
            }
            if (variation != null)
                foreach (List<Ply> var in variation)
                {
                    outString += "[Variation:";
                    foreach (Ply p in var)
                        outString += p.GenerateCompactSource();
                    outString += "]";
                }

            outString += "]";
            /***
                public int Number;
                public int MoveNumber { get { return Number / 2 + 1; } }

                public Square src;
                public Square dest;
                public Piece promo;

                public PGNMoveString refToken;
                public List<PGNComment> comments;
                public List<List<Ply>> variation;
            ***/
            return outString;
        }
    }
}
