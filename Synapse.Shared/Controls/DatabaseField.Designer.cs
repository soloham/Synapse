namespace Synapse
{
    partial class DatabaseField
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
            Syncfusion.Windows.Forms.Tools.CustomImageCollection customImageCollection1 = new Syncfusion.Windows.Forms.Tools.CustomImageCollection();
            Syncfusion.Windows.Forms.Tools.ResetButton resetButton1 = new Syncfusion.Windows.Forms.Tools.ResetButton();
            this.Contianer = new System.Windows.Forms.TableLayoutPanel();
            this.container = new System.Windows.Forms.Panel();
            this.verifyPanel = new Syncfusion.Windows.Forms.Tools.RatingControl();
            this.dataFieldValueField = new Syncfusion.Windows.Forms.Tools.TextBoxExt();
            this.editBtn = new Syncfusion.Windows.Forms.ButtonAdv();
            this.removeBtn = new Syncfusion.Windows.Forms.ButtonAdv();
            this.Contianer.SuspendLayout();
            this.container.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataFieldValueField)).BeginInit();
            this.SuspendLayout();
            // 
            // Contianer
            // 
            this.Contianer.ColumnCount = 3;
            this.Contianer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.Contianer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 51F));
            this.Contianer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 51F));
            this.Contianer.Controls.Add(this.container, 0, 0);
            this.Contianer.Controls.Add(this.editBtn, 1, 0);
            this.Contianer.Controls.Add(this.removeBtn, 2, 0);
            this.Contianer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Contianer.Location = new System.Drawing.Point(0, 0);
            this.Contianer.Name = "Contianer";
            this.Contianer.RowCount = 1;
            this.Contianer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.Contianer.Size = new System.Drawing.Size(342, 46);
            this.Contianer.TabIndex = 0;
            // 
            // container
            // 
            this.container.Controls.Add(this.verifyPanel);
            this.container.Controls.Add(this.dataFieldValueField);
            this.container.Dock = System.Windows.Forms.DockStyle.Fill;
            this.container.Location = new System.Drawing.Point(3, 4);
            this.container.Margin = new System.Windows.Forms.Padding(3, 4, 6, 3);
            this.container.Name = "container";
            this.container.Size = new System.Drawing.Size(231, 39);
            this.container.TabIndex = 56;
            // 
            // verifyPanel
            // 
            this.verifyPanel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.verifyPanel.ApplyGradientColors = false;
            this.verifyPanel.BackColor = System.Drawing.Color.White;
            this.verifyPanel.CanOverrideStyle = true;
            this.verifyPanel.Images = customImageCollection1;
            this.verifyPanel.ItemBackColor = System.Drawing.Color.Transparent;
            this.verifyPanel.ItemsCount = 1;
            this.verifyPanel.Location = new System.Drawing.Point(201, 5);
            this.verifyPanel.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
            this.verifyPanel.Name = "verifyPanel";
            resetButton1.Appearance = Syncfusion.Windows.Forms.ButtonAppearance.Metro;
            resetButton1.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.verifyPanel.ResetButton = resetButton1;
            this.verifyPanel.Size = new System.Drawing.Size(30, 25);
            this.verifyPanel.Style = Syncfusion.Windows.Forms.Tools.RatingControl.Styles.Metro;
            this.verifyPanel.TabIndex = 55;
            this.verifyPanel.ThemeName = "Metro";
            this.verifyPanel.Click += new System.EventHandler(this.VerifyPanel_Click);
            // 
            // dataFieldValueField
            // 
            this.dataFieldValueField.BeforeTouchSize = new System.Drawing.Size(231, 39);
            this.dataFieldValueField.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(209)))), ((int)(((byte)(211)))), ((int)(((byte)(212)))));
            this.dataFieldValueField.BorderSides = System.Windows.Forms.Border3DSide.Bottom;
            this.dataFieldValueField.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.dataFieldValueField.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataFieldValueField.Font = new System.Drawing.Font("Dosis", 18.5F);
            this.dataFieldValueField.HideSelection = false;
            this.dataFieldValueField.Location = new System.Drawing.Point(0, 0);
            this.dataFieldValueField.Name = "dataFieldValueField";
            this.dataFieldValueField.Size = new System.Drawing.Size(231, 39);
            this.dataFieldValueField.Style = Syncfusion.Windows.Forms.Tools.TextBoxExt.theme.Metro;
            this.dataFieldValueField.TabIndex = 53;
            this.dataFieldValueField.Text = "Data Field";
            this.dataFieldValueField.ThemeName = "Metro";
            this.dataFieldValueField.WordWrap = false;
            this.dataFieldValueField.EnabledChanged += new System.EventHandler(this.DataFieldValueField_EnabledChanged);
            this.dataFieldValueField.KeyUp += new System.Windows.Forms.KeyEventHandler(this.dataFieldValueField_KeyUp);
            // 
            // editBtn
            // 
            this.editBtn.Appearance = Syncfusion.Windows.Forms.ButtonAppearance.Metro;
            this.editBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(165)))), ((int)(((byte)(220)))));
            this.editBtn.BeforeTouchSize = new System.Drawing.Size(47, 40);
            this.editBtn.BorderStyleAdv = Syncfusion.Windows.Forms.ButtonAdvBorderStyle.Flat;
            this.editBtn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.editBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.editBtn.ForeColor = System.Drawing.Color.White;
            this.editBtn.Image = global::Synapse.Shared.Properties.Resources.pencil_edit_button;
            this.editBtn.Location = new System.Drawing.Point(241, 3);
            this.editBtn.Margin = new System.Windows.Forms.Padding(1, 3, 3, 3);
            this.editBtn.Name = "editBtn";
            this.editBtn.Size = new System.Drawing.Size(47, 40);
            this.editBtn.TabIndex = 52;
            this.editBtn.ThemeName = "Metro";
            this.editBtn.UseVisualStyle = true;
            this.editBtn.Click += new System.EventHandler(this.editBtn_Click);
            // 
            // removeBtn
            // 
            this.removeBtn.Appearance = Syncfusion.Windows.Forms.ButtonAppearance.Metro;
            this.removeBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(165)))), ((int)(((byte)(220)))));
            this.removeBtn.BeforeTouchSize = new System.Drawing.Size(49, 40);
            this.removeBtn.BorderStyleAdv = Syncfusion.Windows.Forms.ButtonAdvBorderStyle.Flat;
            this.removeBtn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.removeBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.removeBtn.ForeColor = System.Drawing.Color.White;
            this.removeBtn.Image = global::Synapse.Shared.Properties.Resources.cross_close_or_delete_circular_interface_button_symbol;
            this.removeBtn.Location = new System.Drawing.Point(292, 3);
            this.removeBtn.Margin = new System.Windows.Forms.Padding(1, 3, 1, 3);
            this.removeBtn.MetroColor = System.Drawing.Color.Tomato;
            this.removeBtn.Name = "removeBtn";
            this.removeBtn.Size = new System.Drawing.Size(49, 40);
            this.removeBtn.TabIndex = 54;
            this.removeBtn.ThemeName = "Metro";
            this.removeBtn.UseVisualStyle = true;
            this.removeBtn.Click += new System.EventHandler(this.removeBtn_Click);
            // 
            // DatabaseField
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.Contianer);
            this.MinimumSize = new System.Drawing.Size(342, 46);
            this.Name = "DatabaseField";
            this.Size = new System.Drawing.Size(342, 46);
            this.Contianer.ResumeLayout(false);
            this.container.ResumeLayout(false);
            this.container.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataFieldValueField)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel Contianer;
        private System.Windows.Forms.Panel container;
        private Syncfusion.Windows.Forms.Tools.RatingControl verifyPanel;
        private Syncfusion.Windows.Forms.Tools.TextBoxExt dataFieldValueField;
        private Syncfusion.Windows.Forms.ButtonAdv editBtn;
        private Syncfusion.Windows.Forms.ButtonAdv removeBtn;
    }
}
