using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessPosition
{
    public class Ply
    {
        public Guid PlyID { get; set; }

        public int Number { get; set; }
        public int MoveNumber { get { return (Number-1) / 2 + 1; } }

        public Square src { get; set; }
        public Square dest { get; set; }
        public Piece promo { get; set; }

        public PGNMoveString refToken;
        public List<PGNComment> comments;
        public ICollection<ICollection<Ply>> variation;

        public Ply()
        {
            PlyID = Guid.NewGuid();

            src = null;
            dest = null;
            promo = null;
            comments = new List<PGNComment>();
        }

        public Ply(Square s, Square d)
        {
            PlyID = Guid.NewGuid();

            src = new Square(s);
            dest = new Square(d);
            promo = null;
            comments = new List<PGNComment>();
        }
        public Ply(Square s, Square d, Piece p)
        {
            PlyID = Guid.NewGuid();

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
    }
}
