namespace Synapse.Main
{
    using System.Windows.Forms;

    public partial class SplashScreen : Form
    {
        public SplashScreen(string splashText)
        {
            this.InitializeComponent();

            SplashText.Text = splashText;
        }

        public void SetSplashText(string text)
        {
            SplashText.Text = text;
        }

        public void ShowScreen(string text)
        {
            this.SetSplashText(text);
            this.Show();
        }
    }
}