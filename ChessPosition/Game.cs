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
        public PlayerEnum OnMove;
        public string GameDate;
        public string GameRound;
        public string PlayerWhite;
        public string PlayerBlack;
        public int RatingWhite;
        public int RatingBlack;

        public static int Unrated = -1;
        public static int NoRating = -2;

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
                }
                if (!inGameState && outGame != null)
                    return outGame;
            }
            return null;
        }

        public Game()
        {
            CurrentPosition = new Position(Position.StartPosition);
            Plies = new List<Ply>();
            OnMove = PlayerEnum.White;
            RatingBlack = RatingWhite = NoRating;
        }

        public void MakeMove(Ply p)
        {
            // ### advance game state - include castle rights, rep, ep and progress counters
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


        private static List<string> moveDecorators = new List<string>() { "#", "+", "++", "ep", "e.p.", "x" };
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
                    if (castleMarkers.Contains(token))
                    {
                        // src / dest squares should be easy here
                        curPly.Number = curPlyNumber;
                        Square.Rank Krank = curPlyNumber % 2 == 1 ? Square.Rank.R1 : Square.Rank.R8;
                        if (token == castleMarkers[0]) // K-Side
                        {
                            curPly.src = new Square(Krank, Square.File.FE);
                            curPly.dest = new Square(Krank, Square.File.FG);
                        }
                        if (token == castleMarkers[0]) // Q-Side
                        {
                            curPly.src = new Square(Krank, Square.File.FE);
                            curPly.dest = new Square(Krank, Square.File.FC);
                        }
                    }
                    else
                    {
                        string locString = token;
                        Piece curPiece = new Piece(curPlyNumber % 2 == 1 ? PlayerEnum.White : PlayerEnum.Black, Piece.PieceType.Pawn);
                        if (char.IsUpper(token[0]))  // it's a piece designator, other than a P
                        {
                            if (!PieceMapping.ContainsKey(token[0]))
                            {
                                // should handle the error more gracefully than this ...
                                System.Console.WriteLine("Couldn't properly process this move string: " + token);
                                continue;
                            }
                            curPiece.piece = PieceMapping[token[0]];
                            locString = locString.Substring(1); // trim off the piece identifier
                        }
                        // the rest of the move string designates the targetsquare, and potentially a source hint
                        // N2d4, Ree6, Re2e6, cd6, cd, cde.p., cd6e.p., d5, e8Q e8=Q e8(Q) potentially with a + or # afterwards
                        // ### find src and dest squares...

                        for( int i=0; i<moveDecorators.Count; i++ ) 
                        {
                            int thisIndex = locString.IndexOf(moveDecorators[i]);
                            if( thisIndex >= 0 )
                            {
                                // trim off the decorator
                                locString = locString.Substring(0,thisIndex) + locString.Substring(thisIndex+moveDecorators[i].Length);
                                i--;
                            }
                        }
                        Square TargetSquare = new Square();
                        switch (locString.Length)
                        {
                            case 2: // either we have an explicit target square (e6), or an unambiguous P capture (dc)
                                if( Char.IsDigit(locString[1]) )
                                {
                                    TargetSquare.col = (byte)(locString[0] - 'a');
                                    TargetSquare.row = (byte)(locString[1] - '1');
                                }
                                else 
                                {
                                }
                                break;
                            case 3:
                                break;
                            case 4:
                                break;
                        }
                    }

                }
            }
            return true;
        }

    }
}
