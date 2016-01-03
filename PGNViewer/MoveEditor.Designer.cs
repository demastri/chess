namespace PGNViewer
{
    partial class MoveEditor
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
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.CorrMoveTime = new System.Windows.Forms.DateTimePicker();
            this.CorrMoveText = new System.Windows.Forms.TextBox();
            this.MoveNbrLabel = new System.Windows.Forms.Label();
            this.penaltyTime = new System.Windows.Forms.TextBox();
            this.OKButton = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.EditorNowButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(5, 12);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Move Text";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 41);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(60, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Move Time";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(24, 64);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(42, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "Penalty";
            // 
            // CorrMoveTime
            // 
            this.CorrMoveTime.CustomFormat = "MM/dd/yyyy HH:mm";
            this.CorrMoveTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.CorrMoveTime.Location = new System.Drawing.Point(72, 35);
            this.CorrMoveTime.Name = "CorrMoveTime";
            this.CorrMoveTime.Size = new System.Drawing.Size(128, 20);
            this.CorrMoveTime.TabIndex = 15;
            // 
            // CorrMoveText
            // 
            this.CorrMoveText.Location = new System.Drawing.Point(109, 9);
            this.CorrMoveText.Name = "CorrMoveText";
            this.CorrMoveText.ReadOnly = true;
            this.CorrMoveText.Size = new System.Drawing.Size(47, 20);
            this.CorrMoveText.TabIndex = 17;
            // 
            // MoveNbrLabel
            // 
            this.MoveNbrLabel.AutoSize = true;
            this.MoveNbrLabel.Location = new System.Drawing.Point(69, 12);
            this.MoveNbrLabel.Name = "MoveNbrLabel";
            this.MoveNbrLabel.Size = new System.Drawing.Size(34, 13);
            this.MoveNbrLabel.TabIndex = 18;
            this.MoveNbrLabel.Text = "20. ...";
            // 
            // penaltyTime
            // 
            this.penaltyTime.Location = new System.Drawing.Point(72, 61);
            this.penaltyTime.Name = "penaltyTime";
            this.penaltyTime.Size = new System.Drawing.Size(47, 20);
            this.penaltyTime.TabIndex = 19;
            // 
            // OKButton
            // 
            this.OKButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OKButton.Location = new System.Drawing.Point(27, 87);
            this.OKButton.Name = "OKButton";
            this.OKButton.Size = new System.Drawing.Size(57, 23);
            this.OKButton.TabIndex = 20;
            this.OKButton.Text = "OK";
            this.OKButton.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button2.Location = new System.Drawing.Point(109, 87);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(57, 23);
            this.button2.TabIndex = 21;
            this.button2.Text = "Cancel";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // EditorNowButton
            // 
            this.EditorNowButton.Location = new System.Drawing.Point(159, 53);
            this.EditorNowButton.Name = "EditorNowButton";
            this.EditorNowButton.Size = new System.Drawing.Size(41, 23);
            this.EditorNowButton.TabIndex = 22;
            this.EditorNowButton.Text = "Now";
            this.EditorNowButton.UseVisualStyleBackColor = true;
            this.EditorNowButton.Click += new System.EventHandler(this.EditorNowButton_Click);
            // 
            // MoveEditor
            // 
            this.AcceptButton = this.OKButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.button2;
            this.ClientSize = new System.Drawing.Size(210, 120);
            this.Controls.Add(this.EditorNowButton);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.OKButton);
            this.Controls.Add(this.penaltyTime);
            this.Controls.Add(this.MoveNbrLabel);
            this.Controls.Add(this.CorrMoveText);
            this.Controls.Add(this.CorrMoveTime);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Name = "MoveEditor";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "MoveEditor";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.DateTimePicker CorrMoveTime;
        private System.Windows.Forms.TextBox CorrMoveText;
        private System.Windows.Forms.Label MoveNbrLabel;
        private System.Windows.Forms.TextBox penaltyTime;
        private System.Windows.Forms.Button OKButton;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button EditorNowButton;
    }
}