using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;
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
            [EnumDescription("Select Region Orientation")]
            SELECT_REGION_ORIENTATION,
            //[EnumDescription("Select Field Region Type")]
            //SELECT_FIELD_REGION_TYPE,
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
        private Action DoubleStateComboAction;
        private Action IntegerStateComboAction;

        private SynchronizationContext synchronizationContext;

        private Orientation orientation;

        #region Fields Variables
        private RectangleF fieldsRegion;
        private int totalFields;
        private InterSpaceType interFieldsSpaceType;
        private double interFieldsSpace;
        private List<double> interFieldsSpaces = new List<double>();

        private bool drawFields = false;
        private List<RectangleF> drawFieldsRects = new List<RectangleF>();

        private bool drawInterFieldsSpaces = false;
        private List<RectangleF> drawInterFieldsSpacesRects = new List<RectangleF>();
        #endregion

        #region Options Variables
        private RectangleF optionsRegion;
        private int totalOptions;
        private InterSpaceType interOptionsSpaceType;
        private double interOptionsSpace;
        private List<double> interOptionsSpaces = new List<double>();

        private bool drawInterOptionsSpaces = false;
        private bool drawOptions = false;
        private List<RectangleF> drawOptionsRects = new List<RectangleF>();
        private List<RectangleF> drawInterOptionsSpacesRects = new List<RectangleF>();
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

            statePanels.Add(orientationStatePanel);
            statePanels.Add(LabelStatePanel);
            statePanels.Add(IntegerValueStatePanel);
            statePanels.Add(ComboBoxStatePanel);
            statePanels.Add(DoubleValueStatePanel);

            LastStateAction = SetInterOptionsSpace;
        }

        private void SetupForInitialization(Bitmap region)
        {
            walkthroughStatusPanel.Controls.Add(statePanelsPanel, 1, 0);
            statePanelsPanel.Dock = DockStyle.Fill;
            statePanelsPanel.Visible = true;

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

        private void CalculateFieldsRects()
        {
            drawFieldsRects.Clear();

            for (int i = 0; i < totalFields; i++)
            {
                RectangleF curFieldRectF = new RectangleF();

                switch (orientation)
                {
                    case Orientation.Horizontal:
                        switch (interFieldsSpaceType)
                        {
                            case InterSpaceType.CONSTANT:
                                curFieldRectF = new RectangleF(new PointF(fieldsRegion.X, fieldsRegion.Y + (float)(i * (fieldsRegion.Height + interFieldsSpace))), fieldsRegion.Size);
                                break;
                            case InterSpaceType.ARRAY:
                                curFieldRectF = new RectangleF(new PointF(fieldsRegion.X, i == 0 ? fieldsRegion.Y + (float)interFieldsSpaces[0] : drawFieldsRects[i-1].Bottom + (float)interFieldsSpaces[i]), fieldsRegion.Size);
                                break;
                            default:
                                break;
                        }
                        break;
                    case Orientation.Vertical:
                        switch (interFieldsSpaceType)
                        {
                            case InterSpaceType.CONSTANT:
                                curFieldRectF = new RectangleF(new PointF(fieldsRegion.X + (float)(i * (fieldsRegion.Width + interFieldsSpace)), fieldsRegion.Y), fieldsRegion.Size);
                                break;
                            case InterSpaceType.ARRAY:
                                curFieldRectF = new RectangleF(new PointF(i == 0? fieldsRegion.X + (float)interFieldsSpaces[0] : drawFieldsRects[i - 1].Right + (float)interFieldsSpaces[i], fieldsRegion.Y), fieldsRegion.Size);
                                break;
                            default:
                                break;
                        }
                        break;
                    default:
                        break;
                }

                drawFieldsRects.Add(curFieldRectF);
            }
        }
        private void DrawFields(Graphics g)
        {
            GraphicsState originalState;

            for (int i = 0; i < totalFields; i++)
            {
                RectangleF curDrawFieldRectF = imageBox.GetOffsetRectangle(drawFieldsRects[i]);

                originalState = g.Save();

                this.DrawBox(g, Color.CornflowerBlue, curDrawFieldRectF, imageBox.ZoomFactor, 84, 2, DashStyle.Dash);

                g.Restore(originalState);
            }
        }
        private void CalculateInterFieldsSpacesRects()
        {
            drawInterFieldsSpacesRects.Clear();

            RectangleF interFieldsSpaceRegionVertical = new RectangleF(new PointF(fieldsRegion.X + (fieldsRegion.Size.Width / 2f), fieldsRegion.Y + fieldsRegion.Size.Height), new SizeF(20f, (float)interFieldsSpace));
            RectangleF interFieldsSpaceRegionHorizontal = new RectangleF(new PointF(fieldsRegion.X + fieldsRegion.Size.Width, fieldsRegion.Y + (fieldsRegion.Size.Height / 2f)), new SizeF((float)interFieldsSpace, 20f));

            float interSpaceVerticalWidth = 5f;
            float interSpaceHorizontalHeight = 5f;

            for (int i = 0; i < totalFields; i++)
            {
                RectangleF curInterFieldSpaceRectF = new RectangleF();

                switch (orientation)
                {
                    case Orientation.Horizontal:
                        switch (interFieldsSpaceType)
                        {
                            case InterSpaceType.CONSTANT:
                                if (i == totalFields - 1)
                                    break;

                                curInterFieldSpaceRectF = new RectangleF(new PointF(interFieldsSpaceRegionVertical.X, interFieldsSpaceRegionVertical.Y + (i * ((float)interFieldsSpace + fieldsRegion.Height))), new SizeF(interSpaceVerticalWidth, (float)interFieldsSpace));
                                break;
                            case InterSpaceType.ARRAY:
                                curInterFieldSpaceRectF = new RectangleF(new PointF(interFieldsSpaceRegionVertical.X, i == 0 ? fieldsRegion.Y : drawFieldsRects[i - 1].Bottom), new SizeF(interSpaceVerticalWidth, (float)interFieldsSpaces[i]));
                                break;
                            default:
                                break;
                        }
                        break;
                    case Orientation.Vertical:
                        switch (interFieldsSpaceType)
                        {
                            case InterSpaceType.CONSTANT:
                                if (i == totalFields - 1)
                                    break;

                                curInterFieldSpaceRectF = new RectangleF(new PointF(interFieldsSpaceRegionHorizontal.X + (i * ((float)interFieldsSpace + fieldsRegion.Width)), interFieldsSpaceRegionHorizontal.Y), new SizeF((float)interFieldsSpace, interSpaceHorizontalHeight));
                                break;
                            case InterSpaceType.ARRAY:
                                curInterFieldSpaceRectF = new RectangleF(new PointF(i == 0? fieldsRegion.X : drawFieldsRects[i - 1].Right, interFieldsSpaceRegionHorizontal.Y), new SizeF((float)interFieldsSpaces[i], interSpaceHorizontalHeight));
                                break;
                            default:
                                break;
                        }
                        break;
                }

                drawInterFieldsSpacesRects.Add(curInterFieldSpaceRectF);
            }
        }
        private void DrawInterFieldsSpaces(Graphics g)
        {
            GraphicsState originalState;

            for (int i = 0; i < drawInterFieldsSpacesRects.Count; i++)
            {
                RectangleF curDrawInterFieldSpaceRectF = imageBox.GetOffsetRectangle(drawInterFieldsSpacesRects[i]);

                originalState = g.Save();

                this.DrawBox(g, Color.CornflowerBlue, curDrawInterFieldSpaceRectF, imageBox.ZoomFactor, 200, 0);

                g.Restore(originalState);
            }
        }

        private void CalculateOptionsRects()
        {
            drawOptionsRects.Clear();

            RectangleF lastFieldOptionRect = new RectangleF();
            for (int i0 = 0; i0 < totalFields; i0++)
            {
                for (int i = 0; i < totalOptions; i++)
                {
                    RectangleF curOptionRectF = new RectangleF();

                    switch (orientation)
                    {
                        case Orientation.Horizontal:
                            switch (interOptionsSpaceType)
                            {
                                case InterSpaceType.CONSTANT:
                                    switch (interFieldsSpaceType)
                                    {
                                        case InterSpaceType.CONSTANT:
                                            curOptionRectF = new RectangleF(new PointF(optionsRegion.X + (float)(i * (optionsRegion.Width + interOptionsSpace)), optionsRegion.Y + (float)(i0 * (fieldsRegion.Height + interFieldsSpace))), optionsRegion.Size);
                                            break;
                                        case InterSpaceType.ARRAY:
                                            curOptionRectF = new RectangleF(new PointF(optionsRegion.X + (float)(i * (optionsRegion.Width + interOptionsSpace)), i0 == 0? (float)interFieldsSpaces[0] + optionsRegion.Y : (lastFieldOptionRect.Y + lastFieldOptionRect.Height) + (float)(fieldsRegion.Height + interFieldsSpaces[i0])), optionsRegion.Size);
                                            break;
                                        default:
                                            break;
                                    }
                                    break;
                                case InterSpaceType.ARRAY:
                                    switch (interFieldsSpaceType)
                                    {
                                        case InterSpaceType.CONSTANT:
                                            curOptionRectF = new RectangleF(new PointF(i == 0 ? optionsRegion.X + (float)interOptionsSpaces[0] : drawOptionsRects[i - 1].X + drawOptionsRects[i - 1].Width + (float)interOptionsSpaces[i], optionsRegion.Y + (float)(i0 * (fieldsRegion.Height + interFieldsSpace))), optionsRegion.Size);
                                            break;
                                        case InterSpaceType.ARRAY:
                                            curOptionRectF = new RectangleF(new PointF(i == 0 ? optionsRegion.X + (float)interOptionsSpaces[0] : drawOptionsRects[i - 1].X + drawOptionsRects[i - 1].Width + (float)interOptionsSpaces[i], i0 == 0 ? (float)interFieldsSpaces[0] + optionsRegion.Y : (lastFieldOptionRect.Y + lastFieldOptionRect.Height) + (float)(fieldsRegion.Height + interFieldsSpaces[i0])), optionsRegion.Size);
                                            break;
                                        default:
                                            break;
                                    }
                                    break;
                            }
                            break;
                        case Orientation.Vertical:
                            switch (interOptionsSpaceType)
                            {
                                case InterSpaceType.CONSTANT:
                                    switch (interFieldsSpaceType)
                                    {
                                        case InterSpaceType.CONSTANT:
                                            curOptionRectF = new RectangleF(new PointF(optionsRegion.X + (float)(i0 * (fieldsRegion.Width + interFieldsSpace)), optionsRegion.Y + (float)(i * (optionsRegion.Height + interOptionsSpace))), optionsRegion.Size);
                                            break;
                                        case InterSpaceType.ARRAY:
                                            curOptionRectF = new RectangleF(new PointF(i0 == 0? (float)interFieldsSpaces[0] + optionsRegion.X : lastFieldOptionRect.X + fieldsRegion.Width + (float)interFieldsSpaces[i0], optionsRegion.Y + (float)(i * (optionsRegion.Height + interOptionsSpace))), optionsRegion.Size);
                                            break;
                                    }
                                    break;
                                case InterSpaceType.ARRAY:
                                    switch (interFieldsSpaceType)
                                    {
                                        case InterSpaceType.CONSTANT:
                                            curOptionRectF = new RectangleF(new PointF(optionsRegion.X + (float)(i0 * (fieldsRegion.Width + interFieldsSpace)), i == 0? (float)interOptionsSpaces[0] + optionsRegion.Y : drawOptionsRects[i - 1].Bottom + (float)interOptionsSpaces[i]), optionsRegion.Size);
                                            break;
                                        case InterSpaceType.ARRAY:
                                            curOptionRectF = new RectangleF(new PointF(i0 == 0 ? (float)interFieldsSpaces[0] + optionsRegion.X : lastFieldOptionRect.X + fieldsRegion.Width + (float)interFieldsSpaces[i0], i == 0 ? (float)interOptionsSpaces[0] + optionsRegion.Y : drawOptionsRects[i - 1].Bottom + (float)interOptionsSpaces[i]), optionsRegion.Size);
                                            break;
                                    }
                                    break;
                                default:
                                    break;
                            }
                            break;
                    }

                    drawOptionsRects.Add(curOptionRectF);
                }

                lastFieldOptionRect = drawOptionsRects[drawOptionsRects.Count - 1];
            }
        }
        private void DrawOptions(Graphics g)
        {
            GraphicsState originalState;

            for (int i = 0; i < drawOptionsRects.Count; i++)
            {
                RectangleF curDrawOptionRectF = imageBox.GetOffsetRectangle(drawOptionsRects[i]);

                originalState = g.Save();

                this.DrawBox(g, Color.Firebrick, curDrawOptionRectF, imageBox.ZoomFactor, 64, 1, DashStyle.Dash, DashCap.Round);

                g.Restore(originalState);
            }
        }
        private void CalculateInterOptionsSpacesRects()
        {
            drawInterOptionsSpacesRects.Clear();

            RectangleF interOptionsSpaceRegionVertical = new RectangleF(new PointF(optionsRegion.X + (optionsRegion.Size.Width / 2f), optionsRegion.Y + optionsRegion.Size.Height), new SizeF(10f, (float)interOptionsSpace));
            RectangleF interOptionsSpaceRegionHorizontal = new RectangleF(new PointF(optionsRegion.X + optionsRegion.Size.Width, optionsRegion.Y + (optionsRegion.Size.Height / 2f)), new SizeF((float)interOptionsSpace, 10f));

            float interSpaceVerticalWidth = 2.5f;
            float interSpaceHorizontalHeight = 2.5f;

            for (int i0 = 0; i0 < totalFields; i0++)
            {
                for (int i = 0; i < totalOptions; i++)
                {
                    RectangleF curInterOptionSpaceRectF = new RectangleF();

                    switch (orientation)
                    {
                        case Orientation.Horizontal:
                            switch (interOptionsSpaceType)
                            {
                                case InterSpaceType.CONSTANT:
                                    if (i == totalOptions - 1)
                                        break;

                                    switch (interFieldsSpaceType)
                                    {
                                        case InterSpaceType.CONSTANT:
                                            curInterOptionSpaceRectF = new RectangleF(new PointF(interOptionsSpaceRegionHorizontal.X + (float)(i * (interOptionsSpace + optionsRegion.Width)), interOptionsSpaceRegionHorizontal.Y + (float)(i0 * (interFieldsSpace + fieldsRegion.Height))), new SizeF((float)interOptionsSpace, interSpaceHorizontalHeight));
                                            break;
                                        case InterSpaceType.ARRAY:
                                            curInterOptionSpaceRectF = new RectangleF(new PointF(interOptionsSpaceRegionHorizontal.X + (float)(i * (interOptionsSpace + optionsRegion.Width)), interOptionsSpaceRegionHorizontal.Y + (float)(i0 * (interFieldsSpaces[i0] + fieldsRegion.Height))), new SizeF((float)interOptionsSpace, interSpaceHorizontalHeight));
                                            break;
                                    }
                                    break;
                                case InterSpaceType.ARRAY:
                                    switch (interFieldsSpaceType)
                                    {
                                        case InterSpaceType.CONSTANT:
                                            curInterOptionSpaceRectF = new RectangleF(new PointF(i == 0? optionsRegion.X : drawOptionsRects[i - 1].Right, interOptionsSpaceRegionHorizontal.Y + (float)(i0 * (interFieldsSpace + fieldsRegion.Height))), new SizeF((float)interOptionsSpaces[i], interSpaceHorizontalHeight));
                                            break;
                                        case InterSpaceType.ARRAY:
                                            curInterOptionSpaceRectF = new RectangleF(new PointF(i == 0 ? optionsRegion.X : drawOptionsRects[i - 1].Right, interOptionsSpaceRegionHorizontal.Y + (float)(i0 * (interFieldsSpaces[i0] + fieldsRegion.Height))), new SizeF((float)interOptionsSpaces[i], interSpaceHorizontalHeight));
                                            break;
                                    }
                                    break;
                            }
                            break;
                        case Orientation.Vertical:
                            switch (interOptionsSpaceType)
                            {
                                case InterSpaceType.CONSTANT:
                                    if (i == totalOptions - 1)
                                        break;

                                    switch (interFieldsSpaceType)
                                    {
                                        case InterSpaceType.CONSTANT:
                                            curInterOptionSpaceRectF = new RectangleF(new PointF(interOptionsSpaceRegionVertical.X + (float)(i0 * (interFieldsSpace + fieldsRegion.Width)), interOptionsSpaceRegionVertical.Y + (float)(i * (interOptionsSpace + optionsRegion.Height))), new SizeF(interSpaceVerticalWidth, (float)interOptionsSpace));
                                            break;
                                        case InterSpaceType.ARRAY:
                                            curInterOptionSpaceRectF = new RectangleF(new PointF(interOptionsSpaceRegionVertical.X + (float)(i0 * (interFieldsSpaces[i0] + fieldsRegion.Width)), interOptionsSpaceRegionVertical.Y + (float)(i * (interOptionsSpace + optionsRegion.Height))), new SizeF(interSpaceVerticalWidth, (float)interOptionsSpace));
                                            break;
                                    }
                                    break;
                                case InterSpaceType.ARRAY:
                                    switch (interFieldsSpaceType)
                                    {
                                        case InterSpaceType.CONSTANT:
                                            curInterOptionSpaceRectF = new RectangleF(new PointF(interOptionsSpaceRegionVertical.X + (float)(i0 * (interFieldsSpace + fieldsRegion.Width)), i == 0? optionsRegion.Y : drawOptionsRects[i - 1].Bottom), new SizeF(interSpaceVerticalWidth, (float)interOptionsSpaces[i]));
                                            break;
                                        case InterSpaceType.ARRAY:
                                            curInterOptionSpaceRectF = new RectangleF(new PointF(interOptionsSpaceRegionVertical.X + (float)(i0 * (interFieldsSpaces[i0] + fieldsRegion.Width)), i == 0 ? optionsRegion.Y : drawOptionsRects[i - 1].Bottom), new SizeF(interSpaceVerticalWidth, (float)interOptionsSpaces[i]));
                                            break;
                                    }
                                    break;
                            }
                            break;
                    }

                    drawInterOptionsSpacesRects.Add(curInterOptionSpaceRectF);
                }
            }
        }
        private void DrawInterOptionsSpaces(Graphics g)
        {
            GraphicsState originalState;

            for (int i = 0; i < drawInterOptionsSpacesRects.Count; i++)
            {
                RectangleF curDrawInterOptionSpaceRectF = imageBox.GetOffsetRectangle(drawInterOptionsSpacesRects[i]);

                originalState = g.Save();

                this.DrawBox(g, Color.Firebrick, curDrawInterOptionSpaceRectF, imageBox.ZoomFactor, 200, 0);

                g.Restore(originalState);
            }
        }


        #region  ImageBoxPanel Setup
        private void DrawBox(Graphics graphics, Color color, RectangleF rectangle, double scale, int alpha = 64, float borderWidth = 2, DashStyle dashStyle = DashStyle.Dot, DashCap dashCap = DashCap.Flat)
        {
            borderWidth *= (float)scale;

            using (SolidBrush brush = new SolidBrush(Color.FromArgb(alpha, color)))
            {
                graphics.FillRectangle(brush, rectangle);
            }

            if (borderWidth == 0)
                return;

            using (Pen pen = new Pen(color, borderWidth)
            {
                DashStyle = dashStyle,
                DashCap = dashCap
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

            if (drawFields)
            {
                DrawFields(e.Graphics);

                if (drawInterFieldsSpaces)
                    DrawInterFieldsSpaces(e.Graphics);
            }
            if (drawOptions)
            {
                DrawOptions(e.Graphics);

                if (drawInterOptionsSpaces)
                    DrawInterOptionsSpaces(e.Graphics);
            }
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
            WalkthroughState = 0;
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
                case WalkthroughStates.SELECT_REGION_ORIENTATION:
                    orientationComboBox.DataSource = EnumHelper.ToList(typeof(Orientation));
                    orientationComboBox.DisplayMember = "Value";
                    orientationComboBox.ValueMember = "Key";

                    CurrentStatePanel = orientationStatePanel;
                    break;
                //case WalkthroughStates.SELECT_FIELD_REGION_TYPE:
                //    walkthroughDescriptionLabel.Text = "";

                //    CurrentStatePanel = LabelStatePanel;
                //    break;
                case WalkthroughStates.SELECT_FIELD_REGION:
                    walkthroughDescriptionLabel.Text = "";

                    CurrentStatePanel = LabelStatePanel;
                    break;
                case WalkthroughStates.PROVIDE_TOTAL_FIELDS:
                    integerStateValueTextBox.IntegerValue = 0;
                    for (int i = 0; i < integerStateControlsTablePanel.ColumnCount; i++)
                    {
                        integerStateControlsTablePanel.ColumnStyles[i].SizeType = SizeType.Percent;
                        integerStateControlsTablePanel.ColumnStyles[i].Width = i == 0? 0 : i == 1? 100 : 0;
                    }

                    CurrentStatePanel = IntegerValueStatePanel;
                    break;
                case WalkthroughStates.SELECT_INTER_FIELDS_SPACE_TYPE:
                    interSpaceTypeComboBox.DataSource = EnumHelper.ToList(typeof(InterSpaceType));
                    interSpaceTypeComboBox.DisplayMember = "Value";
                    interSpaceTypeComboBox.ValueMember = "Key";

                    CurrentStatePanel = ComboBoxStatePanel;
                    break;
                case WalkthroughStates.SELECT_INTER_FIELDS_SPACE:
                    doubleStateValueTextBox.DoubleValue = 0.00;
                    switch (interFieldsSpaceType)
                    {
                        case InterSpaceType.CONSTANT:
                            for (int i = 0; i < doubleStageControlsPanel.ColumnCount; i++)
                            {
                                doubleStageControlsPanel.ColumnStyles[i].SizeType = SizeType.Percent;
                                doubleStageControlsPanel.ColumnStyles[i].Width = i == 0 ? 0 : i == 1 ? 100 : 0;
                            }
                            break;
                        case InterSpaceType.ARRAY:
                            doubleStateComboBox.Items.Clear();
                            for (int i = 0; i < totalFields; i++)
                            {
                                doubleStateComboBox.Items.Add(i+1);
                            }
                            doubleStateComboBox.SelectedIndex = 0;
                            for (int i = 0; i < doubleStageControlsPanel.ColumnCount; i++)
                            {
                                doubleStageControlsPanel.ColumnStyles[i].SizeType = SizeType.Percent;
                                doubleStageControlsPanel.ColumnStyles[i].Width = i == 0 ? 50 : i == 1 ? 50 : 0;
                            }
                            break;
                    }
                    
                    CurrentStatePanel = DoubleValueStatePanel;
                    break;

                case WalkthroughStates.SELECT_OPTION_REGION:
                    CurrentStatePanel = LabelStatePanel;
                    break;
                case WalkthroughStates.PROVIDE_TOTAL_OPTIONS:
                    integerStateValueTextBox.IntegerValue = 0;
                    for (int i = 0; i < integerStateControlsTablePanel.ColumnCount; i++)
                    {
                        integerStateControlsTablePanel.ColumnStyles[i].SizeType = SizeType.Percent;
                        integerStateControlsTablePanel.ColumnStyles[i].Width = i == 0 ? 0 : i == 1 ? 100 : 0;
                    }

                    CurrentStatePanel = IntegerValueStatePanel;
                    break;
                case WalkthroughStates.SELECT_INTER_OPTIONS_SPACE_TYPE:
                    interSpaceTypeComboBox.DataSource = EnumHelper.ToList(typeof(InterSpaceType));
                    interSpaceTypeComboBox.DisplayMember = "Value";
                    interSpaceTypeComboBox.ValueMember = "Key";

                    CurrentStatePanel = ComboBoxStatePanel;
                    break;
                case WalkthroughStates.SELECT_INTER_OPTIONS_SPACE:
                    doubleStateValueTextBox.DoubleValue = 0.00;
                    switch (interOptionsSpaceType)
                    {
                        case InterSpaceType.CONSTANT:
                            for (int i = 0; i < doubleStageControlsPanel.ColumnCount; i++)
                            {
                                doubleStageControlsPanel.ColumnStyles[i].SizeType = SizeType.Percent;
                                doubleStageControlsPanel.ColumnStyles[i].Width = i == 0 ? 0 : i == 1 ? 100 : 0;
                            }
                            break;
                        case InterSpaceType.ARRAY:
                            doubleStateComboBox.Items.Clear();
                            for (int i = 0; i < totalOptions; i++)
                            {
                                doubleStateComboBox.Items.Add(i+1);
                            }
                            doubleStateComboBox.SelectedIndex = 0;
                            for (int i = 0; i < doubleStageControlsPanel.ColumnCount; i++)
                            {
                                doubleStageControlsPanel.ColumnStyles[i].SizeType = SizeType.Percent;
                                doubleStageControlsPanel.ColumnStyles[i].Width = i == 0 ? 50 : i == 1 ? 50 : 0;
                            }
                            break;
                    }

                    CurrentStatePanel = DoubleValueStatePanel;
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
                case WalkthroughStates.SELECT_REGION_ORIENTATION:
                    orientationComboBox.Text = "Select Orientation";

                    CurrentSetAction = new Action(SetRegionOrientation);
                    break;
                case WalkthroughStates.SELECT_FIELD_REGION:
                    walkthroughDescriptionLabel.Text = "Please select the first field region.";

                    CurrentSetAction = new Action(SetFieldRegion);
                    break;
                case WalkthroughStates.PROVIDE_TOTAL_FIELDS:
                    integerStateLabel.Text = "Total Fields:";

                    CurrentSetAction = new Action(SetTotalFields);
                    break;
                case WalkthroughStates.SELECT_INTER_FIELDS_SPACE_TYPE:
                    interSpaceTypeComboBox.Text = "Inter Fields Space Type";

                    CurrentSetAction = new Action(SetInterFieldsSpaceType);
                    break;
                case WalkthroughStates.SELECT_INTER_FIELDS_SPACE:
                    doubleStateLabel.Text = "Inter Fields Space:";

                    
                    CurrentSetAction = new Action(SetInterFieldsSpace);
                    break;

                case WalkthroughStates.SELECT_OPTION_REGION:
                    walkthroughDescriptionLabel.Text = "Please select the first option region.";

                    CurrentSetAction = new Action(SetOptionRegion);
                    break;
                case WalkthroughStates.PROVIDE_TOTAL_OPTIONS:
                    integerStateLabel.Text = "Total Options:";

                    integerStateValueTextBox.Clear();
                    CurrentSetAction = new Action(SetTotalOptions);
                    break;
                case WalkthroughStates.SELECT_INTER_OPTIONS_SPACE_TYPE:
                    interSpaceTypeComboBox.Text = "Inter Options Space Type";

                    CurrentSetAction = new Action(SetInterOptionsSpaceType);
                    break;
                case WalkthroughStates.SELECT_INTER_OPTIONS_SPACE:
                    doubleStateLabel.Text = "Inter Options Space:";



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
                case WalkthroughStates.SELECT_REGION_ORIENTATION:
                    orientationComboBox.ForeColor = Color.FromArgb(255, 68, 68, 68);
                    break;
                case WalkthroughStates.SELECT_FIELD_REGION:
                    walkthroughDescriptionLabel.ForeColor = Color.FromArgb(255, 68, 68, 68);
                    break;
                case WalkthroughStates.PROVIDE_TOTAL_FIELDS:
                    integerStateLabel.ForeColor = Color.FromArgb(255, 68, 68, 68);
                    break;
                case WalkthroughStates.SELECT_INTER_FIELDS_SPACE_TYPE:
                    interSpaceTypeComboBox.ForeColor = Color.FromArgb(255, 68, 68, 68);
                    break;
                case WalkthroughStates.SELECT_INTER_FIELDS_SPACE:
                    doubleStateLabel.ForeColor = Color.FromArgb(255, 68, 68, 68);
                    doubleStateComboBox.ForeColor = Color.FromArgb(255, 68, 68, 68);
                    break;

                case WalkthroughStates.SELECT_OPTION_REGION:
                    walkthroughDescriptionLabel.ForeColor = Color.FromArgb(255, 68, 68, 68);
                    break;
                case WalkthroughStates.PROVIDE_TOTAL_OPTIONS:
                    integerStateLabel.ForeColor = Color.FromArgb(255, 68, 68, 68);
                    break;
                case WalkthroughStates.SELECT_INTER_OPTIONS_SPACE_TYPE:
                    interSpaceTypeComboBox.ForeColor = Color.FromArgb(255, 68, 68, 68);
                    break;
                case WalkthroughStates.SELECT_INTER_OPTIONS_SPACE:
                    doubleStateLabel.ForeColor = Color.FromArgb(255, 68, 68, 68);
                    doubleStateComboBox.ForeColor = Color.FromArgb(255, 68, 68, 68);
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
                case WalkthroughStates.SELECT_REGION_ORIENTATION:
                    orientationComboBox.ForeColor = Color.Crimson;
                    break;
                case WalkthroughStates.SELECT_FIELD_REGION:
                    walkthroughDescriptionLabel.ForeColor = Color.Crimson;
                    break;
                case WalkthroughStates.PROVIDE_TOTAL_FIELDS:
                    integerStateLabel.ForeColor = Color.Crimson;
                    break;
                case WalkthroughStates.SELECT_INTER_FIELDS_SPACE_TYPE:
                    interSpaceTypeComboBox.ForeColor = Color.Crimson;
                    break;
                case WalkthroughStates.SELECT_INTER_FIELDS_SPACE:
                    doubleStateLabel.ForeColor = Color.Crimson;
                    doubleStateComboBox.ForeColor = Color.Crimson;
                    break;

                case WalkthroughStates.SELECT_OPTION_REGION:
                    walkthroughDescriptionLabel.ForeColor = Color.Crimson;
                    break;
                case WalkthroughStates.PROVIDE_TOTAL_OPTIONS:
                    integerStateLabel.ForeColor = Color.Crimson;
                    break;
                case WalkthroughStates.SELECT_INTER_OPTIONS_SPACE_TYPE:
                    interSpaceTypeComboBox.ForeColor = Color.Crimson;
                    break;
                case WalkthroughStates.SELECT_INTER_OPTIONS_SPACE:
                    doubleStateLabel.ForeColor = Color.Crimson;
                    doubleStateComboBox.ForeColor = Color.Crimson;
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

            OMRRegionData regionData = new OMRRegionData(fieldsRegion, interFieldsSpaceType, interFieldsSpace, interFieldsSpaces.ToArray(), optionsRegion, interOptionsSpaceType, interOptionsSpace, interOptionsSpaces.ToArray());
        }

        private void SetRegionOrientation()
        {
            if (orientationComboBox.SelectedValue == null)
            {
                InvalidateState();
                return;
            }
            else
            {
                Orientation value = (Orientation)orientationComboBox.SelectedValue;
                if (value < 0)
                {
                    InvalidateState();
                    return;
                }

                ValidateState();

                orientation = value;
            }
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
            int value = (int)integerStateValueTextBox.IntegerValue;
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

                switch (interFieldsSpaceType)
                {
                    case InterSpaceType.CONSTANT:
                        break;
                    case InterSpaceType.ARRAY:
                        int toAddCount = totalFields - interFieldsSpaces.Count;
                        for (int i = 0; i < toAddCount; i++)
                            interFieldsSpaces.Add(0.1);
                        break;
                }
            }
        }
        private void SetInterFieldsSpace()
        {
            double value = doubleStateValueTextBox.DoubleValue;
            int fieldIndex = doubleStateComboBox.SelectedIndex + 1;

            if(value < 0 || (fieldIndex <= 0 && interFieldsSpaceType == InterSpaceType.ARRAY))
            {
                InvalidateState();
                return;
            }
            else
                ValidateState();

            switch (interFieldsSpaceType)
            {
                case InterSpaceType.CONSTANT:
                    interFieldsSpace = value;
                    break;
                case InterSpaceType.ARRAY:
                    interFieldsSpaces[fieldIndex-1] = value;
                    break;
                default:
                    break;
            }

            CalculateFieldsRects();
            drawFields = true;

            CalculateInterFieldsSpacesRects();
            drawInterFieldsSpaces = true;

            imageBox.Invalidate();
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
            int value = (int)integerStateValueTextBox.IntegerValue;
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

                switch (interOptionsSpaceType)
                {
                    case InterSpaceType.CONSTANT:
                        break;
                    case InterSpaceType.ARRAY:
                        int toAddCount = totalOptions - interOptionsSpaces.Count;
                        for (int i = 0; i < toAddCount; i++)
                            interOptionsSpaces.Add(0.1);
                        break;
                }
            }
        }
        private void SetInterOptionsSpace()
        {
            double value = doubleStateValueTextBox.DoubleValue;
            int fieldIndex = doubleStateComboBox.SelectedIndex + 1;

            if (value < 0 || (fieldIndex <= 0 && interOptionsSpaceType == InterSpaceType.ARRAY))
            {
                InvalidateState();
                return;
            }
            else
                ValidateState();

            switch (interOptionsSpaceType)
            {
                case InterSpaceType.CONSTANT:
                    interOptionsSpace = value;
                    break;
                case InterSpaceType.ARRAY:
                    interOptionsSpaces[fieldIndex - 1] = value;
                    break;
                default:
                    break;
            }

            CalculateOptionsRects();
            drawOptions = true;

            CalculateInterOptionsSpacesRects();
            drawInterOptionsSpaces = true;

            imageBox.Invalidate();
        }

        private void NextBtn_Click(object sender, EventArgs e)
        {
            CurrentNextAction();
        }
        private void SetBtn_Click(object sender, EventArgs e)
        {
            CurrentSetAction();
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