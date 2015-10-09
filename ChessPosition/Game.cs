﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ChessPosition
{
    public class Game
    {
        public Dictionary<string, string> Tags;
        public List<Ply> Plies;
        public PGNTerminator GameTerm;
        public Position CurrentPosition;
        public PlayerEnum OnMove { get { return CurrentPosition.onMove; } set { CurrentPosition.onMove = value; } }
        public int RatingWhite;
        public int RatingBlack;


        public string PGNSource;

        public static int Unrated = -1;
        public static int NoRating = -2;

        public List<PGNToken> PGNtokens;

        public int curPly;
        // these values are NOT included in the hash, since they're defined here...
        Dictionary<PositionHash, int> repetitions;
        private int progressCounter;

        public static void SaveGameFile(string GameFileLoc, List<Game> games)
        {
            StreamWriter tr = new StreamWriter(GameFileLoc);
            foreach (Game g in games)
                g.SaveCompact(tr);
            tr.Flush();
            tr.Close();
        }
        public static void SavePGNFile(string PGNFileLoc, List<Game> games)
        {
            StreamWriter tr = new StreamWriter(PGNFileLoc);
            foreach (Game g in games)
                g.SavePGN(tr);
            tr.Flush();
            tr.Close();
            // test
            SaveGameFile(PGNFileLoc + ".gam", games);
        }
        public static List<Game> ReadGameFile(string GameFileLoc)
        {
            List<Game> GameRef = new List<Game>();
            if (GameFileLoc != "" && File.Exists(GameFileLoc))
            {
                StreamReader tr = new StreamReader(GameFileLoc);
                while (!tr.EndOfStream)
                    GameRef.Add(new Game(tr));
                tr.Close();
            }
            return GameRef;
        }
        public static List<Game> ReadPGNFile(string PGNFileLoc)
        {
            List<Game> GameRef = new List<Game>();
            if (PGNFileLoc != "" && File.Exists(PGNFileLoc))
            {
                StreamReader tr = new StreamReader(PGNFileLoc);
                PGNTokenizer nextTokenSet = new PGNTokenizer(tr);
                tr.Close();
                for (int i = 0; i < nextTokenSet.GameCount; i++)
                {
                    nextTokenSet.LoadGame(i);
                    GameRef.Add(new Game(nextTokenSet));
                }
            }
            return GameRef;
        }

        public Game(StreamReader tr)
        {
            // ### read it here...
            InitGame();
            bool done = false;
            while (!done && !tr.EndOfStream)
            {
                string compactToken;
                string lastTokenName;
                compactToken = ReadNextCompactTokenString(tr);
                lastTokenName = ParseCompactTokenString(compactToken);

                if (lastTokenName == "Term")
                    done = true;
            }
        }
        public static string ReadNextCompactTokenString(StreamReader tr)
        {
            string outToken = "";
            // every time this runs, the next usable character should be a '['
            bool inToken = false;
            while (!inToken && !tr.EndOfStream)
                inToken = (tr.Read() == (int)'[');
            if (inToken)
            {
                int lBraceCount = 1;
                int nextChar = 0;
                while (!tr.EndOfStream && nextChar != ']' && lBraceCount == 0)
                {
                    nextChar = tr.Read();
                    if (nextChar == '[')
                        lBraceCount++;
                    if (nextChar == ']')
                        lBraceCount--;
                    outToken += ((char)nextChar).ToString();
                }
                if (tr.EndOfStream)
                    return "";
                outToken = outToken.Substring(0, outToken.Length - 1);  // strip off the last brace
            }
            return outToken;
        }
        public static string ReadNextCompactTokenString(string s)
        {
            return ReadNextCompactTokenString(new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(s))));
        }
        private string ParseCompactTokenString(string compactToken)
        {
            string name = "None";
            if (compactToken.IndexOf("Tag") == 0)
            {
                int lbr = compactToken.IndexOf('<');
                int rbr = compactToken.IndexOf('>');
                string key = compactToken.Substring(lbr + 1, rbr - lbr - 1);
                lbr = compactToken.IndexOf('<', lbr + 1);
                rbr = compactToken.IndexOf('>', rbr + 1);
                string val = compactToken.Substring(lbr + 1, rbr - lbr - 1);
                Tags.Add(key, val);
                name = "Tag";
            }
            if (compactToken.IndexOf("Ply") == 0)
            {
                Plies.Add(new Ply(compactToken));
                name = "Ply";
            }
            if (compactToken.IndexOf("Term") == 0)
            {
                int lbr = compactToken.IndexOf('<');
                int rbr = compactToken.IndexOf('>');
                GameTerm = new PGNTerminator(compactToken.Substring(lbr + 1, rbr - lbr - 1));
                name = "Term";
            }
            return name;
        }


        public Game(PGNTokenizer pgt)
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
        private void BuildPlyFromToken(PGNToken token, List<Ply> curPlyCollection, ref Ply lastPly, int curPlyNumber)
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
                        lastPly.comments.Add((PGNComment)token);
                    break;
                case PGNTokenType.Terminator:
                    GameTerm = new PGNTerminator(token.tokenString);
                    Tags["Result"] = GameTerm.tokenString;
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
                            if (lastPly.variation == null)
                                lastPly.variation = new List<List<Ply>>();
                            lastPly.variation.Add(varPlies);
                        }
                        CurrentPosition = new Position(varStartPosition);
                    }
                    break;
            }
        }

        public string GenerateCompactSource()
        {
            string outString = "";
            // tags
            foreach (string tagKey in Tags.Keys)
                outString += "[Tag Name<" + tagKey + "> Val<" + Tags[tagKey] + ">]";

            // moves
            foreach (Ply p in Plies)
                outString += p.GenerateCompactSource();

            // term
            outString += "[Term Val<" + GameTerm.value + ">]";

            return outString;
        }

        public enum GameSaveOptions { IncludeTags = 1, IncludeMoves = 2, IncludeComments = 4, IncludeVariations = 8, IncludeAll = 15, SimpleGameScore = 3, MoveListOnly = 2 };

        private GameSaveOptions SelectedOptions = GameSaveOptions.IncludeAll;
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
                outString += GameTerm.value + Environment.NewLine + Environment.NewLine;
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
                string plyString = ply.GeneratePGNSource(this, curPly, baseStrLen, options);
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

                continue;

                bool WOnMove = (curPly % 2 == 0);
                int curMove = (curPly / 2 + 1);
                if (WOnMove)
                {
                    outString += curMove.ToString() + ".";
                    curLineLength += curMove.ToString().Length + 1;
                }
                ply.refToken.startLocation = baseStrLen + outString.Length;

                outString += ply.refToken.value + " ";
                curLineLength += ply.refToken.value.Length + 1;

                if (((int)options & (int)GameSaveOptions.IncludeComments) != 0)
                {
                    foreach (PGNComment comment in ply.comments)
                        if (comment.isBraceComment)
                        {
                            outString += "{" + comment.value + "} ";
                            curLineLength += comment.value.Length + 2;
                        }
                        else
                        {
                            outString += "; " + comment.value + Environment.NewLine;
                            curLineLength = 0;
                        }
                }

                bool useToken = false;

                if (((int)options & (int)GameSaveOptions.IncludeVariations) != 0)
                {
                    if (useToken)
                    {
                        if (ply.refToken.variations != null)
                            foreach (List<PGNToken> subVar in ply.refToken.variations)  // ### has to actually to be foreach List<Ply> subVar in ply.variations)
                            {
                                outString += "(";
                                foreach (PGNToken t in subVar)
                                    outString += t.tokenString + " ";
                                outString = outString.Trim();
                                outString += ") ";
                            }
                    }
                    else
                    {
                        if (ply.variation != null)
                            foreach (List<Ply> subVar in ply.variation)  // ### has to actually to be foreach List<Ply> subVar in ply.variations)
                            {
                                string varString = GeneratePGNSource(subVar, curPly, outString.Length + 1, -1, options).Trim();
                                outString += "(" + varString + ") ";
                                curLineLength += varString.Length;
                            }
                    }
                }

                if (curLineLength > newLineTrigger && newLineTrigger > 0)
                {
                    outString += Environment.NewLine;
                    curLineLength = 0;
                }

                curPly++;
            }
            return outString;
        }
        public Game()
        {
            InitGame();
        }
        private void InitGame()
        {
            Plies = new List<Ply>();
            repetitions = new Dictionary<PositionHash, int>();
            PGNtokens = new List<PGNToken>();

            Tags = new Dictionary<string, string>();
            Tags.Add("Event", "");  // Seven Tag Roster of mandatory tags for archival work
            Tags.Add("Site", "");
            Tags.Add("Date", "");
            Tags.Add("Round", "");
            Tags.Add("White", "");
            Tags.Add("Black", "");
            Tags.Add("Result", "");

            ResetPosition();
            PGNSource = "";
            GameTerm = new PGNTerminator(PGNTerminator.terminators[(int)PGNTerminator.TerminatorTypes.InProgress]);

            RatingWhite = RatingBlack = NoRating;
        }

        public void ResetPosition()
        {
            CurrentPosition = new Position(Position.StartPosition);
            OnMove = PlayerEnum.White;
            curPly = 0;
            progressCounter = 0;
        }

        public bool AdvancePosition()
        {
            return AdvancePosition(1);
        }
        public bool AdvancePosition(int nbrAhead)
        {
            if (nbrAhead < 0)
            {
                BackPosition(-nbrAhead);
                return true;
            }
            /// advance game state - include castle rights, rep, ep and progress counters
            /// note the newly arrived at position in the repetition count ### test
            /// update the progress counters as appropriate
            for (int i = 0; i < nbrAhead; i++)
            {
                if (curPly >= Plies.Count)
                    return false;
                int prePcCount = CurrentPosition.board.Count;
                bool wasApawn = CurrentPosition.board[Plies[curPly].src].piece == Piece.PieceType.Pawn;
                CurrentPosition.MakeMove(Plies[curPly++]);
                bool wasAcapture = prePcCount != CurrentPosition.board.Count; // it was a capture
                if (wasApawn || wasAcapture)
                    progressCounter = 0;
                else
                    progressCounter++;
                PositionHash thisHash = new PositionHash(CurrentPosition);
                if (repetitions.ContainsKey(thisHash))
                    repetitions[thisHash]++;
                else
                    repetitions.Add(thisHash, 0);
            }
            return true;
        }
        public bool EndOfGame { get { return curPly >= Plies.Count; } }
        public void BackPosition()
        {
            BackPosition(1);
        }
        public void BackPosition(int nbrBack)
        {
            // hack... something weird happened on a takeback - a neighboring P can disappear
            // some kind of goofy ep thing, think it was related to not setting OnMove properly on a reset
            int targetPly = curPly - nbrBack;
            if (targetPly < 0)
                targetPly = 0;
            ResetPosition();
            for (int i = 0; i < targetPly; i++)
                AdvancePosition();
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

            if ((t.key == "WhiteElo" || t.key == "WhiteUSCF") && valString.Length > 0)
                RatingWhite = Convert.ToInt32(valString);
            if ((t.key == "BlackElo" || t.key == "BlackUSCF") && valString.Length > 0)
                RatingBlack = Convert.ToInt32(valString);
        }

        public string ToFEN()
        {
            return CurrentPosition.ToFEN(progressCounter, curPly / 2 + 1);
        }

        public Ply CreateMove(Square src, Square dest)
        {
            Ply outPly = new Ply(src, dest);
            outPly.refToken = new PGNMoveString(src.ToString() + dest.ToString());

            Piece thisPc = CurrentPosition.PieceAt(src);
            Piece capPc = CurrentPosition.PieceAt(dest);

            if (thisPc == null || thisPc.color != OnMove || (capPc != null && capPc.color == OnMove))
                return null;
            List<Square> options = CurrentPosition.FindPieceWithTarget(thisPc, dest, Square.Rank.NONE, Square.File.NONE);
            if (!options.Contains(src))
                return null;
            // at this point, thisPc is the right color, and could move from src->dest
            // Fully defined piece text = (Piece desig)(constrain row)(constrain col)(capture flag)(dest sq)

            // castle
            if (thisPc.piece == Piece.PieceType.King && (Square.File)src.col == Square.File.FE)
                if ((Square.File)dest.col == Square.File.FG &&
                    (CurrentPosition.castleRights & (byte)
                    (OnMove == PlayerEnum.White ?
                    Position.CastleRights.KS_White : Position.CastleRights.KS_Black)) != 0)
                {
                    outPly.refToken.tokenString = "O-O";
                    return outPly;
                }
                else if ((Square.File)dest.col == Square.File.FC &&
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
                    if (s.row == src.row)
                        rankCount++;
                    if (s.col == src.col)
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

        private static List<string> moveDecorators = new List<string>() { "#", "+", "++", "ep", "e.p.", "x", "=", "(", ")" };
        private static List<string> castleMarkers = new List<string>() { "O-O", "O-O-O" };

        public Ply HandleMove(PGNMoveString token, List<Ply> curPlyCollection, int curPlyNumber)
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
                Piece curPiece = new Piece(curPlyNumber % 2 == 1 ? PlayerEnum.White : PlayerEnum.Black, Piece.PieceType.Pawn);
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
                    curPiece.piece = thisPcType;
                    locString = locString.Substring(1); // trim off the piece identifier
                }

                if (char.IsUpper(locString[locString.Length - 1]))   // it's a promotion designator...
                {
                    thisPcType = Piece.FromChar(locString[locString.Length - 1]);
                    promoPiece = new Piece(curPlyNumber % 2 == 1 ? PlayerEnum.White : PlayerEnum.Black, thisPcType);
                    locString = locString.Substring(0, locString.Length - 1); // trim off the promotionidentifier
                }

                // the rest of the move string designates the targetsquare, and potentially a source hint
                // N2d4, Ree6, Re2e6, cd6, cd, cde.p., cd6e.p., d5, e8Q e8=Q e8(Q) potentially with a + or # afterwards
                // move decorators removed above since O-O could be O-O+....

                // find src and dest squares...
                Square TargetSquare = new Square();
                Square SourceSquare = new Square();
                List<Square> options;
                byte rowConstraint = (byte)Square.Rank.NONE;
                byte colConstraint = (byte)Square.File.NONE;
                switch (locString.Length)
                {
                    case 2: // either we have an explicit target square (e6), or an unambiguous P capture (dc)
                        if (Char.IsDigit(locString[1]))
                        {
                            TargetSquare.col = (byte)(locString[0] - 'a');
                            TargetSquare.row = (byte)(locString[1] - '1');
                            options = CurrentPosition.FindPieceWithTarget(curPiece, TargetSquare, Square.Rank.NONE, Square.File.NONE);
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
                            colConstraint = (byte)(locString[0] - 'a');
                            byte targetConstraint = (byte)(locString[1] - 'a');
                            for (int row = 0; row < 8; row++)
                            {
                                TargetSquare = new Square((Square.Rank)row, (Square.File)targetConstraint);
                                if (CurrentPosition.board.ContainsKey(TargetSquare))
                                {
                                    options = CurrentPosition.FindPieceWithTarget(curPiece, TargetSquare, Square.Rank.NONE, (Square.File)colConstraint);
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
                        colConstraint = (byte)(Char.IsLetter(locString[0]) ? (locString[0] - 'a') : (byte)Square.File.NONE);
                        rowConstraint = (byte)(Char.IsDigit(locString[0]) ? (locString[0] - '1') : (byte)Square.Rank.NONE);
                        TargetSquare.col = (byte)(locString[1] - 'a');
                        TargetSquare.row = (byte)(locString[2] - '1');
                        options = CurrentPosition.FindPieceWithTarget(curPiece, TargetSquare, (Square.Rank)rowConstraint, (Square.File)colConstraint);
                        // at this point, there is a restriction on either row or col to validate
                        if (options.Count != 1)
                        {
                            System.Console.WriteLine("couldn't unambiguously process partially constrained move  " + token.tokenString);
                            return null;
                        }

                        curPlyCollection.Add(curPly = new Ply(options[0], TargetSquare, promoPiece));
                        break;
                    case 4:
                        SourceSquare.col = (byte)(locString[0] - 'a');
                        SourceSquare.row = (byte)(locString[1] - '1');
                        TargetSquare.col = (byte)(locString[2] - 'a');
                        TargetSquare.row = (byte)(locString[3] - '1');
                        options = CurrentPosition.FindPieceWithTarget(curPiece, TargetSquare, (Square.Rank)SourceSquare.row, (Square.File)SourceSquare.col);
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

        public void SavePGN(StreamWriter sw)
        {
            int defaultLineTrigger = 90;
            sw.Write(GeneratePGNSource(defaultLineTrigger, GameSaveOptions.IncludeAll));
        }
        public void SaveCompact(StreamWriter sw)
        {
            sw.Write(GenerateCompactSource());
        }
        private List<Ply> HandleVariation(List<PGNToken> var, List<Ply> curPlyCollection, int curPlyNumber)
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
    }
}
