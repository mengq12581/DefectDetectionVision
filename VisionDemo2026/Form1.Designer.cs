namespace VisionDemo2026
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.TableLayoutPanel rootLayout;
        private System.Windows.Forms.TabControl tabImage;
        private System.Windows.Forms.TabPage tabPageAnalysis;
        private System.Windows.Forms.TabPage tabPageMask;
        private System.Windows.Forms.TabPage tabPageEdge;
        private HalconDotNet.HWindowControl hWindowAnalysis;
        private HalconDotNet.HWindowControl hWindowMask;
        private HalconDotNet.HWindowControl hWindowEdge;
        private System.Windows.Forms.TableLayoutPanel rightPanel;
        private System.Windows.Forms.Panel cameraPanel;
        private System.Windows.Forms.Label titleCamera;
        private System.Windows.Forms.Label lblCameraInterface;
        private System.Windows.Forms.ComboBox cmbCameraInterface;
        private System.Windows.Forms.ComboBox cmbCamera;
        private System.Windows.Forms.Label lblExposure;
        private System.Windows.Forms.TextBox txtExposure;
        private Sunny.UI.UIButton btnSearchCamera;
        private Sunny.UI.UIButton btnCaptureSingle;
        private Sunny.UI.UIButton btnStartGrab;
        private Sunny.UI.UIButton btnStopGrab;
        private Sunny.UI.UIButton btnSetExposure;
        private System.Windows.Forms.Panel controlPanel;
        private System.Windows.Forms.Label titleInput;
        private System.Windows.Forms.Label titleAction;
        private System.Windows.Forms.Label titleParameter;
        private Sunny.UI.UIButton btnOpenImage;
        private Sunny.UI.UIButton btnOpenFolder;
        private Sunny.UI.UIButton btnPrevious;
        private Sunny.UI.UIButton btnNext;
        private Sunny.UI.UIButton btnDetect;
        private Sunny.UI.UIButton btnDrawRoi;
        private Sunny.UI.UIButton btnResetRoi;
        private Sunny.UI.UIButton btnExport;
        private System.Windows.Forms.Label lblSource;
        private System.Windows.Forms.Label lblSummary;
        private System.Windows.Forms.Label lblBrightness;
        private System.Windows.Forms.Label lblContrast;
        private System.Windows.Forms.Label lblBlur;
        private System.Windows.Forms.Label lblThresholdMode;
        private System.Windows.Forms.Label lblThreshold;
        private System.Windows.Forms.Label lblAdaptiveBlock;
        private System.Windows.Forms.Label lblMorphOpen;
        private System.Windows.Forms.Label lblMorphClose;
        private System.Windows.Forms.Label lblCannyLow;
        private System.Windows.Forms.Label lblMinArea;
        private System.Windows.Forms.Label lblMaxArea;
        private System.Windows.Forms.Label lblMinSolidity;
        private System.Windows.Forms.Label lblMaxSolidity;
        private System.Windows.Forms.Label lblCircularity;
        private System.Windows.Forms.Label lblMaxAspectRatio;
        private System.Windows.Forms.ComboBox cmbThresholdMode;
        private System.Windows.Forms.CheckBox chkClahe;
        private System.Windows.Forms.NumericUpDown numBrightness;
        private System.Windows.Forms.NumericUpDown numContrast;
        private System.Windows.Forms.NumericUpDown numBlur;
        private System.Windows.Forms.NumericUpDown numThreshold;
        private System.Windows.Forms.NumericUpDown numAdaptiveBlock;
        private System.Windows.Forms.NumericUpDown numMorphOpen;
        private System.Windows.Forms.NumericUpDown numMorphClose;
        private System.Windows.Forms.NumericUpDown numCannyLow;
        private System.Windows.Forms.NumericUpDown numMinArea;
        private System.Windows.Forms.NumericUpDown numMaxArea;
        private System.Windows.Forms.NumericUpDown numMinSolidity;
        private System.Windows.Forms.NumericUpDown numMaxSolidity;
        private System.Windows.Forms.NumericUpDown numCircularity;
        private System.Windows.Forms.NumericUpDown numMaxAspectRatio;

        /// <summary>
        /// 释放窗体设计器创建的托管组件。
        /// </summary>
        /// <param name="disposing">为 true 时释放托管组件；为 false 时仅执行基类释放流程。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码
        /// <summary>
        /// 创建并配置窗体中的全部设计器控件。
        /// </summary>
        private void InitializeComponent()
        {
            this.rootLayout = new System.Windows.Forms.TableLayoutPanel();
            this.tabImage = new System.Windows.Forms.TabControl();
            this.tabPageAnalysis = new System.Windows.Forms.TabPage();
            this.hWindowAnalysis = new HalconDotNet.HWindowControl();
            this.tabPageMask = new System.Windows.Forms.TabPage();
            this.hWindowMask = new HalconDotNet.HWindowControl();
            this.tabPageEdge = new System.Windows.Forms.TabPage();
            this.hWindowEdge = new HalconDotNet.HWindowControl();
            this.rightPanel = new System.Windows.Forms.TableLayoutPanel();
            this.cameraPanel = new System.Windows.Forms.Panel();
            this.titleCamera = new System.Windows.Forms.Label();
            this.lblCameraInterface = new System.Windows.Forms.Label();
            this.cmbCameraInterface = new System.Windows.Forms.ComboBox();
            this.cmbCamera = new System.Windows.Forms.ComboBox();
            this.lblExposure = new System.Windows.Forms.Label();
            this.txtExposure = new System.Windows.Forms.TextBox();
            this.btnSearchCamera = new Sunny.UI.UIButton();
            this.btnCaptureSingle = new Sunny.UI.UIButton();
            this.btnStartGrab = new Sunny.UI.UIButton();
            this.btnStopGrab = new Sunny.UI.UIButton();
            this.btnSetExposure = new Sunny.UI.UIButton();
            this.controlPanel = new System.Windows.Forms.Panel();
            this.titleInput = new System.Windows.Forms.Label();
            this.btnOpenImage = new Sunny.UI.UIButton();
            this.btnOpenFolder = new Sunny.UI.UIButton();
            this.btnPrevious = new Sunny.UI.UIButton();
            this.btnNext = new Sunny.UI.UIButton();
            this.lblSource = new System.Windows.Forms.Label();
            this.titleAction = new System.Windows.Forms.Label();
            this.btnDetect = new Sunny.UI.UIButton();
            this.btnDrawRoi = new Sunny.UI.UIButton();
            this.btnResetRoi = new Sunny.UI.UIButton();
            this.btnExport = new Sunny.UI.UIButton();
            this.lblSummary = new System.Windows.Forms.Label();
            this.titleParameter = new System.Windows.Forms.Label();
            this.lblBrightness = new System.Windows.Forms.Label();
            this.numBrightness = new System.Windows.Forms.NumericUpDown();
            this.lblContrast = new System.Windows.Forms.Label();
            this.numContrast = new System.Windows.Forms.NumericUpDown();
            this.chkClahe = new System.Windows.Forms.CheckBox();
            this.lblBlur = new System.Windows.Forms.Label();
            this.numBlur = new System.Windows.Forms.NumericUpDown();
            this.lblThresholdMode = new System.Windows.Forms.Label();
            this.cmbThresholdMode = new System.Windows.Forms.ComboBox();
            this.lblThreshold = new System.Windows.Forms.Label();
            this.numThreshold = new System.Windows.Forms.NumericUpDown();
            this.lblAdaptiveBlock = new System.Windows.Forms.Label();
            this.numAdaptiveBlock = new System.Windows.Forms.NumericUpDown();
            this.lblMorphOpen = new System.Windows.Forms.Label();
            this.numMorphOpen = new System.Windows.Forms.NumericUpDown();
            this.lblMorphClose = new System.Windows.Forms.Label();
            this.numMorphClose = new System.Windows.Forms.NumericUpDown();
            this.lblCannyLow = new System.Windows.Forms.Label();
            this.numCannyLow = new System.Windows.Forms.NumericUpDown();
            this.lblMinArea = new System.Windows.Forms.Label();
            this.numMinArea = new System.Windows.Forms.NumericUpDown();
            this.lblMaxArea = new System.Windows.Forms.Label();
            this.numMaxArea = new System.Windows.Forms.NumericUpDown();
            this.lblMinSolidity = new System.Windows.Forms.Label();
            this.numMinSolidity = new System.Windows.Forms.NumericUpDown();
            this.lblMaxSolidity = new System.Windows.Forms.Label();
            this.numMaxSolidity = new System.Windows.Forms.NumericUpDown();
            this.lblCircularity = new System.Windows.Forms.Label();
            this.numCircularity = new System.Windows.Forms.NumericUpDown();
            this.lblMaxAspectRatio = new System.Windows.Forms.Label();
            this.numMaxAspectRatio = new System.Windows.Forms.NumericUpDown();
            this.rootLayout.SuspendLayout();
            this.tabImage.SuspendLayout();
            this.tabPageAnalysis.SuspendLayout();
            this.tabPageMask.SuspendLayout();
            this.tabPageEdge.SuspendLayout();
            this.rightPanel.SuspendLayout();
            this.cameraPanel.SuspendLayout();
            this.controlPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numBrightness)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numContrast)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numBlur)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numThreshold)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAdaptiveBlock)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMorphOpen)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMorphClose)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCannyLow)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMinArea)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxArea)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMinSolidity)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxSolidity)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCircularity)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxAspectRatio)).BeginInit();
            this.SuspendLayout();
            // 
            // rootLayout
            // 
            this.rootLayout.ColumnCount = 2;
            this.rootLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.rootLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 380F));
            this.rootLayout.Controls.Add(this.tabImage, 0, 0);
            this.rootLayout.Controls.Add(this.rightPanel, 1, 0);
            this.rootLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rootLayout.Location = new System.Drawing.Point(0, 35);
            this.rootLayout.Name = "rootLayout";
            this.rootLayout.Padding = new System.Windows.Forms.Padding(10);
            this.rootLayout.RowCount = 2;
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.rootLayout.Size = new System.Drawing.Size(1767, 1087);
            this.rootLayout.TabIndex = 0;
            // 
            // tabImage
            // 
            this.tabImage.Controls.Add(this.tabPageAnalysis);
            this.tabImage.Controls.Add(this.tabPageMask);
            this.tabImage.Controls.Add(this.tabPageEdge);
            this.tabImage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabImage.Font = new System.Drawing.Font("Microsoft YaHei UI", 10F);
            this.tabImage.ItemSize = new System.Drawing.Size(120, 28);
            this.tabImage.Location = new System.Drawing.Point(13, 13);
            this.tabImage.Name = "tabImage";
            this.tabImage.SelectedIndex = 0;
            this.tabImage.Size = new System.Drawing.Size(1361, 1041);
            this.tabImage.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.tabImage.TabIndex = 0;
            this.tabImage.SelectedIndexChanged += new System.EventHandler(this.tabImage_SelectedIndexChanged);
            // 
            // tabPageAnalysis
            // 
            this.tabPageAnalysis.Controls.Add(this.hWindowAnalysis);
            this.tabPageAnalysis.Location = new System.Drawing.Point(4, 32);
            this.tabPageAnalysis.Name = "tabPageAnalysis";
            this.tabPageAnalysis.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageAnalysis.Size = new System.Drawing.Size(1353, 1005);
            this.tabPageAnalysis.TabIndex = 0;
            this.tabPageAnalysis.Text = "检测结果";
            this.tabPageAnalysis.UseVisualStyleBackColor = true;
            // 
            // hWindowAnalysis
            // 
            this.hWindowAnalysis.BackColor = System.Drawing.Color.Black;
            this.hWindowAnalysis.BorderColor = System.Drawing.Color.Black;
            this.hWindowAnalysis.Dock = System.Windows.Forms.DockStyle.Fill;
            this.hWindowAnalysis.ImagePart = new System.Drawing.Rectangle(0, 0, 1080, 720);
            this.hWindowAnalysis.Location = new System.Drawing.Point(3, 3);
            this.hWindowAnalysis.Name = "hWindowAnalysis";
            this.hWindowAnalysis.Size = new System.Drawing.Size(1347, 999);
            this.hWindowAnalysis.TabIndex = 0;
            this.hWindowAnalysis.WindowSize = new System.Drawing.Size(1347, 999);
            // 
            // tabPageMask
            // 
            this.tabPageMask.Controls.Add(this.hWindowMask);
            this.tabPageMask.Location = new System.Drawing.Point(4, 32);
            this.tabPageMask.Name = "tabPageMask";
            this.tabPageMask.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageMask.Size = new System.Drawing.Size(1353, 1005);
            this.tabPageMask.TabIndex = 1;
            this.tabPageMask.Text = "二值 Mask";
            this.tabPageMask.UseVisualStyleBackColor = true;
            // 
            // hWindowMask
            // 
            this.hWindowMask.BackColor = System.Drawing.Color.Black;
            this.hWindowMask.BorderColor = System.Drawing.Color.Black;
            this.hWindowMask.Dock = System.Windows.Forms.DockStyle.Fill;
            this.hWindowMask.ImagePart = new System.Drawing.Rectangle(0, 0, 1080, 720);
            this.hWindowMask.Location = new System.Drawing.Point(3, 3);
            this.hWindowMask.Name = "hWindowMask";
            this.hWindowMask.Size = new System.Drawing.Size(1347, 999);
            this.hWindowMask.TabIndex = 0;
            this.hWindowMask.WindowSize = new System.Drawing.Size(1347, 999);
            // 
            // tabPageEdge
            // 
            this.tabPageEdge.Controls.Add(this.hWindowEdge);
            this.tabPageEdge.Location = new System.Drawing.Point(4, 32);
            this.tabPageEdge.Name = "tabPageEdge";
            this.tabPageEdge.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageEdge.Size = new System.Drawing.Size(1353, 1005);
            this.tabPageEdge.TabIndex = 2;
            this.tabPageEdge.Text = "边缘图";
            this.tabPageEdge.UseVisualStyleBackColor = true;
            // 
            // hWindowEdge
            // 
            this.hWindowEdge.BackColor = System.Drawing.Color.Black;
            this.hWindowEdge.BorderColor = System.Drawing.Color.Black;
            this.hWindowEdge.Dock = System.Windows.Forms.DockStyle.Fill;
            this.hWindowEdge.ImagePart = new System.Drawing.Rectangle(0, 0, 1080, 720);
            this.hWindowEdge.Location = new System.Drawing.Point(3, 3);
            this.hWindowEdge.Name = "hWindowEdge";
            this.hWindowEdge.Size = new System.Drawing.Size(1347, 999);
            this.hWindowEdge.TabIndex = 0;
            this.hWindowEdge.WindowSize = new System.Drawing.Size(1347, 999);
            // 
            // rightPanel
            // 
            this.rightPanel.ColumnCount = 1;
            this.rightPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.rightPanel.Controls.Add(this.cameraPanel, 0, 0);
            this.rightPanel.Controls.Add(this.controlPanel, 0, 1);
            this.rightPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rightPanel.Location = new System.Drawing.Point(1380, 13);
            this.rightPanel.Name = "rightPanel";
            this.rightPanel.RowCount = 2;
            this.rightPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 138F));
            this.rightPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.rightPanel.Size = new System.Drawing.Size(374, 1041);
            this.rightPanel.TabIndex = 1;
            // 
            // cameraPanel
            // 
            this.cameraPanel.BackColor = System.Drawing.Color.White;
            this.cameraPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.cameraPanel.Controls.Add(this.titleCamera);
            this.cameraPanel.Controls.Add(this.lblCameraInterface);
            this.cameraPanel.Controls.Add(this.cmbCameraInterface);
            this.cameraPanel.Controls.Add(this.cmbCamera);
            this.cameraPanel.Controls.Add(this.lblExposure);
            this.cameraPanel.Controls.Add(this.txtExposure);
            this.cameraPanel.Controls.Add(this.btnSearchCamera);
            this.cameraPanel.Controls.Add(this.btnCaptureSingle);
            this.cameraPanel.Controls.Add(this.btnStartGrab);
            this.cameraPanel.Controls.Add(this.btnStopGrab);
            this.cameraPanel.Controls.Add(this.btnSetExposure);
            this.cameraPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cameraPanel.Location = new System.Drawing.Point(0, 0);
            this.cameraPanel.Margin = new System.Windows.Forms.Padding(0);
            this.cameraPanel.Name = "cameraPanel";
            this.cameraPanel.Size = new System.Drawing.Size(374, 138);
            this.cameraPanel.TabIndex = 1;
            // 
            // titleCamera
            // 
            this.titleCamera.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(244)))), ((int)(((byte)(250)))));
            this.titleCamera.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.titleCamera.Font = new System.Drawing.Font("Microsoft YaHei UI", 10F, System.Drawing.FontStyle.Bold);
            this.titleCamera.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(90)))), ((int)(((byte)(140)))));
            this.titleCamera.Location = new System.Drawing.Point(7, 4);
            this.titleCamera.Name = "titleCamera";
            this.titleCamera.Size = new System.Drawing.Size(100, 24);
            this.titleCamera.TabIndex = 0;
            this.titleCamera.Text = "图像采集";
            this.titleCamera.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblCameraInterface
            // 
            this.lblCameraInterface.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.lblCameraInterface.Location = new System.Drawing.Point(112, 4);
            this.lblCameraInterface.Name = "lblCameraInterface";
            this.lblCameraInterface.Size = new System.Drawing.Size(35, 24);
            this.lblCameraInterface.TabIndex = 1;
            this.lblCameraInterface.Text = "接口";
            this.lblCameraInterface.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cmbCameraInterface
            // 
            this.cmbCameraInterface.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCameraInterface.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.cmbCameraInterface.FormattingEnabled = true;
            this.cmbCameraInterface.Items.AddRange(new object[] {
            "笔记本相机（MediaFoundation）",
            "网络相机（GigEVision2）"});
            this.cmbCameraInterface.Location = new System.Drawing.Point(145, 3);
            this.cmbCameraInterface.Name = "cmbCameraInterface";
            this.cmbCameraInterface.Size = new System.Drawing.Size(220, 32);
            this.cmbCameraInterface.TabIndex = 2;
            this.cmbCameraInterface.SelectedIndexChanged += new System.EventHandler(this.cmbCameraInterface_SelectedIndexChanged);
            // 
            // cmbCamera
            // 
            this.cmbCamera.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCamera.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.cmbCamera.FormattingEnabled = true;
            this.cmbCamera.Location = new System.Drawing.Point(7, 32);
            this.cmbCamera.Name = "cmbCamera";
            this.cmbCamera.Size = new System.Drawing.Size(220, 32);
            this.cmbCamera.TabIndex = 3;
            this.cmbCamera.SelectedIndexChanged += new System.EventHandler(this.cmbCamera_SelectedIndexChanged);
            // 
            // lblExposure
            // 
            this.lblExposure.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.lblExposure.Location = new System.Drawing.Point(7, 66);
            this.lblExposure.Name = "lblExposure";
            this.lblExposure.Size = new System.Drawing.Size(68, 25);
            this.lblExposure.TabIndex = 5;
            this.lblExposure.Text = "曝光值";
            this.lblExposure.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtExposure
            // 
            this.txtExposure.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.txtExposure.Location = new System.Drawing.Point(73, 64);
            this.txtExposure.Name = "txtExposure";
            this.txtExposure.Size = new System.Drawing.Size(132, 30);
            this.txtExposure.TabIndex = 6;
            this.txtExposure.Text = "5000";
            // 
            // btnSearchCamera
            // 
            this.btnSearchCamera.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSearchCamera.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.btnSearchCamera.Location = new System.Drawing.Point(235, 32);
            this.btnSearchCamera.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnSearchCamera.Name = "btnSearchCamera";
            this.btnSearchCamera.Size = new System.Drawing.Size(130, 28);
            this.btnSearchCamera.TabIndex = 4;
            this.btnSearchCamera.Text = "搜索相机";
            this.btnSearchCamera.TipsFont = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnSearchCamera.Click += new System.EventHandler(this.btnSearchCamera_Click);
            // 
            // btnCaptureSingle
            // 
            this.btnCaptureSingle.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCaptureSingle.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.btnCaptureSingle.Location = new System.Drawing.Point(7, 99);
            this.btnCaptureSingle.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnCaptureSingle.Name = "btnCaptureSingle";
            this.btnCaptureSingle.Size = new System.Drawing.Size(110, 30);
            this.btnCaptureSingle.TabIndex = 8;
            this.btnCaptureSingle.Text = "单次采集";
            this.btnCaptureSingle.TipsFont = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnCaptureSingle.Click += new System.EventHandler(this.btnCaptureSingle_Click);
            // 
            // btnStartGrab
            // 
            this.btnStartGrab.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnStartGrab.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.btnStartGrab.Location = new System.Drawing.Point(123, 99);
            this.btnStartGrab.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnStartGrab.Name = "btnStartGrab";
            this.btnStartGrab.Size = new System.Drawing.Size(110, 30);
            this.btnStartGrab.TabIndex = 9;
            this.btnStartGrab.Text = "连续采集";
            this.btnStartGrab.TipsFont = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnStartGrab.Click += new System.EventHandler(this.btnStartGrab_Click);
            // 
            // btnStopGrab
            // 
            this.btnStopGrab.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnStopGrab.Enabled = false;
            this.btnStopGrab.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.btnStopGrab.Location = new System.Drawing.Point(239, 99);
            this.btnStopGrab.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnStopGrab.Name = "btnStopGrab";
            this.btnStopGrab.Size = new System.Drawing.Size(118, 30);
            this.btnStopGrab.TabIndex = 10;
            this.btnStopGrab.Text = "停止采集";
            this.btnStopGrab.TipsFont = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnStopGrab.Click += new System.EventHandler(this.btnStopGrab_Click);
            // 
            // btnSetExposure
            // 
            this.btnSetExposure.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSetExposure.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.btnSetExposure.Location = new System.Drawing.Point(213, 64);
            this.btnSetExposure.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnSetExposure.Name = "btnSetExposure";
            this.btnSetExposure.Size = new System.Drawing.Size(144, 28);
            this.btnSetExposure.TabIndex = 7;
            this.btnSetExposure.Text = "设置曝光";
            this.btnSetExposure.TipsFont = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnSetExposure.Click += new System.EventHandler(this.btnSetExposure_Click);
            // 
            // controlPanel
            // 
            this.controlPanel.AutoScroll = true;
            this.controlPanel.BackColor = System.Drawing.Color.White;
            this.controlPanel.Controls.Add(this.titleInput);
            this.controlPanel.Controls.Add(this.btnOpenImage);
            this.controlPanel.Controls.Add(this.btnOpenFolder);
            this.controlPanel.Controls.Add(this.btnPrevious);
            this.controlPanel.Controls.Add(this.btnNext);
            this.controlPanel.Controls.Add(this.lblSource);
            this.controlPanel.Controls.Add(this.titleAction);
            this.controlPanel.Controls.Add(this.btnDetect);
            this.controlPanel.Controls.Add(this.btnDrawRoi);
            this.controlPanel.Controls.Add(this.btnResetRoi);
            this.controlPanel.Controls.Add(this.btnExport);
            this.controlPanel.Controls.Add(this.lblSummary);
            this.controlPanel.Controls.Add(this.titleParameter);
            this.controlPanel.Controls.Add(this.lblBrightness);
            this.controlPanel.Controls.Add(this.numBrightness);
            this.controlPanel.Controls.Add(this.lblContrast);
            this.controlPanel.Controls.Add(this.numContrast);
            this.controlPanel.Controls.Add(this.chkClahe);
            this.controlPanel.Controls.Add(this.lblBlur);
            this.controlPanel.Controls.Add(this.numBlur);
            this.controlPanel.Controls.Add(this.lblThresholdMode);
            this.controlPanel.Controls.Add(this.cmbThresholdMode);
            this.controlPanel.Controls.Add(this.lblThreshold);
            this.controlPanel.Controls.Add(this.numThreshold);
            this.controlPanel.Controls.Add(this.lblAdaptiveBlock);
            this.controlPanel.Controls.Add(this.numAdaptiveBlock);
            this.controlPanel.Controls.Add(this.lblMorphOpen);
            this.controlPanel.Controls.Add(this.numMorphOpen);
            this.controlPanel.Controls.Add(this.lblMorphClose);
            this.controlPanel.Controls.Add(this.numMorphClose);
            this.controlPanel.Controls.Add(this.lblCannyLow);
            this.controlPanel.Controls.Add(this.numCannyLow);
            this.controlPanel.Controls.Add(this.lblMinArea);
            this.controlPanel.Controls.Add(this.numMinArea);
            this.controlPanel.Controls.Add(this.lblMaxArea);
            this.controlPanel.Controls.Add(this.numMaxArea);
            this.controlPanel.Controls.Add(this.lblMinSolidity);
            this.controlPanel.Controls.Add(this.numMinSolidity);
            this.controlPanel.Controls.Add(this.lblMaxSolidity);
            this.controlPanel.Controls.Add(this.numMaxSolidity);
            this.controlPanel.Controls.Add(this.lblCircularity);
            this.controlPanel.Controls.Add(this.numCircularity);
            this.controlPanel.Controls.Add(this.lblMaxAspectRatio);
            this.controlPanel.Controls.Add(this.numMaxAspectRatio);
            this.controlPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.controlPanel.Location = new System.Drawing.Point(0, 138);
            this.controlPanel.Margin = new System.Windows.Forms.Padding(0);
            this.controlPanel.Name = "controlPanel";
            this.controlPanel.Padding = new System.Windows.Forms.Padding(8);
            this.controlPanel.Size = new System.Drawing.Size(374, 903);
            this.controlPanel.TabIndex = 1;
            // 
            // titleInput
            // 
            this.titleInput.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(244)))), ((int)(((byte)(250)))));
            this.titleInput.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.titleInput.Font = new System.Drawing.Font("Microsoft YaHei UI", 10F, System.Drawing.FontStyle.Bold);
            this.titleInput.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(90)))), ((int)(((byte)(140)))));
            this.titleInput.Location = new System.Drawing.Point(8, 8);
            this.titleInput.Name = "titleInput";
            this.titleInput.Size = new System.Drawing.Size(358, 26);
            this.titleInput.TabIndex = 0;
            this.titleInput.Text = "图像输入";
            this.titleInput.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnOpenImage
            // 
            this.btnOpenImage.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnOpenImage.Font = new System.Drawing.Font("Microsoft YaHei UI", 10F);
            this.btnOpenImage.Location = new System.Drawing.Point(8, 39);
            this.btnOpenImage.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnOpenImage.Name = "btnOpenImage";
            this.btnOpenImage.Size = new System.Drawing.Size(170, 32);
            this.btnOpenImage.TabIndex = 1;
            this.btnOpenImage.Text = "打开图片";
            this.btnOpenImage.TipsFont = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnOpenImage.Click += new System.EventHandler(this.btnOpenImage_Click);
            // 
            // btnOpenFolder
            // 
            this.btnOpenFolder.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnOpenFolder.Font = new System.Drawing.Font("Microsoft YaHei UI", 10F);
            this.btnOpenFolder.Location = new System.Drawing.Point(188, 39);
            this.btnOpenFolder.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnOpenFolder.Name = "btnOpenFolder";
            this.btnOpenFolder.Size = new System.Drawing.Size(170, 32);
            this.btnOpenFolder.TabIndex = 2;
            this.btnOpenFolder.Text = "打开图片文件夹";
            this.btnOpenFolder.TipsFont = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnOpenFolder.Click += new System.EventHandler(this.btnOpenFolder_Click);
            // 
            // btnPrevious
            // 
            this.btnPrevious.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnPrevious.Enabled = false;
            this.btnPrevious.Font = new System.Drawing.Font("Microsoft YaHei UI", 10F);
            this.btnPrevious.Location = new System.Drawing.Point(8, 76);
            this.btnPrevious.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnPrevious.Name = "btnPrevious";
            this.btnPrevious.Size = new System.Drawing.Size(170, 32);
            this.btnPrevious.TabIndex = 3;
            this.btnPrevious.Text = "上一张";
            this.btnPrevious.TipsFont = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnPrevious.Click += new System.EventHandler(this.btnPrevious_Click);
            // 
            // btnNext
            // 
            this.btnNext.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnNext.Enabled = false;
            this.btnNext.Font = new System.Drawing.Font("Microsoft YaHei UI", 10F);
            this.btnNext.Location = new System.Drawing.Point(188, 76);
            this.btnNext.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(170, 32);
            this.btnNext.TabIndex = 4;
            this.btnNext.Text = "下一张";
            this.btnNext.TipsFont = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // lblSource
            // 
            this.lblSource.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.lblSource.ForeColor = System.Drawing.Color.DimGray;
            this.lblSource.Location = new System.Drawing.Point(8, 111);
            this.lblSource.Name = "lblSource";
            this.lblSource.Size = new System.Drawing.Size(358, 30);
            this.lblSource.TabIndex = 5;
            this.lblSource.Text = "当前未加载图片";
            this.lblSource.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // titleAction
            // 
            this.titleAction.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(244)))), ((int)(((byte)(250)))));
            this.titleAction.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.titleAction.Font = new System.Drawing.Font("Microsoft YaHei UI", 10F, System.Drawing.FontStyle.Bold);
            this.titleAction.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(90)))), ((int)(((byte)(140)))));
            this.titleAction.Location = new System.Drawing.Point(8, 142);
            this.titleAction.Name = "titleAction";
            this.titleAction.Size = new System.Drawing.Size(358, 26);
            this.titleAction.TabIndex = 6;
            this.titleAction.Text = "检测操作";
            this.titleAction.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnDetect
            // 
            this.btnDetect.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDetect.Font = new System.Drawing.Font("Microsoft YaHei UI", 10F);
            this.btnDetect.Location = new System.Drawing.Point(8, 173);
            this.btnDetect.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnDetect.Name = "btnDetect";
            this.btnDetect.Size = new System.Drawing.Size(170, 32);
            this.btnDetect.TabIndex = 7;
            this.btnDetect.Text = "检测当前图片";
            this.btnDetect.TipsFont = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnDetect.Click += new System.EventHandler(this.btnDetect_Click);
            // 
            // btnDrawRoi
            // 
            this.btnDrawRoi.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDrawRoi.Font = new System.Drawing.Font("Microsoft YaHei UI", 10F);
            this.btnDrawRoi.Location = new System.Drawing.Point(188, 173);
            this.btnDrawRoi.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnDrawRoi.Name = "btnDrawRoi";
            this.btnDrawRoi.Size = new System.Drawing.Size(170, 32);
            this.btnDrawRoi.TabIndex = 8;
            this.btnDrawRoi.Text = "绘制 ROI";
            this.btnDrawRoi.TipsFont = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnDrawRoi.Click += new System.EventHandler(this.btnDrawRoi_Click);
            // 
            // btnResetRoi
            // 
            this.btnResetRoi.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnResetRoi.Font = new System.Drawing.Font("Microsoft YaHei UI", 10F);
            this.btnResetRoi.Location = new System.Drawing.Point(8, 209);
            this.btnResetRoi.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnResetRoi.Name = "btnResetRoi";
            this.btnResetRoi.Size = new System.Drawing.Size(170, 32);
            this.btnResetRoi.TabIndex = 9;
            this.btnResetRoi.Text = "重置 ROI";
            this.btnResetRoi.TipsFont = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnResetRoi.Click += new System.EventHandler(this.btnResetRoi_Click);
            // 
            // btnExport
            // 
            this.btnExport.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnExport.Font = new System.Drawing.Font("Microsoft YaHei UI", 10F);
            this.btnExport.Location = new System.Drawing.Point(188, 209);
            this.btnExport.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(170, 32);
            this.btnExport.TabIndex = 10;
            this.btnExport.Text = "导出检测报告";
            this.btnExport.TipsFont = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // lblSummary
            // 
            this.lblSummary.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.lblSummary.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(90)))), ((int)(((byte)(70)))));
            this.lblSummary.Location = new System.Drawing.Point(8, 241);
            this.lblSummary.Name = "lblSummary";
            this.lblSummary.Size = new System.Drawing.Size(358, 28);
            this.lblSummary.TabIndex = 11;
            this.lblSummary.Text = "合格：0    缺陷：0    覆盖率：0.00%";
            this.lblSummary.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // titleParameter
            // 
            this.titleParameter.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(244)))), ((int)(((byte)(250)))));
            this.titleParameter.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.titleParameter.Font = new System.Drawing.Font("Microsoft YaHei UI", 10F, System.Drawing.FontStyle.Bold);
            this.titleParameter.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(90)))), ((int)(((byte)(140)))));
            this.titleParameter.Location = new System.Drawing.Point(8, 275);
            this.titleParameter.Name = "titleParameter";
            this.titleParameter.Size = new System.Drawing.Size(358, 26);
            this.titleParameter.TabIndex = 12;
            this.titleParameter.Text = "检测参数";
            this.titleParameter.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblBrightness
            // 
            this.lblBrightness.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.lblBrightness.Location = new System.Drawing.Point(12, 305);
            this.lblBrightness.Name = "lblBrightness";
            this.lblBrightness.Size = new System.Drawing.Size(170, 25);
            this.lblBrightness.TabIndex = 13;
            this.lblBrightness.Text = "亮度";
            // 
            // numBrightness
            // 
            this.numBrightness.Location = new System.Drawing.Point(190, 305);
            this.numBrightness.Name = "numBrightness";
            this.numBrightness.Size = new System.Drawing.Size(168, 35);
            this.numBrightness.TabIndex = 14;
            this.numBrightness.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.numBrightness.ValueChanged += new System.EventHandler(this.parameter_ValueChanged);
            // 
            // lblContrast
            // 
            this.lblContrast.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.lblContrast.Location = new System.Drawing.Point(12, 346);
            this.lblContrast.Name = "lblContrast";
            this.lblContrast.Size = new System.Drawing.Size(170, 25);
            this.lblContrast.TabIndex = 15;
            this.lblContrast.Text = "对比度";
            // 
            // numContrast
            // 
            this.numContrast.Location = new System.Drawing.Point(190, 346);
            this.numContrast.Maximum = new decimal(new int[] {
            120,
            0,
            0,
            0});
            this.numContrast.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numContrast.Name = "numContrast";
            this.numContrast.Size = new System.Drawing.Size(168, 35);
            this.numContrast.TabIndex = 16;
            this.numContrast.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.numContrast.ValueChanged += new System.EventHandler(this.parameter_ValueChanged);
            // 
            // chkClahe
            // 
            this.chkClahe.AutoSize = true;
            this.chkClahe.Checked = true;
            this.chkClahe.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkClahe.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.chkClahe.Location = new System.Drawing.Point(12, 387);
            this.chkClahe.Name = "chkClahe";
            this.chkClahe.Size = new System.Drawing.Size(198, 28);
            this.chkClahe.TabIndex = 17;
            this.chkClahe.Text = "启用局部对比度增强";
            this.chkClahe.CheckedChanged += new System.EventHandler(this.parameter_ValueChanged);
            // 
            // lblBlur
            // 
            this.lblBlur.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.lblBlur.Location = new System.Drawing.Point(12, 421);
            this.lblBlur.Name = "lblBlur";
            this.lblBlur.Size = new System.Drawing.Size(170, 25);
            this.lblBlur.TabIndex = 18;
            this.lblBlur.Text = "高斯滤波等级";
            // 
            // numBlur
            // 
            this.numBlur.Location = new System.Drawing.Point(190, 421);
            this.numBlur.Maximum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.numBlur.Name = "numBlur";
            this.numBlur.Size = new System.Drawing.Size(168, 35);
            this.numBlur.TabIndex = 19;
            this.numBlur.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numBlur.ValueChanged += new System.EventHandler(this.parameter_ValueChanged);
            // 
            // lblThresholdMode
            // 
            this.lblThresholdMode.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.lblThresholdMode.Location = new System.Drawing.Point(12, 458);
            this.lblThresholdMode.Name = "lblThresholdMode";
            this.lblThresholdMode.Size = new System.Drawing.Size(170, 25);
            this.lblThresholdMode.TabIndex = 20;
            this.lblThresholdMode.Text = "阈值模式";
            // 
            // cmbThresholdMode
            // 
            this.cmbThresholdMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbThresholdMode.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.cmbThresholdMode.FormattingEnabled = true;
            this.cmbThresholdMode.Items.AddRange(new object[] {
            "手动阈值",
            "Otsu 自动阈值",
            "自适应阈值"});
            this.cmbThresholdMode.Location = new System.Drawing.Point(190, 458);
            this.cmbThresholdMode.Name = "cmbThresholdMode";
            this.cmbThresholdMode.Size = new System.Drawing.Size(168, 32);
            this.cmbThresholdMode.TabIndex = 21;
            this.cmbThresholdMode.SelectedIndexChanged += new System.EventHandler(this.parameter_ValueChanged);
            // 
            // lblThreshold
            // 
            this.lblThreshold.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.lblThreshold.Location = new System.Drawing.Point(12, 493);
            this.lblThreshold.Name = "lblThreshold";
            this.lblThreshold.Size = new System.Drawing.Size(170, 25);
            this.lblThreshold.TabIndex = 22;
            this.lblThreshold.Text = "手动阈值";
            // 
            // numThreshold
            // 
            this.numThreshold.Location = new System.Drawing.Point(190, 493);
            this.numThreshold.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numThreshold.Name = "numThreshold";
            this.numThreshold.Size = new System.Drawing.Size(168, 35);
            this.numThreshold.TabIndex = 23;
            this.numThreshold.Value = new decimal(new int[] {
            115,
            0,
            0,
            0});
            this.numThreshold.ValueChanged += new System.EventHandler(this.parameter_ValueChanged);
            // 
            // lblAdaptiveBlock
            // 
            this.lblAdaptiveBlock.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.lblAdaptiveBlock.Location = new System.Drawing.Point(12, 530);
            this.lblAdaptiveBlock.Name = "lblAdaptiveBlock";
            this.lblAdaptiveBlock.Size = new System.Drawing.Size(170, 25);
            this.lblAdaptiveBlock.TabIndex = 24;
            this.lblAdaptiveBlock.Text = "自适应块等级";
            // 
            // numAdaptiveBlock
            // 
            this.numAdaptiveBlock.Location = new System.Drawing.Point(190, 530);
            this.numAdaptiveBlock.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.numAdaptiveBlock.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numAdaptiveBlock.Name = "numAdaptiveBlock";
            this.numAdaptiveBlock.Size = new System.Drawing.Size(168, 35);
            this.numAdaptiveBlock.TabIndex = 25;
            this.numAdaptiveBlock.Value = new decimal(new int[] {
            7,
            0,
            0,
            0});
            this.numAdaptiveBlock.ValueChanged += new System.EventHandler(this.parameter_ValueChanged);
            // 
            // lblMorphOpen
            // 
            this.lblMorphOpen.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.lblMorphOpen.Location = new System.Drawing.Point(12, 567);
            this.lblMorphOpen.Name = "lblMorphOpen";
            this.lblMorphOpen.Size = new System.Drawing.Size(170, 25);
            this.lblMorphOpen.TabIndex = 26;
            this.lblMorphOpen.Text = "开运算半径";
            // 
            // numMorphOpen
            // 
            this.numMorphOpen.Location = new System.Drawing.Point(190, 567);
            this.numMorphOpen.Maximum = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this.numMorphOpen.Name = "numMorphOpen";
            this.numMorphOpen.Size = new System.Drawing.Size(168, 35);
            this.numMorphOpen.TabIndex = 27;
            this.numMorphOpen.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numMorphOpen.ValueChanged += new System.EventHandler(this.parameter_ValueChanged);
            // 
            // lblMorphClose
            // 
            this.lblMorphClose.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.lblMorphClose.Location = new System.Drawing.Point(12, 606);
            this.lblMorphClose.Name = "lblMorphClose";
            this.lblMorphClose.Size = new System.Drawing.Size(170, 25);
            this.lblMorphClose.TabIndex = 28;
            this.lblMorphClose.Text = "闭运算半径";
            // 
            // numMorphClose
            // 
            this.numMorphClose.Location = new System.Drawing.Point(190, 606);
            this.numMorphClose.Maximum = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this.numMorphClose.Name = "numMorphClose";
            this.numMorphClose.Size = new System.Drawing.Size(168, 35);
            this.numMorphClose.TabIndex = 29;
            this.numMorphClose.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.numMorphClose.ValueChanged += new System.EventHandler(this.parameter_ValueChanged);
            // 
            // lblCannyLow
            // 
            this.lblCannyLow.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.lblCannyLow.Location = new System.Drawing.Point(12, 644);
            this.lblCannyLow.Name = "lblCannyLow";
            this.lblCannyLow.Size = new System.Drawing.Size(170, 25);
            this.lblCannyLow.TabIndex = 30;
            this.lblCannyLow.Text = "Canny 低阈值";
            // 
            // numCannyLow
            // 
            this.numCannyLow.Location = new System.Drawing.Point(190, 644);
            this.numCannyLow.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numCannyLow.Name = "numCannyLow";
            this.numCannyLow.Size = new System.Drawing.Size(168, 35);
            this.numCannyLow.TabIndex = 31;
            this.numCannyLow.Value = new decimal(new int[] {
            70,
            0,
            0,
            0});
            this.numCannyLow.ValueChanged += new System.EventHandler(this.parameter_ValueChanged);
            // 
            // lblMinArea
            // 
            this.lblMinArea.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.lblMinArea.Location = new System.Drawing.Point(12, 681);
            this.lblMinArea.Name = "lblMinArea";
            this.lblMinArea.Size = new System.Drawing.Size(170, 25);
            this.lblMinArea.TabIndex = 32;
            this.lblMinArea.Text = "合格最小面积";
            // 
            // numMinArea
            // 
            this.numMinArea.Location = new System.Drawing.Point(190, 681);
            this.numMinArea.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numMinArea.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numMinArea.Name = "numMinArea";
            this.numMinArea.Size = new System.Drawing.Size(168, 35);
            this.numMinArea.TabIndex = 33;
            this.numMinArea.Value = new decimal(new int[] {
            4000,
            0,
            0,
            0});
            this.numMinArea.ValueChanged += new System.EventHandler(this.parameter_ValueChanged);
            // 
            // lblMaxArea
            // 
            this.lblMaxArea.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.lblMaxArea.Location = new System.Drawing.Point(12, 718);
            this.lblMaxArea.Name = "lblMaxArea";
            this.lblMaxArea.Size = new System.Drawing.Size(170, 25);
            this.lblMaxArea.TabIndex = 34;
            this.lblMaxArea.Text = "合格最大面积";
            // 
            // numMaxArea
            // 
            this.numMaxArea.Location = new System.Drawing.Point(190, 718);
            this.numMaxArea.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numMaxArea.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numMaxArea.Name = "numMaxArea";
            this.numMaxArea.Size = new System.Drawing.Size(168, 35);
            this.numMaxArea.TabIndex = 35;
            this.numMaxArea.Value = new decimal(new int[] {
            5500,
            0,
            0,
            0});
            this.numMaxArea.ValueChanged += new System.EventHandler(this.parameter_ValueChanged);
            // 
            // lblMinSolidity
            // 
            this.lblMinSolidity.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.lblMinSolidity.Location = new System.Drawing.Point(12, 756);
            this.lblMinSolidity.Name = "lblMinSolidity";
            this.lblMinSolidity.Size = new System.Drawing.Size(170, 25);
            this.lblMinSolidity.TabIndex = 36;
            this.lblMinSolidity.Text = "最小实心度 (%)";
            // 
            // numMinSolidity
            // 
            this.numMinSolidity.Location = new System.Drawing.Point(190, 756);
            this.numMinSolidity.Name = "numMinSolidity";
            this.numMinSolidity.Size = new System.Drawing.Size(168, 35);
            this.numMinSolidity.TabIndex = 37;
            this.numMinSolidity.Value = new decimal(new int[] {
            80,
            0,
            0,
            0});
            this.numMinSolidity.ValueChanged += new System.EventHandler(this.parameter_ValueChanged);
            // 
            // lblMaxSolidity
            // 
            this.lblMaxSolidity.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.lblMaxSolidity.Location = new System.Drawing.Point(12, 793);
            this.lblMaxSolidity.Name = "lblMaxSolidity";
            this.lblMaxSolidity.Size = new System.Drawing.Size(170, 25);
            this.lblMaxSolidity.TabIndex = 38;
            this.lblMaxSolidity.Text = "最大实心度 (%)";
            // 
            // numMaxSolidity
            // 
            this.numMaxSolidity.Location = new System.Drawing.Point(190, 793);
            this.numMaxSolidity.Name = "numMaxSolidity";
            this.numMaxSolidity.Size = new System.Drawing.Size(168, 35);
            this.numMaxSolidity.TabIndex = 39;
            this.numMaxSolidity.Value = new decimal(new int[] {
            90,
            0,
            0,
            0});
            this.numMaxSolidity.ValueChanged += new System.EventHandler(this.parameter_ValueChanged);
            // 
            // lblCircularity
            // 
            this.lblCircularity.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.lblCircularity.Location = new System.Drawing.Point(12, 830);
            this.lblCircularity.Name = "lblCircularity";
            this.lblCircularity.Size = new System.Drawing.Size(170, 25);
            this.lblCircularity.TabIndex = 40;
            this.lblCircularity.Text = "圆度下限 (%)";
            // 
            // numCircularity
            // 
            this.numCircularity.Location = new System.Drawing.Point(190, 830);
            this.numCircularity.Name = "numCircularity";
            this.numCircularity.Size = new System.Drawing.Size(168, 35);
            this.numCircularity.TabIndex = 41;
            this.numCircularity.Value = new decimal(new int[] {
            80,
            0,
            0,
            0});
            this.numCircularity.ValueChanged += new System.EventHandler(this.parameter_ValueChanged);
            // 
            // lblMaxAspectRatio
            // 
            this.lblMaxAspectRatio.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.lblMaxAspectRatio.Location = new System.Drawing.Point(12, 867);
            this.lblMaxAspectRatio.Name = "lblMaxAspectRatio";
            this.lblMaxAspectRatio.Size = new System.Drawing.Size(170, 25);
            this.lblMaxAspectRatio.TabIndex = 42;
            this.lblMaxAspectRatio.Text = "最大长宽比 (%)";
            // 
            // numMaxAspectRatio
            // 
            this.numMaxAspectRatio.Location = new System.Drawing.Point(190, 867);
            this.numMaxAspectRatio.Maximum = new decimal(new int[] {
            300,
            0,
            0,
            0});
            this.numMaxAspectRatio.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numMaxAspectRatio.Name = "numMaxAspectRatio";
            this.numMaxAspectRatio.Size = new System.Drawing.Size(168, 35);
            this.numMaxAspectRatio.TabIndex = 43;
            this.numMaxAspectRatio.Value = new decimal(new int[] {
            135,
            0,
            0,
            0});
            this.numMaxAspectRatio.ValueChanged += new System.EventHandler(this.parameter_ValueChanged);
            // 
            // Form1
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1767, 1122);
            this.Controls.Add(this.rootLayout);
            this.MinimumSize = new System.Drawing.Size(1050, 650);
            this.Name = "Form1";
            this.Text = "VisionDemo2026 - HALCON 零件缺陷检测";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.ZoomScaleRect = new System.Drawing.Rectangle(22, 22, 1258, 698);
            this.rootLayout.ResumeLayout(false);
            this.tabImage.ResumeLayout(false);
            this.tabPageAnalysis.ResumeLayout(false);
            this.tabPageMask.ResumeLayout(false);
            this.tabPageEdge.ResumeLayout(false);
            this.rightPanel.ResumeLayout(false);
            this.cameraPanel.ResumeLayout(false);
            this.cameraPanel.PerformLayout();
            this.controlPanel.ResumeLayout(false);
            this.controlPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numBrightness)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numContrast)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numBlur)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numThreshold)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAdaptiveBlock)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMorphOpen)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMorphClose)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCannyLow)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMinArea)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxArea)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMinSolidity)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxSolidity)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCircularity)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxAspectRatio)).EndInit();
            this.ResumeLayout(false);

        }
        #endregion
    }
}
