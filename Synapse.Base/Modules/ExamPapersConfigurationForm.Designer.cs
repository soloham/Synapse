namespace Synapse.Modules
{
    partial class ExamPapersConfigurationForm
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
            this.containerFlowPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.emptyListLabel = new Syncfusion.Windows.Forms.Tools.AutoLabel();
            this.paperConfigurationPanel = new System.Windows.Forms.Panel();
            this.paperCodeTitleTable = new System.Windows.Forms.TableLayoutPanel();
            this.finishPaperBtn = new Syncfusion.Windows.Forms.ButtonAdv();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.paperFieldsCountField = new Syncfusion.Windows.Forms.Tools.IntegerTextBox();
            this.paperOptionsCountField = new Syncfusion.Windows.Forms.Tools.IntegerTextBox();
            this.autoLabel3 = new Syncfusion.Windows.Forms.Tools.AutoLabel();
            this.autoLabel4 = new Syncfusion.Windows.Forms.Tools.AutoLabel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.paperDirectionField = new Syncfusion.Windows.Forms.Tools.ComboBoxAdv();
            this.autoLabel8 = new Syncfusion.Windows.Forms.Tools.AutoLabel();
            this.autoLabel2 = new Syncfusion.Windows.Forms.Tools.AutoLabel();
            this.autoLabel1 = new Syncfusion.Windows.Forms.Tools.AutoLabel();
            this.paperTitleField = new Syncfusion.Windows.Forms.Tools.TextBoxExt();
            this.papersListTable = new System.Windows.Forms.TableLayoutPanel();
            this.addNewPaperBtn = new Syncfusion.Windows.Forms.ButtonAdv();
            this.paperCodeField = new Syncfusion.Windows.Forms.Tools.TextBoxExt();
            this.containerFlowPanel.SuspendLayout();
            this.paperConfigurationPanel.SuspendLayout();
            this.paperCodeTitleTable.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.paperFieldsCountField)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.paperOptionsCountField)).BeginInit();
            this.tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.paperDirectionField)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.paperTitleField)).BeginInit();
            this.papersListTable.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.paperCodeField)).BeginInit();
            this.SuspendLayout();
            // 
            // containerFlowPanel
            // 
            this.containerFlowPanel.Controls.Add(this.emptyListLabel);
            this.containerFlowPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.containerFlowPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.containerFlowPanel.Location = new System.Drawing.Point(4, 4);
            this.containerFlowPanel.Margin = new System.Windows.Forms.Padding(1);
            this.containerFlowPanel.Name = "containerFlowPanel";
            this.containerFlowPanel.Size = new System.Drawing.Size(486, 293);
            this.containerFlowPanel.TabIndex = 0;
            // 
            // emptyListLabel
            // 
            this.emptyListLabel.AutoSize = false;
            this.emptyListLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.emptyListLabel.Font = new System.Drawing.Font("Dosis", 20.25F);
            this.emptyListLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.emptyListLabel.Location = new System.Drawing.Point(0, 0);
            this.emptyListLabel.Margin = new System.Windows.Forms.Padding(0);
            this.emptyListLabel.Name = "emptyListLabel";
            this.emptyListLabel.Size = new System.Drawing.Size(493, 299);
            this.emptyListLabel.Style = Syncfusion.Windows.Forms.Tools.AutoLabelStyle.Office2016White;
            this.emptyListLabel.TabIndex = 2;
            this.emptyListLabel.Text = "You currently have no exam papers,\r\nCreate/Import one to continue";
            this.emptyListLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.emptyListLabel.ThemeName = "Office2016White";
            this.emptyListLabel.Visible = false;
            // 
            // paperConfigurationPanel
            // 
            this.paperConfigurationPanel.Controls.Add(this.paperCodeTitleTable);
            this.paperConfigurationPanel.Location = new System.Drawing.Point(464, 2);
            this.paperConfigurationPanel.Name = "paperConfigurationPanel";
            this.paperConfigurationPanel.Padding = new System.Windows.Forms.Padding(3);
            this.paperConfigurationPanel.Size = new System.Drawing.Size(148, 346);
            this.paperConfigurationPanel.TabIndex = 1;
            this.paperConfigurationPanel.Visible = false;
            // 
            // paperCodeTitleTable
            // 
            this.paperCodeTitleTable.ColumnCount = 2;
            this.paperCodeTitleTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 47.88733F));
            this.paperCodeTitleTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 52.11267F));
            this.paperCodeTitleTable.Controls.Add(this.finishPaperBtn, 0, 6);
            this.paperCodeTitleTable.Controls.Add(this.tableLayoutPanel1, 0, 4);
            this.paperCodeTitleTable.Controls.Add(this.tableLayoutPanel2, 0, 2);
            this.paperCodeTitleTable.Controls.Add(this.autoLabel2, 1, 0);
            this.paperCodeTitleTable.Controls.Add(this.autoLabel1, 0, 0);
            this.paperCodeTitleTable.Controls.Add(this.paperTitleField, 1, 1);
            this.paperCodeTitleTable.Controls.Add(this.paperCodeField, 0, 1);
            this.paperCodeTitleTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.paperCodeTitleTable.Location = new System.Drawing.Point(3, 3);
            this.paperCodeTitleTable.Name = "paperCodeTitleTable";
            this.paperCodeTitleTable.RowCount = 7;
            this.paperCodeTitleTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.paperCodeTitleTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.paperCodeTitleTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.paperCodeTitleTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.paperCodeTitleTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.paperCodeTitleTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.paperCodeTitleTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 45F));
            this.paperCodeTitleTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.paperCodeTitleTable.Size = new System.Drawing.Size(142, 340);
            this.paperCodeTitleTable.TabIndex = 0;
            // 
            // finishPaperBtn
            // 
            this.finishPaperBtn.Appearance = Syncfusion.Windows.Forms.ButtonAppearance.Metro;
            this.finishPaperBtn.BackColor = System.Drawing.Color.DarkTurquoise;
            this.finishPaperBtn.BeforeTouchSize = new System.Drawing.Size(142, 45);
            this.finishPaperBtn.BorderStyleAdv = Syncfusion.Windows.Forms.ButtonAdvBorderStyle.Flat;
            this.paperCodeTitleTable.SetColumnSpan(this.finishPaperBtn, 2);
            this.finishPaperBtn.Dock = System.Windows.Forms.DockStyle.Top;
            this.finishPaperBtn.FlatAppearance.BorderSize = 0;
            this.finishPaperBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.finishPaperBtn.Font = new System.Drawing.Font("Dosis", 19F);
            this.finishPaperBtn.ForeColor = System.Drawing.Color.White;
            this.finishPaperBtn.Image = global::Synapse.Properties.Resources.Check;
            this.finishPaperBtn.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.finishPaperBtn.Location = new System.Drawing.Point(0, 294);
            this.finishPaperBtn.Margin = new System.Windows.Forms.Padding(0);
            this.finishPaperBtn.Name = "finishPaperBtn";
            this.finishPaperBtn.Padding = new System.Windows.Forms.Padding(15, 0, 0, 0);
            this.finishPaperBtn.Size = new System.Drawing.Size(142, 45);
            this.finishPaperBtn.TabIndex = 48;
            this.finishPaperBtn.Text = "   FINISH";
            this.finishPaperBtn.ThemeName = "Metro";
            this.finishPaperBtn.Click += new System.EventHandler(this.finishPaperBtn_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.paperCodeTitleTable.SetColumnSpan(this.tableLayoutPanel1, 2);
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.46729F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 49.53271F));
            this.tableLayoutPanel1.Controls.Add(this.paperFieldsCountField, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.paperOptionsCountField, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.autoLabel3, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.autoLabel4, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 199);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.paperCodeTitleTable.SetRowSpan(this.tableLayoutPanel1, 2);
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(136, 92);
            this.tableLayoutPanel1.TabIndex = 37;
            // 
            // paperFieldsCountField
            // 
            this.paperFieldsCountField.BackGroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.paperFieldsCountField.BeforeTouchSize = new System.Drawing.Size(72, 36);
            this.paperFieldsCountField.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(171)))), ((int)(((byte)(171)))), ((int)(((byte)(171)))));
            this.paperFieldsCountField.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.paperFieldsCountField.CornerRadius = 10;
            this.paperFieldsCountField.Dock = System.Windows.Forms.DockStyle.Fill;
            this.paperFieldsCountField.Font = new System.Drawing.Font("Dosis", 17F);
            this.paperFieldsCountField.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.paperFieldsCountField.IntegerValue = ((long)(1));
            this.paperFieldsCountField.Location = new System.Drawing.Point(3, 53);
            this.paperFieldsCountField.MaxValue = ((long)(9999));
            this.paperFieldsCountField.MinimumSize = new System.Drawing.Size(40, 36);
            this.paperFieldsCountField.MinValue = ((long)(1));
            this.paperFieldsCountField.Name = "paperFieldsCountField";
            this.paperFieldsCountField.PositiveColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.paperFieldsCountField.Size = new System.Drawing.Size(62, 36);
            this.paperFieldsCountField.Style = Syncfusion.Windows.Forms.Tools.TextBoxExt.theme.Office2016White;
            this.paperFieldsCountField.TabIndex = 34;
            this.paperFieldsCountField.Text = "1";
            this.paperFieldsCountField.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.paperFieldsCountField.ThemeName = "Office2016White";
            this.paperFieldsCountField.ZeroColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            // 
            // paperOptionsCountField
            // 
            this.paperOptionsCountField.BackGroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.paperOptionsCountField.BeforeTouchSize = new System.Drawing.Size(72, 36);
            this.paperOptionsCountField.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(171)))), ((int)(((byte)(171)))), ((int)(((byte)(171)))));
            this.paperOptionsCountField.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.paperOptionsCountField.CornerRadius = 10;
            this.paperOptionsCountField.Dock = System.Windows.Forms.DockStyle.Fill;
            this.paperOptionsCountField.Font = new System.Drawing.Font("Dosis", 17F);
            this.paperOptionsCountField.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.paperOptionsCountField.IntegerValue = ((long)(1));
            this.paperOptionsCountField.Location = new System.Drawing.Point(71, 53);
            this.paperOptionsCountField.MinimumSize = new System.Drawing.Size(40, 36);
            this.paperOptionsCountField.MinValue = ((long)(1));
            this.paperOptionsCountField.Name = "paperOptionsCountField";
            this.paperOptionsCountField.PositiveColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.paperOptionsCountField.Size = new System.Drawing.Size(62, 36);
            this.paperOptionsCountField.Style = Syncfusion.Windows.Forms.Tools.TextBoxExt.theme.Office2016White;
            this.paperOptionsCountField.TabIndex = 33;
            this.paperOptionsCountField.Text = "1";
            this.paperOptionsCountField.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.paperOptionsCountField.ThemeName = "Office2016White";
            this.paperOptionsCountField.ZeroColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            // 
            // autoLabel3
            // 
            this.autoLabel3.AutoSize = false;
            this.autoLabel3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.autoLabel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.autoLabel3.Font = new System.Drawing.Font("Dosis", 20.25F);
            this.autoLabel3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.autoLabel3.Location = new System.Drawing.Point(71, 3);
            this.autoLabel3.Margin = new System.Windows.Forms.Padding(3);
            this.autoLabel3.Name = "autoLabel3";
            this.autoLabel3.Size = new System.Drawing.Size(62, 44);
            this.autoLabel3.Style = Syncfusion.Windows.Forms.Tools.AutoLabelStyle.Office2016White;
            this.autoLabel3.TabIndex = 30;
            this.autoLabel3.Text = "Options:";
            this.autoLabel3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.autoLabel3.ThemeName = "Office2016White";
            // 
            // autoLabel4
            // 
            this.autoLabel4.AutoSize = false;
            this.autoLabel4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.autoLabel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.autoLabel4.Font = new System.Drawing.Font("Dosis", 20.25F);
            this.autoLabel4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.autoLabel4.Location = new System.Drawing.Point(3, 3);
            this.autoLabel4.Margin = new System.Windows.Forms.Padding(3);
            this.autoLabel4.Name = "autoLabel4";
            this.autoLabel4.Size = new System.Drawing.Size(62, 44);
            this.autoLabel4.Style = Syncfusion.Windows.Forms.Tools.AutoLabelStyle.Office2016White;
            this.autoLabel4.TabIndex = 28;
            this.autoLabel4.Text = "Fields:";
            this.autoLabel4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.autoLabel4.ThemeName = "Office2016White";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 1;
            this.paperCodeTitleTable.SetColumnSpan(this.tableLayoutPanel2, 2);
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.paperDirectionField, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.autoLabel8, 0, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 101);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.paperCodeTitleTable.SetRowSpan(this.tableLayoutPanel2, 2);
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(136, 92);
            this.tableLayoutPanel2.TabIndex = 36;
            // 
            // paperDirectionField
            // 
            this.paperDirectionField.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.paperDirectionField.BeforeTouchSize = new System.Drawing.Size(130, 37);
            this.paperDirectionField.Border3DStyle = System.Windows.Forms.Border3DStyle.Flat;
            this.paperDirectionField.BorderSides = System.Windows.Forms.Border3DSide.Bottom;
            this.paperDirectionField.CanOverrideStyle = true;
            this.paperDirectionField.Dock = System.Windows.Forms.DockStyle.Fill;
            this.paperDirectionField.FlatBorderColor = System.Drawing.Color.White;
            this.paperDirectionField.FlatStyle = Syncfusion.Windows.Forms.Tools.ComboFlatStyle.Flat;
            this.paperDirectionField.Font = new System.Drawing.Font("Dosis", 17.75F);
            this.paperDirectionField.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.paperDirectionField.Location = new System.Drawing.Point(3, 53);
            this.paperDirectionField.MetroBorderColor = System.Drawing.Color.White;
            this.paperDirectionField.Name = "paperDirectionField";
            this.paperDirectionField.Size = new System.Drawing.Size(130, 37);
            this.paperDirectionField.Style = Syncfusion.Windows.Forms.VisualStyle.Office2016White;
            this.paperDirectionField.TabIndex = 36;
            this.paperDirectionField.Text = "Paper Direction";
            this.paperDirectionField.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.paperDirectionField.ThemeName = "Office2016White";
            // 
            // autoLabel8
            // 
            this.autoLabel8.AutoSize = false;
            this.autoLabel8.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.autoLabel8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.autoLabel8.Font = new System.Drawing.Font("Dosis", 20.25F);
            this.autoLabel8.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.autoLabel8.Location = new System.Drawing.Point(3, 3);
            this.autoLabel8.Margin = new System.Windows.Forms.Padding(3);
            this.autoLabel8.Name = "autoLabel8";
            this.autoLabel8.Size = new System.Drawing.Size(130, 44);
            this.autoLabel8.Style = Syncfusion.Windows.Forms.Tools.AutoLabelStyle.Office2016White;
            this.autoLabel8.TabIndex = 28;
            this.autoLabel8.Text = "Direction:";
            this.autoLabel8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.autoLabel8.ThemeName = "Office2016White";
            // 
            // autoLabel2
            // 
            this.autoLabel2.AutoSize = false;
            this.autoLabel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.autoLabel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.autoLabel2.Font = new System.Drawing.Font("Dosis", 20.25F);
            this.autoLabel2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.autoLabel2.Location = new System.Drawing.Point(71, 3);
            this.autoLabel2.Margin = new System.Windows.Forms.Padding(3);
            this.autoLabel2.Name = "autoLabel2";
            this.autoLabel2.Size = new System.Drawing.Size(68, 43);
            this.autoLabel2.Style = Syncfusion.Windows.Forms.Tools.AutoLabelStyle.Office2016White;
            this.autoLabel2.TabIndex = 30;
            this.autoLabel2.Text = "Title:";
            this.autoLabel2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.autoLabel2.ThemeName = "Office2016White";
            // 
            // autoLabel1
            // 
            this.autoLabel1.AutoSize = false;
            this.autoLabel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.autoLabel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.autoLabel1.Font = new System.Drawing.Font("Dosis", 20.25F);
            this.autoLabel1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.autoLabel1.Location = new System.Drawing.Point(3, 3);
            this.autoLabel1.Margin = new System.Windows.Forms.Padding(3);
            this.autoLabel1.Name = "autoLabel1";
            this.autoLabel1.Size = new System.Drawing.Size(62, 43);
            this.autoLabel1.Style = Syncfusion.Windows.Forms.Tools.AutoLabelStyle.Office2016White;
            this.autoLabel1.TabIndex = 28;
            this.autoLabel1.Text = "Code:";
            this.autoLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.autoLabel1.ThemeName = "Office2016White";
            // 
            // paperTitleField
            // 
            this.paperTitleField.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.paperTitleField.BeforeTouchSize = new System.Drawing.Size(72, 36);
            this.paperTitleField.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(171)))), ((int)(((byte)(171)))), ((int)(((byte)(171)))));
            this.paperTitleField.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.paperTitleField.CornerRadius = 10;
            this.paperTitleField.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.paperTitleField.Dock = System.Windows.Forms.DockStyle.Fill;
            this.paperTitleField.Font = new System.Drawing.Font("Dosis", 17F);
            this.paperTitleField.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.paperTitleField.Location = new System.Drawing.Point(71, 52);
            this.paperTitleField.MinimumSize = new System.Drawing.Size(40, 36);
            this.paperTitleField.Name = "paperTitleField";
            this.paperTitleField.Size = new System.Drawing.Size(68, 36);
            this.paperTitleField.Style = Syncfusion.Windows.Forms.Tools.TextBoxExt.theme.Office2016White;
            this.paperTitleField.TabIndex = 33;
            this.paperTitleField.Text = "The Paper";
            this.paperTitleField.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.paperTitleField.ThemeName = "Office2016White";
            // 
            // papersListTable
            // 
            this.papersListTable.ColumnCount = 1;
            this.papersListTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.papersListTable.Controls.Add(this.addNewPaperBtn, 0, 1);
            this.papersListTable.Controls.Add(this.containerFlowPanel, 0, 0);
            this.papersListTable.Location = new System.Drawing.Point(5, 2);
            this.papersListTable.Name = "papersListTable";
            this.papersListTable.Padding = new System.Windows.Forms.Padding(3);
            this.papersListTable.RowCount = 2;
            this.papersListTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.papersListTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 45F));
            this.papersListTable.Size = new System.Drawing.Size(494, 346);
            this.papersListTable.TabIndex = 2;
            // 
            // addNewPaperBtn
            // 
            this.addNewPaperBtn.Appearance = Syncfusion.Windows.Forms.ButtonAppearance.Metro;
            this.addNewPaperBtn.BackColor = System.Drawing.Color.DodgerBlue;
            this.addNewPaperBtn.BeforeTouchSize = new System.Drawing.Size(488, 45);
            this.addNewPaperBtn.BorderStyleAdv = Syncfusion.Windows.Forms.ButtonAdvBorderStyle.Flat;
            this.papersListTable.SetColumnSpan(this.addNewPaperBtn, 2);
            this.addNewPaperBtn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.addNewPaperBtn.FlatAppearance.BorderSize = 0;
            this.addNewPaperBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.addNewPaperBtn.Font = new System.Drawing.Font("Dosis", 19F);
            this.addNewPaperBtn.ForeColor = System.Drawing.Color.White;
            this.addNewPaperBtn.Image = global::Synapse.Properties.Resources.Add_New;
            this.addNewPaperBtn.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.addNewPaperBtn.Location = new System.Drawing.Point(3, 298);
            this.addNewPaperBtn.Margin = new System.Windows.Forms.Padding(0);
            this.addNewPaperBtn.Name = "addNewPaperBtn";
            this.addNewPaperBtn.Padding = new System.Windows.Forms.Padding(15, 0, 0, 0);
            this.addNewPaperBtn.Size = new System.Drawing.Size(488, 45);
            this.addNewPaperBtn.TabIndex = 48;
            this.addNewPaperBtn.Text = "   CREATE NEW";
            this.addNewPaperBtn.ThemeName = "Metro";
            this.addNewPaperBtn.Click += new System.EventHandler(this.addNewPaperBtn_Click);
            // 
            // paperCodeField
            // 
            this.paperCodeField.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.paperCodeField.BeforeTouchSize = new System.Drawing.Size(72, 36);
            this.paperCodeField.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(171)))), ((int)(((byte)(171)))), ((int)(((byte)(171)))));
            this.paperCodeField.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.paperCodeField.CornerRadius = 10;
            this.paperCodeField.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.paperCodeField.Dock = System.Windows.Forms.DockStyle.Fill;
            this.paperCodeField.Font = new System.Drawing.Font("Dosis", 17F);
            this.paperCodeField.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.paperCodeField.Location = new System.Drawing.Point(-7, 54);
            this.paperCodeField.MinimumSize = new System.Drawing.Size(40, 36);
            this.paperCodeField.Name = "paperCodeField";
            this.paperCodeField.Size = new System.Drawing.Size(72, 36);
            this.paperCodeField.Style = Syncfusion.Windows.Forms.Tools.TextBoxExt.theme.Office2016White;
            this.paperCodeField.TabIndex = 34;
            this.paperCodeField.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.paperCodeField.ThemeName = "Office2016White";
            // 
            // ExamPapersConfigurationForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.ClientSize = new System.Drawing.Size(711, 358);
            this.Controls.Add(this.paperConfigurationPanel);
            this.Controls.Add(this.papersListTable);
            this.Font = new System.Drawing.Font("Dosis", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ExamPapersConfigurationForm";
            this.Padding = new System.Windows.Forms.Padding(2, 2, 2, 5);
            this.ShowToolTip = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Style.InactiveShadowOpacity = ((byte)(50));
            this.Style.MdiChild.IconHorizontalAlignment = System.Windows.Forms.HorizontalAlignment.Center;
            this.Style.MdiChild.IconVerticalAlignment = System.Windows.Forms.VisualStyles.VerticalAlignment.Center;
            this.Style.ShadowOpacity = ((byte)(0));
            this.Style.TitleBar.BackColor = System.Drawing.Color.White;
            this.Style.TitleBar.Font = new System.Drawing.Font("Dosis", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Style.TitleBar.TextHorizontalAlignment = System.Windows.Forms.HorizontalAlignment.Center;
            this.Text = "Configure Exam Papers";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DataConfigurationForm_FormClosing);
            this.containerFlowPanel.ResumeLayout(false);
            this.paperConfigurationPanel.ResumeLayout(false);
            this.paperCodeTitleTable.ResumeLayout(false);
            this.paperCodeTitleTable.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.paperFieldsCountField)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.paperOptionsCountField)).EndInit();
            this.tableLayoutPanel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.paperDirectionField)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.paperTitleField)).EndInit();
            this.papersListTable.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.paperCodeField)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        private System.Windows.Forms.FlowLayoutPanel containerFlowPanel;
        private System.Windows.Forms.Panel paperConfigurationPanel;
        private System.Windows.Forms.TableLayoutPanel paperCodeTitleTable;
        private Syncfusion.Windows.Forms.Tools.AutoLabel autoLabel1;
        private Syncfusion.Windows.Forms.Tools.AutoLabel autoLabel2;
        private Syncfusion.Windows.Forms.Tools.TextBoxExt paperTitleField;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private Syncfusion.Windows.Forms.Tools.AutoLabel autoLabel8;
        private Syncfusion.Windows.Forms.Tools.ComboBoxAdv paperDirectionField;
        private System.Windows.Forms.TableLayoutPanel papersListTable;
        private Syncfusion.Windows.Forms.ButtonAdv addNewPaperBtn;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private Syncfusion.Windows.Forms.Tools.IntegerTextBox paperFieldsCountField;
        private Syncfusion.Windows.Forms.Tools.IntegerTextBox paperOptionsCountField;
        private Syncfusion.Windows.Forms.Tools.AutoLabel autoLabel3;
        private Syncfusion.Windows.Forms.Tools.AutoLabel autoLabel4;
        private Syncfusion.Windows.Forms.ButtonAdv finishPaperBtn;
        private Syncfusion.Windows.Forms.Tools.AutoLabel emptyListLabel;
        private Syncfusion.Windows.Forms.Tools.TextBoxExt paperCodeField;
    }
}