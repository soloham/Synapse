namespace Synapse.Modules
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Windows.Forms;

    using Emgu.CV;
    using Emgu.CV.CvEnum;
    using Emgu.CV.Structure;

    using Synapse.Core.Templates;
    using Synapse.Properties;
    using Synapse.Utilities;
    using Synapse.Utilities.Attributes;
    using Synapse.Utilities.Enums;
    using Synapse.Utilities.Objects;

    using Syncfusion.WinForms.Controls;

    public partial class TemplateConfigurationForm : SfForm
    {
        #region Enums

        public enum ConfigurationState
        {
            [EnumDescription("Apply Deskew")] APPLY_DESKEW,
            [EnumDescription("Perform Crop")] PERFORM_CROP,

            [EnumDescription("Select Template Size")]
            SELECT_TEMPLATE_SIZE,

            [EnumDescription("Configure Alignment Pipeline")]
            CONFIGURE_ALIGNMENT_PIPELINE,

            [EnumDescription("Test Alignment Method")]
            TEST_ALIGNMENT_PIPELINE
        }

        #endregion

        #region Events

        public delegate void OnConfigurationFinshed(TemplateConfigurationForm templateConfigurationForm,
            Template.TemplateImage templateImage, List<Template.AlignmentMethod> alignmentMethods,
            Template.AlignmentPipelineResults alignmentPipelineResults);

        public event OnConfigurationFinshed OnConfigurationFinishedEvent;

        public event EventHandler OnFormInitializedEvent;

        #endregion

        #region Properties

        public Mat TemplateImage { get; set; }

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

        private readonly List<Panel> statePanels = new List<Panel>();
        private Panel CurrentStatePanel;
        private ConfigurationState walkthroughState;
        private bool isCurrentSetActionCompleted;
        private Action CurrentSetAction;
        private Action CurrentNextAction;
        private Action LastStateAction;
        private Action DoubleStateComboAction;
        private Action IntegerStateComboAction;
        private Action SelectionRegionChangedAction;
        private Action SelectionRegionResizedAction;

        private SynchronizationContext synchronizationContext;

        private Template referenceTemplate;

        private bool isScaled;

        #region Template Image

        private Mat templateImageCopy;
        private Mat preCroppedImage;
        private Mat croppedImage;
        private Mat resizedImage;
        private RectangleF cropRegion;

        private Size selectedSize;
        private double selectedScale;
        private Deskew.DeskewType deskewType;
        private double selectedDeskewAngle;

        #endregion

        #region Alignment Methods Configuration

        private List<Template.AlignmentMethod> alignmentMethods = new List<Template.AlignmentMethod>();
        private Template.AlignmentPipelineResults alignmentPipelineResults;

        #endregion

        #endregion

        #region Public Methods

        public TemplateConfigurationForm(Bitmap templateImage)
        {
            this.Initialize(templateImage);
        }

        public TemplateConfigurationForm(Template template)
        {
            this.Initialize(template);
        }

        #endregion

        #region Private Methods

        private void Awake()
        {
            this.InitializeComponent();

            synchronizationContext = SynchronizationContext.Current;

            statePanels.Add(LabelStatePanel);
            statePanels.Add(IntegerValueStatePanel);
            statePanels.Add(ComboBoxStatePanel);
            statePanels.Add(DoubleValueStatePanel);
            statePanels.Add(SizeValueStatePanel);
        }

        private void Initialize(Template template)
        {
            this.Awake();

            this.SetupForConfigured(template);

            this.OnConfigurationFinishedEvent += this.OnConfigurationFinishedCallback;

            this.OnFormInitializedEvent?.Invoke(this, EventArgs.Empty);
        }

        private void Initialize(Bitmap regionImage)
        {
            this.Awake();

            this.SetupForConfiguration(regionImage);

            this.OnConfigurationFinishedEvent += this.OnConfigurationFinishedCallback;

            this.OnFormInitializedEvent?.Invoke(this, EventArgs.Empty);
        }

        private void OnConfigurationFinishedCallback(TemplateConfigurationForm templateConfigurationForm,
            Template.TemplateImage templateImage, List<Template.AlignmentMethod> alignmentMethods,
            Template.AlignmentPipelineResults alignmentPipelineResults)
        {
            this.Close();
        }

        private void SetupForConfiguration(Bitmap template = null)
        {
            this.MinimumSize = new Size(500, 560);

            MainLayoutPanel.RowStyles[1].SizeType = SizeType.Absolute;
            MainLayoutPanel.RowStyles[1].Height = 115;

            for (var i = 0; i < configureStatesPanel.RowCount; i++)
            {
                configureStatesPanel.RowStyles[i].SizeType = SizeType.Percent;
                configureStatesPanel.RowStyles[i].Height = i == 0 ? 0 : i == 1 ? 0 : i == 2 ? 50 : i == 3 ? 50 : 0;
            }

            if (!curConfigureStatePanel.Controls.Contains(statePanelsPanel))
            {
                curConfigureStatePanel.Controls.Add(statePanelsPanel, 1, 0);
            }

            statePanelsPanel.Dock = DockStyle.Fill;
            statePanelsPanel.Visible = true;

            configureStatesPanel.Visible = true;

            this.TemplateImage = template == null ? this.TemplateImage : new Image<Gray, byte>(template).Mat;
            templateImageCopy = this.TemplateImage.Clone();
            imageBox.Image = this.TemplateImage.Bitmap;

            this.StartWalkthrough();
        }

        private void SetupForConfigured(Template template, Mat tmpImg = null)
        {
            //Size = new Size(450, 456);
            this.MinimumSize = new Size(500, 456);

            MainLayoutPanel.RowStyles[1].SizeType = SizeType.Absolute;
            MainLayoutPanel.RowStyles[1].Height = 58;

            for (var i = 0; i < configureStatesPanel.RowCount; i++)
            {
                configureStatesPanel.RowStyles[i].SizeType = SizeType.Percent;
                configureStatesPanel.RowStyles[i].Height = i == 0 ? 100 : i == 1 ? 0 : i == 2 ? 0 : i == 3 ? 0 : 0;
            }

            if (!curConfigureStatePanel.Controls.Contains(statePanelsPanel))
            {
                curConfigureStatePanel.Controls.Add(statePanelsPanel, 1, 0);
            }

            statePanelsPanel.Dock = DockStyle.Fill;
            statePanelsPanel.Visible = true;

            var configStatesList = EnumHelper.ToList(typeof(ConfigurationState));
            configStatesList.RemoveAt(0);
            configStatesList.RemoveAt(0);
            configStatesList.RemoveAt(0);
            selectStateComboBox.DataSource = configStatesList;
            selectStateComboBox.DisplayMember = "Value";
            selectStateComboBox.ValueMember = "Key";

            configureStatesPanel.Visible = true;
            CurrentStatePanel = LabelStatePanel;

            this.TemplateImage = tmpImg == null
                ? new Image<Gray, byte>(template.GetTemplateImage.GetBitmap ??
                                        SynapseMain.GetSynapseMain.GetCurrentImage()).Mat
                : tmpImg.ToImage<Gray, byte>().Mat;
            templateImageCopy = this.TemplateImage.Clone();
            imageBox.Image = this.TemplateImage.Bitmap;

            selectedSize = template.GetTemplateImage.Size;
            selectedScale = template.GetTemplateImage.TemplateScale;
            selectedDeskewAngle = template.GetTemplateImage.DeskewAngle;

            referenceTemplate = template;

            alignmentMethods = new List<Template.AlignmentMethod>(template.TemplateData.GetAlignmentPipeline);
        }

        #region  ImageBoxPanel Setup

        private void imageBox_Paint(object sender, PaintEventArgs e)
        {
            // highlight the image
            if (showImageRegionToolStripButton.Checked)
            {
                Functions.DrawBox(e.Graphics, Color.CornflowerBlue, imageBox.GetImageViewPort(), imageBox.ZoomFactor);
            }

            // show the region that will be drawn from the source image
            if (showSourceImageRegionToolStripButton.Checked)
            {
                Functions.DrawBox(e.Graphics, Color.Firebrick,
                    new RectangleF(imageBox.GetImageViewPort().Location, imageBox.GetSourceImageRegion().Size),
                    imageBox.ZoomFactor);
            }

            if (cropRegion != null && this.ConfigWalkthroughState == ConfigurationState.PERFORM_CROP)
            {
                Functions.DrawBox(e.Graphics, Color.DodgerBlue, imageBox.GetOffsetRectangle(cropRegion),
                    imageBox.ZoomFactor, 160, 1);
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
                case ConfigurationState.APPLY_DESKEW:
                    DoubleStateComboAction = () =>
                    {
                        deskewType = (Deskew.DeskewType)doubleStateComboBox.SelectedIndex;
                        switch (deskewType)
                        {
                            case Deskew.DeskewType.Auto:
                                doubleStateValueTextBox.ReadOnly = true;
                                break;

                            case Deskew.DeskewType.Custom:
                                doubleStateValueTextBox.ReadOnly = false;
                                break;
                        }
                    };

                    doubleStateComboBox.DataSource = EnumHelper.ToList(typeof(Deskew.DeskewType));
                    doubleStateComboBox.DisplayMember = "Value";
                    doubleStateComboBox.ValueMember = "Key";

                    for (var i = 0; i < doubleStageControlsPanel.ColumnCount; i++)
                    {
                        doubleStageControlsPanel.ColumnStyles[i].SizeType = SizeType.Percent;
                        doubleStageControlsPanel.ColumnStyles[i].Width = i == 0 ? 50 : i == 1 ? 50 : 0;
                    }

                    CurrentStatePanel = DoubleValueStatePanel;
                    break;

                case ConfigurationState.PERFORM_CROP:
                    CurrentStatePanel = LabelStatePanel;
                    break;

                case ConfigurationState.SELECT_TEMPLATE_SIZE:
                    sizeWidthTextBox.IntegerValue = 0;
                    sizeHeightTextBox.IntegerValue = 0;
                    sizeScaleTrackBar.Minimum = 0;
                    sizeScaleTrackBar.Maximum = 20;
                    sizeScaleTrackBar.Value = 10;

                    CurrentStatePanel = SizeValueStatePanel;
                    break;

                case ConfigurationState.CONFIGURE_ALIGNMENT_PIPELINE:
                    walkthroughDescriptionLabel.Text = "";

                    CurrentStatePanel = LabelStatePanel;
                    break;

                case ConfigurationState.TEST_ALIGNMENT_PIPELINE:
                    walkthroughDescriptionLabel.Text = "";

                    CurrentStatePanel = LabelStatePanel;
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
                case ConfigurationState.APPLY_DESKEW:
                    doubleStateLabel.Text = "Deskew Image:";
                    CurrentSetAction = this.SetTemplateDeskew;

                    setBtn.Text = "APPLY";
                    this.ValidateState();
                    break;

                case ConfigurationState.PERFORM_CROP:
                    if (preCroppedImage != null)
                    {
                        imageBox.Image = preCroppedImage.Bitmap;
                    }

                    if (croppedImage == null)
                    {
                        croppedImage = this.TemplateImage;
                    }

                    imageBox.SelectionRegion = cropRegion;
                    walkthroughDescriptionLabel.Text = "Select template region to crop";
                    CurrentSetAction = this.CropTemplateImage;

                    setBtn.Text = "APPLY";
                    this.ValidateState();
                    break;

                case ConfigurationState.SELECT_TEMPLATE_SIZE:
                    CurrentSetAction = this.SetTemplateSize;

                    setBtn.Text = "SET";
                    this.SetTemplateSize();
                    this.ValidateState();
                    break;

                case ConfigurationState.CONFIGURE_ALIGNMENT_PIPELINE:
                    walkthroughDescriptionLabel.Text = "Configure the template alignment pipeline";
                    CurrentSetAction = this.ConfigureAlignmentPipeline;

                    setBtn.Image = Resources.Gear_WF;
                    setBtn.Text = "CONFIGURE";
                    break;

                case ConfigurationState.TEST_ALIGNMENT_PIPELINE:
                    walkthroughDescriptionLabel.Text = "Browse a sheet to test the alignment pipeline";
                    CurrentSetAction = this.TestAlignmentPipeline;

                    setBtn.Image = Resources.Login_Arrow;
                    setBtn.Text = "BROWSE";
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
                case ConfigurationState.APPLY_DESKEW:
                    break;

                case ConfigurationState.PERFORM_CROP:
                    imageBox.Image = this.TemplateImage.Bitmap;
                    break;

                case ConfigurationState.SELECT_TEMPLATE_SIZE:
                    imageBox.Image = this.TemplateImage.Bitmap;
                    break;

                case ConfigurationState.CONFIGURE_ALIGNMENT_PIPELINE:
                    break;

                case ConfigurationState.TEST_ALIGNMENT_PIPELINE:
                    break;
            }

            if (referenceTemplate != null)
            {
                selectStateComboBox.SelectedIndex = (int)this.ConfigWalkthroughState + 1 - 3;
            }
            else
            {
                this.ConfigWalkthroughState++;
            }
        }

        private void ValidateState()
        {
            walkthroughIndexLabel.ForeColor = Color.FromArgb(255, 68, 68, 68);

            switch (this.ConfigWalkthroughState)
            {
                case ConfigurationState.SELECT_TEMPLATE_SIZE:
                    sizeWidthTextBox.ForeColor = Color.FromArgb(255, 68, 68, 68);
                    sizeHeightTextBox.ForeColor = Color.FromArgb(255, 68, 68, 68);
                    break;

                case ConfigurationState.APPLY_DESKEW:
                    doubleStateValueTextBox.ForeColor = Color.FromArgb(255, 68, 68, 68);
                    break;

                case ConfigurationState.CONFIGURE_ALIGNMENT_PIPELINE:
                    walkthroughDescriptionLabel.ForeColor = Color.FromArgb(255, 68, 68, 68);
                    break;

                case ConfigurationState.TEST_ALIGNMENT_PIPELINE:
                    walkthroughDescriptionLabel.ForeColor = Color.FromArgb(255, 68, 68, 68);
                    break;
            }

            isCurrentSetActionCompleted = true;
        }

        private void InvalidateState()
        {
            walkthroughIndexLabel.ForeColor = Color.Crimson;

            switch (this.ConfigWalkthroughState)
            {
                case ConfigurationState.SELECT_TEMPLATE_SIZE:
                    sizeWidthTextBox.ForeColor = Color.Crimson;
                    sizeHeightTextBox.ForeColor = Color.Crimson;
                    break;

                case ConfigurationState.APPLY_DESKEW:
                    doubleStateValueTextBox.ForeColor = Color.Crimson;
                    break;

                case ConfigurationState.CONFIGURE_ALIGNMENT_PIPELINE:
                    walkthroughDescriptionLabel.ForeColor = Color.Crimson;
                    break;

                case ConfigurationState.TEST_ALIGNMENT_PIPELINE:
                    walkthroughDescriptionLabel.ForeColor = Color.Crimson;
                    break;
            }

            isCurrentSetActionCompleted = false;
        }

        private void EndWalkthrough()
        {
            LastStateAction?.Invoke();

            if (!isCurrentSetActionCompleted)
            {
                this.InvalidateState();

                return;
            }

            imageBox.SelectNone();

            var tmpImg = new Template.TemplateImage(selectedSize, selectedScale, selectedDeskewAngle);
            tmpImg.SetBitmap(this.TemplateImage.Bitmap);
            if (referenceTemplate != null)
            {
                tmpImg.ImageLocation = referenceTemplate.GetTemplateImage.ImageLocation;
            }

            this.OnConfigurationFinishedEvent?.Invoke(this, tmpImg, alignmentMethods, alignmentPipelineResults);
        }

        #region Action Methods

        private void SetTemplateDeskew()
        {
            switch (deskewType)
            {
                case Deskew.DeskewType.Auto:
                    this.TemplateImage =
                        new Image<Gray, byte>(
                            Deskew.DeskewImage(templateImageCopy.Bitmap, 100, out selectedDeskewAngle)).Mat;
                    imageBox.Image = this.TemplateImage.Bitmap;

                    if (selectedDeskewAngle == 0)
                    {
                        break;
                    }

                    doubleStateValueTextBox.DoubleValue = selectedDeskewAngle;
                    break;

                case Deskew.DeskewType.Custom:
                    selectedDeskewAngle = doubleStateValueTextBox.DoubleValue;

                    if (selectedDeskewAngle == 0)
                    {
                        break;
                    }

                    this.TemplateImage =
                        new Image<Gray, byte>(Deskew.DeskewImage(templateImageCopy.Bitmap, selectedDeskewAngle)).Mat;
                    imageBox.Image = this.TemplateImage.Bitmap;
                    break;
            }
        }

        private void CropTemplateImage()
        {
            preCroppedImage = this.TemplateImage.Clone();

            cropRegion = imageBox.SelectionRegion;

            croppedImage = cropRegion == RectangleF.Empty
                ? this.TemplateImage
                : new Mat(this.TemplateImage, Rectangle.Round(cropRegion));

            imageBox.Invalidate();
        }

        private void SetTemplateSize()
        {
            this.TemplateImage = resizedImage;

            selectedSize = this.TemplateImage.Size;
            selectedScale = sizeScaleTrackBar.Value == 0 ? (double)1 / 20 : (double)sizeScaleTrackBar.Value / 10;
        }

        private void ConfigureAlignmentPipeline()
        {
            var alignmentPipelineConfigurationForm =
                new AlignmentPipelineConfigurationForm(alignmentMethods, this.TemplateImage);
            alignmentPipelineConfigurationForm.OnConfigurationFinishedEvent += alignmentMethods =>
            {
                this.alignmentMethods = alignmentMethods;

                if (this.alignmentMethods.Count == 0)
                {
                    this.InvalidateState();

                    if (referenceTemplate != null)
                    {
                        Messages.ShowError("A Minimum of one Alignment Method is required in order to continue");
                        return;
                    }
                }
                else
                {
                    this.ValidateState();
                }

                alignmentPipelineConfigurationForm.Close();
            };
            alignmentPipelineConfigurationForm.ShowDialog();
        }

        private void TestAlignmentPipeline()
        {
            if (alignmentMethods.Count <= 0)
            {
                Messages.ShowError("A Minimum of one Alignment Method is required in order to perform testing");
                this.InvalidateState();

                return;
            }

            if (ImageFileBrowser.ShowDialog() == DialogResult.OK && File.Exists(ImageFileBrowser.FileName))
            {
                var alignmentPipelineTestForm = new AlignmentPipelineTestForm(alignmentMethods, this.TemplateImage,
                    CvInvoke.Imread(ImageFileBrowser.FileName, ImreadModes.Grayscale));
                alignmentPipelineTestForm.OnResultsGeneratedEvent += alignmentPipelineResults =>
                {
                    this.alignmentPipelineResults = alignmentPipelineResults;

                    if (alignmentPipelineResults.AlignmentMethodTestResultsList.Count > 0 &&
                        alignmentPipelineResults.AlignmentMethodTestResultsList.TrueForAll(x =>
                            x.GetAlignmentMethodResultType ==
                            Template.AlignmentPipelineResults.AlignmentMethodResultType.Successful))
                    {
                        this.ValidateState();
                    }
                    else
                    {
                        this.InvalidateState();

                        if (alignmentPipelineResults.AlignmentMethodTestResultsList.Count > 0)
                        {
                            Messages.ShowError(
                                $"The Alignment Method: '{alignmentPipelineResults.AlignmentMethodTestResultsList.First(x => x.GetAlignmentMethodResultType == Template.AlignmentPipelineResults.AlignmentMethodResultType.Failed).AlignmentMethod.MethodName}' failed to pass the testing process. \n\n Configure this method properly in order to complete the process.");
                        }
                        else
                        {
                            Messages.ShowError(
                                "A minimum of one alignment method must be enabled to apply testing operations.");
                        }

                        this.ConfigWalkthroughState = ConfigurationState.CONFIGURE_ALIGNMENT_PIPELINE;
                    }
                };
                alignmentPipelineTestForm.ShowDialog();
            }
        }

        #endregion

        private void ReconfigureBtn_Click(object sender, EventArgs e)
        {
            MainLayoutPanel.RowStyles[1].Height = 165;
            selectStateComboBox.Visible = true;

            for (var i = 0; i < configureStatesPanel.RowCount; i++)
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
            {
                return;
            }

            var configurationState = (ConfigurationState)selectStateComboBox.SelectedIndex + 3;
            this.ConfigWalkthroughState = configurationState;
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

                CvInvoke.Resize(croppedImage, resizedImage, new Size(width, height));
                //resizedImage = croppedImage.Resize(width, height, Emgu.CV.CvEnum.Inter.Cubic);
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

                CvInvoke.Resize(croppedImage, resizedImage, new Size(width, height));
                //resizedImage = croppedImage.Resize(width, height, Emgu.CV.CvEnum.Inter.Cubic);
                imageBox.Image = resizedImage.Bitmap;
            }
        }

        private void SizeScaleTrackBar_ValueChanged(object sender, EventArgs e)
        {
            imageBox.SelectionRegion = RectangleF.Empty;
            var scaleValue = sizeScaleTrackBar.Value == 0 ? (double)1 / 20 : (double)sizeScaleTrackBar.Value / 10;

            if (croppedImage == null)
            {
                return;
            }

            //CvInvoke.Resize(croppedImage, resizedImage, new Size(width, height));
            resizedImage = croppedImage.ToImage<Gray, byte>().Resize(scaleValue, Inter.Cubic).Mat;
            imageBox.Image = resizedImage.Bitmap;

            var resizedSize = resizedImage.Size;

            isScaled = true;
            sizeWidthTextBox.IntegerValue = resizedSize.Width;
            sizeHeightTextBox.IntegerValue = resizedSize.Height;
            isScaled = false;
        }

        private void IntegerStateComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            IntegerStateComboAction?.Invoke();
        }

        private void DoubleStateComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            DoubleStateComboAction?.Invoke();
        }

        #endregion

        #endregion
    }
}