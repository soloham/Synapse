namespace Synapse.Modules
{
    partial class RegistrationAlignmentMethodForm
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
            this.MainLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.configureAnchorsPanel = new System.Windows.Forms.TableLayoutPanel();
            this.initializationButtonsPanel = new System.Windows.Forms.TableLayoutPanel();
            this.setBtn = new Syncfusion.Windows.Forms.ButtonAdv();
            this.nextBtn = new Syncfusion.Windows.Forms.ButtonAdv();
            this.MainLayoutPanel.SuspendLayout();
            this.configureAnchorsPanel.SuspendLayout();
            this.initializationButtonsPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // MainLayoutPanel
            // 
            this.MainLayoutPanel.ColumnCount = 1;
            this.MainLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.MainLayoutPanel.Controls.Add(this.configureAnchorsPanel, 0, 1);
            this.MainLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainLayoutPanel.Location = new System.Drawing.Point(2, 2);
            this.MainLayoutPanel.Name = "MainLayoutPanel";
            this.MainLayoutPanel.RowCount = 2;
            this.MainLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.MainLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 130F));
            this.MainLayoutPanel.Size = new System.Drawing.Size(461, 512);
            this.MainLayoutPanel.TabIndex = 43;
            // 
            // configureAnchorsPanel
            // 
            this.configureAnchorsPanel.BackColor = System.Drawing.Color.White;
            this.configureAnchorsPanel.ColumnCount = 1;
            this.configureAnchorsPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.configureAnchorsPanel.Controls.Add(this.initializationButtonsPanel, 0, 1);
            this.configureAnchorsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.configureAnchorsPanel.Location = new System.Drawing.Point(3, 385);
            this.configureAnchorsPanel.Name = "configureAnchorsPanel";
            this.configureAnchorsPanel.RowCount = 2;
            this.configureAnchorsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 55F));
            this.configureAnchorsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 45F));
            this.configureAnchorsPanel.Size = new System.Drawing.Size(455, 124);
            this.configureAnchorsPanel.TabIndex = 41;
            // 
            // initializationButtonsPanel
            // 
            this.initializationButtonsPanel.ColumnCount = 2;
            this.initializationButtonsPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.initializationButtonsPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.initializationButtonsPanel.Controls.Add(this.setBtn, 0, 0);
            this.initializationButtonsPanel.Controls.Add(this.nextBtn, 1, 0);
            this.initializationButtonsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.initializationButtonsPanel.Location = new System.Drawing.Point(0, 68);
            this.initializationButtonsPanel.Margin = new System.Windows.Forms.Padding(0);
            this.initializationButtonsPanel.Name = "initializationButtonsPanel";
            this.initializationButtonsPanel.RowCount = 1;
            this.initializationButtonsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.initializationButtonsPanel.Size = new System.Drawing.Size(455, 56);
            this.initializationButtonsPanel.TabIndex = 7;
            // 
            // setBtn
            // 
            this.setBtn.Appearance = Syncfusion.Windows.Forms.ButtonAppearance.Metro;
            this.setBtn.BackColor = System.Drawing.Color.DodgerBlue;
            this.setBtn.BeforeTouchSize = new System.Drawing.Size(221, 50);
            this.setBtn.BorderStyleAdv = Syncfusion.Windows.Forms.ButtonAdvBorderStyle.Flat;
            this.setBtn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.setBtn.FlatAppearance.BorderSize = 0;
            this.setBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.setBtn.Font = new System.Drawing.Font("Dosis", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.setBtn.ForeColor = System.Drawing.Color.White;
            this.setBtn.Image = global::Synapse.Properties.Resources.Out;
            this.setBtn.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.setBtn.Location = new System.Drawing.Point(3, 3);
            this.setBtn.Name = "setBtn";
            this.setBtn.Padding = new System.Windows.Forms.Padding(10, 0, 10, 0);
            this.setBtn.Size = new System.Drawing.Size(221, 50);
            this.setBtn.TabIndex = 37;
            this.setBtn.Text = "   SET";
            this.setBtn.ThemeName = "Metro";
            this.setBtn.ThemeStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.setBtn.ThemeStyle.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(171)))), ((int)(((byte)(171)))), ((int)(((byte)(171)))));
            this.setBtn.ThemeStyle.DisabledBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(243)))), ((int)(((byte)(243)))));
            this.setBtn.ThemeStyle.DisabledBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
            this.setBtn.ThemeStyle.DisabledForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(171)))), ((int)(((byte)(171)))), ((int)(((byte)(171)))));
            this.setBtn.ThemeStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.setBtn.ThemeStyle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.setBtn.ThemeStyle.HoverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(236)))), ((int)(((byte)(249)))));
            this.setBtn.ThemeStyle.HoverBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(113)))), ((int)(((byte)(175)))), ((int)(((byte)(229)))));
            this.setBtn.ThemeStyle.HoverForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.setBtn.ThemeStyle.PressedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(205)))), ((int)(((byte)(230)))), ((int)(((byte)(247)))));
            this.setBtn.ThemeStyle.PressedBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(212)))));
            this.setBtn.ThemeStyle.PressedForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.setBtn.UseVisualStyle = false;
            this.setBtn.UseVisualStyleBackColor = false;
            this.setBtn.Click += new System.EventHandler(this.SetBtn_Click);
            // 
            // nextBtn
            // 
            this.nextBtn.Appearance = Syncfusion.Windows.Forms.ButtonAppearance.Metro;
            this.nextBtn.BackColor = System.Drawing.Color.MediumTurquoise;
            this.nextBtn.BeforeTouchSize = new System.Drawing.Size(222, 50);
            this.nextBtn.BorderStyleAdv = Syncfusion.Windows.Forms.ButtonAdvBorderStyle.Flat;
            this.nextBtn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.nextBtn.FlatAppearance.BorderSize = 0;
            this.nextBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.nextBtn.Font = new System.Drawing.Font("Dosis", 20F);
            this.nextBtn.ForeColor = System.Drawing.Color.White;
            this.nextBtn.Image = global::Synapse.Properties.Resources.Check;
            this.nextBtn.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.nextBtn.Location = new System.Drawing.Point(230, 3);
            this.nextBtn.Name = "nextBtn";
            this.nextBtn.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.nextBtn.Size = new System.Drawing.Size(222, 50);
            this.nextBtn.TabIndex = 38;
            this.nextBtn.Text = "   DONE";
            this.nextBtn.ThemeName = "Metro";
            this.nextBtn.ThemeStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.nextBtn.ThemeStyle.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(171)))), ((int)(((byte)(171)))), ((int)(((byte)(171)))));
            this.nextBtn.ThemeStyle.DisabledBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(243)))), ((int)(((byte)(243)))));
            this.nextBtn.ThemeStyle.DisabledBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
            this.nextBtn.ThemeStyle.DisabledForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(171)))), ((int)(((byte)(171)))), ((int)(((byte)(171)))));
            this.nextBtn.ThemeStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.nextBtn.ThemeStyle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.nextBtn.ThemeStyle.HoverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(236)))), ((int)(((byte)(249)))));
            this.nextBtn.ThemeStyle.HoverBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(113)))), ((int)(((byte)(175)))), ((int)(((byte)(229)))));
            this.nextBtn.ThemeStyle.HoverForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.nextBtn.ThemeStyle.PressedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(205)))), ((int)(((byte)(230)))), ((int)(((byte)(247)))));
            this.nextBtn.ThemeStyle.PressedBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(212)))));
            this.nextBtn.ThemeStyle.PressedForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.nextBtn.UseVisualStyle = false;
            this.nextBtn.UseVisualStyleBackColor = false;
            this.nextBtn.Click += new System.EventHandler(this.DoneBtn_Click);
            // 
            // RegistrationAlignmentMethodForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.ClientSize = new System.Drawing.Size(465, 519);
            this.Controls.Add(this.MainLayoutPanel);
            this.Font = new System.Drawing.Font("Dosis", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MinimizeBox = false;
            this.Name = "RegistrationAlignmentMethodForm";
            this.Padding = new System.Windows.Forms.Padding(2, 2, 2, 5);
            this.ShowToolTip = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Style.InactiveShadowOpacity = ((byte)(50));
            this.Style.ShadowOpacity = ((byte)(0));
            this.Style.TitleBar.BackColor = System.Drawing.Color.White;
            this.Style.TitleBar.Font = new System.Drawing.Font("Dosis", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Style.TitleBar.TextHorizontalAlignment = System.Windows.Forms.HorizontalAlignment.Center;
            this.Text = "Registration Alignment Method";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.MainLayoutPanel.ResumeLayout(false);
            this.configureAnchorsPanel.ResumeLayout(false);
            this.initializationButtonsPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel MainLayoutPanel;
        private System.Windows.Forms.TableLayoutPanel configureAnchorsPanel;
        private System.Windows.Forms.TableLayoutPanel initializationButtonsPanel;
        private Syncfusion.Windows.Forms.ButtonAdv setBtn;
        private Syncfusion.Windows.Forms.ButtonAdv nextBtn;
    }
}