using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;
using Synapse.Controls;
using Synapse.Core.Templates;
using Synapse.Utilities;
using Synapse.Utilities.Attributes;
using Synapse.Utilities.Enums;
using Synapse.Utilities.Memory;
using Syncfusion.WinForms.Controls;

namespace Synapse.Modules
{
    public partial class AnchorAlignmentMethodForm : SfForm
    {
        #region Enums

        #endregion

        #region Properties

        #endregion

        #region Variables

        private Image<Gray, byte> templateImage;

        private int pipelineIndex;
        private string methodName;

        private List<AnchorPlaceholderControl> anchorPlaceholderControls = new List<AnchorPlaceholderControl>();

        private Action SelectionRegionChangedAction;
        private Action SelectionRegionResizedAction;

        private SynchronizationContext synchronizationContext;
        private int totalAnchors;
        private int curAnchorIndex;

        #endregion

        #region Events

        internal delegate void OnConfigurationFinshed(Template.AnchorAlignmentMethod anchorAlignmentMethod);
        internal event OnConfigurationFinshed OnConfigurationFinishedEvent;

        #endregion

        #region General Methods
        internal AnchorAlignmentMethodForm(Template.AnchorAlignmentMethod anchorAlignmentMethod, Image<Gray, byte> templateImage)
        {
            InitializeComponent();

            Awake();

            this.templateImage = templateImage;
            var btm = templateImage.ToBitmap();
            pipelineIndex = anchorAlignmentMethod.PipelineIndex;
            methodName = anchorAlignmentMethod.MethodName;
            imageBox.Image = btm;

            Initialize(anchorAlignmentMethod, btm);

        }
        internal AnchorAlignmentMethodForm(Image<Gray, byte> templateImage, int pipelineIndex, string methodName = "Anchors Method")
        {
            InitializeComponent();

            Awake();

            this.templateImage = templateImage;
            this.pipelineIndex = pipelineIndex;
            this.methodName = methodName;
            var btm = templateImage.ToBitmap();
            imageBox.Image = btm;

            Initialize(btm);

        }
        private void Awake()
        {
            synchronizationContext = SynchronizationContext.Current;

            anchorPlaceholderControls.Add(anchorPlaceholderControl1);
            anchorPlaceholderControls.Add(anchorPlaceholderControl2);
            anchorPlaceholderControls.Add(anchorPlaceholderControl3);
            anchorPlaceholderControls.Add(anchorPlaceholderControl4);
        }

        #endregion

        #region Private Methods

        private void Initialize(Template.AnchorAlignmentMethod anchorAlignmentMethod, Bitmap templateImage)
        {
            SetupForConfigured(anchorAlignmentMethod, templateImage);
        }
        private void Initialize(Bitmap templateImage)
        {
            SetupForConfiguration(templateImage);
        }
        private void OnConfigurationFinishedCallback()
        {
        }

        private void SetupForConfigured(Template.AnchorAlignmentMethod anchorAlignmentMethod, Bitmap templateImage)
        {
            var anchors = anchorAlignmentMethod.GetAnchors;
            for (int i = 0; i < anchors.Count; i++)
            {
                AnchorPlaceholderControl anchorPlaceholder = anchorPlaceholderControls[i];
                anchorPlaceholder.Initialize(anchors[i].GetAnchorRegion, (Mat)anchors[i].GetAnchorImage, DeleteAnchorAction);

                anchorPlaceholder.IsCurrent = false;

                if (!anchorPlaceholderControls.Exists(x => x.IsInitialized == false))
                    continue;

                curAnchorIndex = anchorPlaceholderControls.Find(x => x.IsInitialized == false).Index;
                anchorPlaceholderControls[curAnchorIndex].IsCurrent = true;
            }
        }
        private void SetupForConfiguration(Bitmap templateImage = null)
        {

        }

        #region  ImageBoxPanel Setup
        private void imageBox_Paint(object sender, PaintEventArgs e)
        {
            // highlight the image
            if (showImageRegionToolStripButton.Checked)
                Utilities.Functions.DrawBox(e.Graphics, Color.CornflowerBlue, imageBox.GetImageViewPort(), imageBox.ZoomFactor);

            // show the region that will be drawn from the source image
            if (showSourceImageRegionToolStripButton.Checked)
                Utilities.Functions.DrawBox(e.Graphics, Color.Firebrick, new RectangleF(imageBox.GetImageViewPort().Location, imageBox.GetSourceImageRegion().Size), imageBox.ZoomFactor);
        }
        private void imageBox_Resize(object sender, EventArgs e)
        {

        }
        private void imageBox_Scroll(object sender, ScrollEventArgs e)
        {

        }
        private void imageBox_SelectionRegionChanged(object sender, EventArgs e)
        {
            selectionToolStripStatusLabel.Text = Utilities.Functions.FormatRectangle(imageBox.SelectionRegion);

            SelectionRegionChangedAction?.Invoke();
        }
        private void imageBox_Selected(object sender, EventArgs e)
        {

        }
        private void ImageBox_SelectionResized(object sender, EventArgs e)
        {
            SelectionRegionResizedAction?.Invoke();
        }

        private void actualSizeToolStripButton_Click(object sender, EventArgs e)
        {
            imageBox.ActualSize();
        }
        private void selectAllToolStripButton_Click(object sender, EventArgs e)
        {
            imageBox.SelectAll();
        }
        private void selectNoneToolStripButton_Click(object sender, EventArgs e)
        {
            imageBox.SelectNone();
        }
        private void showImageRegionToolStripButton_Click(object sender, EventArgs e)
        {
            imageBox.Invalidate();
        }
        private void zoomInToolStripButton_Click(object sender, EventArgs e)
        {
            imageBox.ZoomIn();
        }
        private void zoomOutToolStripButton_Click(object sender, EventArgs e)
        {
            imageBox.ZoomOut();
        }
        #endregion

        #region Configuration State
        private void ReconfigureBtn_Click(object sender, EventArgs e)
        {
            SetupForConfiguration();
        }

        private void SetBtn_Click(object sender, EventArgs e)
        {
            AddSelectedAnchor();
        }
        private void DoneBtn_Click(object sender, EventArgs e)
        {
            if(anchorPlaceholderControls.TrueForAll(x => x.IsInitialized))
            {
                List<Template.AnchorAlignmentMethod.Anchor> anchors = new List<Template.AnchorAlignmentMethod.Anchor>();
                for (int i = 0; i < anchorPlaceholderControls.Count; i++)
                {
                    anchors.Add(anchorPlaceholderControls[i].GetAnchor);
                }
                Template.AnchorAlignmentMethod anchorAlignmentMethod = new Template.AnchorAlignmentMethod(anchors, templateImage.Size, pipelineIndex, methodName);

                OnConfigurationFinishedEvent?.Invoke(anchorAlignmentMethod);
            }
        }

        private void AddSelectedAnchor()
        {
            if(imageBox.SelectionRegion != RectangleF.Empty && imageBox.SelectionRegion.Width != 0 && imageBox.SelectionRegion.Height != 0)
            {
                AnchorPlaceholderControl anchorPlaceholder = anchorPlaceholderControls[curAnchorIndex];
                anchorPlaceholder.Initialize(imageBox.SelectionRegion, templateImage.Copy(imageBox.SelectionRegion).Mat, DeleteAnchorAction);

                anchorPlaceholderControls.ForEach(x => x.IsCurrent = false);

                if (!anchorPlaceholderControls.Exists(x => x.IsInitialized == false))
                    return;

                curAnchorIndex = anchorPlaceholderControls.Find(x => x.IsInitialized == false).Index;
                anchorPlaceholderControls[curAnchorIndex].IsCurrent = true;
            }
        }

        private void DeleteAnchorAction(AnchorPlaceholderControl anchorPlaceholder)
        {
            anchorPlaceholder.Reset();
            
            anchorPlaceholderControls.ForEach(x => x.IsCurrent = false);
            curAnchorIndex = anchorPlaceholderControls.Find(x => x.IsInitialized == false).Index;
            anchorPlaceholderControls[curAnchorIndex].IsCurrent = true;
        }
        #endregion

        #endregion
    }
}