using Synapse.Core;
using Synapse.Utilities;
using Syncfusion.Windows.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Synapse
{
    public partial class SynapseMain : Syncfusion.Windows.Forms.Tools.RibbonForm
    {
        internal Template CurrentTemplate;

        internal static void RunTemplate(Template template)
        {
            SynapseMain synapseMain = new SynapseMain(template);
            synapseMain.Show();
        }

        internal SynapseMain(Template currentTemplate)
        {
            InitializeComponent();
            Awake();

            CurrentTemplate = currentTemplate;
        }
        void Awake()
        {
            //Pre-Ops
            //-User Interface Setup
            //--Ribbon Tabs Setup
            readingTabPanel.Dock = DockStyle.Fill;
            configTabPanel.Dock = DockStyle.Fill;
            ribbonControl.SelectedTab = configToolStripTabItem;
            //--MetroColor table for MessageBoxAdv
            MetroStyleColorTable metroColorTable = new MetroStyleColorTable();
            metroColorTable.NoButtonBackColor = Color.Red;
            metroColorTable.YesButtonBackColor = Color.SkyBlue;
            metroColorTable.OKButtonBackColor = Color.Green;
            MessageBoxAdv.MetroColorTable = metroColorTable;
            MessageBoxAdv.MessageBoxStyle = MessageBoxAdv.Style.Metro;
        }

        private void ConfigToolStripTabItem_Click(object sender, EventArgs e)
        {
            configTabPanel.Visible = true;
            readingTabPanel.Visible = false;
        }
        private void ReadingToolStripTabItem_Click(object sender, EventArgs e)
        {
            readingTabPanel.Visible = true;
            configTabPanel.Visible = false;
        }

        private void TmpLoadBrowseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ImageFileBrowser.ShowDialog() == DialogResult.OK)
            {
                string location = ImageFileBrowser.FileName;
                try
                {
                    Image tmpImage = Image.FromFile(location);
                    templateImageBox.Image = tmpImage;

                    templateImageBox.TextDisplayMode = Cyotek.Windows.Forms.ImageBoxGridDisplayMode.None;
                    templateImageBox.ZoomToFit();
                }
                catch (Exception ex)
                {
                    Messages.LoadFileException(ex);
                }
            }
        }
    }
}
