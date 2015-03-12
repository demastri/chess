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
            else
            {
                if (false)
                {
                    thisBoard = PokePiece(emptyBoard, 8, 5, new Piece(PlayerEnum.Black, Piece.PieceType.King));
                    thisBoard = PokePiece(thisBoard, 1, 5, new Piece(PlayerEnum.White, Piece.PieceType.King));
                    thisBoard = PokePiece(thisBoard, 2, 1, new Piece(PlayerEnum.White, Piece.PieceType.Pawn));
                    thisBoard = PokePiece(thisBoard, 2, 2, new Piece(PlayerEnum.White, Piece.PieceType.Pawn));
                    thisBoard = PokePiece(thisBoard, 2, 7, new Piece(PlayerEnum.White, Piece.PieceType.Pawn));
                    thisBoard = PokePiece(thisBoard, 2, 8, new Piece(PlayerEnum.White, Piece.PieceType.Pawn));
                    thisBoard = PokePiece(thisBoard, 7, 1, new Piece(PlayerEnum.Black, Piece.PieceType.Pawn));
                    thisBoard = PokePiece(thisBoard, 7, 8, new Piece(PlayerEnum.Black, Piece.PieceType.Pawn));
                }
            }
            boardDisplay.Text = thisBoard;

            FENText.Text = curGame.ToFEN();
        }

        private void HighlightPGNMove()
        {
            if (PGNText.Text == "")
                return;
            int closeTag = 0;
            do
            {
                closeTag = PGNText.Text.IndexOf(']', closeTag + 1);
            } while (PGNText.Text.IndexOf(']', closeTag + 1) > 0);

            int thisPly = curGame.curPly - 1;
            bool isBlack = thisPly % 2 == 1;
            if (thisPly < 0)
                PGNText.Select(closeTag + 3, 0);
            else
            {
                closeTag += 3;
                int tagStart = closeTag;
                closeTag = FindUncommentedMoveNumber(PGNText.Text, closeTag, (thisPly / 2) + 1, ref tagStart);
                closeTag = FindUncommentedToken(PGNText.Text, closeTag, ref tagStart);
                if (isBlack)
                    closeTag = FindUncommentedToken(PGNText.Text, closeTag, ref tagStart);
                PGNText.Select(tagStart, closeTag - tagStart);
                // find first index of movenbr in text that isn't in a comment
                // find next uncommented token (white move)
                // find next uncommented token (black move)

            }
        }
        private int FindUncommentedMoveNumber(string pgn, int start, int moveNbr, ref int tokenStart)
        {
            // ###n actually need to handle commentary and NAGs as well as comments...embedded ECO tags?
            // the proper way to do this is to note the fact that tags can have embedded quoted values
            // comments can appear anywhere outside ot tags, and tags can appear anywhere
            // so, while parsing a PGN entry, we can return any number of items:
            //  a tag, potentially with a key / value or ECO notation
            //  a move number identifier
            //  a move string
            //  an embedded comment
            //  a line ending comment
            string target = moveNbr.ToString() + ".";
            bool inComment = false;
            for (int loc = start; loc < pgn.Length; loc++)
            {
                if (!inComment && pgn[loc] == '{')
                    inComment = true;
                if (inComment && pgn[loc] == '}')
                    inComment = false;
                if (!inComment && pgn[loc] == ',')
                    loc = pgn.IndexOf(Environment.NewLine, loc);
                if (!inComment && pgn.IndexOf(target, loc) == loc)
                {
                    tokenStart = loc;
                    return loc + target.Length;
                }
            }
            return -1;
        }
        private int FindUncommentedToken(string pgn, int start, ref int tokenStart)
        {
            bool inComment = false;
            for (int loc = start; loc < pgn.Length; loc++)
            {
                if (!inComment && pgn[loc] == '{')
                    inComment = true;
                else if (inComment && pgn[loc] == '}')
                    inComment = false;
                else if (!inComment && pgn[loc] == ',')
                    loc = pgn.IndexOf(Environment.NewLine, loc);
                else if (!inComment && !Char.IsWhiteSpace(pgn[loc]))
                {
                    tokenStart = loc;
                    int nlLoc = pgn.IndexOf(Environment.NewLine, tokenStart);
                    int spLoc = pgn.IndexOf(' ', tokenStart);
                    return nlLoc == -1 ? spLoc : (spLoc == -1 ? nlLoc : (nlLoc < spLoc ? nlLoc : spLoc));
                }
            }
            return -1;
        }

        private string PokePiece(string refStr, int rank, int file, Piece pc) // rank/file ranged 1-8
        {
            int locToPoke = ((10 + Environment.NewLine.Length) * (1 + 8 - rank)) + (file);

            int color = ((((rank - 1) % 2) == ((file - 1) % 2)) ? 1 : 0);   // 1 => b
            char pcChar = Char.ToLower(pc.Chess7Char);
            if (color == 1)
                pcChar = Char.ToUpper(pcChar);  // upper case is on a dark square...

            return Utilities.Utils.SwapChar(refStr, locToPoke, pcChar);
        }

        private void GameList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (GameList.SelectedIndices.Count == 0)
                PGNText.Text = "";
            else
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
            AnalysisEngine.AnalysisUpdate += AnalysisEngine_AnalysisUpdate;
        }

        void AnalysisEngine_AnalysisUpdate()
        {
            if (AnalysisEngine != null && AnalysisEngine.curAnalysis != null)
                AnalysisText.Text = AnalysisEngine.curAnalysis.ToString();
        }
        private void UpdateAnalysis()
        {
            if (curGame != null && AnalysisEngine != null)
            {
                AnalysisEngine.Stop();
                AnalysisEngine.SetPostion(curGame.ToFEN());
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (AnalysisEngine != null)
                AnalysisEngine.CheckProgress();
        }


        private void ResetGameButton_Click(object sender, EventArgs e)
        {
            curGame.ResetPosition();
            DrawBoard();
            HighlightPGNMove();
            UpdateAnalysis();
        }
        private void JumpBackButton_Click(object sender, EventArgs e)
        {
            curGame.BackPosition(5);
            DrawBoard();
            HighlightPGNMove();
            UpdateAnalysis();
        }
        private void BackButton_Click(object sender, EventArgs e)
        {
            curGame.BackPosition();
            DrawBoard();
            HighlightPGNMove();
            UpdateAnalysis();
        }

        private void FwdButton_Click(object sender, EventArgs e)
        {
            curGame.AdvancePosition();
            DrawBoard();
            HighlightPGNMove();
            UpdateAnalysis();
        }
        private void JumpFwdButton_Click(object sender, EventArgs e)
        {
            curGame.AdvancePosition(5);
            DrawBoard();
            HighlightPGNMove();
            UpdateAnalysis();
        }
        private void JumpToEndButton_Click(object sender, EventArgs e)
        {
            curGame.AdvancePosition(curGame.Plies.Count);
            DrawBoard();
            HighlightPGNMove();
            UpdateAnalysis();
        }

    }
}
