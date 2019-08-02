namespace Synapse.Controls
{
    partial class TemplateListItem
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
            this.lastActiveLabel = new Syncfusion.Windows.Forms.Tools.AutoLabel();
            this.pinToggleBtn = new Syncfusion.Windows.Forms.ButtonAdv();
            this.templateNameLabel = new Syncfusion.Windows.Forms.Tools.AutoLabel();
            this.SuspendLayout();
            // 
            // lastActiveLabel
            // 
            this.lastActiveLabel.AutoSize = false;
            this.lastActiveLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.lastActiveLabel.Font = new System.Drawing.Font("Dosis", 9.249999F);
            this.lastActiveLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.lastActiveLabel.Location = new System.Drawing.Point(284, 6);
            this.lastActiveLabel.Name = "lastActiveLabel";
            this.lastActiveLabel.Size = new System.Drawing.Size(101, 16);
            this.lastActiveLabel.Style = Syncfusion.Windows.Forms.Tools.AutoLabelStyle.Office2016White;
            this.lastActiveLabel.TabIndex = 1;
            this.lastActiveLabel.Text = "1/1/2000 24:00";
            this.lastActiveLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lastActiveLabel.ThemeName = "Office2016White";
            // 
            // pinToggleBtn
            // 
            this.pinToggleBtn.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.pinToggleBtn.Appearance = Syncfusion.Windows.Forms.ButtonAppearance.Metro;
            this.pinToggleBtn.BackColor = System.Drawing.Color.Transparent;
            this.pinToggleBtn.BeforeTouchSize = new System.Drawing.Size(23, 23);
            this.pinToggleBtn.BorderStyleAdv = Syncfusion.Windows.Forms.ButtonAdvBorderStyle.Flat;
            this.pinToggleBtn.FlatAppearance.BorderSize = 0;
            this.pinToggleBtn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Gainsboro;
            this.pinToggleBtn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightGray;
            this.pinToggleBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.pinToggleBtn.Font = new System.Drawing.Font("Dosis", 19F);
            this.pinToggleBtn.ForeColor = System.Drawing.Color.White;
            this.pinToggleBtn.Image = global::Synapse.Properties.Resources.Pin;
            this.pinToggleBtn.Location = new System.Drawing.Point(355, 25);
            this.pinToggleBtn.Name = "pinToggleBtn";
            this.pinToggleBtn.Size = new System.Drawing.Size(23, 23);
            this.pinToggleBtn.TabIndex = 36;
            this.pinToggleBtn.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.pinToggleBtn.ThemeName = "Metro";
            // 
            // templateNameLabel
            // 
            this.templateNameLabel.AutoEllipsis = true;
            this.templateNameLabel.AutoSize = false;
            this.templateNameLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.templateNameLabel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.templateNameLabel.Font = new System.Drawing.Font("Dosis", 20.25F);
            this.templateNameLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.templateNameLabel.Location = new System.Drawing.Point(13, 8);
            this.templateNameLabel.Name = "templateNameLabel";
            this.templateNameLabel.Size = new System.Drawing.Size(239, 36);
            this.templateNameLabel.Style = Syncfusion.Windows.Forms.Tools.AutoLabelStyle.Office2016White;
            this.templateNameLabel.TabIndex = 37;
            this.templateNameLabel.Text = "Template Name";
            this.templateNameLabel.ThemeName = "Office2016White";
            // 
            // TemplateListItem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(5F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.templateNameLabel);
            this.Controls.Add(this.pinToggleBtn);
            this.Controls.Add(this.lastActiveLabel);
            this.Font = new System.Drawing.Font("Dosis", 8.249999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "TemplateListItem";
            this.Size = new System.Drawing.Size(385, 51);
            this.ResumeLayout(false);

        }

        #endregion
        private Syncfusion.Windows.Forms.Tools.AutoLabel lastActiveLabel;
        private Syncfusion.Windows.Forms.ButtonAdv pinToggleBtn;
        private Syncfusion.Windows.Forms.Tools.AutoLabel templateNameLabel;
    }
}
