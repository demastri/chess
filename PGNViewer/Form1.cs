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

        public PGNViewer()
        {
            InitializeComponent();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string PGNLoc = openFileDialog1.FileName;
                if (PGNLoc != "" && File.Exists(PGNLoc))
                {
                    StreamReader tr = new StreamReader(PGNLoc);
                    GameList.Items.Clear();
                    GameRef = new List<Game>();

                    Game g;
                    while ((g = Game.ReadGame(tr)) != null)
                    {
                        GameList.Items.Add(g.GameDate + " " + g.PlayerWhite + "-" + g.PlayerBlack);
                        GameRef.Add(g);
                    }
                }
            }
        }

        private void PGNViewer_Load(object sender, EventArgs e)
        {
            LoadFont();
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

        Game curGame = null;
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
            if( curGame != null ) {
                foreach (Square sq in curGame.CurrentPosition.board.Keys) 
                {
                    thisBoard = PokePiece(thisBoard, sq.row + 1, sq.col + 1, curGame.CurrentPosition.board[sq] );
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
        }

        private void ResetGameButton_Click(object sender, EventArgs e)
        {
            curGame.ResetPosition();
            DrawBoard();
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            curGame.BackPosition();
            DrawBoard();
        }

        private void FwdButton_Click(object sender, EventArgs e)
        {
            curGame.AdvancePosition();
            DrawBoard();
        }
    }
}
