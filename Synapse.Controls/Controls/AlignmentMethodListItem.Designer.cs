using Synapse.Shared.Resources;
using System.Drawing;

namespace Synapse.Controls
{
    partial class AlignmentMethodListItem
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AlignmentMethodListItem));
            this.containerTable = new System.Windows.Forms.TableLayoutPanel();
            this.methodTypeIcon = new System.Windows.Forms.PictureBox();
            this.methodNameLabel = new Synapse.Controls.AutoLabelEx();
            this.configControlsPanel = new System.Windows.Forms.Panel();
            this.deleteMethodBtn = new Syncfusion.Windows.Forms.ButtonAdv();
            this.moveUpBtn = new Syncfusion.Windows.Forms.ButtonAdv();
            this.moveDownBtn = new Syncfusion.Windows.Forms.ButtonAdv();
            this.configureBtn = new Syncfusion.Windows.Forms.ButtonAdv();
            this.containerTable.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.methodTypeIcon)).BeginInit();
            this.configControlsPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // containerTable
            // 
            this.containerTable.ColumnCount = 3;
            this.containerTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 48F));
            this.containerTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.containerTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60F));
            this.containerTable.Controls.Add(this.methodTypeIcon, 0, 0);
            this.containerTable.Controls.Add(this.methodNameLabel, 1, 0);
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
            // methodTypeIcon
            // 
            this.methodTypeIcon.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(165)))), ((int)(((byte)(225)))));
            this.methodTypeIcon.Dock = System.Windows.Forms.DockStyle.Fill;
            this.methodTypeIcon.Image = Shared.Properties.SharedResources.Text_Braille_WF;
            this.methodTypeIcon.Location = new System.Drawing.Point(0, 0);
            this.methodTypeIcon.Margin = new System.Windows.Forms.Padding(0);
            this.methodTypeIcon.Name = "methodTypeIcon";
            this.methodTypeIcon.Size = new System.Drawing.Size(48, 48);
            this.methodTypeIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.methodTypeIcon.TabIndex = 41;
            this.methodTypeIcon.TabStop = false;
            // 
            // methodNameLabel
            // 
            this.methodNameLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.methodNameLabel.AutoEllipsis = true;
            this.methodNameLabel.AutoSize = false;
            this.methodNameLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.methodNameLabel.Font = new System.Drawing.Font("Dosis", 19.25F);
            this.methodNameLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.methodNameLabel.Location = new System.Drawing.Point(51, 5);
            this.methodNameLabel.Margin = new System.Windows.Forms.Padding(3, 5, 3, 0);
            this.methodNameLabel.Name = "methodNameLabel";
            this.methodNameLabel.Size = new System.Drawing.Size(151, 43);
            this.methodNameLabel.Style = Syncfusion.Windows.Forms.Tools.AutoLabelStyle.Office2016White;
            this.methodNameLabel.TabIndex = 38;
            this.methodNameLabel.Text = "Method Name";
            this.methodNameLabel.ThemeName = "Office2016White";
            this.methodNameLabel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MethodNameLabel_MouseDown);
            this.methodNameLabel.MouseEnter += new System.EventHandler(this.MethodNameLabel_MouseEnter);
            this.methodNameLabel.MouseLeave += new System.EventHandler(this.MethodListItem_MouseLeave);
            this.methodNameLabel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.MethodNameLabel_MouseUp);
            // 
            // configControlsPanel
            // 
            this.configControlsPanel.Controls.Add(this.deleteMethodBtn);
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
            // deleteMethodBtn
            // 
            this.deleteMethodBtn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.deleteMethodBtn.Appearance = Syncfusion.Windows.Forms.ButtonAppearance.Metro;
            this.deleteMethodBtn.BackColor = System.Drawing.Color.Crimson;
            this.deleteMethodBtn.BeforeTouchSize = new System.Drawing.Size(45, 48);
            this.deleteMethodBtn.BorderStyleAdv = Syncfusion.Windows.Forms.ButtonAdvBorderStyle.Flat;
            this.deleteMethodBtn.FlatAppearance.BorderSize = 0;
            this.deleteMethodBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.deleteMethodBtn.Font = new System.Drawing.Font("Dosis", 18.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.deleteMethodBtn.ForeColor = System.Drawing.Color.White;
            this.deleteMethodBtn.Image = ((System.Drawing.Image)(resources.GetObject("deleteMethodBtn.Image")));
            this.deleteMethodBtn.Location = new System.Drawing.Point(191, 0);
            this.deleteMethodBtn.Margin = new System.Windows.Forms.Padding(0);
            this.deleteMethodBtn.Name = "deleteMethodBtn";
            this.deleteMethodBtn.Size = new System.Drawing.Size(45, 48);
            this.deleteMethodBtn.TabIndex = 41;
            this.deleteMethodBtn.ThemeName = "Metro";
            this.deleteMethodBtn.Click += new System.EventHandler(this.DeleteMethodBtn_Click);
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
            this.moveUpBtn.Click += new System.EventHandler(this.MoveUpMethodBtn_Click);
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
            this.moveDownBtn.Click += new System.EventHandler(this.MoveDownMethodBtn_Click);
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
            this.configureBtn.Click += new System.EventHandler(this.ConfigureMethodBtn_Click);
            // 
            // AlignmentMethodListItem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.containerTable);
            this.Name = "AlignmentMethodListItem";
            this.Size = new System.Drawing.Size(441, 48);
            this.MouseLeave += new System.EventHandler(this.MethodListItem_MouseLeave);
            this.containerTable.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.methodTypeIcon)).EndInit();
            this.configControlsPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel containerTable;
        private AutoLabelEx methodNameLabel;
        private System.Windows.Forms.Panel configControlsPanel;
        private Syncfusion.Windows.Forms.ButtonAdv deleteMethodBtn;
        private Syncfusion.Windows.Forms.ButtonAdv moveUpBtn;
        private Syncfusion.Windows.Forms.ButtonAdv moveDownBtn;
        private Syncfusion.Windows.Forms.ButtonAdv configureBtn;
        private System.Windows.Forms.PictureBox methodTypeIcon;
    }
}
