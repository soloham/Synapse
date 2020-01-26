namespace Synapse.Modules
{
    partial class SelectFieldsForDBForm
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.selectFieldsBtn = new Syncfusion.Windows.Forms.ButtonAdv();
            this.fieldsDataGrid = new Syncfusion.WinForms.DataGrid.SfDataGrid();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fieldsDataGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.selectFieldsBtn, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.fieldsDataGrid, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(10, 10);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.Padding = new System.Windows.Forms.Padding(0, 10, 0, 0);
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 55F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(364, 371);
            this.tableLayoutPanel1.TabIndex = 49;
            // 
            // selectFieldsBtn
            // 
            this.selectFieldsBtn.Appearance = Syncfusion.Windows.Forms.ButtonAppearance.Metro;
            this.selectFieldsBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(165)))), ((int)(((byte)(220)))));
            this.selectFieldsBtn.BeforeTouchSize = new System.Drawing.Size(364, 45);
            this.selectFieldsBtn.BorderStyleAdv = Syncfusion.Windows.Forms.ButtonAdvBorderStyle.Flat;
            this.selectFieldsBtn.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.selectFieldsBtn.Font = new System.Drawing.Font("Dosis", 15F);
            this.selectFieldsBtn.ForeColor = System.Drawing.Color.White;
            this.selectFieldsBtn.Location = new System.Drawing.Point(0, 326);
            this.selectFieldsBtn.Margin = new System.Windows.Forms.Padding(0);
            this.selectFieldsBtn.MetroColor = System.Drawing.Color.DodgerBlue;
            this.selectFieldsBtn.Name = "selectFieldsBtn";
            this.selectFieldsBtn.Size = new System.Drawing.Size(364, 45);
            this.selectFieldsBtn.TabIndex = 50;
            this.selectFieldsBtn.Text = "COMPLETE SELECTION";
            this.selectFieldsBtn.ThemeName = "Metro";
            this.selectFieldsBtn.UseVisualStyle = true;
            this.selectFieldsBtn.Click += new System.EventHandler(this.selectFieldsBtn_Click);
            // 
            // fieldsDataGrid
            // 
            this.fieldsDataGrid.AccessibleName = "Table";
            this.fieldsDataGrid.AllowDraggingColumns = true;
            this.fieldsDataGrid.AllowFiltering = true;
            this.fieldsDataGrid.AllowResizingColumns = true;
            this.fieldsDataGrid.AllowResizingHiddenColumns = true;
            this.fieldsDataGrid.AllowTriStateSorting = true;
            this.fieldsDataGrid.AutoSizeColumnsMode = Syncfusion.WinForms.DataGrid.Enums.AutoSizeColumnsMode.Fill;
            this.fieldsDataGrid.BackColor = System.Drawing.Color.White;
            this.fieldsDataGrid.CopyOption = Syncfusion.WinForms.DataGrid.Enums.CopyOptions.None;
            this.fieldsDataGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fieldsDataGrid.EnableDataVirtualization = true;
            this.fieldsDataGrid.Font = new System.Drawing.Font("Dosis", 8.249999F);
            this.fieldsDataGrid.HeaderRowHeight = 35;
            this.fieldsDataGrid.Location = new System.Drawing.Point(0, 10);
            this.fieldsDataGrid.Margin = new System.Windows.Forms.Padding(0);
            this.fieldsDataGrid.Name = "fieldsDataGrid";
            this.fieldsDataGrid.PasteOption = Syncfusion.WinForms.DataGrid.Enums.PasteOptions.None;
            this.fieldsDataGrid.SelectionMode = Syncfusion.WinForms.DataGrid.Enums.GridSelectionMode.Multiple;
            this.fieldsDataGrid.ShowBusyIndicator = true;
            this.fieldsDataGrid.ShowSortNumbers = true;
            this.fieldsDataGrid.Size = new System.Drawing.Size(364, 306);
            this.fieldsDataGrid.TabIndex = 49;
            this.fieldsDataGrid.UsePLINQ = true;
            this.fieldsDataGrid.QueryCellStyle += new Syncfusion.WinForms.DataGrid.Events.QueryCellStyleEventHandler(this.fieldsDataGrid_QueryCellStyle);
            // 
            // SelectFieldsForDBForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.ClientSize = new System.Drawing.Size(384, 391);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Font = new System.Drawing.Font("Dosis", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SelectFieldsForDBForm";
            this.Padding = new System.Windows.Forms.Padding(10);
            this.ShowInTaskbar = false;
            this.ShowToolTip = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Style.InactiveShadowOpacity = ((byte)(50));
            this.Style.ShadowOpacity = ((byte)(0));
            this.Style.TitleBar.BackColor = System.Drawing.Color.White;
            this.Style.TitleBar.Font = new System.Drawing.Font("Dosis", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Style.TitleBar.TextHorizontalAlignment = System.Windows.Forms.HorizontalAlignment.Center;
            this.Text = "Select Fields";
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.fieldsDataGrid)).EndInit();
            this.ResumeLayout(false);

        }
        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private Syncfusion.Windows.Forms.ButtonAdv selectFieldsBtn;
        private Syncfusion.WinForms.DataGrid.SfDataGrid fieldsDataGrid;
    }
}