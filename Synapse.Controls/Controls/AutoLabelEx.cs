using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Synapse.Controls
{
    public partial class AutoLabelEx : Syncfusion.Windows.Forms.Tools.AutoLabel
    {
        #region SystemHandCursorSnippet
        private const int WM_SETCURSOR = 0x0020;
        private const int IDC_HAND = 32649;

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr LoadCursor(IntPtr hInstance, int lpCursorName);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SetCursor(IntPtr hCursor);
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_SETCURSOR)
            {
                // Set the cursor to use the system hand cursor
                SetCursor(LoadCursor(IntPtr.Zero, IDC_HAND));

                // Indicate that the message has been handled
                m.Result = IntPtr.Zero;
                return;
            }

            base.WndProc(ref m);
        }
        #endregion

    }
}
