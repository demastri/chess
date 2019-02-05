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
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
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
    }
}

