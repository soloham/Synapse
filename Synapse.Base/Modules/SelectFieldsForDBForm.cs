namespace Synapse.Modules
{
    using System;
    using System.Data;
    using System.Windows.Forms;
    using System.Windows.Forms.VisualStyles;

    using Syncfusion.WinForms.Controls;
    using Syncfusion.WinForms.DataGrid.Events;

    public partial class SelectFieldsForDBForm : SfForm
    {
        private readonly DataTable fieldsDT = new DataTable();

        public event EventHandler<string[]> SelectFieldsEvent;

        #region General Methods

        public SelectFieldsForDBForm(string[] fields, string[] selectedFields)
        {
            this.InitializeComponent();

            fieldsDT.Columns.Add("SELECT FIELDS TO INSERT");

            for (var i = 0; i < fields.Length; i++) fieldsDT.Rows.Add(fields[i]);

            fieldsDataGrid.DataSource = fieldsDT;

            if (selectedFields == null || selectedFields.Length == 0)
            {
                return;
            }

            for (var i = 0; i < selectedFields.Length; i++)
                fieldsDataGrid.SelectCell(selectedFields[i], fieldsDataGrid.Columns[0]);
        }

        #endregion

        private void selectFieldsBtn_Click(object sender, EventArgs e)
        {
            var totalSelectedFields = fieldsDataGrid.SelectedItems.Count;
            var selectedFields = new string[totalSelectedFields];
            for (var i = 0; i < totalSelectedFields; i++)
                selectedFields[i] = (fieldsDataGrid.SelectedItems[i] as DataRowView).Row[0].ToString();
            this.SelectFieldsEvent?.Invoke(this, selectedFields);
        }

        private void fieldsDataGrid_QueryCellStyle(object sender, QueryCellStyleEventArgs e)
        {
            e.Style.HorizontalAlignment = HorizontalAlignment.Center;
            e.Style.VerticalAlignment = VerticalAlignment.Center;
        }
    }
}