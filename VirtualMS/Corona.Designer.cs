namespace VirtualMS
{
  partial class Corona
  {
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
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
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Corona));
      mainToolStrip = new ToolStrip();
      tsbLoad = new ToolStripButton();
      tsbDelete = new ToolStripButton();
      toolStripSeparator1 = new ToolStripSeparator();
      toolStripSeparator3 = new ToolStripSeparator();
      tsbAdd = new ToolStripButton();
      tsbRemove = new ToolStripButton();
      toolStripSeparator4 = new ToolStripSeparator();
      toolStripSeparator2 = new ToolStripSeparator();
      tsbRun = new ToolStripButton();
      tsbPause = new ToolStripButton();
      tsbStop = new ToolStripButton();
      toolStripSeparator5 = new ToolStripSeparator();
      toolStripSeparator6 = new ToolStripSeparator();
      tsbResetStats = new ToolStripButton();
      splitContainer1 = new SplitContainer();
      splitContainer3 = new SplitContainer();
      fileListBox = new ListBox();
      panel3 = new Panel();
      lblFileInfo = new Label();
      panelMeter = new Panel();
      pan10 = new Panel();
      pan9 = new Panel();
      pan8 = new Panel();
      pan7 = new Panel();
      pan6 = new Panel();
      pan5 = new Panel();
      pan4 = new Panel();
      pan3 = new Panel();
      pan2 = new Panel();
      pan1 = new Panel();
      panel1 = new Panel();
      lblSpeed = new Label();
      tbSpeed = new TrackBar();
      gbStats = new GroupBox();
      lblRunTime = new Label();
      lblScanStats = new Label();
      label3 = new Label();
      label2 = new Label();
      label1 = new Label();
      tabControlMain = new TabControl();
      tabPage4 = new TabPage();
      splitContainer2 = new SplitContainer();
      dgvFiles = new DataGridView();
      rpFileName = new DataGridViewTextBoxColumn();
      rpFilePath = new DataGridViewTextBoxColumn();
      rpRunSpan = new DataGridViewComboBoxColumn();
      rpFirst = new DataGridViewTextBoxColumn();
      rpEnd = new DataGridViewTextBoxColumn();
      tabPage5 = new TabPage();
      plotContainer = new SplitContainer();
      plotTIC = new ScottPlot.WinForms.FormsPlot();
      tabControl1 = new TabControl();
      tabPage1 = new TabPage();
      rtbLog = new RichTextBox();
      tabPage2 = new TabPage();
      plotSpectrum = new ScottPlot.WinForms.FormsPlot();
      labelSpectrum = new Label();
      labelScanNumber = new Label();
      tabPage3 = new TabPage();
      dgvCustomScan = new DataGridView();
      panel2 = new Panel();
      button1 = new Button();
      cbClearCS = new CheckBox();
      nudCustomScans = new NumericUpDown();
      lblCustomScansRT = new Label();
      lblCustomScans = new Label();
      label4 = new Label();
      ofdAdd = new OpenFileDialog();
      statusStrip1 = new StatusStrip();
      mainToolStrip.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
      splitContainer1.Panel1.SuspendLayout();
      splitContainer1.Panel2.SuspendLayout();
      splitContainer1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)splitContainer3).BeginInit();
      splitContainer3.Panel1.SuspendLayout();
      splitContainer3.Panel2.SuspendLayout();
      splitContainer3.SuspendLayout();
      panel3.SuspendLayout();
      panelMeter.SuspendLayout();
      panel1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)tbSpeed).BeginInit();
      gbStats.SuspendLayout();
      tabControlMain.SuspendLayout();
      tabPage4.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)splitContainer2).BeginInit();
      splitContainer2.Panel1.SuspendLayout();
      splitContainer2.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)dgvFiles).BeginInit();
      tabPage5.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)plotContainer).BeginInit();
      plotContainer.Panel1.SuspendLayout();
      plotContainer.Panel2.SuspendLayout();
      plotContainer.SuspendLayout();
      tabControl1.SuspendLayout();
      tabPage1.SuspendLayout();
      tabPage2.SuspendLayout();
      tabPage3.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)dgvCustomScan).BeginInit();
      panel2.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)nudCustomScans).BeginInit();
      SuspendLayout();
      // 
      // mainToolStrip
      // 
      mainToolStrip.AutoSize = false;
      mainToolStrip.ImageScalingSize = new Size(32, 32);
      mainToolStrip.Items.AddRange(new ToolStripItem[] { tsbLoad, tsbDelete, toolStripSeparator1, toolStripSeparator3, tsbAdd, tsbRemove, toolStripSeparator4, toolStripSeparator2, tsbRun, tsbPause, tsbStop, toolStripSeparator5, toolStripSeparator6, tsbResetStats });
      mainToolStrip.Location = new Point(0, 0);
      mainToolStrip.Name = "mainToolStrip";
      mainToolStrip.Size = new Size(1140, 48);
      mainToolStrip.TabIndex = 0;
      mainToolStrip.Text = "toolStrip1";
      // 
      // tsbLoad
      // 
      tsbLoad.AutoSize = false;
      tsbLoad.DisplayStyle = ToolStripItemDisplayStyle.Image;
      tsbLoad.Image = (Image)resources.GetObject("tsbLoad.Image");
      tsbLoad.ImageTransparentColor = Color.Magenta;
      tsbLoad.Name = "tsbLoad";
      tsbLoad.Size = new Size(36, 36);
      tsbLoad.Text = "Open MS Files";
      tsbLoad.Click += toolStripButton1_Click;
      // 
      // tsbDelete
      // 
      tsbDelete.AutoSize = false;
      tsbDelete.DisplayStyle = ToolStripItemDisplayStyle.Image;
      tsbDelete.Enabled = false;
      tsbDelete.Image = (Image)resources.GetObject("tsbDelete.Image");
      tsbDelete.ImageTransparentColor = Color.Magenta;
      tsbDelete.Name = "tsbDelete";
      tsbDelete.Size = new Size(36, 36);
      tsbDelete.Text = "Remove MS Files";
      tsbDelete.Click += tsbDelete_Click;
      // 
      // toolStripSeparator1
      // 
      toolStripSeparator1.Name = "toolStripSeparator1";
      toolStripSeparator1.Size = new Size(6, 48);
      // 
      // toolStripSeparator3
      // 
      toolStripSeparator3.Name = "toolStripSeparator3";
      toolStripSeparator3.Size = new Size(6, 48);
      // 
      // tsbAdd
      // 
      tsbAdd.AutoSize = false;
      tsbAdd.DisplayStyle = ToolStripItemDisplayStyle.Image;
      tsbAdd.Enabled = false;
      tsbAdd.Image = (Image)resources.GetObject("tsbAdd.Image");
      tsbAdd.ImageTransparentColor = Color.Magenta;
      tsbAdd.Name = "tsbAdd";
      tsbAdd.Size = new Size(36, 36);
      tsbAdd.Text = "Add Run";
      tsbAdd.Click += tsbAdd_Click;
      // 
      // tsbRemove
      // 
      tsbRemove.AutoSize = false;
      tsbRemove.DisplayStyle = ToolStripItemDisplayStyle.Image;
      tsbRemove.Enabled = false;
      tsbRemove.Image = (Image)resources.GetObject("tsbRemove.Image");
      tsbRemove.ImageTransparentColor = Color.Magenta;
      tsbRemove.Name = "tsbRemove";
      tsbRemove.Size = new Size(36, 36);
      tsbRemove.Text = "Delete Run";
      tsbRemove.Click += tsbRemove_Click;
      // 
      // toolStripSeparator4
      // 
      toolStripSeparator4.Name = "toolStripSeparator4";
      toolStripSeparator4.Size = new Size(6, 48);
      // 
      // toolStripSeparator2
      // 
      toolStripSeparator2.Name = "toolStripSeparator2";
      toolStripSeparator2.Size = new Size(6, 48);
      // 
      // tsbRun
      // 
      tsbRun.AutoSize = false;
      tsbRun.DisplayStyle = ToolStripItemDisplayStyle.Image;
      tsbRun.Enabled = false;
      tsbRun.Image = (Image)resources.GetObject("tsbRun.Image");
      tsbRun.ImageTransparentColor = Color.Magenta;
      tsbRun.Name = "tsbRun";
      tsbRun.Size = new Size(36, 36);
      tsbRun.Text = "Run";
      tsbRun.Click += tsbRun_Click;
      // 
      // tsbPause
      // 
      tsbPause.AutoSize = false;
      tsbPause.DisplayStyle = ToolStripItemDisplayStyle.Image;
      tsbPause.Enabled = false;
      tsbPause.Image = (Image)resources.GetObject("tsbPause.Image");
      tsbPause.ImageTransparentColor = Color.Magenta;
      tsbPause.Name = "tsbPause";
      tsbPause.Size = new Size(36, 36);
      tsbPause.Text = "Pause";
      tsbPause.Click += tsbPause_Click;
      // 
      // tsbStop
      // 
      tsbStop.AutoSize = false;
      tsbStop.DisplayStyle = ToolStripItemDisplayStyle.Image;
      tsbStop.Enabled = false;
      tsbStop.Image = (Image)resources.GetObject("tsbStop.Image");
      tsbStop.ImageTransparentColor = Color.Magenta;
      tsbStop.Name = "tsbStop";
      tsbStop.Size = new Size(36, 36);
      tsbStop.Text = "Stop";
      tsbStop.Click += tsbStop_Click;
      // 
      // toolStripSeparator5
      // 
      toolStripSeparator5.Name = "toolStripSeparator5";
      toolStripSeparator5.Size = new Size(6, 48);
      // 
      // toolStripSeparator6
      // 
      toolStripSeparator6.Name = "toolStripSeparator6";
      toolStripSeparator6.Size = new Size(6, 48);
      // 
      // tsbResetStats
      // 
      tsbResetStats.DisplayStyle = ToolStripItemDisplayStyle.Image;
      tsbResetStats.Image = (Image)resources.GetObject("tsbResetStats.Image");
      tsbResetStats.ImageTransparentColor = Color.Magenta;
      tsbResetStats.Name = "tsbResetStats";
      tsbResetStats.Size = new Size(36, 43);
      tsbResetStats.Text = "Reset Statistics";
      tsbResetStats.Click += tsbResetStats_Click;
      // 
      // splitContainer1
      // 
      splitContainer1.Dock = DockStyle.Fill;
      splitContainer1.FixedPanel = FixedPanel.Panel1;
      splitContainer1.IsSplitterFixed = true;
      splitContainer1.Location = new Point(0, 48);
      splitContainer1.Name = "splitContainer1";
      // 
      // splitContainer1.Panel1
      // 
      splitContainer1.Panel1.Controls.Add(splitContainer3);
      splitContainer1.Panel1MinSize = 380;
      // 
      // splitContainer1.Panel2
      // 
      splitContainer1.Panel2.Controls.Add(tabControlMain);
      splitContainer1.Size = new Size(1140, 798);
      splitContainer1.SplitterDistance = 380;
      splitContainer1.TabIndex = 2;
      // 
      // splitContainer3
      // 
      splitContainer3.Dock = DockStyle.Fill;
      splitContainer3.FixedPanel = FixedPanel.Panel2;
      splitContainer3.IsSplitterFixed = true;
      splitContainer3.Location = new Point(0, 0);
      splitContainer3.Name = "splitContainer3";
      splitContainer3.Orientation = Orientation.Horizontal;
      // 
      // splitContainer3.Panel1
      // 
      splitContainer3.Panel1.Controls.Add(fileListBox);
      splitContainer3.Panel1.Controls.Add(panel3);
      splitContainer3.Panel1.Controls.Add(panelMeter);
      splitContainer3.Panel1.Controls.Add(panel1);
      // 
      // splitContainer3.Panel2
      // 
      splitContainer3.Panel2.Controls.Add(gbStats);
      splitContainer3.Size = new Size(380, 798);
      splitContainer3.SplitterDistance = 536;
      splitContainer3.TabIndex = 0;
      // 
      // fileListBox
      // 
      fileListBox.Dock = DockStyle.Fill;
      fileListBox.FormattingEnabled = true;
      fileListBox.HorizontalScrollbar = true;
      fileListBox.ItemHeight = 25;
      fileListBox.Location = new Point(0, 56);
      fileListBox.Name = "fileListBox";
      fileListBox.SelectionMode = SelectionMode.MultiExtended;
      fileListBox.Size = new Size(380, 384);
      fileListBox.TabIndex = 0;
      fileListBox.SelectedIndexChanged += fileListBox_SelectedIndexChanged;
      // 
      // panel3
      // 
      panel3.Controls.Add(lblFileInfo);
      panel3.Dock = DockStyle.Bottom;
      panel3.Location = new Point(0, 440);
      panel3.Name = "panel3";
      panel3.Size = new Size(380, 96);
      panel3.TabIndex = 3;
      // 
      // lblFileInfo
      // 
      lblFileInfo.Location = new Point(3, 3);
      lblFileInfo.Name = "lblFileInfo";
      lblFileInfo.Size = new Size(374, 93);
      lblFileInfo.TabIndex = 0;
      lblFileInfo.Text = "Select a single file from the queue for details.";
      // 
      // panelMeter
      // 
      panelMeter.Controls.Add(pan10);
      panelMeter.Controls.Add(pan9);
      panelMeter.Controls.Add(pan8);
      panelMeter.Controls.Add(pan7);
      panelMeter.Controls.Add(pan6);
      panelMeter.Controls.Add(pan5);
      panelMeter.Controls.Add(pan4);
      panelMeter.Controls.Add(pan3);
      panelMeter.Controls.Add(pan2);
      panelMeter.Controls.Add(pan1);
      panelMeter.Dock = DockStyle.Top;
      panelMeter.Location = new Point(0, 32);
      panelMeter.Name = "panelMeter";
      panelMeter.Padding = new Padding(3);
      panelMeter.Size = new Size(380, 24);
      panelMeter.TabIndex = 2;
      panelMeter.Paint += panelMeter_Paint;
      // 
      // pan10
      // 
      pan10.BackColor = Color.Gray;
      pan10.Dock = DockStyle.Left;
      pan10.Location = new Point(327, 3);
      pan10.Name = "pan10";
      pan10.Padding = new Padding(3);
      pan10.Size = new Size(36, 18);
      pan10.TabIndex = 9;
      // 
      // pan9
      // 
      pan9.BackColor = Color.Gray;
      pan9.Dock = DockStyle.Left;
      pan9.Location = new Point(291, 3);
      pan9.Name = "pan9";
      pan9.Padding = new Padding(3);
      pan9.Size = new Size(36, 18);
      pan9.TabIndex = 8;
      // 
      // pan8
      // 
      pan8.BackColor = Color.Gray;
      pan8.Dock = DockStyle.Left;
      pan8.Location = new Point(255, 3);
      pan8.Name = "pan8";
      pan8.Padding = new Padding(3);
      pan8.Size = new Size(36, 18);
      pan8.TabIndex = 7;
      // 
      // pan7
      // 
      pan7.BackColor = Color.Gray;
      pan7.Dock = DockStyle.Left;
      pan7.Location = new Point(219, 3);
      pan7.Name = "pan7";
      pan7.Padding = new Padding(3);
      pan7.Size = new Size(36, 18);
      pan7.TabIndex = 6;
      // 
      // pan6
      // 
      pan6.BackColor = Color.Gray;
      pan6.Dock = DockStyle.Left;
      pan6.Location = new Point(183, 3);
      pan6.Name = "pan6";
      pan6.Padding = new Padding(3);
      pan6.Size = new Size(36, 18);
      pan6.TabIndex = 5;
      // 
      // pan5
      // 
      pan5.BackColor = Color.Gray;
      pan5.Dock = DockStyle.Left;
      pan5.Location = new Point(147, 3);
      pan5.Name = "pan5";
      pan5.Padding = new Padding(3);
      pan5.Size = new Size(36, 18);
      pan5.TabIndex = 4;
      // 
      // pan4
      // 
      pan4.BackColor = Color.Gray;
      pan4.Dock = DockStyle.Left;
      pan4.Location = new Point(111, 3);
      pan4.Name = "pan4";
      pan4.Padding = new Padding(3);
      pan4.Size = new Size(36, 18);
      pan4.TabIndex = 3;
      // 
      // pan3
      // 
      pan3.BackColor = Color.Gray;
      pan3.Dock = DockStyle.Left;
      pan3.Location = new Point(75, 3);
      pan3.Name = "pan3";
      pan3.Padding = new Padding(3);
      pan3.Size = new Size(36, 18);
      pan3.TabIndex = 2;
      // 
      // pan2
      // 
      pan2.BackColor = Color.Gray;
      pan2.Dock = DockStyle.Left;
      pan2.Location = new Point(39, 3);
      pan2.Name = "pan2";
      pan2.Padding = new Padding(3);
      pan2.Size = new Size(36, 18);
      pan2.TabIndex = 1;
      // 
      // pan1
      // 
      pan1.BackColor = Color.Gray;
      pan1.Dock = DockStyle.Left;
      pan1.Location = new Point(3, 3);
      pan1.Name = "pan1";
      pan1.Padding = new Padding(3);
      pan1.Size = new Size(36, 18);
      pan1.TabIndex = 0;
      // 
      // panel1
      // 
      panel1.Controls.Add(lblSpeed);
      panel1.Controls.Add(tbSpeed);
      panel1.Dock = DockStyle.Top;
      panel1.Location = new Point(0, 0);
      panel1.Name = "panel1";
      panel1.Size = new Size(380, 32);
      panel1.TabIndex = 1;
      // 
      // lblSpeed
      // 
      lblSpeed.AutoSize = true;
      lblSpeed.Dock = DockStyle.Left;
      lblSpeed.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
      lblSpeed.Location = new Point(315, 0);
      lblSpeed.Name = "lblSpeed";
      lblSpeed.Size = new Size(38, 32);
      lblSpeed.TabIndex = 1;
      lblSpeed.Text = "1x";
      // 
      // tbSpeed
      // 
      tbSpeed.BackColor = SystemColors.Control;
      tbSpeed.Dock = DockStyle.Left;
      tbSpeed.LargeChange = 1;
      tbSpeed.Location = new Point(0, 0);
      tbSpeed.Maximum = 6;
      tbSpeed.Name = "tbSpeed";
      tbSpeed.Size = new Size(315, 32);
      tbSpeed.TabIndex = 0;
      tbSpeed.ValueChanged += tbSpeed_ValueChanged;
      // 
      // gbStats
      // 
      gbStats.Controls.Add(lblRunTime);
      gbStats.Controls.Add(lblScanStats);
      gbStats.Controls.Add(label3);
      gbStats.Controls.Add(label2);
      gbStats.Controls.Add(label1);
      gbStats.Dock = DockStyle.Fill;
      gbStats.Font = new Font("Segoe UI", 11F, FontStyle.Regular, GraphicsUnit.Point, 0);
      gbStats.Location = new Point(0, 0);
      gbStats.Name = "gbStats";
      gbStats.Size = new Size(380, 258);
      gbStats.TabIndex = 0;
      gbStats.TabStop = false;
      gbStats.Text = "Simulation Statistics";
      // 
      // lblRunTime
      // 
      lblRunTime.AutoSize = true;
      lblRunTime.Font = new Font("Cascadia Code", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
      lblRunTime.Location = new Point(153, 33);
      lblRunTime.Name = "lblRunTime";
      lblRunTime.Size = new Size(98, 96);
      lblRunTime.TabIndex = 4;
      lblRunTime.Text = "00:00\r\n00:00:00\r\n00:00:00\r\n0";
      // 
      // lblScanStats
      // 
      lblScanStats.AutoSize = true;
      lblScanStats.Font = new Font("Cascadia Code", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
      lblScanStats.Location = new Point(153, 173);
      lblScanStats.Name = "lblScanStats";
      lblScanStats.Size = new Size(120, 72);
      lblScanStats.TabIndex = 3;
      lblScanStats.Text = "0        0\r\n0        0\r\n0        0";
      // 
      // label3
      // 
      label3.AutoSize = true;
      label3.Font = new Font("Segoe UI", 9F, FontStyle.Underline, GraphicsUnit.Point, 0);
      label3.Location = new Point(153, 148);
      label3.Name = "label3";
      label3.Size = new Size(170, 25);
      label3.TabIndex = 2;
      label3.Text = "Current        All        ";
      // 
      // label2
      // 
      label2.AutoSize = true;
      label2.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
      label2.Location = new Point(12, 173);
      label2.Name = "label2";
      label2.Size = new Size(103, 75);
      label2.TabIndex = 1;
      label2.Text = "Total Scans:\r\nMS1 Scans:\r\nMS2 Scans:";
      // 
      // label1
      // 
      label1.AutoSize = true;
      label1.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
      label1.Location = new Point(12, 33);
      label1.Name = "label1";
      label1.Size = new Size(134, 100);
      label1.TabIndex = 0;
      label1.Text = "Current Run RT:\r\nSim Run Time:\r\nTotal Sim Time:\r\nTotal Sim Runs:";
      // 
      // tabControlMain
      // 
      tabControlMain.Controls.Add(tabPage4);
      tabControlMain.Controls.Add(tabPage5);
      tabControlMain.Dock = DockStyle.Fill;
      tabControlMain.Location = new Point(0, 0);
      tabControlMain.Name = "tabControlMain";
      tabControlMain.SelectedIndex = 0;
      tabControlMain.Size = new Size(756, 798);
      tabControlMain.TabIndex = 1;
      tabControlMain.SelectedIndexChanged += tabControlMain_SelectedIndexChanged;
      // 
      // tabPage4
      // 
      tabPage4.Controls.Add(splitContainer2);
      tabPage4.Location = new Point(4, 34);
      tabPage4.Name = "tabPage4";
      tabPage4.Padding = new Padding(3);
      tabPage4.Size = new Size(748, 760);
      tabPage4.TabIndex = 0;
      tabPage4.Text = "Run Parameters";
      tabPage4.UseVisualStyleBackColor = true;
      // 
      // splitContainer2
      // 
      splitContainer2.Dock = DockStyle.Fill;
      splitContainer2.Location = new Point(3, 3);
      splitContainer2.Name = "splitContainer2";
      splitContainer2.Orientation = Orientation.Horizontal;
      // 
      // splitContainer2.Panel1
      // 
      splitContainer2.Panel1.Controls.Add(dgvFiles);
      splitContainer2.Size = new Size(742, 754);
      splitContainer2.SplitterDistance = 534;
      splitContainer2.TabIndex = 0;
      // 
      // dgvFiles
      // 
      dgvFiles.AllowUserToAddRows = false;
      dgvFiles.AllowUserToDeleteRows = false;
      dgvFiles.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      dgvFiles.Columns.AddRange(new DataGridViewColumn[] { rpFileName, rpFilePath, rpRunSpan, rpFirst, rpEnd });
      dgvFiles.Dock = DockStyle.Fill;
      dgvFiles.Location = new Point(0, 0);
      dgvFiles.Name = "dgvFiles";
      dgvFiles.RowHeadersWidth = 62;
      dgvFiles.Size = new Size(742, 534);
      dgvFiles.TabIndex = 0;
      dgvFiles.CellValueChanged += dgvFiles_CellValueChanged;
      dgvFiles.EditingControlShowing += dgvFiles_EditingControlShowing;
      dgvFiles.SelectionChanged += dgvFiles_SelectionChanged;
      // 
      // rpFileName
      // 
      rpFileName.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
      rpFileName.HeaderText = "File Name";
      rpFileName.MinimumWidth = 8;
      rpFileName.Name = "rpFileName";
      rpFileName.ReadOnly = true;
      // 
      // rpFilePath
      // 
      rpFilePath.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
      rpFilePath.HeaderText = "Path";
      rpFilePath.MinimumWidth = 8;
      rpFilePath.Name = "rpFilePath";
      rpFilePath.ReadOnly = true;
      // 
      // rpRunSpan
      // 
      rpRunSpan.HeaderText = "Run Span";
      rpRunSpan.Items.AddRange(new object[] { "Spectrum", "Retention Time" });
      rpRunSpan.MinimumWidth = 8;
      rpRunSpan.Name = "rpRunSpan";
      rpRunSpan.Width = 150;
      // 
      // rpFirst
      // 
      rpFirst.HeaderText = "Start";
      rpFirst.MinimumWidth = 8;
      rpFirst.Name = "rpFirst";
      rpFirst.Resizable = DataGridViewTriState.True;
      rpFirst.SortMode = DataGridViewColumnSortMode.NotSortable;
      rpFirst.Width = 150;
      // 
      // rpEnd
      // 
      rpEnd.HeaderText = "End";
      rpEnd.MinimumWidth = 8;
      rpEnd.Name = "rpEnd";
      rpEnd.Resizable = DataGridViewTriState.True;
      rpEnd.SortMode = DataGridViewColumnSortMode.NotSortable;
      rpEnd.Width = 150;
      // 
      // tabPage5
      // 
      tabPage5.Controls.Add(plotContainer);
      tabPage5.Location = new Point(4, 34);
      tabPage5.Name = "tabPage5";
      tabPage5.Padding = new Padding(3);
      tabPage5.Size = new Size(748, 760);
      tabPage5.TabIndex = 1;
      tabPage5.Text = "Real Time Display";
      tabPage5.UseVisualStyleBackColor = true;
      // 
      // plotContainer
      // 
      plotContainer.Dock = DockStyle.Fill;
      plotContainer.Location = new Point(3, 3);
      plotContainer.Name = "plotContainer";
      plotContainer.Orientation = Orientation.Horizontal;
      // 
      // plotContainer.Panel1
      // 
      plotContainer.Panel1.Controls.Add(plotTIC);
      // 
      // plotContainer.Panel2
      // 
      plotContainer.Panel2.Controls.Add(tabControl1);
      plotContainer.Size = new Size(742, 754);
      plotContainer.SplitterDistance = 368;
      plotContainer.TabIndex = 0;
      // 
      // plotTIC
      // 
      plotTIC.DisplayScale = 1.5F;
      plotTIC.Dock = DockStyle.Fill;
      plotTIC.Location = new Point(0, 0);
      plotTIC.Name = "plotTIC";
      plotTIC.Size = new Size(742, 368);
      plotTIC.TabIndex = 0;
      // 
      // tabControl1
      // 
      tabControl1.Alignment = TabAlignment.Bottom;
      tabControl1.Controls.Add(tabPage1);
      tabControl1.Controls.Add(tabPage2);
      tabControl1.Controls.Add(tabPage3);
      tabControl1.Dock = DockStyle.Fill;
      tabControl1.Location = new Point(0, 0);
      tabControl1.Name = "tabControl1";
      tabControl1.SelectedIndex = 0;
      tabControl1.Size = new Size(742, 382);
      tabControl1.TabIndex = 1;
      // 
      // tabPage1
      // 
      tabPage1.Controls.Add(rtbLog);
      tabPage1.Location = new Point(4, 4);
      tabPage1.Name = "tabPage1";
      tabPage1.Padding = new Padding(3);
      tabPage1.Size = new Size(734, 344);
      tabPage1.TabIndex = 0;
      tabPage1.Text = "Message Log";
      tabPage1.UseVisualStyleBackColor = true;
      // 
      // rtbLog
      // 
      rtbLog.Dock = DockStyle.Fill;
      rtbLog.Location = new Point(3, 3);
      rtbLog.Name = "rtbLog";
      rtbLog.Size = new Size(728, 338);
      rtbLog.TabIndex = 0;
      rtbLog.Text = "";
      // 
      // tabPage2
      // 
      tabPage2.Controls.Add(plotSpectrum);
      tabPage2.Controls.Add(labelSpectrum);
      tabPage2.Controls.Add(labelScanNumber);
      tabPage2.Location = new Point(4, 4);
      tabPage2.Name = "tabPage2";
      tabPage2.Padding = new Padding(3);
      tabPage2.Size = new Size(734, 344);
      tabPage2.TabIndex = 1;
      tabPage2.Text = "Spectrum Viewer";
      tabPage2.UseVisualStyleBackColor = true;
      // 
      // plotSpectrum
      // 
      plotSpectrum.DisplayScale = 1.5F;
      plotSpectrum.Dock = DockStyle.Fill;
      plotSpectrum.Location = new Point(3, 53);
      plotSpectrum.Name = "plotSpectrum";
      plotSpectrum.Size = new Size(728, 288);
      plotSpectrum.TabIndex = 0;
      // 
      // labelSpectrum
      // 
      labelSpectrum.AutoSize = true;
      labelSpectrum.Dock = DockStyle.Top;
      labelSpectrum.Location = new Point(3, 28);
      labelSpectrum.Name = "labelSpectrum";
      labelSpectrum.Size = new Size(88, 25);
      labelSpectrum.TabIndex = 1;
      labelSpectrum.Text = "Spectrum";
      // 
      // labelScanNumber
      // 
      labelScanNumber.AutoSize = true;
      labelScanNumber.Dock = DockStyle.Top;
      labelScanNumber.Location = new Point(3, 3);
      labelScanNumber.Name = "labelScanNumber";
      labelScanNumber.Size = new Size(114, 25);
      labelScanNumber.TabIndex = 2;
      labelScanNumber.Text = "ScanNumber";
      // 
      // tabPage3
      // 
      tabPage3.Controls.Add(dgvCustomScan);
      tabPage3.Controls.Add(panel2);
      tabPage3.Location = new Point(4, 4);
      tabPage3.Name = "tabPage3";
      tabPage3.Padding = new Padding(3);
      tabPage3.Size = new Size(734, 344);
      tabPage3.TabIndex = 2;
      tabPage3.Text = "Custom Scans";
      tabPage3.UseVisualStyleBackColor = true;
      // 
      // dgvCustomScan
      // 
      dgvCustomScan.AllowUserToAddRows = false;
      dgvCustomScan.AllowUserToDeleteRows = false;
      dgvCustomScan.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      dgvCustomScan.Dock = DockStyle.Fill;
      dgvCustomScan.Location = new Point(3, 39);
      dgvCustomScan.Name = "dgvCustomScan";
      dgvCustomScan.ReadOnly = true;
      dgvCustomScan.RowHeadersVisible = false;
      dgvCustomScan.RowHeadersWidth = 62;
      dgvCustomScan.Size = new Size(728, 302);
      dgvCustomScan.TabIndex = 1;
      // 
      // panel2
      // 
      panel2.Controls.Add(button1);
      panel2.Controls.Add(cbClearCS);
      panel2.Controls.Add(nudCustomScans);
      panel2.Controls.Add(lblCustomScansRT);
      panel2.Controls.Add(lblCustomScans);
      panel2.Controls.Add(label4);
      panel2.Dock = DockStyle.Top;
      panel2.Location = new Point(3, 3);
      panel2.Name = "panel2";
      panel2.Size = new Size(728, 36);
      panel2.TabIndex = 0;
      // 
      // button1
      // 
      button1.Location = new Point(445, 3);
      button1.Name = "button1";
      button1.Size = new Size(64, 32);
      button1.TabIndex = 8;
      button1.Text = "Clear";
      button1.UseVisualStyleBackColor = true;
      button1.Click += button1_Click;
      // 
      // cbClearCS
      // 
      cbClearCS.AutoSize = true;
      cbClearCS.Checked = true;
      cbClearCS.CheckState = CheckState.Checked;
      cbClearCS.Location = new Point(515, 3);
      cbClearCS.Name = "cbClearCS";
      cbClearCS.Size = new Size(210, 29);
      cbClearCS.TabIndex = 7;
      cbClearCS.Text = "Auto-clear Upon Start";
      cbClearCS.UseVisualStyleBackColor = true;
      // 
      // nudCustomScans
      // 
      nudCustomScans.Location = new Point(3, 3);
      nudCustomScans.Maximum = new decimal(new int[] { 0, 0, 0, 0 });
      nudCustomScans.Name = "nudCustomScans";
      nudCustomScans.RightToLeft = RightToLeft.No;
      nudCustomScans.Size = new Size(128, 31);
      nudCustomScans.TabIndex = 6;
      nudCustomScans.ValueChanged += nudCustomScans_ValueChanged;
      // 
      // lblCustomScansRT
      // 
      lblCustomScansRT.AutoSize = true;
      lblCustomScansRT.Location = new Point(255, 5);
      lblCustomScansRT.Name = "lblCustomScansRT";
      lblCustomScansRT.Size = new Size(35, 25);
      lblCustomScansRT.TabIndex = 5;
      lblCustomScansRT.Text = "RT:";
      // 
      // lblCustomScans
      // 
      lblCustomScans.AutoSize = true;
      lblCustomScans.Location = new Point(172, 5);
      lblCustomScans.Name = "lblCustomScans";
      lblCustomScans.Size = new Size(22, 25);
      lblCustomScans.TabIndex = 4;
      lblCustomScans.Text = "0";
      // 
      // label4
      // 
      label4.AutoSize = true;
      label4.Location = new Point(137, 5);
      label4.Name = "label4";
      label4.Size = new Size(29, 25);
      label4.TabIndex = 3;
      label4.Text = "of";
      // 
      // ofdAdd
      // 
      ofdAdd.Filter = "All MS files|*.mzML;*.mzXML;*.raw|MzML|*.mzML|MzXML|*.mzXML|Thermo Raw|*.raw";
      ofdAdd.Multiselect = true;
      // 
      // statusStrip1
      // 
      statusStrip1.AutoSize = false;
      statusStrip1.ImageScalingSize = new Size(24, 24);
      statusStrip1.Location = new Point(0, 846);
      statusStrip1.Name = "statusStrip1";
      statusStrip1.Size = new Size(1140, 32);
      statusStrip1.TabIndex = 3;
      statusStrip1.Text = "statusStrip1";
      // 
      // Corona
      // 
      AutoScaleDimensions = new SizeF(10F, 25F);
      AutoScaleMode = AutoScaleMode.Font;
      ClientSize = new Size(1140, 878);
      Controls.Add(splitContainer1);
      Controls.Add(mainToolStrip);
      Controls.Add(statusStrip1);
      Icon = (Icon)resources.GetObject("$this.Icon");
      Name = "Corona";
      Text = "Corona";
      mainToolStrip.ResumeLayout(false);
      mainToolStrip.PerformLayout();
      splitContainer1.Panel1.ResumeLayout(false);
      splitContainer1.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
      splitContainer1.ResumeLayout(false);
      splitContainer3.Panel1.ResumeLayout(false);
      splitContainer3.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)splitContainer3).EndInit();
      splitContainer3.ResumeLayout(false);
      panel3.ResumeLayout(false);
      panelMeter.ResumeLayout(false);
      panel1.ResumeLayout(false);
      panel1.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)tbSpeed).EndInit();
      gbStats.ResumeLayout(false);
      gbStats.PerformLayout();
      tabControlMain.ResumeLayout(false);
      tabPage4.ResumeLayout(false);
      splitContainer2.Panel1.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)splitContainer2).EndInit();
      splitContainer2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)dgvFiles).EndInit();
      tabPage5.ResumeLayout(false);
      plotContainer.Panel1.ResumeLayout(false);
      plotContainer.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)plotContainer).EndInit();
      plotContainer.ResumeLayout(false);
      tabControl1.ResumeLayout(false);
      tabPage1.ResumeLayout(false);
      tabPage2.ResumeLayout(false);
      tabPage2.PerformLayout();
      tabPage3.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)dgvCustomScan).EndInit();
      panel2.ResumeLayout(false);
      panel2.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)nudCustomScans).EndInit();
      ResumeLayout(false);
    }

    #endregion

    private ToolStrip mainToolStrip;
    private SplitContainer splitContainer1;
    private SplitContainer splitContainer3;
    private SplitContainer plotContainer;
    private ListBox fileListBox;
    private ToolStripButton tsbAdd;
    private OpenFileDialog ofdAdd;
    private ToolStripButton tsbRemove;
    private ScottPlot.WinForms.FormsPlot plotTIC;
    private ToolStripButton tsbRun;
    private RichTextBox rtbLog;
    private TabControl tabControl1;
    private TabPage tabPage1;
    private TabPage tabPage2;
    private ScottPlot.WinForms.FormsPlot plotSpectrum;
    private Label labelSpectrum;
    private Label labelScanNumber;
    private StatusStrip statusStrip1;
    private GroupBox gbStats;
    private Label label1;
    private Label label2;
    private Label lblScanStats;
    private Label label3;
    private Label lblRunTime;
    private Panel panel1;
    private TrackBar tbSpeed;
    private Label lblSpeed;
    private ToolStripButton tsbPause;
    private ToolStripButton tsbStop;
    private TabPage tabPage3;
    private DataGridView dgvCustomScan;
    private Panel panel2;
    private Label lblCustomScansRT;
    private Label lblCustomScans;
    private Label label4;
    private NumericUpDown nudCustomScans;
    private TabControl tabControlMain;
    private TabPage tabPage4;
    private TabPage tabPage5;
    private SplitContainer splitContainer2;
    private DataGridView dgvFiles;
    private ToolStripButton tsbLoad;
    private ToolStripButton tsbDelete;
    private ToolStripSeparator toolStripSeparator1;
    private ToolStripSeparator toolStripSeparator2;
    private ToolStripSeparator toolStripSeparator3;
    private ToolStripSeparator toolStripSeparator4;
    private Panel panelMeter;
    private Panel pan2;
    private Panel pan1;
    private Panel pan5;
    private Panel pan4;
    private Panel pan3;
    private Panel pan10;
    private Panel pan9;
    private Panel pan8;
    private Panel pan7;
    private Panel pan6;
    private Panel panel3;
    private Label lblFileInfo;
    private DataGridViewTextBoxColumn rpFileName;
    private DataGridViewTextBoxColumn rpFilePath;
    private DataGridViewComboBoxColumn rpRunSpan;
    private DataGridViewTextBoxColumn rpFirst;
    private DataGridViewTextBoxColumn rpEnd;
    private Button button1;
    private CheckBox cbClearCS;
    private ToolStripSeparator toolStripSeparator5;
    private ToolStripSeparator toolStripSeparator6;
    private ToolStripButton tsbResetStats;
  }
}
