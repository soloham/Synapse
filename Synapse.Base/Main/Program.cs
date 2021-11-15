namespace Synapse
{
    using System;
    using System.Drawing;
    using System.Net;
    using System.Windows.Forms;

    using Synapse.Main;
    using Synapse.Modules;
    using Synapse.Utilities;
    using Synapse.Utilities.Memory;

    using Syncfusion.HighContrastTheme.WinForms;
    using Syncfusion.Licensing;
    using Syncfusion.Windows.Forms;
    using Syncfusion.WinForms.Controls;
    using Syncfusion.WinForms.Themes;

    using Office2016Theme = Syncfusion.WinForms.Themes.Office2016Theme;

    public static class Program
    {
        public static string LicenseKey;
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

            LSTM.Initialize();

            LicenseKey = LSTM.LoadLicenseKey();

            SyncfusionLicenseProvider.RegisterLicense(
                "NTMzMDE5QDMxMzcyZTMzMmUzMExURzFTUm9DU1FuMExMVVpLY0dxTVNiVHhISmMzZmNMYmN1T1BSQ2J2SlE9");
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

            if (string.IsNullOrEmpty(LicenseKey))
            {
                var enterValueForm = new EnterValueForm(false);
                enterValueForm.OnValueSet += (sender, s) =>
                {
                    ValidateLicenseKey(s, isValid =>
                    {
                        if (isValid)
                        {
                            LSTM.SaveLicenseKey(s);
                            enterValueForm.Dispose();

                            if (!DefaultSplashScreen.IsDisposed)
                            {
                                DefaultSplashScreen?.Hide();
                            }
                        }
                        else
                        {
                            Messages.ShowError("Invalid License Key.");
                            Application.Exit();
                        }
                    });
                };
                enterValueForm.ShowDialog();
            }
            else
            {
                ValidateLicenseKey(LicenseKey, isValid =>
                {
                    if (!isValid)
                    {
                        Messages.ShowError("Invalid License Key.");
                        Application.Exit();
                    }

                    if (!DefaultSplashScreen.IsDisposed)
                    {
                        DefaultSplashScreen?.Hide();
                    }
                });
            }

            DefaultSplashScreen.ShowDialog();
            Application.Run(new TemplatesHub());
        }

        public static void ValidateLicenseKey(string key, Action<bool> callback)
        {
            using (var wc = new WebClient())
            {
                wc.DownloadStringAsync(new Uri($"http://synapse-terminal.cf/verify?key={key}"));

                wc.DownloadStringCompleted += (s, es) =>
                {
                    if (es.Cancelled || es.Error != null)
                    {
                        callback.Invoke(false);
                        return;
                    }

                    var contents = es.Result;
                    callback.Invoke(contents == "OK");
                };
            }
        }
    }
}