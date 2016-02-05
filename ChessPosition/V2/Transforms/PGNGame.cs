using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ChessPosition.V2.PGN
{
    public class PGNGame : Game
    {
        // structures needed for PGN translation
        public enum GameSaveOptions { IncludeTags = 1, IncludeMoves = 2, IncludeComments = 4, IncludeVariations = 8, IncludeAll = 15, SimpleGameScore = 3, MoveListOnly = 2 };
        private static List<string> moveDecorators = new List<string>() { "#", "+", "++", "ep", "e.p.", "x", "=", "(", ")" };
        private static List<string> castleMarkers = new List<string>() { "O-O", "O-O-O" };
        public string PGNSource;
        public List<PGNToken> PGNtokens;

        public PGNGame(PGNTokenizer pgt)
        {
            InitGame();

            if (pgt.tokens.Count > 0) // ok, we have one
            {
                PGNtokens = pgt.tokens;
                PGNSource = "";
                Ply lastPly = null;
                foreach (PGNToken token in PGNtokens)
                {
                    BuildPlyFromToken(token, Plies, ref lastPly, Plies.Count);

                    if (token.tokenType == PGNTokenType.Terminator)
                        break;
                }
                // ok, we're out of tokens, must be a game in progress...that's the default...
                ResetPosition();
                PGNSource = GeneratePGNSource();
                return;
            }
        }
        protected override void InitGame()
        {
            base.InitGame();
            PGNSource = "";
            PGNtokens = new List<PGNToken>();
        }
        private void BuildPlyFromToken(PGNToken token, ICollection<Ply> curPlyCollection, ref Ply lastPly, int curPlyNumber)
        {
            switch (token.tokenType)
            {
                case PGNTokenType.Tag:
                    HandleTag((PGNTag)token);
                    break;
                case PGNTokenType.Invalid:
                    break;
                case PGNTokenType.MoveNumber:
                    break;
                case PGNTokenType.Comment:  // could be one of many
                    if (lastPly != null)
                        lastPly.comments.Add( new Comment( !((PGNComment)token).isBraceComment, token.value) );
                    break;
                case PGNTokenType.Terminator:
                    Terminator = new PGNTerminator(token.tokenString).terminatorType;
                    Tags["Result"] = TerminatorString;
                    break;
                case PGNTokenType.MoveString:
                    lastPly = HandleMove((PGNMoveString)token, curPlyCollection, curPlyNumber);
                    if (((PGNMoveString)token).variations != null)
                    {
                        Position varStartPosition = new Position(CurrentPosition);
                        foreach (List<PGNToken> var in ((PGNMoveString)token).variations)
                        {
                            CurrentPosition = new Position(varStartPosition);
                            List<Ply> varPlies = HandleVariation(var, curPlyCollection, curPlyNumber);
                            if (lastPly.variations == null)
                                lastPly.variations = new List<List<Ply>>();
                            lastPly.variations.Add(varPlies);
                        }
                        CurrentPosition = new Position(varStartPosition);
                    }
                    break;
            }
        }

        public string GeneratePGNSource()
        {
            return GeneratePGNSource(-1, GameSaveOptions.IncludeAll);
        }
        public string GeneratePGNSource(GameSaveOptions options)
        {
            return GeneratePGNSource(-1, options);
        }
        public string GeneratePGNSource(int newLineTrigger, GameSaveOptions options)
        {
            string outString = "";
            // tags
            if (((int)options & (int)GameSaveOptions.IncludeTags) != 0)
            {
                foreach (string tagKey in Tags.Keys)
                    outString += "[" + tagKey + " \"" + Tags[tagKey] + "\"]" + Environment.NewLine;
                outString += Environment.NewLine;
            }

            // moves
            if (((int)options & (int)GameSaveOptions.IncludeMoves) != 0)
            {
                outString += GeneratePGNSource(Plies, 0, outString.Length, newLineTrigger, options);

                // term
                outString += TerminatorString + Environment.NewLine + Environment.NewLine;
            }

            return outString;
        }
        public string GeneratePGNSource(List<Ply> variation, int curPly, int baseStrLen, int newLineTrigger, GameSaveOptions options)
        {
            string outString = "";
            int curLineLength = 0;

            // moves
            foreach (Ply ply in variation)
            {
                string plyString = PGNPly.GeneratePGNSource(CurrentPosition, Plies[curPly], curPly, options);
                ply.refToken.startLocation = baseStrLen + outString.Length;
                int dotLoc = plyString.IndexOf('.');

                if (dotLoc >= 0 && dotLoc < 6)
                    ply.refToken.startLocation += dotLoc + 1;
                outString += plyString;
                curLineLength += plyString.Length;

                if (curLineLength > newLineTrigger && newLineTrigger > 0)
                {
                    outString += Environment.NewLine;
                    curLineLength = 0;
                }

                curPly++;
            }
            return outString;
        }
        private void HandleTag(PGNTag t)
        {
            string valString = t.value;
            if (valString.Length >= 2 && valString[0] == '\"' && valString[valString.Length - 1] == '\"')
            {
                valString = valString.Substring(1);
                valString = valString.Substring(0, valString.Length - 1);
            }
            if (Tags.ContainsKey(t.key))
                Tags[t.key] = valString;
            else
                Tags.Add(t.key, valString);
        }
        public Ply HandleMove(PGNMoveString token, ICollection<Ply> curPlyCollection, int curPlyNumber)
        {
            curPlyNumber++;
            int curMoveNumber = (curPlyNumber - 1) / 2;

            string locString = token.value.Trim();

            for (int i = 0; i < moveDecorators.Count; i++)
            {
                int thisIndex = locString.IndexOf(moveDecorators[i]);
                if (thisIndex >= 0)
                {
                    // trim off the decorator
                    locString = locString.Substring(0, thisIndex) + locString.Substring(thisIndex + moveDecorators[i].Length);
                    i--;
                }
            }

            Ply curPly = null;
            if (castleMarkers.Contains(locString))
            {
                // src / dest squares should be easy here
                Square.Rank Krank = curPlyNumber % 2 == 1 ? Square.Rank.R1 : Square.Rank.R8;
                if (locString == castleMarkers[0]) // K-Side
                {
                    curPly = new Ply(
                        new Square(Krank, Square.File.FE),
                        new Square(Krank, Square.File.FG));
                    curPlyCollection.Add(curPly);
                }
                if (locString == castleMarkers[1]) // Q-Side
                {
                    curPly = new Ply(
                        new Square(Krank, Square.File.FE),
                        new Square(Krank, Square.File.FC));
                    curPlyCollection.Add(curPly);
                }
            }
            else
            {
                Piece curPiece = Piece.PieceFactory(curPlyNumber % 2 == 1 ? PlayerEnum.White : PlayerEnum.Black, Piece.PieceType.Pawn);

                Piece promoPiece = null;
                Piece.PieceType thisPcType = Piece.PieceType.Invalid;

                if (char.IsUpper(locString[0]))  // it's a piece designator, other than a P
                {
                    thisPcType = Piece.FromChar(locString[0]);
                    if (thisPcType == Piece.PieceType.Invalid)
                    {
                        // should handle the error more gracefully than this ...
                        System.Console.WriteLine("Couldn't properly process this move string: " + token);
                        return null;
                    }
                    curPiece = Piece.PieceFactory(curPiece.color, thisPcType);
                    locString = locString.Substring(1); // trim off the piece identifier
                }

                if (char.IsUpper(locString[locString.Length - 1]))   // it's a promotion designator...
                {
                    thisPcType = Piece.FromChar(locString[locString.Length - 1]);
                    promoPiece = Piece.PieceFactory(curPiece.color, thisPcType);
                    locString = locString.Substring(0, locString.Length - 1); // trim off the promotionidentifier
                }

                // the rest of the move string designates the targetsquare, and potentially a source hint
                // N2d4, Ree6, Re2e6, cd6, cd, cde.p., cd6e.p., d5, e8Q e8=Q e8(Q) potentially with a + or # afterwards
                // move decorators removed above since O-O could be O-O+....

                // find src and dest squares...
                Square TargetSquare = new Square();
                Square SourceSquare = new Square();
                List<Square> options;
                Square.Rank rowConstraint = Square.Rank.NONE;
                Square.File colConstraint = Square.File.NONE;
                switch (locString.Length)
                {
                    case 2: // either we have an explicit target square (e6), or an unambiguous P capture (dc)
                        if (Char.IsDigit(locString[1]))
                        {
                            TargetSquare.file = (Square.File)(locString[0] - 'a');
                            TargetSquare.rank = (Square.Rank)(locString[1] - '1');
                            options = CurrentPosition.FindPieceWithTarget(curPiece, TargetSquare, new Square(rowConstraint, colConstraint));
                            if (options.Count != 1)
                            {
                                System.Console.WriteLine("couldn't unambiguously process move <no pc for target> " + token.tokenString);
                                return null;
                            }
                            curPlyCollection.Add(curPly = new Ply(options[0], TargetSquare, promoPiece));
                        }
                        else
                        {
                            if (curPiece.piece != Piece.PieceType.Pawn)
                            {
                                System.Console.WriteLine("couldn't unambiguously process move <not P for col capture> " + token);
                                return null;
                            }

                            // cd -> how to id the source and target squares??  certainly both squares are col constrained
                            // the source square is constrained but how to constrain the targets?
                            // it's a capture on that file - we can start by listing all occupied squares on that file
                            // any source constrained returns should be unambiguously correct...
                            // if we could take more than one piece with a pawn on that file from this one it would need another qualifier
                            List<Square> possOpts = new List<Square>();
                            SourceSquare = null;
                            colConstraint = (Square.File)(locString[0] - 'a');
                            Square.File targetConstraint = (Square.File)(locString[1] - 'a');
                            for (Square.Rank row = Square.Rank.R1; row <= Square.Rank.R8; row++)
                            {
                                TargetSquare = new Square(row, targetConstraint);
                                if (CurrentPosition.board.ContainsKey(TargetSquare))
                                {
                                    options = CurrentPosition.FindPieceWithTarget(curPiece, TargetSquare, new Square(rowConstraint, colConstraint));
                                    if (options.Count > 1 || (options.Count > 0 && SourceSquare != null))
                                    {
                                        System.Console.WriteLine("couldn't unambiguously process move <col capture>  " + token);
                                        return null;
                                    }

                                    if (options.Count == 1)
                                        SourceSquare = options[0];
                                }
                            }
                            if (SourceSquare == null)
                            {
                                System.Console.WriteLine("couldn't unambiguously process move <col capture>  " + token.tokenString);
                                return null;
                            }
                            else
                                curPlyCollection.Add(curPly = new Ply(SourceSquare, TargetSquare, promoPiece));
                        }
                        break;
                    case 3:
                        colConstraint = (Char.IsLetter(locString[0]) ? (Square.File)(locString[0] - 'a') : Square.File.NONE);
                        rowConstraint = (Char.IsDigit(locString[0]) ? (Square.Rank)(locString[0] - '1') : Square.Rank.NONE);
                        TargetSquare.file = (Square.File)(locString[1] - 'a');
                        TargetSquare.rank = (Square.Rank)(locString[2] - '1');
                        options = CurrentPosition.FindPieceWithTarget(curPiece, TargetSquare, new Square(rowConstraint, colConstraint));
                        // at this point, there is a restriction on either row or col to validate
                        if (options.Count != 1)
                        {
                            System.Console.WriteLine("couldn't unambiguously process partially constrained move  " + token.tokenString);
                            return null;
                        }

                        curPlyCollection.Add(curPly = new Ply(options[0], TargetSquare, promoPiece));
                        break;
                    case 4:
                        SourceSquare.file = (Square.File)(locString[0] - 'a');
                        SourceSquare.rank = (Square.Rank)(locString[1] - '1');
                        TargetSquare.file = (Square.File)(locString[2] - 'a');
                        TargetSquare.rank = (Square.Rank)(locString[3] - '1');
                        options = CurrentPosition.FindPieceWithTarget(curPiece, TargetSquare, SourceSquare);
                        if (!options.Contains(SourceSquare) || options.Count != 1)
                        {
                            System.Console.WriteLine("couldn't find specified piece for move  " + token.tokenString);
                            return null;
                        }
                        curPlyCollection.Add(curPly = new Ply(SourceSquare, TargetSquare, promoPiece));
                        break;
                }
            }
            curPly.Number = curPlyNumber;
            curPly.refToken = token;
            CurrentPosition.MakeMove(curPly);
            curPly.refToken.isCheck = CurrentPosition.IsCheck();
            curPlyNumber++;
            return curPly;
        }
        public void SavePGN(StreamWriter sw)
        {
            int defaultLineTrigger = 90;
            sw.Write(GeneratePGNSource(defaultLineTrigger, GameSaveOptions.IncludeAll));
        }
        public Ply CreateMove(Square src, Square dest)
        {
            Ply outPly = new Ply(src, dest);
            outPly.refToken = new PGNMoveString(src.ToString() + dest.ToString());

            Piece thisPc = CurrentPosition.PieceAt(src);
            Piece capPc = CurrentPosition.PieceAt(dest);

            if (thisPc == null || thisPc.color != OnMove || (capPc != null && capPc.color == OnMove))
                return null;
            List<Square> options = CurrentPosition.FindPieceWithTarget(thisPc, dest, new Square());
            if (!options.Contains(src))
                return null;
            // at this point, thisPc is the right color, and could move from src->dest
            // Fully defined piece text = (Piece desig)(constrain row)(constrain col)(capture flag)(dest sq)

            // castle
            if (thisPc.piece == Piece.PieceType.King && src.file == Square.File.FE)
                if (dest.file == Square.File.FG &&
                    (CurrentPosition.castleRights & (byte)
                    (OnMove == PlayerEnum.White ?
                    Position.CastleRights.KS_White : Position.CastleRights.KS_Black)) != 0)
                {
                    outPly.refToken.tokenString = "O-O";
                    return outPly;
                }
                else if (dest.file == Square.File.FC &&
                    (CurrentPosition.castleRights & (byte)
                    (OnMove == PlayerEnum.White ?
                    Position.CastleRights.QS_White : Position.CastleRights.QS_Black)) != 0)
                {
                    outPly.refToken.tokenString = "O-O-O";
                    return outPly;
                }

            // move
            outPly.refToken.tokenString = thisPc.piece == Piece.PieceType.Pawn ? "" : thisPc.ToString();

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
                    outPly.refToken.tokenString += src.ToString().Substring(0, 1);
                else if (rankCount == 1)
                    // otherwise if there's only one on the src rank, use the rank as a dif
                    outPly.refToken.tokenString += src.ToString().Substring(1, 1);
                else
                    // otherwise use the full square
                    outPly.refToken.tokenString += src.ToString();
            }

            // capture or cap ep
            if (capPc != null || CurrentPosition.epLoc == dest && thisPc.piece == Piece.PieceType.Pawn)
                outPly.refToken.tokenString += "x";
            // dest sq
            outPly.refToken.tokenString += dest.ToString();

            return outPly;
        }

        private List<Ply> HandleVariation(List<PGNToken> var, ICollection<Ply> curPlyCollection, int curPlyNumber)
        {
            /// the token contains any embedded MoveText
            /// simply turn them into Plies or add to existing plies as necessary, and return the set
            List<Ply> outPly = new List<Ply>();
            Ply curPly = null;
            foreach (PGNToken token in var)
            {
                BuildPlyFromToken(token, outPly, ref curPly, curPlyNumber + outPly.Count);
            }
            return outPly;
        }
    
        public string BuildMoveList()
        {
            string outStr = "";
            foreach (PGNToken t in PGNtokens)
            {
                switch (t.tokenType)
                {
                    case PGNTokenType.MoveNumber:
                        outStr += t.tokenString;
                        break;
                    case PGNTokenType.MoveString:
                        outStr += t.tokenString + " ";
                        break;
                }
            }
            if (Plies.Count % 2 == 0) // white on move
                outStr += (Plies.Count / 2 + 1).ToString() + ".";
            outStr += " ...";
            return outStr;
        }

    }
}
