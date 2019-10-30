namespace Synapse.Modules
{
    partial class EnterValueForm
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
            this.valueTextBox = new Syncfusion.Windows.Forms.Tools.TextBoxExt();
            this.finishBtn = new Syncfusion.Windows.Forms.ButtonAdv();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)(this.valueTextBox)).BeginInit();
            this.tableLayoutPanel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // valueTextBox
            // 
            this.valueTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.valueTextBox.BeforeTouchSize = new System.Drawing.Size(240, 37);
            this.valueTextBox.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(171)))), ((int)(((byte)(171)))), ((int)(((byte)(171)))));
            this.valueTextBox.BorderSides = ((System.Windows.Forms.Border3DSide)(((System.Windows.Forms.Border3DSide.Left | System.Windows.Forms.Border3DSide.Top) 
            | System.Windows.Forms.Border3DSide.Bottom)));
            this.valueTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.valueTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.valueTextBox.Font = new System.Drawing.Font("Dosis", 17.75F);
            this.valueTextBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.valueTextBox.Location = new System.Drawing.Point(5, 0);
            this.valueTextBox.Margin = new System.Windows.Forms.Padding(5, 0, 5, 8);
            this.valueTextBox.MinimumSize = new System.Drawing.Size(1, 1);
            this.valueTextBox.Name = "valueTextBox";
            this.valueTextBox.PasswordChar = '.';
            this.valueTextBox.Size = new System.Drawing.Size(240, 37);
            this.valueTextBox.Style = Syncfusion.Windows.Forms.Tools.TextBoxExt.theme.Office2016White;
            this.valueTextBox.TabIndex = 41;
            this.valueTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.valueTextBox.ThemeName = "Office2016White";
            this.valueTextBox.ThemeStyle.BorderColor = System.Drawing.Color.Gainsboro;
            this.valueTextBox.ThemeStyle.CornerRadius = 0;
            // 
            // finishBtn
            // 
            this.finishBtn.Appearance = Syncfusion.Windows.Forms.ButtonAppearance.Metro;
            this.finishBtn.BackColor = System.Drawing.Color.MediumAquamarine;
            this.finishBtn.BeforeTouchSize = new System.Drawing.Size(62, 35);
            this.finishBtn.BorderStyleAdv = Syncfusion.Windows.Forms.ButtonAdvBorderStyle.Flat;
            this.finishBtn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.finishBtn.FlatAppearance.BorderSize = 0;
            this.finishBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.finishBtn.Font = new System.Drawing.Font("Dosis", 20.25F);
            this.finishBtn.ForeColor = System.Drawing.Color.White;
            this.finishBtn.Image = global::Synapse.Properties.Resources.Check;
            this.finishBtn.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.finishBtn.Location = new System.Drawing.Point(253, 3);
            this.finishBtn.Name = "finishBtn";
            this.finishBtn.Padding = new System.Windows.Forms.Padding(10, 0, 10, 0);
            this.finishBtn.Size = new System.Drawing.Size(62, 35);
            this.finishBtn.TabIndex = 42;
            this.finishBtn.ThemeName = "Metro";
            this.finishBtn.Click += new System.EventHandler(this.finishBtn_Click);
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 2;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 78.93082F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 21.06918F));
            this.tableLayoutPanel3.Controls.Add(this.finishBtn, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.valueTextBox, 0, 0);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(2, 2);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 1;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(318, 41);
            this.tableLayoutPanel3.TabIndex = 43;
            // 
            // EnterValueForm
            // 
            this.ClientSize = new System.Drawing.Size(322, 45);
            this.Controls.Add(this.tableLayoutPanel3);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EnterValueForm";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.valueTextBox)).EndInit();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        private System.Windows.Forms.FlowLayoutPanel containerFlowPanel;
        private System.Windows.Forms.Panel paperConfigurationPanel;
        private Syncfusion.Windows.Forms.Tools.AutoLabel emptyListLabel;
        private System.Windows.Forms.TableLayoutPanel paperCodeTitleTable;
        private Syncfusion.Windows.Forms.Tools.AutoLabel autoLabel1;
        private Syncfusion.Windows.Forms.Tools.IntegerTextBox paperCodeField;
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
        private Syncfusion.Windows.Forms.Tools.TextBoxExt valueTextBox;
        private Syncfusion.Windows.Forms.ButtonAdv finishBtn;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
    }
}