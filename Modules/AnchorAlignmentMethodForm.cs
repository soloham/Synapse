using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;
using Synapse.Controls;
using Synapse.Core.Templates;
using Syncfusion.WinForms.Controls;

namespace Synapse.Modules
{
    public partial class AnchorAlignmentMethodForm : SfForm
    {
        #region Enums
        public enum ConfigurationState
        {
            SELECT_DOWNSCALING,
            SELECT_ANCHORS
        }
        #endregion

        #region Properties
        public Image<Gray, byte> TemplateImage { get; set; }
        public ConfigurationState ConfigWalkthroughState { get { return walkthroughState; } set { walkthroughState = value; SetupState(value); } }
        #endregion

        #region Variables

        private Image<Gray, byte> templateImage;

        private int pipelineIndex;
        private string methodName;

        private List<AnchorPlaceholderControl> anchorPlaceholderControls = new List<AnchorPlaceholderControl>();

        private Panel CurrentStatePanel;
        private ConfigurationState walkthroughState;
        private bool isCurrentSetActionCompleted;
        private Action CurrentSetAction;
        private Action CurrentNextAction;
        private Action LastStateAction;

        private Image<Gray, byte> templateImageCopy;
        private Image<Gray, byte> resizedImage;

        private Size selectedSize;
        private double selectedScale;

        private Action SelectionRegionChangedAction;
        private Action SelectionRegionResizedAction;

        private SynchronizationContext synchronizationContext;
        private int totalAnchors;
        private int curAnchorIndex;

        private bool isScaled = false;

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
            MinimumSize = new Size(500, 560);

            MainLayoutPanel.RowStyles[1].SizeType = SizeType.Absolute;
            MainLayoutPanel.RowStyles[1].Height = 130;

            for (int i = 0; i < configureStatesPanel.RowCount; i++)
            {
                configureStatesPanel.RowStyles[i].SizeType = SizeType.Percent;
                configureStatesPanel.RowStyles[i].Height = i == 0 ? 0 : i == 1 ? 0 : i == 2 ? 55 : i == 3 ? 45 : 0;
            }

            if (!curConfigureStatePanel.Controls.Contains(statePanelsPanel))
                curConfigureStatePanel.Controls.Add(statePanelsPanel, 1, 0);
            statePanelsPanel.Dock = DockStyle.Fill;
            statePanelsPanel.Visible = true;

            configureStatesPanel.Visible = true;

            TemplateImage = templateImage == null ? TemplateImage : new Image<Gray, byte>(templateImage);
            templateImageCopy = TemplateImage.Clone();
            imageBox.Image = TemplateImage.Bitmap;

            StartWalkthrough();
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

        #region Configuration Setup
        private void StartWalkthrough()
        {
            nextBtn.Text = "NEXT";
            nextBtn.BackColor = Color.LightSlateGray;

            CurrentStatePanel = null;

            ConfigWalkthroughState = 0;
            CurrentNextAction = new Action(NextState);
        }
        private void InitializeStatePanel(ConfigurationState state)
        {
            walkthroughIndexLabel.Dock = DockStyle.None;
            walkthroughIndexLabel.Text = (int)(ConfigWalkthroughState + 1) + ".";
            walkthroughIndexLabel.Dock = DockStyle.Fill;

            if (CurrentStatePanel != null)
            {
                Panel curStatePanel = CurrentStatePanel;
                curStatePanel.Dock = DockStyle.None;
                curStatePanel.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;
                curStatePanel.Visible = false;
            }

            switch (state)
            {
                case ConfigurationState.SELECT_DOWNSCALING:
                    sizeWidthTextBox.IntegerValue = 0;
                    sizeHeightTextBox.IntegerValue = 0;
                    sizeScaleTrackBar.Minimum = 0;
                    sizeScaleTrackBar.Maximum = 10;
                    sizeScaleTrackBar.Value = 10;

                    CurrentStatePanel = SizeValueStatePanel;
                    break;
                case ConfigurationState.SELECT_ANCHORS:
                    CurrentStatePanel = AnchorsStatePanel;
                    break;
            }

            CurrentStatePanel.Location = new Point(400, 0);
            CurrentStatePanel.Visible = true;
            CurrentStatePanel.Dock = DockStyle.Fill;
        }
        private void SetupState(ConfigurationState walkthroughState)
        {
            InitializeStatePanel(walkthroughState);

            SelectionRegionChangedAction = null;
            SelectionRegionResizedAction = null;

            nextBtn.Text = "NEXT";
            nextBtn.BackColor = Color.LightSlateGray;
            nextBtn.Image = Properties.Resources.Login_Arrow;
            CurrentNextAction = new Action(NextState);

            switch (walkthroughState)
            {
                case ConfigurationState.SELECT_DOWNSCALING:
                    CurrentSetAction = new Action(SetTemplateSize);

                    setBtn.Text = "SET";
                    SetTemplateSize();
                    ValidateState();
                    break;
                case ConfigurationState.SELECT_ANCHORS:
                    CurrentSetAction = new Action(AddSelectedAnchor);

                    anchorsStatePanelLabel.Text = curAnchorIndex == 0 ? "Top Left: " : curAnchorIndex == 1 ? "Top Right: " : curAnchorIndex == 2 ? "Bottom Right: " : curAnchorIndex == 3 ? "Bottom Left: " : curAnchorIndex == 4 ? "Test Region: " : "Anchors";

                    setBtn.Image = Properties.Resources.Login_Arrow;
                    setBtn.Text = "SET";
                    nextBtn.Text = "FINISH";
                    nextBtn.BackColor = Color.MediumTurquoise;
                    nextBtn.Image = Properties.Resources.Check;

                    CurrentNextAction = new Action(EndWalkthrough);
                    break;
            }
        }
        private void NextState()
        {
            if (!isCurrentSetActionCompleted)
            {
                InvalidateState();

                return;
            }

            ValidateState();

            isCurrentSetActionCompleted = false;

            switch (ConfigWalkthroughState)
            {
                case ConfigurationState.SELECT_DOWNSCALING:
                    imageBox.Image = TemplateImage.Bitmap;
                    break;
                case ConfigurationState.SELECT_ANCHORS:
                    break;
            }

            ConfigWalkthroughState++;
        }
        private void ValidateState()
        {
            walkthroughIndexLabel.ForeColor = Color.FromArgb(255, 68, 68, 68);

            switch (ConfigWalkthroughState)
            {
                case ConfigurationState.SELECT_DOWNSCALING:
                    sizeWidthTextBox.ForeColor = Color.FromArgb(255, 68, 68, 68);
                    sizeHeightTextBox.ForeColor = Color.FromArgb(255, 68, 68, 68);
                    break;
                case ConfigurationState.SELECT_ANCHORS:
                    anchorsStatePanelLabel.ForeColor = Color.FromArgb(255, 68, 68, 68);
                    break;
            }

            isCurrentSetActionCompleted = true;
        }
        private void InvalidateState()
        {
            walkthroughIndexLabel.ForeColor = Color.Crimson;

            switch (ConfigWalkthroughState)
            {
                case ConfigurationState.SELECT_DOWNSCALING:
                    sizeWidthTextBox.ForeColor = Color.Crimson;
                    sizeHeightTextBox.ForeColor = Color.Crimson;
                    break;
                case ConfigurationState.SELECT_ANCHORS:
                    anchorsStatePanelLabel.ForeColor = Color.Crimson;
                    break;
            }
            
            isCurrentSetActionCompleted = false;
        }
        private void EndWalkthrough()
        {
            LastStateAction?.Invoke();

            imageBox.SelectNone();

            if (anchorPlaceholderControls.TrueForAll(x => x.IsInitialized) && testPointPlaceholderControl.IsInitialized)
            {
                List<Template.AnchorAlignmentMethod.Anchor> anchors = new List<Template.AnchorAlignmentMethod.Anchor>();
                for (int i = 0; i < anchorPlaceholderControls.Count; i++)
                {
                    anchors.Add(anchorPlaceholderControls[i].GetAnchor);
                }
                Template.AnchorAlignmentMethod.Anchor testPointAnchor = testPointPlaceholderControl.GetAnchor;

                Template.AnchorAlignmentMethod anchorAlignmentMethod = new Template.AnchorAlignmentMethod(anchors, testPointAnchor, templateImage.Size, pipelineIndex, methodName, selectedSize, selectedScale);

                OnConfigurationFinishedEvent?.Invoke(anchorAlignmentMethod);

                ValidateState();
            }
            else
                InvalidateState();
        }

        #region Action Methods
        private void SetTemplateSize()
        {
            TemplateImage = resizedImage;

            selectedSize = TemplateImage.Size;
            selectedScale = sizeScaleTrackBar.Value == 0 ? (double)1 / 15 : (double)sizeScaleTrackBar.Value / 10;
        }
        private void AddSelectedAnchor()
        {
            if (imageBox.SelectionRegion != RectangleF.Empty && imageBox.SelectionRegion.Width != 0 && imageBox.SelectionRegion.Height != 0)
            {
                anchorsStatePanelLabel.Text = curAnchorIndex == 0? "Top Right:" : curAnchorIndex == 1? "Bottom Right:" : curAnchorIndex == 2 ? "Bottom Left:" : curAnchorIndex == 3 ? "Test Region:" : "Anchors:";

                if (curAnchorIndex == testPointPlaceholderControl.Index)
                {
                    testPointPlaceholderControl.Initialize(imageBox.SelectionRegion, TemplateImage.Copy(imageBox.SelectionRegion).Mat, DeleteAnchorAction);
                    testPointPlaceholderControl.IsCurrent = false;
                }
                else
                {

                    AnchorPlaceholderControl anchorPlaceholder = anchorPlaceholderControls[curAnchorIndex];
                    anchorPlaceholder.Initialize(imageBox.SelectionRegion, TemplateImage.Copy(imageBox.SelectionRegion).Mat, DeleteAnchorAction);

                    anchorPlaceholderControls.ForEach(x => x.IsCurrent = false);
                    testPointPlaceholderControl.IsCurrent = false;

                    if (!anchorPlaceholderControls.Exists(x => x.IsInitialized == false))
                    {
                        if (!testPointPlaceholderControl.IsInitialized)
                        {
                            curAnchorIndex = testPointPlaceholderControl.Index;
                            testPointPlaceholderControl.IsCurrent = true;
                        }
                    }
                    else
                    {
                        curAnchorIndex = anchorPlaceholderControls.Find(x => x.IsInitialized == false).Index;
                        anchorPlaceholderControls[curAnchorIndex].IsCurrent = true;
                    }
                }
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

        private void ReconfigureBtn_Click(object sender, EventArgs e)
        {
            MainLayoutPanel.RowStyles[1].Height = 165;
            selectStateComboBox.Visible = true;

            for (int i = 0; i < configureStatesPanel.RowCount; i++)
            {
                configureStatesPanel.RowStyles[i].SizeType = SizeType.Percent;
                configureStatesPanel.RowStyles[i].Height = i == 0 ? 0 : i == 1 ? 35 : i == 2 ? 30 : i == 3 ? 35 : 0;
            }
        }
        private void NextBtn_Click(object sender, EventArgs e)
        {
            CurrentNextAction();
        }
        private void SetBtn_Click(object sender, EventArgs e)
        {
            CurrentSetAction();
        }
        private void SelectStateComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (selectStateComboBox.SelectedIndex == -1)
                return;

            ConfigurationState configurationState = (ConfigurationState)selectStateComboBox.SelectedIndex + 3;
            ConfigWalkthroughState = configurationState;
        }
        private void SizeWidthTextBox_IntegerValueChanged(object sender, EventArgs e)
        {
            if (isScaled)
                return;

            imageBox.SelectionRegion = RectangleF.Empty;
            int width = (int)sizeWidthTextBox.IntegerValue;
            int height = (int)sizeHeightTextBox.IntegerValue;

            if (TemplateImage.Width != width)
            {
                if (height <= 1 || width <= 1)
                    return;

                resizedImage = templateImage.Resize(width, height, Emgu.CV.CvEnum.Inter.Cubic);
                imageBox.Image = TemplateImage.Bitmap;
            }
        }
        private void SizeHeightTextBox_IntegerValueChanged(object sender, EventArgs e)
        {
            if (isScaled)
                return;

            imageBox.SelectionRegion = RectangleF.Empty;
            int width = (int)sizeWidthTextBox.IntegerValue;
            int height = (int)sizeHeightTextBox.IntegerValue;

            if (TemplateImage.Height != height)
            {
                if (height <= 1 || width <= 1)
                    return;

                resizedImage = templateImage.Resize(width, height, Emgu.CV.CvEnum.Inter.Cubic);
                imageBox.Image = TemplateImage.Bitmap;
            }
        }
        private void SizeScaleTrackBar_ValueChanged(object sender, EventArgs e)
        {
            imageBox.SelectionRegion = RectangleF.Empty;
            double scaleValue = sizeScaleTrackBar.Value == 0 ? (double)1 / 20 : (double)sizeScaleTrackBar.Value / 10;

            if (templateImage == null)
                return;

            resizedImage = templateImage.Resize(scaleValue, Emgu.CV.CvEnum.Inter.Cubic);
            imageBox.Image = resizedImage.Bitmap;

            Size resizedSize = resizedImage.Size;

            isScaled = true;
            sizeWidthTextBox.IntegerValue = resizedSize.Width;
            sizeHeightTextBox.IntegerValue = resizedSize.Height;
            isScaled = false;
        }
        #endregion

        #endregion
    }
}