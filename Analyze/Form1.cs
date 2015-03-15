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
        void AnalysisEngine_AnalysisUpdate()
        {
            if (AnalysisEngine != null && AnalysisEngine.curAnalysis != null)
                lastAnalysisString.Text = AnalysisEngine.curAnalysis.ToString();
            else
                if (AnalysisEngine != null )
                    lastAnalysisString.Text = AnalysisEngine.lastEngineReply;

        }
        private void RunMenuItem_Click(object sender, EventArgs e)
        {
            if (PGNLoc.Text != "" && File.Exists(PGNLoc.Text))
            {
                string EngineName = "None";
                StreamReader tr = new StreamReader(PGNLoc.Text);

                Engine engine = Engine.InitEngine(EngineName);
                engine.AnalysisUpdate += AnalysisEngine_AnalysisUpdate;

                Dictionary<PositionHash, Analysis> totalAnalysis = new Dictionary<PositionHash, Analysis>();
                // seed with start position
                totalAnalysis[Position.StartPosition.Hash] = null;  // StartAnalysisAsync(engine, Position.StartPosition);
                Analysis refAnalysis;
                Analysis newAnalysis;

                foreach (Game g in GameRef)
                {
                    UpdateGameDigest(g.RatingWhite, g.RatingBlack, g.Plies.Last().MoveNumber);
                    g.ResetPosition();
                    while (!g.EndOfGame)
                    {
                        refAnalysis = totalAnalysis[g.CurrentPosition.Hash];

                        g.AdvancePosition();

                        if (totalAnalysis[g.CurrentPosition.Hash] == null)
                            totalAnalysis[g.CurrentPosition.Hash] = null; // StartAnalysisAsync(engine, g.CurrentPosition);
                        newAnalysis = totalAnalysis[g.CurrentPosition.Hash];

                        decimal delta = Math.Abs(refAnalysis.Score - newAnalysis.Score);

                        int MoveNumber = (g.curPly / 2) + 1;
                        WriteMoveResult(g.OnMove, MoveNumber,
                            g.OnMove == PlayerEnum.White ? g.RatingWhite : g.RatingBlack,
                            g.OnMove == PlayerEnum.White ? g.RatingBlack : g.RatingWhite,
                            delta);
                    }
                }
            }
        }
        public void WriteMoveResult(PlayerEnum onMove, int moveNbr, int moveRating, int oppRating, decimal delta)
        {
            // ### we can aggregate them however we want from here...
        }
        public void UpdateGameDigest(int RatingWhite, int RatingBlack, int MoveCount)
        {
            // ### we can aggregate them however we want from here...
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

        private void AnalysisButton_Click(object sender, EventArgs e)
        {
            string EngineName = "Stockfish";
            AnalysisEngine = Engine.InitEngine(EngineName);

            AnalysisEngine.AnalysisUpdate += AnalysisEngine_AnalysisUpdate;

            totalAnalysis = new Dictionary<PositionHash, Analysis>();

            initAnalysis = true;
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

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (initAnalysis)
                curAnalysisPositionIndex = -1;

            if (AnalysisEngine != null)
                AnalysisEngine.CheckProgress();
            TimeSpan curAnalysisTime = DateTime.Now - curAnalysisStartTime;
            if (initAnalysis || (curAnalysisPositionIndex >= 0 && curAnalysisTime > posAnalysisTimeLimit))
            {
                if( !initAnalysis )
                    StoreAnalysis();
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

        private void StoreAnalysis()
        {
            totalAnalysis[repetitionCount.ElementAt(curAnalysisPositionIndex).Key.Rehydrate().Hash] = AnalysisEngine.curAnalysis;
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
                    g.AdvancePosition();
                    newAnalysis = totalAnalysis[g.CurrentPosition.Hash];

                    decimal delta = Math.Abs(refAnalysis.Score - newAnalysis.Score);
                    bool blunder = delta >= 2.5m;
                    if (blunder)
                        Console.WriteLine("got one...");

                    int MoveNumber = (g.curPly / 2) + 1;
                    WriteMoveResult(g.OnMove, MoveNumber,
                        g.OnMove == PlayerEnum.White ? g.RatingWhite : g.RatingBlack,
                        g.OnMove == PlayerEnum.White ? g.RatingBlack : g.RatingWhite,
                        delta);
                }
            }
        }
    }
}
