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
        public AnalyzeForm()
        {
            InitializeComponent();
        }

        private void OpenMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                PGNLoc.Text = openFileDialog1.FileName;

        }

        private void RunMenuItem_Click(object sender, EventArgs e)
        {
            if (PGNLoc.Text != "" && File.Exists(PGNLoc.Text))
            {
                StreamReader tr = new StreamReader(PGNLoc.Text);

                Engine engine = Engine.InitEngine();

                Dictionary<PositionHash, Analysis> totalAnalysis = new Dictionary<PositionHash, Analysis>();
                // seed with start position
                totalAnalysis[Position.StartPosition.Hash] = engine.GenerateAnalysis(Position.StartPosition);
                Analysis refAnalysis;
                Analysis newAnalysis;

                Game g;
                while ((g = Game.ReadGame(tr)) != null)
                {
                    UpdateGameDigest(g.RatingWhite, g.RatingBlack, g.Plies.Last().MoveNumber);
                    g.ResetPosition();
                    while (!g.EndOfGame)
                    {
                        refAnalysis = totalAnalysis[g.CurrentPosition.Hash];

                        g.AdvancePosition();

                        if (totalAnalysis[g.CurrentPosition.Hash] == null)
                            totalAnalysis[g.CurrentPosition.Hash] = engine.GenerateAnalysis(g.CurrentPosition);
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
    }
}
