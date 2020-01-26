using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Drawing;
using Synapse.Core.Configurations;
using Synapse.Core.Managers;
using Synapse.Core.Templates;
using Syncfusion.WinForms.Controls;
using System.Threading;
using System.Windows.Threading;
using Synapse.Utilities.Objects;
using System.Threading.Tasks;
using Synapse.Utilities;
using System.Data;
using System.Collections.ObjectModel;

namespace Synapse.Modules
{
    public partial class SelectFieldsForDBForm : SfForm
    {
        private DataTable fieldsDT = new DataTable();

        public event EventHandler<string[]> SelectFieldsEvent;
        #region General Methods
        public SelectFieldsForDBForm(string[] fields, string[] selectedFields)
        {
            InitializeComponent();

            fieldsDT.Columns.Add("SELECT FIELDS TO INSERT");

            for (int i = 0; i < fields.Length; i++)
            {
                fieldsDT.Rows.Add(fields[i]);
            }

            fieldsDataGrid.DataSource = fieldsDT;

            if (selectedFields == null || selectedFields.Length == 0) return;
            for (int i = 0; i < selectedFields.Length; i++)
            {
                fieldsDataGrid.SelectCell(selectedFields[i], fieldsDataGrid.Columns[0]);
            }
        }
        #endregion

        private void selectFieldsBtn_Click(object sender, EventArgs e)
        {
            int totalSelectedFields = fieldsDataGrid.SelectedItems.Count;
            string[] selectedFields = new string[totalSelectedFields];
            for (int i = 0; i < totalSelectedFields; i++)
            {
                selectedFields[i] = (fieldsDataGrid.SelectedItems[i] as DataRowView).Row[0].ToString();
            }
            SelectFieldsEvent?.Invoke(this, selectedFields);
        }

        private void fieldsDataGrid_QueryCellStyle(object sender, Syncfusion.WinForms.DataGrid.Events.QueryCellStyleEventArgs e)
        {
            e.Style.HorizontalAlignment = HorizontalAlignment.Center;
            e.Style.VerticalAlignment = System.Windows.Forms.VisualStyles.VerticalAlignment.Center;
        }
    }
}