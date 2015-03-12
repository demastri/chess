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
                totalAnalysis[Position.StartPosition.Hash] = GenerateAnalysis(engine, Position.StartPosition);
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
                            totalAnalysis[g.CurrentPosition.Hash] = GenerateAnalysis(engine, g.CurrentPosition);
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
            Engine engine = Engine.InitEngine(EngineName);

            engine.AnalysisUpdate += AnalysisEngine_AnalysisUpdate;

            totalAnalysis = new Dictionary<PositionHash, Analysis>();

            foreach (PositionHash ph in repetitionCount.Keys)   // initialized on file load
            {
                totalAnalysis[Position.StartPosition.Hash] = GenerateAnalysis(engine, ph.Rehydrate());
            }

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
        private Analysis GenerateAnalysis(Engine e, Position p)
        {
            e.CheckProgress();
            e.StartAnalysis(p);
            System.Threading.Thread.Sleep(2000);
            e.CheckProgress();
            e.Stop();
            return e.curAnalysis;
        }
    }
}
