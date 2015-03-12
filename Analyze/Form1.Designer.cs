﻿namespace Analyze
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
            this.PGNLoc = new System.Windows.Forms.TextBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.OpenMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.RunMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.FileStatsButton = new System.Windows.Forms.Button();
            this.AnalysisButton = new System.Windows.Forms.Button();
            this.TotalGamesLabel = new System.Windows.Forms.Label();
            this.TotalPliesLabel = new System.Windows.Forms.Label();
            this.TotalRatedLabel = new System.Windows.Forms.Label();
            this.UniquePosLabel = new System.Windows.Forms.Label();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // PGNLoc
            // 
            this.PGNLoc.Location = new System.Drawing.Point(12, 27);
            this.PGNLoc.Name = "PGNLoc";
            this.PGNLoc.Size = new System.Drawing.Size(259, 20);
            this.PGNLoc.TabIndex = 1;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(284, 24);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.OpenMenuItem,
            this.RunMenuItem});
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
            // RunMenuItem
            // 
            this.RunMenuItem.Name = "RunMenuItem";
            this.RunMenuItem.Size = new System.Drawing.Size(103, 22);
            this.RunMenuItem.Text = "&Run";
            this.RunMenuItem.Click += new System.EventHandler(this.RunMenuItem_Click);
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
            // AnalyzeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
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
        private System.Windows.Forms.ToolStripMenuItem RunMenuItem;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button FileStatsButton;
        private System.Windows.Forms.Button AnalysisButton;
        private System.Windows.Forms.Label TotalGamesLabel;
        private System.Windows.Forms.Label TotalPliesLabel;
        private System.Windows.Forms.Label TotalRatedLabel;
        private System.Windows.Forms.Label UniquePosLabel;
    }
}

