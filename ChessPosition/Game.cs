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

        public string PGNSource;

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
                    outGame.PGNSource += l + Environment.NewLine;
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
            PGNSource = "";
            RatingBlack = RatingWhite = NoRating;
        }

        public void MakeMove(Ply p)
        {
            /// advance game state - include castle rights, rep, ep and progress counters
            /// 
            /// in most cases this is simply move the piece from the src sq to the dest sq
            /// if there's a piece on the target square, it needs to be removed
            /// ### if the piece is a K or R, update castle rights (and castle if appropriate)
            /// ### if the piece is a P, check for both an ep capture on this move and update ep state
            /// ### note the newly arrived at position in the repetition count
            /// ### update the progress counters as appropriate

            Piece srcPiece = CurrentPosition.board[p.src];
            Piece destPiece = CurrentPosition.board.ContainsKey(p.dest) ? CurrentPosition.board[p.dest] : null;

            if (OnMove != srcPiece.color)
                System.Console.WriteLine("Moving the wrong color piece...");
            if (destPiece != null && OnMove == srcPiece.color)
                System.Console.WriteLine("Taking the wrong color piece...");
            
            CurrentPosition.board.Remove(p.src);
            CurrentPosition.board[p.dest] = srcPiece;

            if (srcPiece.piece == Piece.PieceType.King)
            {
                if (srcPiece.color == PlayerEnum.White)
                {
                    CurrentPosition.castleRights = (byte)(CurrentPosition.castleRights & ~(byte)Position.CastleRights.KS_White);
                    CurrentPosition.castleRights = (byte)(CurrentPosition.castleRights & ~(byte)Position.CastleRights.QS_White);
                }
                else
                {
                    CurrentPosition.castleRights = (byte)(CurrentPosition.castleRights & ~(byte)Position.CastleRights.KS_Black);
                    CurrentPosition.castleRights = (byte)(CurrentPosition.castleRights & ~(byte)Position.CastleRights.QS_Black);
                }
                if (Math.Abs(p.src.col - p.dest.col) == 2)  // castling...
                {
                    Square.Rank rank = srcPiece.color == PlayerEnum.White ? Square.Rank.R1 : Square.Rank.R8;
                    Square.File file = p.dest.col == (byte)Square.File.FC ? Square.File.FA : Square.File.FH;
                    Piece castleRook = null;
                    if( CurrentPosition.board.ContainsKey( new Square( rank, Square.File.FA ) ) ) 
                    {
                        castleRook = CurrentPosition.board[new Square(rank, Square.File.FA)];
                    }
                }
            }
            if (srcPiece.piece == Piece.PieceType.Rook)
            {
            }
            if (srcPiece.piece == Piece.PieceType.Pawn)
            {
            }

            OnMove = (OnMove == PlayerEnum.White ? PlayerEnum.Black : PlayerEnum.White);
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
                            Plies.Add(curPly);
                        }
                        if (token == castleMarkers[1]) // Q-Side
                        {
                            curPly.src = new Square(Krank, Square.File.FE);
                            curPly.dest = new Square(Krank, Square.File.FC);
                            Plies.Add(curPly);
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

                                    Plies.Add(curPly = new Ply(options[0], TargetSquare));
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
                                            if (options.Count > 1 || (options.Count>0 && SourceSquare != null) )
                                                System.Console.WriteLine("couldn't unambiguously process move <col capture>  " + token);
                                            if (options.Count == 1)
                                                SourceSquare = options[0];
                                        }
                                    }
                                    if( SourceSquare == null )
                                        System.Console.WriteLine("couldn't unambiguously process move <col capture>  " + token);
                                    else
                                        Plies.Add(curPly=new Ply(SourceSquare, TargetSquare));
                                }
                                break;
                            case 3:
                                rowConstraint = (byte)(Char.IsLetter(locString[0]) ? (locString[0] - '1') : (byte)Square.Rank.NONE);
                                colConstraint = (byte)(Char.IsDigit(locString[0]) ? (locString[0] - 'a') : (byte)Square.File.NONE);
                                TargetSquare.col = (byte)(locString[1] - 'a');
                                TargetSquare.row = (byte)(locString[2] - '1');
                                options = CurrentPosition.FindPieceWithTarget(curPiece, TargetSquare, (Square.Rank)rowConstraint, (Square.File)colConstraint);
                                // at this point, there is a restriction on either row or col to validate
                                if (options.Count != 1)
                                    System.Console.WriteLine("couldn't unambiguously process partially constrained move  " + token);
                                Plies.Add(curPly=new Ply(options[0], TargetSquare));
                                break;
                            case 4:
                                SourceSquare.col = (byte)(locString[0] - 'a');
                                SourceSquare.row = (byte)(locString[1] - '1');
                                TargetSquare.col = (byte)(locString[2] - 'a');
                                TargetSquare.row = (byte)(locString[3] - '1');
                                options = CurrentPosition.FindPieceWithTarget(curPiece, TargetSquare, (Square.Rank)SourceSquare.row, (Square.File)SourceSquare.col);
                                if( !options.Contains( SourceSquare ) || options.Count != 1 )
                                    System.Console.WriteLine("couldn't find specified piece for move  " + token);
                                Plies.Add(curPly=new Ply(SourceSquare, TargetSquare));
                                break;
                        }
                    }

                    MakeMove(curPly);
                    curPlyNumber++;
                }
            }
            return true;
        }
    }
}
