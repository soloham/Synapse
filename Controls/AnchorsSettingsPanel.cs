using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Synapse.Core.Templates;
using System.Threading;

namespace Synapse.Controls
{
    public partial class AnchorsSettingsPanel : UserControl
    {
        #region Properties
        internal List<Template.AnchorAlignmentMethod.Anchor> Anchors { get; set; }
        #endregion

        #region Variables
        private SynchronizationContext synchronizationContext;
        #endregion

        public AnchorsSettingsPanel()
        {
            InitializeComponent();
        }
        internal AnchorsSettingsPanel(List<Template.AnchorAlignmentMethod.Anchor> anchors)
        {
            InitializeComponent();
            synchronizationContext = SynchronizationContext.Current;

            Anchors = anchors;
            InitializeAnchorsPanel(anchors);
        }

        private void InitializeAnchorsPanel(List<Template.AnchorAlignmentMethod.Anchor> anchors)
        {
            for (int i = 0; i < anchors.Count; i++)
            {
                PictureBox curPB = (PictureBox)anchorMethodTablePanel.Controls[i];
                curPB.Image = anchors[i].GetAnchorImage.Bitmap;
            }
        }
    }
}
