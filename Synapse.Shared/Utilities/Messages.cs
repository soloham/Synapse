using System;
using System.Windows.Forms;
using MessageBoxAdv = Syncfusion.Windows.Forms.MessageBoxAdv;

namespace Synapse.Utilities
{
    public static class Messages
    {
        public static void DeleteDirectoryException(Exception ex, string title = "Hold On", MessageBoxButtons buttons = MessageBoxButtons.OK, MessageBoxIcon icon = MessageBoxIcon.Error)
        {
            MessageBoxAdv.Show("An error occured while deleting the folder. \n \n Error: " + ex.Message, title, buttons, icon);
        }
        public static void DeleteFileException(Exception ex, string title = "Hold On", MessageBoxButtons buttons = MessageBoxButtons.OK, MessageBoxIcon icon = MessageBoxIcon.Error)
        {
            MessageBoxAdv.Show("An error occured while deleting the file. \n \n Error: " + ex.Message, title, buttons, icon);
        }
        public static void LoadFileException(Exception ex, string title = "Hold On", MessageBoxButtons buttons = MessageBoxButtons.OK, MessageBoxIcon icon = MessageBoxIcon.Error)
        {
            MessageBoxAdv.Show("An error occured while loading the file. \n \n Error: " + ex.Message, title, buttons, icon);
        }
        public static void SaveFileException(Exception ex, string title = "Hold On", MessageBoxButtons buttons = MessageBoxButtons.OK, MessageBoxIcon icon = MessageBoxIcon.Error)
        {
            MessageBoxAdv.Show("An error occured while saving the file. \n \n Error: " + ex.Message, title, buttons, icon);
        }
        public static DialogResult ShowQuestion(string question, string title = "Hold On", MessageBoxButtons buttons = MessageBoxButtons.YesNo, MessageBoxIcon icon = MessageBoxIcon.Question)
        {
            return MessageBoxAdv.Show(question, title, buttons, icon);
        }
        public static DialogResult ShowError(string error, string title = "Hold On", MessageBoxButtons buttons = MessageBoxButtons.OK, MessageBoxIcon icon = MessageBoxIcon.Error)
        {
            return MessageBoxAdv.Show(error, title, buttons, icon);
        }
        public static DialogResult ShowInformation(string info, string title = "Info", MessageBoxButtons buttons = MessageBoxButtons.OK, MessageBoxIcon icon = MessageBoxIcon.Information)
        {
            return MessageBoxAdv.Show(info, title, buttons, icon);
        }
    }
}
