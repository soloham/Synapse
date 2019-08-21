namespace Synapse.Modules
{
    partial class AlignmentPipelineConfigurationForm
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
            this.comboBoxStateComboBox = new Syncfusion.Windows.Forms.Tools.ComboBoxAdv();
            this.addMethodBtn = new Syncfusion.Windows.Forms.ButtonAdv();
            this.containerFlowPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.emptyListLabel = new Syncfusion.Windows.Forms.Tools.AutoLabel();
            this.createMethodNameTextBox = new Syncfusion.Windows.Forms.Tools.TextBoxExt();
            this.configControlsPanel = new System.Windows.Forms.TableLayoutPanel();
            this.addNewMethodBtn = new Syncfusion.Windows.Forms.ButtonAdv();
            this.finishBtn = new Syncfusion.Windows.Forms.ButtonAdv();
            this.mainTablePanel.SuspendLayout();
            this.ComboBoxStatePanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.comboBoxStateComboBox)).BeginInit();
            this.containerFlowPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.createMethodNameTextBox)).BeginInit();
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
            this.mainTablePanel.Location = new System.Drawing.Point(0, 0);
            this.mainTablePanel.Name = "mainTablePanel";
            this.mainTablePanel.Padding = new System.Windows.Forms.Padding(8, 0, 8, 5);
            this.mainTablePanel.RowCount = 3;
            this.mainTablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.mainTablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mainTablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.mainTablePanel.Size = new System.Drawing.Size(486, 293);
            this.mainTablePanel.TabIndex = 0;
            // 
            // ComboBoxStatePanel
            // 
            this.ComboBoxStatePanel.Controls.Add(this.createMethodNameTextBox);
            this.ComboBoxStatePanel.Controls.Add(this.comboBoxStateComboBox);
            this.ComboBoxStatePanel.Controls.Add(this.addMethodBtn);
            this.ComboBoxStatePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ComboBoxStatePanel.Location = new System.Drawing.Point(8, 0);
            this.ComboBoxStatePanel.Margin = new System.Windows.Forms.Padding(0);
            this.ComboBoxStatePanel.Name = "ComboBoxStatePanel";
            this.ComboBoxStatePanel.Size = new System.Drawing.Size(470, 40);
            this.ComboBoxStatePanel.TabIndex = 41;
            this.ComboBoxStatePanel.Visible = false;
            // 
            // comboBoxStateComboBox
            // 
            this.comboBoxStateComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxStateComboBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.comboBoxStateComboBox.BeforeTouchSize = new System.Drawing.Size(152, 37);
            this.comboBoxStateComboBox.Border3DStyle = System.Windows.Forms.Border3DStyle.Flat;
            this.comboBoxStateComboBox.FlatBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(171)))), ((int)(((byte)(171)))), ((int)(((byte)(171)))));
            this.comboBoxStateComboBox.FlatStyle = Syncfusion.Windows.Forms.Tools.ComboFlatStyle.Flat;
            this.comboBoxStateComboBox.Font = new System.Drawing.Font("Dosis", 16.9F);
            this.comboBoxStateComboBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.comboBoxStateComboBox.Location = new System.Drawing.Point(0, 2);
            this.comboBoxStateComboBox.MetroBorderColor = System.Drawing.Color.White;
            this.comboBoxStateComboBox.Name = "comboBoxStateComboBox";
            this.comboBoxStateComboBox.Size = new System.Drawing.Size(152, 37);
            this.comboBoxStateComboBox.Style = Syncfusion.Windows.Forms.VisualStyle.Office2016White;
            this.comboBoxStateComboBox.TabIndex = 0;
            this.comboBoxStateComboBox.Text = "Combo Box";
            this.comboBoxStateComboBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.comboBoxStateComboBox.ThemeName = "Office2016White";
            // 
            // addMethodBtn
            // 
            this.addMethodBtn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.addMethodBtn.Appearance = Syncfusion.Windows.Forms.ButtonAppearance.Metro;
            this.addMethodBtn.BackColor = System.Drawing.Color.MediumTurquoise;
            this.addMethodBtn.BeforeTouchSize = new System.Drawing.Size(59, 37);
            this.addMethodBtn.BorderStyleAdv = Syncfusion.Windows.Forms.ButtonAdvBorderStyle.Flat;
            this.addMethodBtn.FlatAppearance.BorderSize = 0;
            this.addMethodBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.addMethodBtn.Font = new System.Drawing.Font("Dosis", 19F);
            this.addMethodBtn.ForeColor = System.Drawing.Color.White;
            this.addMethodBtn.Image = global::Synapse.Properties.Resources.Add_New;
            this.addMethodBtn.Location = new System.Drawing.Point(407, 2);
            this.addMethodBtn.Margin = new System.Windows.Forms.Padding(0);
            this.addMethodBtn.Name = "addMethodBtn";
            this.addMethodBtn.Size = new System.Drawing.Size(59, 37);
            this.addMethodBtn.TabIndex = 39;
            this.addMethodBtn.ThemeName = "Metro";
            this.addMethodBtn.Click += new System.EventHandler(this.AddMethodBtn_Click);
            // 
            // containerFlowPanel
            // 
            this.containerFlowPanel.Controls.Add(this.emptyListLabel);
            this.containerFlowPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.containerFlowPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.containerFlowPanel.Location = new System.Drawing.Point(9, 41);
            this.containerFlowPanel.Margin = new System.Windows.Forms.Padding(1, 1, 3, 1);
            this.containerFlowPanel.Name = "containerFlowPanel";
            this.containerFlowPanel.Size = new System.Drawing.Size(466, 186);
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
            this.emptyListLabel.Text = "You currently have no alignment methods,\r\nCreate one to continue";
            this.emptyListLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.emptyListLabel.ThemeName = "Office2016White";
            this.emptyListLabel.Visible = false;
            // 
            // createMethodNameTextBox
            // 
            this.createMethodNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.createMethodNameTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.createMethodNameTextBox.BeforeTouchSize = new System.Drawing.Size(257, 37);
            this.createMethodNameTextBox.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(171)))), ((int)(((byte)(171)))), ((int)(((byte)(171)))));
            this.createMethodNameTextBox.BorderSides = ((System.Windows.Forms.Border3DSide)(((System.Windows.Forms.Border3DSide.Left | System.Windows.Forms.Border3DSide.Top) 
            | System.Windows.Forms.Border3DSide.Bottom)));
            this.createMethodNameTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.createMethodNameTextBox.Font = new System.Drawing.Font("Dosis", 17.75F);
            this.createMethodNameTextBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.createMethodNameTextBox.Location = new System.Drawing.Point(150, 2);
            this.createMethodNameTextBox.Margin = new System.Windows.Forms.Padding(5, 0, 0, 8);
            this.createMethodNameTextBox.MinimumSize = new System.Drawing.Size(1, 1);
            this.createMethodNameTextBox.Name = "createMethodNameTextBox";
            this.createMethodNameTextBox.Size = new System.Drawing.Size(257, 37);
            this.createMethodNameTextBox.Style = Syncfusion.Windows.Forms.Tools.TextBoxExt.theme.Office2016White;
            this.createMethodNameTextBox.TabIndex = 40;
            this.createMethodNameTextBox.Text = "New Method";
            this.createMethodNameTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.createMethodNameTextBox.ThemeName = "Office2016White";
            this.createMethodNameTextBox.ThemeStyle.BorderColor = System.Drawing.Color.Gainsboro;
            this.createMethodNameTextBox.ThemeStyle.CornerRadius = 0;
            // 
            // configControlsPanel
            // 
            this.configControlsPanel.ColumnCount = 2;
            this.configControlsPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.configControlsPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.configControlsPanel.Controls.Add(this.addNewMethodBtn, 0, 0);
            this.configControlsPanel.Controls.Add(this.finishBtn, 1, 0);
            this.configControlsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.configControlsPanel.Location = new System.Drawing.Point(11, 231);
            this.configControlsPanel.Name = "configControlsPanel";
            this.configControlsPanel.RowCount = 1;
            this.configControlsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.configControlsPanel.Size = new System.Drawing.Size(464, 54);
            this.configControlsPanel.TabIndex = 42;
            // 
            // addNewMethodBtn
            // 
            this.addNewMethodBtn.Appearance = Syncfusion.Windows.Forms.ButtonAppearance.Metro;
            this.addNewMethodBtn.BackColor = System.Drawing.Color.DodgerBlue;
            this.addNewMethodBtn.BeforeTouchSize = new System.Drawing.Size(226, 48);
            this.addNewMethodBtn.BorderStyleAdv = Syncfusion.Windows.Forms.ButtonAdvBorderStyle.Flat;
            this.addNewMethodBtn.FlatAppearance.BorderSize = 0;
            this.addNewMethodBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.addNewMethodBtn.Font = new System.Drawing.Font("Dosis", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.addNewMethodBtn.ForeColor = System.Drawing.Color.White;
            this.addNewMethodBtn.Image = global::Synapse.Properties.Resources.Add_New;
            this.addNewMethodBtn.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.addNewMethodBtn.Location = new System.Drawing.Point(3, 3);
            this.addNewMethodBtn.Name = "addNewMethodBtn";
            this.addNewMethodBtn.Padding = new System.Windows.Forms.Padding(10, 0, 10, 0);
            this.addNewMethodBtn.Size = new System.Drawing.Size(226, 48);
            this.addNewMethodBtn.TabIndex = 39;
            this.addNewMethodBtn.Text = "       ADD METHOD";
            this.addNewMethodBtn.ThemeName = "Metro";
            this.addNewMethodBtn.ThemeStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.addNewMethodBtn.ThemeStyle.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(171)))), ((int)(((byte)(171)))), ((int)(((byte)(171)))));
            this.addNewMethodBtn.ThemeStyle.DisabledBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(243)))), ((int)(((byte)(243)))));
            this.addNewMethodBtn.ThemeStyle.DisabledBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
            this.addNewMethodBtn.ThemeStyle.DisabledForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(171)))), ((int)(((byte)(171)))), ((int)(((byte)(171)))));
            this.addNewMethodBtn.ThemeStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.addNewMethodBtn.ThemeStyle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.addNewMethodBtn.ThemeStyle.HoverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(236)))), ((int)(((byte)(249)))));
            this.addNewMethodBtn.ThemeStyle.HoverBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(113)))), ((int)(((byte)(175)))), ((int)(((byte)(229)))));
            this.addNewMethodBtn.ThemeStyle.HoverForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.addNewMethodBtn.ThemeStyle.PressedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(205)))), ((int)(((byte)(230)))), ((int)(((byte)(247)))));
            this.addNewMethodBtn.ThemeStyle.PressedBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(212)))));
            this.addNewMethodBtn.ThemeStyle.PressedForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.addNewMethodBtn.UseVisualStyle = false;
            this.addNewMethodBtn.UseVisualStyleBackColor = false;
            this.addNewMethodBtn.Click += new System.EventHandler(this.AddNewMethodBtn_Click);
            // 
            // finishBtn
            // 
            this.finishBtn.Appearance = Syncfusion.Windows.Forms.ButtonAppearance.Metro;
            this.finishBtn.BackColor = System.Drawing.Color.MediumAquamarine;
            this.finishBtn.BeforeTouchSize = new System.Drawing.Size(226, 48);
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
            this.finishBtn.Size = new System.Drawing.Size(226, 48);
            this.finishBtn.TabIndex = 40;
            this.finishBtn.Text = "   FINISH";
            this.finishBtn.ThemeName = "Metro";
            this.finishBtn.Click += new System.EventHandler(this.FinishBtn_Click);
            // 
            // AlignmentPipelineConfigurationForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.ClientSize = new System.Drawing.Size(486, 293);
            this.Controls.Add(this.mainTablePanel);
            this.Font = new System.Drawing.Font("Dosis", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(740, 412);
            this.MinimizeBox = false;
            this.Name = "AlignmentPipelineConfigurationForm";
            this.Padding = new System.Windows.Forms.Padding(0);
            this.ShowToolTip = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Style.InactiveShadowOpacity = ((byte)(50));
            this.Style.ShadowOpacity = ((byte)(0));
            this.Style.TitleBar.BackColor = System.Drawing.Color.White;
            this.Style.TitleBar.Font = new System.Drawing.Font("Dosis", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Style.TitleBar.TextHorizontalAlignment = System.Windows.Forms.HorizontalAlignment.Center;
            this.Text = "Configure Alignment Pipeline";
            this.mainTablePanel.ResumeLayout(false);
            this.ComboBoxStatePanel.ResumeLayout(false);
            this.ComboBoxStatePanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.comboBoxStateComboBox)).EndInit();
            this.containerFlowPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.createMethodNameTextBox)).EndInit();
            this.configControlsPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        private System.Windows.Forms.TableLayoutPanel mainTablePanel;
        private System.Windows.Forms.FlowLayoutPanel containerFlowPanel;
        private Syncfusion.Windows.Forms.Tools.AutoLabel emptyListLabel;
        private Syncfusion.Windows.Forms.ButtonAdv addMethodBtn;
        private System.Windows.Forms.Panel ComboBoxStatePanel;
        private Syncfusion.Windows.Forms.Tools.ComboBoxAdv comboBoxStateComboBox;
        private Syncfusion.Windows.Forms.Tools.TextBoxExt createMethodNameTextBox;
        private System.Windows.Forms.TableLayoutPanel configControlsPanel;
        private Syncfusion.Windows.Forms.ButtonAdv addNewMethodBtn;
        private Syncfusion.Windows.Forms.ButtonAdv finishBtn;
    }
}