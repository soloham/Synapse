namespace Synapse.Controls
{
    partial class CustomDataEntryListItem
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
            this.containerTable = new System.Windows.Forms.TableLayoutPanel();
            this.fieldTypeIcon = new System.Windows.Forms.PictureBox();
            this.fieldNameLabel = new Synapse.Controls.AutoLabelEx();
            this.fieldControlsPanel = new System.Windows.Forms.Panel();
            this.deleteFieldBtn = new Syncfusion.Windows.Forms.ButtonAdv();
            this.moveUpBtn = new Syncfusion.Windows.Forms.ButtonAdv();
            this.moveDownBtn = new Syncfusion.Windows.Forms.ButtonAdv();
            this.configureBtn = new Syncfusion.Windows.Forms.ButtonAdv();
            this.containerTable.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fieldTypeIcon)).BeginInit();
            this.fieldControlsPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // containerTable
            // 
            this.containerTable.ColumnCount = 3;
            this.containerTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 48F));
            this.containerTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.containerTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60F));
            this.containerTable.Controls.Add(this.fieldTypeIcon, 0, 0);
            this.containerTable.Controls.Add(this.fieldNameLabel, 1, 0);
            this.containerTable.Controls.Add(this.fieldControlsPanel, 2, 0);
            this.containerTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.containerTable.Location = new System.Drawing.Point(0, 0);
            this.containerTable.Name = "containerTable";
            this.containerTable.RowCount = 1;
            this.containerTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.containerTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 48F));
            this.containerTable.Size = new System.Drawing.Size(441, 48);
            this.containerTable.TabIndex = 0;
            // 
            // fieldTypeIcon
            // 
            this.fieldTypeIcon.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(165)))), ((int)(((byte)(225)))));
            this.fieldTypeIcon.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fieldTypeIcon.Image = Shared.Properties.SharedResources.Text_Braille_WF;
            this.fieldTypeIcon.Location = new System.Drawing.Point(0, 0);
            this.fieldTypeIcon.Margin = new System.Windows.Forms.Padding(0);
            this.fieldTypeIcon.Name = "fieldTypeIcon";
            this.fieldTypeIcon.Size = new System.Drawing.Size(48, 48);
            this.fieldTypeIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.fieldTypeIcon.TabIndex = 41;
            this.fieldTypeIcon.TabStop = false;
            // 
            // fieldNameLabel
            // 
            this.fieldNameLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.fieldNameLabel.AutoEllipsis = true;
            this.fieldNameLabel.AutoSize = false;
            this.fieldNameLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.fieldNameLabel.Font = new System.Drawing.Font("Dosis", 19.25F);
            this.fieldNameLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.fieldNameLabel.Location = new System.Drawing.Point(51, 5);
            this.fieldNameLabel.Margin = new System.Windows.Forms.Padding(3, 5, 3, 0);
            this.fieldNameLabel.Name = "fieldNameLabel";
            this.fieldNameLabel.Size = new System.Drawing.Size(151, 43);
            this.fieldNameLabel.Style = Syncfusion.Windows.Forms.Tools.AutoLabelStyle.Office2016White;
            this.fieldNameLabel.TabIndex = 38;
            this.fieldNameLabel.Text = "Field Name";
            this.fieldNameLabel.ThemeName = "Office2016White";
            this.fieldNameLabel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.FieldNameLabel_MouseDown);
            this.fieldNameLabel.MouseEnter += new System.EventHandler(this.FieldNameLabel_MouseEnter);
            this.fieldNameLabel.MouseLeave += new System.EventHandler(this.ConfigureDataListItem_MouseLeave);
            this.fieldNameLabel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.FieldNameLabel_MouseUp);
            // 
            // fieldControlsPanel
            // 
            this.fieldControlsPanel.Controls.Add(this.deleteFieldBtn);
            this.fieldControlsPanel.Controls.Add(this.moveUpBtn);
            this.fieldControlsPanel.Controls.Add(this.moveDownBtn);
            this.fieldControlsPanel.Controls.Add(this.configureBtn);
            this.fieldControlsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fieldControlsPanel.Location = new System.Drawing.Point(205, 0);
            this.fieldControlsPanel.Margin = new System.Windows.Forms.Padding(0);
            this.fieldControlsPanel.Name = "fieldControlsPanel";
            this.fieldControlsPanel.Size = new System.Drawing.Size(236, 48);
            this.fieldControlsPanel.TabIndex = 40;
            // 
            // deleteFieldBtn
            // 
            this.deleteFieldBtn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.deleteFieldBtn.Appearance = Syncfusion.Windows.Forms.ButtonAppearance.Metro;
            this.deleteFieldBtn.BackColor = System.Drawing.Color.Crimson;
            this.deleteFieldBtn.BeforeTouchSize = new System.Drawing.Size(45, 48);
            this.deleteFieldBtn.BorderStyleAdv = Syncfusion.Windows.Forms.ButtonAdvBorderStyle.Flat;
            this.deleteFieldBtn.FlatAppearance.BorderSize = 0;
            this.deleteFieldBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.deleteFieldBtn.Font = new System.Drawing.Font("Dosis", 18.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.deleteFieldBtn.ForeColor = System.Drawing.Color.White;
            this.deleteFieldBtn.Image = Shared.Properties.SharedResources.Delete_WF;
            this.deleteFieldBtn.Location = new System.Drawing.Point(191, 0);
            this.deleteFieldBtn.Margin = new System.Windows.Forms.Padding(0);
            this.deleteFieldBtn.Name = "deleteFieldBtn";
            this.deleteFieldBtn.Size = new System.Drawing.Size(45, 48);
            this.deleteFieldBtn.TabIndex = 41;
            this.deleteFieldBtn.ThemeName = "Metro";
            this.deleteFieldBtn.Click += new System.EventHandler(this.DeleteCustomDataEntryBtn_Click);
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
            this.moveUpBtn.Image = Shared.Properties.SharedResources.ArrowUp_WF;
            this.moveUpBtn.Location = new System.Drawing.Point(147, 0);
            this.moveUpBtn.Margin = new System.Windows.Forms.Padding(0);
            this.moveUpBtn.Name = "moveUpBtn";
            this.moveUpBtn.Size = new System.Drawing.Size(45, 48);
            this.moveUpBtn.TabIndex = 42;
            this.moveUpBtn.ThemeName = "Metro";
            this.moveUpBtn.Click += new System.EventHandler(this.MoveUpCustomDataEntryBtn_Click);
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
            this.moveDownBtn.Image = Shared.Properties.SharedResources.ArrowDown_WF;
            this.moveDownBtn.Location = new System.Drawing.Point(102, 0);
            this.moveDownBtn.Margin = new System.Windows.Forms.Padding(0);
            this.moveDownBtn.Name = "moveDownBtn";
            this.moveDownBtn.Size = new System.Drawing.Size(45, 48);
            this.moveDownBtn.TabIndex = 43;
            this.moveDownBtn.ThemeName = "Metro";
            this.moveDownBtn.UseVisualStyle = false;
            this.moveDownBtn.Click += new System.EventHandler(this.MoveDownCustomDataEntryBtn_Click);
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
            this.configureBtn.Image = Shared.Properties.SharedResources.Gear__03WF;
            this.configureBtn.Location = new System.Drawing.Point(58, 0);
            this.configureBtn.Margin = new System.Windows.Forms.Padding(0);
            this.configureBtn.Name = "configureBtn";
            this.configureBtn.Size = new System.Drawing.Size(45, 48);
            this.configureBtn.TabIndex = 44;
            this.configureBtn.ThemeName = "Metro";
            this.configureBtn.Click += new System.EventHandler(this.ConfigureFieldBtn_Click);
            // 
            // CustomDataEntryListItem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.containerTable);
            this.Name = "CustomDataEntryListItem";
            this.Size = new System.Drawing.Size(441, 48);
            this.MouseLeave += new System.EventHandler(this.ConfigureDataListItem_MouseLeave);
            this.containerTable.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.fieldTypeIcon)).EndInit();
            this.fieldControlsPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel containerTable;
        private AutoLabelEx fieldNameLabel;
        private System.Windows.Forms.Panel fieldControlsPanel;
        private Syncfusion.Windows.Forms.ButtonAdv deleteFieldBtn;
        private Syncfusion.Windows.Forms.ButtonAdv moveUpBtn;
        private Syncfusion.Windows.Forms.ButtonAdv moveDownBtn;
        private Syncfusion.Windows.Forms.ButtonAdv configureBtn;
        private System.Windows.Forms.PictureBox fieldTypeIcon;
    }
}
