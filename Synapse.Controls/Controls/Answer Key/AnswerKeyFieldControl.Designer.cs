namespace Synapse.Controls.Answer_Key
{
    partial class AnswerKeyFieldControl
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
            this.MainTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.fieldIndexLabel = new Syncfusion.Windows.Forms.Tools.GradientLabel();
            this.optionsTable = new System.Windows.Forms.TableLayoutPanel();
            this.fieldSelectionPanel = new System.Windows.Forms.Panel();
            this.selectionLabel = new Syncfusion.Windows.Forms.Tools.GradientLabel();
            this.setOptionField = new Syncfusion.Windows.Forms.Tools.TextBoxExt();
            this.MainTableLayoutPanel.SuspendLayout();
            this.fieldSelectionPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.setOptionField)).BeginInit();
            this.SuspendLayout();
            // 
            // MainTableLayoutPanel
            // 
            this.MainTableLayoutPanel.AutoSize = true;
            this.MainTableLayoutPanel.ColumnCount = 3;
            this.MainTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 45F));
            this.MainTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.MainTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 65F));
            this.MainTableLayoutPanel.Controls.Add(this.fieldIndexLabel, 0, 0);
            this.MainTableLayoutPanel.Controls.Add(this.optionsTable, 1, 0);
            this.MainTableLayoutPanel.Controls.Add(this.fieldSelectionPanel, 2, 0);
            this.MainTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainTableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.MainTableLayoutPanel.Name = "MainTableLayoutPanel";
            this.MainTableLayoutPanel.RowCount = 1;
            this.MainTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.MainTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.MainTableLayoutPanel.Size = new System.Drawing.Size(350, 50);
            this.MainTableLayoutPanel.TabIndex = 0;
            // 
            // fieldIndexLabel
            // 
            this.fieldIndexLabel.BackgroundColor = new Syncfusion.Drawing.BrushInfo(Syncfusion.Drawing.GradientStyle.Horizontal, System.Drawing.Color.DodgerBlue, System.Drawing.Color.DeepSkyBlue);
            this.fieldIndexLabel.BeforeTouchSize = new System.Drawing.Size(45, 50);
            this.fieldIndexLabel.BorderAppearance = System.Windows.Forms.BorderStyle.None;
            this.fieldIndexLabel.BorderSides = ((System.Windows.Forms.Border3DSide)((((System.Windows.Forms.Border3DSide.Left | System.Windows.Forms.Border3DSide.Top) 
            | System.Windows.Forms.Border3DSide.Right) 
            | System.Windows.Forms.Border3DSide.Bottom)));
            this.fieldIndexLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fieldIndexLabel.Font = new System.Drawing.Font("Dosis", 14.25F);
            this.fieldIndexLabel.ForeColor = System.Drawing.Color.White;
            this.fieldIndexLabel.Location = new System.Drawing.Point(0, 0);
            this.fieldIndexLabel.Margin = new System.Windows.Forms.Padding(0);
            this.fieldIndexLabel.Name = "fieldIndexLabel";
            this.fieldIndexLabel.Size = new System.Drawing.Size(45, 50);
            this.fieldIndexLabel.TabIndex = 1;
            this.fieldIndexLabel.Text = "1.";
            this.fieldIndexLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // optionsTable
            // 
            this.optionsTable.ColumnCount = 3;
            this.optionsTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.optionsTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.optionsTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.optionsTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.optionsTable.Location = new System.Drawing.Point(45, 0);
            this.optionsTable.Margin = new System.Windows.Forms.Padding(0);
            this.optionsTable.Name = "optionsTable";
            this.optionsTable.RowCount = 1;
            this.optionsTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.optionsTable.Size = new System.Drawing.Size(240, 50);
            this.optionsTable.TabIndex = 5;
            // 
            // fieldSelectionPanel
            // 
            this.fieldSelectionPanel.Controls.Add(this.selectionLabel);
            this.fieldSelectionPanel.Controls.Add(this.setOptionField);
            this.fieldSelectionPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fieldSelectionPanel.Location = new System.Drawing.Point(285, 0);
            this.fieldSelectionPanel.Margin = new System.Windows.Forms.Padding(0);
            this.fieldSelectionPanel.Name = "fieldSelectionPanel";
            this.fieldSelectionPanel.Size = new System.Drawing.Size(65, 50);
            this.fieldSelectionPanel.TabIndex = 6;
            // 
            // selectionLabel
            // 
            this.selectionLabel.BackgroundColor = new Syncfusion.Drawing.BrushInfo();
            this.selectionLabel.BeforeTouchSize = new System.Drawing.Size(65, 50);
            this.selectionLabel.BorderAppearance = System.Windows.Forms.BorderStyle.None;
            this.selectionLabel.BorderSides = ((System.Windows.Forms.Border3DSide)((((System.Windows.Forms.Border3DSide.Left | System.Windows.Forms.Border3DSide.Top) 
            | System.Windows.Forms.Border3DSide.Right) 
            | System.Windows.Forms.Border3DSide.Bottom)));
            this.selectionLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.selectionLabel.Font = new System.Drawing.Font("Dosis", 18F);
            this.selectionLabel.ForeColor = System.Drawing.Color.Black;
            this.selectionLabel.Location = new System.Drawing.Point(0, 0);
            this.selectionLabel.Margin = new System.Windows.Forms.Padding(0);
            this.selectionLabel.Name = "selectionLabel";
            this.selectionLabel.Size = new System.Drawing.Size(65, 50);
            this.selectionLabel.TabIndex = 13;
            this.selectionLabel.Text = "A";
            this.selectionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.selectionLabel.DoubleClick += new System.EventHandler(this.selectionLabel_DoubleClick);
            // 
            // setOptionField
            // 
            this.setOptionField.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.setOptionField.BeforeTouchSize = new System.Drawing.Size(65, 49);
            this.setOptionField.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(171)))), ((int)(((byte)(171)))), ((int)(((byte)(171)))));
            this.setOptionField.BorderSides = System.Windows.Forms.Border3DSide.Left;
            this.setOptionField.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.setOptionField.Dock = System.Windows.Forms.DockStyle.Fill;
            this.setOptionField.Font = new System.Drawing.Font("Dosis", 23.5F);
            this.setOptionField.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.setOptionField.Location = new System.Drawing.Point(0, 0);
            this.setOptionField.Margin = new System.Windows.Forms.Padding(0);
            this.setOptionField.MinimumSize = new System.Drawing.Size(65, 49);
            this.setOptionField.Name = "setOptionField";
            this.setOptionField.Size = new System.Drawing.Size(65, 49);
            this.setOptionField.Style = Syncfusion.Windows.Forms.Tools.TextBoxExt.theme.Office2016White;
            this.setOptionField.TabIndex = 12;
            this.setOptionField.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.setOptionField.ThemeName = "Office2016White";
            this.setOptionField.KeyDown += new System.Windows.Forms.KeyEventHandler(this.setOptionField_KeyDown);
            this.setOptionField.KeyUp += new System.Windows.Forms.KeyEventHandler(this.setOptionField_KeyUp);
            // 
            // AnswerKeyFieldControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.MainTableLayoutPanel);
            this.Font = new System.Drawing.Font("Dosis", 11.25F);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "AnswerKeyFieldControl";
            this.Size = new System.Drawing.Size(350, 50);
            this.SizeChanged += new System.EventHandler(this.AnswerKeyFieldControl_SizeChanged);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.AnswerKeyFieldControl_KeyDown);
            this.MainTableLayoutPanel.ResumeLayout(false);
            this.fieldSelectionPanel.ResumeLayout(false);
            this.fieldSelectionPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.setOptionField)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel MainTableLayoutPanel;
        private System.Windows.Forms.TableLayoutPanel optionsTable;
        private System.Windows.Forms.Panel fieldSelectionPanel;
        private Syncfusion.Windows.Forms.Tools.TextBoxExt setOptionField;
        private Syncfusion.Windows.Forms.Tools.GradientLabel fieldIndexLabel;
        private Syncfusion.Windows.Forms.Tools.GradientLabel selectionLabel;
    }
}
