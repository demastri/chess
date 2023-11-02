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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.boardDisplay = new System.Windows.Forms.TextBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.FileMenuStrip = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SaveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.preprocstrip = new System.Windows.Forms.ToolStripMenuItem();
            this.LoadFromDBToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SaveToDBToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.openLastFileMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.mruMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.mruMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.mruMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.mruMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
            this.mruMenuItem5 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.modeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.analysisToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.correspondenceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
            this.corrGridView = new System.Windows.Forms.DataGridView();
            this.MoveNbr = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.White = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.WMoveTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.WMoveReflection = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Black = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BMoveTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BMoveReflection = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CorrMoveTime = new System.Windows.Forms.DateTimePicker();
            this.CorrLabel = new System.Windows.Forms.Label();
            this.CorrMoveText = new System.Windows.Forms.TextBox();
            this.CorrTimeNow = new System.Windows.Forms.Button();
            this.CorrMoveNbr = new System.Windows.Forms.TextBox();
            this.CorrUpdate = new System.Windows.Forms.Button();
            this.CorrPublish = new System.Windows.Forms.Button();
            this.ReflTimeLabel = new System.Windows.Forms.Label();
            this.ckInvertBoard = new System.Windows.Forms.CheckBox();
            this.GameList = new System.Windows.Forms.TreeView();
            this.corrNameLabel = new System.Windows.Forms.Label();
            this.corrName = new System.Windows.Forms.TextBox();
            this.corrTZ = new System.Windows.Forms.TextBox();
            this.corrTZLabel = new System.Windows.Forms.Label();
            this.corrTemplateList = new System.Windows.Forms.ComboBox();
            this.PopMoveButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.resultCombo = new System.Windows.Forms.ComboBox();
            this.tagEditorGrid = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.capturedPieceDisplay = new System.Windows.Forms.TextBox();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.writeAsFENstrip = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.corrGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tagEditorGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // boardDisplay
            // 
            this.boardDisplay.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.boardDisplay.Location = new System.Drawing.Point(352, 101);
            this.boardDisplay.Multiline = true;
            this.boardDisplay.Name = "boardDisplay";
            this.boardDisplay.ReadOnly = true;
            this.boardDisplay.Size = new System.Drawing.Size(235, 342);
            this.boardDisplay.TabIndex = 1;
            this.boardDisplay.MouseDown += new System.Windows.Forms.MouseEventHandler(this.boardDisplay_MouseDown);
            this.boardDisplay.MouseMove += new System.Windows.Forms.MouseEventHandler(this.boardDisplay_MouseMove);
            this.boardDisplay.MouseUp += new System.Windows.Forms.MouseEventHandler(this.boardDisplay_MouseUp);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.FileMenuStrip,
            this.modeToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1045, 24);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // FileMenuStrip
            // 
            this.FileMenuStrip.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.SaveAsToolStripMenuItem,
            this.closeToolStripMenuItem1,
            this.toolStripSeparator4,
            this.preprocstrip,
            this.writeAsFENstrip,
            this.LoadFromDBToolStripMenuItem,
            this.SaveToDBToolStripMenuItem,
            this.toolStripSeparator3,
            this.openLastFileMenu,
            this.toolStripSeparator1,
            this.mruMenuItem1,
            this.mruMenuItem2,
            this.mruMenuItem3,
            this.mruMenuItem4,
            this.mruMenuItem5,
            this.toolStripSeparator2,
            this.exitToolStripMenuItem});
            this.FileMenuStrip.Name = "FileMenuStrip";
            this.FileMenuStrip.Size = new System.Drawing.Size(37, 20);
            this.FileMenuStrip.Text = "&File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            this.openToolStripMenuItem.Text = "&Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Enabled = false;
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            this.saveToolStripMenuItem.Text = "&Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // SaveAsToolStripMenuItem
            // 
            this.SaveAsToolStripMenuItem.Enabled = false;
            this.SaveAsToolStripMenuItem.Name = "SaveAsToolStripMenuItem";
            this.SaveAsToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            this.SaveAsToolStripMenuItem.Text = "Sa&ve As";
            this.SaveAsToolStripMenuItem.Click += new System.EventHandler(this.SaveAsToolStripMenuItem_Click);
            // 
            // closeToolStripMenuItem1
            // 
            this.closeToolStripMenuItem1.Name = "closeToolStripMenuItem1";
            this.closeToolStripMenuItem1.Size = new System.Drawing.Size(182, 22);
            this.closeToolStripMenuItem1.Text = "C&lose";
            this.closeToolStripMenuItem1.Click += new System.EventHandler(this.closeToolStripMenuItem1_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(179, 6);
            // 
            // preprocstrip
            // 
            this.preprocstrip.Name = "preprocstrip";
            this.preprocstrip.Size = new System.Drawing.Size(182, 22);
            this.preprocstrip.Text = "Preprocess File";
            this.preprocstrip.Click += new System.EventHandler(this.preprocstrip_Click);
            // 
            // LoadFromDBToolStripMenuItem
            // 
            this.LoadFromDBToolStripMenuItem.Name = "LoadFromDBToolStripMenuItem";
            this.LoadFromDBToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            this.LoadFromDBToolStripMenuItem.Text = "Load From &Database";
            this.LoadFromDBToolStripMenuItem.Click += new System.EventHandler(this.LoadFromDBToolStripMenuItem_Click);
            // 
            // SaveToDBToolStripMenuItem
            // 
            this.SaveToDBToolStripMenuItem.Enabled = false;
            this.SaveToDBToolStripMenuItem.Name = "SaveToDBToolStripMenuItem";
            this.SaveToDBToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            this.SaveToDBToolStripMenuItem.Text = "Save  To Data&base";
            this.SaveToDBToolStripMenuItem.Click += new System.EventHandler(this.SaveToDBToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(179, 6);
            // 
            // openLastFileMenu
            // 
            this.openLastFileMenu.Name = "openLastFileMenu";
            this.openLastFileMenu.Size = new System.Drawing.Size(182, 22);
            this.openLastFileMenu.Text = "Open L&ast File";
            this.openLastFileMenu.Click += new System.EventHandler(this.toolStripMenuItem1_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(179, 6);
            // 
            // mruMenuItem1
            // 
            this.mruMenuItem1.Name = "mruMenuItem1";
            this.mruMenuItem1.Size = new System.Drawing.Size(182, 22);
            this.mruMenuItem1.Text = "&1 - MRU1";
            this.mruMenuItem1.Click += new System.EventHandler(this.mruMenuItem_Click);
            // 
            // mruMenuItem2
            // 
            this.mruMenuItem2.Name = "mruMenuItem2";
            this.mruMenuItem2.Size = new System.Drawing.Size(182, 22);
            this.mruMenuItem2.Text = "&2 - MRU2";
            this.mruMenuItem2.Click += new System.EventHandler(this.mruMenuItem_Click);
            // 
            // mruMenuItem3
            // 
            this.mruMenuItem3.Name = "mruMenuItem3";
            this.mruMenuItem3.Size = new System.Drawing.Size(182, 22);
            this.mruMenuItem3.Text = "&3 - MRU3";
            this.mruMenuItem3.Click += new System.EventHandler(this.mruMenuItem_Click);
            // 
            // mruMenuItem4
            // 
            this.mruMenuItem4.Name = "mruMenuItem4";
            this.mruMenuItem4.Size = new System.Drawing.Size(182, 22);
            this.mruMenuItem4.Text = "&4 - MRU4";
            this.mruMenuItem4.Click += new System.EventHandler(this.mruMenuItem_Click);
            // 
            // mruMenuItem5
            // 
            this.mruMenuItem5.Name = "mruMenuItem5";
            this.mruMenuItem5.Size = new System.Drawing.Size(182, 22);
            this.mruMenuItem5.Text = "&5 - MRU5";
            this.mruMenuItem5.Click += new System.EventHandler(this.mruMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(179, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // modeToolStripMenuItem
            // 
            this.modeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.viewerToolStripMenuItem,
            this.analysisToolStripMenuItem,
            this.correspondenceToolStripMenuItem});
            this.modeToolStripMenuItem.Name = "modeToolStripMenuItem";
            this.modeToolStripMenuItem.Size = new System.Drawing.Size(50, 20);
            this.modeToolStripMenuItem.Text = "&Mode";
            // 
            // viewerToolStripMenuItem
            // 
            this.viewerToolStripMenuItem.Checked = true;
            this.viewerToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.viewerToolStripMenuItem.Name = "viewerToolStripMenuItem";
            this.viewerToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.viewerToolStripMenuItem.Text = "&Viewer";
            this.viewerToolStripMenuItem.Click += new System.EventHandler(this.viewerToolStripMenuItem_Click);
            // 
            // analysisToolStripMenuItem
            // 
            this.analysisToolStripMenuItem.Name = "analysisToolStripMenuItem";
            this.analysisToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.analysisToolStripMenuItem.Text = "&Analysis";
            this.analysisToolStripMenuItem.Click += new System.EventHandler(this.analysisToolStripMenuItem_Click);
            // 
            // correspondenceToolStripMenuItem
            // 
            this.correspondenceToolStripMenuItem.Name = "correspondenceToolStripMenuItem";
            this.correspondenceToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.correspondenceToolStripMenuItem.Text = "&Correspondence";
            this.correspondenceToolStripMenuItem.Click += new System.EventHandler(this.correspondenceToolStripMenuItem_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.DefaultExt = "pgn";
            this.openFileDialog1.Filter = "PGN files (*.pgn)|*.pgn|All files (*.*)|*.*";
            // 
            // PGNText
            // 
            this.PGNText.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.PGNText.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PGNText.HideSelection = false;
            this.PGNText.Location = new System.Drawing.Point(593, 311);
            this.PGNText.Multiline = true;
            this.PGNText.Name = "PGNText";
            this.PGNText.ReadOnly = true;
            this.PGNText.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.PGNText.Size = new System.Drawing.Size(439, 161);
            this.PGNText.TabIndex = 3;
            this.PGNText.MouseClick += new System.Windows.Forms.MouseEventHandler(this.PGNText_Click);
            // 
            // ResetGameButton
            // 
            this.ResetGameButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ResetGameButton.Location = new System.Drawing.Point(365, 449);
            this.ResetGameButton.Name = "ResetGameButton";
            this.ResetGameButton.Size = new System.Drawing.Size(30, 23);
            this.ResetGameButton.TabIndex = 4;
            this.ResetGameButton.Text = "|<";
            this.ResetGameButton.UseVisualStyleBackColor = true;
            this.ResetGameButton.Click += new System.EventHandler(this.ResetGameButton_Click);
            // 
            // BackButton
            // 
            this.BackButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.BackButton.Location = new System.Drawing.Point(437, 449);
            this.BackButton.Name = "BackButton";
            this.BackButton.Size = new System.Drawing.Size(30, 23);
            this.BackButton.TabIndex = 5;
            this.BackButton.Text = "<";
            this.BackButton.UseVisualStyleBackColor = true;
            this.BackButton.Click += new System.EventHandler(this.BackButton_Click);
            // 
            // FwdButton
            // 
            this.FwdButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.FwdButton.Location = new System.Drawing.Point(473, 449);
            this.FwdButton.Name = "FwdButton";
            this.FwdButton.Size = new System.Drawing.Size(30, 23);
            this.FwdButton.TabIndex = 6;
            this.FwdButton.Text = ">";
            this.FwdButton.UseVisualStyleBackColor = true;
            this.FwdButton.Click += new System.EventHandler(this.FwdButton_Click);
            // 
            // FENText
            // 
            this.FENText.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.FENText.Location = new System.Drawing.Point(593, 285);
            this.FENText.Name = "FENText";
            this.FENText.ReadOnly = true;
            this.FENText.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.FENText.Size = new System.Drawing.Size(439, 20);
            this.FENText.TabIndex = 7;
            // 
            // EngineList
            // 
            this.EngineList.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.EngineList.FormattingEnabled = true;
            this.EngineList.Items.AddRange(new object[] {
            "None",
            "Stockfish",
            "Crafty"});
            this.EngineList.Location = new System.Drawing.Point(873, 27);
            this.EngineList.Name = "EngineList";
            this.EngineList.Size = new System.Drawing.Size(160, 21);
            this.EngineList.TabIndex = 8;
            this.EngineList.Text = "None";
            this.EngineList.SelectedIndexChanged += new System.EventHandler(this.EngineList_SelectedIndexChanged);
            // 
            // AnalysisText
            // 
            this.AnalysisText.Location = new System.Drawing.Point(593, 54);
            this.AnalysisText.Multiline = true;
            this.AnalysisText.Name = "AnalysisText";
            this.AnalysisText.ReadOnly = true;
            this.AnalysisText.Size = new System.Drawing.Size(406, 225);
            this.AnalysisText.TabIndex = 9;
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // JumpBackButton
            // 
            this.JumpBackButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.JumpBackButton.Location = new System.Drawing.Point(401, 449);
            this.JumpBackButton.Name = "JumpBackButton";
            this.JumpBackButton.Size = new System.Drawing.Size(30, 23);
            this.JumpBackButton.TabIndex = 10;
            this.JumpBackButton.Text = "<<";
            this.JumpBackButton.UseVisualStyleBackColor = true;
            this.JumpBackButton.Click += new System.EventHandler(this.JumpBackButton_Click);
            // 
            // JumpFwdButton
            // 
            this.JumpFwdButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.JumpFwdButton.Location = new System.Drawing.Point(509, 449);
            this.JumpFwdButton.Name = "JumpFwdButton";
            this.JumpFwdButton.Size = new System.Drawing.Size(30, 23);
            this.JumpFwdButton.TabIndex = 11;
            this.JumpFwdButton.Text = ">>";
            this.JumpFwdButton.UseVisualStyleBackColor = true;
            this.JumpFwdButton.Click += new System.EventHandler(this.JumpFwdButton_Click);
            // 
            // JumpToEndButton
            // 
            this.JumpToEndButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.JumpToEndButton.Location = new System.Drawing.Point(545, 449);
            this.JumpToEndButton.Name = "JumpToEndButton";
            this.JumpToEndButton.Size = new System.Drawing.Size(30, 23);
            this.JumpToEndButton.TabIndex = 12;
            this.JumpToEndButton.Text = ">|";
            this.JumpToEndButton.UseVisualStyleBackColor = true;
            this.JumpToEndButton.Click += new System.EventHandler(this.JumpToEndButton_Click);
            // 
            // corrGridView
            // 
            this.corrGridView.AllowUserToAddRows = false;
            this.corrGridView.AllowUserToDeleteRows = false;
            this.corrGridView.AllowUserToResizeColumns = false;
            this.corrGridView.AllowUserToResizeRows = false;
            this.corrGridView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.corrGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.corrGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.MoveNbr,
            this.White,
            this.WMoveTime,
            this.WMoveReflection,
            this.Black,
            this.BMoveTime,
            this.BMoveReflection});
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.corrGridView.DefaultCellStyle = dataGridViewCellStyle1;
            this.corrGridView.Location = new System.Drawing.Point(593, 54);
            this.corrGridView.Name = "corrGridView";
            this.corrGridView.ReadOnly = true;
            this.corrGridView.RowHeadersVisible = false;
            this.corrGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.corrGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.corrGridView.ShowEditingIcon = false;
            this.corrGridView.Size = new System.Drawing.Size(439, 147);
            this.corrGridView.TabIndex = 13;
            this.corrGridView.Click += new System.EventHandler(this.corrGridView_Click);
            this.corrGridView.DoubleClick += new System.EventHandler(this.corrGridView_DoubleClick);
            // 
            // MoveNbr
            // 
            this.MoveNbr.HeaderText = "Move #";
            this.MoveNbr.Name = "MoveNbr";
            this.MoveNbr.ReadOnly = true;
            this.MoveNbr.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.MoveNbr.Width = 40;
            // 
            // White
            // 
            this.White.HeaderText = "White";
            this.White.Name = "White";
            this.White.ReadOnly = true;
            this.White.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.White.Width = 50;
            // 
            // WMoveTime
            // 
            this.WMoveTime.HeaderText = "Time";
            this.WMoveTime.Name = "WMoveTime";
            this.WMoveTime.ReadOnly = true;
            this.WMoveTime.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.WMoveTime.Width = 110;
            // 
            // WMoveReflection
            // 
            this.WMoveReflection.HeaderText = "Refl Tm";
            this.WMoveReflection.Name = "WMoveReflection";
            this.WMoveReflection.ReadOnly = true;
            this.WMoveReflection.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.WMoveReflection.Width = 25;
            // 
            // Black
            // 
            this.Black.HeaderText = "Black";
            this.Black.Name = "Black";
            this.Black.ReadOnly = true;
            this.Black.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Black.Width = 50;
            // 
            // BMoveTime
            // 
            this.BMoveTime.HeaderText = "Time";
            this.BMoveTime.Name = "BMoveTime";
            this.BMoveTime.ReadOnly = true;
            this.BMoveTime.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.BMoveTime.Width = 110;
            // 
            // BMoveReflection
            // 
            this.BMoveReflection.HeaderText = "Refl Tm";
            this.BMoveReflection.Name = "BMoveReflection";
            this.BMoveReflection.ReadOnly = true;
            this.BMoveReflection.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.BMoveReflection.Width = 30;
            // 
            // CorrMoveTime
            // 
            this.CorrMoveTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.CorrMoveTime.CustomFormat = "MM/dd/yyyy HH:mm";
            this.CorrMoveTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.CorrMoveTime.Location = new System.Drawing.Point(856, 208);
            this.CorrMoveTime.Name = "CorrMoveTime";
            this.CorrMoveTime.Size = new System.Drawing.Size(128, 20);
            this.CorrMoveTime.TabIndex = 14;
            // 
            // CorrLabel
            // 
            this.CorrLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.CorrLabel.AutoSize = true;
            this.CorrLabel.Location = new System.Drawing.Point(612, 211);
            this.CorrLabel.Name = "CorrLabel";
            this.CorrLabel.Size = new System.Drawing.Size(120, 13);
            this.CorrLabel.TabIndex = 15;
            this.CorrLabel.Text = "Next Move Text / Time:";
            // 
            // CorrMoveText
            // 
            this.CorrMoveText.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.CorrMoveText.Location = new System.Drawing.Point(803, 208);
            this.CorrMoveText.Name = "CorrMoveText";
            this.CorrMoveText.Size = new System.Drawing.Size(47, 20);
            this.CorrMoveText.TabIndex = 16;
            // 
            // CorrTimeNow
            // 
            this.CorrTimeNow.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.CorrTimeNow.Location = new System.Drawing.Point(990, 206);
            this.CorrTimeNow.Name = "CorrTimeNow";
            this.CorrTimeNow.Size = new System.Drawing.Size(42, 23);
            this.CorrTimeNow.TabIndex = 17;
            this.CorrTimeNow.Text = "Now";
            this.CorrTimeNow.UseVisualStyleBackColor = true;
            this.CorrTimeNow.Click += new System.EventHandler(this.CorrTimeNow_Click);
            // 
            // CorrMoveNbr
            // 
            this.CorrMoveNbr.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.CorrMoveNbr.Location = new System.Drawing.Point(750, 208);
            this.CorrMoveNbr.Name = "CorrMoveNbr";
            this.CorrMoveNbr.ReadOnly = true;
            this.CorrMoveNbr.Size = new System.Drawing.Size(47, 20);
            this.CorrMoveNbr.TabIndex = 18;
            // 
            // CorrUpdate
            // 
            this.CorrUpdate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.CorrUpdate.Location = new System.Drawing.Point(924, 234);
            this.CorrUpdate.Name = "CorrUpdate";
            this.CorrUpdate.Size = new System.Drawing.Size(51, 23);
            this.CorrUpdate.TabIndex = 19;
            this.CorrUpdate.Text = "Update";
            this.CorrUpdate.UseVisualStyleBackColor = true;
            this.CorrUpdate.Click += new System.EventHandler(this.CorrUpdate_Click);
            // 
            // CorrPublish
            // 
            this.CorrPublish.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.CorrPublish.Location = new System.Drawing.Point(981, 234);
            this.CorrPublish.Name = "CorrPublish";
            this.CorrPublish.Size = new System.Drawing.Size(51, 23);
            this.CorrPublish.TabIndex = 20;
            this.CorrPublish.Text = "Publish";
            this.CorrPublish.UseVisualStyleBackColor = true;
            this.CorrPublish.Click += new System.EventHandler(this.CorrPublish_Click);
            // 
            // ReflTimeLabel
            // 
            this.ReflTimeLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ReflTimeLabel.AutoSize = true;
            this.ReflTimeLabel.Location = new System.Drawing.Point(611, 230);
            this.ReflTimeLabel.Name = "ReflTimeLabel";
            this.ReflTimeLabel.Size = new System.Drawing.Size(220, 13);
            this.ReflTimeLabel.TabIndex = 21;
            this.ReflTimeLabel.Text = "Reflection Time:  Total/Used/Remain (W/B):";
            // 
            // ckInvertBoard
            // 
            this.ckInvertBoard.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ckInvertBoard.AutoSize = true;
            this.ckInvertBoard.Location = new System.Drawing.Point(615, 31);
            this.ckInvertBoard.Name = "ckInvertBoard";
            this.ckInvertBoard.Size = new System.Drawing.Size(84, 17);
            this.ckInvertBoard.TabIndex = 22;
            this.ckInvertBoard.Text = "Invert Board";
            this.ckInvertBoard.UseVisualStyleBackColor = true;
            this.ckInvertBoard.Click += new System.EventHandler(this.ckInvertBoard_Click);
            // 
            // GameList
            // 
            this.GameList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.GameList.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.GameList.HideSelection = false;
            this.GameList.Location = new System.Drawing.Point(12, 27);
            this.GameList.Name = "GameList";
            this.GameList.Size = new System.Drawing.Size(334, 174);
            this.GameList.TabIndex = 23;
            this.GameList.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.GameList_SelectedIndexChanged);
            // 
            // corrNameLabel
            // 
            this.corrNameLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.corrNameLabel.AutoSize = true;
            this.corrNameLabel.Location = new System.Drawing.Point(612, 262);
            this.corrNameLabel.Name = "corrNameLabel";
            this.corrNameLabel.Size = new System.Drawing.Size(55, 13);
            this.corrNameLabel.TabIndex = 24;
            this.corrNameLabel.Text = "My Name:";
            // 
            // corrName
            // 
            this.corrName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.corrName.Location = new System.Drawing.Point(672, 259);
            this.corrName.Name = "corrName";
            this.corrName.Size = new System.Drawing.Size(78, 20);
            this.corrName.TabIndex = 25;
            this.corrName.Text = "SomeName";
            this.corrName.TextChanged += new System.EventHandler(this.corrName_TextChanged);
            // 
            // corrTZ
            // 
            this.corrTZ.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.corrTZ.Location = new System.Drawing.Point(873, 236);
            this.corrTZ.Name = "corrTZ";
            this.corrTZ.Size = new System.Drawing.Size(47, 20);
            this.corrTZ.TabIndex = 27;
            this.corrTZ.Text = "CT";
            // 
            // corrTZLabel
            // 
            this.corrTZLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.corrTZLabel.AutoSize = true;
            this.corrTZLabel.Location = new System.Drawing.Point(824, 239);
            this.corrTZLabel.Name = "corrTZLabel";
            this.corrTZLabel.Size = new System.Drawing.Size(43, 13);
            this.corrTZLabel.TabIndex = 26;
            this.corrTZLabel.Text = "Corr TZ";
            // 
            // corrTemplateList
            // 
            this.corrTemplateList.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.corrTemplateList.FormattingEnabled = true;
            this.corrTemplateList.Items.AddRange(new object[] {
            "None",
            "Stockfish",
            "Crafty"});
            this.corrTemplateList.Location = new System.Drawing.Point(707, 27);
            this.corrTemplateList.Name = "corrTemplateList";
            this.corrTemplateList.Size = new System.Drawing.Size(160, 21);
            this.corrTemplateList.TabIndex = 28;
            this.corrTemplateList.Text = "None";
            this.corrTemplateList.SelectedIndexChanged += new System.EventHandler(this.corrTemplateList_SelectedIndexChanged);
            // 
            // PopMoveButton
            // 
            this.PopMoveButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.PopMoveButton.Location = new System.Drawing.Point(964, 263);
            this.PopMoveButton.Name = "PopMoveButton";
            this.PopMoveButton.Size = new System.Drawing.Size(68, 23);
            this.PopMoveButton.TabIndex = 29;
            this.PopMoveButton.Text = "Pop Move";
            this.PopMoveButton.UseVisualStyleBackColor = true;
            this.PopMoveButton.Click += new System.EventHandler(this.PopMoveButton_Click);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(824, 266);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(75, 13);
            this.label1.TabIndex = 30;
            this.label1.Text = "Update Result";
            // 
            // resultCombo
            // 
            this.resultCombo.AllowDrop = true;
            this.resultCombo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.resultCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.resultCombo.FormattingEnabled = true;
            this.resultCombo.Items.AddRange(new object[] {
            "*",
            "1-0",
            "0-1",
            "1/2-1/2"});
            this.resultCombo.Location = new System.Drawing.Point(905, 262);
            this.resultCombo.Name = "resultCombo";
            this.resultCombo.Size = new System.Drawing.Size(46, 21);
            this.resultCombo.TabIndex = 31;
            this.resultCombo.SelectedIndexChanged += new System.EventHandler(this.resultCombo_SelectedIndexChanged);
            // 
            // tagEditorGrid
            // 
            this.tagEditorGrid.AllowUserToResizeColumns = false;
            this.tagEditorGrid.AllowUserToResizeRows = false;
            this.tagEditorGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.tagEditorGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.tagEditorGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2});
            this.tagEditorGrid.Location = new System.Drawing.Point(12, 206);
            this.tagEditorGrid.Name = "tagEditorGrid";
            this.tagEditorGrid.RowHeadersVisible = false;
            this.tagEditorGrid.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.tagEditorGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.tagEditorGrid.ShowEditingIcon = false;
            this.tagEditorGrid.Size = new System.Drawing.Size(334, 266);
            this.tagEditorGrid.TabIndex = 32;
            this.tagEditorGrid.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.tagEditorGrid_CellValueChanged);
            this.tagEditorGrid.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.tagEditorGrid_RowsAdded);
            this.tagEditorGrid.RowsRemoved += new System.Windows.Forms.DataGridViewRowsRemovedEventHandler(this.tagEditorGrid_RowsRemoved);
            // 
            // Column1
            // 
            this.Column1.HeaderText = "Tag";
            this.Column1.MaxInputLength = 20;
            this.Column1.Name = "Column1";
            this.Column1.Width = 70;
            // 
            // Column2
            // 
            this.Column2.HeaderText = "Value";
            this.Column2.MaxInputLength = 100;
            this.Column2.Name = "Column2";
            this.Column2.Width = 225;
            // 
            // capturedPieceDisplay
            // 
            this.capturedPieceDisplay.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.capturedPieceDisplay.Location = new System.Drawing.Point(352, 33);
            this.capturedPieceDisplay.Multiline = true;
            this.capturedPieceDisplay.Name = "capturedPieceDisplay";
            this.capturedPieceDisplay.ReadOnly = true;
            this.capturedPieceDisplay.Size = new System.Drawing.Size(235, 62);
            this.capturedPieceDisplay.TabIndex = 33;
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.DefaultExt = "pgn";
            this.saveFileDialog1.Filter = "PGN files (*.pgn)|*.pgn|All files (*.*)|*.*";
            // 
            // writeAsFENstrip
            // 
            this.writeAsFENstrip.Name = "writeAsFENstrip";
            this.writeAsFENstrip.Size = new System.Drawing.Size(182, 22);
            this.writeAsFENstrip.Text = "Write As FEN";
            this.writeAsFENstrip.Click += new System.EventHandler(this.writeAsFENstrip_Click);
            // 
            // PGNViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1045, 484);
            this.Controls.Add(this.capturedPieceDisplay);
            this.Controls.Add(this.tagEditorGrid);
            this.Controls.Add(this.resultCombo);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.PopMoveButton);
            this.Controls.Add(this.corrTemplateList);
            this.Controls.Add(this.corrTZ);
            this.Controls.Add(this.corrTZLabel);
            this.Controls.Add(this.corrName);
            this.Controls.Add(this.corrNameLabel);
            this.Controls.Add(this.GameList);
            this.Controls.Add(this.ckInvertBoard);
            this.Controls.Add(this.ReflTimeLabel);
            this.Controls.Add(this.CorrPublish);
            this.Controls.Add(this.CorrUpdate);
            this.Controls.Add(this.CorrMoveNbr);
            this.Controls.Add(this.CorrTimeNow);
            this.Controls.Add(this.CorrMoveText);
            this.Controls.Add(this.CorrLabel);
            this.Controls.Add(this.CorrMoveTime);
            this.Controls.Add(this.corrGridView);
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
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "PGNViewer";
            this.Text = "PGN Viewer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PGNViewer_FormClosing);
            this.Load += new System.EventHandler(this.PGNViewer_Load);
            this.Resize += new System.EventHandler(this.PGNViewer_Resize);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.corrGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tagEditorGrid)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox boardDisplay;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem FileMenuStrip;
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
        private System.Windows.Forms.ToolStripMenuItem modeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem analysisToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem correspondenceToolStripMenuItem;
        private System.Windows.Forms.DataGridView corrGridView;
        private System.Windows.Forms.DateTimePicker CorrMoveTime;
        private System.Windows.Forms.Label CorrLabel;
        private System.Windows.Forms.TextBox CorrMoveText;
        private System.Windows.Forms.Button CorrTimeNow;
        private System.Windows.Forms.TextBox CorrMoveNbr;
        private System.Windows.Forms.Button CorrUpdate;
        private System.Windows.Forms.Button CorrPublish;
        private System.Windows.Forms.Label ReflTimeLabel;
        private System.Windows.Forms.CheckBox ckInvertBoard;
        private System.Windows.Forms.TreeView GameList;
        private System.Windows.Forms.Label corrNameLabel;
        private System.Windows.Forms.TextBox corrName;
        private System.Windows.Forms.TextBox corrTZ;
        private System.Windows.Forms.Label corrTZLabel;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem mruMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem mruMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem mruMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem mruMenuItem4;
        private System.Windows.Forms.ToolStripMenuItem mruMenuItem5;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ComboBox corrTemplateList;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem openLastFileMenu;
        private System.Windows.Forms.Button PopMoveButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox resultCombo;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.DataGridView tagEditorGrid;
        private System.Windows.Forms.DataGridViewTextBoxColumn MoveNbr;
        private System.Windows.Forms.DataGridViewTextBoxColumn White;
        private System.Windows.Forms.DataGridViewTextBoxColumn WMoveTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn WMoveReflection;
        private System.Windows.Forms.DataGridViewTextBoxColumn Black;
        private System.Windows.Forms.DataGridViewTextBoxColumn BMoveTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn BMoveReflection;
        private System.Windows.Forms.TextBox capturedPieceDisplay;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.ToolStripMenuItem SaveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem LoadFromDBToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem SaveToDBToolStripMenuItem;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.ToolStripMenuItem preprocstrip;
        private System.Windows.Forms.ToolStripMenuItem writeAsFENstrip;
    }
}

