using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ChessPosition
{
    public class Game
    {
        public List<Ply> Plies;
        public Position CurrentPosition;
        public PlayerEnum OnMove { get { return CurrentPosition.onMove; } set { CurrentPosition.onMove = value; } }
        public string GameDate;
        public string GameRound;
        public string PlayerWhite;
        public string PlayerBlack;
        public int RatingWhite;
        public int RatingBlack;

        public string PGNSource;

        public static int Unrated = -1;
        public static int NoRating = -2;

        public int curPly;
        // these values are NOT included in the hash, since they're defined here...
        Dictionary<PositionHash, int> repetitions;
        private int progressCounter;

        public static List<Game> ReadPGNFile(string PGNFileLoc)
        {
            List<Game> GameRef = new List<Game>();
            if (PGNFileLoc != "" && File.Exists(PGNFileLoc))
            {
                StreamReader tr = new StreamReader(PGNFileLoc);
                GameRef = new List<Game>();

                try
                {
                    while (true)
                        GameRef.Add(new Game(tr));
                }
                catch (InvalidOperationException e)
                {
                    ;// ok, eof likely
                }
            }
            return GameRef;
        }

        public Game(StreamReader r) // read from PGN stream
        {
            PGNGame nextTokenSet = new PGNGame(r);
            if (nextTokenSet.tokens.Count > 0) // ok, we have one
            {
                InitGame();
                PGNSource = nextTokenSet.pgn;
                foreach (PGNGame.PGNToken token in nextTokenSet.tokens)
                {
                    switch (token.tokenType)
                    {
                        case PGNGame.TokenType.Tag:
                            HandleTag((PGNGame.Tag)token);
                            break;
                        case PGNGame.TokenType.Invalid:
                        case PGNGame.TokenType.MoveNumber:
                        case PGNGame.TokenType.Comment:
                        case PGNGame.TokenType.Terminator:
                            // these can be skipped
                            break;
                        case PGNGame.TokenType.MoveString:
                            HandleMove((PGNGame.MoveString)token);
                            break;
                    }
                    if (token.tokenType == PGNGame.TokenType.Terminator)
                    {
                        ResetPosition();
                        return;
                    }
                }
            }
            throw new InvalidOperationException();
        }

        public Game()
        {
            InitGame();
        }
        private void InitGame()
        {
            Plies = new List<Ply>();
            repetitions = new Dictionary<PositionHash, int>();

            ResetPosition();
            PGNSource = "";
            RatingBlack = RatingWhite = NoRating;
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

        private void HandleTag(PGNGame.Tag t)
        {
            switch (t.key)
            {
                case "Event":
                    break;
                case "Site":
                    break;
                case "Date":
                    GameDate = t.value;
                    break;
                case "Round":
                    GameRound = t.value;
                    break;
                case "White":
                    PlayerWhite = t.value;
                    break;
                case "Black":
                    PlayerBlack = t.value;
                    break;
                case "Result":
                    break;
                case "WhiteElo":
                    RatingWhite = (t.value == "" ? NoRating : (t.value == "-" ? Unrated : Convert.ToInt32(t.value)));
                    break;
                case "BlackElo":
                    RatingBlack = (t.value == "" ? NoRating : (t.value == "-" ? Unrated : Convert.ToInt32(t.value)));
                    break;
                case "WhiteUSCF":
                    RatingWhite = Convert.ToInt32(t.value);
                    break;
                case "BlackUSCF":
                    RatingBlack = Convert.ToInt32(t.value);
                    break;
            }
        }

        public string ToFEN()
        {
            return CurrentPosition.ToFEN(progressCounter, curPly / 2 + 1);
        }


        private static List<string> moveDecorators = new List<string>() { "#", "+", "++", "ep", "e.p.", "x", "=", "(", ")" };
        private static List<string> endMarkers = new List<string>() { "1-0", "0-1", "1/2-1/2", "*" };
        private static List<string> gapMarkers = new List<string>() { "{}", "," };
        private static List<string> castleMarkers = new List<string>() { "O-O", "O-O-O" };
        private static Dictionary<char, Piece.PieceType> PieceMapping = new Dictionary<char, Piece.PieceType>()
        {
            { 'R', Piece.PieceType.Rook },
            { 'N', Piece.PieceType.Knight },
            { 'B', Piece.PieceType.Bishop },
            { 'Q', Piece.PieceType.Queen },
            { 'K', Piece.PieceType.King },
            { 'P', Piece.PieceType.Pawn }
        };

        private void HandleMove(PGNGame.MoveString token)
        {
            int curPlyNumber = Plies.Count + 1;
            int curMoveNumber = (curPlyNumber - 1) / 2;

            string locString = token.value;

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

            Ply curPly = new Ply();
            if (castleMarkers.Contains(locString))
            {
                // src / dest squares should be easy here
                curPly.Number = curPlyNumber;
                Square.Rank Krank = curPlyNumber % 2 == 1 ? Square.Rank.R1 : Square.Rank.R8;
                if (locString == castleMarkers[0]) // K-Side
                {
                    curPly.src = new Square(Krank, Square.File.FE);
                    curPly.dest = new Square(Krank, Square.File.FG);
                    Plies.Add(curPly);
                }
                if (locString == castleMarkers[1]) // Q-Side
                {
                    curPly.src = new Square(Krank, Square.File.FE);
                    curPly.dest = new Square(Krank, Square.File.FC);
                    Plies.Add(curPly);
                }
            }
            else
            {
                Piece curPiece = new Piece(curPlyNumber % 2 == 1 ? PlayerEnum.White : PlayerEnum.Black, Piece.PieceType.Pawn);
                Piece promoPiece = null;

                if (char.IsUpper(locString[0]))  // it's a piece designator, other than a P
                {
                    if (!PieceMapping.ContainsKey(locString[0]))
                    {
                        // should handle the error more gracefully than this ...
                        System.Console.WriteLine("Couldn't properly process this move string: " + token);
                        return;
                    }
                    curPiece.piece = PieceMapping[locString[0]];
                    locString = locString.Substring(1); // trim off the piece identifier
                }

                if (char.IsUpper(locString[locString.Length - 1]))   // it's a promotion designator...
                {
                    promoPiece = new Piece(curPlyNumber % 2 == 1 ? PlayerEnum.White : PlayerEnum.Black, PieceMapping[locString[locString.Length - 1]]);
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
                                System.Console.WriteLine("couldn't unambiguously process move <no pc for target> " + token);
                            Plies.Add(curPly = new Ply(options[0], TargetSquare, promoPiece));
                        }
                        else
                        {
                            if (curPiece.piece != Piece.PieceType.Pawn)
                                System.Console.WriteLine("couldn't unambiguously process move <not P for col capture> " + token);
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
                                        System.Console.WriteLine("couldn't unambiguously process move <col capture>  " + token);
                                    if (options.Count == 1)
                                        SourceSquare = options[0];
                                }
                            }
                            if (SourceSquare == null)
                                System.Console.WriteLine("couldn't unambiguously process move <col capture>  " + token);
                            else
                                Plies.Add(curPly = new Ply(SourceSquare, TargetSquare, promoPiece));
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
                            System.Console.WriteLine("couldn't unambiguously process partially constrained move  " + token);
                        Plies.Add(curPly = new Ply(options[0], TargetSquare, promoPiece));
                        break;
                    case 4:
                        SourceSquare.col = (byte)(locString[0] - 'a');
                        SourceSquare.row = (byte)(locString[1] - '1');
                        TargetSquare.col = (byte)(locString[2] - 'a');
                        TargetSquare.row = (byte)(locString[3] - '1');
                        options = CurrentPosition.FindPieceWithTarget(curPiece, TargetSquare, (Square.Rank)SourceSquare.row, (Square.File)SourceSquare.col);
                        if (!options.Contains(SourceSquare) || options.Count != 1)
                            System.Console.WriteLine("couldn't find specified piece for move  " + token);
                        Plies.Add(curPly = new Ply(SourceSquare, TargetSquare, promoPiece));
                        break;
                }
            }
            CurrentPosition.MakeMove(curPly);
            curPlyNumber++;
        }
    }
}
