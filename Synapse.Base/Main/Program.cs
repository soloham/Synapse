using Synapse.Main;
using Syncfusion.HighContrastTheme.WinForms;
using Syncfusion.Windows.Forms;
using Syncfusion.WinForms.Controls;
using Syncfusion.WinForms.Themes;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Synapse
{
    public static class Program
    {
        public static SplashScreen DefaultSplashScreen;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>

        [STAThread]
        static void Main()
        {

        }

        public static void Run()
        {
            Application.SetCompatibleTextRenderingDefault(false);

            DefaultSplashScreen = new SplashScreen("Loading...");
            DefaultSplashScreen.Show();

            Utilities.Memory.LSTM.Initialize();

            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MTUyNzg0QDMxMzcyZTMzMmUzMEZudkNxODcvMHhRUmRTQjFzS05uaVBoOENoRjZJQlhSL0dGTGMwNi9CTkE9");
            SfSkinManager.LoadAssembly(typeof(Syncfusion.WinForms.Themes.Office2016Theme).Assembly);
            SfSkinManager.LoadAssembly(typeof(Office2019Theme).Assembly);
            SfSkinManager.LoadAssembly(typeof(HighContrastTheme).Assembly);
            Application.EnableVisualStyles();

            //--MetroColor table for MessageBoxAdv
            MetroStyleColorTable metroColorTable = new MetroStyleColorTable();
            metroColorTable.NoButtonBackColor = Color.Red;
            metroColorTable.YesButtonBackColor = Color.SkyBlue;
            metroColorTable.OKButtonBackColor = Color.Green;
            MessageBoxAdv.MetroColorTable = metroColorTable;
            MessageBoxAdv.MessageBoxStyle = MessageBoxAdv.Style.Metro;

            Application.Run(new Modules.TemplatesHub());
        }
    }
}
