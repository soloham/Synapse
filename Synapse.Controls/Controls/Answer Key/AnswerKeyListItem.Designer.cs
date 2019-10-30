using Synapse.Shared.Resources;
using System.Drawing;

namespace Synapse.Controls
{
    partial class AnswerKeyListItem
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
            this.configureBtn = new Syncfusion.Windows.Forms.ButtonAdv();
            this.deleteBtn = new Syncfusion.Windows.Forms.ButtonAdv();
            this.keyTitleLabel = new Synapse.Controls.AutoLabelEx();
            this.containerTable.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // containerTable
            // 
            this.containerTable.ColumnCount = 2;
            this.containerTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.containerTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 105F));
            this.containerTable.Controls.Add(this.keyTitleLabel, 0, 0);
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
            this.panel1.Controls.Add(this.configureBtn);
            this.panel1.Controls.Add(this.deleteBtn);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(336, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(105, 48);
            this.panel1.TabIndex = 47;
            // 
            // configureBtn
            // 
            this.configureBtn.Appearance = Syncfusion.Windows.Forms.ButtonAppearance.Metro;
            this.configureBtn.BackColor = System.Drawing.Color.DodgerBlue;
            this.configureBtn.BeforeTouchSize = new System.Drawing.Size(53, 48);
            this.configureBtn.BorderStyleAdv = Syncfusion.Windows.Forms.ButtonAdvBorderStyle.Flat;
            this.configureBtn.Dock = System.Windows.Forms.DockStyle.Right;
            this.configureBtn.FlatAppearance.BorderSize = 0;
            this.configureBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.configureBtn.Font = new System.Drawing.Font("Dosis", 19F);
            this.configureBtn.ForeColor = System.Drawing.Color.White;
            this.configureBtn.Image = Shared.Properties.SharedResources.Gear__03WF;
            this.configureBtn.Location = new System.Drawing.Point(-1, 0);
            this.configureBtn.Margin = new System.Windows.Forms.Padding(0);
            this.configureBtn.Name = "configureBtn";
            this.configureBtn.Size = new System.Drawing.Size(53, 48);
            this.configureBtn.TabIndex = 46;
            this.configureBtn.ThemeName = "Metro";
            this.configureBtn.Click += new System.EventHandler(this.ConfigureBtn_Click);
            // 
            // deleteBtn
            // 
            this.deleteBtn.Appearance = Syncfusion.Windows.Forms.ButtonAppearance.Metro;
            this.deleteBtn.BackColor = System.Drawing.Color.Crimson;
            this.deleteBtn.BeforeTouchSize = new System.Drawing.Size(53, 48);
            this.deleteBtn.BorderStyleAdv = Syncfusion.Windows.Forms.ButtonAdvBorderStyle.Flat;
            this.deleteBtn.Dock = System.Windows.Forms.DockStyle.Right;
            this.deleteBtn.FlatAppearance.BorderSize = 0;
            this.deleteBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.deleteBtn.Font = new System.Drawing.Font("Dosis", 18.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.deleteBtn.ForeColor = System.Drawing.Color.White;
            this.deleteBtn.Image = Shared.Properties.SharedResources.Delete_WF;
            this.deleteBtn.Location = new System.Drawing.Point(52, 0);
            this.deleteBtn.Margin = new System.Windows.Forms.Padding(0);
            this.deleteBtn.Name = "deleteBtn";
            this.deleteBtn.Size = new System.Drawing.Size(53, 48);
            this.deleteBtn.TabIndex = 45;
            this.deleteBtn.ThemeName = "Metro";
            this.deleteBtn.Click += new System.EventHandler(this.DeleteBtn_Click);
            // 
            // keyTitleLabel
            // 
            this.keyTitleLabel.AutoEllipsis = true;
            this.keyTitleLabel.AutoSize = false;
            this.keyTitleLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.keyTitleLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.keyTitleLabel.Font = new System.Drawing.Font("Dosis", 19.25F);
            this.keyTitleLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.keyTitleLabel.Location = new System.Drawing.Point(3, 5);
            this.keyTitleLabel.Margin = new System.Windows.Forms.Padding(3, 5, 3, 0);
            this.keyTitleLabel.Name = "keyTitleLabel";
            this.keyTitleLabel.Size = new System.Drawing.Size(330, 43);
            this.keyTitleLabel.Style = Syncfusion.Windows.Forms.Tools.AutoLabelStyle.Office2016White;
            this.keyTitleLabel.TabIndex = 38;
            this.keyTitleLabel.Text = "Key Title";
            this.keyTitleLabel.ThemeName = "Office2016White";
            this.keyTitleLabel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.AnswerTitleLabel_MouseDown);
            this.keyTitleLabel.MouseEnter += new System.EventHandler(this.AnswerTitleLabel_MouseEnter);
            this.keyTitleLabel.MouseLeave += new System.EventHandler(this.AnswerKeyListItem_MouseLeave);
            this.keyTitleLabel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.AnswerKeyTitleLabel_MouseUp);
            // 
            // AnswerKeyListItem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.containerTable);
            this.Name = "AnswerKeyListItem";
            this.Size = new System.Drawing.Size(441, 48);
            this.MouseLeave += new System.EventHandler(this.AnswerKeyListItem_MouseLeave);
            this.containerTable.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel containerTable;
        private AutoLabelEx keyTitleLabel;
        private System.Windows.Forms.Panel panel1;
        private Syncfusion.Windows.Forms.ButtonAdv configureBtn;
        private Syncfusion.Windows.Forms.ButtonAdv deleteBtn;
    }
}
