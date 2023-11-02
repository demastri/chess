namespace ChessableToPGN
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
            this.inputChessableHTML = new System.Windows.Forms.TextBox();
            this.outputPGN = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.GenPGN = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.VariationHeader = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.ChapterHeader = new System.Windows.Forms.TextBox();
            this.GameTag = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.variationURL = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.getHTML = new System.Windows.Forms.Button();
            this.localWV = new Microsoft.Web.WebView2.WinForms.WebView2();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.localWV)).BeginInit();
            this.SuspendLayout();
            // 
            // inputChessableHTML
            // 
            this.inputChessableHTML.Location = new System.Drawing.Point(12, 110);
            this.inputChessableHTML.MaxLength = 999999;
            this.inputChessableHTML.Multiline = true;
            this.inputChessableHTML.Name = "inputChessableHTML";
            this.inputChessableHTML.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.inputChessableHTML.Size = new System.Drawing.Size(333, 182);
            this.inputChessableHTML.TabIndex = 0;
            this.inputChessableHTML.TextChanged += new System.EventHandler(this.inputChessableHTML_TextChanged);
            // 
            // outputPGN
            // 
            this.outputPGN.Location = new System.Drawing.Point(397, 51);
            this.outputPGN.Multiline = true;
            this.outputPGN.Name = "outputPGN";
            this.outputPGN.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.outputPGN.Size = new System.Drawing.Size(379, 451);
            this.outputPGN.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 94);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(112, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Input From Chessable:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(397, 35);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(68, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Output PGN:";
            // 
            // GenPGN
            // 
            this.GenPGN.Location = new System.Drawing.Point(351, 269);
            this.GenPGN.Name = "GenPGN";
            this.GenPGN.Size = new System.Drawing.Size(40, 23);
            this.GenPGN.TabIndex = 4;
            this.GenPGN.Text = "Go";
            this.GenPGN.UseVisualStyleBackColor = true;
            this.GenPGN.Click += new System.EventHandler(this.GenPGN_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 74);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(69, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Line Caption:";
            // 
            // VariationHeader
            // 
            this.VariationHeader.Location = new System.Drawing.Point(88, 71);
            this.VariationHeader.Name = "VariationHeader";
            this.VariationHeader.Size = new System.Drawing.Size(303, 20);
            this.VariationHeader.TabIndex = 6;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 45);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(70, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Chapter Title:";
            // 
            // ChapterHeader
            // 
            this.ChapterHeader.Location = new System.Drawing.Point(88, 45);
            this.ChapterHeader.Name = "ChapterHeader";
            this.ChapterHeader.Size = new System.Drawing.Size(303, 20);
            this.ChapterHeader.TabIndex = 8;
            // 
            // GameTag
            // 
            this.GameTag.Location = new System.Drawing.Point(473, 18);
            this.GameTag.Name = "GameTag";
            this.GameTag.Size = new System.Drawing.Size(193, 20);
            this.GameTag.TabIndex = 10;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(397, 18);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(60, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "Game Tag:";
            // 
            // variationURL
            // 
            this.variationURL.Location = new System.Drawing.Point(88, 19);
            this.variationURL.Name = "variationURL";
            this.variationURL.Size = new System.Drawing.Size(257, 20);
            this.variationURL.TabIndex = 12;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 19);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(76, 13);
            this.label6.TabIndex = 11;
            this.label6.Text = "Variation URL:";
            // 
            // getHTML
            // 
            this.getHTML.Location = new System.Drawing.Point(351, 17);
            this.getHTML.Name = "getHTML";
            this.getHTML.Size = new System.Drawing.Size(40, 23);
            this.getHTML.TabIndex = 13;
            this.getHTML.Text = "Get";
            this.getHTML.UseVisualStyleBackColor = true;
            this.getHTML.Click += new System.EventHandler(this.getHTML_Click);
            // 
            // localWV
            // 
            this.localWV.AllowExternalDrop = true;
            this.localWV.CreationProperties = null;
            this.localWV.DefaultBackgroundColor = System.Drawing.Color.White;
            this.localWV.Location = new System.Drawing.Point(15, 298);
            this.localWV.Name = "localWV";
            this.localWV.Size = new System.Drawing.Size(321, 227);
            this.localWV.TabIndex = 14;
            this.localWV.ZoomFactor = 1D;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(726, 14);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(48, 23);
            this.button1.TabIndex = 15;
            this.button1.Text = "Clip V";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(672, 13);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(48, 23);
            this.button2.TabIndex = 16;
            this.button2.Text = "< Clip";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(343, 110);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(48, 23);
            this.button3.TabIndex = 17;
            this.button3.Text = "<Paste";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(786, 537);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.localWV);
            this.Controls.Add(this.getHTML);
            this.Controls.Add(this.variationURL);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.GameTag);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.ChapterHeader);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.VariationHeader);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.GenPGN);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.outputPGN);
            this.Controls.Add(this.inputChessableHTML);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.localWV)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox inputChessableHTML;
        private System.Windows.Forms.TextBox outputPGN;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button GenPGN;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox VariationHeader;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox ChapterHeader;
        private System.Windows.Forms.TextBox GameTag;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox variationURL;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button getHTML;
        private Microsoft.Web.WebView2.WinForms.WebView2 localWV;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
    }
}

