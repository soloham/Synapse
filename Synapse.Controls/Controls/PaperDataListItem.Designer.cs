namespace Synapse.Controls
{
    partial class PaperDataListItem
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.configurePaperBtn = new Syncfusion.Windows.Forms.ButtonAdv();
            this.deletePaperBtn = new Syncfusion.Windows.Forms.ButtonAdv();
            this.paperTitleLabel = new Synapse.Controls.AutoLabelEx();
            this.containerTable.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // containerTable
            // 
            this.containerTable.ColumnCount = 2;
            this.containerTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.containerTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 95F));
            this.containerTable.Controls.Add(this.paperTitleLabel, 0, 0);
            this.containerTable.Controls.Add(this.panel1, 1, 0);
            this.containerTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.containerTable.Location = new System.Drawing.Point(0, 0);
            this.containerTable.Name = "containerTable";
            this.containerTable.RowCount = 1;
            this.containerTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.containerTable.Size = new System.Drawing.Size(441, 48);
            this.containerTable.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.configurePaperBtn);
            this.panel1.Controls.Add(this.deletePaperBtn);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(346, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(95, 48);
            this.panel1.TabIndex = 47;
            // 
            // configurePaperBtn
            // 
            this.configurePaperBtn.Appearance = Syncfusion.Windows.Forms.ButtonAppearance.Metro;
            this.configurePaperBtn.BackColor = System.Drawing.Color.DodgerBlue;
            this.configurePaperBtn.BeforeTouchSize = new System.Drawing.Size(45, 48);
            this.configurePaperBtn.BorderStyleAdv = Syncfusion.Windows.Forms.ButtonAdvBorderStyle.Flat;
            this.configurePaperBtn.Dock = System.Windows.Forms.DockStyle.Right;
            this.configurePaperBtn.FlatAppearance.BorderSize = 0;
            this.configurePaperBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.configurePaperBtn.Font = new System.Drawing.Font("Dosis", 19F);
            this.configurePaperBtn.ForeColor = System.Drawing.Color.White;
            this.configurePaperBtn.Image = Shared.Properties.SharedResources.Gear__03WF;
            this.configurePaperBtn.Location = new System.Drawing.Point(5, 0);
            this.configurePaperBtn.Margin = new System.Windows.Forms.Padding(0);
            this.configurePaperBtn.Name = "configurePaperBtn";
            this.configurePaperBtn.Size = new System.Drawing.Size(45, 48);
            this.configurePaperBtn.TabIndex = 46;
            this.configurePaperBtn.ThemeName = "Metro";
            this.configurePaperBtn.Click += new System.EventHandler(this.ConfigurePaperBtn_Click);
            // 
            // deletePaperBtn
            // 
            this.deletePaperBtn.Appearance = Syncfusion.Windows.Forms.ButtonAppearance.Metro;
            this.deletePaperBtn.BackColor = System.Drawing.Color.Crimson;
            this.deletePaperBtn.BeforeTouchSize = new System.Drawing.Size(45, 48);
            this.deletePaperBtn.BorderStyleAdv = Syncfusion.Windows.Forms.ButtonAdvBorderStyle.Flat;
            this.deletePaperBtn.Dock = System.Windows.Forms.DockStyle.Right;
            this.deletePaperBtn.FlatAppearance.BorderSize = 0;
            this.deletePaperBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.deletePaperBtn.Font = new System.Drawing.Font("Dosis", 18.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.deletePaperBtn.ForeColor = System.Drawing.Color.White;
            this.deletePaperBtn.Image = Shared.Properties.SharedResources.Delete_WF;
            this.deletePaperBtn.Location = new System.Drawing.Point(50, 0);
            this.deletePaperBtn.Margin = new System.Windows.Forms.Padding(0);
            this.deletePaperBtn.Name = "deletePaperBtn";
            this.deletePaperBtn.Size = new System.Drawing.Size(45, 48);
            this.deletePaperBtn.TabIndex = 45;
            this.deletePaperBtn.ThemeName = "Metro";
            this.deletePaperBtn.Click += new System.EventHandler(this.DeleteConfigBtn_Click);
            // 
            // paperTitleLabel
            // 
            this.paperTitleLabel.AutoEllipsis = true;
            this.paperTitleLabel.AutoSize = false;
            this.paperTitleLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.paperTitleLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.paperTitleLabel.Font = new System.Drawing.Font("Dosis", 19.25F);
            this.paperTitleLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.paperTitleLabel.Location = new System.Drawing.Point(3, 5);
            this.paperTitleLabel.Margin = new System.Windows.Forms.Padding(3, 5, 3, 0);
            this.paperTitleLabel.Name = "paperTitleLabel";
            this.paperTitleLabel.Size = new System.Drawing.Size(340, 43);
            this.paperTitleLabel.Style = Syncfusion.Windows.Forms.Tools.AutoLabelStyle.Office2016White;
            this.paperTitleLabel.TabIndex = 38;
            this.paperTitleLabel.Text = "Paper Title";
            this.paperTitleLabel.ThemeName = "Office2016White";
            this.paperTitleLabel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PaperNameLabel_MouseDown);
            this.paperTitleLabel.MouseEnter += new System.EventHandler(this.PaperNameLabel_MouseEnter);
            this.paperTitleLabel.MouseLeave += new System.EventHandler(this.PaperDataListItem_MouseLeave);
            this.paperTitleLabel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.PaperNameLabel_MouseUp);
            // 
            // PaperDataListItem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.containerTable);
            this.Name = "PaperDataListItem";
            this.Size = new System.Drawing.Size(441, 48);
            this.MouseLeave += new System.EventHandler(this.PaperDataListItem_MouseLeave);
            this.containerTable.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel containerTable;
        private AutoLabelEx paperTitleLabel;
        private System.Windows.Forms.Panel panel1;
        private Syncfusion.Windows.Forms.ButtonAdv configurePaperBtn;
        private Syncfusion.Windows.Forms.ButtonAdv deletePaperBtn;
    }
}
