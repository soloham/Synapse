using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Synapse.Core.Configurations;
using Synapse.Utilities.Attributes;
using Synapse.Utilities.Enums;
using Syncfusion.WinForms.Controls;
using WinFormAnimation;
using static Synapse.Core.Configurations.OMRRegionData;

namespace Synapse.Modules
{
    public partial class OMRConfigurationForm : SfForm
    {
        #region Enums
        public enum WalkthroughStates
        {
            [EnumDescription("Select Field Region")]
            SELECT_FIELD_REGION,
            [EnumDescription("Provide Total Fields")]
            PROVIDE_TOTAL_FIELDS,
            [EnumDescription("Select Inter Fields Space Type")]
            SELECT_INTER_FIELDS_SPACE_TYPE,
            [EnumDescription("Select Inter Fields Space")]
            SELECT_INTER_FIELDS_SPACE,

            [EnumDescription("Select Option Region")]
            SELECT_OPTION_REGION,
            [EnumDescription("Provide Total Options")]
            PROVIDE_TOTAL_OPTIONS,
            [EnumDescription("Select Inter Options Space Type")]
            SELECT_INTER_OPTIONS_SPACE_TYPE,
            [EnumDescription("Select Inter Options Space")]
            SELECT_INTER_OPTIONS_SPACE,
        }
        #endregion

        #region Properties
        public OMRRegionData RegionData { get; set; }
        public Bitmap RegionImage { get; set; }
        public WalkthroughStates WalkthroughState { get { return walkthroughState; } set { walkthroughState = value; SetupState(value); } }
        #endregion

        #region Variables

        private List<Panel> statePanels = new List<Panel>();
        private Panel CurrentStatePanel;
        private WalkthroughStates walkthroughState;
        private bool isCurrentSetActionCompleted;
        private Action CurrentSetAction;
        private Action CurrentNextAction;
        private Action LastStateAction;

        private SynchronizationContext synchronizationContext;

        #region Fields Variables
        private RectangleF fieldsRegion;
        private int totalFields;
        private InterSpaceType interFieldsSpaceType;
        private double interFieldsSpace;
        private double[] interFieldsSpaces;
        #endregion

        #region Options Variables
        private RectangleF optionsRegion;
        private int totalOptions;
        private InterSpaceType interOptionsSpaceType;
        private double interOptionsSpace;
        private double[] interOptionsSpaces;
        #endregion

        #endregion

        #region Public Methods
        public OMRConfigurationForm(Bitmap regionImage)
        {
            InitializeComponent();

            Initialize();

            SetupForInitialization(regionImage);
        }
        public OMRConfigurationForm(OMRRegionData regionData, Bitmap regionImage)
        {
            InitializeComponent();

            Initialize();

            SetupForConfiguration(regionData, regionImage);
        }
        #endregion

        #region Private Methods
        private void Initialize()
        {
            synchronizationContext = SynchronizationContext.Current;

            statePanels.Add(generalStatePanel);
            statePanels.Add(totalObjectsStatePanel);
            statePanels.Add(interSpaceTypeStatePanel);
            statePanels.Add(interSpaceValueStatePanel);

            LastStateAction = SetInterOptionsSpace;
        }
        private void SetupForInitialization(Bitmap region)
        {
            Size = new Size(450, 505);
            MinimumSize = Size;
            walkthroughPanel.Visible = true;

            RegionImage = region;
            imageBox.Text = "";
            imageBox.Image = RegionImage;

            StartWalkthrough();
        }
        private void SetupForConfiguration(OMRRegionData regionData, Bitmap region)
        {
            walkthroughPanel.Visible = false;
            imageBoxPanel.Dock = DockStyle.Fill;
            Size = new Size(450, 395);
            MinimumSize = Size;

            RegionData = regionData;
            RegionImage = region;
            imageBox.Text = "";
            imageBox.Image = region;


        }

        #region  ImageBoxPanel Setup
        private void DrawBox(Graphics graphics, Color color, RectangleF rectangle, double scale)
        {
            float penWidth;

            penWidth = 2 * (float)scale;

            using (SolidBrush brush = new SolidBrush(Color.FromArgb(64, color)))
            {
                graphics.FillRectangle(brush, rectangle);
            }

            using (Pen pen = new Pen(color, penWidth)
            {
                DashStyle = DashStyle.Dot,
                DashCap = DashCap.Round
            })
            {
                graphics.DrawRectangle(pen, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
            }
        }

        private void imageBox_Paint(object sender, PaintEventArgs e)
        {
            // highlight the image
            if (showImageRegionToolStripButton.Checked)
                this.DrawBox(e.Graphics, Color.CornflowerBlue, imageBox.GetImageViewPort(), imageBox.ZoomFactor);

            // show the region that will be drawn from the source image
            if (showSourceImageRegionToolStripButton.Checked)
                this.DrawBox(e.Graphics, Color.Firebrick, new RectangleF(imageBox.GetImageViewPort().Location, imageBox.GetSourceImageRegion().Size), imageBox.ZoomFactor);
        }
        private void imageBox_Resize(object sender, EventArgs e)
        {

        }
        private void imageBox_Scroll(object sender, ScrollEventArgs e)
        {

        }
        private void imageBox_Selected(object sender, EventArgs e)
        {
        }
        private void imageBox_SelectionRegionChanged(object sender, EventArgs e)
        {
            selectionToolStripStatusLabel.Text = Utilities.Functions.FormatRectangle(imageBox.SelectionRegion);
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

        #region WalkthroughPanel Setup
        private void StartWalkthrough()
        {
            WalkthroughState = WalkthroughStates.SELECT_FIELD_REGION;
            CurrentNextAction = new Action(NextState);
        }
        private void SetStatePanel(WalkthroughStates state)
        {
            walkthroughIndexLabel.Dock = DockStyle.None;
            new Animator2D(new Path2D(walkthroughIndexLabel.Location.ToFloat2D(), new Float2D(0, 50), 160)).Play(walkthroughIndexLabel, Animator2D.KnownProperties.Location, new SafeInvoker(() =>
            {
                synchronizationContext.Send(new SendOrPostCallback(
                    delegate (object st)
                    {
                        walkthroughIndexLabel.Text = (int)(WalkthroughState + 1) + ".";
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
                    case WalkthroughStates.SELECT_FIELD_REGION:
                        walkthroughDescriptionLabel.Text = "";

                        CurrentStatePanel = generalStatePanel;
                        break;
                    case WalkthroughStates.PROVIDE_TOTAL_FIELDS:
                        totalObjectsTextBox.Clear();

                        CurrentStatePanel = totalObjectsStatePanel;
                        break;
                    case WalkthroughStates.SELECT_INTER_FIELDS_SPACE_TYPE:
                        interSpaceTypeComboBox.DataSource = EnumHelper.ToList(typeof(InterSpaceType));
                        interSpaceTypeComboBox.DisplayMember = "Value";
                        interSpaceTypeComboBox.ValueMember = "Key";

                        CurrentStatePanel = interSpaceTypeStatePanel;
                        break;
                    case WalkthroughStates.SELECT_INTER_FIELDS_SPACE:
                        interSpaceValueTextBox.Clear();

                        CurrentStatePanel = interSpaceValueStatePanel;
                        break;

                    case WalkthroughStates.SELECT_OPTION_REGION:
                        CurrentStatePanel = generalStatePanel;
                        break;
                    case WalkthroughStates.PROVIDE_TOTAL_OPTIONS:
                        totalObjectsTextBox.Clear();

                        CurrentStatePanel = totalObjectsStatePanel;
                        break;
                    case WalkthroughStates.SELECT_INTER_OPTIONS_SPACE_TYPE:
                        interSpaceTypeComboBox.DataSource = EnumHelper.ToList(typeof(InterSpaceType));
                        interSpaceTypeComboBox.DisplayMember = "Value";
                        interSpaceTypeComboBox.ValueMember = "Key";

                        CurrentStatePanel = interSpaceTypeStatePanel;
                        break;
                    case WalkthroughStates.SELECT_INTER_OPTIONS_SPACE:
                        interSpaceValueTextBox.Clear();

                        CurrentStatePanel = interSpaceValueStatePanel;
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
        private void SetupState(WalkthroughStates walkthroughState)
        {
            SetStatePanel(walkthroughState);

            switch (walkthroughState)
            {
                case WalkthroughStates.SELECT_FIELD_REGION:
                    walkthroughDescriptionLabel.Text = "Please select the first field region.";

                    CurrentSetAction = new Action(SetFieldRegion);
                    break;
                case WalkthroughStates.PROVIDE_TOTAL_FIELDS:
                    totalObjectsLabel.Text = "Total Fields:";

                    CurrentSetAction = new Action(SetTotalFields);
                    break;
                case WalkthroughStates.SELECT_INTER_FIELDS_SPACE_TYPE:
                    interSpaceTypeComboBox.Text = "Inter Fields Space Type";

                    CurrentSetAction = new Action(SetInterFieldsSpaceType);
                    break;
                case WalkthroughStates.SELECT_INTER_FIELDS_SPACE:
                    interSpaceValueLabel.Text = "Inter Fields Space:";

                    CurrentSetAction = new Action(SetInterFieldsSpace);
                    break;

                case WalkthroughStates.SELECT_OPTION_REGION:
                    walkthroughDescriptionLabel.Text = "Please select the first option region.";

                    CurrentSetAction = new Action(SetOptionRegion);
                    break;
                case WalkthroughStates.PROVIDE_TOTAL_OPTIONS:
                    totalObjectsLabel.Text = "Total Options:";

                    totalObjectsTextBox.Clear();
                    CurrentSetAction = new Action(SetTotalOptions);
                    break;
                case WalkthroughStates.SELECT_INTER_OPTIONS_SPACE_TYPE:
                    interSpaceTypeComboBox.Text = "Inter Options Space Type";

                    CurrentSetAction = new Action(SetInterOptionsSpaceType);
                    break;
                case WalkthroughStates.SELECT_INTER_OPTIONS_SPACE:
                    interSpaceValueLabel.Text = "Inter Options Space:";

                    CurrentSetAction = new Action(SetInterOptionsSpace);

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
            else
            {
                ValidateState();
            }

            isCurrentSetActionCompleted = false;
            imageBox.SelectNone();

            WalkthroughState++;
        }
        private void ValidateState()
        {
            walkthroughIndexLabel.ForeColor = Color.FromArgb(255, 68, 68, 68);
            switch (WalkthroughState)
            {
                case WalkthroughStates.SELECT_FIELD_REGION:
                    walkthroughDescriptionLabel.ForeColor = Color.FromArgb(255, 68, 68, 68);
                    break;
                case WalkthroughStates.PROVIDE_TOTAL_FIELDS:
                    totalObjectsLabel.ForeColor = Color.FromArgb(255, 68, 68, 68);
                    break;
                case WalkthroughStates.SELECT_INTER_FIELDS_SPACE_TYPE:
                    interSpaceTypeComboBox.ForeColor = Color.FromArgb(255, 68, 68, 68);
                    break;
                case WalkthroughStates.SELECT_INTER_FIELDS_SPACE:
                    interSpaceValueLabel.ForeColor = Color.FromArgb(255, 68, 68, 68);
                    break;

                case WalkthroughStates.SELECT_OPTION_REGION:
                    walkthroughDescriptionLabel.ForeColor = Color.FromArgb(255, 68, 68, 68);
                    break;
                case WalkthroughStates.PROVIDE_TOTAL_OPTIONS:
                    totalObjectsLabel.ForeColor = Color.FromArgb(255, 68, 68, 68);
                    break;
                case WalkthroughStates.SELECT_INTER_OPTIONS_SPACE_TYPE:
                    interSpaceTypeComboBox.ForeColor = Color.FromArgb(255, 68, 68, 68);
                    break;
                case WalkthroughStates.SELECT_INTER_OPTIONS_SPACE:
                    interSpaceValueLabel.ForeColor = Color.FromArgb(255, 68, 68, 68);
                    break;
                default:
                    break;
            }    

            isCurrentSetActionCompleted = true;
        }
        private void InvalidateState()
        {
            walkthroughIndexLabel.ForeColor = Color.Crimson;
            switch (WalkthroughState)
            {
                case WalkthroughStates.SELECT_FIELD_REGION:
                    walkthroughDescriptionLabel.ForeColor = Color.Crimson;
                    break;
                case WalkthroughStates.PROVIDE_TOTAL_FIELDS:
                    totalObjectsLabel.ForeColor = Color.Crimson;
                    break;
                case WalkthroughStates.SELECT_INTER_FIELDS_SPACE_TYPE:
                    interSpaceTypeComboBox.ForeColor = Color.Crimson;
                    break;
                case WalkthroughStates.SELECT_INTER_FIELDS_SPACE:
                    interSpaceValueLabel.ForeColor = Color.Crimson;
                    break;

                case WalkthroughStates.SELECT_OPTION_REGION:
                    walkthroughDescriptionLabel.ForeColor = Color.Crimson;
                    break;
                case WalkthroughStates.PROVIDE_TOTAL_OPTIONS:
                    totalObjectsLabel.ForeColor = Color.Crimson;
                    break;
                case WalkthroughStates.SELECT_INTER_OPTIONS_SPACE_TYPE:
                    interSpaceTypeComboBox.ForeColor = Color.Crimson;
                    break;
                case WalkthroughStates.SELECT_INTER_OPTIONS_SPACE:
                    interSpaceValueLabel.ForeColor = Color.Crimson;
                    break;
                default:
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

            walkthroughPanel.Visible = false;
            imageBoxPanel.Dock = DockStyle.Fill;
            Size = new Size(450, 395);
            MinimumSize = Size;

            OMRRegionData regionData = new OMRRegionData(fieldsRegion, interFieldsSpaceType, interFieldsSpace, interFieldsSpaces, optionsRegion, interOptionsSpaceType, interOptionsSpace, interOptionsSpaces);
        }

        private void SetFieldRegion()
        {
            if (imageBox.SelectionRegion.IsEmpty)
            {
                InvalidateState();
                return;
            }
            else
                ValidateState();

            fieldsRegion = imageBox.SelectionRegion;
        }
        private void SetTotalFields()
        {
            int value = (int)totalObjectsTextBox.IntegerValue;
            if(value <= 0)
            {
                InvalidateState();
                return;
            }
            else
                ValidateState();

            totalFields = value;
        }
        private void SetInterFieldsSpaceType()
        {
            if (interSpaceTypeComboBox.SelectedValue == null)
            {
                InvalidateState();
                return;
            }
            else
            {
                InterSpaceType value = (InterSpaceType)interSpaceTypeComboBox.SelectedValue;
                if (value < 0)
                {
                    InvalidateState();
                    return;
                }

                ValidateState();

                interFieldsSpaceType = value;
            }
        }
        private void SetInterFieldsSpace()
        {
            double value = interSpaceValueTextBox.DoubleValue;
            if(value <= 0)
            {
                InvalidateState();
                return;
            }
            else
                ValidateState();

            interFieldsSpace = value;
        }

        private void SetOptionRegion()
        {
            if (imageBox.SelectionRegion.IsEmpty)
            {
                InvalidateState();
                return;
            }
            else
                ValidateState();

            optionsRegion = imageBox.SelectionRegion;
        }
        private void SetTotalOptions()
        {
            int value = (int)totalObjectsTextBox.IntegerValue;
            if (value <= 0)
            {
                InvalidateState();
                return;
            }
            else
                ValidateState();

            totalOptions = value;
        }
        private void SetInterOptionsSpaceType()
        {
            if (interSpaceTypeComboBox.SelectedValue == null)
            {
                InvalidateState();
                return;
            }
            else
            {
                InterSpaceType value = (InterSpaceType)interSpaceTypeComboBox.SelectedValue;
                if (value < 0)
                {
                    InvalidateState();
                    return;
                }

                ValidateState();

                interOptionsSpaceType = value;
            }
        }
        private void SetInterOptionsSpace()
        {
            double value = interSpaceValueTextBox.DoubleValue;
            if (value <= 0)
            {
                InvalidateState();
                return;
            }
            else
                ValidateState();

            interOptionsSpace = value;
        }

        private void NextBtn_Click(object sender, EventArgs e)
        {
            CurrentNextAction();
        }
        private void SetBtn_Click(object sender, EventArgs e)
        {
            CurrentSetAction();
        }
        #endregion
        #endregion
    }
}