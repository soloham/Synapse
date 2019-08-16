namespace Synapse.Modules
{
    partial class DataConfigurationForm
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
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.containerFlowPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.emptyListLabel = new Syncfusion.Windows.Forms.Tools.AutoLabel();
            this.containerFlowPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // containerFlowPanel
            // 
            this.containerFlowPanel.Controls.Add(this.emptyListLabel);
            this.containerFlowPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.containerFlowPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.containerFlowPanel.Location = new System.Drawing.Point(2, 2);
            this.containerFlowPanel.Margin = new System.Windows.Forms.Padding(1);
            this.containerFlowPanel.Name = "containerFlowPanel";
            this.containerFlowPanel.Size = new System.Drawing.Size(493, 286);
            this.containerFlowPanel.TabIndex = 0;
            // 
            // emptyListLabel
            // 
            this.emptyListLabel.AutoSize = false;
            this.emptyListLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.emptyListLabel.Font = new System.Drawing.Font("Dosis", 20.25F);
            this.emptyListLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.emptyListLabel.Location = new System.Drawing.Point(0, 0);
            this.emptyListLabel.Margin = new System.Windows.Forms.Padding(0);
            this.emptyListLabel.Name = "emptyListLabel";
            this.emptyListLabel.Size = new System.Drawing.Size(493, 286);
            this.emptyListLabel.Style = Syncfusion.Windows.Forms.Tools.AutoLabelStyle.Office2016White;
            this.emptyListLabel.TabIndex = 1;
            this.emptyListLabel.Text = "You currently have no configurations,\r\nCreate/Import one to continue";
            this.emptyListLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.emptyListLabel.ThemeName = "Office2016White";
            this.emptyListLabel.Visible = false;
            // 
            // DataConfigurationForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.ClientSize = new System.Drawing.Size(497, 293);
            this.Controls.Add(this.containerFlowPanel);
            this.Font = new System.Drawing.Font("Dosis", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(740, 412);
            this.MinimizeBox = false;
            this.Name = "DataConfigurationForm";
            this.Padding = new System.Windows.Forms.Padding(2, 2, 2, 5);
            this.ShowToolTip = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Style.InactiveShadowOpacity = ((byte)(50));
            this.Style.ShadowOpacity = ((byte)(0));
            this.Style.TitleBar.BackColor = System.Drawing.Color.White;
            this.Style.TitleBar.Font = new System.Drawing.Font("Dosis", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Style.TitleBar.TextHorizontalAlignment = System.Windows.Forms.HorizontalAlignment.Center;
            this.Text = "Configure Data";
            this.containerFlowPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        private System.Windows.Forms.FlowLayoutPanel containerFlowPanel;
        private Syncfusion.Windows.Forms.Tools.AutoLabel emptyListLabel;
    }
}