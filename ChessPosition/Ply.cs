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
        public string GeneratePGNSource(Game refGame, int curPly, int baseStrLen, int options)
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

            if ((options & (int)Game.PGNOptions.IncludeComments) != 0)
            {
                foreach (PGNComment comment in comments)
                    if (comment.isBraceComment)
                    {
                        outString += "{" + comment.value + "} ";
                    }
                    else
                    {
                        outString += "; " + comment.value + Environment.NewLine;
                    }
            }

            if ((options & (int)Game.PGNOptions.IncludeVariations) != 0)
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

    }
}
