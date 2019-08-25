namespace Synapse.Controls
{
    partial class AlignmentMethodResultControl
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.alignmentTimeTablePanel = new System.Windows.Forms.TableLayoutPanel();
            this.alignmentTimeLabel = new Syncfusion.Windows.Forms.Tools.AutoLabel();
            this.alignmentTimeValueLabel = new Syncfusion.Windows.Forms.Tools.AutoLabel();
            this.tableLayoutPanel1.SuspendLayout();
            this.alignmentTimeTablePanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.alignmentTimeTablePanel, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(450, 380);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // alignmentTimeTablePanel
            // 
            this.alignmentTimeTablePanel.ColumnCount = 2;
            this.alignmentTimeTablePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60F));
            this.alignmentTimeTablePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.alignmentTimeTablePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.alignmentTimeTablePanel.Controls.Add(this.alignmentTimeValueLabel, 0, 0);
            this.alignmentTimeTablePanel.Controls.Add(this.alignmentTimeLabel, 0, 0);
            this.alignmentTimeTablePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.alignmentTimeTablePanel.Location = new System.Drawing.Point(3, 3);
            this.alignmentTimeTablePanel.Name = "alignmentTimeTablePanel";
            this.alignmentTimeTablePanel.RowCount = 1;
            this.alignmentTimeTablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.alignmentTimeTablePanel.Size = new System.Drawing.Size(444, 50);
            this.alignmentTimeTablePanel.TabIndex = 12;
            // 
            // alignmentTimeLabel
            // 
            this.alignmentTimeLabel.AutoSize = false;
            this.alignmentTimeLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.alignmentTimeLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.alignmentTimeLabel.Font = new System.Drawing.Font("Dosis", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.alignmentTimeLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.alignmentTimeLabel.Location = new System.Drawing.Point(3, 3);
            this.alignmentTimeLabel.Margin = new System.Windows.Forms.Padding(3);
            this.alignmentTimeLabel.Name = "alignmentTimeLabel";
            this.alignmentTimeLabel.Size = new System.Drawing.Size(260, 44);
            this.alignmentTimeLabel.Style = Syncfusion.Windows.Forms.Tools.AutoLabelStyle.Office2016White;
            this.alignmentTimeLabel.TabIndex = 10;
            this.alignmentTimeLabel.Text = "Alignment Time:";
            this.alignmentTimeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.alignmentTimeLabel.ThemeName = "Office2016White";
            // 
            // alignmentTimeValueLabel
            // 
            this.alignmentTimeValueLabel.AutoSize = false;
            this.alignmentTimeValueLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.alignmentTimeValueLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.alignmentTimeValueLabel.Font = new System.Drawing.Font("Dosis", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.alignmentTimeValueLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.alignmentTimeValueLabel.Location = new System.Drawing.Point(269, 3);
            this.alignmentTimeValueLabel.Margin = new System.Windows.Forms.Padding(3);
            this.alignmentTimeValueLabel.Name = "alignmentTimeValueLabel";
            this.alignmentTimeValueLabel.Size = new System.Drawing.Size(172, 44);
            this.alignmentTimeValueLabel.Style = Syncfusion.Windows.Forms.Tools.AutoLabelStyle.Office2016White;
            this.alignmentTimeValueLabel.TabIndex = 11;
            this.alignmentTimeValueLabel.Text = "0ms";
            this.alignmentTimeValueLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.alignmentTimeValueLabel.ThemeName = "Office2016White";
            // 
            // AlignmentMethodResultControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "AlignmentMethodResultControl";
            this.Size = new System.Drawing.Size(450, 380);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.alignmentTimeTablePanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel alignmentTimeTablePanel;
        private Syncfusion.Windows.Forms.Tools.AutoLabel alignmentTimeValueLabel;
        private Syncfusion.Windows.Forms.Tools.AutoLabel alignmentTimeLabel;
    }
}
