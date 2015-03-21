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
using System.Drawing.Text;
using System.Runtime.InteropServices;

namespace PGNViewer
{
    public partial class PGNViewer : Form
    {
        List<Game> GameRef;
        Engine AnalysisEngine;
        Game curGame;

        public PGNViewer()
        {
            InitializeComponent();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string PGNFileLoc = openFileDialog1.FileName;
                GameRef = Game.ReadPGNFile(PGNFileLoc);
                GameList.Items.Clear();
                foreach (Game g in GameRef)
                {
                    GameList.Items.Add(g.GameDate + " " + g.PlayerWhite + "-" + g.PlayerBlack);
                }
            }
        }

        private void PGNViewer_Load(object sender, EventArgs e)
        {
            LoadFont();
            curGame = null;
            AnalysisEngine = null;
            timer1.Start();
        }

        [DllImport("gdi32.dll")]
        private static extern IntPtr AddFontMemResourceEx(IntPtr pbFont, uint cbFont, IntPtr pdv, [In] ref uint pcFonts);

        private PrivateFontCollection pfc;
        private void LoadFont()
        {
            pfc = new PrivateFontCollection();

            byte[] fontdata = Properties.Resources.Chess_7_Font;
            IntPtr fontMemPointer = Marshal.AllocCoTaskMem(fontdata.Length);
            Marshal.Copy(fontdata, 0, fontMemPointer, fontdata.Length);
            pfc.AddMemoryFont(fontMemPointer, fontdata.Length);

            uint dummy = 0;

            AddFontMemResourceEx(fontMemPointer, (uint)fontdata.Length, IntPtr.Zero, ref dummy);
            System.Runtime.InteropServices.Marshal.FreeCoTaskMem(fontMemPointer);

            boardDisplay.Font = new Font(pfc.Families[0], 16);
            DrawBoard();
        }

        private void DrawBoard()
        {
            string emptyBoard =
                  "!\"\"\"\"\"\"\"\"#" + Environment.NewLine // top line
                + "ç + + + +%" + Environment.NewLine    // h-rank with rank ID
                + "æ+ + + + %" + Environment.NewLine
                + "å + + + +%" + Environment.NewLine
                + "ä+ + + + %" + Environment.NewLine
                + "ã + + + +%" + Environment.NewLine
                + "â+ + + + %" + Environment.NewLine
                + "á + + + +%" + Environment.NewLine
                + "à+ + + + %" + Environment.NewLine    // a-rank with rank ID
                + "/èéêëìíîï)" + Environment.NewLine;    // bottom line w/fileID

            string thisBoard = emptyBoard;
            if (curGame != null)
            {
                foreach (Square sq in curGame.CurrentPosition.board.Keys)
                {
                    thisBoard = PokePiece(thisBoard, sq.row + 1, sq.col + 1, curGame.CurrentPosition.board[sq]);
                }
            }
            boardDisplay.Text = thisBoard;

            FENText.Text = curGame.ToFEN();
        }

        private void HighlightPGNMove()
        {
            int thisPly = curGame.curPly - 1;
            bool isBlack = thisPly % 2 == 1;
            if (thisPly < 0 && PGNText.Text != null)
                PGNText.Select(0, 0);
            else
            {
                int moveNbr = (thisPly / 2) + 1;
                bool haveMove = false;
                bool nextPly = !isBlack;
                PGNMoveString thisMove = null;
                foreach (PGNToken t in curGame.PGNtokens)
                {
                    if (!haveMove && t.tokenType == PGNTokenType.MoveNumber && ((PGNMoveNumber)t).value == moveNbr)
                        haveMove = true;
                    else if (haveMove && t.tokenType == PGNTokenType.MoveNumber)
                        ; // errror
                    else if (haveMove && t.tokenType == PGNTokenType.MoveString)
                        if (nextPly)
                        {
                            thisMove = (PGNMoveString)t;
                            break;
                        }
                        else
                            nextPly = true;
                }
                PGNText.Select(thisMove.startLocation, thisMove.value.Length);
            }
        }
        private string PokePiece(string refStr, int rank, int file, Piece pc) // rank/file ranged 1-8
        {
            int locToPoke = ((10 + Environment.NewLine.Length) * (1 + 8 - rank)) + (file);
            bool isWhite = (((((rank - 1) % 2) == ((file - 1) % 2)) ? 1 : 0) == 0);   // 1 => b
            char pcChar = isWhite ? Char.ToLower(pc.ToChess7Char) : Char.ToUpper(pc.ToChess7Char);  // upper case is on a dark square...

            return Utilities.Utils.SwapChar(refStr, locToPoke, pcChar);
        }

        private void GameList_SelectedIndexChanged(object sender, EventArgs e)
        {
            PGNText.Text = "";
            if (GameList.SelectedIndices.Count == 1)
            {
                PGNText.Text = GameRef[GameList.SelectedIndices[0]].PGNSource;
                curGame = GameRef[GameList.SelectedIndices[0]];
                curGame.ResetPosition();
            }
            DrawBoard();
            HighlightPGNMove();
            UpdateAnalysis();
        }


        private void EngineList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (AnalysisEngine != null)
            {
                AnalysisEngine.Quit();
            }
            AnalysisEngine = Engine.InitEngine(EngineList.Text);
            UpdateAnalysis();
            AnalysisEngine.AnalysisUpdateEvent += AnalysisEngine_AnalysisUpdate;
        }

        void AnalysisEngine_AnalysisUpdate(int thisID)
        {
            if (AnalysisEngine != null && AnalysisEngine.curAnalysis != null)
                AnalysisText.Text = AnalysisEngine.curAnalysis.ToString();
        }
        private void UpdateAnalysis()
        {
            if (AnalysisEngine != null)
            {
                AnalysisEngine.Stop();
                if (curGame != null)
                {
                    AnalysisEngine.SetPostion(curGame.ToFEN());
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (AnalysisEngine != null)
                AnalysisEngine.CheckProgress();
        }


        private void ResetGameButton_Click(object sender, EventArgs e)
        {
            ChangePosition(0);
        }
        private void JumpBackButton_Click(object sender, EventArgs e)
        {
            ChangePosition(-5);
        }
        private void BackButton_Click(object sender, EventArgs e)
        {
            ChangePosition(-1);
        }

        private void FwdButton_Click(object sender, EventArgs e)
        {
            ChangePosition(1);
        }
        private void JumpFwdButton_Click(object sender, EventArgs e)
        {
            ChangePosition(5);
        }
        private void JumpToEndButton_Click(object sender, EventArgs e)
        {
            ChangePosition(curGame.Plies.Count);
        }

        private void ChangePosition(int rel)
        {
            if (rel == 0)
                curGame.ResetPosition();
            else
                curGame.AdvancePosition(rel);
            DrawBoard();
            HighlightPGNMove();
            UpdateAnalysis();
        }

    }
}
