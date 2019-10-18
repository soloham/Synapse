namespace Synapse.Modules
{
    partial class DataEditForm
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
            this.imageBox = new Cyotek.Windows.Forms.Demo.ImageBoxEx();
            this.dataValueTextBox = new Syncfusion.Windows.Forms.Tools.TextBoxExt();
            this.setDataValueBtn = new Syncfusion.Windows.Forms.ButtonAdv();
            ((System.ComponentModel.ISupportInitialize)(this.dataValueTextBox)).BeginInit();
            this.SuspendLayout();
            // 
            // imageBox
            // 
            this.imageBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.imageBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.imageBox.DragHandleSize = 10;
            this.imageBox.DropShadowSize = 5;
            this.imageBox.Font = new System.Drawing.Font("Dosis", 14F);
            this.imageBox.GridColor = System.Drawing.Color.White;
            this.imageBox.ImageBorderColor = System.Drawing.SystemColors.ActiveBorder;
            this.imageBox.ImageBorderStyle = Cyotek.Windows.Forms.ImageBoxBorderStyle.FixedSingleGlowShadow;
            this.imageBox.Location = new System.Drawing.Point(3, 2);
            this.imageBox.Margin = new System.Windows.Forms.Padding(0);
            this.imageBox.Name = "imageBox";
            this.imageBox.SelectionMode = Cyotek.Windows.Forms.ImageBoxSelectionMode.Rectangle;
            this.imageBox.Size = new System.Drawing.Size(492, 185);
            this.imageBox.StepSize = new System.Drawing.Size(8, 8);
            this.imageBox.TabIndex = 3;
            this.imageBox.Text = "Configuration Region";
            this.imageBox.Paint += new System.Windows.Forms.PaintEventHandler(this.imageBox_Paint);
            // 
            // dataValueTextBox
            // 
            this.dataValueTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.dataValueTextBox.BeforeTouchSize = new System.Drawing.Size(331, 42);
            this.dataValueTextBox.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(171)))), ((int)(((byte)(171)))), ((int)(((byte)(171)))));
            this.dataValueTextBox.BorderSides = ((System.Windows.Forms.Border3DSide)((System.Windows.Forms.Border3DSide.Left | System.Windows.Forms.Border3DSide.Right)));
            this.dataValueTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.dataValueTextBox.Font = new System.Drawing.Font("Dosis", 20.75F);
            this.dataValueTextBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.dataValueTextBox.Location = new System.Drawing.Point(3, 194);
            this.dataValueTextBox.Name = "dataValueTextBox";
            this.dataValueTextBox.Size = new System.Drawing.Size(331, 42);
            this.dataValueTextBox.Style = Syncfusion.Windows.Forms.Tools.TextBoxExt.theme.Office2016White;
            this.dataValueTextBox.TabIndex = 16;
            this.dataValueTextBox.Text = "Value";
            this.dataValueTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.dataValueTextBox.ThemeName = "Office2016White";
            // 
            // setDataValueBtn
            // 
            this.setDataValueBtn.Appearance = Syncfusion.Windows.Forms.ButtonAppearance.Metro;
            this.setDataValueBtn.BackColor = System.Drawing.Color.DodgerBlue;
            this.setDataValueBtn.BeforeTouchSize = new System.Drawing.Size(155, 42);
            this.setDataValueBtn.BorderStyleAdv = Syncfusion.Windows.Forms.ButtonAdvBorderStyle.Flat;
            this.setDataValueBtn.FlatAppearance.BorderSize = 0;
            this.setDataValueBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.setDataValueBtn.Font = new System.Drawing.Font("Dosis", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.setDataValueBtn.ForeColor = System.Drawing.Color.White;
            this.setDataValueBtn.Image = global::Synapse.Properties.Resources.Check;
            this.setDataValueBtn.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.setDataValueBtn.Location = new System.Drawing.Point(340, 194);
            this.setDataValueBtn.Name = "setDataValueBtn";
            this.setDataValueBtn.Padding = new System.Windows.Forms.Padding(10, 0, 10, 0);
            this.setDataValueBtn.Size = new System.Drawing.Size(155, 42);
            this.setDataValueBtn.TabIndex = 38;
            this.setDataValueBtn.Text = "   SET";
            this.setDataValueBtn.ThemeName = "Metro";
            this.setDataValueBtn.ThemeStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.setDataValueBtn.ThemeStyle.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(171)))), ((int)(((byte)(171)))), ((int)(((byte)(171)))));
            this.setDataValueBtn.ThemeStyle.DisabledBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(243)))), ((int)(((byte)(243)))));
            this.setDataValueBtn.ThemeStyle.DisabledBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
            this.setDataValueBtn.ThemeStyle.DisabledForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(171)))), ((int)(((byte)(171)))), ((int)(((byte)(171)))));
            this.setDataValueBtn.ThemeStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.setDataValueBtn.ThemeStyle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.setDataValueBtn.ThemeStyle.HoverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(236)))), ((int)(((byte)(249)))));
            this.setDataValueBtn.ThemeStyle.HoverBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(113)))), ((int)(((byte)(175)))), ((int)(((byte)(229)))));
            this.setDataValueBtn.ThemeStyle.HoverForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.setDataValueBtn.ThemeStyle.PressedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(205)))), ((int)(((byte)(230)))), ((int)(((byte)(247)))));
            this.setDataValueBtn.ThemeStyle.PressedBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(212)))));
            this.setDataValueBtn.ThemeStyle.PressedForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.setDataValueBtn.UseVisualStyle = false;
            this.setDataValueBtn.UseVisualStyleBackColor = false;
            this.setDataValueBtn.Click += new System.EventHandler(this.setDataValueBtn_Click);
            // 
            // DataEditForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.ClientSize = new System.Drawing.Size(497, 244);
            this.Controls.Add(this.setDataValueBtn);
            this.Controls.Add(this.dataValueTextBox);
            this.Controls.Add(this.imageBox);
            this.Font = new System.Drawing.Font("Dosis", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(740, 412);
            this.MinimizeBox = false;
            this.Name = "DataEditForm";
            this.Padding = new System.Windows.Forms.Padding(8, 2, 2, 5);
            this.ShowToolTip = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Style.InactiveShadowOpacity = ((byte)(50));
            this.Style.ShadowOpacity = ((byte)(0));
            this.Style.TitleBar.BackColor = System.Drawing.Color.White;
            this.Style.TitleBar.Font = new System.Drawing.Font("Dosis", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Style.TitleBar.TextHorizontalAlignment = System.Windows.Forms.HorizontalAlignment.Center;
            this.Text = "Edit: ";
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.DataEditForm_KeyUp);
            ((System.ComponentModel.ISupportInitialize)(this.dataValueTextBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        private Cyotek.Windows.Forms.Demo.ImageBoxEx imageBox;
        private Syncfusion.Windows.Forms.Tools.TextBoxExt dataValueTextBox;
        private Syncfusion.Windows.Forms.ButtonAdv setDataValueBtn;
    }
}