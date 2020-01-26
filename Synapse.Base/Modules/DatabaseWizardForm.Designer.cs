namespace Synapse.Modules
{
    partial class DatabaseWizardForm
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
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.databasesTabControl = new Syncfusion.Windows.Forms.Tools.TabControlAdv();
            this.sqlDBTabPage = new Syncfusion.Windows.Forms.Tools.TabPageAdv();
            this.panel1 = new System.Windows.Forms.Panel();
            this.DBFieldsPanel = new System.Windows.Forms.Panel();
            this.emptyListLabel = new Syncfusion.Windows.Forms.Tools.AutoLabel();
            this.startInsertionBtn = new Syncfusion.Windows.Forms.ButtonAdv();
            this.sqlConnectionStringField = new Syncfusion.Windows.Forms.Tools.TextBoxExt();
            this.openDBConnectionBtn = new Syncfusion.Windows.Forms.ButtonAdv();
            this.sqlDBFields = new System.Windows.Forms.TableLayoutPanel();
            this.databaseTableCB = new Syncfusion.Windows.Forms.Tools.ComboBoxAdv();
            this.SplashText = new Syncfusion.Windows.Forms.Tools.AutoLabel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.selectFieldsBtn = new Syncfusion.Windows.Forms.ButtonAdv();
            this.autoLabel1 = new Syncfusion.Windows.Forms.Tools.AutoLabel();
            this.injectionProgressBar = new System.Windows.Forms.ProgressBar();
            this.connectionStatusPanel = new System.Windows.Forms.Panel();
            this.saveCSBtn = new Syncfusion.Windows.Forms.ButtonAdv();
            this.viewCSBtn = new Syncfusion.Windows.Forms.ButtonAdv();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.databasesTabControl)).BeginInit();
            this.databasesTabControl.SuspendLayout();
            this.sqlDBTabPage.SuspendLayout();
            this.panel1.SuspendLayout();
            this.DBFieldsPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sqlConnectionStringField)).BeginInit();
            this.sqlDBFields.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.databaseTableCB)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // databasesTabControl
            // 
            this.databasesTabControl.ActiveTabFont = new System.Drawing.Font("Dosis", 18.25F);
            this.databasesTabControl.ActiveTabForeColor = System.Drawing.Color.Empty;
            this.databasesTabControl.Alignment = System.Windows.Forms.TabAlignment.Left;
            this.databasesTabControl.BackColor = System.Drawing.Color.Black;
            this.databasesTabControl.BeforeTouchSize = new System.Drawing.Size(666, 464);
            this.databasesTabControl.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.databasesTabControl.CanOverrideStyle = true;
            this.databasesTabControl.CloseButtonForeColor = System.Drawing.Color.Empty;
            this.databasesTabControl.CloseButtonHoverForeColor = System.Drawing.Color.Empty;
            this.databasesTabControl.CloseButtonPressedForeColor = System.Drawing.Color.Empty;
            this.databasesTabControl.CloseTabOnMiddleClick = false;
            this.databasesTabControl.Controls.Add(this.sqlDBTabPage);
            this.databasesTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.databasesTabControl.FixedSingleBorderColor = System.Drawing.Color.Black;
            this.databasesTabControl.FocusOnTabClick = false;
            this.databasesTabControl.Font = new System.Drawing.Font("Dosis", 15.25F);
            this.databasesTabControl.InActiveTabForeColor = System.Drawing.Color.Empty;
            this.databasesTabControl.Location = new System.Drawing.Point(0, 10);
            this.databasesTabControl.Margin = new System.Windows.Forms.Padding(0);
            this.databasesTabControl.Name = "databasesTabControl";
            this.databasesTabControl.RotateTextWhenVertical = true;
            this.databasesTabControl.SeparatorColor = System.Drawing.SystemColors.ControlDark;
            this.databasesTabControl.ShowSeparator = false;
            this.databasesTabControl.Size = new System.Drawing.Size(666, 464);
            this.databasesTabControl.TabIndex = 0;
            this.databasesTabControl.TabStyle = typeof(Syncfusion.Windows.Forms.Tools.TabRendererOffice2016White);
            this.databasesTabControl.ThemeName = "TabRendererOffice2016White";
            this.databasesTabControl.ThemesEnabled = true;
            this.databasesTabControl.ThemeStyle.PrimitiveButtonStyle.DisabledNextPageImage = null;
            this.databasesTabControl.VSLikeScrollButton = true;
            this.databasesTabControl.SelectedIndexChanged += new System.EventHandler(this.databasesTabControl_SelectedIndexChanged);
            // 
            // sqlDBTabPage
            // 
            this.sqlDBTabPage.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.sqlDBTabPage.Controls.Add(this.panel1);
            this.sqlDBTabPage.Controls.Add(this.connectionStatusPanel);
            this.sqlDBTabPage.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(38)))), ((int)(((byte)(38)))));
            this.sqlDBTabPage.Image = null;
            this.sqlDBTabPage.ImageSize = new System.Drawing.Size(16, 16);
            this.sqlDBTabPage.Location = new System.Drawing.Point(145, 2);
            this.sqlDBTabPage.Name = "sqlDBTabPage";
            this.sqlDBTabPage.Padding = new System.Windows.Forms.Padding(1, 0, 1, 1);
            this.sqlDBTabPage.ShowCloseButton = true;
            this.sqlDBTabPage.Size = new System.Drawing.Size(519, 460);
            this.sqlDBTabPage.TabIndex = 1;
            this.sqlDBTabPage.Text = "Microsoft SQL";
            this.sqlDBTabPage.ThemesEnabled = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.saveCSBtn);
            this.panel1.Controls.Add(this.viewCSBtn);
            this.panel1.Controls.Add(this.DBFieldsPanel);
            this.panel1.Controls.Add(this.startInsertionBtn);
            this.panel1.Controls.Add(this.sqlConnectionStringField);
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Controls.Add(this.openDBConnectionBtn);
            this.panel1.Controls.Add(this.sqlDBFields);
            this.panel1.Controls.Add(this.tableLayoutPanel1);
            this.panel1.Controls.Add(this.injectionProgressBar);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(1, 8);
            this.panel1.Margin = new System.Windows.Forms.Padding(3, 20, 3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(517, 451);
            this.panel1.TabIndex = 52;
            // 
            // DBFieldsPanel
            // 
            this.DBFieldsPanel.AutoScroll = true;
            this.DBFieldsPanel.Controls.Add(this.emptyListLabel);
            this.DBFieldsPanel.Location = new System.Drawing.Point(8, 205);
            this.DBFieldsPanel.Name = "DBFieldsPanel";
            this.DBFieldsPanel.Size = new System.Drawing.Size(501, 188);
            this.DBFieldsPanel.TabIndex = 60;
            // 
            // emptyListLabel
            // 
            this.emptyListLabel.AutoSize = false;
            this.emptyListLabel.BackColor = System.Drawing.Color.Transparent;
            this.emptyListLabel.Font = new System.Drawing.Font("Dosis", 20F);
            this.emptyListLabel.ForeColor = System.Drawing.Color.Black;
            this.emptyListLabel.Location = new System.Drawing.Point(-3, -1);
            this.emptyListLabel.Margin = new System.Windows.Forms.Padding(5);
            this.emptyListLabel.Name = "emptyListLabel";
            this.emptyListLabel.Size = new System.Drawing.Size(504, 189);
            this.emptyListLabel.TabIndex = 5;
            this.emptyListLabel.Text = "No fields selected";
            this.emptyListLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // startInsertionBtn
            // 
            this.startInsertionBtn.Appearance = Syncfusion.Windows.Forms.ButtonAppearance.Metro;
            this.startInsertionBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(165)))), ((int)(((byte)(220)))));
            this.startInsertionBtn.BeforeTouchSize = new System.Drawing.Size(500, 45);
            this.startInsertionBtn.BorderStyleAdv = Syncfusion.Windows.Forms.ButtonAdvBorderStyle.Flat;
            this.startInsertionBtn.Font = new System.Drawing.Font("Dosis", 17F);
            this.startInsertionBtn.ForeColor = System.Drawing.Color.White;
            this.startInsertionBtn.Location = new System.Drawing.Point(9, 398);
            this.startInsertionBtn.Margin = new System.Windows.Forms.Padding(0);
            this.startInsertionBtn.MetroColor = System.Drawing.Color.DodgerBlue;
            this.startInsertionBtn.Name = "startInsertionBtn";
            this.startInsertionBtn.Size = new System.Drawing.Size(500, 45);
            this.startInsertionBtn.TabIndex = 59;
            this.startInsertionBtn.Text = "START INSERTION";
            this.startInsertionBtn.ThemeName = "Metro";
            this.startInsertionBtn.UseVisualStyle = true;
            this.startInsertionBtn.Click += new System.EventHandler(this.startInsertionBtn_Click);
            // 
            // sqlConnectionStringField
            // 
            this.sqlConnectionStringField.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.sqlConnectionStringField.BeforeTouchSize = new System.Drawing.Size(377, 36);
            this.sqlConnectionStringField.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(171)))), ((int)(((byte)(171)))), ((int)(((byte)(171)))));
            this.sqlConnectionStringField.BorderSides = ((System.Windows.Forms.Border3DSide)(((System.Windows.Forms.Border3DSide.Left | System.Windows.Forms.Border3DSide.Top) 
            | System.Windows.Forms.Border3DSide.Bottom)));
            this.sqlConnectionStringField.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.sqlConnectionStringField.Font = new System.Drawing.Font("Dosis", 16.75F);
            this.sqlConnectionStringField.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.sqlConnectionStringField.Location = new System.Drawing.Point(8, 9);
            this.sqlConnectionStringField.Margin = new System.Windows.Forms.Padding(5, 0, 0, 8);
            this.sqlConnectionStringField.MinimumSize = new System.Drawing.Size(1, 1);
            this.sqlConnectionStringField.Name = "sqlConnectionStringField";
            this.sqlConnectionStringField.Size = new System.Drawing.Size(377, 36);
            this.sqlConnectionStringField.Style = Syncfusion.Windows.Forms.Tools.TextBoxExt.theme.Office2016White;
            this.sqlConnectionStringField.TabIndex = 58;
            this.sqlConnectionStringField.Text = "Enter Connection String";
            this.sqlConnectionStringField.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.sqlConnectionStringField.ThemeName = "Office2016White";
            this.sqlConnectionStringField.ThemeStyle.BorderColor = System.Drawing.Color.Gainsboro;
            this.sqlConnectionStringField.ThemeStyle.CornerRadius = 0;
            // 
            // openDBConnectionBtn
            // 
            this.openDBConnectionBtn.Appearance = Syncfusion.Windows.Forms.ButtonAppearance.Metro;
            this.openDBConnectionBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(165)))), ((int)(((byte)(220)))));
            this.openDBConnectionBtn.BeforeTouchSize = new System.Drawing.Size(500, 39);
            this.openDBConnectionBtn.BorderStyleAdv = Syncfusion.Windows.Forms.ButtonAdvBorderStyle.Flat;
            this.openDBConnectionBtn.Font = new System.Drawing.Font("Dosis", 15F);
            this.openDBConnectionBtn.ForeColor = System.Drawing.Color.White;
            this.openDBConnectionBtn.Location = new System.Drawing.Point(9, 51);
            this.openDBConnectionBtn.Margin = new System.Windows.Forms.Padding(0);
            this.openDBConnectionBtn.MetroColor = System.Drawing.Color.SlateGray;
            this.openDBConnectionBtn.Name = "openDBConnectionBtn";
            this.openDBConnectionBtn.Size = new System.Drawing.Size(500, 39);
            this.openDBConnectionBtn.TabIndex = 52;
            this.openDBConnectionBtn.Text = "OPEN  CONNECTION";
            this.openDBConnectionBtn.ThemeName = "Metro";
            this.openDBConnectionBtn.UseVisualStyle = true;
            this.openDBConnectionBtn.Click += new System.EventHandler(this.openDBConnectionBtn_Click);
            // 
            // sqlDBFields
            // 
            this.sqlDBFields.ColumnCount = 2;
            this.sqlDBFields.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 35.80705F));
            this.sqlDBFields.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 64.19295F));
            this.sqlDBFields.Controls.Add(this.databaseTableCB, 0, 0);
            this.sqlDBFields.Controls.Add(this.SplashText, 0, 0);
            this.sqlDBFields.Location = new System.Drawing.Point(9, 108);
            this.sqlDBFields.Name = "sqlDBFields";
            this.sqlDBFields.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.sqlDBFields.RowCount = 1;
            this.sqlDBFields.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 30.55556F));
            this.sqlDBFields.Size = new System.Drawing.Size(500, 52);
            this.sqlDBFields.TabIndex = 53;
            // 
            // databaseTableCB
            // 
            this.databaseTableCB.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.databaseTableCB.BeforeTouchSize = new System.Drawing.Size(315, 40);
            this.databaseTableCB.Border3DStyle = System.Windows.Forms.Border3DStyle.Flat;
            this.databaseTableCB.BorderSides = System.Windows.Forms.Border3DSide.Bottom;
            this.databaseTableCB.CanOverrideStyle = true;
            this.databaseTableCB.Dock = System.Windows.Forms.DockStyle.Fill;
            this.databaseTableCB.FlatBorderColor = System.Drawing.Color.White;
            this.databaseTableCB.FlatStyle = Syncfusion.Windows.Forms.Tools.ComboFlatStyle.Flat;
            this.databaseTableCB.Font = new System.Drawing.Font("Dosis", 19F);
            this.databaseTableCB.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.databaseTableCB.Location = new System.Drawing.Point(182, 8);
            this.databaseTableCB.MetroBorderColor = System.Drawing.Color.White;
            this.databaseTableCB.Name = "databaseTableCB";
            this.databaseTableCB.Size = new System.Drawing.Size(315, 40);
            this.databaseTableCB.Style = Syncfusion.Windows.Forms.VisualStyle.Office2016White;
            this.databaseTableCB.TabIndex = 16;
            this.databaseTableCB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.databaseTableCB.ThemeName = "Office2016White";
            // 
            // SplashText
            // 
            this.SplashText.AutoSize = false;
            this.SplashText.BackColor = System.Drawing.Color.Transparent;
            this.SplashText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SplashText.Font = new System.Drawing.Font("Dosis", 19F);
            this.SplashText.ForeColor = System.Drawing.Color.Black;
            this.SplashText.Location = new System.Drawing.Point(3, 5);
            this.SplashText.Name = "SplashText";
            this.SplashText.Size = new System.Drawing.Size(173, 47);
            this.SplashText.TabIndex = 4;
            this.SplashText.Text = "Database Table:";
            this.SplashText.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 35.80705F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 64.19295F));
            this.tableLayoutPanel1.Controls.Add(this.selectFieldsBtn, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.autoLabel1, 0, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(9, 160);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 30.55556F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(500, 39);
            this.tableLayoutPanel1.TabIndex = 54;
            // 
            // selectFieldsBtn
            // 
            this.selectFieldsBtn.Appearance = Syncfusion.Windows.Forms.ButtonAppearance.Metro;
            this.selectFieldsBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(165)))), ((int)(((byte)(220)))));
            this.selectFieldsBtn.BeforeTouchSize = new System.Drawing.Size(321, 39);
            this.selectFieldsBtn.BorderStyleAdv = Syncfusion.Windows.Forms.ButtonAdvBorderStyle.Flat;
            this.selectFieldsBtn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.selectFieldsBtn.Font = new System.Drawing.Font("Dosis", 15F);
            this.selectFieldsBtn.ForeColor = System.Drawing.Color.White;
            this.selectFieldsBtn.KeepFocusRectangle = false;
            this.selectFieldsBtn.Location = new System.Drawing.Point(179, 0);
            this.selectFieldsBtn.Margin = new System.Windows.Forms.Padding(0);
            this.selectFieldsBtn.MetroColor = System.Drawing.Color.Gray;
            this.selectFieldsBtn.Name = "selectFieldsBtn";
            this.selectFieldsBtn.Size = new System.Drawing.Size(321, 39);
            this.selectFieldsBtn.TabIndex = 47;
            this.selectFieldsBtn.Text = "SELECT FIELDS";
            this.selectFieldsBtn.ThemeName = "Metro";
            this.selectFieldsBtn.ThemeStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.selectFieldsBtn.ThemeStyle.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(171)))), ((int)(((byte)(171)))), ((int)(((byte)(171)))));
            this.selectFieldsBtn.ThemeStyle.DisabledBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(243)))), ((int)(((byte)(243)))));
            this.selectFieldsBtn.ThemeStyle.DisabledBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
            this.selectFieldsBtn.ThemeStyle.DisabledForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(171)))), ((int)(((byte)(171)))), ((int)(((byte)(171)))));
            this.selectFieldsBtn.ThemeStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.selectFieldsBtn.ThemeStyle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.selectFieldsBtn.ThemeStyle.HoverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(236)))), ((int)(((byte)(249)))));
            this.selectFieldsBtn.ThemeStyle.HoverBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(113)))), ((int)(((byte)(175)))), ((int)(((byte)(229)))));
            this.selectFieldsBtn.ThemeStyle.HoverForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.selectFieldsBtn.ThemeStyle.PressedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(205)))), ((int)(((byte)(230)))), ((int)(((byte)(247)))));
            this.selectFieldsBtn.ThemeStyle.PressedBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(212)))));
            this.selectFieldsBtn.ThemeStyle.PressedForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.selectFieldsBtn.UseVisualStyle = true;
            this.selectFieldsBtn.Click += new System.EventHandler(this.selectFieldsBtn_Click);
            // 
            // autoLabel1
            // 
            this.autoLabel1.AutoSize = false;
            this.autoLabel1.BackColor = System.Drawing.Color.Transparent;
            this.autoLabel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.autoLabel1.Font = new System.Drawing.Font("Dosis", 19F);
            this.autoLabel1.ForeColor = System.Drawing.Color.Black;
            this.autoLabel1.Location = new System.Drawing.Point(3, 0);
            this.autoLabel1.Name = "autoLabel1";
            this.autoLabel1.Size = new System.Drawing.Size(173, 39);
            this.autoLabel1.TabIndex = 4;
            this.autoLabel1.Text = "Fields To Insert:";
            this.autoLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // injectionProgressBar
            // 
            this.injectionProgressBar.Location = new System.Drawing.Point(9, 398);
            this.injectionProgressBar.Name = "injectionProgressBar";
            this.injectionProgressBar.Size = new System.Drawing.Size(500, 45);
            this.injectionProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.injectionProgressBar.TabIndex = 74;
            // 
            // connectionStatusPanel
            // 
            this.connectionStatusPanel.BackColor = System.Drawing.Color.SlateGray;
            this.connectionStatusPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.connectionStatusPanel.Location = new System.Drawing.Point(1, 0);
            this.connectionStatusPanel.Name = "connectionStatusPanel";
            this.connectionStatusPanel.Size = new System.Drawing.Size(517, 8);
            this.connectionStatusPanel.TabIndex = 0;
            // 
            // saveCSBtn
            // 
            this.saveCSBtn.Appearance = Syncfusion.Windows.Forms.ButtonAppearance.Metro;
            this.saveCSBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(165)))), ((int)(((byte)(220)))));
            this.saveCSBtn.BeforeTouchSize = new System.Drawing.Size(83, 36);
            this.saveCSBtn.BorderStyleAdv = Syncfusion.Windows.Forms.ButtonAdvBorderStyle.Flat;
            this.saveCSBtn.Font = new System.Drawing.Font("Dosis", 15F);
            this.saveCSBtn.ForeColor = System.Drawing.Color.White;
            this.saveCSBtn.KeepFocusRectangle = false;
            this.saveCSBtn.Location = new System.Drawing.Point(426, 9);
            this.saveCSBtn.Margin = new System.Windows.Forms.Padding(0);
            this.saveCSBtn.MetroColor = System.Drawing.Color.Gray;
            this.saveCSBtn.Name = "saveCSBtn";
            this.saveCSBtn.Size = new System.Drawing.Size(83, 36);
            this.saveCSBtn.TabIndex = 76;
            this.saveCSBtn.Text = "SAVE";
            this.saveCSBtn.ThemeName = "Metro";
            this.saveCSBtn.UseVisualStyle = true;
            this.saveCSBtn.Click += new System.EventHandler(this.saveCSBtn_Click);
            // 
            // viewCSBtn
            // 
            this.viewCSBtn.Appearance = Syncfusion.Windows.Forms.ButtonAppearance.Office2016White;
            this.viewCSBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(165)))), ((int)(((byte)(220)))));
            this.viewCSBtn.BackgroundImage = global::Synapse.Properties.Resources.locateOptionsBtnIcon_ReadingTab;
            this.viewCSBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.viewCSBtn.BeforeTouchSize = new System.Drawing.Size(41, 36);
            this.viewCSBtn.BorderStyleAdv = Syncfusion.Windows.Forms.ButtonAdvBorderStyle.Flat;
            this.viewCSBtn.Font = new System.Drawing.Font("Dosis", 15F);
            this.viewCSBtn.Image = global::Synapse.Properties.Resources.Show_01_WF;
            this.viewCSBtn.KeepFocusRectangle = false;
            this.viewCSBtn.Location = new System.Drawing.Point(385, 9);
            this.viewCSBtn.Margin = new System.Windows.Forms.Padding(0);
            this.viewCSBtn.MetroColor = System.Drawing.Color.White;
            this.viewCSBtn.Name = "viewCSBtn";
            this.viewCSBtn.Size = new System.Drawing.Size(41, 36);
            this.viewCSBtn.TabIndex = 75;
            this.viewCSBtn.ThemeName = "Office2016White";
            this.viewCSBtn.UseVisualStyle = true;
            this.viewCSBtn.Click += new System.EventHandler(this.viewCSBtn_Click);
            this.viewCSBtn.MouseDown += new System.Windows.Forms.MouseEventHandler(this.viewCSBtn_MouseDown);
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Gray;
            this.pictureBox1.Location = new System.Drawing.Point(0, 100);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(518, 1);
            this.pictureBox1.TabIndex = 57;
            this.pictureBox1.TabStop = false;
            // 
            // DatabaseWizardForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.ClientSize = new System.Drawing.Size(666, 479);
            this.Controls.Add(this.databasesTabControl);
            this.Font = new System.Drawing.Font("Dosis", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DatabaseWizardForm";
            this.Padding = new System.Windows.Forms.Padding(0, 10, 0, 5);
            this.ShowToolTip = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Style.InactiveShadowOpacity = ((byte)(50));
            this.Style.ShadowOpacity = ((byte)(0));
            this.Style.TitleBar.BackColor = System.Drawing.Color.White;
            this.Style.TitleBar.Font = new System.Drawing.Font("Dosis", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Style.TitleBar.TextHorizontalAlignment = System.Windows.Forms.HorizontalAlignment.Center;
            this.Text = "Database Wizard";
            ((System.ComponentModel.ISupportInitialize)(this.databasesTabControl)).EndInit();
            this.databasesTabControl.ResumeLayout(false);
            this.sqlDBTabPage.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.DBFieldsPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.sqlConnectionStringField)).EndInit();
            this.sqlDBFields.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.databaseTableCB)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        private Syncfusion.Windows.Forms.Tools.TabControlAdv databasesTabControl;
        private Syncfusion.Windows.Forms.Tools.TabPageAdv sqlDBTabPage;
        private System.Windows.Forms.Panel connectionStatusPanel;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TableLayoutPanel sqlDBFields;
        private Syncfusion.Windows.Forms.Tools.ComboBoxAdv databaseTableCB;
        private Syncfusion.Windows.Forms.Tools.AutoLabel SplashText;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private Syncfusion.Windows.Forms.ButtonAdv selectFieldsBtn;
        private Syncfusion.Windows.Forms.Tools.AutoLabel autoLabel1;
        private Syncfusion.Windows.Forms.ButtonAdv openDBConnectionBtn;
        private Syncfusion.Windows.Forms.Tools.TextBoxExt sqlConnectionStringField;
        private Syncfusion.Windows.Forms.ButtonAdv startInsertionBtn;
        private Syncfusion.Windows.Forms.Tools.AutoLabel emptyListLabel;
        private System.Windows.Forms.Panel DBFieldsPanel;
        private System.Windows.Forms.ProgressBar injectionProgressBar;
        private Syncfusion.Windows.Forms.ButtonAdv saveCSBtn;
        private Syncfusion.Windows.Forms.ButtonAdv viewCSBtn;
    }
}