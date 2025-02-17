using static System.Net.Mime.MediaTypeNames;
using System.Drawing;
using System.Windows.Forms;
using System.Xml.Linq;

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
      this.mainToolStrip = new System.Windows.Forms.ToolStrip();
      this.tspAdd = new System.Windows.Forms.ToolStripButton();
      this.tsbRemove = new System.Windows.Forms.ToolStripButton();
      this.tsbRun = new System.Windows.Forms.ToolStripButton();
      this.tsbPause = new System.Windows.Forms.ToolStripButton();
      this.tsbStop = new System.Windows.Forms.ToolStripButton();
      this.splitContainer1 = new System.Windows.Forms.SplitContainer();
      this.splitContainer3 = new System.Windows.Forms.SplitContainer();
      this.fileListBox = new System.Windows.Forms.ListBox();
      this.panel1 = new System.Windows.Forms.Panel();
      this.lblSpeed = new System.Windows.Forms.Label();
      this.tbSpeed = new System.Windows.Forms.TrackBar();
      this.gbStats = new System.Windows.Forms.GroupBox();
      this.lblRunTime = new System.Windows.Forms.Label();
      this.lblScanStats = new System.Windows.Forms.Label();
      this.label3 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.label1 = new System.Windows.Forms.Label();
      this.plotContainer = new System.Windows.Forms.SplitContainer();
      this.plotTIC = new ScottPlot.WinForms.FormsPlot();
      this.tabControl1 = new System.Windows.Forms.TabControl();
      this.tabPage1 = new System.Windows.Forms.TabPage();
      this.rtbLog = new System.Windows.Forms.RichTextBox();
      this.tabPage2 = new System.Windows.Forms.TabPage();
      this.plotSpectrum = new ScottPlot.WinForms.FormsPlot();
      this.labelSpectrum = new System.Windows.Forms.Label();
      this.labelScanNumber = new System.Windows.Forms.Label();
      this.ofdAdd = new System.Windows.Forms.OpenFileDialog();
      this.statusStrip1 = new System.Windows.Forms.StatusStrip();
      this.mainToolStrip.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
      this.splitContainer1.Panel1.SuspendLayout();
      this.splitContainer1.Panel2.SuspendLayout();
      this.splitContainer1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
      this.splitContainer3.Panel1.SuspendLayout();
      this.splitContainer3.Panel2.SuspendLayout();
      this.splitContainer3.SuspendLayout();
      this.panel1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.tbSpeed)).BeginInit();
      this.gbStats.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.plotContainer)).BeginInit();
      this.plotContainer.Panel1.SuspendLayout();
      this.plotContainer.Panel2.SuspendLayout();
      this.plotContainer.SuspendLayout();
      this.tabControl1.SuspendLayout();
      this.tabPage1.SuspendLayout();
      this.tabPage2.SuspendLayout();
      this.SuspendLayout();
      // 
      // mainToolStrip
      // 
      this.mainToolStrip.AutoSize = false;
      this.mainToolStrip.ImageScalingSize = new System.Drawing.Size(32, 32);
      this.mainToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tspAdd,
            this.tsbRemove,
            this.tsbRun,
            this.tsbPause,
            this.tsbStop});
      this.mainToolStrip.Location = new System.Drawing.Point(0, 0);
      this.mainToolStrip.Name = "mainToolStrip";
      this.mainToolStrip.Size = new System.Drawing.Size(915, 36);
      this.mainToolStrip.TabIndex = 0;
      this.mainToolStrip.Text = "toolStrip1";
      // 
      // tspAdd
      // 
      this.tspAdd.AutoSize = false;
      this.tspAdd.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.tspAdd.Image = ((System.Drawing.Image)(resources.GetObject("tspAdd.Image")));
      this.tspAdd.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
      this.tspAdd.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.tspAdd.Margin = new System.Windows.Forms.Padding(0);
      this.tspAdd.Name = "tspAdd";
      this.tspAdd.Size = new System.Drawing.Size(32, 32);
      this.tspAdd.Text = "Add Run";
      this.tspAdd.Click += new System.EventHandler(this.tspAdd_Click);
      // 
      // tsbRemove
      // 
      this.tsbRemove.AutoSize = false;
      this.tsbRemove.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.tsbRemove.Image = ((System.Drawing.Image)(resources.GetObject("tsbRemove.Image")));
      this.tsbRemove.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
      this.tsbRemove.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.tsbRemove.Margin = new System.Windows.Forms.Padding(0);
      this.tsbRemove.Name = "tsbRemove";
      this.tsbRemove.Size = new System.Drawing.Size(32, 32);
      this.tsbRemove.Text = "Delete Run";
      this.tsbRemove.Click += new System.EventHandler(this.tsbRemove_Click);
      // 
      // tsbRun
      // 
      this.tsbRun.AutoSize = false;
      this.tsbRun.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.tsbRun.Image = ((System.Drawing.Image)(resources.GetObject("tsbRun.Image")));
      this.tsbRun.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
      this.tsbRun.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.tsbRun.Margin = new System.Windows.Forms.Padding(0);
      this.tsbRun.Name = "tsbRun";
      this.tsbRun.Size = new System.Drawing.Size(32, 32);
      this.tsbRun.Text = "Run";
      this.tsbRun.Click += new System.EventHandler(this.tsbRun_Click);
      // 
      // tsbPause
      // 
      this.tsbPause.AutoSize = false;
      this.tsbPause.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.tsbPause.Image = ((System.Drawing.Image)(resources.GetObject("tsbPause.Image")));
      this.tsbPause.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
      this.tsbPause.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.tsbPause.Name = "tsbPause";
      this.tsbPause.Size = new System.Drawing.Size(32, 32);
      this.tsbPause.Text = "Pause";
      this.tsbPause.Click += new System.EventHandler(this.tsbPause_Click);
      // 
      // tsbStop
      // 
      this.tsbStop.AutoSize = false;
      this.tsbStop.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.tsbStop.Image = ((System.Drawing.Image)(resources.GetObject("tsbStop.Image")));
      this.tsbStop.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
      this.tsbStop.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.tsbStop.Name = "tsbStop";
      this.tsbStop.Size = new System.Drawing.Size(32, 32);
      this.tsbStop.Text = "Stop";
      this.tsbStop.Click += new System.EventHandler(this.tsbStop_Click);
      // 
      // splitContainer1
      // 
      this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
      this.splitContainer1.Location = new System.Drawing.Point(0, 36);
      this.splitContainer1.Margin = new System.Windows.Forms.Padding(2);
      this.splitContainer1.Name = "splitContainer1";
      // 
      // splitContainer1.Panel1
      // 
      this.splitContainer1.Panel1.Controls.Add(this.splitContainer3);
      this.splitContainer1.Panel1MinSize = 240;
      // 
      // splitContainer1.Panel2
      // 
      this.splitContainer1.Panel2.Controls.Add(this.plotContainer);
      this.splitContainer1.Size = new System.Drawing.Size(915, 611);
      this.splitContainer1.SplitterDistance = 240;
      this.splitContainer1.SplitterWidth = 2;
      this.splitContainer1.TabIndex = 2;
      // 
      // splitContainer3
      // 
      this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
      this.splitContainer3.Location = new System.Drawing.Point(0, 0);
      this.splitContainer3.Margin = new System.Windows.Forms.Padding(2);
      this.splitContainer3.Name = "splitContainer3";
      this.splitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal;
      // 
      // splitContainer3.Panel1
      // 
      this.splitContainer3.Panel1.Controls.Add(this.fileListBox);
      this.splitContainer3.Panel1.Controls.Add(this.panel1);
      // 
      // splitContainer3.Panel2
      // 
      this.splitContainer3.Panel2.Controls.Add(this.gbStats);
      this.splitContainer3.Size = new System.Drawing.Size(240, 611);
      this.splitContainer3.SplitterDistance = 369;
      this.splitContainer3.SplitterWidth = 2;
      this.splitContainer3.TabIndex = 0;
      // 
      // fileListBox
      // 
      this.fileListBox.Dock = System.Windows.Forms.DockStyle.Fill;
      this.fileListBox.FormattingEnabled = true;
      this.fileListBox.HorizontalScrollbar = true;
      this.fileListBox.Location = new System.Drawing.Point(0, 32);
      this.fileListBox.Margin = new System.Windows.Forms.Padding(2);
      this.fileListBox.Name = "fileListBox";
      this.fileListBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
      this.fileListBox.Size = new System.Drawing.Size(240, 337);
      this.fileListBox.TabIndex = 0;
      // 
      // panel1
      // 
      this.panel1.Controls.Add(this.lblSpeed);
      this.panel1.Controls.Add(this.tbSpeed);
      this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
      this.panel1.Location = new System.Drawing.Point(0, 0);
      this.panel1.Margin = new System.Windows.Forms.Padding(2);
      this.panel1.Name = "panel1";
      this.panel1.Size = new System.Drawing.Size(240, 32);
      this.panel1.TabIndex = 1;
      // 
      // lblSpeed
      // 
      this.lblSpeed.AutoSize = true;
      this.lblSpeed.Dock = System.Windows.Forms.DockStyle.Left;
      this.lblSpeed.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblSpeed.Location = new System.Drawing.Point(189, 0);
      this.lblSpeed.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
      this.lblSpeed.Name = "lblSpeed";
      this.lblSpeed.Size = new System.Drawing.Size(26, 21);
      this.lblSpeed.TabIndex = 1;
      this.lblSpeed.Text = "1x";
      // 
      // tbSpeed
      // 
      this.tbSpeed.BackColor = System.Drawing.SystemColors.Control;
      this.tbSpeed.Dock = System.Windows.Forms.DockStyle.Left;
      this.tbSpeed.LargeChange = 1;
      this.tbSpeed.Location = new System.Drawing.Point(0, 0);
      this.tbSpeed.Margin = new System.Windows.Forms.Padding(2);
      this.tbSpeed.Maximum = 6;
      this.tbSpeed.Name = "tbSpeed";
      this.tbSpeed.Size = new System.Drawing.Size(189, 32);
      this.tbSpeed.TabIndex = 0;
      this.tbSpeed.ValueChanged += new System.EventHandler(this.tbSpeed_ValueChanged);
      // 
      // gbStats
      // 
      this.gbStats.Controls.Add(this.lblRunTime);
      this.gbStats.Controls.Add(this.lblScanStats);
      this.gbStats.Controls.Add(this.label3);
      this.gbStats.Controls.Add(this.label2);
      this.gbStats.Controls.Add(this.label1);
      this.gbStats.Dock = System.Windows.Forms.DockStyle.Fill;
      this.gbStats.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.gbStats.Location = new System.Drawing.Point(0, 0);
      this.gbStats.Margin = new System.Windows.Forms.Padding(2);
      this.gbStats.Name = "gbStats";
      this.gbStats.Padding = new System.Windows.Forms.Padding(2);
      this.gbStats.Size = new System.Drawing.Size(240, 240);
      this.gbStats.TabIndex = 0;
      this.gbStats.TabStop = false;
      this.gbStats.Text = "Simulation Statistics";
      // 
      // lblRunTime
      // 
      this.lblRunTime.AutoSize = true;
      this.lblRunTime.Font = new System.Drawing.Font("Cascadia Code", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblRunTime.Location = new System.Drawing.Point(126, 26);
      this.lblRunTime.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
      this.lblRunTime.Name = "lblRunTime";
      this.lblRunTime.Size = new System.Drawing.Size(63, 64);
      this.lblRunTime.TabIndex = 4;
      this.lblRunTime.Text = "00:00\r\n00:00:00\r\n00:00:00\r\n0";
      // 
      // lblScanStats
      // 
      this.lblScanStats.AutoSize = true;
      this.lblScanStats.Font = new System.Drawing.Font("Cascadia Code", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblScanStats.Location = new System.Drawing.Point(124, 120);
      this.lblScanStats.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
      this.lblScanStats.Name = "lblScanStats";
      this.lblScanStats.Size = new System.Drawing.Size(77, 48);
      this.lblScanStats.TabIndex = 3;
      this.lblScanStats.Text = "0        0\r\n0        0\r\n0        0";
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label3.Location = new System.Drawing.Point(124, 102);
      this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(109, 15);
      this.label3.TabIndex = 2;
      this.label3.Text = "Current        All        ";
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label2.Location = new System.Drawing.Point(11, 117);
      this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(76, 51);
      this.label2.TabIndex = 1;
      this.label2.Text = "Total Scans:\r\nMS1 Scans:\r\nMS2 Scans:";
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label1.Location = new System.Drawing.Point(11, 22);
      this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(98, 68);
      this.label1.TabIndex = 0;
      this.label1.Text = "Current Run RT:\r\nSim Run Time:\r\nTotal Sim Time:\r\nTotal Sim Runs:";
      // 
      // plotContainer
      // 
      this.plotContainer.Dock = System.Windows.Forms.DockStyle.Fill;
      this.plotContainer.Location = new System.Drawing.Point(0, 0);
      this.plotContainer.Margin = new System.Windows.Forms.Padding(2);
      this.plotContainer.Name = "plotContainer";
      this.plotContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
      // 
      // plotContainer.Panel1
      // 
      this.plotContainer.Panel1.Controls.Add(this.plotTIC);
      // 
      // plotContainer.Panel2
      // 
      this.plotContainer.Panel2.Controls.Add(this.tabControl1);
      this.plotContainer.Size = new System.Drawing.Size(673, 611);
      this.plotContainer.SplitterDistance = 296;
      this.plotContainer.SplitterWidth = 2;
      this.plotContainer.TabIndex = 0;
      // 
      // plotTIC
      // 
      this.plotTIC.DisplayScale = 1.5F;
      this.plotTIC.Dock = System.Windows.Forms.DockStyle.Fill;
      this.plotTIC.Location = new System.Drawing.Point(0, 0);
      this.plotTIC.Margin = new System.Windows.Forms.Padding(2);
      this.plotTIC.Name = "plotTIC";
      this.plotTIC.Size = new System.Drawing.Size(673, 296);
      this.plotTIC.TabIndex = 0;
      // 
      // tabControl1
      // 
      this.tabControl1.Alignment = System.Windows.Forms.TabAlignment.Bottom;
      this.tabControl1.Controls.Add(this.tabPage1);
      this.tabControl1.Controls.Add(this.tabPage2);
      this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tabControl1.Location = new System.Drawing.Point(0, 0);
      this.tabControl1.Margin = new System.Windows.Forms.Padding(2);
      this.tabControl1.Name = "tabControl1";
      this.tabControl1.SelectedIndex = 0;
      this.tabControl1.Size = new System.Drawing.Size(673, 313);
      this.tabControl1.TabIndex = 1;
      // 
      // tabPage1
      // 
      this.tabPage1.Controls.Add(this.rtbLog);
      this.tabPage1.Location = new System.Drawing.Point(4, 4);
      this.tabPage1.Margin = new System.Windows.Forms.Padding(2);
      this.tabPage1.Name = "tabPage1";
      this.tabPage1.Padding = new System.Windows.Forms.Padding(2);
      this.tabPage1.Size = new System.Drawing.Size(665, 287);
      this.tabPage1.TabIndex = 0;
      this.tabPage1.Text = "Message Log";
      this.tabPage1.UseVisualStyleBackColor = true;
      // 
      // rtbLog
      // 
      this.rtbLog.Dock = System.Windows.Forms.DockStyle.Fill;
      this.rtbLog.Location = new System.Drawing.Point(2, 2);
      this.rtbLog.Margin = new System.Windows.Forms.Padding(2);
      this.rtbLog.Name = "rtbLog";
      this.rtbLog.Size = new System.Drawing.Size(661, 283);
      this.rtbLog.TabIndex = 0;
      this.rtbLog.Text = "";
      // 
      // tabPage2
      // 
      this.tabPage2.Controls.Add(this.plotSpectrum);
      this.tabPage2.Controls.Add(this.labelSpectrum);
      this.tabPage2.Controls.Add(this.labelScanNumber);
      this.tabPage2.Location = new System.Drawing.Point(4, 4);
      this.tabPage2.Margin = new System.Windows.Forms.Padding(2);
      this.tabPage2.Name = "tabPage2";
      this.tabPage2.Padding = new System.Windows.Forms.Padding(2);
      this.tabPage2.Size = new System.Drawing.Size(665, 287);
      this.tabPage2.TabIndex = 1;
      this.tabPage2.Text = "Spectrum Viewer";
      this.tabPage2.UseVisualStyleBackColor = true;
      // 
      // plotSpectrum
      // 
      this.plotSpectrum.DisplayScale = 1.5F;
      this.plotSpectrum.Dock = System.Windows.Forms.DockStyle.Fill;
      this.plotSpectrum.Location = new System.Drawing.Point(2, 28);
      this.plotSpectrum.Margin = new System.Windows.Forms.Padding(2);
      this.plotSpectrum.Name = "plotSpectrum";
      this.plotSpectrum.Size = new System.Drawing.Size(661, 257);
      this.plotSpectrum.TabIndex = 0;
      // 
      // labelSpectrum
      // 
      this.labelSpectrum.AutoSize = true;
      this.labelSpectrum.Dock = System.Windows.Forms.DockStyle.Top;
      this.labelSpectrum.Location = new System.Drawing.Point(2, 15);
      this.labelSpectrum.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
      this.labelSpectrum.Name = "labelSpectrum";
      this.labelSpectrum.Size = new System.Drawing.Size(52, 13);
      this.labelSpectrum.TabIndex = 1;
      this.labelSpectrum.Text = "Spectrum";
      // 
      // labelScanNumber
      // 
      this.labelScanNumber.AutoSize = true;
      this.labelScanNumber.Dock = System.Windows.Forms.DockStyle.Top;
      this.labelScanNumber.Location = new System.Drawing.Point(2, 2);
      this.labelScanNumber.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
      this.labelScanNumber.Name = "labelScanNumber";
      this.labelScanNumber.Size = new System.Drawing.Size(69, 13);
      this.labelScanNumber.TabIndex = 2;
      this.labelScanNumber.Text = "ScanNumber";
      // 
      // ofdAdd
      // 
      this.ofdAdd.Filter = "Thermo Raw|*.raw|MzML|*.mzML|All files|*.*";
      this.ofdAdd.FilterIndex = 3;
      this.ofdAdd.Multiselect = true;
      // 
      // statusStrip1
      // 
      this.statusStrip1.AutoSize = false;
      this.statusStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
      this.statusStrip1.Location = new System.Drawing.Point(0, 647);
      this.statusStrip1.Name = "statusStrip1";
      this.statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 8, 0);
      this.statusStrip1.Size = new System.Drawing.Size(915, 17);
      this.statusStrip1.TabIndex = 3;
      this.statusStrip1.Text = "statusStrip1";
      // 
      // VirtualMs
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
      this.ClientSize = new System.Drawing.Size(915, 664);
      this.Controls.Add(this.splitContainer1);
      this.Controls.Add(this.mainToolStrip);
      this.Controls.Add(this.statusStrip1);
      this.Margin = new System.Windows.Forms.Padding(2);
      this.Name = "VirtualMs";
      this.Text = "Virtual MS";
      this.mainToolStrip.ResumeLayout(false);
      this.mainToolStrip.PerformLayout();
      this.splitContainer1.Panel1.ResumeLayout(false);
      this.splitContainer1.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
      this.splitContainer1.ResumeLayout(false);
      this.splitContainer3.Panel1.ResumeLayout(false);
      this.splitContainer3.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
      this.splitContainer3.ResumeLayout(false);
      this.panel1.ResumeLayout(false);
      this.panel1.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.tbSpeed)).EndInit();
      this.gbStats.ResumeLayout(false);
      this.gbStats.PerformLayout();
      this.plotContainer.Panel1.ResumeLayout(false);
      this.plotContainer.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.plotContainer)).EndInit();
      this.plotContainer.ResumeLayout(false);
      this.tabControl1.ResumeLayout(false);
      this.tabPage1.ResumeLayout(false);
      this.tabPage2.ResumeLayout(false);
      this.tabPage2.PerformLayout();
      this.ResumeLayout(false);

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
