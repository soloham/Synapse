using Synapse.Core.Templates;
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
        #region Properties
        internal Template CurrentTemplate { get { return currentTemplate; } set { } }
        private Template currentTemplate;
        #endregion

        #region Variables
        #endregion

        #region Static Methods
        internal static void RunTemplate(Template template)
        {
            SynapseMain synapseMain = new SynapseMain(template);
            synapseMain.Text = "Synapse - " + template.GetTemplateName;
            synapseMain.Show();
        }
        #endregion

        #region Internal Methods
        internal SynapseMain(Template currentTemplate)
        {
            InitializeComponent();
            this.currentTemplate = currentTemplate;

            Awake();
        }
        #endregion

        #region Private Methods
        #region Main
        void Awake()
        {
            //Pre-Ops
            //-User Interface Setup
            //--Ribbon Tabs Setup
            readingTabPanel.Dock = DockStyle.Fill;
            configTabPanel.Dock = DockStyle.Fill;
            ribbonControl.SelectedTab = configToolStripTabItem;
        }
        #endregion
        #region UI
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
        private void SynapseMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
        #endregion
        #endregion
    }
}
