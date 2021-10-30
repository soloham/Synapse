namespace Synapse.Controls
{
    using System.Drawing;
    using System.Reflection;
    using System.Windows.Forms;

    using Syncfusion.WinForms.DataGrid;

    public partial class SfDataGridEx : Panel
    {
        public SfDataGrid DataGrid = new SfDataGrid();
        public Color BorderColor { get; set; }

        public SfDataGridEx()
        {
            this.DockPadding.All = 1;
            this.BackColor = this.BorderColor;
            this.BorderStyle = BorderStyle.None;
            DataGrid.Dock = DockStyle.Fill;
            DataGrid.Style.BorderStyle = BorderStyle.None;
            var method = DataGrid.GetType().GetMethod("UpdateStyles", BindingFlags.NonPublic | BindingFlags.Instance);
            method.Invoke(DataGrid, null);
            this.Controls.Add(DataGrid);
        }
    }
}