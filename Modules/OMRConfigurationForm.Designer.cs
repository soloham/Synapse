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
            this.walkthorughStatusPanel = new System.Windows.Forms.TableLayoutPanel();
            this.statePanelsPanel = new System.Windows.Forms.Panel();
            this.totalObjectsStatePanel = new System.Windows.Forms.Panel();
            this.totalObjectsLabel = new Syncfusion.Windows.Forms.Tools.AutoLabel();
            this.totalObjectsTextBox = new Syncfusion.Windows.Forms.Tools.IntegerTextBox();
            this.interSpaceTypeStatePanel = new System.Windows.Forms.Panel();
            this.interSpaceTypeComboBox = new Syncfusion.Windows.Forms.Tools.ComboBoxAdv();
            this.generalStatePanel = new System.Windows.Forms.Panel();
            this.walkthroughDescriptionLabel = new Syncfusion.Windows.Forms.Tools.AutoLabel();
            this.interSpaceValueStatePanel = new System.Windows.Forms.Panel();
            this.interSpaceValueLabel = new Syncfusion.Windows.Forms.Tools.AutoLabel();
            this.interSpaceValueTextBox = new Syncfusion.Windows.Forms.Tools.DoubleTextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.walkthroughIndexLabel = new Syncfusion.Windows.Forms.Tools.AutoLabel();
            this.imageBoxPanel.SuspendLayout();
            this.imageBoxControlsToolStrip.SuspendLayout();
            this.initializationButtonsPanel.SuspendLayout();
            this.walkthroughPanel.SuspendLayout();
            this.walkthorughStatusPanel.SuspendLayout();
            this.statePanelsPanel.SuspendLayout();
            this.totalObjectsStatePanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.totalObjectsTextBox)).BeginInit();
            this.interSpaceTypeStatePanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.interSpaceTypeComboBox)).BeginInit();
            this.generalStatePanel.SuspendLayout();
            this.interSpaceValueStatePanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.interSpaceValueTextBox)).BeginInit();
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
            this.imageBoxPanel.Controls.Add(this.imageBoxControlsToolStrip);
            this.imageBoxPanel.Controls.Add(this.imageBox);
            this.imageBoxPanel.Location = new System.Drawing.Point(10, 8);
            this.imageBoxPanel.Name = "imageBoxPanel";
            this.imageBoxPanel.Size = new System.Drawing.Size(423, 338);
            this.imageBoxPanel.TabIndex = 5;
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
            this.walkthroughPanel.Controls.Add(this.walkthorughStatusPanel);
            this.walkthroughPanel.Controls.Add(this.initializationButtonsPanel);
            this.walkthroughPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.walkthroughPanel.Location = new System.Drawing.Point(10, 349);
            this.walkthroughPanel.Name = "walkthroughPanel";
            this.walkthroughPanel.Size = new System.Drawing.Size(423, 110);
            this.walkthroughPanel.TabIndex = 39;
            // 
            // walkthorughStatusPanel
            // 
            this.walkthorughStatusPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.walkthorughStatusPanel.ColumnCount = 2;
            this.walkthorughStatusPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.walkthorughStatusPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 90F));
            this.walkthorughStatusPanel.Controls.Add(this.statePanelsPanel, 1, 0);
            this.walkthorughStatusPanel.Controls.Add(this.panel1, 0, 0);
            this.walkthorughStatusPanel.Location = new System.Drawing.Point(0, 0);
            this.walkthorughStatusPanel.Name = "walkthorughStatusPanel";
            this.walkthorughStatusPanel.RowCount = 1;
            this.walkthorughStatusPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.walkthorughStatusPanel.Size = new System.Drawing.Size(423, 52);
            this.walkthorughStatusPanel.TabIndex = 8;
            // 
            // statePanelsPanel
            // 
            this.statePanelsPanel.Controls.Add(this.totalObjectsStatePanel);
            this.statePanelsPanel.Controls.Add(this.interSpaceTypeStatePanel);
            this.statePanelsPanel.Controls.Add(this.generalStatePanel);
            this.statePanelsPanel.Controls.Add(this.interSpaceValueStatePanel);
            this.statePanelsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.statePanelsPanel.Location = new System.Drawing.Point(42, 0);
            this.statePanelsPanel.Margin = new System.Windows.Forms.Padding(0);
            this.statePanelsPanel.Name = "statePanelsPanel";
            this.statePanelsPanel.Size = new System.Drawing.Size(381, 52);
            this.statePanelsPanel.TabIndex = 8;
            // 
            // totalObjectsStatePanel
            // 
            this.totalObjectsStatePanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.totalObjectsStatePanel.Controls.Add(this.totalObjectsLabel);
            this.totalObjectsStatePanel.Controls.Add(this.totalObjectsTextBox);
            this.totalObjectsStatePanel.Location = new System.Drawing.Point(400, 0);
            this.totalObjectsStatePanel.Margin = new System.Windows.Forms.Padding(0);
            this.totalObjectsStatePanel.Name = "totalObjectsStatePanel";
            this.totalObjectsStatePanel.Size = new System.Drawing.Size(375, 52);
            this.totalObjectsStatePanel.TabIndex = 10;
            this.totalObjectsStatePanel.Visible = false;
            // 
            // totalObjectsLabel
            // 
            this.totalObjectsLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.totalObjectsLabel.AutoSize = false;
            this.totalObjectsLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.totalObjectsLabel.Font = new System.Drawing.Font("Dosis", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.totalObjectsLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.totalObjectsLabel.Location = new System.Drawing.Point(3, 5);
            this.totalObjectsLabel.Margin = new System.Windows.Forms.Padding(3);
            this.totalObjectsLabel.Name = "totalObjectsLabel";
            this.totalObjectsLabel.Size = new System.Drawing.Size(229, 42);
            this.totalObjectsLabel.Style = Syncfusion.Windows.Forms.Tools.AutoLabelStyle.Office2016White;
            this.totalObjectsLabel.TabIndex = 10;
            this.totalObjectsLabel.Text = "Total Objects:";
            this.totalObjectsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.totalObjectsLabel.ThemeName = "Office2016White";
            // 
            // totalObjectsTextBox
            // 
            this.totalObjectsTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.totalObjectsTextBox.BackGroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.totalObjectsTextBox.BeforeTouchSize = new System.Drawing.Size(126, 42);
            this.totalObjectsTextBox.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(171)))), ((int)(((byte)(171)))), ((int)(((byte)(171)))));
            this.totalObjectsTextBox.BorderSides = ((System.Windows.Forms.Border3DSide)((System.Windows.Forms.Border3DSide.Left | System.Windows.Forms.Border3DSide.Right)));
            this.totalObjectsTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.totalObjectsTextBox.DeleteSelectionOnNegative = true;
            this.totalObjectsTextBox.Font = new System.Drawing.Font("Dosis", 20.75F);
            this.totalObjectsTextBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.totalObjectsTextBox.IntegerValue = ((long)(1));
            this.totalObjectsTextBox.Location = new System.Drawing.Point(237, 5);
            this.totalObjectsTextBox.Name = "totalObjectsTextBox";
            this.totalObjectsTextBox.PositiveColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.totalObjectsTextBox.Size = new System.Drawing.Size(135, 42);
            this.totalObjectsTextBox.Style = Syncfusion.Windows.Forms.Tools.TextBoxExt.theme.Office2016White;
            this.totalObjectsTextBox.TabIndex = 0;
            this.totalObjectsTextBox.Text = "1";
            this.totalObjectsTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.totalObjectsTextBox.ThemeName = "Office2016White";
            this.totalObjectsTextBox.ZeroColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            // 
            // interSpaceTypeStatePanel
            // 
            this.interSpaceTypeStatePanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.interSpaceTypeStatePanel.Controls.Add(this.interSpaceTypeComboBox);
            this.interSpaceTypeStatePanel.Location = new System.Drawing.Point(400, 0);
            this.interSpaceTypeStatePanel.Margin = new System.Windows.Forms.Padding(0);
            this.interSpaceTypeStatePanel.Name = "interSpaceTypeStatePanel";
            this.interSpaceTypeStatePanel.Size = new System.Drawing.Size(375, 52);
            this.interSpaceTypeStatePanel.TabIndex = 11;
            this.interSpaceTypeStatePanel.Visible = false;
            // 
            // interSpaceTypeComboBox
            // 
            this.interSpaceTypeComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.interSpaceTypeComboBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.interSpaceTypeComboBox.BeforeTouchSize = new System.Drawing.Size(375, 44);
            this.interSpaceTypeComboBox.Border3DStyle = System.Windows.Forms.Border3DStyle.Flat;
            this.interSpaceTypeComboBox.BorderSides = System.Windows.Forms.Border3DSide.Bottom;
            this.interSpaceTypeComboBox.FlatBorderColor = System.Drawing.Color.White;
            this.interSpaceTypeComboBox.FlatStyle = Syncfusion.Windows.Forms.Tools.ComboFlatStyle.Flat;
            this.interSpaceTypeComboBox.Font = new System.Drawing.Font("Dosis", 20.75F);
            this.interSpaceTypeComboBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.interSpaceTypeComboBox.Location = new System.Drawing.Point(0, 4);
            this.interSpaceTypeComboBox.MetroBorderColor = System.Drawing.Color.White;
            this.interSpaceTypeComboBox.Name = "interSpaceTypeComboBox";
            this.interSpaceTypeComboBox.Size = new System.Drawing.Size(375, 44);
            this.interSpaceTypeComboBox.Style = Syncfusion.Windows.Forms.VisualStyle.Office2016White;
            this.interSpaceTypeComboBox.TabIndex = 0;
            this.interSpaceTypeComboBox.Text = "Inter Space Type";
            this.interSpaceTypeComboBox.ThemeName = "Office2016White";
            // 
            // generalStatePanel
            // 
            this.generalStatePanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.generalStatePanel.Controls.Add(this.walkthroughDescriptionLabel);
            this.generalStatePanel.Location = new System.Drawing.Point(400, 0);
            this.generalStatePanel.Margin = new System.Windows.Forms.Padding(0);
            this.generalStatePanel.Name = "generalStatePanel";
            this.generalStatePanel.Size = new System.Drawing.Size(378, 52);
            this.generalStatePanel.TabIndex = 9;
            this.generalStatePanel.Visible = false;
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
            this.walkthroughDescriptionLabel.Size = new System.Drawing.Size(378, 52);
            this.walkthroughDescriptionLabel.Style = Syncfusion.Windows.Forms.Tools.AutoLabelStyle.Office2016White;
            this.walkthroughDescriptionLabel.TabIndex = 9;
            this.walkthroughDescriptionLabel.Text = "Configuration Walkthrough";
            this.walkthroughDescriptionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.walkthroughDescriptionLabel.ThemeName = "Office2016White";
            // 
            // interSpaceValueStatePanel
            // 
            this.interSpaceValueStatePanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.interSpaceValueStatePanel.Controls.Add(this.interSpaceValueLabel);
            this.interSpaceValueStatePanel.Controls.Add(this.interSpaceValueTextBox);
            this.interSpaceValueStatePanel.Location = new System.Drawing.Point(400, 0);
            this.interSpaceValueStatePanel.Margin = new System.Windows.Forms.Padding(0);
            this.interSpaceValueStatePanel.Name = "interSpaceValueStatePanel";
            this.interSpaceValueStatePanel.Size = new System.Drawing.Size(375, 52);
            this.interSpaceValueStatePanel.TabIndex = 12;
            // 
            // interSpaceValueLabel
            // 
            this.interSpaceValueLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.interSpaceValueLabel.AutoSize = false;
            this.interSpaceValueLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.interSpaceValueLabel.Font = new System.Drawing.Font("Dosis", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.interSpaceValueLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.interSpaceValueLabel.Location = new System.Drawing.Point(3, 5);
            this.interSpaceValueLabel.Margin = new System.Windows.Forms.Padding(3);
            this.interSpaceValueLabel.Name = "interSpaceValueLabel";
            this.interSpaceValueLabel.Size = new System.Drawing.Size(237, 42);
            this.interSpaceValueLabel.Style = Syncfusion.Windows.Forms.Tools.AutoLabelStyle.Office2016White;
            this.interSpaceValueLabel.TabIndex = 11;
            this.interSpaceValueLabel.Text = "Inter Space Value:";
            this.interSpaceValueLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.interSpaceValueLabel.ThemeName = "Office2016White";
            // 
            // interSpaceValueTextBox
            // 
            this.interSpaceValueTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.interSpaceValueTextBox.BackGroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.interSpaceValueTextBox.BeforeTouchSize = new System.Drawing.Size(126, 42);
            this.interSpaceValueTextBox.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(171)))), ((int)(((byte)(171)))), ((int)(((byte)(171)))));
            this.interSpaceValueTextBox.BorderSides = ((System.Windows.Forms.Border3DSide)((System.Windows.Forms.Border3DSide.Left | System.Windows.Forms.Border3DSide.Right)));
            this.interSpaceValueTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.interSpaceValueTextBox.DeleteSelectionOnNegative = true;
            this.interSpaceValueTextBox.DoubleValue = 1D;
            this.interSpaceValueTextBox.Font = new System.Drawing.Font("Dosis", 20.75F);
            this.interSpaceValueTextBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.interSpaceValueTextBox.Location = new System.Drawing.Point(246, 5);
            this.interSpaceValueTextBox.Name = "interSpaceValueTextBox";
            this.interSpaceValueTextBox.PositiveColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.interSpaceValueTextBox.Size = new System.Drawing.Size(126, 42);
            this.interSpaceValueTextBox.Style = Syncfusion.Windows.Forms.Tools.TextBoxExt.theme.Office2016White;
            this.interSpaceValueTextBox.TabIndex = 0;
            this.interSpaceValueTextBox.Text = "1.00";
            this.interSpaceValueTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.interSpaceValueTextBox.ThemeName = "Office2016White";
            this.interSpaceValueTextBox.ZeroColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
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
            this.imageBoxControlsToolStrip.ResumeLayout(false);
            this.imageBoxControlsToolStrip.PerformLayout();
            this.initializationButtonsPanel.ResumeLayout(false);
            this.walkthroughPanel.ResumeLayout(false);
            this.walkthorughStatusPanel.ResumeLayout(false);
            this.statePanelsPanel.ResumeLayout(false);
            this.totalObjectsStatePanel.ResumeLayout(false);
            this.totalObjectsStatePanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.totalObjectsTextBox)).EndInit();
            this.interSpaceTypeStatePanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.interSpaceTypeComboBox)).EndInit();
            this.generalStatePanel.ResumeLayout(false);
            this.interSpaceValueStatePanel.ResumeLayout(false);
            this.interSpaceValueStatePanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.interSpaceValueTextBox)).EndInit();
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
        private System.Windows.Forms.TableLayoutPanel walkthorughStatusPanel;
        private System.Windows.Forms.Panel statePanelsPanel;
        private System.Windows.Forms.Panel generalStatePanel;
        private Syncfusion.Windows.Forms.Tools.AutoLabel walkthroughDescriptionLabel;
        private System.Windows.Forms.Panel totalObjectsStatePanel;
        private Syncfusion.Windows.Forms.Tools.IntegerTextBox totalObjectsTextBox;
        private System.Windows.Forms.Panel interSpaceTypeStatePanel;
        private Syncfusion.Windows.Forms.Tools.ComboBoxAdv interSpaceTypeComboBox;
        private System.Windows.Forms.Panel interSpaceValueStatePanel;
        private Syncfusion.Windows.Forms.Tools.DoubleTextBox interSpaceValueTextBox;
        private Syncfusion.Windows.Forms.Tools.AutoLabel totalObjectsLabel;
        private Syncfusion.Windows.Forms.Tools.AutoLabel interSpaceValueLabel;
        private System.Windows.Forms.Panel panel1;
        private Syncfusion.Windows.Forms.Tools.AutoLabel walkthroughIndexLabel;
    }
}