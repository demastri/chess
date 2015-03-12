namespace PGNViewer
{
    partial class PGNViewer
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.GameList = new System.Windows.Forms.ListView();
            this.boardDisplay = new System.Windows.Forms.TextBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.PGNText = new System.Windows.Forms.TextBox();
            this.ResetGameButton = new System.Windows.Forms.Button();
            this.BackButton = new System.Windows.Forms.Button();
            this.FwdButton = new System.Windows.Forms.Button();
            this.FENText = new System.Windows.Forms.TextBox();
            this.EngineList = new System.Windows.Forms.ComboBox();
            this.AnalysisText = new System.Windows.Forms.TextBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.JumpBackButton = new System.Windows.Forms.Button();
            this.JumpFwdButton = new System.Windows.Forms.Button();
            this.JumpToEndButton = new System.Windows.Forms.Button();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // GameList
            // 
            this.GameList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.GameList.Location = new System.Drawing.Point(12, 27);
            this.GameList.MultiSelect = false;
            this.GameList.Name = "GameList";
            this.GameList.Size = new System.Drawing.Size(298, 388);
            this.GameList.TabIndex = 0;
            this.GameList.UseCompatibleStateImageBehavior = false;
            this.GameList.View = System.Windows.Forms.View.List;
            this.GameList.SelectedIndexChanged += new System.EventHandler(this.GameList_SelectedIndexChanged);
            // 
            // boardDisplay
            // 
            this.boardDisplay.Location = new System.Drawing.Point(316, 27);
            this.boardDisplay.Multiline = true;
            this.boardDisplay.Name = "boardDisplay";
            this.boardDisplay.ReadOnly = true;
            this.boardDisplay.Size = new System.Drawing.Size(256, 223);
            this.boardDisplay.TabIndex = 1;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(854, 24);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.openToolStripMenuItem.Text = "&Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // PGNText
            // 
            this.PGNText.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PGNText.HideSelection = false;
            this.PGNText.Location = new System.Drawing.Point(316, 282);
            this.PGNText.Multiline = true;
            this.PGNText.Name = "PGNText";
            this.PGNText.ReadOnly = true;
            this.PGNText.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.PGNText.Size = new System.Drawing.Size(526, 133);
            this.PGNText.TabIndex = 3;
            // 
            // ResetGameButton
            // 
            this.ResetGameButton.Location = new System.Drawing.Point(578, 227);
            this.ResetGameButton.Name = "ResetGameButton";
            this.ResetGameButton.Size = new System.Drawing.Size(30, 23);
            this.ResetGameButton.TabIndex = 4;
            this.ResetGameButton.Text = "|<";
            this.ResetGameButton.UseVisualStyleBackColor = true;
            this.ResetGameButton.Click += new System.EventHandler(this.ResetGameButton_Click);
            // 
            // BackButton
            // 
            this.BackButton.Location = new System.Drawing.Point(650, 227);
            this.BackButton.Name = "BackButton";
            this.BackButton.Size = new System.Drawing.Size(30, 23);
            this.BackButton.TabIndex = 5;
            this.BackButton.Text = "<";
            this.BackButton.UseVisualStyleBackColor = true;
            this.BackButton.Click += new System.EventHandler(this.BackButton_Click);
            // 
            // FwdButton
            // 
            this.FwdButton.Location = new System.Drawing.Point(686, 227);
            this.FwdButton.Name = "FwdButton";
            this.FwdButton.Size = new System.Drawing.Size(30, 23);
            this.FwdButton.TabIndex = 6;
            this.FwdButton.Text = ">";
            this.FwdButton.UseVisualStyleBackColor = true;
            this.FwdButton.Click += new System.EventHandler(this.FwdButton_Click);
            // 
            // FENText
            // 
            this.FENText.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.FENText.Location = new System.Drawing.Point(316, 256);
            this.FENText.Name = "FENText";
            this.FENText.ReadOnly = true;
            this.FENText.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.FENText.Size = new System.Drawing.Size(525, 20);
            this.FENText.TabIndex = 7;
            // 
            // EngineList
            // 
            this.EngineList.FormattingEnabled = true;
            this.EngineList.Items.AddRange(new object[] {
            "None",
            "Stockfish",
            "Crafty"});
            this.EngineList.Location = new System.Drawing.Point(578, 27);
            this.EngineList.Name = "EngineList";
            this.EngineList.Size = new System.Drawing.Size(160, 21);
            this.EngineList.TabIndex = 8;
            this.EngineList.Text = "None";
            this.EngineList.SelectedIndexChanged += new System.EventHandler(this.EngineList_SelectedIndexChanged);
            // 
            // AnalysisText
            // 
            this.AnalysisText.Location = new System.Drawing.Point(578, 54);
            this.AnalysisText.Multiline = true;
            this.AnalysisText.Name = "AnalysisText";
            this.AnalysisText.ReadOnly = true;
            this.AnalysisText.Size = new System.Drawing.Size(254, 167);
            this.AnalysisText.TabIndex = 9;
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // JumpBackButton
            // 
            this.JumpBackButton.Location = new System.Drawing.Point(614, 227);
            this.JumpBackButton.Name = "JumpBackButton";
            this.JumpBackButton.Size = new System.Drawing.Size(30, 23);
            this.JumpBackButton.TabIndex = 10;
            this.JumpBackButton.Text = "<<";
            this.JumpBackButton.UseVisualStyleBackColor = true;
            this.JumpBackButton.Click += new System.EventHandler(this.JumpBackButton_Click);
            // 
            // JumpFwdButton
            // 
            this.JumpFwdButton.Location = new System.Drawing.Point(722, 227);
            this.JumpFwdButton.Name = "JumpFwdButton";
            this.JumpFwdButton.Size = new System.Drawing.Size(30, 23);
            this.JumpFwdButton.TabIndex = 11;
            this.JumpFwdButton.Text = ">>";
            this.JumpFwdButton.UseVisualStyleBackColor = true;
            this.JumpFwdButton.Click += new System.EventHandler(this.JumpFwdButton_Click);
            // 
            // JumpToEndButton
            // 
            this.JumpToEndButton.Location = new System.Drawing.Point(758, 227);
            this.JumpToEndButton.Name = "JumpToEndButton";
            this.JumpToEndButton.Size = new System.Drawing.Size(30, 23);
            this.JumpToEndButton.TabIndex = 12;
            this.JumpToEndButton.Text = ">|";
            this.JumpToEndButton.UseVisualStyleBackColor = true;
            this.JumpToEndButton.Click += new System.EventHandler(this.JumpToEndButton_Click);
            // 
            // PGNViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(854, 427);
            this.Controls.Add(this.JumpToEndButton);
            this.Controls.Add(this.JumpFwdButton);
            this.Controls.Add(this.JumpBackButton);
            this.Controls.Add(this.AnalysisText);
            this.Controls.Add(this.EngineList);
            this.Controls.Add(this.FENText);
            this.Controls.Add(this.FwdButton);
            this.Controls.Add(this.BackButton);
            this.Controls.Add(this.ResetGameButton);
            this.Controls.Add(this.PGNText);
            this.Controls.Add(this.boardDisplay);
            this.Controls.Add(this.GameList);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "PGNViewer";
            this.Text = "PGN Viewer";
            this.Load += new System.EventHandler(this.PGNViewer_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView GameList;
        private System.Windows.Forms.TextBox boardDisplay;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.TextBox PGNText;
        private System.Windows.Forms.Button ResetGameButton;
        private System.Windows.Forms.Button BackButton;
        private System.Windows.Forms.Button FwdButton;
        private System.Windows.Forms.TextBox FENText;
        private System.Windows.Forms.ComboBox EngineList;
        private System.Windows.Forms.TextBox AnalysisText;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button JumpBackButton;
        private System.Windows.Forms.Button JumpFwdButton;
        private System.Windows.Forms.Button JumpToEndButton;
    }
}

