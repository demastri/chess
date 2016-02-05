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

        public static int Unrated = -1;
        public static int NoRating = -2;
        
        #endregion

        #region constructors (empty)
        public Game()
        {
            InitGame();
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

        #endregion
    }
}
