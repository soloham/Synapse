namespace Synapse.Modules
{
    partial class TemplatesHub
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TemplatesHub));
            this.templatesLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.createTemplatePanel = new System.Windows.Forms.FlowLayoutPanel();
            this.createTemplateNameContainer = new System.Windows.Forms.Panel();
            this.createTemplateNameTextBox = new Syncfusion.Windows.Forms.Tools.TextBoxExt();
            this.addTemplateBtn = new Syncfusion.Windows.Forms.ButtonAdv();
            this.emptyListLabel = new Syncfusion.Windows.Forms.Tools.AutoLabel();
            this.containerPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.editTemplatePanel = new System.Windows.Forms.FlowLayoutPanel();
            this.editTemplateNameContainer = new System.Windows.Forms.Panel();
            this.editTemplateNameTextBoxExt = new Syncfusion.Windows.Forms.Tools.TextBoxExt();
            this.setTemplateNameBtn = new Syncfusion.Windows.Forms.ButtonAdv();
            this.deleteTemplateBtn = new Syncfusion.Windows.Forms.ButtonAdv();
            this.editTemplateBtn = new Syncfusion.Windows.Forms.ButtonAdv();
            this.loadTemplateBtn = new Syncfusion.Windows.Forms.ButtonAdv();
            this.importTemplateBtn = new Syncfusion.Windows.Forms.ButtonAdv();
            this.createTemplateBtn = new Syncfusion.Windows.Forms.ButtonAdv();
            this.createTemplatePanel.SuspendLayout();
            this.createTemplateNameContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.createTemplateNameTextBox)).BeginInit();
            this.containerPanel.SuspendLayout();
            this.editTemplatePanel.SuspendLayout();
            this.editTemplateNameContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.editTemplateNameTextBoxExt)).BeginInit();
            this.SuspendLayout();
            // 
            // templatesLayoutPanel
            // 
            this.templatesLayoutPanel.AutoSize = true;
            this.templatesLayoutPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.templatesLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.templatesLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
            this.templatesLayoutPanel.Name = "templatesLayoutPanel";
            this.templatesLayoutPanel.Size = new System.Drawing.Size(0, 0);
            this.templatesLayoutPanel.TabIndex = 36;
            this.templatesLayoutPanel.WrapContents = false;
            // 
            // createTemplatePanel
            // 
            this.createTemplatePanel.AutoScroll = true;
            this.createTemplatePanel.AutoSize = true;
            this.createTemplatePanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.createTemplatePanel.Controls.Add(this.createTemplateNameContainer);
            this.createTemplatePanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.createTemplatePanel.Location = new System.Drawing.Point(0, 0);
            this.createTemplatePanel.Margin = new System.Windows.Forms.Padding(0, 0, 0, 7);
            this.createTemplatePanel.Name = "createTemplatePanel";
            this.createTemplatePanel.Size = new System.Drawing.Size(385, 57);
            this.createTemplatePanel.TabIndex = 37;
            this.createTemplatePanel.Visible = false;
            // 
            // createTemplateNameContainer
            // 
            this.createTemplateNameContainer.Controls.Add(this.createTemplateNameTextBox);
            this.createTemplateNameContainer.Controls.Add(this.addTemplateBtn);
            this.createTemplateNameContainer.Location = new System.Drawing.Point(0, 0);
            this.createTemplateNameContainer.Margin = new System.Windows.Forms.Padding(0);
            this.createTemplateNameContainer.Name = "createTemplateNameContainer";
            this.createTemplateNameContainer.Size = new System.Drawing.Size(385, 57);
            this.createTemplateNameContainer.TabIndex = 35;
            // 
            // createTemplateNameTextBox
            // 
            this.createTemplateNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.createTemplateNameTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.createTemplateNameTextBox.BeforeTouchSize = new System.Drawing.Size(320, 45);
            this.createTemplateNameTextBox.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(171)))), ((int)(((byte)(171)))), ((int)(((byte)(171)))));
            this.createTemplateNameTextBox.BorderSides = ((System.Windows.Forms.Border3DSide)(((System.Windows.Forms.Border3DSide.Left | System.Windows.Forms.Border3DSide.Top) 
            | System.Windows.Forms.Border3DSide.Bottom)));
            this.createTemplateNameTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.createTemplateNameTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this, "NewTemplateName", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.createTemplateNameTextBox.Font = new System.Drawing.Font("Dosis", 21.65F);
            this.createTemplateNameTextBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.createTemplateNameTextBox.Location = new System.Drawing.Point(2, 5);
            this.createTemplateNameTextBox.Margin = new System.Windows.Forms.Padding(5, 0, 0, 8);
            this.createTemplateNameTextBox.MinimumSize = new System.Drawing.Size(100, 45);
            this.createTemplateNameTextBox.Name = "createTemplateNameTextBox";
            this.createTemplateNameTextBox.Size = new System.Drawing.Size(320, 45);
            this.createTemplateNameTextBox.Style = Syncfusion.Windows.Forms.Tools.TextBoxExt.theme.Office2016White;
            this.createTemplateNameTextBox.TabIndex = 2;
            this.createTemplateNameTextBox.Text = "New Template";
            this.createTemplateNameTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.createTemplateNameTextBox.ThemeName = "Office2016White";
            this.createTemplateNameTextBox.ThemeStyle.BorderColor = System.Drawing.Color.Gainsboro;
            this.createTemplateNameTextBox.ThemeStyle.CornerRadius = 0;
            // 
            // addTemplateBtn
            // 
            this.addTemplateBtn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.addTemplateBtn.Appearance = Syncfusion.Windows.Forms.ButtonAppearance.Metro;
            this.addTemplateBtn.BackColor = System.Drawing.Color.MediumTurquoise;
            this.addTemplateBtn.BeforeTouchSize = new System.Drawing.Size(63, 45);
            this.addTemplateBtn.BorderStyleAdv = Syncfusion.Windows.Forms.ButtonAdvBorderStyle.Flat;
            this.addTemplateBtn.FlatAppearance.BorderSize = 0;
            this.addTemplateBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.addTemplateBtn.Font = new System.Drawing.Font("Dosis", 19F);
            this.addTemplateBtn.ForeColor = System.Drawing.Color.White;
            this.addTemplateBtn.Image = global::Synapse.Properties.Resources.Add_New;
            this.addTemplateBtn.Location = new System.Drawing.Point(322, 5);
            this.addTemplateBtn.Margin = new System.Windows.Forms.Padding(0);
            this.addTemplateBtn.Name = "addTemplateBtn";
            this.addTemplateBtn.Size = new System.Drawing.Size(63, 45);
            this.addTemplateBtn.TabIndex = 34;
            this.addTemplateBtn.ThemeName = "Metro";
            this.addTemplateBtn.Click += new System.EventHandler(this.AddTemplateBtn_Click);
            // 
            // emptyListLabel
            // 
            this.emptyListLabel.AutoSize = false;
            this.emptyListLabel.Font = new System.Drawing.Font("Dosis", 20.25F);
            this.emptyListLabel.ForeColor = System.Drawing.Color.Black;
            this.emptyListLabel.Location = new System.Drawing.Point(0, 128);
            this.emptyListLabel.Margin = new System.Windows.Forms.Padding(0);
            this.emptyListLabel.Name = "emptyListLabel";
            this.emptyListLabel.Size = new System.Drawing.Size(385, 275);
            this.emptyListLabel.TabIndex = 0;
            this.emptyListLabel.Text = "You currently have no templates,\r\nCreate/Import one to continue";
            this.emptyListLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.emptyListLabel.Visible = false;
            // 
            // containerPanel
            // 
            this.containerPanel.AutoScroll = true;
            this.containerPanel.Controls.Add(this.templatesLayoutPanel);
            this.containerPanel.Controls.Add(this.createTemplatePanel);
            this.containerPanel.Controls.Add(this.editTemplatePanel);
            this.containerPanel.Controls.Add(this.emptyListLabel);
            this.containerPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.containerPanel.Location = new System.Drawing.Point(15, 9);
            this.containerPanel.Margin = new System.Windows.Forms.Padding(0);
            this.containerPanel.Name = "containerPanel";
            this.containerPanel.Size = new System.Drawing.Size(409, 354);
            this.containerPanel.TabIndex = 38;
            this.containerPanel.WrapContents = false;
            // 
            // editTemplatePanel
            // 
            this.editTemplatePanel.AutoScroll = true;
            this.editTemplatePanel.AutoSize = true;
            this.editTemplatePanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.editTemplatePanel.Controls.Add(this.editTemplateNameContainer);
            this.editTemplatePanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.editTemplatePanel.Location = new System.Drawing.Point(0, 64);
            this.editTemplatePanel.Margin = new System.Windows.Forms.Padding(0, 0, 0, 7);
            this.editTemplatePanel.Name = "editTemplatePanel";
            this.editTemplatePanel.Size = new System.Drawing.Size(385, 57);
            this.editTemplatePanel.TabIndex = 38;
            this.editTemplatePanel.Visible = false;
            // 
            // editTemplateNameContainer
            // 
            this.editTemplateNameContainer.Controls.Add(this.editTemplateNameTextBoxExt);
            this.editTemplateNameContainer.Controls.Add(this.setTemplateNameBtn);
            this.editTemplateNameContainer.Location = new System.Drawing.Point(0, 0);
            this.editTemplateNameContainer.Margin = new System.Windows.Forms.Padding(0);
            this.editTemplateNameContainer.Name = "editTemplateNameContainer";
            this.editTemplateNameContainer.Size = new System.Drawing.Size(385, 57);
            this.editTemplateNameContainer.TabIndex = 35;
            // 
            // editTemplateNameTextBoxExt
            // 
            this.editTemplateNameTextBoxExt.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.editTemplateNameTextBoxExt.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.editTemplateNameTextBoxExt.BeforeTouchSize = new System.Drawing.Size(320, 45);
            this.editTemplateNameTextBoxExt.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(171)))), ((int)(((byte)(171)))), ((int)(((byte)(171)))));
            this.editTemplateNameTextBoxExt.BorderSides = ((System.Windows.Forms.Border3DSide)(((System.Windows.Forms.Border3DSide.Left | System.Windows.Forms.Border3DSide.Top) 
            | System.Windows.Forms.Border3DSide.Bottom)));
            this.editTemplateNameTextBoxExt.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.editTemplateNameTextBoxExt.DataBindings.Add(new System.Windows.Forms.Binding("Text", this, "EditTemplateName", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.editTemplateNameTextBoxExt.Font = new System.Drawing.Font("Dosis", 21.65F);
            this.editTemplateNameTextBoxExt.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.editTemplateNameTextBoxExt.Location = new System.Drawing.Point(1, 6);
            this.editTemplateNameTextBoxExt.Margin = new System.Windows.Forms.Padding(5, 0, 0, 8);
            this.editTemplateNameTextBoxExt.MinimumSize = new System.Drawing.Size(100, 45);
            this.editTemplateNameTextBoxExt.Name = "editTemplateNameTextBoxExt";
            this.editTemplateNameTextBoxExt.Size = new System.Drawing.Size(320, 45);
            this.editTemplateNameTextBoxExt.Style = Syncfusion.Windows.Forms.Tools.TextBoxExt.theme.Office2016White;
            this.editTemplateNameTextBoxExt.TabIndex = 35;
            this.editTemplateNameTextBoxExt.Text = "Template Name";
            this.editTemplateNameTextBoxExt.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.editTemplateNameTextBoxExt.ThemeName = "Office2016White";
            this.editTemplateNameTextBoxExt.ThemeStyle.BorderColor = System.Drawing.Color.Gainsboro;
            this.editTemplateNameTextBoxExt.ThemeStyle.CornerRadius = 0;
            // 
            // setTemplateNameBtn
            // 
            this.setTemplateNameBtn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.setTemplateNameBtn.Appearance = Syncfusion.Windows.Forms.ButtonAppearance.Metro;
            this.setTemplateNameBtn.BackColor = System.Drawing.Color.CornflowerBlue;
            this.setTemplateNameBtn.BeforeTouchSize = new System.Drawing.Size(63, 45);
            this.setTemplateNameBtn.BorderStyleAdv = Syncfusion.Windows.Forms.ButtonAdvBorderStyle.Flat;
            this.setTemplateNameBtn.FlatAppearance.BorderSize = 0;
            this.setTemplateNameBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.setTemplateNameBtn.Font = new System.Drawing.Font("Dosis", 19F);
            this.setTemplateNameBtn.ForeColor = System.Drawing.Color.White;
            this.setTemplateNameBtn.Image = global::Synapse.Properties.Resources.Check;
            this.setTemplateNameBtn.Location = new System.Drawing.Point(322, 6);
            this.setTemplateNameBtn.Margin = new System.Windows.Forms.Padding(0);
            this.setTemplateNameBtn.Name = "setTemplateNameBtn";
            this.setTemplateNameBtn.Size = new System.Drawing.Size(63, 45);
            this.setTemplateNameBtn.TabIndex = 36;
            this.setTemplateNameBtn.ThemeName = "Metro";
            this.setTemplateNameBtn.Click += new System.EventHandler(this.SetTemplateNameBtn_Click);
            // 
            // deleteTemplateBtn
            // 
            this.deleteTemplateBtn.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.deleteTemplateBtn.Appearance = Syncfusion.Windows.Forms.ButtonAppearance.Metro;
            this.deleteTemplateBtn.BackColor = System.Drawing.Color.Crimson;
            this.deleteTemplateBtn.BeforeTouchSize = new System.Drawing.Size(290, 66);
            this.deleteTemplateBtn.BorderStyleAdv = Syncfusion.Windows.Forms.ButtonAdvBorderStyle.Flat;
            this.deleteTemplateBtn.FlatAppearance.BorderSize = 0;
            this.deleteTemplateBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.deleteTemplateBtn.Font = new System.Drawing.Font("Dosis", 19F);
            this.deleteTemplateBtn.ForeColor = System.Drawing.Color.White;
            this.deleteTemplateBtn.Image = ((System.Drawing.Image)(resources.GetObject("deleteTemplateBtn.Image")));
            this.deleteTemplateBtn.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.deleteTemplateBtn.Location = new System.Drawing.Point(421, 297);
            this.deleteTemplateBtn.Name = "deleteTemplateBtn";
            this.deleteTemplateBtn.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.deleteTemplateBtn.Size = new System.Drawing.Size(290, 66);
            this.deleteTemplateBtn.TabIndex = 35;
            this.deleteTemplateBtn.Text = "     Delete Template";
            this.deleteTemplateBtn.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.deleteTemplateBtn.ThemeName = "Metro";
            this.deleteTemplateBtn.Click += new System.EventHandler(this.DeleteTemplateBtn_Click);
            // 
            // editTemplateBtn
            // 
            this.editTemplateBtn.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.editTemplateBtn.Appearance = Syncfusion.Windows.Forms.ButtonAppearance.Metro;
            this.editTemplateBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(165)))), ((int)(((byte)(220)))));
            this.editTemplateBtn.BeforeTouchSize = new System.Drawing.Size(290, 66);
            this.editTemplateBtn.BorderStyleAdv = Syncfusion.Windows.Forms.ButtonAdvBorderStyle.Flat;
            this.editTemplateBtn.FlatAppearance.BorderSize = 0;
            this.editTemplateBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.editTemplateBtn.Font = new System.Drawing.Font("Dosis", 19F);
            this.editTemplateBtn.ForeColor = System.Drawing.Color.White;
            this.editTemplateBtn.Image = global::Synapse.Properties.Resources.Text_Highlight;
            this.editTemplateBtn.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.editTemplateBtn.Location = new System.Drawing.Point(421, 225);
            this.editTemplateBtn.Name = "editTemplateBtn";
            this.editTemplateBtn.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.editTemplateBtn.Size = new System.Drawing.Size(290, 66);
            this.editTemplateBtn.TabIndex = 34;
            this.editTemplateBtn.Text = "     Edit Template";
            this.editTemplateBtn.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.editTemplateBtn.ThemeName = "Metro";
            this.editTemplateBtn.Click += new System.EventHandler(this.EditTemplateBtn_Click);
            // 
            // loadTemplateBtn
            // 
            this.loadTemplateBtn.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.loadTemplateBtn.Appearance = Syncfusion.Windows.Forms.ButtonAppearance.Metro;
            this.loadTemplateBtn.BackColor = System.Drawing.Color.DodgerBlue;
            this.loadTemplateBtn.BeforeTouchSize = new System.Drawing.Size(290, 66);
            this.loadTemplateBtn.BorderStyleAdv = Syncfusion.Windows.Forms.ButtonAdvBorderStyle.Flat;
            this.loadTemplateBtn.FlatAppearance.BorderSize = 0;
            this.loadTemplateBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.loadTemplateBtn.Font = new System.Drawing.Font("Dosis", 19F);
            this.loadTemplateBtn.ForeColor = System.Drawing.Color.White;
            this.loadTemplateBtn.Image = global::Synapse.Properties.Resources.Media_Play;
            this.loadTemplateBtn.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.loadTemplateBtn.Location = new System.Drawing.Point(421, 9);
            this.loadTemplateBtn.Name = "loadTemplateBtn";
            this.loadTemplateBtn.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.loadTemplateBtn.Size = new System.Drawing.Size(290, 66);
            this.loadTemplateBtn.TabIndex = 33;
            this.loadTemplateBtn.Text = "     Load Template";
            this.loadTemplateBtn.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.loadTemplateBtn.ThemeName = "Metro";
            this.loadTemplateBtn.Click += new System.EventHandler(this.LoadTemplateBtn_Click);
            // 
            // importTemplateBtn
            // 
            this.importTemplateBtn.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.importTemplateBtn.Appearance = Syncfusion.Windows.Forms.ButtonAppearance.Metro;
            this.importTemplateBtn.BackColor = System.Drawing.Color.SlateGray;
            this.importTemplateBtn.BeforeTouchSize = new System.Drawing.Size(290, 66);
            this.importTemplateBtn.BorderStyleAdv = Syncfusion.Windows.Forms.ButtonAdvBorderStyle.Flat;
            this.importTemplateBtn.FlatAppearance.BorderSize = 0;
            this.importTemplateBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.importTemplateBtn.Font = new System.Drawing.Font("Dosis", 19F);
            this.importTemplateBtn.ForeColor = System.Drawing.Color.White;
            this.importTemplateBtn.Image = global::Synapse.Properties.Resources.Import__02;
            this.importTemplateBtn.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.importTemplateBtn.Location = new System.Drawing.Point(421, 153);
            this.importTemplateBtn.Name = "importTemplateBtn";
            this.importTemplateBtn.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.importTemplateBtn.Size = new System.Drawing.Size(290, 66);
            this.importTemplateBtn.TabIndex = 31;
            this.importTemplateBtn.Text = "     Import Existing";
            this.importTemplateBtn.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.importTemplateBtn.ThemeName = "Metro";
            // 
            // createTemplateBtn
            // 
            this.createTemplateBtn.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.createTemplateBtn.Appearance = Syncfusion.Windows.Forms.ButtonAppearance.Metro;
            this.createTemplateBtn.BackColor = System.Drawing.Color.MediumAquamarine;
            this.createTemplateBtn.BeforeTouchSize = new System.Drawing.Size(290, 66);
            this.createTemplateBtn.BorderStyleAdv = Syncfusion.Windows.Forms.ButtonAdvBorderStyle.Flat;
            this.createTemplateBtn.FlatAppearance.BorderSize = 0;
            this.createTemplateBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.createTemplateBtn.Font = new System.Drawing.Font("Dosis", 19F);
            this.createTemplateBtn.ForeColor = System.Drawing.Color.White;
            this.createTemplateBtn.Image = global::Synapse.Properties.Resources.Add_New;
            this.createTemplateBtn.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.createTemplateBtn.Location = new System.Drawing.Point(421, 81);
            this.createTemplateBtn.Name = "createTemplateBtn";
            this.createTemplateBtn.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.createTemplateBtn.Size = new System.Drawing.Size(290, 66);
            this.createTemplateBtn.TabIndex = 30;
            this.createTemplateBtn.Text = "     Create Template";
            this.createTemplateBtn.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.createTemplateBtn.ThemeName = "Metro";
            this.createTemplateBtn.Click += new System.EventHandler(this.CreateTemplateBtn_Click);
            // 
            // TemplatesHub
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.ClientSize = new System.Drawing.Size(724, 373);
            this.Controls.Add(this.containerPanel);
            this.Controls.Add(this.deleteTemplateBtn);
            this.Controls.Add(this.editTemplateBtn);
            this.Controls.Add(this.loadTemplateBtn);
            this.Controls.Add(this.importTemplateBtn);
            this.Controls.Add(this.createTemplateBtn);
            this.Font = new System.Drawing.Font("Dosis", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(740, 412);
            this.MinimizeBox = false;
            this.Name = "TemplatesHub";
            this.Padding = new System.Windows.Forms.Padding(2, 2, 2, 5);
            this.ShowToolTip = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Style.InactiveShadowOpacity = ((byte)(50));
            this.Style.ShadowOpacity = ((byte)(0));
            this.Style.TitleBar.BackColor = System.Drawing.Color.White;
            this.Style.TitleBar.Font = new System.Drawing.Font("Dosis", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Style.TitleBar.TextHorizontalAlignment = System.Windows.Forms.HorizontalAlignment.Center;
            this.Text = "Templates Hub";
            this.TopMost = true;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.TemplatesHub_FormClosed);
            this.createTemplatePanel.ResumeLayout(false);
            this.createTemplateNameContainer.ResumeLayout(false);
            this.createTemplateNameContainer.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.createTemplateNameTextBox)).EndInit();
            this.containerPanel.ResumeLayout(false);
            this.containerPanel.PerformLayout();
            this.editTemplatePanel.ResumeLayout(false);
            this.editTemplateNameContainer.ResumeLayout(false);
            this.editTemplateNameContainer.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.editTemplateNameTextBoxExt)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Syncfusion.Windows.Forms.ButtonAdv deleteTemplateBtn;
        private Syncfusion.Windows.Forms.ButtonAdv editTemplateBtn;
        private Syncfusion.Windows.Forms.ButtonAdv loadTemplateBtn;
        private Syncfusion.Windows.Forms.ButtonAdv importTemplateBtn;
        private Syncfusion.Windows.Forms.ButtonAdv createTemplateBtn;
        private System.Windows.Forms.FlowLayoutPanel templatesLayoutPanel;
        private System.Windows.Forms.FlowLayoutPanel createTemplatePanel;
        private Syncfusion.Windows.Forms.Tools.AutoLabel emptyListLabel;
        private System.Windows.Forms.FlowLayoutPanel containerPanel;
        private Syncfusion.Windows.Forms.Tools.TextBoxExt createTemplateNameTextBox;
        private Syncfusion.Windows.Forms.ButtonAdv addTemplateBtn;
        private System.Windows.Forms.Panel createTemplateNameContainer;
        private System.Windows.Forms.FlowLayoutPanel editTemplatePanel;
        private System.Windows.Forms.Panel editTemplateNameContainer;
        private Syncfusion.Windows.Forms.Tools.TextBoxExt editTemplateNameTextBoxExt;
        private Syncfusion.Windows.Forms.ButtonAdv setTemplateNameBtn;
    }
}