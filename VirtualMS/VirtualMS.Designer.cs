namespace VirtualMS
{
  partial class VirtualMs
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VirtualMs));
      mainToolStrip = new ToolStrip();
      tspAdd = new ToolStripButton();
      tsbRemove = new ToolStripButton();
      tsbRun = new ToolStripButton();
      tsbPause = new ToolStripButton();
      tsbStop = new ToolStripButton();
      splitContainer1 = new SplitContainer();
      splitContainer3 = new SplitContainer();
      fileListBox = new ListBox();
      panel1 = new Panel();
      lblSpeed = new Label();
      tbSpeed = new TrackBar();
      gbStats = new GroupBox();
      lblRunTime = new Label();
      lblScanStats = new Label();
      label3 = new Label();
      label2 = new Label();
      label1 = new Label();
      plotContainer = new SplitContainer();
      plotTIC = new ScottPlot.WinForms.FormsPlot();
      tabControl1 = new TabControl();
      tabPage1 = new TabPage();
      rtbLog = new RichTextBox();
      tabPage2 = new TabPage();
      plotSpectrum = new ScottPlot.WinForms.FormsPlot();
      labelSpectrum = new Label();
      labelScanNumber = new Label();
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
      panel1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)tbSpeed).BeginInit();
      gbStats.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)plotContainer).BeginInit();
      plotContainer.Panel1.SuspendLayout();
      plotContainer.Panel2.SuspendLayout();
      plotContainer.SuspendLayout();
      tabControl1.SuspendLayout();
      tabPage1.SuspendLayout();
      tabPage2.SuspendLayout();
      SuspendLayout();
      // 
      // mainToolStrip
      // 
      mainToolStrip.AutoSize = false;
      mainToolStrip.ImageScalingSize = new Size(32, 32);
      mainToolStrip.Items.AddRange(new ToolStripItem[] { tspAdd, tsbRemove, tsbRun, tsbPause, tsbStop });
      mainToolStrip.Location = new Point(0, 0);
      mainToolStrip.Name = "mainToolStrip";
      mainToolStrip.Size = new Size(1140, 48);
      mainToolStrip.TabIndex = 0;
      mainToolStrip.Text = "toolStrip1";
      // 
      // tspAdd
      // 
      tspAdd.AutoSize = false;
      tspAdd.DisplayStyle = ToolStripItemDisplayStyle.Image;
      tspAdd.Image = (Image)resources.GetObject("tspAdd.Image");
      tspAdd.ImageTransparentColor = Color.Magenta;
      tspAdd.Name = "tspAdd";
      tspAdd.Size = new Size(36, 36);
      tspAdd.Text = "Add Run";
      tspAdd.Click += tspAdd_Click;
      // 
      // tsbRemove
      // 
      tsbRemove.AutoSize = false;
      tsbRemove.DisplayStyle = ToolStripItemDisplayStyle.Image;
      tsbRemove.Image = (Image)resources.GetObject("tsbRemove.Image");
      tsbRemove.ImageTransparentColor = Color.Magenta;
      tsbRemove.Name = "tsbRemove";
      tsbRemove.Size = new Size(36, 36);
      tsbRemove.Text = "Delete Run";
      tsbRemove.Click += tsbRemove_Click;
      // 
      // tsbRun
      // 
      tsbRun.AutoSize = false;
      tsbRun.DisplayStyle = ToolStripItemDisplayStyle.Image;
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
      tsbStop.Image = (Image)resources.GetObject("tsbStop.Image");
      tsbStop.ImageTransparentColor = Color.Magenta;
      tsbStop.Name = "tsbStop";
      tsbStop.Size = new Size(36, 36);
      tsbStop.Text = "Stop";
      tsbStop.Click += tsbStop_Click;
      // 
      // splitContainer1
      // 
      splitContainer1.Dock = DockStyle.Fill;
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
      splitContainer1.Panel2.Controls.Add(plotContainer);
      splitContainer1.Size = new Size(1140, 798);
      splitContainer1.SplitterDistance = 380;
      splitContainer1.TabIndex = 2;
      // 
      // splitContainer3
      // 
      splitContainer3.Dock = DockStyle.Fill;
      splitContainer3.Location = new Point(0, 0);
      splitContainer3.Name = "splitContainer3";
      splitContainer3.Orientation = Orientation.Horizontal;
      // 
      // splitContainer3.Panel1
      // 
      splitContainer3.Panel1.Controls.Add(fileListBox);
      splitContainer3.Panel1.Controls.Add(panel1);
      // 
      // splitContainer3.Panel2
      // 
      splitContainer3.Panel2.Controls.Add(gbStats);
      splitContainer3.Size = new Size(380, 798);
      splitContainer3.SplitterDistance = 484;
      splitContainer3.TabIndex = 0;
      // 
      // fileListBox
      // 
      fileListBox.Dock = DockStyle.Fill;
      fileListBox.FormattingEnabled = true;
      fileListBox.HorizontalScrollbar = true;
      fileListBox.ItemHeight = 25;
      fileListBox.Location = new Point(0, 32);
      fileListBox.Name = "fileListBox";
      fileListBox.SelectionMode = SelectionMode.MultiExtended;
      fileListBox.Size = new Size(380, 452);
      fileListBox.TabIndex = 0;
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
      gbStats.Size = new Size(380, 310);
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
      // plotContainer
      // 
      plotContainer.Dock = DockStyle.Fill;
      plotContainer.Location = new Point(0, 0);
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
      plotContainer.Size = new Size(756, 798);
      plotContainer.SplitterDistance = 390;
      plotContainer.TabIndex = 0;
      // 
      // plotTIC
      // 
      plotTIC.DisplayScale = 1.5F;
      plotTIC.Dock = DockStyle.Fill;
      plotTIC.Location = new Point(0, 0);
      plotTIC.Name = "plotTIC";
      plotTIC.Size = new Size(756, 390);
      plotTIC.TabIndex = 0;
      // 
      // tabControl1
      // 
      tabControl1.Alignment = TabAlignment.Bottom;
      tabControl1.Controls.Add(tabPage1);
      tabControl1.Controls.Add(tabPage2);
      tabControl1.Dock = DockStyle.Fill;
      tabControl1.Location = new Point(0, 0);
      tabControl1.Name = "tabControl1";
      tabControl1.SelectedIndex = 0;
      tabControl1.Size = new Size(756, 404);
      tabControl1.TabIndex = 1;
      // 
      // tabPage1
      // 
      tabPage1.Controls.Add(rtbLog);
      tabPage1.Location = new Point(4, 4);
      tabPage1.Name = "tabPage1";
      tabPage1.Padding = new Padding(3);
      tabPage1.Size = new Size(748, 366);
      tabPage1.TabIndex = 0;
      tabPage1.Text = "Message Log";
      tabPage1.UseVisualStyleBackColor = true;
      // 
      // rtbLog
      // 
      rtbLog.Dock = DockStyle.Fill;
      rtbLog.Location = new Point(3, 3);
      rtbLog.Name = "rtbLog";
      rtbLog.Size = new Size(742, 360);
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
      tabPage2.Size = new Size(748, 366);
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
      plotSpectrum.Size = new Size(742, 310);
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
      // ofdAdd
      // 
      ofdAdd.Filter = "Thermo Raw|*.raw|MzML|*.mzML|All files|*.*";
      ofdAdd.FilterIndex = 3;
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
      // VirtualMs
      // 
      AutoScaleDimensions = new SizeF(10F, 25F);
      AutoScaleMode = AutoScaleMode.Font;
      ClientSize = new Size(1140, 878);
      Controls.Add(splitContainer1);
      Controls.Add(mainToolStrip);
      Controls.Add(statusStrip1);
      Name = "VirtualMs";
      Text = "Virtual MS";
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
      panel1.ResumeLayout(false);
      panel1.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)tbSpeed).EndInit();
      gbStats.ResumeLayout(false);
      gbStats.PerformLayout();
      plotContainer.Panel1.ResumeLayout(false);
      plotContainer.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)plotContainer).EndInit();
      plotContainer.ResumeLayout(false);
      tabControl1.ResumeLayout(false);
      tabPage1.ResumeLayout(false);
      tabPage2.ResumeLayout(false);
      tabPage2.PerformLayout();
      ResumeLayout(false);
    }

    #endregion

    private ToolStrip mainToolStrip;
    private SplitContainer splitContainer1;
    private SplitContainer splitContainer3;
    private SplitContainer plotContainer;
    private ListBox fileListBox;
    private ToolStripButton tspAdd;
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
  }
}
