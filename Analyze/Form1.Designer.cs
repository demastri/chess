namespace Analyze
{
    partial class AnalyzeForm
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
            this.PGNLoc = new System.Windows.Forms.TextBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.OpenMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.FileStatsButton = new System.Windows.Forms.Button();
            this.AnalysisButton = new System.Windows.Forms.Button();
            this.TotalGamesLabel = new System.Windows.Forms.Label();
            this.TotalPliesLabel = new System.Windows.Forms.Label();
            this.TotalRatedLabel = new System.Windows.Forms.Label();
            this.UniquePosLabel = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.AnalysisIndexLabel = new System.Windows.Forms.Label();
            this.lastAnalysisString = new System.Windows.Forms.Label();
            this.UseFarm = new System.Windows.Forms.CheckBox();
            this.nbrEngines = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.useLocalFarm = new System.Windows.Forms.CheckBox();
            this.timeEntry = new System.Windows.Forms.TextBox();
            this.depthEntry = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // PGNLoc
            // 
            this.PGNLoc.Location = new System.Drawing.Point(12, 27);
            this.PGNLoc.Name = "PGNLoc";
            this.PGNLoc.Size = new System.Drawing.Size(259, 20);
            this.PGNLoc.TabIndex = 1;
            this.PGNLoc.Text = "D:\\Projects\\Workspaces\\BBRepos\\Chess\\Data\\interesting.pgn";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(344, 24);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.OpenMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // OpenMenuItem
            // 
            this.OpenMenuItem.Name = "OpenMenuItem";
            this.OpenMenuItem.Size = new System.Drawing.Size(103, 22);
            this.OpenMenuItem.Text = "&Open";
            this.OpenMenuItem.Click += new System.EventHandler(this.OpenMenuItem_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.DefaultExt = "pgn";
            this.openFileDialog1.Filter = "PGN FIles|*.pgn|All Files|*.*";
            // 
            // FileStatsButton
            // 
            this.FileStatsButton.Location = new System.Drawing.Point(196, 53);
            this.FileStatsButton.Name = "FileStatsButton";
            this.FileStatsButton.Size = new System.Drawing.Size(75, 23);
            this.FileStatsButton.TabIndex = 3;
            this.FileStatsButton.Text = "File Stats";
            this.FileStatsButton.UseVisualStyleBackColor = true;
            this.FileStatsButton.Click += new System.EventHandler(this.FileStatsButton_Click);
            // 
            // AnalysisButton
            // 
            this.AnalysisButton.Location = new System.Drawing.Point(196, 82);
            this.AnalysisButton.Name = "AnalysisButton";
            this.AnalysisButton.Size = new System.Drawing.Size(75, 23);
            this.AnalysisButton.TabIndex = 4;
            this.AnalysisButton.Text = "Do Analysis";
            this.AnalysisButton.UseVisualStyleBackColor = true;
            this.AnalysisButton.Click += new System.EventHandler(this.AnalysisButton_Click);
            // 
            // TotalGamesLabel
            // 
            this.TotalGamesLabel.AutoSize = true;
            this.TotalGamesLabel.Location = new System.Drawing.Point(12, 50);
            this.TotalGamesLabel.Name = "TotalGamesLabel";
            this.TotalGamesLabel.Size = new System.Drawing.Size(82, 13);
            this.TotalGamesLabel.TabIndex = 5;
            this.TotalGamesLabel.Text = "Total Games: ---";
            // 
            // TotalPliesLabel
            // 
            this.TotalPliesLabel.AutoSize = true;
            this.TotalPliesLabel.Location = new System.Drawing.Point(12, 76);
            this.TotalPliesLabel.Name = "TotalPliesLabel";
            this.TotalPliesLabel.Size = new System.Drawing.Size(71, 13);
            this.TotalPliesLabel.TabIndex = 6;
            this.TotalPliesLabel.Text = "Total Plies: ---";
            // 
            // TotalRatedLabel
            // 
            this.TotalRatedLabel.AutoSize = true;
            this.TotalRatedLabel.Location = new System.Drawing.Point(12, 63);
            this.TotalRatedLabel.Name = "TotalRatedLabel";
            this.TotalRatedLabel.Size = new System.Drawing.Size(78, 13);
            this.TotalRatedLabel.TabIndex = 7;
            this.TotalRatedLabel.Text = "Total Rated: ---";
            // 
            // UniquePosLabel
            // 
            this.UniquePosLabel.AutoSize = true;
            this.UniquePosLabel.Location = new System.Drawing.Point(13, 87);
            this.UniquePosLabel.Name = "UniquePosLabel";
            this.UniquePosLabel.Size = new System.Drawing.Size(77, 13);
            this.UniquePosLabel.TabIndex = 8;
            this.UniquePosLabel.Text = "Unique Pos: ---";
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // AnalysisIndexLabel
            // 
            this.AnalysisIndexLabel.AutoSize = true;
            this.AnalysisIndexLabel.Location = new System.Drawing.Point(12, 162);
            this.AnalysisIndexLabel.Name = "AnalysisIndexLabel";
            this.AnalysisIndexLabel.Size = new System.Drawing.Size(126, 13);
            this.AnalysisIndexLabel.TabIndex = 9;
            this.AnalysisIndexLabel.Text = "Current Analysis Index: ---";
            // 
            // lastAnalysisString
            // 
            this.lastAnalysisString.AutoSize = true;
            this.lastAnalysisString.Location = new System.Drawing.Point(13, 240);
            this.lastAnalysisString.Name = "lastAnalysisString";
            this.lastAnalysisString.Size = new System.Drawing.Size(48, 13);
            this.lastAnalysisString.TabIndex = 10;
            this.lastAnalysisString.Text = "Analysis:";
            // 
            // UseFarm
            // 
            this.UseFarm.AutoSize = true;
            this.UseFarm.Checked = true;
            this.UseFarm.CheckState = System.Windows.Forms.CheckState.Checked;
            this.UseFarm.Location = new System.Drawing.Point(196, 112);
            this.UseFarm.Name = "UseFarm";
            this.UseFarm.Size = new System.Drawing.Size(71, 17);
            this.UseFarm.TabIndex = 11;
            this.UseFarm.Text = "Use Farm";
            this.UseFarm.UseVisualStyleBackColor = true;
            // 
            // nbrEngines
            // 
            this.nbrEngines.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.nbrEngines.FormattingEnabled = true;
            this.nbrEngines.Items.AddRange(new object[] {
            "1",
            "2",
            "4",
            "6"});
            this.nbrEngines.Location = new System.Drawing.Point(145, 110);
            this.nbrEngines.Name = "nbrEngines";
            this.nbrEngines.Size = new System.Drawing.Size(45, 21);
            this.nbrEngines.TabIndex = 12;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(94, 116);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(45, 13);
            this.label1.TabIndex = 13;
            this.label1.Text = "Engines";
            // 
            // useLocalFarm
            // 
            this.useLocalFarm.AutoSize = true;
            this.useLocalFarm.Checked = true;
            this.useLocalFarm.CheckState = System.Windows.Forms.CheckState.Checked;
            this.useLocalFarm.Location = new System.Drawing.Point(196, 135);
            this.useLocalFarm.Name = "useLocalFarm";
            this.useLocalFarm.Size = new System.Drawing.Size(78, 17);
            this.useLocalFarm.TabIndex = 14;
            this.useLocalFarm.Text = "Local Farm";
            this.useLocalFarm.UseVisualStyleBackColor = true;
            // 
            // timeEntry
            // 
            this.timeEntry.Location = new System.Drawing.Point(109, 178);
            this.timeEntry.Name = "timeEntry";
            this.timeEntry.Size = new System.Drawing.Size(41, 20);
            this.timeEntry.TabIndex = 15;
            this.timeEntry.Text = "-1";
            // 
            // depthEntry
            // 
            this.depthEntry.Location = new System.Drawing.Point(196, 178);
            this.depthEntry.Name = "depthEntry";
            this.depthEntry.Size = new System.Drawing.Size(41, 20);
            this.depthEntry.TabIndex = 16;
            this.depthEntry.Text = "20";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(51, 185);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(52, 13);
            this.label2.TabIndex = 17;
            this.label2.Text = "Time (ms)";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(156, 185);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(36, 13);
            this.label3.TabIndex = 18;
            this.label3.Text = "Depth";
            // 
            // AnalyzeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(344, 313);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.depthEntry);
            this.Controls.Add(this.timeEntry);
            this.Controls.Add(this.useLocalFarm);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.nbrEngines);
            this.Controls.Add(this.UseFarm);
            this.Controls.Add(this.lastAnalysisString);
            this.Controls.Add(this.AnalysisIndexLabel);
            this.Controls.Add(this.UniquePosLabel);
            this.Controls.Add(this.TotalRatedLabel);
            this.Controls.Add(this.TotalPliesLabel);
            this.Controls.Add(this.TotalGamesLabel);
            this.Controls.Add(this.AnalysisButton);
            this.Controls.Add(this.FileStatsButton);
            this.Controls.Add(this.PGNLoc);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "AnalyzeForm";
            this.Text = "BulkAnalyze";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AnalyzeForm_FormClosing);
            this.Load += new System.EventHandler(this.AnalyzeForm_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox PGNLoc;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem OpenMenuItem;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button FileStatsButton;
        private System.Windows.Forms.Button AnalysisButton;
        private System.Windows.Forms.Label TotalGamesLabel;
        private System.Windows.Forms.Label TotalPliesLabel;
        private System.Windows.Forms.Label TotalRatedLabel;
        private System.Windows.Forms.Label UniquePosLabel;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label AnalysisIndexLabel;
        private System.Windows.Forms.Label lastAnalysisString;
        private System.Windows.Forms.CheckBox UseFarm;
        private System.Windows.Forms.ComboBox nbrEngines;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox useLocalFarm;
        private System.Windows.Forms.TextBox timeEntry;
        private System.Windows.Forms.TextBox depthEntry;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
    }
}

