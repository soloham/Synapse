namespace Synapse.Controls
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Windows.Forms;

    using Synapse.Core.Templates;

    public partial class AnchorsSettingsPanel : UserControl
    {
        #region Properties

        public List<Template.AnchorAlignmentMethod.Anchor> Anchors { get; set; }

        #endregion

        #region Variables

        private SynchronizationContext synchronizationContext;

        #endregion

        public AnchorsSettingsPanel()
        {
            this.InitializeComponent();
        }

        public AnchorsSettingsPanel(List<Template.AnchorAlignmentMethod.Anchor> anchors)
        {
            this.InitializeComponent();
            synchronizationContext = SynchronizationContext.Current;

            this.Anchors = anchors;
            this.InitializeAnchorsPanel(anchors);
        }

        private void InitializeAnchorsPanel(List<Template.AnchorAlignmentMethod.Anchor> anchors)
        {
            for (var i = 0; i < anchors.Count; i++)
            {
                var curPB = (PictureBox)anchorMethodTablePanel.Controls[i];
                curPB.Image = anchors[i].GetAnchorImage.Bitmap;
            }
        }
    }
}