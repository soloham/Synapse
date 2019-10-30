using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Syncfusion.WinForms.DataGrid;
using System.Reflection;

namespace Synapse.Controls
{
    public partial class SfDataGridEx : Panel
    {
        public SfDataGrid DataGrid = new SfDataGrid();
        public Color BorderColor { get; set; }

        public SfDataGridEx() : base()
        {
            this.DockPadding.All = 1;
            this.BackColor = BorderColor;
            this.BorderStyle = BorderStyle.None;
            this.DataGrid.Dock = DockStyle.Fill;
            this.DataGrid.Style.BorderStyle = BorderStyle.None;
            MethodInfo method = this.DataGrid.GetType().GetMethod("UpdateStyles", BindingFlags.NonPublic | BindingFlags.Instance);
            method.Invoke(this.DataGrid, null);
            this.Controls.Add(this.DataGrid);
        }
    }
}
