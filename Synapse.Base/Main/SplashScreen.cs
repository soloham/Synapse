using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Synapse.Main
{
    public partial class SplashScreen : Form
    {
        public SplashScreen(string splashText)
        {
            InitializeComponent();

            SplashText.Text = splashText;
        }

        public void SetSplashText(string text)
        {
            SplashText.Text = text;
        }

        public void ShowScreen(string text)
        {
            SetSplashText(text);
            Show();
        }
    }
}
