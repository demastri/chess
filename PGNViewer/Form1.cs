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
using ChessPosition;
using System.Drawing.Text;
using System.Runtime.InteropServices;

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

        List<Game> GameRef;
        Engine AnalysisEngine;
        Game curGame;
        string curPGNFileLoc = "";
        TreeNode inProgOnMoveNode = null;
        TreeNode inProgWaitingNode = null;
        TreeNode complNode = null;
        MRUHandler mruList;

        public PGNViewer()
        {
            InitializeComponent();
            mruList = new MRUHandler();
            mruList.InitList();
            UpdateMRUMenu();
        }

        private void ReloadGamesFromFile()
        {
            LoadGamesFromFile(curPGNFileLoc);
        }
        private void LoadGamesFromFile(string fn)
        {
            curPGNFileLoc = fn;
            GameRef = Game.ReadPGNFile(curPGNFileLoc);
            GameList.Nodes.Clear();
            if (curDisplayMode == 3)    // corr
            {
                inProgOnMoveNode = GameList.Nodes.Add("In Progress - On Move");
                inProgWaitingNode = GameList.Nodes.Add("In Progress - Waiting");
                complNode = GameList.Nodes.Add("Complete");
                foreach (Game g in GameRef)
                {
                    bool ImWhite = g.Tags["White"] == corrName.Text;
                    bool WOnMove = g.Plies.Count % 2 == 0;
                    string s = g.Tags["Date"] + " " + g.Tags["White"] + "-" + g.Tags["Black"];
                    if (g.Tags["Result"] == "*" || g.Tags["Result"] == "")
                    {
                        if ((ImWhite && WOnMove) || (!ImWhite && !WOnMove))
                            inProgOnMoveNode.Nodes.Add(s, s);
                        else
                            inProgWaitingNode.Nodes.Add(s, s);
                    }
                    else
                        complNode.Nodes.Add(s, s);
                }
            }
            else
            {
                foreach (Game g in GameRef)
                {
                    GameList.Nodes.Add(g.Tags["Date"] + " " + g.Tags["White"] + "-" + g.Tags["Black"]);
                }
            }
            curGame = null;
            DrawBoard();
            UpdateCorrespondence();
            HighlightPGNMove();
            HighlightCorrMove();
            UpdateAnalysis();
            UpdateFormTitle();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                LoadGamesFromFile(openFileDialog1.FileName);
                mruList.AddMRUFile(openFileDialog1.FileName);
                mruList.UpdateList();
                UpdateMRUMenu();
            }
        }

        private void PGNViewer_Load(object sender, EventArgs e)
        {
            LoadFont();
            curGame = null;
            AnalysisEngine = null;
            SetMode("correspondence");
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
            CleanupDrag();

            int limitingSize = boardDisplay.Width < boardDisplay.Height ? boardDisplay.Width : boardDisplay.Height;
            double fontFactor = 13.75;
            int fontSize = (int)(limitingSize / fontFactor);

            if (fontSize == 0)
                return;
            boardDisplay.Font = new Font(pfc.Families[0], fontSize);

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
                    thisBoard = PokePiece(thisBoard, sq.row + 1, sq.col + 1, curGame.CurrentPosition.board[sq]);
                }
                FENText.Text = curGame.ToFEN();
            }
            else
                FENText.Text = "";

            boardDisplay.Text = thisBoard;
            boardDisplay.Select(0, 0);
        }

        int curMoveTextStart = 0;
        int curMoveTextEnd = 0;
        private int findPlyAtLocation(int loc)
        {
            for (int i = 0; i <= curGame.Plies.Count; i++)
            {
                ChangePosition(0);
                ChangePosition(i);
                if (curMoveTextStart <= loc && loc <= curMoveTextEnd)
                    return i;
                if (curMoveTextStart > loc)
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
                curMoveTextStart = curMoveTextEnd = 0;
            }
            else
            {
                int moveNbr = (thisPly / 2) + 1;
                bool haveMove = false;
                bool nextPly = !isBlack;
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
            }
        }
        private void HighlightCorrMove()
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

        private string PokePiece(string refStr, int rank, int file, Piece pc) // rank/file ranged 1-8
        {
            if (ckInvertBoard.Checked)
            {
                rank = 9 - rank;
                file = 9 - file;
            }
            int locToPoke = ((10 + Environment.NewLine.Length) * (1 + 8 - rank)) + (file);
            bool isWhite = (((((rank - 1) % 2) == ((file - 1) % 2)) ? 1 : 0) == 0);   // 1 => b
            char pcChar = isWhite ? Char.ToLower(pc.ToChess7Char) : Char.ToUpper(pc.ToChess7Char);  // upper case is on a dark square...

            return Utilities.Utils.SwapChar(refStr, locToPoke, pcChar);
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
            AnalysisEngine = Engine.InitEngine(EngineList.Text);
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
                    AnalysisEngine.SetPostion(new EngineParameters("stockfish", 50, -1), curGame.ToFEN());
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
            HighlightCorrMove();
            UpdateAnalysis();
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

        private void InitCorrTemplateList()
        {
            corrTemplateList.Items.Clear();
            DirectoryInfo templates = new DirectoryInfo(Directory.GetCurrentDirectory() + "/Resources");
            FileInfo[] tFiles = templates.GetFiles("*.html");
            foreach (FileInfo fi in tFiles)
            {
                corrTemplateList.Items.Add(fi.Name.Substring(0, fi.Name.Length - 5));
            }
            corrTemplateList.SelectedIndex = corrTemplateList.Items.Count - 1;
        }

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

            DateTime lastMoveTime = DateTime.MinValue;
            totalCorrTimeW = 30 + (30 * (curGame.Plies.Count / 20));
            totalCorrTimeB = 30 + (30 * ((curGame.Plies.Count - 1) / 20));
            usedCorrTimeW = 0;
            usedCorrTimeB = 0;
            lastCorrTimeW = 0;
            lastCorrTimeB = 0;
            corrTimes = new List<int>();

            for (int i = 0; i < curGame.Plies.Count; i++)
            {
                Ply p = curGame.Plies[i];
                PGNToken s = curGame.PGNtokens[i];
                int thisReflTime = 0;

                corrGridView.Rows[i / 2].Cells["MoveNbr"].Value = 1 + i / 2;
                corrGridView.Rows[i / 2].Cells[(i % 2 == 0) ? "White" : "Black"].Value = p.refToken.tokenString;
                if (p.comment != null)
                {
                    DateTime thisMoveTime = CommentTime(p.comment.tokenString);
                    if (thisMoveTime != DateTime.MinValue && lastMoveTime != DateTime.MinValue && thisMoveTime > lastMoveTime)
                    {
                        TimeSpan refl = thisMoveTime - lastMoveTime;
                        thisReflTime = refl.Days;
                    }
                    corrGridView.Rows[i / 2].Cells[(i % 2 == 0) ? "WMoveTime" : "BMoveTime"].Value = p.comment.value;
                    lastMoveTime = thisMoveTime;
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
            string selectedText = curGame.Tags["Date"] + " " + curGame.Tags["White"] + "-" + curGame.Tags["Black"];

            string possMove = CorrMoveText.Text + " ";
            DateTime possTime = CorrMoveTime.Value;

            // this a valid time for the game?
            foreach (Ply p in curGame.Plies)
            {
                if (p.comment != null)
                {
                    DateTime commentTime = CommentTime(p.comment.tokenString);
                    if (commentTime > possTime)
                    {
                        MessageBox.Show("There was a later time already posted to this game");
                        return;
                    }
                }
            }
            // is this a valid move in the position?
            // append this to the game
            // ###% actually refactor this so almost all of it lives in the game class...
            Ply newMove = null;
            if (possMove.Trim() == "")
            {
                MessageBox.Show("There was a problem with the provided move");
                return;
            }
            PGNMoveString moveStr = new PGNMoveString(possMove, 0);
            curGame.ResetPosition();
            curGame.AdvancePosition(curGame.Plies.Count);
            if ((newMove = curGame.HandleMove(moveStr)) == null)
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
            PGNText.Text = PGNText.Text.Substring(0, PGNText.Text.Length - Environment.NewLine.Length);
            int curLineLength = PGNText.Text.Length - PGNText.Text.LastIndexOf(Environment.NewLine);
            int newMovePairLength = 4 + 2 * (5 + 15 + 3); // move nbr + 2*(move, time, pad)
            int maxLineLength = 100;
            if (isAWhiteMove && curLineLength + newMovePairLength > maxLineLength)    // put it a new line...
                PGNText.Text += Environment.NewLine;

            if (isAWhiteMove)
            {
                string nbrText = (curGame.Plies.Count / 2 + 1).ToString() + ".";
                PGNMoveNumber moveNbr = new PGNMoveNumber(nbrText, 0);
                curGame.PGNtokens.Add(moveNbr);
                PGNText.Text += nbrText;
            }

            curGame.PGNtokens.Add(moveStr);
            newMove.refToken.startLocation = PGNText.Text.Length;
            PGNText.Text += possMove;

            PGNComment timeComment = new PGNComment(possTime.ToString("(MM/dd/yyyy HHmm)"), 0);
            curGame.PGNtokens.Add(timeComment);
            PGNText.Text += timeComment.tokenString + " ";
            newMove.comment = timeComment;

            PGNText.Text += Environment.NewLine;

            if (termToken != null)
                curGame.PGNtokens.Add(termToken);

            UpdateCorrespondence();

            // move to the end...
            curGame.ResetPosition();
            curGame.AdvancePosition(curGame.Plies.Count);
            DrawBoard();
            HighlightPGNMove();
            HighlightCorrMove();

            // save the updated game / file
            Game.SavePGNFile(curPGNFileLoc, GameRef);

            ReloadGamesFromFile();

            TreeNode n = GameList.Nodes.Find(selectedText, true)[0];
            GameList.SelectedNode = n;
            GameList_SelectedIndexChanged(null, null);

            CorrMoveText.Text = "";
        }

        private void CorrPublish_Click(object sender, EventArgs e)
        {
            // generate the templated grid
            // show in a browser / copy to clipboard...

            CorrResponseDialog respDialog = new CorrResponseDialog();
            string htmlResp;

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
        private DateTime CommentTime(string st)
        {
            // (05/10/2015 1224)
            DateTime thisTime;
            if (DateTime.TryParseExact(st, "(MM/dd/yyyy HHmm)", CultureInfo.InvariantCulture, DateTimeStyles.None, out thisTime))
                return thisTime;
            return DateTime.MinValue;
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
                            if (lastWPly >= 0)
                                tempStr = curGame.Plies[lastWPly].refToken.tokenString;
                            refStr = refStr.Replace(token, tempStr);
                            break;
                        case "WhiteMoveTime":
                            lastWPly -= 2 * offset;
                            if (lastWPly >= 0 && curGame.Plies[lastWPly].comment != null)
                                tempStr = curGame.Plies[lastWPly].comment.value + " " + corrTZ.Text;
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
                            if (lastWPly >= 0 && curGame.Plies[lastWPly].comment != null)
                                tempStr = curGame.Plies[lastWPly].comment.value + " " + corrTZ.Text;
                            refStr = refStr.Replace(token, tempStr);
                            break;

                        case "BlackMoveNbr":
                            lastBPly -= 2 * offset;
                            if (lastBPly >= 0)
                                tempStr = ((lastBPly / 2) + 1).ToString() + ". ... ";
                            refStr = refStr.Replace(token, tempStr);
                            break;
                        case "BlackMove":
                            lastBPly -= 2 * offset;
                            if (lastBPly >= 0 && curGame.Plies.Count > lastBPly)
                                tempStr = curGame.Plies[lastBPly].refToken.tokenString;
                            refStr = refStr.Replace(token, tempStr);
                            break;
                        case "BlackMoveTime":
                            lastBPly -= 2 * offset;
                            if (lastBPly >= 0 && curGame.Plies.Count > lastBPly && curGame.Plies[lastBPly].comment != null)
                                tempStr = curGame.Plies[lastBPly].comment.value + " " + corrTZ.Text;
                            refStr = refStr.Replace(token, tempStr);
                            break;
                        case "BlackReflTime":
                            lastBPly -= 2 * offset;
                            if (lastBPly >= 0 && curGame.Plies.Count > lastBPly)
                                tempStr = corrTimes[lastBPly].ToString();
                            refStr = refStr.Replace(token, tempStr);
                            break;
                        case "BlackClockStart":
                            lastBPly -= 1 + (2 * offset);
                            if (lastBPly >= 0 && curGame.Plies.Count > lastBPly && curGame.Plies[lastBPly].comment != null)
                                tempStr = curGame.Plies[lastBPly].comment.value + " " + corrTZ.Text;
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
                        case "PGNSource":
                            // actually just move numbers and moves here...
                            refStr = refStr.Replace(token, "<br>" + curGame.BuildMoveList() + "<br><br>");
                            break;
                        case "Diagram":
                            // pull the current text from the board display
                            string boardText = boardDisplay.Text;
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
            DrawBoard();

        }

        private void ckInvertBoard_Click(object sender, EventArgs e)
        {
            DrawBoard();
        }

        private void GameList_SelectedIndexChanged(object sender, TreeViewEventArgs e)
        {
            PGNText.Text = "";
            curGame = null;
            PGNText.Text = "";
            if (GameList.SelectedNode != null)
            {
                foreach (Game g in GameRef)
                    if (g.Tags["Date"] + " " + g.Tags["White"] + "-" + g.Tags["Black"] == GameList.SelectedNode.Text)
                    {
                        curGame = g;
                        break;
                    }
                if (curGame != null)
                {
                    PGNText.Text = curGame.PGNSource;
                    curGame.ResetPosition();
                }
            }
            if (curGame != null)
            {
                ckInvertBoard.Checked = (curGame.Tags["Black"] == corrName.Text);
                if (curGame.Tags["Result"] == "*" || curGame.Tags["Result"] == "")
                    curGame.AdvancePosition(curGame.Plies.Count);
            }
            DrawBoard();
            HighlightPGNMove();
            UpdateCorrespondence();
            HighlightCorrMove();
            UpdateAnalysis();
            UpdateFormTitle();
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
                int mruIndex = mruList.MRUFiles.Count - i;
                if (mruIndex >= 0)
                {
                    menu.Text = "&" + i.ToString() + " - " + mruList.MRUFiles[mruIndex];
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

            ChangePosition(0);
            ChangePosition(newPly);
        }
        bool inDrag = false;
        int dragStartPosition = -1;
        Square dragStartSquare = null;
        Square dragEndSquare = null;
        private void InitDrag(MouseEventArgs e)
        {
            int rowLength = 12;
            int rowBoardOffset = 1;
            int startIndex = boardDisplay.SelectionStart;
            dragStartSquare = new Square(
                (byte)(8 - startIndex / rowLength),
                (byte)((startIndex % rowLength) - rowBoardOffset)
                );
        }

        private void boardDisplay_DoubleClick(object sender, EventArgs e)
        {
            int rowLength = 12;
            int rowBoardOffset = 1;
            int startIndex = boardDisplay.SelectionStart;

            startIndex = dragStartPosition;
            boardDisplay.SelectionStart = startIndex;
            boardDisplay.SelectionLength = 1;

            byte thisRow = (byte)(8 - startIndex / rowLength);
            byte thisCol = (byte)((startIndex % rowLength) - rowBoardOffset);

            if (ckInvertBoard.Checked)
            {
                thisRow = (byte)(7 - thisRow);
                thisCol = (byte)(7 - thisCol);
            }

            if (!inDrag)   // start one...
            {
                // use last clicked location as the point
                dragStartSquare = new Square( thisRow, thisCol );
                inDrag = true;
            }
            else  // end one
            {
                dragEndSquare = new Square( thisRow, thisCol );

                // at this point, generate the appropriate move text and populate the proposed move text box
                // Piece-target square, augmented for source square, capture - including ep, castle

                Ply p = curGame.CreateMove( dragStartSquare, dragEndSquare );
                CorrMoveText.Text = (p != null ? p.refToken.tokenString : "");

                CleanupDrag();
            }

        }
        private void CleanupDrag()
        {
            dragStartSquare = dragEndSquare = null;
            inDrag = false;
        }
        private void boardDisplay_Click(object sender, EventArgs e)
        {
            dragStartPosition = boardDisplay.SelectionStart;
        }

    }
}
