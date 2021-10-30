namespace Synapse
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    using Synapse.Main;
    using Synapse.Modules;
    using Synapse.Utilities.Memory;

    using Syncfusion.HighContrastTheme.WinForms;
    using Syncfusion.Licensing;
    using Syncfusion.Windows.Forms;
    using Syncfusion.WinForms.Controls;
    using Syncfusion.WinForms.Themes;

    using Office2016Theme = Syncfusion.WinForms.Themes.Office2016Theme;

    public static class Program
    {
        public static SplashScreen DefaultSplashScreen;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
        }

        public static void Run()
        {
            Application.SetCompatibleTextRenderingDefault(false);

            DefaultSplashScreen = new SplashScreen("Loading...");
            DefaultSplashScreen.Show();

            LSTM.Initialize();

            SyncfusionLicenseProvider.RegisterLicense(
                "MTUyNzg0QDMxMzcyZTMzMmUzMEZudkNxODcvMHhRUmRTQjFzS05uaVBoOENoRjZJQlhSL0dGTGMwNi9CTkE9");
            SfSkinManager.LoadAssembly(typeof(Office2016Theme).Assembly);
            SfSkinManager.LoadAssembly(typeof(Office2019Theme).Assembly);
            SfSkinManager.LoadAssembly(typeof(HighContrastTheme).Assembly);
            Application.EnableVisualStyles();

            //--MetroColor table for MessageBoxAdv
            var metroColorTable = new MetroStyleColorTable();
            metroColorTable.NoButtonBackColor = Color.Red;
            metroColorTable.YesButtonBackColor = Color.SkyBlue;
            metroColorTable.OKButtonBackColor = Color.Green;
            MessageBoxAdv.MetroColorTable = metroColorTable;
            MessageBoxAdv.MessageBoxStyle = MessageBoxAdv.Style.Metro;

            Application.Run(new TemplatesHub());
        }
    }
}