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
            this.lastActiveLabel = new Synapse.Controls.AutoLabelEx();
            this.templateNameLabel = new Synapse.Controls.AutoLabelEx();
            this.pinToggleBtn = new Synapse.Controls.ButtonAdvEx();
            this.SuspendLayout();
            // 
            // lastActiveLabel
            // 
            this.lastActiveLabel.AutoEllipsis = true;
            this.lastActiveLabel.AutoSize = false;
            this.lastActiveLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.lastActiveLabel.Font = new System.Drawing.Font("Dosis", 9.249999F);
            this.lastActiveLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.lastActiveLabel.Location = new System.Drawing.Point(209, 8);
            this.lastActiveLabel.Name = "lastActiveLabel";
            this.lastActiveLabel.Size = new System.Drawing.Size(171, 16);
            this.lastActiveLabel.Style = Syncfusion.Windows.Forms.Tools.AutoLabelStyle.Office2016White;
            this.lastActiveLabel.TabIndex = 38;
            this.lastActiveLabel.Text = "1/1/2000 24:00";
            this.lastActiveLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lastActiveLabel.ThemeName = "Office2016White";
            this.lastActiveLabel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TemplateNameLabel_MouseDown);
            this.lastActiveLabel.MouseEnter += new System.EventHandler(this.TemplateNameLabel_MouseEnter);
            this.lastActiveLabel.MouseLeave += new System.EventHandler(this.TemplateListItem_MouseLeave);
            this.lastActiveLabel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.TemplateNameLabel_MouseUp);
            // 
            // templateNameLabel
            // 
            this.templateNameLabel.AutoEllipsis = true;
            this.templateNameLabel.AutoSize = false;
            this.templateNameLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.templateNameLabel.Font = new System.Drawing.Font("Dosis", 21.25F);
            this.templateNameLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.templateNameLabel.Location = new System.Drawing.Point(12, 9);
            this.templateNameLabel.Name = "templateNameLabel";
            this.templateNameLabel.Size = new System.Drawing.Size(235, 39);
            this.templateNameLabel.Style = Syncfusion.Windows.Forms.Tools.AutoLabelStyle.Office2016White;
            this.templateNameLabel.TabIndex = 37;
            this.templateNameLabel.Text = "Template Name";
            this.templateNameLabel.ThemeName = "Office2016White";
            this.templateNameLabel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TemplateNameLabel_MouseDown);
            this.templateNameLabel.MouseEnter += new System.EventHandler(this.TemplateNameLabel_MouseEnter);
            this.templateNameLabel.MouseLeave += new System.EventHandler(this.TemplateListItem_MouseLeave);
            this.templateNameLabel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.TemplateNameLabel_MouseUp);
            // 
            // pinToggleBtn
            // 
            this.pinToggleBtn.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.pinToggleBtn.Appearance = Syncfusion.Windows.Forms.ButtonAppearance.Metro;
            this.pinToggleBtn.BackColor = System.Drawing.Color.White;
            this.pinToggleBtn.BeforeTouchSize = new System.Drawing.Size(23, 25);
            this.pinToggleBtn.BorderStyleAdv = Syncfusion.Windows.Forms.ButtonAdvBorderStyle.Raised;
            this.pinToggleBtn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pinToggleBtn.FlatAppearance.BorderSize = 0;
            this.pinToggleBtn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Gainsboro;
            this.pinToggleBtn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightGray;
            this.pinToggleBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.pinToggleBtn.Font = new System.Drawing.Font("Dosis", 19F);
            this.pinToggleBtn.ForeColor = System.Drawing.Color.White;
            this.pinToggleBtn.Image = global::Synapse.Shared.Properties.SharedResources.Unpin;
            this.pinToggleBtn.Location = new System.Drawing.Point(355, 25);
            this.pinToggleBtn.Name = "pinToggleBtn";
            this.pinToggleBtn.Size = new System.Drawing.Size(23, 25);
            this.pinToggleBtn.TabIndex = 36;
            this.pinToggleBtn.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.pinToggleBtn.ThemeName = "Metro";
            this.pinToggleBtn.UseVisualStyle = false;
            this.pinToggleBtn.Visible = false;
            this.pinToggleBtn.Click += new System.EventHandler(this.PinToggleBtn_Click);
            this.pinToggleBtn.MouseEnter += new System.EventHandler(this.TemplateNameLabel_MouseEnter);
            // 
            // TemplateListItem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(5F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.lastActiveLabel);
            this.Controls.Add(this.templateNameLabel);
            this.Controls.Add(this.pinToggleBtn);
            this.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Font = new System.Drawing.Font("Dosis", 8.249999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "TemplateListItem";
            this.Size = new System.Drawing.Size(385, 60);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TemplateNameLabel_MouseDown);
            this.MouseEnter += new System.EventHandler(this.TemplateNameLabel_MouseEnter);
            this.MouseLeave += new System.EventHandler(this.TemplateListItem_MouseLeave);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.TemplateNameLabel_MouseUp);
            this.ResumeLayout(false);

        }

        #endregion
        private Synapse.Controls.ButtonAdvEx pinToggleBtn;
        private Synapse.Controls.AutoLabelEx templateNameLabel;
        private Synapse.Controls.AutoLabelEx lastActiveLabel;
    }
}
