namespace Synapse.Modules
{
    using System;
    using System.Drawing;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using System.Windows.Forms.VisualStyles;

    using Syncfusion.WinForms.Controls;
    using Syncfusion.XlsIO;

    public partial class SpreadSheetTestForm : SfForm
    {
        public SpreadSheetTestForm()
        {
            this.InitializeComponent();

            this.Style.TitleBar.Height = 26;
            this.Style.TitleBar.BackColor = Color.White;
            this.Style.TitleBar.IconBackColor = Color.FromArgb(15, 161, 212);
            this.BackColor = Color.White;
            this.Style.TitleBar.ForeColor = ColorTranslator.FromHtml("#343434");
            this.Style.TitleBar.CloseButtonForeColor = Color.DarkGray;
            this.Style.TitleBar.MaximizeButtonForeColor = Color.DarkGray;
            this.Style.TitleBar.MinimizeButtonForeColor = Color.DarkGray;
            this.Style.TitleBar.HelpButtonForeColor = Color.DarkGray;
            this.Style.TitleBar.IconHorizontalAlignment = HorizontalAlignment.Left;
            this.Style.TitleBar.Font =
                this.Font = new Font("Microsoft Sans Serif", 11F, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.Style.TitleBar.TextHorizontalAlignment = HorizontalAlignment.Center;
            this.Style.TitleBar.TextVerticalAlignment = VerticalAlignment.Center;
        }

        private async void addNewFieldBtn_Click(object sender, EventArgs e)
        {
            var excelEngine = new ExcelEngine();
            var application = excelEngine.Excel;
            application.DefaultVersion = ExcelVersion.Excel2016;

            var workbook = application.Workbooks.Create(1);
            var namedSheet = workbook.Worksheets.Create("Processed Data");

            spreadsheet1.Open(workbook);

            for (var i = 0; i < 10000; i++)
            {
                namedSheet[$"A{i + 1}"].Text = 5 * (i + 1) + "";
                namedSheet.Activate();
                await Task.Delay(200);
            }
        }
    }
}