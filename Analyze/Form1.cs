using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using ChessPosition;

namespace Analyze
{
    public partial class AnalyzeForm : Form
    {
        List<Game> GameRef;

        Dictionary<PositionHash, Analysis> totalAnalysis;
        Dictionary<PositionHash, int> repetitionCount;
        Engine AnalysisEngine = null;
        int curAnalysisPositionIndex = -1;
        DateTime curAnalysisStartTime;
        TimeSpan posAnalysisTimeLimit;
        bool initAnalysis;

        public AnalyzeForm()
        {
            InitializeComponent();
        }

        private void OpenMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string PGNFileLoc = openFileDialog1.FileName;
                PGNLoc.Text = PGNFileLoc;
                GameRef = Game.ReadPGNFile(PGNFileLoc);
                FileStatsButton_Click(null, null);
            }
            else
            {
                PGNLoc.Text = "";
                TotalGamesLabel.Text = "Total Games: ---";
                TotalRatedLabel.Text = "Total Rated: ---";
                TotalPliesLabel.Text = "Total Plies: ---";
                UniquePosLabel.Text = "Unique Pos: ---";
            }
        }

        int completedAnalysisRequests;
        AnalysisFarmClient thisFarmClient = null;

        AnalysisFarm thisFarm = null;

        private void AnalysisButton_Click(object sender, EventArgs e)
        {
            if (UseFarm.Checked)
                AnalyzeViaFarm();
            else
                AnalyzeViaEngine();
        }
        DateTime farmStartTime;
        private void AnalyzeViaFarm()
        {
            if (thisFarm != null)
            {
                thisFarm.Shutdown();
                thisFarm = null;
            }
            if (thisFarm == null)
            {
                thisFarm = new AnalysisFarm(Convert.ToInt32(nbrEngines.Text), this);
                thisFarm.Start();
            }
            farmStartTime = DateTime.Now;
            totalAnalysis = new Dictionary<PositionHash, Analysis>();

            completedAnalysisRequests = 0;
            AnalysisIndexLabel.Text = "Current Analysis Index: 0";

            string EngineName = "Stockfish";
            EngineParameters eParams = new EngineParameters(EngineName, 20, -1);

            thisFarmClient = new AnalysisFarmClient(this);

            // thisFarm.AnalysisUpdateEvent += AnalysisEngine_AnalysisUpdate;   // probably not needed for this app...
            thisFarmClient.AnalysisCompleteEvent += AnalysisEngine_AnalysisCompleteV2;

            foreach (PositionHash ph in repetitionCount.Keys)
                thisFarmClient.SubmitAnalysisRequest(eParams, ph.Rehydrate().ToFEN(1,1));
        }
        void AnalysisEngine_AnalysisCompleteV2(Analysis a)  // needs to refer to the actual analysis request...
        {
            StoreAnalysis(a);
            TimeSpan runTime = DateTime.Now - farmStartTime;
            UpdateLabelText( "Cur Index: " + (curAnalysisPositionIndex=a.AnalysisID).ToString() + " Ttl Rtnd: " + (++completedAnalysisRequests).ToString() + " Run Time: "+runTime.ToString());
        }
        delegate void SetTextCallback(string s);
        private void UpdateLabelText(string s)
        {
            if (AnalysisIndexLabel.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(UpdateLabelText);
                Invoke(d, new object[] { s });
            }
            else
            {
                AnalysisIndexLabel.Text = s;
            }
        }

        private void AnalyzeViaEngine()
        {
            string EngineName = "Stockfish";
            AnalysisEngine = Engine.InitEngine(EngineName);

            AnalysisEngine.AnalysisUpdateEvent += AnalysisEngine_AnalysisUpdate;
            AnalysisEngine.AnalysisCompleteEvent += AnalysisEngine_AnalysisComplete;

            totalAnalysis = new Dictionary<PositionHash, Analysis>();

            initAnalysis = true;
        }
        void AnalysisEngine_AnalysisComplete(int thisID)
        {
            if (AnalysisEngine != null && AnalysisEngine.curAnalysis != null)
                lastAnalysisString.Text = AnalysisEngine.curAnalysis.ToString();
            else
                if (AnalysisEngine != null)
                    lastAnalysisString.Text = AnalysisEngine.lastEngineReply;
        }
        void AnalysisEngine_AnalysisUpdate(int thisID)
        {
            if (AnalysisEngine != null && AnalysisEngine.curAnalysis != null)
                lastAnalysisString.Text = AnalysisEngine.curAnalysis.ToString();
            else
                if (AnalysisEngine != null)
                    lastAnalysisString.Text = AnalysisEngine.lastEngineReply;

            if (AnalysisEngine != null && AnalysisEngine.AnalysisComplete())
                ;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (UseFarm.Checked)
            {
                if (completedAnalysisRequests == repetitionCount.Count && curAnalysisPositionIndex != -1)
                {
                    AnalyzeDeltas();
                    curAnalysisPositionIndex = -1;
                    thisFarm.Shutdown();
                    thisFarmClient.Quit();
                    thisFarmClient = null;
                }
                return;
            }

            if (initAnalysis)
                curAnalysisPositionIndex = -1;

            if (AnalysisEngine != null)
                AnalysisEngine.CheckProgress();
            TimeSpan curAnalysisTime = DateTime.Now - curAnalysisStartTime;
            if (initAnalysis || (curAnalysisPositionIndex >= 0 &&
                (/*curAnalysisTime > posAnalysisTimeLimit ||*/ AnalysisEngine.AnalysisComplete())))
            {
                if (AnalysisEngine.curAnalysis != null && AnalysisEngine.curAnalysis.isComplete)
                    System.Console.WriteLine("ran to the end!");
                if (!initAnalysis)
                    StoreAnalysis(AnalysisEngine.curAnalysis);
                if (++curAnalysisPositionIndex < repetitionCount.Count)
                    StartAnalysisAsync(repetitionCount.ElementAt(curAnalysisPositionIndex).Key.Rehydrate());
                else
                {
                    AnalyzeDeltas();
                    curAnalysisPositionIndex = -1;
                }
            }
            AnalysisIndexLabel.Text = "Current Analysis Index: " + curAnalysisPositionIndex.ToString();
            initAnalysis = false;
        }

        private void FileStatsButton_Click(object sender, EventArgs e)
        {
            // how many games
            int totalGames = 0;
            int ratedGames = 0;
            int totalPlies = 0;
            foreach (Game g in GameRef)
            {
                // how many games
                totalGames++;
                // how many rated (class?)
                if (g.RatingWhite != Game.NoRating && g.RatingBlack != Game.NoRating)
                    ratedGames++;
                // how many plies
                totalPlies += g.Plies.Count;
            }
            // how many unique positions
            repetitionCount = IdentifyUniquePositions();

            TotalGamesLabel.Text = "Total Games: " + totalGames.ToString();
            TotalRatedLabel.Text = "Total Rated: " + ratedGames.ToString();
            TotalPliesLabel.Text = "Total Plies: " + totalPlies.ToString();
            UniquePosLabel.Text = "Unique Pos: " + repetitionCount.Count.ToString();
        }
        private Dictionary<PositionHash, int> IdentifyUniquePositions()
        {
            Game refStart = new Game();
            string refFEN = refStart.ToFEN();
            PositionHash refHash = new PositionHash(refStart.CurrentPosition);
            int refHashCode = refHash.GetHashCode();

            Dictionary<PositionHash, int> outCount = new Dictionary<PositionHash, int>();
            foreach (Game g in GameRef)
            {
                g.ResetPosition();
                PositionHash ph = new PositionHash(g.CurrentPosition);

                string thisFEN = g.ToFEN();
                PositionHash thisHash = new PositionHash(g.CurrentPosition);
                int thisHashCode = thisHash.GetHashCode();

                if (thisFEN != refFEN)
                    Console.WriteLine("FEN Mismatch");
                if (thisHash != refHash)
                    Console.WriteLine("Hash Mismatch");
                if (thisHashCode != refHashCode)
                    Console.WriteLine("HashCode Mismatch");


                if (outCount.ContainsKey(ph))
                    outCount[ph]++;
                else
                    outCount.Add(ph, 1);

                foreach (Ply p in g.Plies)
                {
                    g.AdvancePosition();
                    ph = new PositionHash(g.CurrentPosition);
                    if (outCount.ContainsKey(ph))
                        outCount[ph]++;
                    else
                        outCount.Add(ph, 1);
                }
            }
            return outCount;
        }
        private void StartAnalysisAsync(Position p)
        {
            curAnalysisStartTime = DateTime.Now;
            AnalysisEngine.CheckProgress();
            AnalysisEngine.StartAnalysis(p);
        }

        private void AnalyzeForm_Load(object sender, EventArgs e)
        {
            curAnalysisPositionIndex = -1;
            AnalysisEngine = null;
            initAnalysis = false;
            curAnalysisStartTime = DateTime.Now;
            posAnalysisTimeLimit = new TimeSpan(0, 0, 0, 0, 2000);
            timer1.Start();
        }


        private void StoreAnalysis(Analysis analysis)
        {
            totalAnalysis[repetitionCount.ElementAt(analysis.AnalysisID-1).Key.Rehydrate().Hash] = analysis;
        }

        private void AnalyzeDeltas()
        {
            Analysis refAnalysis;
            Analysis newAnalysis;
            foreach (Game g in GameRef)
            {
                UpdateGameDigest(g.RatingWhite, g.RatingBlack, g.Plies.Last().MoveNumber);
                g.ResetPosition();
                while (!g.EndOfGame)
                {
                    refAnalysis = totalAnalysis[g.CurrentPosition.Hash];
                    int MoveNumber = (g.curPly / 2) + 1;

                    g.AdvancePosition();
                    newAnalysis = totalAnalysis[g.CurrentPosition.Hash];
                    decimal delta = Math.Abs(refAnalysis.Score - newAnalysis.Score);
                    bool blunder = IsThisABlunder(MoveNumber, delta, refAnalysis.Score);

                    g.AdvancePosition(-1);
                    WriteMoveResult(g.OnMove, MoveNumber,
                        g.OnMove == PlayerEnum.White ? g.RatingWhite : g.RatingBlack,
                        g.OnMove == PlayerEnum.White ? g.RatingBlack : g.RatingWhite,
                        delta,
                        blunder);
                    g.AdvancePosition();
                }
            }
        }
        string logLocation = "C:\\HostWrapper\\analysys.txt";
        public void WriteMoveResult(PlayerEnum onMove, int moveNbr, int moveRating, int oppRating, decimal delta, bool isBlunder)
        {
            string outString = String.Format("|{0}|{1}|{2}|{3}|{4}|{5}|",
                (onMove == PlayerEnum.White ? "w" : "b"), moveNbr, moveRating, oppRating, delta, (isBlunder ? "Y" : "N"));
            // ### we can aggregate them however we want from here...
            StreamWriter log = new StreamWriter(logLocation, true);
            log.WriteLine(outString);
            log.Flush();
            log.Close();
        }
        public void UpdateGameDigest(int RatingWhite, int RatingBlack, int MoveCount)
        {
            // ### we can aggregate them however we want from here...
        }

        private bool IsThisABlunder(int move, decimal delta, decimal incomingScore)
        {
            // this is somewhat subjective
            // con something be a blunder if it still leads to a dead won game (simplification)?
            // is something a blunder if you're dead lost (trying for counterplay)?
            // should your move be counted as "not a blunder" for stats if it's book or tablebase bound?
            // does engine analysis even matter for "book" lines...
            // do exhibition or skittles games matter, or only rated events?

            // store the delta and the subejective flag
            // 0 = not a tumor, 1 = simplification, 2 = counterplay, 3 = book

            bool blunder = delta >= 2.5m;
            if (blunder)
                Console.WriteLine("got one...");
            return blunder;
        }

        private void AnalyzeForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (thisFarmClient != null)
                thisFarmClient.Stop();
            if (thisFarm != null)
                thisFarm.Shutdown();

            thisFarmClient = null;
            thisFarm = null;
        }

    }
}
