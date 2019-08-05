using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MessageBoxAdv = Syncfusion.Windows.Forms.MessageBoxAdv;

namespace Synapse.Utilities
{
    internal static class Messages
    {
        public static void LoadFileException(Exception ex, string title = "Hold On", MessageBoxButtons buttons = MessageBoxButtons.OK, MessageBoxIcon icon = MessageBoxIcon.Error)
        {
            MessageBoxAdv.Show("An error occured while loading the file. \n \n Error: " + ex.Message, title, buttons, icon);
        }
        public static void SaveFileException(Exception ex, string title = "Hold On", MessageBoxButtons buttons = MessageBoxButtons.OK, MessageBoxIcon icon = MessageBoxIcon.Error)
        {
            MessageBoxAdv.Show("An error occured while saving the file. \n \n Error: " + ex.Message, title, buttons, icon);
        }
    }
}
