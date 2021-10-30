namespace Synapse.Modules
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Threading;
    using System.Windows.Forms;

    using Emgu.CV;
    using Emgu.CV.CvEnum;
    using Emgu.CV.Structure;

    using Synapse.Controls;
    using Synapse.Core.Templates;
    using Synapse.Properties;
    using Synapse.Utilities;
    using Synapse.Utilities.Attributes;
    using Synapse.Utilities.Enums;

    using Syncfusion.Windows.Forms.Tools;
    using Syncfusion.WinForms.Controls;

    using Action = System.Action;

    public partial class AnchorAlignmentMethodForm : SfForm
    {
        #region Enums

        public enum ConfigurationState
        {
            [EnumDescription("Select Downscaling Factor")]
            SELECT_DOWNSCALING,
            [EnumDescription("Select Anchors")] SELECT_ANCHORS
        }

        #endregion

        #region Properties

        public Image<Gray, byte> TemplateImage { get; set; }

        public ConfigurationState ConfigWalkthroughState
        {
            get => walkthroughState;
            set
            {
                walkthroughState = value;
                this.SetupState(value);
            }
        }

        #endregion

        #region Variables

        private readonly Mat templateImage;

        private readonly int pipelineIndex;
        private readonly string methodName;

        private Template.AnchorAlignmentMethod referenceAnchorAlignmentMethod;

        private readonly List<AnchorPlaceholderControl>
            anchorPlaceholderControls = new List<AnchorPlaceholderControl>();

        private readonly List<Template.AnchorAlignmentMethod.Anchor> anchors =
            new List<Template.AnchorAlignmentMethod.Anchor>();

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

        private bool isScaled;

        #endregion

        #region Events

        public delegate void OnConfigurationFinshed(Template.AnchorAlignmentMethod anchorAlignmentMethod);

        public event OnConfigurationFinshed OnConfigurationFinishedEvent;

        #endregion

        #region General Methods

        public AnchorAlignmentMethodForm(Template.AnchorAlignmentMethod anchorAlignmentMethod, Mat templateImage)
        {
            this.InitializeComponent();

            this.Awake();

            this.templateImage = templateImage;
            var btm = templateImage.Bitmap;
            pipelineIndex = anchorAlignmentMethod.PipelineIndex;
            methodName = anchorAlignmentMethod.MethodName;
            imageBox.Image = btm;

            this.Initialize(anchorAlignmentMethod, btm);
        }

        public AnchorAlignmentMethodForm(Mat templateImage, int pipelineIndex, string methodName = "Anchors Method")
        {
            this.InitializeComponent();

            this.Awake();

            this.templateImage = templateImage;
            this.pipelineIndex = pipelineIndex;
            this.methodName = methodName;
            var btm = templateImage.Bitmap;
            imageBox.Image = btm;

            this.Initialize(btm);
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
            this.SetupForConfigured(anchorAlignmentMethod, templateImage);
        }

        private void Initialize(Bitmap templateImage)
        {
            this.SetupForConfiguration(templateImage);
        }

        private void OnConfigurationFinishedCallback()
        {
        }

        private void SetupForConfigured(Template.AnchorAlignmentMethod anchorAlignmentMethod, Bitmap templateImage)
        {
            referenceAnchorAlignmentMethod = anchorAlignmentMethod;

            this.MinimumSize = new Size(500, 600);

            MainLayoutPanel.RowStyles[1].SizeType = SizeType.Absolute;
            MainLayoutPanel.RowStyles[1].Height = 58;

            statePanelsPanel.Controls.Remove(SizeValueStatePanel);

            for (var i = 0; i < configureStatesPanel.RowCount; i++)
            {
                configureStatesPanel.RowStyles[i].SizeType = SizeType.Percent;
                configureStatesPanel.RowStyles[i].Height = i == 0 ? 100 : i == 1 ? 0 : i == 2 ? 0 : i == 3 ? 0 : 0;
            }

            var list = EnumHelper.ToList(typeof(ConfigurationState));
            list.RemoveAt(0);
            selectStateComboBox.DataSource = list;
            selectStateComboBox.DisplayMember = "Value";
            selectStateComboBox.ValueMember = "Key";

            if (!curConfigureStatePanel.Controls.Contains(statePanelsPanel))
            {
                curConfigureStatePanel.Controls.Add(statePanelsPanel, 1, 0);
            }

            statePanelsPanel.Dock = DockStyle.Fill;
            statePanelsPanel.Visible = true;

            configureStatesPanel.Visible = true;

            resizedImage = new Image<Gray, byte>(templateImage).Resize(anchorAlignmentMethod.GetDownscaleSize.Width,
                anchorAlignmentMethod.GetDownscaleSize.Height, Inter.Cubic);
            this.TemplateImage = templateImage == null ? this.TemplateImage : resizedImage;
            templateImageCopy = this.TemplateImage.Clone();
            imageBox.Image = this.TemplateImage.Bitmap;

            var refAnchors = anchorAlignmentMethod.GetAnchors;
            for (var i = 0; i < refAnchors.Count; i++)
            {
                var anchorPlaceholder = anchorPlaceholderControls[i];
                anchorPlaceholder.Initialize(refAnchors[i].GetAnchorRegion, refAnchors[i].GetAnchorImage,
                    this.DeleteAnchorAction);
                anchorPlaceholder.IsCurrent = false;
                anchors.Add(anchorPlaceholder.GetAnchor);

                if (!anchorPlaceholderControls.Exists(x => x.IsInitialized == false))
                {
                    continue;
                }

                curAnchorIndex = anchorPlaceholderControls.Find(x => x.IsInitialized == false).Index;
                anchorPlaceholderControls[curAnchorIndex].IsCurrent = true;
            }

            testPointPlaceholderControl.Initialize(anchorAlignmentMethod.GetTestAnchor.GetAnchorRegion,
                anchorAlignmentMethod.GetTestAnchor.GetAnchorImage, this.DeleteAnchorAction);
        }

        private void SetupForConfiguration(Bitmap templateImage = null)
        {
            this.MinimumSize = new Size(500, 560);

            MainLayoutPanel.RowStyles[1].SizeType = SizeType.Absolute;
            MainLayoutPanel.RowStyles[1].Height = 130;

            for (var i = 0; i < configureStatesPanel.RowCount; i++)
            {
                configureStatesPanel.RowStyles[i].SizeType = SizeType.Percent;
                configureStatesPanel.RowStyles[i].Height = i == 0 ? 0 : i == 1 ? 0 : i == 2 ? 55 : i == 3 ? 45 : 0;
            }

            if (!curConfigureStatePanel.Controls.Contains(statePanelsPanel))
            {
                curConfigureStatePanel.Controls.Add(statePanelsPanel, 1, 0);
            }

            statePanelsPanel.Dock = DockStyle.Fill;
            statePanelsPanel.Visible = true;

            configureStatesPanel.Visible = true;

            this.TemplateImage = templateImage == null ? this.TemplateImage : new Image<Gray, byte>(templateImage);
            templateImageCopy = this.TemplateImage.Clone();
            imageBox.Image = this.TemplateImage.Bitmap;

            this.StartWalkthrough();
        }

        #region  ImageBoxPanel Setup

        private void imageBox_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            // highlight the image
            if (showImageRegionToolStripButton.Checked)
            {
                Functions.DrawBox(g, Color.CornflowerBlue, imageBox.GetImageViewPort(), imageBox.ZoomFactor);
            }

            // show the region that will be drawn from the source image
            if (showSourceImageRegionToolStripButton.Checked)
            {
                Functions.DrawBox(g, Color.Firebrick,
                    new RectangleF(imageBox.GetImageViewPort().Location, imageBox.GetSourceImageRegion().Size),
                    imageBox.ZoomFactor);
            }

            if (anchors.Count > 0)
            {
                for (var i = 0; i < anchors.Count; i++)
                {
                    var anchor = anchors[i];

                    var anchorRegion = anchor.GetAnchorRegion;
                    if (originalImageToggle.ToggleState == ToggleButtonState.Active && resizedImage != null)
                    {
                        anchorRegion = Functions.ResizeRegion(anchorRegion, resizedImage.Size, templateImage.Size);
                    }

                    Functions.DrawBox(g, imageBox.GetOffsetRectangle(anchorRegion), imageBox.ZoomFactor,
                        Color.FromArgb(150, Color.DodgerBlue));
                }
            }

            if (testPointPlaceholderControl.IsInitialized)
            {
                var testRegion = testPointPlaceholderControl.GetAnchor.GetAnchorRegion;
                if (originalImageToggle.ToggleState == ToggleButtonState.Active && resizedImage != null)
                {
                    testRegion = Functions.ResizeRegion(testRegion, resizedImage.Size, templateImage.Size);
                }

                Functions.DrawBox(g, imageBox.GetOffsetRectangle(testRegion), imageBox.ZoomFactor,
                    Color.FromArgb(150, Color.MediumAquamarine));
            }
        }

        private void imageBox_Resize(object sender, EventArgs e)
        {
        }

        private void imageBox_Scroll(object sender, ScrollEventArgs e)
        {
        }

        private void imageBox_SelectionRegionChanged(object sender, EventArgs e)
        {
            selectionToolStripStatusLabel.Text = Functions.FormatRectangle(imageBox.SelectionRegion);

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

            this.ConfigWalkthroughState = 0;
            CurrentNextAction = this.NextState;
        }

        private void InitializeStatePanel(ConfigurationState state)
        {
            walkthroughIndexLabel.Dock = DockStyle.None;
            walkthroughIndexLabel.Text = (int)(this.ConfigWalkthroughState + 1) + ".";
            walkthroughIndexLabel.Dock = DockStyle.Fill;

            if (CurrentStatePanel != null)
            {
                var curStatePanel = CurrentStatePanel;
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
            this.InitializeStatePanel(walkthroughState);

            SelectionRegionChangedAction = null;
            SelectionRegionResizedAction = null;

            nextBtn.Text = "NEXT";
            nextBtn.BackColor = Color.LightSlateGray;
            nextBtn.Image = Resources.Login_Arrow;
            CurrentNextAction = this.NextState;

            switch (walkthroughState)
            {
                case ConfigurationState.SELECT_DOWNSCALING:
                    CurrentSetAction = this.SetTemplateSize;

                    setBtn.Text = "SET";
                    this.SetTemplateSize();
                    this.ValidateState();
                    break;

                case ConfigurationState.SELECT_ANCHORS:
                    CurrentSetAction = this.AddSelectedAnchor;

                    anchorsStatePanelLabel.Text = curAnchorIndex == 0 ? "Top Left: " :
                        curAnchorIndex == 1 ? "Top Right: " :
                        curAnchorIndex == 2 ? "Bottom Right: " :
                        curAnchorIndex == 3 ? "Bottom Left: " :
                        curAnchorIndex == 4 ? "Test Region: " : "Anchors";

                    setBtn.Image = Resources.Login_Arrow;
                    setBtn.Text = "SET";
                    nextBtn.Text = "FINISH";
                    nextBtn.BackColor = Color.MediumTurquoise;
                    nextBtn.Image = Resources.Check;

                    CurrentNextAction = this.EndWalkthrough;
                    break;
            }
        }

        private void NextState()
        {
            if (!isCurrentSetActionCompleted)
            {
                this.InvalidateState();

                return;
            }

            this.ValidateState();

            isCurrentSetActionCompleted = false;

            switch (this.ConfigWalkthroughState)
            {
                case ConfigurationState.SELECT_DOWNSCALING:
                    imageBox.Image = this.TemplateImage.Bitmap;
                    break;

                case ConfigurationState.SELECT_ANCHORS:
                    break;
            }

            this.ConfigWalkthroughState++;
        }

        private void ValidateState()
        {
            walkthroughIndexLabel.ForeColor = Color.FromArgb(255, 68, 68, 68);

            switch (this.ConfigWalkthroughState)
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

            switch (this.ConfigWalkthroughState)
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
                var anchors = new List<Template.AnchorAlignmentMethod.Anchor>();
                for (var i = 0; i < anchorPlaceholderControls.Count; i++)
                    anchors.Add(anchorPlaceholderControls[i].GetAnchor);
                var testPointAnchor = testPointPlaceholderControl.GetAnchor;

                if (referenceAnchorAlignmentMethod != null)
                {
                    referenceAnchorAlignmentMethod.SetAnchors(anchors);
                    referenceAnchorAlignmentMethod.SetTestAnchor(testPointAnchor);

                    this.OnConfigurationFinishedEvent?.Invoke(referenceAnchorAlignmentMethod);
                }
                else
                {
                    var anchorAlignmentMethod = new Template.AnchorAlignmentMethod(anchors, testPointAnchor,
                        templateImage.Size, pipelineIndex, methodName, selectedSize, selectedScale);
                    this.OnConfigurationFinishedEvent?.Invoke(anchorAlignmentMethod);
                }

                this.ValidateState();
            }
            else
            {
                this.InvalidateState();
            }
        }

        #region Action Methods

        private void SetTemplateSize()
        {
            this.TemplateImage = resizedImage;

            selectedSize = this.TemplateImage.Size;
            selectedScale = sizeScaleTrackBar.Value == 0 ? (double)1 / 15 : (double)sizeScaleTrackBar.Value / 10;
        }

        private void AddSelectedAnchor()
        {
            if (imageBox.SelectionRegion != RectangleF.Empty && imageBox.SelectionRegion.Width != 0 &&
                imageBox.SelectionRegion.Height != 0 && (anchorPlaceholderControls.Exists(x => x.IsCurrent) ||
                                                         testPointPlaceholderControl.IsCurrent))
            {
                var selectionRegion = imageBox.SelectionRegion;
                if (originalImageToggle.ToggleState == ToggleButtonState.Active && resizedImage != null)
                {
                    selectionRegion = Functions.ResizeRegion(selectionRegion, templateImage.Size, resizedImage.Size);
                }

                if (curAnchorIndex == testPointPlaceholderControl.Index)
                {
                    testPointPlaceholderControl.Initialize(selectionRegion,
                        this.TemplateImage.Copy(selectionRegion).Mat, this.DeleteAnchorAction);
                    testPointPlaceholderControl.IsCurrent = false;
                }
                else
                {
                    var anchorPlaceholder = anchorPlaceholderControls[curAnchorIndex];
                    anchorPlaceholder.Initialize(selectionRegion, this.TemplateImage.Copy(selectionRegion).Mat,
                        this.DeleteAnchorAction);

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

                    anchors.Add(anchorPlaceholder.GetAnchor);
                }
            }

            if (curAnchorIndex != testPointPlaceholderControl.Index)
            {
                anchorsStatePanelLabel.Text = anchorPlaceholderControls[curAnchorIndex].IsInitialized ? "Anchors:" :
                    curAnchorIndex == 0 ? "Top Left:" :
                    curAnchorIndex == 1 ? "Top Right:" :
                    curAnchorIndex == 2 ? "Bottom Right:" :
                    curAnchorIndex == 3 ? "Bottom Left:" : "Anchors:";
            }
            else
            {
                anchorsStatePanelLabel.Text = testPointPlaceholderControl.IsInitialized ? "Anchors:" : "Test Region:";
            }

            imageBox.Invalidate();
        }

        private void DeleteAnchorAction(AnchorPlaceholderControl anchorPlaceholder)
        {
            if (anchors.Contains(anchorPlaceholder.GetAnchor))
            {
                anchors.Remove(anchorPlaceholder.GetAnchor);
            }

            anchorPlaceholder.Reset();
            anchorPlaceholderControls.ForEach(x => x.IsCurrent = false);
            testPointPlaceholderControl.IsCurrent = false;
            if (!anchorPlaceholderControls.Exists(x => x.IsInitialized == false))
            {
                if (testPointPlaceholderControl.IsInitialized == false)
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

            anchorsStatePanelLabel.Text = curAnchorIndex == 0 ? "Top Left:" :
                curAnchorIndex == 1 ? "Top Right:" :
                curAnchorIndex == 2 ? "Bottom Right:" :
                curAnchorIndex == 3 ? "Bottom Left:" :
                curAnchorIndex == 4 ? "Test Region:" : "Anchors:";

            imageBox.Invalidate();
        }

        #endregion

        private void ReconfigureBtn_Click(object sender, EventArgs e)
        {
            MainLayoutPanel.RowStyles[1].Height = 185;
            selectStateComboBox.Visible = true;

            for (var i = 0; i < configureStatesPanel.RowCount; i++)
            {
                configureStatesPanel.RowStyles[i].SizeType = SizeType.Percent;
                configureStatesPanel.RowStyles[i].Height = i == 0 ? 0 : i == 1 ? 35 : i == 2 ? 35 : i == 3 ? 30 : 0;
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
            {
                return;
            }

            if (referenceAnchorAlignmentMethod != null)
            {
                this.ConfigWalkthroughState = ConfigurationState.SELECT_ANCHORS;
            }
            else
            {
                var configurationState = (ConfigurationState)selectStateComboBox.SelectedIndex;
                this.ConfigWalkthroughState = configurationState;
            }
        }

        private void SizeWidthTextBox_IntegerValueChanged(object sender, EventArgs e)
        {
            if (isScaled)
            {
                return;
            }

            imageBox.SelectionRegion = RectangleF.Empty;
            var width = (int)sizeWidthTextBox.IntegerValue;
            var height = (int)sizeHeightTextBox.IntegerValue;

            if (this.TemplateImage.Width != width)
            {
                if (height <= 1 || width <= 1)
                {
                    return;
                }

                CvInvoke.Resize(templateImage, resizedImage, new Size(width, height));
                //resizedImage = templateImage.Resize(width, height, Emgu.CV.CvEnum.Inter.Cubic);
                imageBox.Image = resizedImage.Bitmap;
            }
        }

        private void SizeHeightTextBox_IntegerValueChanged(object sender, EventArgs e)
        {
            if (isScaled)
            {
                return;
            }

            imageBox.SelectionRegion = RectangleF.Empty;
            var width = (int)sizeWidthTextBox.IntegerValue;
            var height = (int)sizeHeightTextBox.IntegerValue;

            if (this.TemplateImage.Height != height)
            {
                if (height <= 1 || width <= 1)
                {
                    return;
                }

                CvInvoke.Resize(templateImage, resizedImage, new Size(width, height));
                //resizedImage = templateImage.Resize(width, height, Emgu.CV.CvEnum.Inter.Cubic);
                imageBox.Image = resizedImage.Bitmap;
            }
        }

        private void SizeScaleTrackBar_ValueChanged(object sender, EventArgs e)
        {
            imageBox.SelectionRegion = RectangleF.Empty;
            var scaleValue = sizeScaleTrackBar.Value == 0 ? (double)1 / 20 : (double)sizeScaleTrackBar.Value / 10;

            if (templateImage == null)
            {
                return;
            }

            //CvInvoke.Resize(templateImage, resizedImage, new Size(width, height));
            resizedImage = templateImage.ToImage<Gray, byte>().Resize(scaleValue, Inter.Cubic);
            imageBox.Image = resizedImage.Bitmap;

            var resizedSize = resizedImage.Size;

            isScaled = true;
            sizeWidthTextBox.IntegerValue = resizedSize.Width;
            sizeHeightTextBox.IntegerValue = resizedSize.Height;
            isScaled = false;
        }

        #endregion

        #endregion

        private void OriginalImageToggle_ToggleStateChanged(object sender, ToggleStateChangedEventArgs e)
        {
            switch (e.ToggleState)
            {
                case ToggleButtonState.Active:
                    imageBox.Image = templateImage.Bitmap;

                    if (imageBox.SelectionRegion != RectangleF.Empty)
                    {
                        imageBox.SelectionRegion = Functions.ResizeRegion(imageBox.SelectionRegion, resizedImage.Size,
                            templateImage.Size);
                    }

                    break;

                case ToggleButtonState.Inactive:
                    imageBox.Image = resizedImage.Bitmap;

                    if (imageBox.SelectionRegion != RectangleF.Empty)
                    {
                        imageBox.SelectionRegion = Functions.ResizeRegion(imageBox.SelectionRegion, templateImage.Size,
                            resizedImage.Size);
                    }

                    break;
            }
        }
    }
}