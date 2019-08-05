using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Synapse
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Utilities.LSTM.Initialize();

            Application.Run(new global::Synapse.Modules.TemplatesHub());
        }
    }
}
