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
        public List<Ply> Plies;
        public Position CurrentPosition;
        public PlayerEnum OnMove;
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

        public static Game ReadGame(StreamReader r)
        {
            // this pulls a text based PGN game from a stream
            // separates it into a number of plies (or half moves)
            // keeps some notional information (ratings for now)
            // initializes game state - onMove, position
            Game outGame = null;
            bool inGameState = false;

            while (!r.EndOfStream)
            {
                string l = r.ReadLine().Trim();
                if (!inGameState)   // see if this is the start of a new game!
                {
                    if (l.Length > 0 && (l[0] == '[' || Char.IsDigit(l[0])))
                    {
                        outGame = new Game();
                        inGameState = true;
                    }
                }
                if (inGameState)   // process the current data line (close when needed...)
                {
                    if (l.Length == 0)
                        continue;
                    // read the current line - init the plies and players
                    if (l[0] == '[')
                        outGame.HandleTag(l);
                    // set inGameState as appropriate if EOG
                    if (Char.IsDigit(l[0]))
                        inGameState = outGame.HandleMoves(l);
                    outGame.PGNSource += l + Environment.NewLine;
                }
                if (!inGameState && outGame != null)
                {
                    outGame.ResetPosition();
                    return outGame;
                }
            }
            return null;
        }

        public Game()
        {
            Plies = new List<Ply>();
            repetitions = new Dictionary<PositionHash, int>();

            ResetPosition();
            PGNSource = "";
            RatingBlack = RatingWhite = NoRating;
        }

        public void ResetPosition()
        {
            OnMove = PlayerEnum.White;
            CurrentPosition = new Position(Position.StartPosition);
            curPly = 0;
            progressCounter = 0;
        }
        public bool AdvancePosition()
        {
            /// advance game state - include castle rights, rep, ep and progress counters
            /// note the newly arrived at position in the repetition count ### test
            /// update the progress counters as appropriate
            if (curPly < Plies.Count)
            {
                int prePcCount = CurrentPosition.board.Count;
                bool wasApawn = CurrentPosition.board[Plies[curPly].src].piece == Piece.PieceType.Pawn;
                CurrentPosition.MakeMove(Plies[curPly++], ref OnMove);
                bool wasAcapture = prePcCount != CurrentPosition.board.Count; // it was a capture
                if ( wasApawn || wasAcapture )
                    progressCounter = 0;
                else
                    progressCounter++;
                PositionHash thisHash = new PositionHash( CurrentPosition);
                if (repetitions.ContainsKey(thisHash))
                    repetitions[thisHash]++;
                else
                    repetitions.Add(thisHash, 0);
                return true;
            }
            return false;
        }
        public bool EndOfGame { get { return curPly >= Plies.Count; } }
        public void BackPosition()
        {
            // hack... something weird happened on a takeback - a neighboring P can disappear
            // some kind of goofy ep thing, think it was related to not setting OnMove properly on a reset
            int targetPly = curPly-1;
            ResetPosition();
            for (int i = 0; i < targetPly; i++)
                AdvancePosition();
        }

        private static List<string> ratingTags = new List<string>() { "WhiteElo", "BlackElo", "WhiteUSCF", "BlackUSCF" };
        private void HandleTag(string s)
        {
            if (s[0] != '[' || s[s.Length - 1] != ']')
                return;
            s = s.Substring(1, s.Length - 2);

            string tag = s.Substring(0, s.IndexOf(' '));
            int firstQuote = s.IndexOf('"');
            int secondQuote = s.IndexOf('"', firstQuote + 1);
            string val = s.Substring(firstQuote + 1, secondQuote - firstQuote - 1);
            // so far supports ratings only - placeholders for the STR (Seven Tag Roster) and two ratings sources
            switch (tag)
            {
                case "Event":
                    break;
                case "Site":
                    break;
                case "Date":
                    GameDate = val;
                    break;
                case "Round":
                    GameRound = val;
                    break;
                case "White":
                    PlayerWhite = val;
                    break;
                case "Black":
                    PlayerBlack = val;
                    break;
                case "Result":
                    break;
                case "WhiteElo":
                    RatingWhite = (val == "" ? NoRating : (val == "-" ? Unrated : Convert.ToInt32(val)));
                    break;
                case "BlackElo":
                    RatingBlack = (val == "" ? NoRating : (val == "-" ? Unrated : Convert.ToInt32(val)));
                    break;
                case "WhiteUSCF":
                    RatingWhite = Convert.ToInt32(val);
                    break;
                case "BlackUSCF":
                    RatingBlack = Convert.ToInt32(val);
                    break;
            }
        }

        public string ToFEN()
        {
            return CurrentPosition.ToFEN(OnMove, CurrentPosition.castleRights, CurrentPosition.epLoc, progressCounter, curPly/2+1);
        }


        private static List<string> moveDecorators = new List<string>() { "#", "+", "++", "ep", "e.p.", "x", "=", "(", ")" };
        private static List<string> endMarkers = new List<string>() { "1-0", "0-1", "1/2-1/2" };
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

        private bool HandleMoves(string s)
        {
            int curMoveNumber = 1;
            int curPlyNumber = 1;
            string[] tokens = s.Split(new char[] { '.', ' ' });
            foreach (string token in tokens)
            {
                if (token.Trim() == "")
                    continue;
                Ply curPly = new Ply();
                if (endMarkers.Contains(token))
                    return false;
                if (Char.IsDigit(token[0]))  // move number
                {
                    curMoveNumber = Convert.ToInt32(token);
                    curPlyNumber = (curMoveNumber - 1) * 2 + 1;
                }
                else // it's a move notation, start with short algebraic...
                {
                    string locString = token;

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
                                continue;
                            }
                            curPiece.piece = PieceMapping[locString[0]];
                            locString = locString.Substring(1); // trim off the piece identifier
                        }

                        if( char.IsUpper( locString[ locString.Length-1 ] ) )   // it's a promotion designator...
                        {
                            promoPiece = new Piece(curPlyNumber % 2 == 1 ? PlayerEnum.White : PlayerEnum.Black, PieceMapping[locString[locString.Length - 1]]);
                            locString = locString.Substring( 0, locString.Length-1 ); // trim off the promotionidentifier
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
                    CurrentPosition.MakeMove(curPly, ref OnMove);
                    curPlyNumber++;
                }
            }
            return true;
        }
    }
}
