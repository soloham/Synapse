namespace Synapse.Modules
{
    partial class AddFieldsForm
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
            this.mainTablePanel = new System.Windows.Forms.TableLayoutPanel();
            this.ComboBoxStatePanel = new System.Windows.Forms.Panel();
            this.newFieldNameTextBox = new Syncfusion.Windows.Forms.Tools.TextBoxExt();
            this.newFieldType = new Syncfusion.Windows.Forms.Tools.ComboBoxAdv();
            this.addFieldBtn = new Syncfusion.Windows.Forms.ButtonAdv();
            this.containerFlowPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.emptyListLabel = new Syncfusion.Windows.Forms.Tools.AutoLabel();
            this.configControlsPanel = new System.Windows.Forms.TableLayoutPanel();
            this.addNewFieldBtn = new Syncfusion.Windows.Forms.ButtonAdv();
            this.finishBtn = new Syncfusion.Windows.Forms.ButtonAdv();
            this.mainTablePanel.SuspendLayout();
            this.ComboBoxStatePanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.newFieldNameTextBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.newFieldType)).BeginInit();
            this.containerFlowPanel.SuspendLayout();
            this.configControlsPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainTablePanel
            // 
            this.mainTablePanel.ColumnCount = 1;
            this.mainTablePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mainTablePanel.Controls.Add(this.ComboBoxStatePanel, 0, 0);
            this.mainTablePanel.Controls.Add(this.containerFlowPanel, 0, 1);
            this.mainTablePanel.Controls.Add(this.configControlsPanel, 0, 2);
            this.mainTablePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainTablePanel.Location = new System.Drawing.Point(8, 2);
            this.mainTablePanel.Name = "mainTablePanel";
            this.mainTablePanel.Padding = new System.Windows.Forms.Padding(8, 0, 8, 5);
            this.mainTablePanel.RowCount = 3;
            this.mainTablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.mainTablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mainTablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.mainTablePanel.Size = new System.Drawing.Size(487, 286);
            this.mainTablePanel.TabIndex = 1;
            // 
            // ComboBoxStatePanel
            // 
            this.ComboBoxStatePanel.Controls.Add(this.newFieldNameTextBox);
            this.ComboBoxStatePanel.Controls.Add(this.newFieldType);
            this.ComboBoxStatePanel.Controls.Add(this.addFieldBtn);
            this.ComboBoxStatePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ComboBoxStatePanel.Location = new System.Drawing.Point(8, 0);
            this.ComboBoxStatePanel.Margin = new System.Windows.Forms.Padding(0);
            this.ComboBoxStatePanel.Name = "ComboBoxStatePanel";
            this.ComboBoxStatePanel.Size = new System.Drawing.Size(471, 40);
            this.ComboBoxStatePanel.TabIndex = 41;
            this.ComboBoxStatePanel.Visible = false;
            // 
            // newFieldNameTextBox
            // 
            this.newFieldNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.newFieldNameTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.newFieldNameTextBox.BeforeTouchSize = new System.Drawing.Size(257, 37);
            this.newFieldNameTextBox.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(171)))), ((int)(((byte)(171)))), ((int)(((byte)(171)))));
            this.newFieldNameTextBox.BorderSides = ((System.Windows.Forms.Border3DSide)(((System.Windows.Forms.Border3DSide.Left | System.Windows.Forms.Border3DSide.Top) 
            | System.Windows.Forms.Border3DSide.Bottom)));
            this.newFieldNameTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.newFieldNameTextBox.Font = new System.Drawing.Font("Dosis", 17.75F);
            this.newFieldNameTextBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.newFieldNameTextBox.Location = new System.Drawing.Point(151, 2);
            this.newFieldNameTextBox.Margin = new System.Windows.Forms.Padding(5, 0, 0, 8);
            this.newFieldNameTextBox.MinimumSize = new System.Drawing.Size(1, 1);
            this.newFieldNameTextBox.Name = "newFieldNameTextBox";
            this.newFieldNameTextBox.Size = new System.Drawing.Size(257, 37);
            this.newFieldNameTextBox.Style = Syncfusion.Windows.Forms.Tools.TextBoxExt.theme.Office2016White;
            this.newFieldNameTextBox.TabIndex = 40;
            this.newFieldNameTextBox.Text = "New Method";
            this.newFieldNameTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.newFieldNameTextBox.ThemeName = "Office2016White";
            this.newFieldNameTextBox.ThemeStyle.BorderColor = System.Drawing.Color.Gainsboro;
            this.newFieldNameTextBox.ThemeStyle.CornerRadius = 0;
            // 
            // newFieldType
            // 
            this.newFieldType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.newFieldType.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.newFieldType.BeforeTouchSize = new System.Drawing.Size(153, 37);
            this.newFieldType.Border3DStyle = System.Windows.Forms.Border3DStyle.Flat;
            this.newFieldType.FlatBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(171)))), ((int)(((byte)(171)))), ((int)(((byte)(171)))));
            this.newFieldType.FlatStyle = Syncfusion.Windows.Forms.Tools.ComboFlatStyle.Flat;
            this.newFieldType.Font = new System.Drawing.Font("Dosis", 16.9F);
            this.newFieldType.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.newFieldType.Location = new System.Drawing.Point(0, 2);
            this.newFieldType.MetroBorderColor = System.Drawing.Color.White;
            this.newFieldType.Name = "newFieldType";
            this.newFieldType.Size = new System.Drawing.Size(153, 37);
            this.newFieldType.Style = Syncfusion.Windows.Forms.VisualStyle.Office2016White;
            this.newFieldType.TabIndex = 0;
            this.newFieldType.Text = "Combo Box";
            this.newFieldType.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.newFieldType.ThemeName = "Office2016White";
            // 
            // addFieldBtn
            // 
            this.addFieldBtn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.addFieldBtn.Appearance = Syncfusion.Windows.Forms.ButtonAppearance.Metro;
            this.addFieldBtn.BackColor = System.Drawing.Color.MediumTurquoise;
            this.addFieldBtn.BeforeTouchSize = new System.Drawing.Size(59, 37);
            this.addFieldBtn.BorderStyleAdv = Syncfusion.Windows.Forms.ButtonAdvBorderStyle.Flat;
            this.addFieldBtn.FlatAppearance.BorderSize = 0;
            this.addFieldBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.addFieldBtn.Font = new System.Drawing.Font("Dosis", 19F);
            this.addFieldBtn.ForeColor = System.Drawing.Color.White;
            this.addFieldBtn.Image = global::Synapse.Properties.Resources.Add_New;
            this.addFieldBtn.Location = new System.Drawing.Point(408, 2);
            this.addFieldBtn.Margin = new System.Windows.Forms.Padding(0);
            this.addFieldBtn.Name = "addFieldBtn";
            this.addFieldBtn.Size = new System.Drawing.Size(59, 37);
            this.addFieldBtn.TabIndex = 39;
            this.addFieldBtn.ThemeName = "Metro";
            this.addFieldBtn.Click += new System.EventHandler(this.addFieldBtn_Click);
            // 
            // containerFlowPanel
            // 
            this.containerFlowPanel.Controls.Add(this.emptyListLabel);
            this.containerFlowPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.containerFlowPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.containerFlowPanel.Location = new System.Drawing.Point(9, 41);
            this.containerFlowPanel.Margin = new System.Windows.Forms.Padding(1, 1, 3, 1);
            this.containerFlowPanel.Name = "containerFlowPanel";
            this.containerFlowPanel.Size = new System.Drawing.Size(467, 179);
            this.containerFlowPanel.TabIndex = 1;
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
            this.emptyListLabel.Size = new System.Drawing.Size(468, 187);
            this.emptyListLabel.Style = Syncfusion.Windows.Forms.Tools.AutoLabelStyle.Office2016White;
            this.emptyListLabel.TabIndex = 1;
            this.emptyListLabel.Text = "You currently have no fields,\r\nCreate one to continue";
            this.emptyListLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.emptyListLabel.ThemeName = "Office2016White";
            this.emptyListLabel.Visible = false;
            // 
            // configControlsPanel
            // 
            this.configControlsPanel.ColumnCount = 2;
            this.configControlsPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.configControlsPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.configControlsPanel.Controls.Add(this.addNewFieldBtn, 0, 0);
            this.configControlsPanel.Controls.Add(this.finishBtn, 1, 0);
            this.configControlsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.configControlsPanel.Location = new System.Drawing.Point(11, 224);
            this.configControlsPanel.Name = "configControlsPanel";
            this.configControlsPanel.RowCount = 1;
            this.configControlsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.configControlsPanel.Size = new System.Drawing.Size(465, 54);
            this.configControlsPanel.TabIndex = 42;
            // 
            // addNewFieldBtn
            // 
            this.addNewFieldBtn.Appearance = Syncfusion.Windows.Forms.ButtonAppearance.Metro;
            this.addNewFieldBtn.BackColor = System.Drawing.Color.DodgerBlue;
            this.addNewFieldBtn.BeforeTouchSize = new System.Drawing.Size(226, 48);
            this.addNewFieldBtn.BorderStyleAdv = Syncfusion.Windows.Forms.ButtonAdvBorderStyle.Flat;
            this.addNewFieldBtn.FlatAppearance.BorderSize = 0;
            this.addNewFieldBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.addNewFieldBtn.Font = new System.Drawing.Font("Dosis", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.addNewFieldBtn.ForeColor = System.Drawing.Color.White;
            this.addNewFieldBtn.Image = global::Synapse.Properties.Resources.Add_New;
            this.addNewFieldBtn.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.addNewFieldBtn.Location = new System.Drawing.Point(3, 3);
            this.addNewFieldBtn.Name = "addNewFieldBtn";
            this.addNewFieldBtn.Padding = new System.Windows.Forms.Padding(10, 0, 10, 0);
            this.addNewFieldBtn.Size = new System.Drawing.Size(226, 48);
            this.addNewFieldBtn.TabIndex = 39;
            this.addNewFieldBtn.Text = "       ADD FIELD";
            this.addNewFieldBtn.ThemeName = "Metro";
            this.addNewFieldBtn.ThemeStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.addNewFieldBtn.ThemeStyle.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(171)))), ((int)(((byte)(171)))), ((int)(((byte)(171)))));
            this.addNewFieldBtn.ThemeStyle.DisabledBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(243)))), ((int)(((byte)(243)))));
            this.addNewFieldBtn.ThemeStyle.DisabledBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
            this.addNewFieldBtn.ThemeStyle.DisabledForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(171)))), ((int)(((byte)(171)))), ((int)(((byte)(171)))));
            this.addNewFieldBtn.ThemeStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.addNewFieldBtn.ThemeStyle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.addNewFieldBtn.ThemeStyle.HoverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(236)))), ((int)(((byte)(249)))));
            this.addNewFieldBtn.ThemeStyle.HoverBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(113)))), ((int)(((byte)(175)))), ((int)(((byte)(229)))));
            this.addNewFieldBtn.ThemeStyle.HoverForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.addNewFieldBtn.ThemeStyle.PressedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(205)))), ((int)(((byte)(230)))), ((int)(((byte)(247)))));
            this.addNewFieldBtn.ThemeStyle.PressedBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(212)))));
            this.addNewFieldBtn.ThemeStyle.PressedForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.addNewFieldBtn.UseVisualStyle = false;
            this.addNewFieldBtn.UseVisualStyleBackColor = false;
            // 
            // finishBtn
            // 
            this.finishBtn.Appearance = Syncfusion.Windows.Forms.ButtonAppearance.Metro;
            this.finishBtn.BackColor = System.Drawing.Color.MediumAquamarine;
            this.finishBtn.BeforeTouchSize = new System.Drawing.Size(227, 48);
            this.finishBtn.BorderStyleAdv = Syncfusion.Windows.Forms.ButtonAdvBorderStyle.Flat;
            this.finishBtn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.finishBtn.FlatAppearance.BorderSize = 0;
            this.finishBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.finishBtn.Font = new System.Drawing.Font("Dosis", 20.25F);
            this.finishBtn.ForeColor = System.Drawing.Color.White;
            this.finishBtn.Image = global::Synapse.Properties.Resources.Check;
            this.finishBtn.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.finishBtn.Location = new System.Drawing.Point(235, 3);
            this.finishBtn.Name = "finishBtn";
            this.finishBtn.Padding = new System.Windows.Forms.Padding(10, 0, 10, 0);
            this.finishBtn.Size = new System.Drawing.Size(227, 48);
            this.finishBtn.TabIndex = 40;
            this.finishBtn.Text = "   FINISH";
            this.finishBtn.ThemeName = "Metro";
            // 
            // AddFieldsForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.ClientSize = new System.Drawing.Size(497, 293);
            this.Controls.Add(this.mainTablePanel);
            this.Font = new System.Drawing.Font("Dosis", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(740, 412);
            this.MinimizeBox = false;
            this.Name = "AddFieldsForm";
            this.Padding = new System.Windows.Forms.Padding(8, 2, 2, 5);
            this.ShowToolTip = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Style.InactiveShadowOpacity = ((byte)(50));
            this.Style.ShadowOpacity = ((byte)(0));
            this.Style.TitleBar.BackColor = System.Drawing.Color.White;
            this.Style.TitleBar.Font = new System.Drawing.Font("Dosis", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Style.TitleBar.TextHorizontalAlignment = System.Windows.Forms.HorizontalAlignment.Center;
            this.Text = "Add Fields";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DataConfigurationForm_FormClosing);
            this.mainTablePanel.ResumeLayout(false);
            this.ComboBoxStatePanel.ResumeLayout(false);
            this.ComboBoxStatePanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.newFieldNameTextBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.newFieldType)).EndInit();
            this.containerFlowPanel.ResumeLayout(false);
            this.configControlsPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        private System.Windows.Forms.TableLayoutPanel mainTablePanel;
        private System.Windows.Forms.Panel ComboBoxStatePanel;
        private Syncfusion.Windows.Forms.Tools.TextBoxExt newFieldNameTextBox;
        private Syncfusion.Windows.Forms.Tools.ComboBoxAdv newFieldType;
        private Syncfusion.Windows.Forms.ButtonAdv addFieldBtn;
        private System.Windows.Forms.FlowLayoutPanel containerFlowPanel;
        private Syncfusion.Windows.Forms.Tools.AutoLabel emptyListLabel;
        private System.Windows.Forms.TableLayoutPanel configControlsPanel;
        private Syncfusion.Windows.Forms.ButtonAdv addNewFieldBtn;
        private Syncfusion.Windows.Forms.ButtonAdv finishBtn;
    }
}