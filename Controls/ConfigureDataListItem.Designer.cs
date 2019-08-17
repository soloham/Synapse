namespace Synapse.Controls
{
    partial class ConfigureDataListItem
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigureDataListItem));
            this.containerTable = new System.Windows.Forms.TableLayoutPanel();
            this.configNameLabel = new Synapse.Controls.AutoLabelEx();
            this.configControlsPanel = new System.Windows.Forms.Panel();
            this.deleteConfigBtn = new Syncfusion.Windows.Forms.ButtonAdv();
            this.moveUpBtn = new Syncfusion.Windows.Forms.ButtonAdv();
            this.moveDownBtn = new Syncfusion.Windows.Forms.ButtonAdv();
            this.configureBtn = new Syncfusion.Windows.Forms.ButtonAdv();
            this.configTypeIcon = new System.Windows.Forms.PictureBox();
            this.containerTable.SuspendLayout();
            this.configControlsPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.configTypeIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // containerTable
            // 
            this.containerTable.ColumnCount = 3;
            this.containerTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 48F));
            this.containerTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.containerTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60F));
            this.containerTable.Controls.Add(this.configTypeIcon, 0, 0);
            this.containerTable.Controls.Add(this.configNameLabel, 1, 0);
            this.containerTable.Controls.Add(this.configControlsPanel, 2, 0);
            this.containerTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.containerTable.Location = new System.Drawing.Point(0, 0);
            this.containerTable.Name = "containerTable";
            this.containerTable.RowCount = 1;
            this.containerTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.containerTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 48F));
            this.containerTable.Size = new System.Drawing.Size(441, 48);
            this.containerTable.TabIndex = 0;
            // 
            // configNameLabel
            // 
            this.configNameLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.configNameLabel.AutoEllipsis = true;
            this.configNameLabel.AutoSize = false;
            this.configNameLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.configNameLabel.Font = new System.Drawing.Font("Dosis", 19.25F);
            this.configNameLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.configNameLabel.Location = new System.Drawing.Point(51, 5);
            this.configNameLabel.Margin = new System.Windows.Forms.Padding(3, 5, 3, 0);
            this.configNameLabel.Name = "configNameLabel";
            this.configNameLabel.Size = new System.Drawing.Size(151, 43);
            this.configNameLabel.Style = Syncfusion.Windows.Forms.Tools.AutoLabelStyle.Office2016White;
            this.configNameLabel.TabIndex = 38;
            this.configNameLabel.Text = "Configuration Name";
            this.configNameLabel.ThemeName = "Office2016White";
            this.configNameLabel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ConfigNameLabel_MouseDown);
            this.configNameLabel.MouseEnter += new System.EventHandler(this.ConfigNameLabel_MouseEnter);
            this.configNameLabel.MouseLeave += new System.EventHandler(this.ConfigDataListItem_MouseLeave);
            this.configNameLabel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ConfigNameLabel_MouseUp);
            // 
            // configControlsPanel
            // 
            this.configControlsPanel.Controls.Add(this.deleteConfigBtn);
            this.configControlsPanel.Controls.Add(this.moveUpBtn);
            this.configControlsPanel.Controls.Add(this.moveDownBtn);
            this.configControlsPanel.Controls.Add(this.configureBtn);
            this.configControlsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.configControlsPanel.Location = new System.Drawing.Point(205, 0);
            this.configControlsPanel.Margin = new System.Windows.Forms.Padding(0);
            this.configControlsPanel.Name = "configControlsPanel";
            this.configControlsPanel.Size = new System.Drawing.Size(236, 48);
            this.configControlsPanel.TabIndex = 40;
            // 
            // deleteConfigBtn
            // 
            this.deleteConfigBtn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.deleteConfigBtn.Appearance = Syncfusion.Windows.Forms.ButtonAppearance.Metro;
            this.deleteConfigBtn.BackColor = System.Drawing.Color.Crimson;
            this.deleteConfigBtn.BeforeTouchSize = new System.Drawing.Size(45, 48);
            this.deleteConfigBtn.BorderStyleAdv = Syncfusion.Windows.Forms.ButtonAdvBorderStyle.Flat;
            this.deleteConfigBtn.FlatAppearance.BorderSize = 0;
            this.deleteConfigBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.deleteConfigBtn.Font = new System.Drawing.Font("Dosis", 18.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.deleteConfigBtn.ForeColor = System.Drawing.Color.White;
            this.deleteConfigBtn.Image = ((System.Drawing.Image)(resources.GetObject("deleteConfigBtn.Image")));
            this.deleteConfigBtn.Location = new System.Drawing.Point(191, 0);
            this.deleteConfigBtn.Margin = new System.Windows.Forms.Padding(0);
            this.deleteConfigBtn.Name = "deleteConfigBtn";
            this.deleteConfigBtn.Size = new System.Drawing.Size(45, 48);
            this.deleteConfigBtn.TabIndex = 41;
            this.deleteConfigBtn.ThemeName = "Metro";
            this.deleteConfigBtn.Click += new System.EventHandler(this.DeleteConfigBtn_Click);
            // 
            // moveUpBtn
            // 
            this.moveUpBtn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.moveUpBtn.Appearance = Syncfusion.Windows.Forms.ButtonAppearance.Metro;
            this.moveUpBtn.BackColor = System.Drawing.Color.LightSlateGray;
            this.moveUpBtn.BeforeTouchSize = new System.Drawing.Size(45, 48);
            this.moveUpBtn.BorderStyleAdv = Syncfusion.Windows.Forms.ButtonAdvBorderStyle.Flat;
            this.moveUpBtn.FlatAppearance.BorderSize = 0;
            this.moveUpBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.moveUpBtn.Font = new System.Drawing.Font("Dosis", 19F);
            this.moveUpBtn.ForeColor = System.Drawing.Color.White;
            this.moveUpBtn.Image = global::Synapse.Properties.Resources.ArrowUp_WF;
            this.moveUpBtn.Location = new System.Drawing.Point(147, 0);
            this.moveUpBtn.Margin = new System.Windows.Forms.Padding(0);
            this.moveUpBtn.Name = "moveUpBtn";
            this.moveUpBtn.Size = new System.Drawing.Size(45, 48);
            this.moveUpBtn.TabIndex = 42;
            this.moveUpBtn.ThemeName = "Metro";
            this.moveUpBtn.Click += new System.EventHandler(this.MoveUpConfigBtn_Click);
            // 
            // moveDownBtn
            // 
            this.moveDownBtn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.moveDownBtn.Appearance = Syncfusion.Windows.Forms.ButtonAppearance.Metro;
            this.moveDownBtn.BackColor = System.Drawing.Color.SlateGray;
            this.moveDownBtn.BeforeTouchSize = new System.Drawing.Size(45, 48);
            this.moveDownBtn.BorderStyleAdv = Syncfusion.Windows.Forms.ButtonAdvBorderStyle.Flat;
            this.moveDownBtn.FlatAppearance.BorderSize = 0;
            this.moveDownBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.moveDownBtn.Font = new System.Drawing.Font("Dosis", 19F);
            this.moveDownBtn.ForeColor = System.Drawing.Color.White;
            this.moveDownBtn.Image = global::Synapse.Properties.Resources.ArrowDown_WF;
            this.moveDownBtn.Location = new System.Drawing.Point(102, 0);
            this.moveDownBtn.Margin = new System.Windows.Forms.Padding(0);
            this.moveDownBtn.Name = "moveDownBtn";
            this.moveDownBtn.Size = new System.Drawing.Size(45, 48);
            this.moveDownBtn.TabIndex = 43;
            this.moveDownBtn.ThemeName = "Metro";
            this.moveDownBtn.UseVisualStyle = false;
            this.moveDownBtn.Click += new System.EventHandler(this.MoveDownConfigBtn_Click);
            // 
            // configureBtn
            // 
            this.configureBtn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.configureBtn.Appearance = Syncfusion.Windows.Forms.ButtonAppearance.Metro;
            this.configureBtn.BackColor = System.Drawing.Color.DodgerBlue;
            this.configureBtn.BeforeTouchSize = new System.Drawing.Size(45, 48);
            this.configureBtn.BorderStyleAdv = Syncfusion.Windows.Forms.ButtonAdvBorderStyle.Flat;
            this.configureBtn.FlatAppearance.BorderSize = 0;
            this.configureBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.configureBtn.Font = new System.Drawing.Font("Dosis", 19F);
            this.configureBtn.ForeColor = System.Drawing.Color.White;
            this.configureBtn.Image = global::Synapse.Properties.Resources.Gear__03WF;
            this.configureBtn.Location = new System.Drawing.Point(58, 0);
            this.configureBtn.Margin = new System.Windows.Forms.Padding(0);
            this.configureBtn.Name = "configureBtn";
            this.configureBtn.Size = new System.Drawing.Size(45, 48);
            this.configureBtn.TabIndex = 44;
            this.configureBtn.ThemeName = "Metro";
            this.configureBtn.Click += new System.EventHandler(this.ConfigureConfigBtn_Click);
            // 
            // configTypeIcon
            // 
            this.configTypeIcon.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(165)))), ((int)(((byte)(225)))));
            this.configTypeIcon.Dock = System.Windows.Forms.DockStyle.Fill;
            this.configTypeIcon.Image = global::Synapse.Properties.Resources.Text_Braille_WF;
            this.configTypeIcon.Location = new System.Drawing.Point(0, 0);
            this.configTypeIcon.Margin = new System.Windows.Forms.Padding(0);
            this.configTypeIcon.Name = "configTypeIcon";
            this.configTypeIcon.Size = new System.Drawing.Size(48, 48);
            this.configTypeIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.configTypeIcon.TabIndex = 41;
            this.configTypeIcon.TabStop = false;
            // 
            // ConfigureDataListItem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.containerTable);
            this.Name = "ConfigureDataListItem";
            this.Size = new System.Drawing.Size(441, 48);
            this.MouseLeave += new System.EventHandler(this.ConfigDataListItem_MouseLeave);
            this.containerTable.ResumeLayout(false);
            this.configControlsPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.configTypeIcon)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel containerTable;
        private AutoLabelEx configNameLabel;
        private System.Windows.Forms.Panel configControlsPanel;
        private Syncfusion.Windows.Forms.ButtonAdv deleteConfigBtn;
        private Syncfusion.Windows.Forms.ButtonAdv moveUpBtn;
        private Syncfusion.Windows.Forms.ButtonAdv moveDownBtn;
        private Syncfusion.Windows.Forms.ButtonAdv configureBtn;
        private System.Windows.Forms.PictureBox configTypeIcon;
    }
}
