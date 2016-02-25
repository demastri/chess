using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessPosition.V2
{
    public class Game
    {
        #region operator Overrides
        /// none
        #endregion

        #region enums and static definitions

        public enum Terminators { WWin = 0, BWin = 1, Draw = 2, InProgress = 3 };
        public static List<string> terminators = new List<string>() { "1-0", "0-1", "1/2-1/2", "*" };
        protected static List<string> moveDecorators = new List<string>() { "#", "+", "++", "ep", "e.p.", "x", "=", "(", ")" };
        protected static List<string> castleMarkers = new List<string>() { "O-O", "O-O-O" };

        public static int Unrated = -1;
        public static int NoRating = -2;

        #endregion

        #region constructors (empty)
        public Game()
        {
            InitGame();
        }
        public Game(Game refGame)
        {
            CopyGame(refGame);
        }

        protected virtual void CopyGame(Game refGame)
        {
            InitGame();

            Plies = refGame.Plies;
            Terminator = refGame.Terminator;
            Tags = refGame.Tags;
        }

        protected virtual void InitGame()
        {
            GameID = Guid.NewGuid();
            Plies = new List<Ply>();
            repetitions = new Dictionary<PositionHash, int>();
            Terminator = Terminators.InProgress;

            Tags = new Dictionary<string, string>();
            Tags.Add("Event", "");  // Seven Tag Roster of mandatory tags for archival work
            Tags.Add("Site", "");
            Tags.Add("Date", "");
            Tags.Add("Round", "");
            Tags.Add("White", "");
            Tags.Add("Black", "");
            Tags.Add("Result", "");

            ResetPosition();
        }

        public void ResetPosition()
        {
            CurrentPosition = new Position(Position.StartPosition);
            OnMove = PlayerEnum.White;
            curPly = 0;
            progressCounter = 0;
        }

        #endregion

        #region properties

        public Guid GameID { get; set; }
        public Dictionary<string, string> Tags { get; set; }
        public List<Ply> Plies { get; set; }
        public Terminators Terminator { get; set; }
        public string TerminatorString { get { return terminators[(int)Terminator]; } }

        public int RatingWhite { get { return Convert.ToInt32((Tags.ContainsKey("WhiteElo") ? Tags["WhiteElo"] : (Tags.ContainsKey("WhiteUSCF") ? Tags["WhiteUSCF"] : NoRating.ToString()))); } }
        public int RatingBlack { get { return Convert.ToInt32((Tags.ContainsKey("BlackElo") ? Tags["BlackElo"] : (Tags.ContainsKey("BlackUSCF") ? Tags["BlackUSCF"] : NoRating.ToString()))); } }
        public string PlayerWhite { get { return Tags.ContainsKey("White") ? Tags["White"] : ""; } }
        public string PlayerBlack { get { return Tags.ContainsKey("Black") ? Tags["Black"] : ""; } }

        // actual internal structures needed by the game "walking" - current position, etc
        public Position CurrentPosition;
        public PlayerEnum OnMove { get { return CurrentPosition.onMove; } set { CurrentPosition.onMove = value; } }
        public int curPly;
        public bool EndOfGame { get { return curPly >= Plies.Count; } }

        // structures needed for FEN translation
        private Dictionary<PositionHash, int> repetitions { get; set; }
        private int progressCounter { get; set; }

        #endregion

        #region string/char conversions

        public string ToFEN()
        {
            return CurrentPosition.ToFEN(progressCounter, curPly / 2 + 1);
        }

        #endregion

        #region domain logic

        public bool AdvanceTo(int plyNbr)
        {
            ResetPosition();
            return AdvancePosition(plyNbr);
        }
        public bool AdvancePosition()
        {
            return AdvancePosition(1);
        }
        public bool AdvancePosition(int nbrAhead)
        {
            if (nbrAhead < 0)
                return BackPosition(-nbrAhead);

            /// advance game state - include castle rights, rep, ep and progress counters
            /// note the newly arrived at position in the repetition count ### test
            /// update the progress counters as appropriate
            for (int i = 0; i < nbrAhead; i++)
            {
                if (curPly >= Plies.Count)
                    return false;
                int prePcCount = CurrentPosition.board.Count;
                bool wasApawn = CurrentPosition.board[Plies.ElementAt(curPly).src].piece == Piece.PieceType.Pawn;
                CurrentPosition.MakeMove(Plies.ElementAt(curPly++));
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
        public bool BackPosition()
        {
            return BackPosition(1);
        }
        public bool BackPosition(int nbrBack)
        {
            if (nbrBack < 0)
                return AdvancePosition(-nbrBack);

            // hack... something weird happened on a takeback - a neighboring P can disappear
            // some kind of goofy ep thing, think it was related to not setting OnMove properly on a reset
            // so simply reset, then advance to the target...
            int targetPly = curPly - nbrBack;
            if (targetPly < 0)
                targetPly = 0;
            ResetPosition();
            return AdvancePosition(targetPly);
        }
        public Ply CreateMove(Square src, Square dest)
        {
            Ply outPly = new Ply(src, dest);

            Piece thisPc = CurrentPosition.PieceAt(src);
            Piece capPc = CurrentPosition.PieceAt(dest);

            if (thisPc == null || thisPc.color != OnMove || (capPc != null && capPc.color == OnMove))
                return null;
            List<Square> options = CurrentPosition.FindPieceWithTarget(thisPc, dest, src);
            if (options.Count != 1)
                return null;
            // at this point, thisPc is the right color, and could move from src->dest
            // Fully defined piece text = (Piece desig)(constrain row)(constrain col)(capture flag)(dest sq)

            return outPly;
        }
        public void AddMoveToPly(string sanMove, Ply outPly)
        {
            int curPlyNumber = curPly + 1;
            int curMoveNumber = (curPlyNumber - 1) / 2;
            PlayerEnum curPlayer = (curPlyNumber % 2 == 1 ? PlayerEnum.White : PlayerEnum.Black);
            Square.Rank Krank = (curPlayer == PlayerEnum.White ? Square.Rank.R1 : Square.Rank.R8);

            string locString = sanMove.Trim();

            for (int i = 0; i < moveDecorators.Count; i++)
                locString = locString.Replace(moveDecorators[i], "");

            if (castleMarkers.Contains(locString))
            {
                // src / dest squares should be easy here
                if (locString == castleMarkers[0]) // K-Side
                {
                    outPly.src = new Square(Krank, Square.File.FE);
                    outPly.dest = new Square(Krank, Square.File.FG);
                }
                if (locString == castleMarkers[1]) // Q-Side
                {
                    outPly.src = new Square(Krank, Square.File.FE);
                    outPly.dest = new Square(Krank, Square.File.FC);
                }
                return; // move is complete - return
            }
            else
            {
                Piece.PieceType movePcType = Piece.PieceType.Pawn;
                Piece.PieceType promoPcType = Piece.PieceType.Invalid;

                // it's a piece designator, other than a P
                if (char.IsUpper(locString[0]))
                {
                    movePcType = Piece.FromChar(locString[0]);
                    if (movePcType == Piece.PieceType.Invalid)
                    {
                        // should handle the error more gracefully than this ...
                        System.Console.WriteLine("Couldn't properly process this move string: " + sanMove);
                        return;
                    }
                    locString = locString.Substring(1); // trim off the piece identifier
                }

                // set up the piece we're working with
                Piece curPiece = Piece.PieceFactory(curPlayer, movePcType);
                Piece promoPiece = null;

                if (curPiece.piece == Piece.PieceType.Pawn && char.IsUpper(locString[locString.Length - 1]))   // it's a promotion designator...
                {
                    promoPcType = Piece.FromChar(locString[locString.Length - 1]);
                    promoPiece = Piece.PieceFactory(curPlayer, promoPcType);
                    locString = locString.Substring(0, locString.Length - 1); // trim off the promotionidentifier
                    outPly.promo = promoPiece;
                }

                // the rest of the move string designates the targetsquare, and potentially a source hint
                // N2d4, Ree6, Re2e6, cd6, cd, cde.p., cd6e.p., d5, e8Q e8=Q e8(Q) potentially with a + or # afterwards
                // move decorators removed above since O-O could be O-O+....
                // so the resulting string can only be [optional src file][optional src rank][dest file][optional (for pawn caps) dest rank]


                // find src and dest squares...
                Square TargetSquare = new Square();
                Square SourceSquare = new Square();
                List<Square> options;
                Square constraint = new Square(Square.Rank.NONE, Square.File.NONE);
                switch (locString.Length)
                {
                    case 2: // either we have an explicit target square (e6), or an unambiguous P capture (dc)
                        if (Char.IsDigit(locString[1])) // ok - target is specified
                        {
                            TargetSquare.file = (Square.File)(locString[0] - 'a');
                            TargetSquare.rank = (Square.Rank)(locString[1] - '1');
                            options = CurrentPosition.FindPieceWithTarget(curPiece, TargetSquare, constraint);
                            if (options.Count != 1)
                            {
                                System.Console.WriteLine("couldn't unambiguously process move <no pc for target> " + sanMove);
                                return;
                            }
                            outPly.src = new Square(options[0]);
                            outPly.dest = new Square(TargetSquare);
                            return;
                        }
                        else
                        {
                            // this has to be a file/file pawn capture
                            if (curPiece.piece != Piece.PieceType.Pawn)
                            {
                                System.Console.WriteLine("couldn't unambiguously process move <not P for col capture> " + sanMove);
                                return;
                            }

                            // cd -> how to id the source and target squares??  certainly both squares are col constrained
                            // the source square is constrained but how to constrain the targets?
                            // it's a capture on that file - we can start by listing all occupied squares on that file
                            // any source constrained returns should be unambiguously correct...
                            // if we could take more than one piece with a pawn on that file from this one it would need another qualifier

                            // ### want to write:
                            SourceSquare = new Square(Square.Rank.NONE, (Square.File)(locString[0] - 'a'));
                            constraint = new Square(Square.Rank.NONE, (Square.File)(locString[1] - 'a'));
                            options = CurrentPosition.FindPieceWithTarget(curPiece, constraint, SourceSquare);
                            // and this should be constrained in an appropriate way
                            if (options.Count != 1)
                            {
                                System.Console.WriteLine("couldn't unambiguously process move <col capture>  " + sanMove);
                                return;
                            }
                            constraint.rank = options[0].rank + (curPlayer == PlayerEnum.White ? 1 : -1);
                            outPly.src = new Square(options[0]);
                            outPly.dest = new Square(constraint);
                            // ###
                        }

                        break;
                    case 3:
                        constraint.file = (Char.IsLetter(locString[0]) ? (Square.File)(locString[0] - 'a') : Square.File.NONE);
                        constraint.rank = (Char.IsDigit(locString[0]) ? (Square.Rank)(locString[0] - '1') : Square.Rank.NONE);
                        TargetSquare = new Square(
                            (Square.Rank)(locString[2] - '1'),
                            (Square.File)(locString[1] - 'a'));
                        options = CurrentPosition.FindPieceWithTarget(curPiece, TargetSquare, constraint);
                        // at this point, there is a restriction on either row or col to validate
                        if (options.Count != 1)
                        {
                            System.Console.WriteLine("couldn't unambiguously process partially constrained move  " + sanMove);
                            return;
                        }
                        outPly.src = new Square(options[0]);
                        outPly.dest = new Square(TargetSquare);
                        break;
                    case 4:
                        SourceSquare = new Square(
                            (Square.Rank)(locString[1] - '1'),
                            (Square.File)(locString[0] - 'a'));
                        TargetSquare = new Square(
                            (Square.Rank)(locString[3] - '1'),
                            (Square.File)(locString[2] - 'a'));
                        options = CurrentPosition.FindPieceWithTarget(curPiece, TargetSquare, SourceSquare);
                        if (!options.Contains(SourceSquare) || options.Count != 1)
                        {
                            System.Console.WriteLine("couldn't find specified piece for move  " + sanMove);
                            return;
                        }
                        outPly.src = new Square(SourceSquare);
                        outPly.dest = new Square(TargetSquare);
                        break;
                }
            }
            return;
        }

        #endregion
    }
}
