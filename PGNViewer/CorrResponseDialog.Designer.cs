namespace PGNViewer
{
    partial class CorrResponseDialog
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
            this.htmlView = new System.Windows.Forms.WebBrowser();
            this.SendToClipboard = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // htmlView
            // 
            this.htmlView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.htmlView.Location = new System.Drawing.Point(0, 0);
            this.htmlView.MinimumSize = new System.Drawing.Size(20, 20);
            this.htmlView.Name = "htmlView";
            this.htmlView.Size = new System.Drawing.Size(458, 574);
            this.htmlView.TabIndex = 0;
            // 
            // SendToClipboard
            // 
            this.SendToClipboard.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.SendToClipboard.Location = new System.Drawing.Point(371, 551);
            this.SendToClipboard.Name = "SendToClipboard";
            this.SendToClipboard.Size = new System.Drawing.Size(75, 23);
            this.SendToClipboard.TabIndex = 1;
            this.SendToClipboard.Text = "To Clipboard";
            this.SendToClipboard.UseVisualStyleBackColor = true;
            this.SendToClipboard.Click += new System.EventHandler(this.SendToClipboard_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(290, 551);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 2;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // CorrResponseDialog
            // 
            this.AcceptButton = this.SendToClipboard;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(458, 574);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.SendToClipboard);
            this.Controls.Add(this.htmlView);
            this.Name = "CorrResponseDialog";
            this.Text = "CorrResponseDialog";
            this.Shown += new System.EventHandler(this.CorrResponseDialog_Shown);
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.WebBrowser htmlView;
        private System.Windows.Forms.Button SendToClipboard;
        private System.Windows.Forms.Button cancelButton;

    }
}