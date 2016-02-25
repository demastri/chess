#define useV2

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.IO;
using System.Xml;
#if useV2
using ChessPosition.V2;
using ChessPosition.V2.PGN;
#else
using ChessPosition;
#endif
using System.Drawing.Text;
using System.Runtime.InteropServices;
using JPD.Utilities;

namespace PGNViewer
{
    public partial class PGNViewer : Form
    {
        // open items for basic correspondence use:
        // generate HTML grid for mail inclusion
        // generate the templated grid
        // show in a browser / copy to clipboard...
        // (validate?) and commit updates validates/updates to screen, needs to save updated game to file
        // save the updated game / file (decorations ###% Right now has to be in the ENTERED STRING)
        // terminators & results updating & displaying properly
        // grouping games in files (in corr mode) by complete / in progress

        GameList refGameList;

        List<Game> GameRef { get { return refGameList.Games; } }
        ChessPosition.Engine AnalysisEngine;
        Game curGame;
        string curPGNFileLoc = "";
        TreeNode inProgOnMoveNode = null;
        TreeNode inProgWaitingNode = null;
        TreeNode complNode = null;
        bool saveFileOnUpdate = false;
        bool FileHasChanged = false;

        int[] refCorrTimeControl = { 10, 30 };
        int[] corrTimeControl = { 10, 30 };

        public PGNViewer()
        {
            InitializeComponent();

            UpdateMRUMenu();
        }

        private void ReloadGamesFromFile()
        {
            LoadGamesFromFile(curPGNFileLoc);
        }
        private void LoadGamesFromDB(string connDetail)
        {
            updatingDisplay = true;
            refGameList = new DbGameList(connDetail, Settings.AppSettings["MyName"]);
            curGame = null;
            saveToolStripMenuItem.Enabled = true;

            Redraw(true, false, false, false);
        }
        private void LoadGamesFromFile(string fn)
        {
            updatingDisplay = true;
            curPGNFileLoc = fn;
#if useV2
            refGameList = new FileGameList(fn, Settings.AppSettings["MyName"]);
#else
            refGameList = new GameList(fn, Settings.AppSettings["MyName"]);
#endif
            curGame = null;
            saveToolStripMenuItem.Enabled = true;

            Redraw(true, false, false, false);
        }
        private void UpdateGameListDisplay()
        {
            GameList.AfterSelect -= new System.Windows.Forms.TreeViewEventHandler(this.GameList_SelectedIndexChanged);

            string selectedText = "";
            if (curGame != null)
                selectedText = curGame.Tags["Event"] + " " + curGame.Tags["White"] + "-" + curGame.Tags["Black"];

            GameList.Nodes.Clear();
            if (curDisplayMode == 3)    // corr
            {
                inProgOnMoveNode = GameList.Nodes.Add("In Progress - On Move");
                inProgWaitingNode = GameList.Nodes.Add("In Progress - Waiting");
                complNode = GameList.Nodes.Add("Complete");
                foreach (Game g in GameRef)
                {
                    int waitDays = g.Plies.Count <= 0 ? 0 : (DateTime.Now - CommentTime(g.Plies.ElementAt(g.Plies.Count - 1).comments)).Days;

                    bool ImWhite = g.Tags["White"] == corrName.Text;
                    bool WOnMove = g.Plies.Count % 2 == 0;
                    string s = g.Tags["Event"] + " (" + ((int)((g.Plies.Count - 1) / 2) + 1) + " m - " + waitDays.ToString() + " d) " + g.Tags["White"] + "-" + g.Tags["Black"];

                    TreeNode thisGame = new TreeNode(s);
                    thisGame.Tag = g;

                    if (g.Tags["Result"] == "*" || g.Tags["Result"] == "")
                    {
                        if ((ImWhite && WOnMove) || (!ImWhite && !WOnMove))
                        {
                            TreeNode eventNode = FindGameNode(inProgOnMoveNode.Nodes, g.Tags["Event"]);
                            if (eventNode == null)
                                eventNode = inProgOnMoveNode.Nodes.Add(g.Tags["Event"]);

                            int i = FindInsertIndex(eventNode.Nodes, waitDays, true);
                            eventNode.Nodes.Insert(i, thisGame);
                        }
                        else
                        {
                            TreeNode eventNode = FindGameNode(inProgWaitingNode.Nodes, g.Tags["Event"]);
                            if (eventNode == null)
                                eventNode = inProgWaitingNode.Nodes.Add(g.Tags["Event"]);

                            int i = FindInsertIndex(eventNode.Nodes, waitDays, true);
                            eventNode.Nodes.Insert(i, thisGame);
                        }
                    }
                    else
                    {
                        TreeNode eventNode = FindGameNode(complNode.Nodes, g.Tags["Event"]);
                        if (eventNode == null)
                            eventNode = complNode.Nodes.Add(g.Tags["Event"]);

                        s = g.Tags["Event"] + " " + g.Tags["White"] + "-" + g.Tags["Black"] + " : " + g.Tags["Result"];
                        thisGame.Text = s;
                        eventNode.Nodes.Add(thisGame);
                    }
                }
            }
            else
            {
                foreach (Game g in GameRef)
                {
                    TreeNode thisGame = new TreeNode(g.Tags["Event"] + " " + g.Tags["White"] + "-" + g.Tags["Black"]);
                    thisGame.Tag = g;
                    GameList.Nodes.Add(thisGame);
                }
            }

            if (curGame != null)
            {
                TreeNode n = FindGameNode(GameList.Nodes, selectedText);
                GameList.SelectedNode = n;
                UpdatePGNText();
                UpdatePGNTags();
                SetInitialPosition();
            }
            GameList.ExpandAll();
            GameList.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.GameList_SelectedIndexChanged);
        }
        private int FindInsertIndex(TreeNodeCollection n, int ageInDays, bool decreasing)
        {
            int outIndex = -1;
            while (++outIndex < n.Count)
            {
                string s = n[outIndex].Text;
                int l = s.IndexOf(" - ");
                int r = s.IndexOf(" d)");
                if (l >= 0 && r >= 0)
                {
                    int thisDay = Convert.ToInt32(s.Substring(l + 3, r - l - 3));
                    if ((decreasing && thisDay <= ageInDays) || (!decreasing && thisDay >= ageInDays))
                        return outIndex;
                }
            }
            return n.Count;
        }


        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                LoadGamesFromFile(openFileDialog1.FileName);
                Settings.AppSettings.Add("MRUList", openFileDialog1.FileName);
                UpdateMRUMenu();
            }
        }

        private void PGNViewer_Load(object sender, EventArgs e)
        {
            LoadFont();
            curGame = null;
            AnalysisEngine = null;
            timer1.Start();

            refGameList = new GameList();
            SetMode(Settings.AppSettings["StartMode"]);
            OpenLastFile();
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
        }

        private void DrawBoard()
        {
            CleanupDrag();
            string WPawns = "pppppppp" + Environment.NewLine;
            string WPieces = "rrnnbbq" + Environment.NewLine;
            string BPawns = "oooooooo" + Environment.NewLine;
            string BPieces = "ttmmvvw" + Environment.NewLine;

            float maxFontWide = boardDisplay.Width / (10f * 1.4f);
            float maxFontHigh = boardDisplay.Height / (10f * 1.4f);
            float maxFont = maxFontWide < maxFontHigh ? maxFontWide : maxFontHigh;

            if (maxFont < 0.005)
                return;
            boardDisplay.Font = new Font(pfc.Families[0], maxFont);

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

            Graphics gr = boardDisplay.CreateGraphics();
            Dictionary<char, SizeF> charWidths = new Dictionary<char, SizeF>();
            foreach (char c in emptyBoard)
            {

                if (charWidths.ContainsKey(c))
                    continue;
                charWidths.Add(c, gr.MeasureString(c.ToString(), boardDisplay.Font));

                float ckFont = boardDisplay.Width / (float)gr.MeasureString("!\"\"\"\"\"\"\"\"#", boardDisplay.Font).ToSize().Width;
            }

            if (ckInvertBoard.Checked)
            {
                emptyBoard =
                      "!\"\"\"\"\"\"\"\"#" + Environment.NewLine // top line
                    + "à + + + +%" + Environment.NewLine    // a-rank with rank ID
                    + "á+ + + + %" + Environment.NewLine    // b
                    + "â + + + +%" + Environment.NewLine    // c
                    + "ã+ + + + %" + Environment.NewLine    // d
                    + "ä + + + +%" + Environment.NewLine    // e
                    + "å+ + + + %" + Environment.NewLine    // f
                    + "æ + + + +%" + Environment.NewLine    // g
                    + "ç+ + + + %" + Environment.NewLine    // h-rank with rank ID
                    + "/ïîíìëêéè)" + Environment.NewLine;    // bottom line w/fileID
            }

            string thisBoard = emptyBoard;
            if (curGame != null)
            {
                foreach (Square sq in curGame.CurrentPosition.board.Keys)
                {
                    Piece thisPc = curGame.CurrentPosition.board[sq];
                    char thisPcChar = Char.ToLower(thisPc.ToChess7Char);
                    if (thisPc.piece == Piece.PieceType.Pawn)
                    {
                        string refStr = (thisPc.color == PlayerEnum.White ? WPawns : BPawns).Substring(1);
                        if (thisPc.color == PlayerEnum.White)
                            WPawns = refStr;
                        else
                            BPawns = refStr;
                    }
                    else if (thisPc.piece != Piece.PieceType.King)
                    {
                        string refStr = (thisPc.color == PlayerEnum.White ? WPieces : BPieces);

                        int loc = refStr.IndexOf(thisPcChar);
                        refStr = refStr.Substring(0, loc) + refStr.Substring(loc + 1);
                        if (thisPc.color == PlayerEnum.White)
                            WPieces = refStr;
                        else
                            BPieces = refStr;
                    }

#if useV2
                    thisBoard = PokePiece(thisBoard, sq.rank + 1, sq.file + 1, thisPc);
#else
                    thisBoard = PokePiece(thisBoard, sq.row + 1, sq.col + 1, thisPc);
#endif
                }
                FENText.Text = curGame.ToFEN();
            }
            else
                FENText.Text = "";

            capturedPieceDisplay.Font = new Font(pfc.Families[0], 9);
            bool showDeltaPieces = false;
            if (showDeltaPieces)
            {
                capturedPieceDisplay.Font = new Font(pfc.Families[0], 18);

            }
            boardDisplay.Text = thisBoard;
            capturedPieceDisplay.Text = WPieces + WPawns + BPawns + BPieces;
            boardDisplay.Select(0, 0);
        }

        private int findPlyAtLocation(int loc)
        {
            for (int i = 0; i <= curGame.Plies.Count; i++)
            {
                if (drawGame.plyStart[i] <= loc && drawGame.plyEnd[i] >= loc)
                    return i;
                if (drawGame.plyStart[i] > loc)
                    return i - 1;
            }
            return curGame.Plies.Count;
        }
        private void HighlightPGNMove()
        {
            if (curGame == null)
            {
                PGNText.Text = "";
                return;
            }
            int thisPly = curGame.curPly - 1;
            bool isBlack = thisPly % 2 == 1;
            if (thisPly < 0 && PGNText.Text != null)
            {
                PGNText.Select(0, 0);
            }
            else
            {
                int moveNbr = (thisPly / 2) + 1;
                bool haveMove = false;
                bool nextPly = !isBlack;
#if useV2
                int startLoc, moveLength;
                drawGame.GetMoveLocations(thisPly, out startLoc, out moveLength);
                PGNText.Select(startLoc, moveLength);
                PGNText.ScrollToCaret();
#else
                PGNMoveString thisMove = null;
                PGNToken lastToken = null;
                foreach (PGNToken t in curGame.PGNtokens)
                {
                    if (!haveMove && t.tokenType == PGNTokenType.MoveNumber && ((PGNMoveNumber)t).value == moveNbr)
                        haveMove = true;
                    else if (haveMove && t.tokenType == PGNTokenType.MoveNumber)
                        ; // error
                    else if (haveMove && t.tokenType == PGNTokenType.MoveString)
                        if (nextPly)
                        {
                            thisMove = (PGNMoveString)t;
                            break;
                        }
                        else
                            nextPly = true;
                    lastToken = t;
                }
                PGNText.Select(thisMove.startLocation, thisMove.value.Length);
                PGNText.ScrollToCaret();
                curMoveTextStart = thisMove.startLocation;
                curMoveTextEnd = thisMove.startLocation + thisMove.value.Length;
                if (lastToken != null && lastToken.tokenType == PGNTokenType.MoveNumber)
                    curMoveTextStart -= lastToken.tokenString.Length;
#endif
            }
        }
        private void HighlightCorrMoveInGrid()
        {
            if (curGame == null)
                return;
            int curRow = (curGame.curPly - 1) / 2;
            int curCol = ((curGame.curPly - 1) % 2 == 0) ? 1 : 4;

            corrGridView.ClearSelection();
            if (curGame.curPly > 0)
            {
                corrGridView.Rows[curRow].Cells[curCol].Selected = true;
                corrGridView.FirstDisplayedScrollingRowIndex = (curRow < 2 ? 0 : (curRow - 2));
            }
        }

#if useV2
        private string PokePiece(string refStr, Square.Rank rank, Square.File file, Piece pc) // rank/file ranged 1-8
#else
        private string PokePiece(string refStr, int rank, int file, Piece pc) // rank/file ranged 1-8
#endif
        {
            if (ckInvertBoard.Checked)
            {
                rank = 9 - rank;
                file = 9 - file;
            }
            int locToPoke = ((10 + Environment.NewLine.Length) * (1 + 8 - (int)rank)) + (int)(file);
            bool isWhite = ((((((int)rank - 1) % 2) == (((int)file - 1) % 2)) ? 1 : 0) == 0);   // 1 => b
            char pcChar = isWhite ? Char.ToLower(pc.ToChess7Char) : Char.ToUpper(pc.ToChess7Char);  // upper case is on a dark square...

            return JPD.Utilities.Utils.SwapChar(refStr, locToPoke, pcChar);
        }

        private void DisableAnalysis()
        {
            if (AnalysisEngine != null)
                AnalysisEngine.Quit();
            EngineList.Text = "None";
            EngineList.Enabled = false;
            AnalysisText.Enabled = false;
            EngineList.Visible = false;
            AnalysisText.Visible = false;
        }
        private void EnableAnalysis()
        {
            if (AnalysisEngine != null)
                AnalysisEngine.Quit();
            EngineList.Text = "None";
            EngineList.Enabled = true;
            AnalysisText.Enabled = true;
            EngineList.Visible = true;
            AnalysisText.Visible = true;
        }

        private void EngineList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (AnalysisEngine != null)
            {
                AnalysisEngine.Quit();
            }
            AnalysisEngine = ChessPosition.Engine.InitEngine(EngineList.Text);
            UpdateAnalysis();
            AnalysisEngine.AnalysisUpdateEvent += AnalysisEngine_AnalysisUpdate;
        }

        void AnalysisEngine_AnalysisUpdate(int thisID)
        {
            if (AnalysisEngine != null && AnalysisEngine.curAnalysisRequest.thisAnalysis != null)
                AnalysisText.Text = AnalysisEngine.curAnalysisRequest.thisAnalysis.ToString();
        }
        private void UpdateAnalysis()
        {
            if (AnalysisEngine != null)
            {
                AnalysisEngine.Stop();
                if (curGame != null)
                {
                    AnalysisEngine.SetPostion(new ChessPosition.EngineParameters("stockfish", 50, -1), curGame.ToFEN());
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
            Redraw(false, false, false, false);
        }
        private void ChangeTo(int abs)
        {
            curGame.AdvanceTo(abs);
            Redraw(false, false, false, false);
        }

        private void viewerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetMode("viewer");
        }

        private void analysisToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetMode("analysis");
        }

        private void correspondenceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetMode("correspondence");
        }

        int curDisplayMode = -1;
        private void SetMode(string mode)
        {
            if (mode == null)
                mode = Settings.AppSettings["StartMode"] = "correspondence";

            // save for next start (if this isn't a load...
            if (curDisplayMode != -1)
                Settings.AppSettings["StartMode"] = mode;

            if (viewerToolStripMenuItem.Checked = (mode == "viewer"))
            {
                curDisplayMode = 1;
                DisableAnalysis();
                DisableCorrespondence();
            }
            if (analysisToolStripMenuItem.Checked = (mode == "analysis"))
            {
                curDisplayMode = 2;
                EnableAnalysis();
                DisableCorrespondence();
            }
            if (correspondenceToolStripMenuItem.Checked = (mode == "correspondence"))
            {
                curDisplayMode = 3;
                DisableAnalysis();
                EnableCorrespondence();
            }
            Redraw(false, false, false, false);
        }
        private void DisableCorrespondence()
        {
            corrTemplateList.Visible = corrTZLabel.Visible = corrTZ.Visible = false;
            corrNameLabel.Visible = corrName.Visible = false;
            corrGridView.Enabled = corrGridView.Visible = false;
            ReflTimeLabel.Visible = CorrPublish.Visible = CorrUpdate.Visible =
            CorrMoveNbr.Visible = CorrLabel.Visible = CorrMoveText.Visible = CorrMoveTime.Visible = CorrTimeNow.Visible = false;
        }
        private void EnableCorrespondence()
        {
            corrTemplateList.Visible = corrTZLabel.Visible = corrTZ.Visible = true;
            corrNameLabel.Visible = corrName.Visible = true;
            corrGridView.Enabled = corrGridView.Visible = true;
            ReflTimeLabel.Visible = CorrPublish.Visible = CorrUpdate.Visible =
            CorrMoveNbr.Visible = CorrLabel.Visible = CorrMoveText.Visible = CorrMoveTime.Visible = CorrTimeNow.Visible = true;

            CorrUpdate.Enabled = false;

            InitCorrContext();
            InitCorrTemplateList();
        }
        int totalCorrTimeW = 0;
        int totalCorrTimeB = 0;
        int usedCorrTimeW = 0;
        int usedCorrTimeB = 0;
        int lastCorrTimeW = 0;
        int lastCorrTimeB = 0;
        List<int> corrTimes;
        int remainCorrTimeW = 0;
        int remainCorrTimeB = 0;

        private void InitCorrContext()
        {
            corrName.Text = Settings.AppSettings["MyName"] == null ? "Enter Name Here" : Settings.AppSettings["MyName"];
            if (Settings.AppSettings["CorrTimeControl"] == null || Settings.AppSettings.Count("CorrTimeControl") != 2)
            {
                Settings.AppSettings.Clear("CorrTimeControl");
                Settings.AppSettings["CorrTimeControl", 0] = "10";
                Settings.AppSettings["CorrTimeControl", 1] = "30";
            }
            corrTimeControl[0] = Convert.ToInt32(Settings.AppSettings["CorrTimeControl", 0]);
            corrTimeControl[1] = Convert.ToInt32(Settings.AppSettings["CorrTimeControl", 1]);
        }

        private void InitCorrTemplateList()
        {
            corrTemplateList.Items.Clear();
            DirectoryInfo templates = new DirectoryInfo(Directory.GetCurrentDirectory() + "/Resources");
            FileInfo[] tFiles = templates.GetFiles("*.html");
            foreach (FileInfo fi in tFiles)
            {
                corrTemplateList.Items.Add(fi.Name.Substring(0, fi.Name.Length - 5));
            }
            // this is ok - if a setting exists, use that ###.
            string defaultTemplate = Settings.AppSettings["CorrTemplate"];
            if (defaultTemplate != null && corrTemplateList.Items.Contains(defaultTemplate))
                corrTemplateList.SelectedItem = defaultTemplate;
            else
                corrTemplateList.SelectedIndex = corrTemplateList.Items.Count - 1;
        }
        bool updatingDisplay = false;
        private void UpdateCorrespondence()
        {
            // follow highlighted move from pgn viewer
            // enter move as W/B
            // enter move time as W/B - button for "~now"
            // calculate reflection time (including totals)
            // (validate?) and commit updates validates/updates to screen, needs to save updated game to file
            // ###% move game to "other" (complete) file
            // generate HTML grid for mail inclusion
            // pgn viewer follow highlighted grid move :) ###%
            // pgn reader has to deal with "in-progress" games
            // open/complete folders in grid?

            // at this point we can load the grid with each player's moves (1 row per full ply)
            // if there's time associated with the move we can keep reflection time at that point as well
            corrGridView.Rows.Clear();
            ReflTimeLabel.Text = "Reflection Time:  Tot/Used/Rem (W/B): ";
            if (curGame == null)
                return;

            int rows = (curGame.Plies.Count - 1) / 2 + 1;

            corrGridView.Rows.Insert(0, rows);

            CorrMoveNbr.Text = (curGame.Plies.Count / 2 + 1).ToString() + ((curGame.Plies.Count % 2 == 0) ? "" : " ... ");

            //corrGridView.Columns[1].HeaderText = curGame.PlayerWhite;
            //corrGridView.Columns[4].HeaderText = curGame.PlayerBlack;

            corrTimeControl[0] = refCorrTimeControl[0];
            corrTimeControl[1] = refCorrTimeControl[1];
            if (curGame.Tags.ContainsKey("TimeControl"))
            {
                string testStr = curGame.Tags["TimeControl"];
                int l = testStr.IndexOf('/');
                if (l >= 0)
                {
                    corrTimeControl[0] = Convert.ToInt32(testStr.Substring(0, l));
                    corrTimeControl[1] = Convert.ToInt32(testStr.Substring(l + 1));
                }
            }


            DateTime lastMoveTime = DateTime.MinValue;
            totalCorrTimeW = corrTimeControl[1] + (corrTimeControl[1] * (curGame.Plies.Count / (2 * corrTimeControl[0])));
            totalCorrTimeB = corrTimeControl[1] + (corrTimeControl[1] * ((curGame.Plies.Count - 1) / (2 * corrTimeControl[0])));
            usedCorrTimeW = 0;
            usedCorrTimeB = 0;
            lastCorrTimeW = 0;
            lastCorrTimeB = 0;
            corrTimes = new List<int>();

            for (int i = 0; i < curGame.Plies.Count; i++)
            {
                Ply p = curGame.Plies[i];
#if !useV2
                PGNToken s = curGame.PGNtokens[i];
#endif
                int thisReflTime = 0;

                corrGridView.Rows[i / 2].Cells["MoveNbr"].Value = 1 + i / 2;
#if useV2
                string baseStr = drawGame.PGNPlySource[i];
                if (baseStr.IndexOf('.') >= 0)
                    baseStr = baseStr.Substring(baseStr.IndexOf('.') + 1);
#else
                string baseStr = p.refToken.value;
#endif
                corrGridView.Rows[i / 2].Cells[(i % 2 == 0) ? "White" : "Black"].Value = baseStr;

                if (p.comments != null)
                {
                    DateTime thisMoveTime = CommentTime(p.comments);
                    if (thisMoveTime != DateTime.MinValue && lastMoveTime != DateTime.MinValue && thisMoveTime > lastMoveTime)
                    {
                        TimeSpan refl = thisMoveTime - lastMoveTime;
                        thisReflTime = refl.Days;
                    }
                    // should be a time value
                    corrGridView.Rows[i / 2].Cells[(i % 2 == 0) ? "WMoveTime" : "BMoveTime"].Value = CommentTimeString(p.comments);
                    lastMoveTime = thisMoveTime;

                    thisReflTime += PenaltyTime(p.comments);
                }
                else
                {
                    lastMoveTime = DateTime.MinValue;
                    thisReflTime = 0;
                }
                corrTimes.Add(thisReflTime);
                if (i % 2 == 0) // W
                {
                    lastCorrTimeW = thisReflTime;
                    usedCorrTimeW += thisReflTime;
                }
                else
                {
                    lastCorrTimeB = thisReflTime;
                    usedCorrTimeB += thisReflTime;
                }
                corrGridView.Rows[i / 2].Cells[(i % 2 == 0) ? "WMoveReflection" : "BMoveReflection"].Value = thisReflTime;
            }
            remainCorrTimeW = totalCorrTimeW - usedCorrTimeW;
            remainCorrTimeB = totalCorrTimeB - usedCorrTimeB;
            ReflTimeLabel.Text = "Reflection Time:  Tot/Used/Rem (W/B): " + Environment.NewLine +
                totalCorrTimeW.ToString() + " / " + usedCorrTimeW.ToString() + " / " + remainCorrTimeW.ToString() + "     " +
                totalCorrTimeB.ToString() + " / " + usedCorrTimeB.ToString() + " / " + remainCorrTimeB.ToString();

#if useV2
            resultCombo.Text = curGame.TerminatorString;
#else
            resultCombo.Text = curGame.GameTerm.value;
#endif
        }

        private void CorrTimeNow_Click(object sender, EventArgs e)
        {
            DateTime outTime = DateTime.Now;
            outTime = outTime.AddMinutes(5);
            outTime = new DateTime(outTime.Year, outTime.Month, outTime.Day,
                outTime.Hour, outTime.Minute - (outTime.Minute % 5), 0);
            CorrMoveTime.Value = outTime;
        }

        private void CorrUpdate_Click(object sender, EventArgs e)
        {
            string possMove = CorrMoveText.Text + " ";
            DateTime possTime = CorrMoveTime.Value;
            // this a valid time for the game?
            DateTime commentTime = CommentTime(curGame.Plies[curGame.Plies.Count - 1].comments);
            if (commentTime > possTime)
            {
                MessageBox.Show("There was a later time already posted to this game");
                return;
            }
            // is this a valid move in the position?
            Ply newMove = null;
            if (possMove.Trim() == "")
            {
                MessageBox.Show("There was a problem with the provided move");
                return;
            }
#if useV2
            curGame.AdvanceTo(curGame.Plies.Count);

            newMove = new Ply();
            curGame.AddMoveToPly(possMove, newMove);

            if (newMove == null || newMove.src == Square.None())
            {
                MessageBox.Show("There was a problem with the provided move");
                return;
            }
            newMove.Number = curGame.Plies.Count;
            curGame.Plies.Add(newMove);

            // append this to the game
            // append this to the game text(s)...
            // add the time comment 
            // regen the text++
            newMove.comments.Add(new Comment(false, possTime.ToString("MM/dd/yyyy HHmm")));

            drawGame = new PGNGame(curGame);
            drawGame.GeneratePGNSource(PGNGame.GameSaveOptions.MoveListOnly);
            PGNText.Text = drawGame.PGNSource;

#else
            // append this to the game
            // ###% actually refactor this so almost all of it lives in the game class...
            PGNMoveString moveStr = new PGNMoveString(possMove);
            curGame.ResetPosition();
            curGame.AdvancePosition(curGame.Plies.Count);
            if ((newMove = curGame.HandleMove(moveStr, curGame.Plies, curGame.Plies.Count)) == null)
            {
                MessageBox.Show("There was a problem with the provided move");
                return;
            }
            newMove.refToken = moveStr;

            // append this to the game text(s)...

            // better handle EOL (put new W moves on new line if some length would be reached)
            // comes in with the PGN ending with a line terminator
            // should get the current final line length
            // remove the existing nl
            // if this is a black move or adding a new white/black pair would be < 100 char
            // insert a nl here
            // insert these tokens after the existing newline character and add a nl
            // add the time comment 

            PGNToken termToken = null;

            foreach (PGNToken t in curGame.PGNtokens)
                if (t.tokenType == PGNTokenType.Terminator)
                {
                    termToken = t;
                    break;
                }
            if (termToken != null)
                curGame.PGNtokens.Remove(termToken);

            bool isAWhiteMove = (curGame.Plies.Count % 2 == 1);
            if (isAWhiteMove)
            {
                string nbrText = (curGame.Plies.Count / 2 + 1).ToString() + ".";
                PGNMoveNumber moveNbr = new PGNMoveNumber(nbrText, 0);
                curGame.PGNtokens.Add(moveNbr);
            }
            curGame.PGNtokens.Add(moveStr);

            newMove.refToken.startLocation = PGNText.Text.Length;

            PGNComment timeComment = new PGNComment(possTime.ToString("{MM/dd/yyyy HHmm}"));
            curGame.PGNtokens.Add(timeComment);

            newMove.comments.Add(timeComment);

            PGNText.Text = (curGame.PGNSource = curGame.GeneratePGNSource(Game.GameSaveOptions.MoveListOnly)) + Environment.NewLine;

            if (termToken != null)
                curGame.PGNtokens.Add(termToken);
#endif

            // move to the end...
            curGame.AdvanceTo(curGame.Plies.Count);

            // save the updated game / file
            if (saveFileOnUpdate)
            {
                refGameList.Save(curPGNFileLoc, Settings.AppSettings["MyName"]);
                ReloadGamesFromFile();
                Redraw(true, false, true, false);
            }
            else
                Redraw(false, false, true, false);

            CorrMoveText.Text = "";
            CorrUpdate.Enabled = false;
        }

        private TreeNode FindGameNode(TreeNodeCollection list, string text)
        {
            foreach (TreeNode node in list)
            {
                string testStr = node.Text;
                int l = testStr.IndexOf('(');
                int r = testStr.IndexOf(')');
                if (testStr.IndexOf(text) >= 0)
                    return node;

                if (l >= 0 && r >= 0)
                {
                    testStr = testStr.Substring(0, l - 1) + testStr.Substring(r + 1);
                    if (text == testStr)
                        return node;
                }
                TreeNode isKidNode = FindGameNode(node.Nodes, text);
                if (isKidNode != null)
                    return isKidNode;
            }
            return null;
        }

        private void CorrPublish_Click(object sender, EventArgs e)
        {
            // generate the templated grid
            // show in a browser / copy to clipboard...

            CorrResponseDialog respDialog = new CorrResponseDialog();

            MemoryStream outStream = new MemoryStream();
            StreamWriter writer = new StreamWriter(outStream);

            StreamReader tr = new StreamReader(Directory.GetCurrentDirectory() + "/Resources/" + corrTemplateList.SelectedItem + ".html");
            while (!tr.EndOfStream)
            {
                string thisLine = tr.ReadLine();
                thisLine = ReplaceTags(thisLine);
                writer.Write(thisLine);
            }
            writer.Flush();
            outStream.Position = 0;

            respDialog.htmlView.DocumentStream = outStream;

            respDialog.ShowDialog();

        }
#if useV2
        public static int FindTimeComment(List<Comment> cmts)
        {
            foreach (Comment comment in cmts)
#else
        public static int FindTimeComment(List<PGNComment> cmts)
        {
            foreach (PGNComment comment in cmts)
#endif
            {
                // (05/10/2015 1224)
                DateTime thisTime;
                if (DateTime.TryParseExact(comment.value, "MM/dd/yyyy HHmm", CultureInfo.InvariantCulture, DateTimeStyles.None, out thisTime))
                    return cmts.IndexOf(comment);
            }
            return -1;
        }
#if useV2
        public static int FindPenaltyComment(List<Comment> cmts)
#else
        public static int FindPenaltyComment(List<PGNComment> cmts)
#endif
        {
            string[] timetags = { "PenaltyDays:", "TimeAtStart:" };
            foreach (string pDayTag in timetags)
#if useV2
                foreach (Comment comment in cmts)
#else
                foreach (PGNComment comment in cmts)
#endif
                {
                    if (comment.value.IndexOf(pDayTag) == 0)
                        return cmts.IndexOf(comment);
                }
            return -1;
        }
#if useV2
        public static int PenaltyTime(List<Comment> cmts)
#else
        public static int PenaltyTime(List<PGNComment> cmts)
#endif
        {
            int index = FindPenaltyComment(cmts);
            if (index >= 0)
            {
                int penaltyTime = 0;
                if (Int32.TryParse(cmts[index].value.Substring(cmts[index].value.IndexOf(':') + 1), out penaltyTime))
                    return penaltyTime;
            }
            return 0;
        }
#if useV2
        public static void UpdatePenaltyTime(List<Comment> cmts, int newTime, int plyNbr)
#else
        public static void UpdatePenaltyTime(List<PGNComment> cmts, int newTime, int plyNbr)
#endif
        {
            int index = FindPenaltyComment(cmts);
            if (index >= 0)
            {
                if (newTime > 0)
                    cmts[index].value = (plyNbr == 0 ? "TimeAtStart:" : "PenaltyDays:") + newTime.ToString(); // update the existing value if it exists
                else
                    cmts.RemoveAt(index);   // remove it if it exists...
            }
            else
            {
                if (newTime > 0)    // have new time and didn't before (if don't have new time, do nothing...
                {
#if useV2
                    if (plyNbr == 0)
                        cmts.Add(new Comment(false, "TimeAtStart:" + newTime.ToString()));
                    else
                        cmts.Add(new Comment(false, "PenaltyDays:" + newTime.ToString() + ""));
#else
                    if (plyNbr == 0)
                        cmts.Add(new PGNComment("{TimeAtStart:" + newTime.ToString() + "}"));
                    else
                        cmts.Add(new PGNComment("{PenaltyDays:" + newTime.ToString() + "}"));
#endif
                }
            }
        }
#if useV2
        public static DateTime CommentTime(List<Comment> cmts)
#else
        public static DateTime CommentTime(List<PGNComment> cmts)
#endif
        {
            int index = FindTimeComment(cmts);
            if (index >= 0)
            {
                DateTime thisTime = DateTime.MinValue;
                DateTime.TryParseExact(cmts[index].value, "MM/dd/yyyy HHmm", CultureInfo.InvariantCulture, DateTimeStyles.None, out thisTime);
                return thisTime;
            }
            return DateTime.MinValue;
        }
#if useV2
        public static void UpdateCommentTime(List<Comment> cmts, DateTime newVal)
#else
        public static void UpdateCommentTime(List<PGNComment> cmts, DateTime newVal)
#endif
        {
            string outStr = newVal.ToString("MM/dd/yyyy HHmm");
            int index = FindTimeComment(cmts);
            if (index >= 0)
                cmts[index].value = outStr;
            else
#if useV2
                cmts.Add(new Comment(false, outStr));
#else
                cmts.Add(new PGNComment("{" + outStr + "}"));
#endif
        }
#if useV2
        public static string CommentTimeString(List<Comment> cmts)
#else
        public static string CommentTimeString(List<PGNComment> cmts)
#endif
        {
            int index = FindTimeComment(cmts);
            if (index >= 0)
                return cmts[index].value;
            return "";
        }
        private string ReplaceTags(string refStr)
        {
            bool whiteOnMove = (curGame.Plies.Count % 2 == 0);
            int lastWPly = curGame.Plies.Count - (whiteOnMove ? 1 : 0) - 1;
            int lastBPly = curGame.Plies.Count - (whiteOnMove ? 0 : 1) - 1;
            int lastMoveNbr = (curGame.Plies.Count - 1) / 2 + 1;
            string tempStr = "";
            string[] tokens = refStr.Split(new char[] { '<', '>', ' ' });
            foreach (string token in tokens)
            {
                if (token.Length > 1 && token[0] == '@')
                {
                    string key = token.Substring(1);
                    int offsetLoc = key.IndexOf('-');
                    int offset = 0;
                    if (offsetLoc >= 0)
                    {
                        offset = Convert.ToInt32(key.Substring(offsetLoc + 1));
                        key = key.Substring(0, offsetLoc);
                    }
                    switch (key)
                    {
                        case "MoveNbr":
                            lastMoveNbr -= offset;
                            if (lastMoveNbr > 0)
                                tempStr = lastMoveNbr <= 0 ? "" : (lastMoveNbr.ToString() + ".");
                            refStr = refStr.Replace(token, tempStr);
                            break;

                        case "WhiteMoveNbr":
                            lastWPly -= 2 * offset;
                            if (lastWPly >= 0)
                                tempStr = ((lastWPly / 2) + 1).ToString() + ". ";
                            refStr = refStr.Replace(token, tempStr);
                            break;
                        case "WhiteMove":
                            lastWPly -= 2 * offset;
#if !useV2
                            if (lastWPly >= 0)
                                tempStr = curGame.Plies.ElementAt(lastWPly).refToken.tokenString;
#endif
                            refStr = refStr.Replace(token, tempStr);
                            break;
                        case "WhiteMoveTime":
                            lastWPly -= 2 * offset;
                            if (lastWPly >= 0 && curGame.Plies.ElementAt(lastWPly).comments != null)
                                tempStr = CommentTimeString(curGame.Plies.ElementAt(lastWPly).comments) + " " + corrTZ.Text;
                            refStr = refStr.Replace(token, tempStr);
                            break;
                        case "WhiteReflTime":
                            lastWPly -= 2 * offset;
                            if (lastWPly >= 0)
                                tempStr = corrTimes[lastWPly].ToString();
                            refStr = refStr.Replace(token, tempStr);
                            break;
                        case "WhiteClockStart":
                            lastWPly -= 1 + (2 * offset);
                            if (lastWPly >= 0 && curGame.Plies.ElementAt(lastWPly).comments != null)
                                tempStr = CommentTimeString(curGame.Plies.ElementAt(lastWPly).comments) + " " + corrTZ.Text;
                            refStr = refStr.Replace(token, tempStr);
                            break;

                        case "BlackGridMoveNbr":
                            if (!whiteOnMove)
                                offset -= 1;
                            goto case "BlackMoveNbr";
                        case "BlackMoveNbr":
                            lastBPly -= 2 * offset;
                            if (lastBPly >= 0)
                                tempStr = ((lastBPly / 2) + 1).ToString() + ". ... ";
                            refStr = refStr.Replace(token, tempStr);
                            break;
                        case "BlackGridMove":
                            if (!whiteOnMove)
                                offset -= 1;
                            goto case "BlackMove";
                        case "BlackMove":
                            lastBPly -= 2 * offset;
#if !useV2
                            if (lastBPly >= 0 && curGame.Plies.Count > lastBPly)
                                tempStr = curGame.Plies.ElementAt(lastBPly).refToken.tokenString;
#endif
                            refStr = refStr.Replace(token, tempStr);
                            break;
                        case "BlackGridMoveTime":
                            if (!whiteOnMove)
                                offset -= 1;
                            goto case "BlackMoveTime";
                        case "BlackMoveTime":
                            lastBPly -= 2 * offset;
                            if (lastBPly >= 0 && curGame.Plies.Count > lastBPly && curGame.Plies.ElementAt(lastBPly).comments != null)
                                tempStr = CommentTimeString(curGame.Plies.ElementAt(lastBPly).comments) + " " + corrTZ.Text;
                            refStr = refStr.Replace(token, tempStr);
                            break;
                        case "BlackGridReflTime":
                            if (!whiteOnMove)
                                offset -= 1;
                            goto case "BlackReflTime";
                        case "BlackReflTime":
                            lastBPly -= 2 * offset;
                            if (lastBPly >= 0 && curGame.Plies.Count > lastBPly)
                                tempStr = corrTimes[lastBPly].ToString();
                            refStr = refStr.Replace(token, tempStr);
                            break;
                        case "BlackGridClockStart":
                            if (!whiteOnMove)
                                offset -= 1;
                            goto case "BlackClockStart";
                        case "BlackClockStart":
                            lastBPly -= 1 + (2 * offset);
                            if (lastBPly >= 0 && curGame.Plies.Count > lastBPly && curGame.Plies.ElementAt(lastBPly).comments != null)
                                tempStr = CommentTimeString(curGame.Plies.ElementAt(lastBPly).comments) + " " + corrTZ.Text;
                            refStr = refStr.Replace(token, tempStr);
                            break;

                        case "EventName":
                            refStr = refStr.Replace(token, curGame.Tags["Event"]);
                            break;
                        case "GameName":
                            //refStr = refStr.Replace(token, curGame.Tags["Round"]);
                            refStr = refStr.Replace(token, curGame.Tags["White"] + " - " + curGame.Tags["Black"]);
                            break;
                        case "WhiteName":
                            refStr = refStr.Replace(token, curGame.Tags["White"]);
                            break;
                        case "WhiteReflTimeOverview":
                            tempStr = (remainCorrTimeW + lastCorrTimeW).ToString() + " / " + lastCorrTimeW.ToString() + " / " + remainCorrTimeW.ToString();
                            refStr = refStr.Replace(token, tempStr);
                            break;
                        case "BlackName":
                            refStr = refStr.Replace(token, curGame.Tags["Black"]);
                            break;
                        case "BlackReflTimeOverview":
                            tempStr = (remainCorrTimeB + lastCorrTimeB).ToString() + " / " + lastCorrTimeB.ToString() + " / " + remainCorrTimeB.ToString();
                            refStr = refStr.Replace(token, tempStr);
                            break;
#if useV2
#else
                        case "PGNWithTags":
                            // actually just move numbers and moves here...
                            string thisScore = curGame.GeneratePGNSource(10000, Game.GameSaveOptions.SimpleGameScore);
                            refStr = thisScore.Replace(Environment.NewLine, "<br>");
                            break;
                        case "PGNSource":
                            // actually just move numbers and moves here...
                            refStr = refStr.Replace(token, "<br>" + curGame.GeneratePGNSource(Game.GameSaveOptions.MoveListOnly) + "<br><br>");
                            break;
#endif
                        case "Diagram":
                            // pull the current text from the board display
                            string boardText = boardDisplay.Text;

                            int thisIndex = 0;
                            for (int i = 0; i < 10; i++)
                                thisIndex = boardText.IndexOf(Environment.NewLine, thisIndex + 1);
                            boardText = boardText.Substring(0, thisIndex);

                            // draw it into a picture box (font/size)
                            PictureBox outBox = new PictureBox();
                            outBox.Width = outBox.Height = 150;
                            picBox_Paint(outBox, boardText, 14);

                            // extract the image data from the picture box
                            string imageInBase64 = ConvertImageToBase64String(outBox.Image);

                            // build the img tag contents
                            string imgStr = "<img id=\"myImg\" src=\"data:image/bmp;base64," + imageInBase64 + "\" alt=\"My Image data in base 64\" />";

                            // write the image tag to the template
                            refStr = refStr.Replace(token, "" + imgStr + "");
                            break;
                        case "MoveAndTimeGrid":
                            string tableString = "<Table border=\"1\" cellpadding=\"5\">";
                            tableString += "<tr>";
                            foreach (DataGridViewColumn c in corrGridView.Columns)
                                tableString += "<td>" + c.HeaderText + "</td>";
                            tableString += "</tr>";
                            foreach (DataGridViewRow r in corrGridView.Rows)
                            {
                                tableString += "<tr>";
                                foreach (DataGridViewCell c in r.Cells)
                                    tableString += "<td>" + c.Value.ToString() + "</td>";
                                tableString += "</tr>";
                            }
                            tableString += "<tr>";
                            tableString += "<td colspan=3>Totals</td><td>" + (usedCorrTimeW).ToString() + "</td>";
                            tableString += "<td colspan=2></td><td>" + (usedCorrTimeB).ToString() + "</td>";
                            tableString += "</tr>";
                            tableString += "</Table>";
                            refStr = refStr.Replace(token, "" + tableString + "");
                            break;
                    }
                }
                if (token.Length > 0 && token[0] == '@')
                    refStr = refStr.Replace(token, "Some New Value -" + token.Substring(1) + "-");
            }
            return refStr;
        }
        private void picBox_Paint(PictureBox pBox, string s, int fontSize)
        {
            Bitmap bmp = new Bitmap(pBox.Width, pBox.Height);
            Graphics gr = Graphics.FromImage(bmp);

            Pen p = new Pen(Color.Red);
            p.Width = 0.2f;

            Brush bk = Brushes.Black;

            gr.Clear(Color.White);
            Font myFont = new Font(pfc.Families[0], fontSize, FontStyle.Regular, GraphicsUnit.Pixel);
            gr.DrawString(s, myFont, bk, new Point(0, 0));
            pBox.Image = bmp;
        }
        private string ConvertImageToBase64String(Image img)
        {
            MemoryStream ms = new MemoryStream();
            img.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);

            return Convert.ToBase64String(ms.ToArray());
        }

        private void PGNViewer_Resize(object sender, EventArgs e)
        {
            Redraw(false, false, false, false);
        }

        private void ckInvertBoard_Click(object sender, EventArgs e)
        {
            Redraw(false, false, false, false);
        }

        private void GameList_SelectedIndexChanged(object sender, TreeViewEventArgs e)
        {
            UpdatePGNText();
            UpdatePGNTags();
            SetInitialPosition();
            Redraw(false, true, false, false);
        }
        private void UpdatePGNText()
        {
            curGame = null;
            PGNText.Text = "";
            if (GameList.SelectedNode != null && GameList.SelectedNode.Tag != null)
            {
                curGame = (Game)GameList.SelectedNode.Tag;
#if useV2
                drawGame = new PGNGame(curGame);
                drawGame.GeneratePGNSource(PGNGame.GameSaveOptions.MoveListOnly);
                PGNText.Text = drawGame.PGNSource;
#else
                PGNText.Text = curGame.PGNSource = curGame.GeneratePGNSource(Game.GameSaveOptions.MoveListOnly);
#endif
            }
        }
        bool initTags = false;
        private void UpdatePGNTags()
        {
            initTags = true;
            tagEditorGrid.Rows.Clear();
            if (curGame != null)
            {
                tagEditorGrid.Rows.Insert(0, curGame.Tags.Count);
                for (int i = 0; i < curGame.Tags.Count; i++)
                {
                    string key = curGame.Tags.Keys.ElementAt(i);
                    tagEditorGrid.Rows[i].Cells[0].Value = key;
                    tagEditorGrid.Rows[i].Cells[1].Value = curGame.Tags[key];
                }
            }
            initTags = false;
        }
        private void SetInitialPosition()
        {
            if (curGame != null)
            {
                ckInvertBoard.Checked = (curGame.Tags["Black"] == corrName.Text);
                if (curGame.Tags["Result"] == "*" || curGame.Tags["Result"] == "")
                    curGame.AdvancePosition(curGame.Plies.Count);
                else
                    curGame.ResetPosition();
            }
        }
        private void UpdateFormTitle()
        {
            Text = "PGN Viewer";
            if (curPGNFileLoc != "")
            {
                Text += " - " + curPGNFileLoc;
                if (curGame != null)
                    Text += " (" + curGame.Tags["White"] + "-" + curGame.Tags["Black"] + ")";
            }
        }
        private void UpdateMRUMenu()
        {
            for (int i = 1; i <= 5; i++)
            {
                ToolStripItem menu = FileMenuStrip.DropDownItems["mruMenuItem" + i.ToString()];
                int mruIndex = Settings.AppSettings.Count("MRUList") - i;
                if (mruIndex >= 0)
                {
                    menu.Text = "&" + i.ToString() + " - " + Settings.AppSettings["MRUList", mruIndex];
                    menu.Visible = true;
                }
                else
                {
                    menu.Visible = false;
                }
            }
        }

        private void mruMenuItem_Click(object sender, EventArgs e)
        {
            string fn = sender.ToString().Substring(5);
            LoadGamesFromFile(fn);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void closeToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (CheckFileChanges())
                LoadGamesFromFile("");
        }

        private void corrGridView_Click(object sender, EventArgs e)
        {
            int thisRow = corrGridView.SelectedCells[0].RowIndex;
            int thisCol = corrGridView.SelectedCells[0].ColumnIndex;
            bool isWhite = (thisCol < 4);

            int newPly = 2 * thisRow + (isWhite ? 1 : 2);

            ChangePosition(0);
            ChangePosition(newPly);
        }

        private void PGNText_Click(object sender, EventArgs e)
        {
            int newPly = findPlyAtLocation(PGNText.SelectionStart);

            ChangeTo(newPly + 1);
        }
        bool inDrag = false;
        int dragStartPosition = -1;
        int dragEndPosition = -1;
        Square dragStartSquare = null;
        Square dragEndSquare = null;
        Cursor refCursor = null;
        private void InitDrag(MouseEventArgs e)
        {
            int rowLength = 12;
            int rowBoardOffset = 1;
            int startIndex = boardDisplay.SelectionStart;
#if useV2
            dragStartSquare = new Square(
                (Square.Rank)(8 - startIndex / rowLength),
                (Square.File)((startIndex % rowLength) - rowBoardOffset)
                );
#else
            dragStartSquare = new Square(
                (byte)(8 - startIndex / rowLength),
                (byte)((startIndex % rowLength) - rowBoardOffset)
                );
#endif
        }

        private void boardDisplay_DoubleClick(object sender, EventArgs e)
        {
            MouseEventArgs me = (MouseEventArgs)e;
            Point clickPos = me.Location;
            clickPos.X -= (int)(boardDisplay.Font.Size / 2.01); // by default, if you're on the right half of a char, it'll select the next one, fix that.

            int rowLength = 12;
            int rowBoardOffset = 1;
            int startIndex = boardDisplay.GetCharIndexFromPosition(clickPos);

            dragStartPosition = boardDisplay.SelectionStart = startIndex;
            boardDisplay.SelectionLength = 1;

#if useV2
            Square.Rank thisRow = (Square.Rank)(8 - startIndex / rowLength);
            Square.File thisCol = (Square.File)((startIndex % rowLength) - rowBoardOffset);

            if (ckInvertBoard.Checked)
            {
                thisRow = (7 - thisRow);
                thisCol = (7 - thisCol);
            }
#else
            byte thisRow = (byte)(8 - startIndex / rowLength);
            byte thisCol = (byte)((startIndex % rowLength) - rowBoardOffset);

            if (ckInvertBoard.Checked)
            {
                thisRow = (byte)(7 - thisRow);
                thisCol = (byte)(7 - thisCol);
            }
#endif

            if (!inDrag)   // start one...
            {
                // use last clicked location as the point
                dragStartSquare = new Square(thisRow, thisCol);
                inDrag = true;

                CorrMoveText.Text = "st->" + dragStartSquare.ToString();
            }
            else  // end one
            {
                dragEndSquare = new Square(thisRow, thisCol);

                // at this point, generate the appropriate move text and populate the proposed move text box
                // Piece-target square, augmented for source square, capture - including ep, castle
#if !useV2
                Ply p = curGame.CreateMove(dragStartSquare, dragEndSquare);
                CorrMoveText.Text = (p != null ? p.refToken.tokenString : "");
#endif

                CleanupDrag();
            }

        }
        private void CleanupDrag()
        {
            dragStartPosition = -1;
            dragStartSquare = dragEndSquare = null;
            inDrag = false;

            if (refCursor != null)
                this.Cursor = refCursor;
            CorrUpdate.Enabled = false;
        }
        private void boardDisplay_Click(object sender, EventArgs e)
        {
            dragStartPosition = boardDisplay.SelectionStart;
        }

        bool openLastFile = false;
        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            openLastFileMenu.Checked = openLastFile = !openLastFile;
            Settings.AppSettings["OpenLast"] = (openLastFile ? "y" : "n");
        }
        private void OpenLastFile()
        {
            openLastFile = (Settings.AppSettings["OpenLast"] == "y");
            openLastFileMenu.Checked = openLastFile;

            if (openLastFile && Settings.AppSettings.Count("MRUList") > 0)
            {
                LoadGamesFromFile(Settings.AppSettings["MRUList", Settings.AppSettings.Count("MRUList") - 1]);
            }
        }

        private void corrTemplateList_SelectedIndexChanged(object sender, EventArgs e)
        {
            // update the setting appropriately
            Settings.AppSettings["CorrTemplate"] = corrTemplateList.SelectedItem.ToString();
        }

        private void corrName_TextChanged(object sender, EventArgs e)
        {
            Settings.AppSettings["MyName"] = corrName.Text;
        }

        private void PopMoveButton_Click(object sender, EventArgs e)
        {
            curGame.Plies.Remove(curGame.Plies.ElementAt(curGame.Plies.Count - 1));
            if (saveFileOnUpdate)
            {
                refGameList.Save(curPGNFileLoc, Settings.AppSettings["MyName"]);
                ReloadGamesFromFile();
            }
            else
            {
                curGame.ResetPosition();
                curGame.AdvancePosition(curGame.Plies.Count);

                Redraw(false, false, true, false);
            }
        }

        private void resultCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
#if useV2
            curGame.Tags["Result"] = (string)resultCombo.SelectedItem;
#else
            curGame.GameTerm = new PGNTerminator((string)resultCombo.SelectedItem);
            foreach (PGNToken t in curGame.PGNtokens)
                if (t.tokenType == PGNTokenType.Terminator)
                {
                    curGame.PGNtokens.Remove(t);
                    break;
                }
            curGame.PGNtokens.Add(curGame.GameTerm);
            Game oldGame = curGame;

            curGame.PGNSource = curGame.GeneratePGNSource(Game.GameSaveOptions.MoveListOnly);
#endif
            if (!updatingDisplay && saveFileOnUpdate)
            {
                refGameList.Save(curPGNFileLoc, Settings.AppSettings["MyName"]);
                ReloadGamesFromFile();
            }
            else
            {
                Redraw(false, false, false, true);
            }
        }

        private Square GetBoardPositionFromMouseLocation(MouseEventArgs e)
        {
            MouseEventArgs me = (MouseEventArgs)e;

            int rowLength = 12;
            int rowBoardOffset = 1;
            int startIndex = GetCharIndexFromMouseLocation(e);

            dragStartPosition = boardDisplay.SelectionStart = startIndex;
            boardDisplay.SelectionLength = 1;

#if useV2
            Square.Rank thisRow = (Square.Rank)(8 - startIndex / rowLength);
            Square.File thisCol = (Square.File)((startIndex % rowLength) - rowBoardOffset);

            if (ckInvertBoard.Checked)
            {
                thisRow = (7 - thisRow);
                thisCol = (7 - thisCol);
            }
#else
            byte thisRow = (byte)(8 - startIndex / rowLength);
            byte thisCol = (byte)((startIndex % rowLength) - rowBoardOffset);

            if (ckInvertBoard.Checked)
            {
                thisRow = (byte)(7 - thisRow);
                thisCol = (byte)(7 - thisCol);
            }
#endif

            return new Square(thisRow, thisCol);
        }
        private int GetCharIndexFromMouseLocation(MouseEventArgs e)
        {
            MouseEventArgs me = (MouseEventArgs)e;
            Point clickPos = me.Location;
            clickPos.X -= (int)(boardDisplay.Font.Size / 2.01); // by default, if you're on the right half of a char, it'll select the next one, fix that.

            return boardDisplay.GetCharIndexFromPosition(clickPos);
        }


        private void boardDisplay_MouseDown(object sender, MouseEventArgs e)
        {
            Square boardPos = GetBoardPositionFromMouseLocation(e);
            Piece refPc = (curGame.CurrentPosition.board.ContainsKey(boardPos) ? curGame.CurrentPosition.board[boardPos] : null);

            if (!inDrag && refPc != null && refPc.color == curGame.CurrentPosition.onMove)   // start one...
            {
                // use last clicked location as the point
                dragStartSquare = boardPos;
                inDrag = true;

                CorrMoveText.Text = "st->" + dragStartSquare.ToString();

                int startIndex = GetCharIndexFromMouseLocation(e);
                // at this point we can also change the cursor...
                refCursor = this.Cursor;
                this.Cursor = new Cursor("Resources/Piece.cur");

                dragStartPosition = boardDisplay.SelectionStart = startIndex;
                boardDisplay.SelectionLength = 1;
                CorrUpdate.Enabled = true;
            }
            else  // end one
            {
                CleanupDrag();
            }
        }

        private void boardDisplay_MouseUp(object sender, MouseEventArgs e)
        {
            Square boardPos = GetBoardPositionFromMouseLocation(e);

            if (inDrag)   // end one...
            {
                dragEndSquare = boardPos;

                // at this point, generate the appropriate move text and populate the proposed move text box
                // Piece-target square, augmented for source square, capture - including ep, castle
#if useV2
                Ply p = curGame.CreateMove(dragStartSquare, dragEndSquare);
                string thisText = PGNPly.GeneratePGNSource(curGame.CurrentPosition, p, curGame.Plies.Count, PGNGame.GameSaveOptions.MoveListOnly, false);
                CorrMoveText.Text = thisText;
#else
                Ply p = curGame.CreateMove(dragStartSquare, dragEndSquare);
                CorrMoveText.Text = (p != null ? p.refToken.tokenString : "");
#endif
                dragEndPosition = GetCharIndexFromMouseLocation(e);
                boardDisplay.SelectionStart = dragEndPosition;
                boardDisplay.SelectionLength = 1;

                CorrUpdate_Click(null, null);   // updates the display as well (calls Redraw)
            }
            CleanupDrag();
        }

        private void boardDisplay_MouseMove(object sender, MouseEventArgs e)
        {
            if (inDrag)
            {
                boardDisplay.SelectionStart = dragStartPosition;
                boardDisplay.SelectionLength = 1;
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (FileHasChanged)
                refGameList.Save(curPGNFileLoc, Settings.AppSettings["MyName"]);
            FileHasChanged = false;
        }

        private bool CheckFileChanges()
        {
            // Confirm user wants to close
            if (FileHasChanged &&
                        MessageBox.Show(this, "There are unsaved changes,\nare you sure you want to close?", "Closing", MessageBoxButtons.YesNo) == DialogResult.No)
                return false;
            return true;
        }
        private void PGNViewer_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.WindowsShutDown) return;

            if (!CheckFileChanges())
                e.Cancel = true;
        }
        PGNGame drawGame = null;
        private void Redraw(bool newFile, bool newGame, bool newMove, bool newMetaData)   // if curGame is in a consistent state here, we should be able to update it based on actions, then call this method - done.
        {
            if (newFile || newMove || newMetaData)
                UpdateGameListDisplay();    // refreshes the game list on the left side of the display - doesn't select a game...
            UpdatePGNText();
            DrawBoard();                // updates the board for the curGame.curPosition
            UpdateCorrespondence();     // this loads all moves into the corr grid, updates refl time text and per move, and resultsCombo based on curGame.GameTerm.value
            HighlightPGNMove();         // updates the board for the curGame.curPly
            HighlightCorrMoveInGrid();  // highlights the appropriate position in the corr Grid
            UpdateAnalysis();           // if the analysis engine is running, update the position it's looking at
            UpdateFormTitle();
            if (newMove || newMetaData)
                FileHasChanged = true;
        }

        private void tagEditorGrid_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            if (curGame == null || initTags)
                return;
            return;

            initTags = true;
            string newROwName = "NewTag";
            for (int i = 1; i < 1000 && curGame.Tags.Keys.Contains(newROwName); i++)
                newROwName = "NewTag" + i.ToString();
            tagEditorGrid.Rows[e.RowIndex].Cells[0].Value = newROwName;
            tagEditorGrid.Rows[e.RowIndex].Cells[1].Value = "NewValue";
            curGame.Tags.Add(newROwName, "NewValue");
            initTags = false;
        }

        private void tagEditorGrid_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            if (curGame == null || initTags)
                return;

        }

        private string[] PGNSTR = { "Event", "Site", "Date", "Round", "White", "Black", "Result" };

        private void tagEditorGrid_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (curGame == null || initTags)
                return;
            string oldkey = "";
            if (e.RowIndex < curGame.Tags.Count)
                oldkey = curGame.Tags.Keys.ElementAt(e.RowIndex);
            string newVal = tagEditorGrid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
            string oldVal = oldkey == "" ? "" : curGame.Tags[oldkey];
            if (e.ColumnIndex == 0)    // tag name
            {
                // can't change SevenTagRoster keys
                if (PGNSTR.Contains(oldkey))
                    MessageBox.Show("Can't modify a Seven Tag Roster entry");
                else
                    if (newVal == "")
                        MessageBox.Show("Tag name cannot be empty");
                    else
                        if (newVal != oldkey && curGame.Tags.Keys.Contains(newVal))
                            MessageBox.Show("Tag name must be unique");
                        else
                        {
                            // ok = go ahead and update the Tags dictionary
                            if (oldkey != "")
                                curGame.Tags.Remove(oldkey);
                            curGame.Tags.Add(newVal, oldVal);
                        }
            }
            else   // value
            {
                curGame.Tags[oldkey] = newVal;
            }
        }

        private void corrGridView_DoubleClick(object sender, EventArgs e)
        {
            MoveEditor editorDialog = new MoveEditor();
            editorDialog.Init(curGame.Plies.ElementAt(curGame.curPly - 1));
            if (editorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                // update the move time and penalty time
                Ply outPly = editorDialog.Extract();
                UpdateCommentTime(curGame.Plies.ElementAt(curGame.curPly - 1).comments, CommentTime(outPly.comments));
                UpdatePenaltyTime(curGame.Plies.ElementAt(curGame.curPly - 1).comments, PenaltyTime(outPly.comments), curGame.curPly - 1);
                Redraw(false, false, false, true);
            }
        }

    }
}
