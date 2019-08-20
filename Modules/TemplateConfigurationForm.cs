using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;
using Emgu.CV;
using Emgu.CV.Structure;
using Synapse.Core.Templates;
using Synapse.Utilities.Attributes;
using Synapse.Utilities.Enums;
using Synapse.Utilities.Objects;
using Syncfusion.WinForms.Controls;
using WinFormAnimation;

namespace Synapse.Modules
{
    public partial class TemplateConfigurationForm : SfForm
    {
        #region Enums
        public enum ConfigurationState
        {
            [EnumDescription("Apply Deskew")]
            APPLY_DESKEW,
            [EnumDescription("Perform Crop")]
            PERFORM_CROP,
            [EnumDescription("Select Template Size")]
            SELECT_TEMPLATE_SIZE,
            [EnumDescription("Configure Alignment Pipeline")]
            CONFIGURE_ALIGNMENT_PIPELINE,
            [EnumDescription("Test Alignment Method")]
            TEST_ALIGNMENT_PIPELINE,
        }
        #endregion

        #region Events 

        public delegate void OnConfigurationFinshed(Bitmap templateImage);
        public event OnConfigurationFinshed OnConfigurationFinishedEvent;

        public event EventHandler OnFormInitializedEvent;

        #endregion

        #region Properties
        public Image<Gray, byte> TemplateImage { get; set; }

        public ConfigurationState ConfigWalkthroughState { get { return walkthroughState; } set { walkthroughState = value; SetupState(value); } }
        #endregion

        #region Variables

        private List<Panel> statePanels = new List<Panel>();
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

        #region Template Image
        private Image<Gray, byte> templateImageCopy;
        private Image<Gray, byte> preCroppedImage;
        private Image<Gray, byte> croppedImage;
        private RectangleF cropRegion;

        private Size selectedSize;
        private double selectedScale;
        private Deskew.DeskewType deskewType;
        private double selectedDeskewAngle;

        #endregion

        #endregion

        #region Public Methods
        public TemplateConfigurationForm(Bitmap templateImage)
        {
            Initialize(templateImage);
        }
        internal TemplateConfigurationForm(Template template)
        {
            Initialize(template);
        }
        #endregion

        #region Private Methods
        private void Awake()
        {
            InitializeComponent();

            synchronizationContext = SynchronizationContext.Current;

            statePanels.Add(LabelStatePanel);
            statePanels.Add(IntegerValueStatePanel);
            statePanels.Add(ComboBoxStatePanel);
            statePanels.Add(DoubleValueStatePanel);
            statePanels.Add(SizeValueStatePanel);
        }

        private void Initialize(Template template)
        {
            Awake();

            SetupForConfigured(template);

            OnConfigurationFinishedEvent += OnConfigurationFinishedCallback;

            OnFormInitializedEvent?.Invoke(this, EventArgs.Empty);
        }
        private void Initialize(Bitmap regionImage)
        {
            Awake();

            SetupForConfiguration(regionImage);

            OnConfigurationFinishedEvent += OnConfigurationFinishedCallback;

            OnFormInitializedEvent?.Invoke(this, EventArgs.Empty);
        }
        private void OnConfigurationFinishedCallback(Bitmap templateImage)
        {

        }

        private void SetupForConfiguration(Bitmap template = null)
        {
            MinimumSize = new Size(500, 560);

            MainLayoutPanel.RowStyles[1].SizeType = SizeType.Absolute;
            MainLayoutPanel.RowStyles[1].Height = 115;

            for (int i = 0; i < configureStatesPanel.RowCount; i++)
            {
                configureStatesPanel.RowStyles[i].SizeType = SizeType.Percent;
                configureStatesPanel.RowStyles[i].Height = i == 0 ? 0 : i == 1 ? 0 : i == 2 ? 50 : i == 3 ? 50 : 0;
            }
            
            if(!curConfigureStatePanel.Controls.Contains(statePanelsPanel))
                curConfigureStatePanel.Controls.Add(statePanelsPanel, 1, 0);
            statePanelsPanel.Dock = DockStyle.Fill;
            statePanelsPanel.Visible = true;

            configureStatesPanel.Visible = true;

            TemplateImage = template == null ? TemplateImage : new Image<Gray, byte>(template);
            templateImageCopy = TemplateImage.Clone();
            imageBox.Text = "";
            imageBox.Image = TemplateImage.Bitmap;

            StartWalkthrough();
        }
        private void SetupForConfigured(Template template, Bitmap tmpImg = null)
        {
            //Size = new Size(450, 456);
            MinimumSize = new Size(500, 456);

            MainLayoutPanel.RowStyles[1].SizeType = SizeType.Absolute;
            MainLayoutPanel.RowStyles[1].Height = 58;

            for (int i = 0; i < configureStatesPanel.RowCount; i++)
            {
                configureStatesPanel.RowStyles[i].SizeType = SizeType.Percent;
                configureStatesPanel.RowStyles[i].Height = i == 0 ? 100 : i == 1 ? 0 : i == 2 ? 0 : i == 3 ? 0 : 0;
            }

            selectStateComboBox.DataSource = EnumHelper.ToList(typeof(ConfigurationState));
            selectStateComboBox.DisplayMember = "Value";
            selectStateComboBox.ValueMember = "Key";

            configureStatesPanel.Visible = true;
            CurrentStatePanel = LabelStatePanel;

            TemplateImage = tmpImg == null ? TemplateImage : new Image<Gray, byte>(tmpImg);
            templateImageCopy = TemplateImage.Clone();
            imageBox.Text = "";
            imageBox.Image = tmpImg;
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

            if (cropRegion != null && ConfigWalkthroughState == ConfigurationState.PERFORM_CROP)
                Utilities.Functions.DrawBox(e.Graphics, Color.DodgerBlue, imageBox.GetOffsetRectangle(cropRegion), imageBox.ZoomFactor, 160, 1);
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
            new Animator2D(new Path2D(walkthroughIndexLabel.Location.ToFloat2D(), new Float2D(0, 50), 160)).Play(walkthroughIndexLabel, Animator2D.KnownProperties.Location, new SafeInvoker(() =>
            {
                synchronizationContext.Send(new SendOrPostCallback(
                    delegate (object st)
                    {
                        walkthroughIndexLabel.Text = (int)(ConfigWalkthroughState + 1) + ".";
                    }),
                null);

                new Animator2D(new Path2D(walkthroughIndexLabel.Location.ToFloat2D(), new Float2D(0, 0), 160)).Play(walkthroughIndexLabel, Animator2D.KnownProperties.Location, new SafeInvoker(() =>
                {
                    synchronizationContext.Send(new SendOrPostCallback(
                    delegate (object st)
                    {
                        walkthroughIndexLabel.Dock = DockStyle.Fill;
                    }),
                    null);
                }));
            }));

            if (CurrentStatePanel != null)
            {
                Panel curStatePanel = CurrentStatePanel;
                curStatePanel.Dock = DockStyle.None;
                curStatePanel.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;
                new Animator2D(new Path2D(CurrentStatePanel.Location.ToFloat2D(), new Float2D(-400, 0), 250)).Play(CurrentStatePanel, Animator2D.KnownProperties.Location, new SafeInvoker(() =>
                {
                    synchronizationContext.Send(new SendOrPostCallback(
                    delegate (object st)
                    {
                        curStatePanel.Visible = false;
                    }),
                    null);
                }));
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

                    for (int i = 0; i < doubleStageControlsPanel.ColumnCount; i++)
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
                    
                    CurrentStatePanel = IntegerValueStatePanel;
                    break;
            }

            CurrentStatePanel.Location = new Point(400, 0);
            CurrentStatePanel.Visible = true;
            new Animator2D(new Path2D(CurrentStatePanel.Location.ToFloat2D(), new Float2D(0, 0), 250)).Play(CurrentStatePanel, Animator2D.KnownProperties.Location, new SafeInvoker(() =>
            {
                synchronizationContext.Send(new SendOrPostCallback(
                    delegate (object st)
                    {
                        CurrentStatePanel.Dock = DockStyle.Fill;
                    }),
                null);
            }));
        }
        private void SetupState(ConfigurationState walkthroughState)
        {
            InitializeStatePanel(walkthroughState);

            SelectionRegionChangedAction = null;
            SelectionRegionResizedAction = null;
            switch (walkthroughState)
            {
                case ConfigurationState.APPLY_DESKEW:
                    doubleStateLabel.Text = "Deskew Image:";
                    CurrentSetAction = new Action(SetTemplateDeskew);

                    setBtn.Text = "APPLY";
                    nextBtn.Text = "NEXT";
                    nextBtn.BackColor = Color.LightSlateGray;
                    ValidateState();
                    CurrentNextAction = new Action(NextState);
                    break;
                case ConfigurationState.PERFORM_CROP:
                    if(preCroppedImage != null) imageBox.Image = preCroppedImage.Bitmap;
                    if(croppedImage == null) croppedImage = TemplateImage;
                    imageBox.SelectionRegion = cropRegion;
                    walkthroughDescriptionLabel.Text = "Select template region to crop";
                    CurrentSetAction = new Action(CropTemplateImage);

                    setBtn.Text = "APPLY";
                    nextBtn.Text = "NEXT";
                    nextBtn.BackColor = Color.LightSlateGray;
                    ValidateState();
                    CurrentNextAction = new Action(NextState);
                    break;
                case ConfigurationState.SELECT_TEMPLATE_SIZE:
                    CurrentSetAction = new Action(SetTemplateSize);

                    setBtn.Text = "SET";
                    nextBtn.Text = "NEXT";
                    nextBtn.BackColor = Color.LightSlateGray;
                    ValidateState();
                    CurrentNextAction = new Action(NextState);
                    break;
                case ConfigurationState.CONFIGURE_ALIGNMENT_PIPELINE:
                    walkthroughDescriptionLabel.Text = "Configure the template alignment pipeline";
                    CurrentSetAction = new Action(ConfigureAlignmentPipeline);

                    setBtn.Image = Properties.Resources.Gear_WF;
                    setBtn.Text = "CONFIGURE";
                    nextBtn.Text = "NEXT";
                    nextBtn.BackColor = Color.LightSlateGray;
                    CurrentNextAction = new Action(NextState);
                    break;
                case ConfigurationState.TEST_ALIGNMENT_PIPELINE:


                    nextBtn.Text = "FINISH";
                    nextBtn.BackColor = Color.MediumTurquoise;
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
                case ConfigurationState.APPLY_DESKEW:
                    break;
                case ConfigurationState.PERFORM_CROP:
                    imageBox.Image = TemplateImage.Bitmap;
                    break;
                case ConfigurationState.SELECT_TEMPLATE_SIZE:
                    break;
                case ConfigurationState.CONFIGURE_ALIGNMENT_PIPELINE:
                    break;
                case ConfigurationState.TEST_ALIGNMENT_PIPELINE:
                    break;
            }

            ConfigWalkthroughState++;
        }
        private void ValidateState()
        {
            walkthroughIndexLabel.ForeColor = Color.FromArgb(255, 68, 68, 68);

            switch (ConfigWalkthroughState)
            {
                case ConfigurationState.SELECT_TEMPLATE_SIZE:
                    sizeWidthTextBox.ForeColor = Color.FromArgb(255, 68, 68, 68);
                    sizeHeightTextBox.ForeColor = Color.FromArgb(255, 68, 68, 68);
                    break;
                case ConfigurationState.APPLY_DESKEW:
                    doubleStateValueTextBox.ForeColor = Color.FromArgb(255, 68, 68, 68);
                    break;
                case ConfigurationState.CONFIGURE_ALIGNMENT_PIPELINE:
                    break;
                case ConfigurationState.TEST_ALIGNMENT_PIPELINE:
                    break;
            }

            isCurrentSetActionCompleted = true;
        }
        private void InvalidateState()
        {
            walkthroughIndexLabel.ForeColor = Color.Crimson;

            switch (ConfigWalkthroughState)
            {
                case ConfigurationState.SELECT_TEMPLATE_SIZE:
                    sizeWidthTextBox.ForeColor = Color.Crimson;
                    sizeHeightTextBox.ForeColor = Color.Crimson;
                    break;
                case ConfigurationState.APPLY_DESKEW:
                    doubleStateValueTextBox.ForeColor = Color.Crimson;
                    break;
                case ConfigurationState.CONFIGURE_ALIGNMENT_PIPELINE:
                    break;
                case ConfigurationState.TEST_ALIGNMENT_PIPELINE:
                    break;
            }

            isCurrentSetActionCompleted = false;
        }
        private void EndWalkthrough()
        {
            LastStateAction();

            if (!isCurrentSetActionCompleted)
                return;

            imageBox.SelectNone();

            //OMRRegionData regionData = new OMRRegionData(totalFields, fieldsRegion, interFieldsSpaceType, interFieldsSpace, interFieldsSpaces.ToArray(), totalOptions, optionsRegion, interOptionsSpaceType, interOptionsSpace, interOptionsSpaces.ToArray());
            //OnConfigurationFinishedEvent?.Invoke(TemplateName, orientation, regionData);
        }

        #region Action Methods
        private void SetTemplateDeskew()
        {
            switch (deskewType)
            {
                case Deskew.DeskewType.Auto:
                    TemplateImage = Deskew.DeskewImage(templateImageCopy, 100, out selectedDeskewAngle);
                    imageBox.Image = TemplateImage.Bitmap;

                    if (selectedDeskewAngle == 0)
                        break;

                    doubleStateValueTextBox.DoubleValue = selectedDeskewAngle;
                    break;
                case Deskew.DeskewType.Custom:
                    selectedDeskewAngle = doubleStateValueTextBox.DoubleValue;

                    if (selectedDeskewAngle == 0)
                        break;

                    TemplateImage = Deskew.DeskewImage(templateImageCopy, selectedDeskewAngle);
                    imageBox.Image = TemplateImage.Bitmap;
                    break;
            }
        }
        private void CropTemplateImage()
        {
            preCroppedImage = TemplateImage.Clone();

            cropRegion = imageBox.SelectionRegion;

            croppedImage = cropRegion == RectangleF.Empty? TemplateImage : TemplateImage.Copy(cropRegion);

            imageBox.Invalidate();
        }
        private void SetTemplateSize()
        {
            selectedSize = TemplateImage.Size;
            selectedScale = sizeScaleTrackBar.Value == 0 ? (double)1/20 : (double)sizeScaleTrackBar.Value/10;
        }
        private void ConfigureAlignmentPipeline()
        {
            AlignmentPipelineConfigurationForm alignmentPipelineConfigurationForm = new AlignmentPipelineConfigurationForm(new List<Template.AlignmentMethod>());
            alignmentPipelineConfigurationForm.ShowDialog();
        }
        #endregion

        private void ReconfigureBtn_Click(object sender, EventArgs e)
        {
            SetupForConfiguration();
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
            ConfigurationState configurationState = (ConfigurationState)selectStateComboBox.SelectedIndex;
            ConfigWalkthroughState = configurationState;
        }
        private void IntegerStateComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            IntegerStateComboAction?.Invoke();
        }
        private void DoubleStateComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            DoubleStateComboAction?.Invoke();
        }
        private void SizeScaleTrackBar_ValueChanged(object sender, EventArgs e)
        {
            imageBox.SelectionRegion = RectangleF.Empty;
            double scaleValue = sizeScaleTrackBar.Value == 0 ? (double)1 / 20 : (double)sizeScaleTrackBar.Value / 10;

            TemplateImage = croppedImage.Resize(scaleValue, Emgu.CV.CvEnum.Inter.Cubic);
            imageBox.Image = TemplateImage.Bitmap;

            sizeWidthTextBox.IntegerValue = TemplateImage.Size.Width;
            sizeHeightTextBox.IntegerValue = TemplateImage.Size.Height;
        }
        private void SizeWidthTextBox_IntegerValueChanged(object sender, EventArgs e)
        {
            imageBox.SelectionRegion = RectangleF.Empty;
            int width = (int)sizeWidthTextBox.IntegerValue;
            int height = (int)sizeHeightTextBox.IntegerValue;

            if (TemplateImage.Width != width)
            {
                if (height <= 1 || width <= 1)
                    return;

                TemplateImage = croppedImage.Resize(width, height, Emgu.CV.CvEnum.Inter.Cubic);
                imageBox.Image = TemplateImage.Bitmap;
            }
        }
        private void SizeHeightTextBox_IntegerValueChanged(object sender, EventArgs e)
        {
            imageBox.SelectionRegion = RectangleF.Empty;
            int width = (int)sizeWidthTextBox.IntegerValue;
            int height = (int)sizeHeightTextBox.IntegerValue;

            if (TemplateImage.Height != height)
            {
                if (height <= 1 || width <= 1)
                    return;

                TemplateImage = croppedImage.Resize(width, height, Emgu.CV.CvEnum.Inter.Cubic);
                imageBox.Image = TemplateImage.Bitmap;
            }
        }
        #endregion

        #endregion
    }
} 