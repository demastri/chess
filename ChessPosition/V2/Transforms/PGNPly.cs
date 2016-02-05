using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessPosition.V2;

namespace ChessPosition.V2.PGN
{
    public class PGNPly
    {
        public static string GeneratePGNSource(Position curPos, Ply thisPly, int curPlyNbr, PGNGame.GameSaveOptions options)
        {
            string outString = "";

            bool WOnMove = (curPlyNbr % 2 == 0);
            int curMove = (curPlyNbr / 2 + 1);
            if (WOnMove)
            {
                outString += curMove.ToString() + ".";
            }

            /// ok - need to actually generate the move string here
            /// ###
            /// outstring += something short algebraic notation
            outString += CreatePlyMove(curPos, thisPly) + " ";

            bool shouldIncludeComments = (((int)options & (int)PGNGame.GameSaveOptions.IncludeComments) != 0);

            foreach (Comment comment in thisPly.comments)
            {
                if (!comment.isToEOL)
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

            if (((int)options & (int)PGNGame.GameSaveOptions.IncludeVariations) != 0)
            {
                if (thisPly.variations != null)
                    foreach (List<Ply> subVar in thisPly.variations)
                    {
                        Position varPos = new Position(curPos);
                        int varPlyNbr = curPlyNbr;
                        foreach (Ply nextPly in subVar)
                        {
                            string varString = PGNPly.GeneratePGNSource(varPos, nextPly, varPlyNbr++, options).Trim();
                            outString += "(" + varString + ") ";
                            varPos.MakeMove(nextPly);
                        }
                    }
            }
            return outString;
        }

        public static string CreatePlyMove(Position refPos, Ply thisPly)
        {
            string outString = "";

            Square src = thisPly.src;
            Square dest = thisPly.dest;

            Piece thisPc = refPos.PieceAt(src);
            Piece capPc = refPos.PieceAt(dest);

            if (thisPc == null || thisPc.color != refPos.onMove || (capPc != null && capPc.color == refPos.onMove))
                return null;
            List<Square> options = refPos.FindPieceWithTarget(thisPc, dest, new Square());
            if (!options.Contains(src))
                return null;
            // at this point, thisPc is the right color, and could move from src->dest
            // Fully defined piece text = (Piece desig)(constrain row)(constrain col)(capture flag)(dest sq)
            //  ### promotion and check/mate would be nice, too...

            // castle
            if (thisPc.piece == Piece.PieceType.King && src.file == Square.File.FE)
            {
                if (dest.file == Square.File.FG &&
                    (refPos.castleRights & (byte)
                    (refPos.onMove == PlayerEnum.White ?
                    Position.CastleRights.KS_White : Position.CastleRights.KS_Black)) != 0)
                {
                    outString = "O-O";
                }
                else if (dest.file == Square.File.FC &&
                    (refPos.castleRights & (byte)
                    (refPos.onMove == PlayerEnum.White ?
                    Position.CastleRights.QS_White : Position.CastleRights.QS_Black)) != 0)
                {
                    outString = "O-O-O";
                }
            }
            else
            {
                // move
                outString = thisPc.piece == Piece.PieceType.Pawn ? "" : thisPc.ToString();

                // constraining piece... or it's a capture with a P, need to state the source...
                if (options.Count > 1 || (options.Count == 1 && capPc != null && thisPc.piece == Piece.PieceType.Pawn))
                {
                    int rankCount = 0;
                    int fileCount = 0;
                    foreach (Square s in options)
                    {
                        if (s.rank == src.rank)
                            rankCount++;
                        if (s.file == src.file)
                            fileCount++;
                    }
                    if (fileCount == 1)
                        // if there's only one on the src file, use the file as a diff
                        outString += src.ToString().Substring(0, 1);
                    else if (rankCount == 1)
                        // otherwise if there's only one on the src rank, use the rank as a dif
                        outString += src.ToString().Substring(1, 1);
                    else
                        // otherwise use the full square
                        outString += src.ToString();
                }

                // capture or cap ep
                // # captures strictly don't require a specific placeholder
                if (capPc != null || refPos.epLoc == dest && thisPc.piece == Piece.PieceType.Pawn)
                    outString += "x";

                // dest sq
                // # a pawn capture, when it's the only possibility between the files involved, strictly doesn't require the rank
                outString += dest.ToString();

                //  promotion
                if (thisPly.promo != null && thisPly.promo.piece != Piece.PieceType.Invalid)
                    outString += "=" + thisPly.promo.piece.ToString();
            }
            //  check/mate would be nice, too... ?
            Position nextPos = new Position(refPos);
            nextPos.MakeMove(thisPly);
            if (nextPos.IsCheck())
            {
                if (nextPos.CandidateMoves().Count() == 0) // it's mate...
                    outString += "#";
                else
                    outString += "+";
            }

            return outString;
        }
    }
}
