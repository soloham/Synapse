namespace Synapse.Modules
{
    partial class ICRConfigurationForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ICRConfigurationForm));
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.MainLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.configureStatesPanel = new System.Windows.Forms.TableLayoutPanel();
            this.selectStateComboBox = new Syncfusion.Windows.Forms.Tools.ComboBoxAdv();
            this.curConfigureStatePanel = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.walkthroughIndexLabel = new Syncfusion.Windows.Forms.Tools.AutoLabel();
            this.initializationButtonsPanel = new System.Windows.Forms.TableLayoutPanel();
            this.setBtn = new Syncfusion.Windows.Forms.ButtonAdv();
            this.nextBtn = new Syncfusion.Windows.Forms.ButtonAdv();
            this.reconfigureBtn = new Syncfusion.Windows.Forms.ButtonAdv();
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
            this.imageBox = new Cyotek.Windows.Forms.Demo.ImageBoxEx();
            this.MainLayoutPanel.SuspendLayout();
            this.configureStatesPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.selectStateComboBox)).BeginInit();
            this.curConfigureStatePanel.SuspendLayout();
            this.panel1.SuspendLayout();
            this.initializationButtonsPanel.SuspendLayout();
            this.imageBoxPanel.SuspendLayout();
            this.imageBoxControlsToolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // MainLayoutPanel
            // 
            this.MainLayoutPanel.ColumnCount = 1;
            this.MainLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.MainLayoutPanel.Controls.Add(this.configureStatesPanel, 0, 1);
            this.MainLayoutPanel.Controls.Add(this.imageBoxPanel, 0, 0);
            this.MainLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainLayoutPanel.Location = new System.Drawing.Point(8, 2);
            this.MainLayoutPanel.Name = "MainLayoutPanel";
            this.MainLayoutPanel.RowCount = 2;
            this.MainLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.MainLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 115F));
            this.MainLayoutPanel.Size = new System.Drawing.Size(664, 366);
            this.MainLayoutPanel.TabIndex = 43;
            // 
            // configureStatesPanel
            // 
            this.configureStatesPanel.BackColor = System.Drawing.Color.White;
            this.configureStatesPanel.ColumnCount = 1;
            this.configureStatesPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.configureStatesPanel.Controls.Add(this.selectStateComboBox, 0, 1);
            this.configureStatesPanel.Controls.Add(this.curConfigureStatePanel, 0, 2);
            this.configureStatesPanel.Controls.Add(this.initializationButtonsPanel, 0, 3);
            this.configureStatesPanel.Controls.Add(this.reconfigureBtn, 0, 0);
            this.configureStatesPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.configureStatesPanel.Location = new System.Drawing.Point(3, 254);
            this.configureStatesPanel.Name = "configureStatesPanel";
            this.configureStatesPanel.RowCount = 4;
            this.configureStatesPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 0F));
            this.configureStatesPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 0F));
            this.configureStatesPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.configureStatesPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.configureStatesPanel.Size = new System.Drawing.Size(658, 109);
            this.configureStatesPanel.TabIndex = 41;
            // 
            // selectStateComboBox
            // 
            this.selectStateComboBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.selectStateComboBox.BeforeTouchSize = new System.Drawing.Size(652, 43);
            this.selectStateComboBox.Border3DStyle = System.Windows.Forms.Border3DStyle.Flat;
            this.selectStateComboBox.BorderSides = System.Windows.Forms.Border3DSide.Bottom;
            this.selectStateComboBox.CanOverrideStyle = true;
            this.selectStateComboBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.selectStateComboBox.FlatBorderColor = System.Drawing.Color.White;
            this.selectStateComboBox.FlatStyle = Syncfusion.Windows.Forms.Tools.ComboFlatStyle.Flat;
            this.selectStateComboBox.Font = new System.Drawing.Font("Dosis", 20.75F);
            this.selectStateComboBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.selectStateComboBox.Location = new System.Drawing.Point(3, 3);
            this.selectStateComboBox.MetroBorderColor = System.Drawing.Color.White;
            this.selectStateComboBox.Name = "selectStateComboBox";
            this.selectStateComboBox.Size = new System.Drawing.Size(652, 43);
            this.selectStateComboBox.Style = Syncfusion.Windows.Forms.VisualStyle.Office2016White;
            this.selectStateComboBox.TabIndex = 15;
            this.selectStateComboBox.Text = "Region Settings";
            this.selectStateComboBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.selectStateComboBox.ThemeName = "Office2016White";
            this.selectStateComboBox.Visible = false;
            // 
            // curConfigureStatePanel
            // 
            this.curConfigureStatePanel.BackColor = System.Drawing.Color.White;
            this.curConfigureStatePanel.ColumnCount = 2;
            this.curConfigureStatePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.curConfigureStatePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 90F));
            this.curConfigureStatePanel.Controls.Add(this.panel1, 0, 0);
            this.curConfigureStatePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.curConfigureStatePanel.Location = new System.Drawing.Point(0, 0);
            this.curConfigureStatePanel.Margin = new System.Windows.Forms.Padding(0);
            this.curConfigureStatePanel.Name = "curConfigureStatePanel";
            this.curConfigureStatePanel.RowCount = 1;
            this.curConfigureStatePanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.curConfigureStatePanel.Size = new System.Drawing.Size(658, 54);
            this.curConfigureStatePanel.TabIndex = 8;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.walkthroughIndexLabel);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(65, 54);
            this.panel1.TabIndex = 9;
            // 
            // walkthroughIndexLabel
            // 
            this.walkthroughIndexLabel.AutoSize = false;
            this.walkthroughIndexLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.walkthroughIndexLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.walkthroughIndexLabel.Font = new System.Drawing.Font("Dosis", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.walkthroughIndexLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.walkthroughIndexLabel.Location = new System.Drawing.Point(0, 0);
            this.walkthroughIndexLabel.Margin = new System.Windows.Forms.Padding(0);
            this.walkthroughIndexLabel.Name = "walkthroughIndexLabel";
            this.walkthroughIndexLabel.Size = new System.Drawing.Size(65, 54);
            this.walkthroughIndexLabel.Style = Syncfusion.Windows.Forms.Tools.AutoLabelStyle.Office2016White;
            this.walkthroughIndexLabel.TabIndex = 8;
            this.walkthroughIndexLabel.Text = "1.";
            this.walkthroughIndexLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.walkthroughIndexLabel.ThemeName = "Office2016White";
            // 
            // initializationButtonsPanel
            // 
            this.initializationButtonsPanel.ColumnCount = 2;
            this.initializationButtonsPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.initializationButtonsPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.initializationButtonsPanel.Controls.Add(this.setBtn, 0, 0);
            this.initializationButtonsPanel.Controls.Add(this.nextBtn, 1, 0);
            this.initializationButtonsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.initializationButtonsPanel.Location = new System.Drawing.Point(0, 54);
            this.initializationButtonsPanel.Margin = new System.Windows.Forms.Padding(0);
            this.initializationButtonsPanel.Name = "initializationButtonsPanel";
            this.initializationButtonsPanel.RowCount = 1;
            this.initializationButtonsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.initializationButtonsPanel.Size = new System.Drawing.Size(658, 55);
            this.initializationButtonsPanel.TabIndex = 7;
            // 
            // setBtn
            // 
            this.setBtn.Appearance = Syncfusion.Windows.Forms.ButtonAppearance.Metro;
            this.setBtn.BackColor = System.Drawing.Color.DodgerBlue;
            this.setBtn.BeforeTouchSize = new System.Drawing.Size(323, 49);
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
            this.setBtn.Size = new System.Drawing.Size(323, 49);
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
            // 
            // nextBtn
            // 
            this.nextBtn.Appearance = Syncfusion.Windows.Forms.ButtonAppearance.Metro;
            this.nextBtn.BackColor = System.Drawing.Color.LightSlateGray;
            this.nextBtn.BeforeTouchSize = new System.Drawing.Size(323, 49);
            this.nextBtn.BorderStyleAdv = Syncfusion.Windows.Forms.ButtonAdvBorderStyle.Flat;
            this.nextBtn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.nextBtn.FlatAppearance.BorderSize = 0;
            this.nextBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.nextBtn.Font = new System.Drawing.Font("Dosis", 20F);
            this.nextBtn.ForeColor = System.Drawing.Color.White;
            this.nextBtn.Image = global::Synapse.Properties.Resources.Out;
            this.nextBtn.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.nextBtn.Location = new System.Drawing.Point(332, 3);
            this.nextBtn.Name = "nextBtn";
            this.nextBtn.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.nextBtn.Size = new System.Drawing.Size(323, 49);
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
            // 
            // reconfigureBtn
            // 
            this.reconfigureBtn.Appearance = Syncfusion.Windows.Forms.ButtonAppearance.Metro;
            this.reconfigureBtn.BackColor = System.Drawing.Color.CornflowerBlue;
            this.reconfigureBtn.BeforeTouchSize = new System.Drawing.Size(652, 1);
            this.reconfigureBtn.BorderStyleAdv = Syncfusion.Windows.Forms.ButtonAdvBorderStyle.Flat;
            this.reconfigureBtn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.reconfigureBtn.FlatAppearance.BorderSize = 0;
            this.reconfigureBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.reconfigureBtn.Font = new System.Drawing.Font("Dosis", 22.25F);
            this.reconfigureBtn.ForeColor = System.Drawing.Color.White;
            this.reconfigureBtn.Image = global::Synapse.Properties.Resources.Gear_02_WF;
            this.reconfigureBtn.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.reconfigureBtn.Location = new System.Drawing.Point(3, 3);
            this.reconfigureBtn.Name = "reconfigureBtn";
            this.reconfigureBtn.Padding = new System.Windows.Forms.Padding(10, 0, 10, 0);
            this.reconfigureBtn.Size = new System.Drawing.Size(652, 1);
            this.reconfigureBtn.TabIndex = 40;
            this.reconfigureBtn.Text = "   CONFIGURE";
            this.reconfigureBtn.ThemeName = "Metro";
            this.reconfigureBtn.ThemeStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.reconfigureBtn.ThemeStyle.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(171)))), ((int)(((byte)(171)))), ((int)(((byte)(171)))));
            this.reconfigureBtn.ThemeStyle.DisabledBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(243)))), ((int)(((byte)(243)))));
            this.reconfigureBtn.ThemeStyle.DisabledBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
            this.reconfigureBtn.ThemeStyle.DisabledForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(171)))), ((int)(((byte)(171)))), ((int)(((byte)(171)))));
            this.reconfigureBtn.ThemeStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.reconfigureBtn.ThemeStyle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.reconfigureBtn.ThemeStyle.HoverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(236)))), ((int)(((byte)(249)))));
            this.reconfigureBtn.ThemeStyle.HoverBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(113)))), ((int)(((byte)(175)))), ((int)(((byte)(229)))));
            this.reconfigureBtn.ThemeStyle.HoverForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.reconfigureBtn.ThemeStyle.PressedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(205)))), ((int)(((byte)(230)))), ((int)(((byte)(247)))));
            this.reconfigureBtn.ThemeStyle.PressedBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(212)))));
            this.reconfigureBtn.ThemeStyle.PressedForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.reconfigureBtn.UseVisualStyle = false;
            this.reconfigureBtn.UseVisualStyleBackColor = false;
            // 
            // imageBoxPanel
            // 
            this.imageBoxPanel.Controls.Add(this.imageBoxControlsToolStrip);
            this.imageBoxPanel.Controls.Add(this.imageBox);
            this.imageBoxPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.imageBoxPanel.Location = new System.Drawing.Point(3, 3);
            this.imageBoxPanel.Name = "imageBoxPanel";
            this.imageBoxPanel.Size = new System.Drawing.Size(658, 245);
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
            this.imageBoxControlsToolStrip.Size = new System.Drawing.Size(658, 25);
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
            // 
            // zoomInToolStripButton
            // 
            this.zoomInToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.zoomInToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("zoomInToolStripButton.Image")));
            this.zoomInToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.zoomInToolStripButton.Name = "zoomInToolStripButton";
            this.zoomInToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.zoomInToolStripButton.Text = "Zoom In";
            // 
            // zoomOutToolStripButton
            // 
            this.zoomOutToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.zoomOutToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("zoomOutToolStripButton.Image")));
            this.zoomOutToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.zoomOutToolStripButton.Name = "zoomOutToolStripButton";
            this.zoomOutToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.zoomOutToolStripButton.Text = "Zoom Out";
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
            // 
            // selectNoneToolStripButton
            // 
            this.selectNoneToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.selectNoneToolStripButton.Image = global::Synapse.Properties.Resources.selection;
            this.selectNoneToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.selectNoneToolStripButton.Name = "selectNoneToolStripButton";
            this.selectNoneToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.selectNoneToolStripButton.Text = "Select None";
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
            this.imageBox.Size = new System.Drawing.Size(661, 219);
            this.imageBox.StepSize = new System.Drawing.Size(8, 8);
            this.imageBox.TabIndex = 2;
            this.imageBox.Text = "Configuration Region";
            // 
            // ICRConfigurationForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.ClientSize = new System.Drawing.Size(674, 373);
            this.Controls.Add(this.MainLayoutPanel);
            this.Font = new System.Drawing.Font("Dosis", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(740, 412);
            this.MinimizeBox = false;
            this.Name = "ICRConfigurationForm";
            this.Padding = new System.Windows.Forms.Padding(8, 2, 2, 5);
            this.ShowToolTip = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Style.InactiveShadowOpacity = ((byte)(50));
            this.Style.ShadowOpacity = ((byte)(0));
            this.Style.TitleBar.BackColor = System.Drawing.Color.White;
            this.Style.TitleBar.Font = new System.Drawing.Font("Dosis", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Style.TitleBar.TextHorizontalAlignment = System.Windows.Forms.HorizontalAlignment.Center;
            this.Text = "ICR Configuration Tool";
            this.MainLayoutPanel.ResumeLayout(false);
            this.configureStatesPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.selectStateComboBox)).EndInit();
            this.curConfigureStatePanel.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.initializationButtonsPanel.ResumeLayout(false);
            this.imageBoxPanel.ResumeLayout(false);
            this.imageBoxPanel.PerformLayout();
            this.imageBoxControlsToolStrip.ResumeLayout(false);
            this.imageBoxControlsToolStrip.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        private System.Windows.Forms.TableLayoutPanel MainLayoutPanel;
        private System.Windows.Forms.TableLayoutPanel configureStatesPanel;
        private Syncfusion.Windows.Forms.Tools.ComboBoxAdv selectStateComboBox;
        private System.Windows.Forms.TableLayoutPanel curConfigureStatePanel;
        private System.Windows.Forms.Panel panel1;
        private Syncfusion.Windows.Forms.Tools.AutoLabel walkthroughIndexLabel;
        private System.Windows.Forms.TableLayoutPanel initializationButtonsPanel;
        private Syncfusion.Windows.Forms.ButtonAdv setBtn;
        private Syncfusion.Windows.Forms.ButtonAdv nextBtn;
        private Syncfusion.Windows.Forms.ButtonAdv reconfigureBtn;
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
        private Cyotek.Windows.Forms.Demo.ImageBoxEx imageBox;
    }
}