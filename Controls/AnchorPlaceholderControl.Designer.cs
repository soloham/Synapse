namespace Synapse.Controls
{
    partial class AnchorPlaceholderControl
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
            this.mainTablePanel = new System.Windows.Forms.TableLayoutPanel();
            this.deleteAnchorBtn = new Syncfusion.Windows.Forms.ButtonAdv();
            this.anchorImagePBox = new System.Windows.Forms.PictureBox();
            this.mainTablePanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.anchorImagePBox)).BeginInit();
            this.SuspendLayout();
            // 
            // mainTablePanel
            // 
            this.mainTablePanel.ColumnCount = 1;
            this.mainTablePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mainTablePanel.Controls.Add(this.deleteAnchorBtn, 0, 1);
            this.mainTablePanel.Controls.Add(this.anchorImagePBox, 0, 0);
            this.mainTablePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainTablePanel.Font = new System.Drawing.Font("Dosis", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mainTablePanel.Location = new System.Drawing.Point(0, 0);
            this.mainTablePanel.Name = "mainTablePanel";
            this.mainTablePanel.RowCount = 2;
            this.mainTablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 70F));
            this.mainTablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.mainTablePanel.Size = new System.Drawing.Size(60, 75);
            this.mainTablePanel.TabIndex = 0;
            // 
            // deleteAnchorBtn
            // 
            this.deleteAnchorBtn.Appearance = Syncfusion.Windows.Forms.ButtonAppearance.Metro;
            this.deleteAnchorBtn.BackColor = System.Drawing.Color.Crimson;
            this.deleteAnchorBtn.BeforeTouchSize = new System.Drawing.Size(60, 23);
            this.deleteAnchorBtn.BorderStyleAdv = Syncfusion.Windows.Forms.ButtonAdvBorderStyle.Flat;
            this.deleteAnchorBtn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.deleteAnchorBtn.Enabled = false;
            this.deleteAnchorBtn.FlatAppearance.BorderSize = 0;
            this.deleteAnchorBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.deleteAnchorBtn.Font = new System.Drawing.Font("Dosis", 18.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.deleteAnchorBtn.ForeColor = System.Drawing.Color.White;
            this.deleteAnchorBtn.Image = global::Synapse.Properties.Resources.Delete_WF_Sm;
            this.deleteAnchorBtn.Location = new System.Drawing.Point(0, 52);
            this.deleteAnchorBtn.Margin = new System.Windows.Forms.Padding(0);
            this.deleteAnchorBtn.Name = "deleteAnchorBtn";
            this.deleteAnchorBtn.Size = new System.Drawing.Size(60, 23);
            this.deleteAnchorBtn.TabIndex = 42;
            this.deleteAnchorBtn.ThemeName = "Metro";
            this.deleteAnchorBtn.Click += new System.EventHandler(this.DeleteAnchorBtn_Click);
            // 
            // anchorImagePBox
            // 
            this.anchorImagePBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.anchorImagePBox.Location = new System.Drawing.Point(0, 0);
            this.anchorImagePBox.Margin = new System.Windows.Forms.Padding(0);
            this.anchorImagePBox.Name = "anchorImagePBox";
            this.anchorImagePBox.Size = new System.Drawing.Size(60, 52);
            this.anchorImagePBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.anchorImagePBox.TabIndex = 43;
            this.anchorImagePBox.TabStop = false;
            // 
            // AnchorPlaceholderControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.mainTablePanel);
            this.Name = "AnchorPlaceholderControl";
            this.Size = new System.Drawing.Size(60, 75);
            this.mainTablePanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.anchorImagePBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel mainTablePanel;
        private Syncfusion.Windows.Forms.ButtonAdv deleteAnchorBtn;
        private System.Windows.Forms.PictureBox anchorImagePBox;
    }
}
