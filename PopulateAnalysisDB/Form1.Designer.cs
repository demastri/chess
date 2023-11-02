namespace PopulateAnalysisDB
{
    partial class Form1
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
            this.textToDBButton = new System.Windows.Forms.Button();
            this.selectFileButton = new System.Windows.Forms.Button();
            this.ckMainLinesOnly = new System.Windows.Forms.CheckBox();
            this.positionsToDBButton = new System.Windows.Forms.Button();
            this.fileLabel = new System.Windows.Forms.Label();
            this.nbrGamesLabel = new System.Windows.Forms.Label();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.pgnTextBox = new System.Windows.Forms.TextBox();
            this.prevGame = new System.Windows.Forms.Button();
            this.nextGame = new System.Windows.Forms.Button();
            this.GameLabel = new System.Windows.Forms.Label();
            this.PullEndPos = new System.Windows.Forms.Button();
            this.EndPosTag = new System.Windows.Forms.Label();
            this.AnalyzePositions = new System.Windows.Forms.Button();
            this.analysisTag = new System.Windows.Forms.Label();
            this.rawCounts = new System.Windows.Forms.TextBox();
            this.wtdCounts = new System.Windows.Forms.TextBox();
            this.inclWWin = new System.Windows.Forms.CheckBox();
            this.inclBWin = new System.Windows.Forms.CheckBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // textToDBButton
            // 
            this.textToDBButton.Location = new System.Drawing.Point(13, 76);
            this.textToDBButton.Name = "textToDBButton";
            this.textToDBButton.Size = new System.Drawing.Size(98, 23);
            this.textToDBButton.TabIndex = 0;
            this.textToDBButton.Text = "Text to DB";
            this.textToDBButton.UseVisualStyleBackColor = true;
            this.textToDBButton.Click += new System.EventHandler(this.textToDBButton_Click);
            // 
            // selectFileButton
            // 
            this.selectFileButton.Location = new System.Drawing.Point(12, 47);
            this.selectFileButton.Name = "selectFileButton";
            this.selectFileButton.Size = new System.Drawing.Size(99, 23);
            this.selectFileButton.TabIndex = 1;
            this.selectFileButton.Text = "Select File";
            this.selectFileButton.UseVisualStyleBackColor = true;
            this.selectFileButton.Click += new System.EventHandler(this.selectFileButton_Click);
            // 
            // ckMainLinesOnly
            // 
            this.ckMainLinesOnly.AutoSize = true;
            this.ckMainLinesOnly.Location = new System.Drawing.Point(125, 111);
            this.ckMainLinesOnly.Name = "ckMainLinesOnly";
            this.ckMainLinesOnly.Size = new System.Drawing.Size(94, 17);
            this.ckMainLinesOnly.TabIndex = 2;
            this.ckMainLinesOnly.Text = "only main lines";
            this.ckMainLinesOnly.UseVisualStyleBackColor = true;
            // 
            // positionsToDBButton
            // 
            this.positionsToDBButton.Location = new System.Drawing.Point(13, 105);
            this.positionsToDBButton.Name = "positionsToDBButton";
            this.positionsToDBButton.Size = new System.Drawing.Size(98, 23);
            this.positionsToDBButton.TabIndex = 3;
            this.positionsToDBButton.Text = "Positions -> DB";
            this.positionsToDBButton.UseVisualStyleBackColor = true;
            // 
            // fileLabel
            // 
            this.fileLabel.AutoSize = true;
            this.fileLabel.Location = new System.Drawing.Point(122, 52);
            this.fileLabel.Name = "fileLabel";
            this.fileLabel.Size = new System.Drawing.Size(83, 13);
            this.fileLabel.TabIndex = 4;
            this.fileLabel.Text = "no File Selected";
            // 
            // nbrGamesLabel
            // 
            this.nbrGamesLabel.AutoSize = true;
            this.nbrGamesLabel.Location = new System.Drawing.Point(122, 81);
            this.nbrGamesLabel.Name = "nbrGamesLabel";
            this.nbrGamesLabel.Size = new System.Drawing.Size(79, 13);
            this.nbrGamesLabel.TabIndex = 5;
            this.nbrGamesLabel.Text = "0 games stored";
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // pgnTextBox
            // 
            this.pgnTextBox.AcceptsReturn = true;
            this.pgnTextBox.Location = new System.Drawing.Point(12, 134);
            this.pgnTextBox.Multiline = true;
            this.pgnTextBox.Name = "pgnTextBox";
            this.pgnTextBox.ReadOnly = true;
            this.pgnTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.pgnTextBox.Size = new System.Drawing.Size(334, 264);
            this.pgnTextBox.TabIndex = 6;
            // 
            // prevGame
            // 
            this.prevGame.Location = new System.Drawing.Point(98, 404);
            this.prevGame.Name = "prevGame";
            this.prevGame.Size = new System.Drawing.Size(33, 23);
            this.prevGame.TabIndex = 7;
            this.prevGame.Text = "<<";
            this.prevGame.UseVisualStyleBackColor = true;
            this.prevGame.Click += new System.EventHandler(this.prevGame_Click);
            // 
            // nextGame
            // 
            this.nextGame.Location = new System.Drawing.Point(198, 404);
            this.nextGame.Name = "nextGame";
            this.nextGame.Size = new System.Drawing.Size(33, 23);
            this.nextGame.TabIndex = 8;
            this.nextGame.Text = ">>";
            this.nextGame.UseVisualStyleBackColor = true;
            this.nextGame.Click += new System.EventHandler(this.nextGame_Click);
            // 
            // GameLabel
            // 
            this.GameLabel.AutoSize = true;
            this.GameLabel.Location = new System.Drawing.Point(149, 409);
            this.GameLabel.Name = "GameLabel";
            this.GameLabel.Size = new System.Drawing.Size(34, 13);
            this.GameLabel.TabIndex = 9;
            this.GameLabel.Text = "0 of 0";
            // 
            // PullEndPos
            // 
            this.PullEndPos.Location = new System.Drawing.Point(417, 47);
            this.PullEndPos.Name = "PullEndPos";
            this.PullEndPos.Size = new System.Drawing.Size(103, 23);
            this.PullEndPos.TabIndex = 10;
            this.PullEndPos.Text = "EndPos <- DB";
            this.PullEndPos.UseVisualStyleBackColor = true;
            this.PullEndPos.Click += new System.EventHandler(this.PullEndPos_Click);
            // 
            // EndPosTag
            // 
            this.EndPosTag.AutoSize = true;
            this.EndPosTag.Location = new System.Drawing.Point(526, 52);
            this.EndPosTag.Name = "EndPosTag";
            this.EndPosTag.Size = new System.Drawing.Size(89, 13);
            this.EndPosTag.TabIndex = 11;
            this.EndPosTag.Text = "0 positions stored";
            // 
            // AnalyzePositions
            // 
            this.AnalyzePositions.Location = new System.Drawing.Point(417, 81);
            this.AnalyzePositions.Name = "AnalyzePositions";
            this.AnalyzePositions.Size = new System.Drawing.Size(103, 23);
            this.AnalyzePositions.TabIndex = 12;
            this.AnalyzePositions.Text = "Analyze";
            this.AnalyzePositions.UseVisualStyleBackColor = true;
            this.AnalyzePositions.Click += new System.EventHandler(this.AnalyzePositions_Click);
            // 
            // analysisTag
            // 
            this.analysisTag.AutoSize = true;
            this.analysisTag.Location = new System.Drawing.Point(526, 86);
            this.analysisTag.Name = "analysisTag";
            this.analysisTag.Size = new System.Drawing.Size(89, 13);
            this.analysisTag.TabIndex = 13;
            this.analysisTag.Text = "0 decisive games";
            // 
            // rawCounts
            // 
            this.rawCounts.Location = new System.Drawing.Point(377, 134);
            this.rawCounts.Multiline = true;
            this.rawCounts.Name = "rawCounts";
            this.rawCounts.ReadOnly = true;
            this.rawCounts.Size = new System.Drawing.Size(370, 130);
            this.rawCounts.TabIndex = 14;
            // 
            // wtdCounts
            // 
            this.wtdCounts.Location = new System.Drawing.Point(377, 270);
            this.wtdCounts.Multiline = true;
            this.wtdCounts.Name = "wtdCounts";
            this.wtdCounts.ReadOnly = true;
            this.wtdCounts.Size = new System.Drawing.Size(370, 130);
            this.wtdCounts.TabIndex = 15;
            // 
            // inclWWin
            // 
            this.inclWWin.AutoSize = true;
            this.inclWWin.Location = new System.Drawing.Point(621, 47);
            this.inclWWin.Name = "inclWWin";
            this.inclWWin.Size = new System.Drawing.Size(61, 17);
            this.inclWWin.TabIndex = 16;
            this.inclWWin.Text = "Incl 1-0";
            this.inclWWin.UseVisualStyleBackColor = true;
            // 
            // inclBWin
            // 
            this.inclBWin.AutoSize = true;
            this.inclBWin.Location = new System.Drawing.Point(621, 62);
            this.inclBWin.Name = "inclBWin";
            this.inclBWin.Size = new System.Drawing.Size(61, 17);
            this.inclBWin.TabIndex = 17;
            this.inclBWin.Text = "Incl 0-1";
            this.inclBWin.UseVisualStyleBackColor = true;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Enabled = false;
            this.checkBox1.Location = new System.Drawing.Point(684, 47);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(63, 17);
            this.checkBox1.TabIndex = 18;
            this.checkBox1.Text = "Incl 1/2";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.inclBWin);
            this.Controls.Add(this.inclWWin);
            this.Controls.Add(this.wtdCounts);
            this.Controls.Add(this.rawCounts);
            this.Controls.Add(this.analysisTag);
            this.Controls.Add(this.AnalyzePositions);
            this.Controls.Add(this.EndPosTag);
            this.Controls.Add(this.PullEndPos);
            this.Controls.Add(this.GameLabel);
            this.Controls.Add(this.nextGame);
            this.Controls.Add(this.prevGame);
            this.Controls.Add(this.pgnTextBox);
            this.Controls.Add(this.nbrGamesLabel);
            this.Controls.Add(this.fileLabel);
            this.Controls.Add(this.positionsToDBButton);
            this.Controls.Add(this.ckMainLinesOnly);
            this.Controls.Add(this.selectFileButton);
            this.Controls.Add(this.textToDBButton);
            this.Name = "Form1";
            this.Text = "Populate Analysis DB";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button textToDBButton;
        private System.Windows.Forms.Button selectFileButton;
        private System.Windows.Forms.CheckBox ckMainLinesOnly;
        private System.Windows.Forms.Button positionsToDBButton;
        private System.Windows.Forms.Label fileLabel;
        private System.Windows.Forms.Label nbrGamesLabel;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.TextBox pgnTextBox;
        private System.Windows.Forms.Button prevGame;
        private System.Windows.Forms.Button nextGame;
        private System.Windows.Forms.Label GameLabel;
        private System.Windows.Forms.Button PullEndPos;
        private System.Windows.Forms.Label EndPosTag;
        private System.Windows.Forms.Button AnalyzePositions;
        private System.Windows.Forms.Label analysisTag;
        private System.Windows.Forms.TextBox rawCounts;
        private System.Windows.Forms.TextBox wtdCounts;
        private System.Windows.Forms.CheckBox inclWWin;
        private System.Windows.Forms.CheckBox inclBWin;
        private System.Windows.Forms.CheckBox checkBox1;
    }
}

