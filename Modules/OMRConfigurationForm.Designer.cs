namespace Synapse.Modules
{
    partial class OMRConfigurationForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OMRConfigurationForm));
            this.imageBox = new Cyotek.Windows.Forms.Demo.ImageBoxEx();
            this.imageBoxPanel = new System.Windows.Forms.Panel();
            this.statePanelsPanel = new System.Windows.Forms.Panel();
            this.IntegerValueStatePanel = new System.Windows.Forms.Panel();
            this.IntegerStatePanelFlowLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.integerStateLabel = new Syncfusion.Windows.Forms.Tools.AutoLabel();
            this.integerStateControlsTablePanel = new System.Windows.Forms.TableLayoutPanel();
            this.integerStateComboBox = new Syncfusion.Windows.Forms.Tools.ComboBoxAdv();
            this.integerStateValueTextBox = new Syncfusion.Windows.Forms.Tools.IntegerTextBox();
            this.DoubleValueStatePanel = new System.Windows.Forms.Panel();
            this.doubleStageTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.doubleStateLabel = new Syncfusion.Windows.Forms.Tools.AutoLabel();
            this.doubleStageControlsPanel = new System.Windows.Forms.TableLayoutPanel();
            this.doubleStateComboBox = new Syncfusion.Windows.Forms.Tools.ComboBoxAdv();
            this.doubleStateValueTextBox = new Syncfusion.Windows.Forms.Tools.DoubleTextBox();
            this.ComboBoxStatePanel = new System.Windows.Forms.Panel();
            this.interSpaceTypeComboBox = new Syncfusion.Windows.Forms.Tools.ComboBoxAdv();
            this.LabelStatePanel = new System.Windows.Forms.Panel();
            this.walkthroughDescriptionLabel = new Syncfusion.Windows.Forms.Tools.AutoLabel();
            this.orientationStatePanel = new System.Windows.Forms.Panel();
            this.orientationComboBox = new Syncfusion.Windows.Forms.Tools.ComboBoxAdv();
            this.imageBoxControlsToolStrip = new System.Windows.Forms.ToolStrip();
            this.showImageRegionToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.showSourceImageRegionToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.actualSizeToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.zoomInToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.zoomOutToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.selectAllToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.selectNoneToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.selectionToolStripStatusLabel = new System.Windows.Forms.ToolStripLabel();
            this.initializationButtonsPanel = new System.Windows.Forms.TableLayoutPanel();
            this.setBtn = new Syncfusion.Windows.Forms.ButtonAdv();
            this.nextBtn = new Syncfusion.Windows.Forms.ButtonAdv();
            this.walkthroughPanel = new System.Windows.Forms.Panel();
            this.walkthroughStatusPanel = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.walkthroughIndexLabel = new Syncfusion.Windows.Forms.Tools.AutoLabel();
            this.imageBoxPanel.SuspendLayout();
            this.statePanelsPanel.SuspendLayout();
            this.IntegerValueStatePanel.SuspendLayout();
            this.IntegerStatePanelFlowLayoutPanel.SuspendLayout();
            this.integerStateControlsTablePanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.integerStateComboBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.integerStateValueTextBox)).BeginInit();
            this.DoubleValueStatePanel.SuspendLayout();
            this.doubleStageTableLayoutPanel.SuspendLayout();
            this.doubleStageControlsPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.doubleStateComboBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.doubleStateValueTextBox)).BeginInit();
            this.ComboBoxStatePanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.interSpaceTypeComboBox)).BeginInit();
            this.LabelStatePanel.SuspendLayout();
            this.orientationStatePanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.orientationComboBox)).BeginInit();
            this.imageBoxControlsToolStrip.SuspendLayout();
            this.initializationButtonsPanel.SuspendLayout();
            this.walkthroughPanel.SuspendLayout();
            this.walkthroughStatusPanel.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // imageBox
            // 
            this.imageBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.imageBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.imageBox.DragHandleSize = 10;
            this.imageBox.DropShadowSize = 5;
            this.imageBox.Font = new System.Drawing.Font("Dosis", 14F);
            this.imageBox.GridColor = System.Drawing.Color.White;
            this.imageBox.ImageBorderColor = System.Drawing.SystemColors.ActiveBorder;
            this.imageBox.ImageBorderStyle = Cyotek.Windows.Forms.ImageBoxBorderStyle.FixedSingleGlowShadow;
            this.imageBox.Location = new System.Drawing.Point(0, 24);
            this.imageBox.Margin = new System.Windows.Forms.Padding(0);
            this.imageBox.Name = "imageBox";
            this.imageBox.SelectionMode = Cyotek.Windows.Forms.ImageBoxSelectionMode.Rectangle;
            this.imageBox.Size = new System.Drawing.Size(423, 314);
            this.imageBox.StepSize = new System.Drawing.Size(8, 8);
            this.imageBox.TabIndex = 2;
            this.imageBox.Text = "Configuration Region";
            this.imageBox.Selected += new System.EventHandler<System.EventArgs>(this.imageBox_Selected);
            this.imageBox.SelectionRegionChanged += new System.EventHandler(this.imageBox_SelectionRegionChanged);
            this.imageBox.Scroll += new System.Windows.Forms.ScrollEventHandler(this.imageBox_Scroll);
            this.imageBox.Paint += new System.Windows.Forms.PaintEventHandler(this.imageBox_Paint);
            this.imageBox.Resize += new System.EventHandler(this.imageBox_Resize);
            // 
            // imageBoxPanel
            // 
            this.imageBoxPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.imageBoxPanel.Controls.Add(this.statePanelsPanel);
            this.imageBoxPanel.Controls.Add(this.imageBoxControlsToolStrip);
            this.imageBoxPanel.Controls.Add(this.imageBox);
            this.imageBoxPanel.Location = new System.Drawing.Point(10, 8);
            this.imageBoxPanel.Name = "imageBoxPanel";
            this.imageBoxPanel.Size = new System.Drawing.Size(423, 338);
            this.imageBoxPanel.TabIndex = 5;
            // 
            // statePanelsPanel
            // 
            this.statePanelsPanel.Controls.Add(this.IntegerValueStatePanel);
            this.statePanelsPanel.Controls.Add(this.DoubleValueStatePanel);
            this.statePanelsPanel.Controls.Add(this.ComboBoxStatePanel);
            this.statePanelsPanel.Controls.Add(this.LabelStatePanel);
            this.statePanelsPanel.Controls.Add(this.orientationStatePanel);
            this.statePanelsPanel.Location = new System.Drawing.Point(0, 60);
            this.statePanelsPanel.Margin = new System.Windows.Forms.Padding(0);
            this.statePanelsPanel.Name = "statePanelsPanel";
            this.statePanelsPanel.Size = new System.Drawing.Size(423, 278);
            this.statePanelsPanel.TabIndex = 8;
            this.statePanelsPanel.Visible = false;
            // 
            // IntegerValueStatePanel
            // 
            this.IntegerValueStatePanel.Controls.Add(this.IntegerStatePanelFlowLayoutPanel);
            this.IntegerValueStatePanel.Location = new System.Drawing.Point(26, 95);
            this.IntegerValueStatePanel.Margin = new System.Windows.Forms.Padding(0);
            this.IntegerValueStatePanel.Name = "IntegerValueStatePanel";
            this.IntegerValueStatePanel.Size = new System.Drawing.Size(375, 39);
            this.IntegerValueStatePanel.TabIndex = 10;
            this.IntegerValueStatePanel.Visible = false;
            // 
            // IntegerStatePanelFlowLayoutPanel
            // 
            this.IntegerStatePanelFlowLayoutPanel.ColumnCount = 2;
            this.IntegerStatePanelFlowLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60F));
            this.IntegerStatePanelFlowLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.IntegerStatePanelFlowLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.IntegerStatePanelFlowLayoutPanel.Controls.Add(this.integerStateLabel, 0, 0);
            this.IntegerStatePanelFlowLayoutPanel.Controls.Add(this.integerStateControlsTablePanel, 1, 0);
            this.IntegerStatePanelFlowLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.IntegerStatePanelFlowLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.IntegerStatePanelFlowLayoutPanel.Name = "IntegerStatePanelFlowLayoutPanel";
            this.IntegerStatePanelFlowLayoutPanel.RowCount = 1;
            this.IntegerStatePanelFlowLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.IntegerStatePanelFlowLayoutPanel.Size = new System.Drawing.Size(375, 39);
            this.IntegerStatePanelFlowLayoutPanel.TabIndex = 11;
            // 
            // integerStateLabel
            // 
            this.integerStateLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.integerStateLabel.AutoSize = false;
            this.integerStateLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.integerStateLabel.Font = new System.Drawing.Font("Dosis", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.integerStateLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.integerStateLabel.Location = new System.Drawing.Point(3, 3);
            this.integerStateLabel.Margin = new System.Windows.Forms.Padding(3);
            this.integerStateLabel.Name = "integerStateLabel";
            this.integerStateLabel.Size = new System.Drawing.Size(219, 33);
            this.integerStateLabel.Style = Syncfusion.Windows.Forms.Tools.AutoLabelStyle.Office2016White;
            this.integerStateLabel.TabIndex = 10;
            this.integerStateLabel.Text = "Integer Value:";
            this.integerStateLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.integerStateLabel.ThemeName = "Office2016White";
            // 
            // integerStateControlsTablePanel
            // 
            this.integerStateControlsTablePanel.ColumnCount = 2;
            this.integerStateControlsTablePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.integerStateControlsTablePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.integerStateControlsTablePanel.Controls.Add(this.integerStateComboBox, 0, 0);
            this.integerStateControlsTablePanel.Controls.Add(this.integerStateValueTextBox, 1, 0);
            this.integerStateControlsTablePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.integerStateControlsTablePanel.Location = new System.Drawing.Point(225, 0);
            this.integerStateControlsTablePanel.Margin = new System.Windows.Forms.Padding(0);
            this.integerStateControlsTablePanel.Name = "integerStateControlsTablePanel";
            this.integerStateControlsTablePanel.RowCount = 1;
            this.integerStateControlsTablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.integerStateControlsTablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.integerStateControlsTablePanel.Size = new System.Drawing.Size(150, 39);
            this.integerStateControlsTablePanel.TabIndex = 11;
            // 
            // integerStateComboBox
            // 
            this.integerStateComboBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.integerStateComboBox.BeforeTouchSize = new System.Drawing.Size(69, 44);
            this.integerStateComboBox.Border3DStyle = System.Windows.Forms.Border3DStyle.Flat;
            this.integerStateComboBox.BorderSides = System.Windows.Forms.Border3DSide.Bottom;
            this.integerStateComboBox.CanOverrideStyle = true;
            this.integerStateComboBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.integerStateComboBox.FlatBorderColor = System.Drawing.Color.White;
            this.integerStateComboBox.FlatStyle = Syncfusion.Windows.Forms.Tools.ComboFlatStyle.Flat;
            this.integerStateComboBox.Font = new System.Drawing.Font("Dosis", 20.75F);
            this.integerStateComboBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.integerStateComboBox.Location = new System.Drawing.Point(3, 3);
            this.integerStateComboBox.MetroBorderColor = System.Drawing.Color.White;
            this.integerStateComboBox.Name = "integerStateComboBox";
            this.integerStateComboBox.Size = new System.Drawing.Size(69, 44);
            this.integerStateComboBox.Style = Syncfusion.Windows.Forms.VisualStyle.Office2016White;
            this.integerStateComboBox.TabIndex = 13;
            this.integerStateComboBox.Text = "ComboBox";
            this.integerStateComboBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.integerStateComboBox.ThemeName = "Office2016White";
            this.integerStateComboBox.SelectedIndexChanged += new System.EventHandler(this.IntegerStateComboBox_SelectedIndexChanged);
            // 
            // integerStateValueTextBox
            // 
            this.integerStateValueTextBox.BackGroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.integerStateValueTextBox.BeforeTouchSize = new System.Drawing.Size(69, 42);
            this.integerStateValueTextBox.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(171)))), ((int)(((byte)(171)))), ((int)(((byte)(171)))));
            this.integerStateValueTextBox.BorderSides = ((System.Windows.Forms.Border3DSide)((System.Windows.Forms.Border3DSide.Left | System.Windows.Forms.Border3DSide.Right)));
            this.integerStateValueTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.integerStateValueTextBox.DeleteSelectionOnNegative = true;
            this.integerStateValueTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.integerStateValueTextBox.Font = new System.Drawing.Font("Dosis", 20.75F);
            this.integerStateValueTextBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.integerStateValueTextBox.IntegerValue = ((long)(1));
            this.integerStateValueTextBox.Location = new System.Drawing.Point(78, 3);
            this.integerStateValueTextBox.Name = "integerStateValueTextBox";
            this.integerStateValueTextBox.PositiveColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.integerStateValueTextBox.Size = new System.Drawing.Size(69, 42);
            this.integerStateValueTextBox.Style = Syncfusion.Windows.Forms.Tools.TextBoxExt.theme.Office2016White;
            this.integerStateValueTextBox.TabIndex = 14;
            this.integerStateValueTextBox.Text = "1";
            this.integerStateValueTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.integerStateValueTextBox.ThemeName = "Office2016White";
            this.integerStateValueTextBox.ZeroColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            // 
            // DoubleValueStatePanel
            // 
            this.DoubleValueStatePanel.Controls.Add(this.doubleStageTableLayoutPanel);
            this.DoubleValueStatePanel.Location = new System.Drawing.Point(23, 194);
            this.DoubleValueStatePanel.Margin = new System.Windows.Forms.Padding(0);
            this.DoubleValueStatePanel.Name = "DoubleValueStatePanel";
            this.DoubleValueStatePanel.Size = new System.Drawing.Size(375, 44);
            this.DoubleValueStatePanel.TabIndex = 12;
            // 
            // doubleStageTableLayoutPanel
            // 
            this.doubleStageTableLayoutPanel.ColumnCount = 2;
            this.doubleStageTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60F));
            this.doubleStageTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.doubleStageTableLayoutPanel.Controls.Add(this.doubleStateLabel, 0, 0);
            this.doubleStageTableLayoutPanel.Controls.Add(this.doubleStageControlsPanel, 1, 0);
            this.doubleStageTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.doubleStageTableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.doubleStageTableLayoutPanel.Name = "doubleStageTableLayoutPanel";
            this.doubleStageTableLayoutPanel.RowCount = 1;
            this.doubleStageTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.doubleStageTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.doubleStageTableLayoutPanel.Size = new System.Drawing.Size(375, 44);
            this.doubleStageTableLayoutPanel.TabIndex = 0;
            // 
            // doubleStateLabel
            // 
            this.doubleStateLabel.AutoSize = false;
            this.doubleStateLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.doubleStateLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.doubleStateLabel.Font = new System.Drawing.Font("Dosis", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.doubleStateLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.doubleStateLabel.Location = new System.Drawing.Point(3, 3);
            this.doubleStateLabel.Margin = new System.Windows.Forms.Padding(3);
            this.doubleStateLabel.Name = "doubleStateLabel";
            this.doubleStateLabel.Size = new System.Drawing.Size(219, 38);
            this.doubleStateLabel.Style = Syncfusion.Windows.Forms.Tools.AutoLabelStyle.Office2016White;
            this.doubleStateLabel.TabIndex = 13;
            this.doubleStateLabel.Text = "Double Value:";
            this.doubleStateLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.doubleStateLabel.ThemeName = "Office2016White";
            // 
            // doubleStageControlsPanel
            // 
            this.doubleStageControlsPanel.ColumnCount = 2;
            this.doubleStageControlsPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.doubleStageControlsPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.doubleStageControlsPanel.Controls.Add(this.doubleStateComboBox, 0, 0);
            this.doubleStageControlsPanel.Controls.Add(this.doubleStateValueTextBox, 1, 0);
            this.doubleStageControlsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.doubleStageControlsPanel.Location = new System.Drawing.Point(225, 0);
            this.doubleStageControlsPanel.Margin = new System.Windows.Forms.Padding(0);
            this.doubleStageControlsPanel.Name = "doubleStageControlsPanel";
            this.doubleStageControlsPanel.RowCount = 1;
            this.doubleStageControlsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.doubleStageControlsPanel.Size = new System.Drawing.Size(150, 44);
            this.doubleStageControlsPanel.TabIndex = 14;
            // 
            // doubleStateComboBox
            // 
            this.doubleStateComboBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.doubleStateComboBox.BeforeTouchSize = new System.Drawing.Size(69, 44);
            this.doubleStateComboBox.Border3DStyle = System.Windows.Forms.Border3DStyle.Flat;
            this.doubleStateComboBox.BorderSides = System.Windows.Forms.Border3DSide.Bottom;
            this.doubleStateComboBox.CanOverrideStyle = true;
            this.doubleStateComboBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.doubleStateComboBox.FlatBorderColor = System.Drawing.Color.White;
            this.doubleStateComboBox.FlatStyle = Syncfusion.Windows.Forms.Tools.ComboFlatStyle.Flat;
            this.doubleStateComboBox.Font = new System.Drawing.Font("Dosis", 20.75F);
            this.doubleStateComboBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.doubleStateComboBox.Location = new System.Drawing.Point(3, 3);
            this.doubleStateComboBox.MetroBorderColor = System.Drawing.Color.White;
            this.doubleStateComboBox.Name = "doubleStateComboBox";
            this.doubleStateComboBox.Size = new System.Drawing.Size(69, 44);
            this.doubleStateComboBox.Style = Syncfusion.Windows.Forms.VisualStyle.Office2016White;
            this.doubleStateComboBox.TabIndex = 14;
            this.doubleStateComboBox.Text = "ComboBox";
            this.doubleStateComboBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.doubleStateComboBox.ThemeName = "Office2016White";
            this.doubleStateComboBox.SelectedIndexChanged += new System.EventHandler(this.DoubleStateComboBox_SelectedIndexChanged);
            // 
            // doubleStateValueTextBox
            // 
            this.doubleStateValueTextBox.BackGroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.doubleStateValueTextBox.BeforeTouchSize = new System.Drawing.Size(69, 42);
            this.doubleStateValueTextBox.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(171)))), ((int)(((byte)(171)))), ((int)(((byte)(171)))));
            this.doubleStateValueTextBox.BorderSides = ((System.Windows.Forms.Border3DSide)((System.Windows.Forms.Border3DSide.Left | System.Windows.Forms.Border3DSide.Right)));
            this.doubleStateValueTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.doubleStateValueTextBox.DeleteSelectionOnNegative = true;
            this.doubleStateValueTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.doubleStateValueTextBox.DoubleValue = 1D;
            this.doubleStateValueTextBox.Font = new System.Drawing.Font("Dosis", 20.75F);
            this.doubleStateValueTextBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.doubleStateValueTextBox.Location = new System.Drawing.Point(78, 3);
            this.doubleStateValueTextBox.Name = "doubleStateValueTextBox";
            this.doubleStateValueTextBox.PositiveColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.doubleStateValueTextBox.Size = new System.Drawing.Size(69, 42);
            this.doubleStateValueTextBox.Style = Syncfusion.Windows.Forms.Tools.TextBoxExt.theme.Office2016White;
            this.doubleStateValueTextBox.TabIndex = 13;
            this.doubleStateValueTextBox.Text = "1.00";
            this.doubleStateValueTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.doubleStateValueTextBox.ThemeName = "Office2016White";
            this.doubleStateValueTextBox.ZeroColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            // 
            // ComboBoxStatePanel
            // 
            this.ComboBoxStatePanel.Controls.Add(this.interSpaceTypeComboBox);
            this.ComboBoxStatePanel.Location = new System.Drawing.Point(23, 143);
            this.ComboBoxStatePanel.Margin = new System.Windows.Forms.Padding(0);
            this.ComboBoxStatePanel.Name = "ComboBoxStatePanel";
            this.ComboBoxStatePanel.Size = new System.Drawing.Size(375, 31);
            this.ComboBoxStatePanel.TabIndex = 11;
            this.ComboBoxStatePanel.Visible = false;
            // 
            // interSpaceTypeComboBox
            // 
            this.interSpaceTypeComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.interSpaceTypeComboBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.interSpaceTypeComboBox.BeforeTouchSize = new System.Drawing.Size(372, 44);
            this.interSpaceTypeComboBox.Border3DStyle = System.Windows.Forms.Border3DStyle.Flat;
            this.interSpaceTypeComboBox.BorderSides = System.Windows.Forms.Border3DSide.Bottom;
            this.interSpaceTypeComboBox.FlatBorderColor = System.Drawing.Color.White;
            this.interSpaceTypeComboBox.FlatStyle = Syncfusion.Windows.Forms.Tools.ComboFlatStyle.Flat;
            this.interSpaceTypeComboBox.Font = new System.Drawing.Font("Dosis", 20.75F);
            this.interSpaceTypeComboBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.interSpaceTypeComboBox.Location = new System.Drawing.Point(0, 4);
            this.interSpaceTypeComboBox.MetroBorderColor = System.Drawing.Color.White;
            this.interSpaceTypeComboBox.Name = "interSpaceTypeComboBox";
            this.interSpaceTypeComboBox.Size = new System.Drawing.Size(372, 44);
            this.interSpaceTypeComboBox.Style = Syncfusion.Windows.Forms.VisualStyle.Office2016White;
            this.interSpaceTypeComboBox.TabIndex = 0;
            this.interSpaceTypeComboBox.Text = "Inter Space Type";
            this.interSpaceTypeComboBox.ThemeName = "Office2016White";
            // 
            // LabelStatePanel
            // 
            this.LabelStatePanel.Controls.Add(this.walkthroughDescriptionLabel);
            this.LabelStatePanel.Location = new System.Drawing.Point(26, 51);
            this.LabelStatePanel.Margin = new System.Windows.Forms.Padding(0);
            this.LabelStatePanel.Name = "LabelStatePanel";
            this.LabelStatePanel.Size = new System.Drawing.Size(375, 41);
            this.LabelStatePanel.TabIndex = 9;
            this.LabelStatePanel.Visible = false;
            // 
            // walkthroughDescriptionLabel
            // 
            this.walkthroughDescriptionLabel.AutoSize = false;
            this.walkthroughDescriptionLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.walkthroughDescriptionLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.walkthroughDescriptionLabel.Font = new System.Drawing.Font("Dosis", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.walkthroughDescriptionLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.walkthroughDescriptionLabel.Location = new System.Drawing.Point(0, 0);
            this.walkthroughDescriptionLabel.Margin = new System.Windows.Forms.Padding(3);
            this.walkthroughDescriptionLabel.Name = "walkthroughDescriptionLabel";
            this.walkthroughDescriptionLabel.Size = new System.Drawing.Size(375, 41);
            this.walkthroughDescriptionLabel.Style = Syncfusion.Windows.Forms.Tools.AutoLabelStyle.Office2016White;
            this.walkthroughDescriptionLabel.TabIndex = 9;
            this.walkthroughDescriptionLabel.Text = "Configuration Walkthrough";
            this.walkthroughDescriptionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.walkthroughDescriptionLabel.ThemeName = "Office2016White";
            // 
            // orientationStatePanel
            // 
            this.orientationStatePanel.Controls.Add(this.orientationComboBox);
            this.orientationStatePanel.Location = new System.Drawing.Point(26, 0);
            this.orientationStatePanel.Margin = new System.Windows.Forms.Padding(0);
            this.orientationStatePanel.Name = "orientationStatePanel";
            this.orientationStatePanel.Size = new System.Drawing.Size(375, 32);
            this.orientationStatePanel.TabIndex = 13;
            this.orientationStatePanel.Visible = false;
            // 
            // orientationComboBox
            // 
            this.orientationComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.orientationComboBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.orientationComboBox.BeforeTouchSize = new System.Drawing.Size(375, 44);
            this.orientationComboBox.Border3DStyle = System.Windows.Forms.Border3DStyle.Flat;
            this.orientationComboBox.BorderSides = System.Windows.Forms.Border3DSide.Bottom;
            this.orientationComboBox.FlatBorderColor = System.Drawing.Color.White;
            this.orientationComboBox.FlatStyle = Syncfusion.Windows.Forms.Tools.ComboFlatStyle.Flat;
            this.orientationComboBox.Font = new System.Drawing.Font("Dosis", 20.75F);
            this.orientationComboBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.orientationComboBox.Location = new System.Drawing.Point(0, 4);
            this.orientationComboBox.MetroBorderColor = System.Drawing.Color.White;
            this.orientationComboBox.Name = "orientationComboBox";
            this.orientationComboBox.Size = new System.Drawing.Size(375, 44);
            this.orientationComboBox.Style = Syncfusion.Windows.Forms.VisualStyle.Office2016White;
            this.orientationComboBox.TabIndex = 0;
            this.orientationComboBox.Text = "Select Orientation";
            this.orientationComboBox.ThemeName = "Office2016White";
            // 
            // imageBoxControlsToolStrip
            // 
            this.imageBoxControlsToolStrip.BackColor = System.Drawing.Color.Transparent;
            this.imageBoxControlsToolStrip.CanOverflow = false;
            this.imageBoxControlsToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.imageBoxControlsToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showImageRegionToolStripButton,
            this.showSourceImageRegionToolStripButton,
            this.toolStripSeparator1,
            this.actualSizeToolStripButton,
            this.zoomInToolStripButton,
            this.zoomOutToolStripButton,
            this.toolStripSeparator2,
            this.selectAllToolStripButton,
            this.selectNoneToolStripButton,
            this.toolStripSeparator3,
            this.selectionToolStripStatusLabel});
            this.imageBoxControlsToolStrip.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.imageBoxControlsToolStrip.Location = new System.Drawing.Point(0, 0);
            this.imageBoxControlsToolStrip.Name = "imageBoxControlsToolStrip";
            this.imageBoxControlsToolStrip.Padding = new System.Windows.Forms.Padding(0);
            this.imageBoxControlsToolStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.imageBoxControlsToolStrip.Size = new System.Drawing.Size(423, 25);
            this.imageBoxControlsToolStrip.TabIndex = 4;
            // 
            // showImageRegionToolStripButton
            // 
            this.showImageRegionToolStripButton.CheckOnClick = true;
            this.showImageRegionToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.showImageRegionToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("showImageRegionToolStripButton.Image")));
            this.showImageRegionToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.showImageRegionToolStripButton.Name = "showImageRegionToolStripButton";
            this.showImageRegionToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.showImageRegionToolStripButton.Text = "Show Image Region";
            this.showImageRegionToolStripButton.Click += new System.EventHandler(this.showImageRegionToolStripButton_Click);
            // 
            // showSourceImageRegionToolStripButton
            // 
            this.showSourceImageRegionToolStripButton.CheckOnClick = true;
            this.showSourceImageRegionToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.showSourceImageRegionToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("showSourceImageRegionToolStripButton.Image")));
            this.showSourceImageRegionToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.showSourceImageRegionToolStripButton.Name = "showSourceImageRegionToolStripButton";
            this.showSourceImageRegionToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.showSourceImageRegionToolStripButton.Text = "Show Source Image Region";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // actualSizeToolStripButton
            // 
            this.actualSizeToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.actualSizeToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("actualSizeToolStripButton.Image")));
            this.actualSizeToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.actualSizeToolStripButton.Name = "actualSizeToolStripButton";
            this.actualSizeToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.actualSizeToolStripButton.Text = "Actual Size";
            this.actualSizeToolStripButton.Click += new System.EventHandler(this.actualSizeToolStripButton_Click);
            // 
            // zoomInToolStripButton
            // 
            this.zoomInToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.zoomInToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("zoomInToolStripButton.Image")));
            this.zoomInToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.zoomInToolStripButton.Name = "zoomInToolStripButton";
            this.zoomInToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.zoomInToolStripButton.Text = "Zoom In";
            this.zoomInToolStripButton.Click += new System.EventHandler(this.zoomInToolStripButton_Click);
            // 
            // zoomOutToolStripButton
            // 
            this.zoomOutToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.zoomOutToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("zoomOutToolStripButton.Image")));
            this.zoomOutToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.zoomOutToolStripButton.Name = "zoomOutToolStripButton";
            this.zoomOutToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.zoomOutToolStripButton.Text = "Zoom Out";
            this.zoomOutToolStripButton.Click += new System.EventHandler(this.zoomOutToolStripButton_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // selectAllToolStripButton
            // 
            this.selectAllToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.selectAllToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("selectAllToolStripButton.Image")));
            this.selectAllToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.selectAllToolStripButton.Name = "selectAllToolStripButton";
            this.selectAllToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.selectAllToolStripButton.Text = "Select All";
            this.selectAllToolStripButton.Click += new System.EventHandler(this.selectAllToolStripButton_Click);
            // 
            // selectNoneToolStripButton
            // 
            this.selectNoneToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.selectNoneToolStripButton.Image = global::Synapse.Properties.Resources.selection;
            this.selectNoneToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.selectNoneToolStripButton.Name = "selectNoneToolStripButton";
            this.selectNoneToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.selectNoneToolStripButton.Text = "Select None";
            this.selectNoneToolStripButton.Click += new System.EventHandler(this.selectNoneToolStripButton_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // selectionToolStripStatusLabel
            // 
            this.selectionToolStripStatusLabel.Font = new System.Drawing.Font("Dosis", 11F);
            this.selectionToolStripStatusLabel.Name = "selectionToolStripStatusLabel";
            this.selectionToolStripStatusLabel.Size = new System.Drawing.Size(0, 22);
            // 
            // initializationButtonsPanel
            // 
            this.initializationButtonsPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.initializationButtonsPanel.ColumnCount = 2;
            this.initializationButtonsPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.initializationButtonsPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.initializationButtonsPanel.Controls.Add(this.setBtn, 0, 0);
            this.initializationButtonsPanel.Controls.Add(this.nextBtn, 1, 0);
            this.initializationButtonsPanel.Location = new System.Drawing.Point(0, 55);
            this.initializationButtonsPanel.Margin = new System.Windows.Forms.Padding(0);
            this.initializationButtonsPanel.Name = "initializationButtonsPanel";
            this.initializationButtonsPanel.RowCount = 1;
            this.initializationButtonsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.initializationButtonsPanel.Size = new System.Drawing.Size(423, 55);
            this.initializationButtonsPanel.TabIndex = 7;
            // 
            // setBtn
            // 
            this.setBtn.Appearance = Syncfusion.Windows.Forms.ButtonAppearance.Metro;
            this.setBtn.BackColor = System.Drawing.Color.DodgerBlue;
            this.setBtn.BeforeTouchSize = new System.Drawing.Size(205, 49);
            this.setBtn.BorderStyleAdv = Syncfusion.Windows.Forms.ButtonAdvBorderStyle.Flat;
            this.setBtn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.setBtn.FlatAppearance.BorderSize = 0;
            this.setBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.setBtn.Font = new System.Drawing.Font("Dosis", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.setBtn.ForeColor = System.Drawing.Color.White;
            this.setBtn.Image = global::Synapse.Properties.Resources.Check;
            this.setBtn.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.setBtn.Location = new System.Drawing.Point(3, 3);
            this.setBtn.Name = "setBtn";
            this.setBtn.Padding = new System.Windows.Forms.Padding(10, 0, 10, 0);
            this.setBtn.Size = new System.Drawing.Size(205, 49);
            this.setBtn.TabIndex = 37;
            this.setBtn.Text = "   SET";
            this.setBtn.ThemeName = "Metro";
            this.setBtn.ThemeStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.setBtn.ThemeStyle.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(171)))), ((int)(((byte)(171)))), ((int)(((byte)(171)))));
            this.setBtn.ThemeStyle.DisabledBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(243)))), ((int)(((byte)(243)))));
            this.setBtn.ThemeStyle.DisabledBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
            this.setBtn.ThemeStyle.DisabledForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(171)))), ((int)(((byte)(171)))), ((int)(((byte)(171)))));
            this.setBtn.ThemeStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.setBtn.ThemeStyle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.setBtn.ThemeStyle.HoverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(236)))), ((int)(((byte)(249)))));
            this.setBtn.ThemeStyle.HoverBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(113)))), ((int)(((byte)(175)))), ((int)(((byte)(229)))));
            this.setBtn.ThemeStyle.HoverForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.setBtn.ThemeStyle.PressedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(205)))), ((int)(((byte)(230)))), ((int)(((byte)(247)))));
            this.setBtn.ThemeStyle.PressedBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(212)))));
            this.setBtn.ThemeStyle.PressedForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.setBtn.UseVisualStyle = false;
            this.setBtn.UseVisualStyleBackColor = false;
            this.setBtn.Click += new System.EventHandler(this.SetBtn_Click);
            // 
            // nextBtn
            // 
            this.nextBtn.Appearance = Syncfusion.Windows.Forms.ButtonAppearance.Metro;
            this.nextBtn.BackColor = System.Drawing.Color.LightSlateGray;
            this.nextBtn.BeforeTouchSize = new System.Drawing.Size(206, 49);
            this.nextBtn.BorderStyleAdv = Syncfusion.Windows.Forms.ButtonAdvBorderStyle.Flat;
            this.nextBtn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.nextBtn.FlatAppearance.BorderSize = 0;
            this.nextBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.nextBtn.Font = new System.Drawing.Font("Dosis", 20F);
            this.nextBtn.ForeColor = System.Drawing.Color.White;
            this.nextBtn.Image = global::Synapse.Properties.Resources.Out;
            this.nextBtn.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.nextBtn.Location = new System.Drawing.Point(214, 3);
            this.nextBtn.Name = "nextBtn";
            this.nextBtn.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.nextBtn.Size = new System.Drawing.Size(206, 49);
            this.nextBtn.TabIndex = 38;
            this.nextBtn.Text = "   NEXT";
            this.nextBtn.ThemeName = "Metro";
            this.nextBtn.ThemeStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.nextBtn.ThemeStyle.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(171)))), ((int)(((byte)(171)))), ((int)(((byte)(171)))));
            this.nextBtn.ThemeStyle.DisabledBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(243)))), ((int)(((byte)(243)))));
            this.nextBtn.ThemeStyle.DisabledBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
            this.nextBtn.ThemeStyle.DisabledForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(171)))), ((int)(((byte)(171)))), ((int)(((byte)(171)))));
            this.nextBtn.ThemeStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.nextBtn.ThemeStyle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.nextBtn.ThemeStyle.HoverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(236)))), ((int)(((byte)(249)))));
            this.nextBtn.ThemeStyle.HoverBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(113)))), ((int)(((byte)(175)))), ((int)(((byte)(229)))));
            this.nextBtn.ThemeStyle.HoverForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.nextBtn.ThemeStyle.PressedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(205)))), ((int)(((byte)(230)))), ((int)(((byte)(247)))));
            this.nextBtn.ThemeStyle.PressedBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(212)))));
            this.nextBtn.ThemeStyle.PressedForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.nextBtn.UseVisualStyle = false;
            this.nextBtn.UseVisualStyleBackColor = false;
            this.nextBtn.Click += new System.EventHandler(this.NextBtn_Click);
            // 
            // walkthroughPanel
            // 
            this.walkthroughPanel.Controls.Add(this.walkthroughStatusPanel);
            this.walkthroughPanel.Controls.Add(this.initializationButtonsPanel);
            this.walkthroughPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.walkthroughPanel.Location = new System.Drawing.Point(10, 349);
            this.walkthroughPanel.Name = "walkthroughPanel";
            this.walkthroughPanel.Size = new System.Drawing.Size(423, 110);
            this.walkthroughPanel.TabIndex = 39;
            // 
            // walkthroughStatusPanel
            // 
            this.walkthroughStatusPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.walkthroughStatusPanel.ColumnCount = 2;
            this.walkthroughStatusPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.walkthroughStatusPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 90F));
            this.walkthroughStatusPanel.Controls.Add(this.panel1, 0, 0);
            this.walkthroughStatusPanel.Location = new System.Drawing.Point(0, 0);
            this.walkthroughStatusPanel.Name = "walkthroughStatusPanel";
            this.walkthroughStatusPanel.RowCount = 1;
            this.walkthroughStatusPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.walkthroughStatusPanel.Size = new System.Drawing.Size(423, 52);
            this.walkthroughStatusPanel.TabIndex = 8;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.walkthroughIndexLabel);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(42, 52);
            this.panel1.TabIndex = 9;
            // 
            // walkthroughIndexLabel
            // 
            this.walkthroughIndexLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.walkthroughIndexLabel.AutoSize = false;
            this.walkthroughIndexLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.walkthroughIndexLabel.Font = new System.Drawing.Font("Dosis", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.walkthroughIndexLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.walkthroughIndexLabel.Location = new System.Drawing.Point(0, 0);
            this.walkthroughIndexLabel.Margin = new System.Windows.Forms.Padding(0);
            this.walkthroughIndexLabel.Name = "walkthroughIndexLabel";
            this.walkthroughIndexLabel.Size = new System.Drawing.Size(42, 52);
            this.walkthroughIndexLabel.Style = Syncfusion.Windows.Forms.Tools.AutoLabelStyle.Office2016White;
            this.walkthroughIndexLabel.TabIndex = 8;
            this.walkthroughIndexLabel.Text = "1.";
            this.walkthroughIndexLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.walkthroughIndexLabel.ThemeName = "Office2016White";
            // 
            // OMRConfigurationForm
            // 
            this.ClientSize = new System.Drawing.Size(443, 464);
            this.Controls.Add(this.walkthroughPanel);
            this.Controls.Add(this.imageBoxPanel);
            this.Font = new System.Drawing.Font("Dosis", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MinimizeBox = false;
            this.Name = "OMRConfigurationForm";
            this.Padding = new System.Windows.Forms.Padding(10, 5, 10, 5);
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.ShowToolTip = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Style.InactiveShadowOpacity = ((byte)(50));
            this.Style.ShadowOpacity = ((byte)(0));
            this.Style.TitleBar.BackColor = System.Drawing.Color.White;
            this.Style.TitleBar.Font = new System.Drawing.Font("Dosis", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Style.TitleBar.TextHorizontalAlignment = System.Windows.Forms.HorizontalAlignment.Center;
            this.Text = "OMR Configuration Tool";
            this.imageBoxPanel.ResumeLayout(false);
            this.imageBoxPanel.PerformLayout();
            this.statePanelsPanel.ResumeLayout(false);
            this.IntegerValueStatePanel.ResumeLayout(false);
            this.IntegerStatePanelFlowLayoutPanel.ResumeLayout(false);
            this.integerStateControlsTablePanel.ResumeLayout(false);
            this.integerStateControlsTablePanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.integerStateComboBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.integerStateValueTextBox)).EndInit();
            this.DoubleValueStatePanel.ResumeLayout(false);
            this.doubleStageTableLayoutPanel.ResumeLayout(false);
            this.doubleStageControlsPanel.ResumeLayout(false);
            this.doubleStageControlsPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.doubleStateComboBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.doubleStateValueTextBox)).EndInit();
            this.ComboBoxStatePanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.interSpaceTypeComboBox)).EndInit();
            this.LabelStatePanel.ResumeLayout(false);
            this.orientationStatePanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.orientationComboBox)).EndInit();
            this.imageBoxControlsToolStrip.ResumeLayout(false);
            this.imageBoxControlsToolStrip.PerformLayout();
            this.initializationButtonsPanel.ResumeLayout(false);
            this.walkthroughPanel.ResumeLayout(false);
            this.walkthroughStatusPanel.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Cyotek.Windows.Forms.Demo.ImageBoxEx imageBox;
        private System.Windows.Forms.Panel imageBoxPanel;
        private System.Windows.Forms.ToolStrip imageBoxControlsToolStrip;
        private System.Windows.Forms.ToolStripButton showImageRegionToolStripButton;
        private System.Windows.Forms.ToolStripButton showSourceImageRegionToolStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton actualSizeToolStripButton;
        private System.Windows.Forms.ToolStripButton zoomInToolStripButton;
        private System.Windows.Forms.ToolStripButton zoomOutToolStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton selectAllToolStripButton;
        private System.Windows.Forms.ToolStripButton selectNoneToolStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripLabel selectionToolStripStatusLabel;
        private System.Windows.Forms.TableLayoutPanel initializationButtonsPanel;
        private Syncfusion.Windows.Forms.ButtonAdv setBtn;
        private Syncfusion.Windows.Forms.ButtonAdv nextBtn;
        private System.Windows.Forms.Panel walkthroughPanel;
        private System.Windows.Forms.TableLayoutPanel walkthroughStatusPanel;
        private System.Windows.Forms.Panel statePanelsPanel;
        private System.Windows.Forms.Panel LabelStatePanel;
        private Syncfusion.Windows.Forms.Tools.AutoLabel walkthroughDescriptionLabel;
        private System.Windows.Forms.Panel IntegerValueStatePanel;
        private System.Windows.Forms.Panel ComboBoxStatePanel;
        private Syncfusion.Windows.Forms.Tools.ComboBoxAdv interSpaceTypeComboBox;
        private System.Windows.Forms.Panel DoubleValueStatePanel;
        private Syncfusion.Windows.Forms.Tools.AutoLabel integerStateLabel;
        private System.Windows.Forms.Panel panel1;
        private Syncfusion.Windows.Forms.Tools.AutoLabel walkthroughIndexLabel;
        private System.Windows.Forms.Panel orientationStatePanel;
        private Syncfusion.Windows.Forms.Tools.ComboBoxAdv orientationComboBox;
        private System.Windows.Forms.TableLayoutPanel IntegerStatePanelFlowLayoutPanel;
        private System.Windows.Forms.TableLayoutPanel integerStateControlsTablePanel;
        private Syncfusion.Windows.Forms.Tools.ComboBoxAdv integerStateComboBox;
        private Syncfusion.Windows.Forms.Tools.IntegerTextBox integerStateValueTextBox;
        private System.Windows.Forms.TableLayoutPanel doubleStageTableLayoutPanel;
        private Syncfusion.Windows.Forms.Tools.AutoLabel doubleStateLabel;
        private System.Windows.Forms.TableLayoutPanel doubleStageControlsPanel;
        private Syncfusion.Windows.Forms.Tools.ComboBoxAdv doubleStateComboBox;
        private Syncfusion.Windows.Forms.Tools.DoubleTextBox doubleStateValueTextBox;
    }
}