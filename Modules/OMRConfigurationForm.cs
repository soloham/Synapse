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
using Synapse.Core.Configurations;
using Synapse.Core.Managers;
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
        public enum ConfigurationState
        {
            [EnumDescription("Select Region Name")]
            SELECT_REGION_NAME,
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

        #region Events 

        public delegate void OnConfigurationFinshed(string regionName, Orientation regionOrientaion, OMRRegionData regionData);
        public event OnConfigurationFinshed OnConfigurationFinishedEvent;

        public event EventHandler OnFormInitializedEvent;

        #endregion

        #region Properties
        public string RegionName { get; set; }
        public OMRRegionData RegionData { get; set; }
        public Bitmap RegionImage { get; set; }
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
            Initialize(regionImage);
        }
        internal OMRConfigurationForm(OMRConfiguration omrConfig, Bitmap regionImage)
        {
            Initialize(omrConfig, regionImage);
        }
        #endregion

        #region Private Methods
        private void Initialize(OMRConfiguration omrConfig, Bitmap regionImage)
        {
            synchronizationContext = SynchronizationContext.Current;

            InitializeComponent();

            statePanels.Add(LabelStatePanel);
            statePanels.Add(IntegerValueStatePanel);
            statePanels.Add(ComboBoxStatePanel);
            statePanels.Add(DoubleValueStatePanel);

            LastStateAction = SetInterOptionsSpace;

            SetupForConfigured(omrConfig, regionImage);

            OnConfigurationFinishedEvent += OnConfigurationFinishedCallback;

            OnFormInitializedEvent?.Invoke(this, EventArgs.Empty);
        }
        private void Initialize(Bitmap regionImage)
        {
            synchronizationContext = SynchronizationContext.Current;

            InitializeComponent();

            statePanels.Add(LabelStatePanel);
            statePanels.Add(IntegerValueStatePanel);
            statePanels.Add(ComboBoxStatePanel);
            statePanels.Add(DoubleValueStatePanel);

            LastStateAction = SetInterOptionsSpace;

            SetupForConfiguration(regionImage);

            OnConfigurationFinishedEvent += OnConfigurationFinishedCallback;

            OnFormInitializedEvent?.Invoke(this, EventArgs.Empty);
        }

        private void OnConfigurationFinishedCallback(string name, Orientation orientation, OMRRegionData _regionData)
        {
            SetupForConfigured(_regionData, RegionImage);
        }

        private void SetupForConfiguration(Bitmap region = null)
        {
            if (RegionData == null)
            {
                //Size = new Size(450, 505);
                MinimumSize = new Size(450, 505);

                MainLayoutPanel.RowStyles[1].SizeType = SizeType.Absolute;
                MainLayoutPanel.RowStyles[1].Height = 115;

                for (int i = 0; i < configureStatesPanel.RowCount; i++)
                {
                    configureStatesPanel.RowStyles[i].SizeType = SizeType.Percent;
                    configureStatesPanel.RowStyles[i].Height = i == 0 ? 0 : i == 1 ? 0 : i == 2 ? 50 : i == 3 ? 50 : 0;
                }
            }
            else
            {
                //Size = new Size(450, 570);
                MinimumSize = new Size(450, 505);

                MainLayoutPanel.RowStyles[1].SizeType = SizeType.Absolute;
                MainLayoutPanel.RowStyles[1].Height = 185;

                for (int i = 0; i < configureStatesPanel.RowCount; i++)
                {
                    configureStatesPanel.RowStyles[i].SizeType = SizeType.Percent;
                    configureStatesPanel.RowStyles[i].Height = i == 0 ? 0 : i == 1 ? 30 : i == 2 ? 35 : i == 3 ? 35 : 0;
                }

                selectStateComboBox.Visible = true;

                ConfigWalkthroughState = 0;
            }
            
            if(!curConfigureStatePanel.Controls.Contains(statePanelsPanel))
                curConfigureStatePanel.Controls.Add(statePanelsPanel, 1, 0);
            statePanelsPanel.Dock = DockStyle.Fill;
            statePanelsPanel.Visible = true;

            configureStatesPanel.Visible = true;

            RegionImage = region == null? RegionImage : region;
            imageBox.Image = RegionImage;

            StartWalkthrough();
        }
        private void SetupForConfigured(OMRConfiguration omrConfig, Bitmap region = null)
        {
            //Size = new Size(450, 456);
            MinimumSize = new Size(450, 456);

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

            RegionName = omrConfig.Title;
            RegionData = omrConfig.RegionData;
            totalFields = RegionData.TotalFields;
            totalOptions = RegionData.TotalOptions;
            orientation = omrConfig.Orientation;
            fieldsRegion = omrConfig.RegionData.FieldsRegion;
            optionsRegion = omrConfig.RegionData.OptionsRegion;
            interOptionsSpace = omrConfig.RegionData.InterOptionsSpace;
            interOptionsSpaceType = omrConfig.RegionData.InterOptionsSpaceType;
            interFieldsSpace = omrConfig.RegionData.InterFieldsSpace;
            interFieldsSpaceType = omrConfig.RegionData.InterFieldsSpaceType;
            interOptionsSpaces = omrConfig.RegionData.InterOptionsSpaces.ToList();
            interFieldsSpaces = omrConfig.RegionData.InterFieldsSpaces.ToList();

            RegionImage = region == null? RegionImage : region;
            imageBox.Image = region;

            drawFields = true;
            drawInterFieldsSpaces = true;
            drawOptions = true;
            drawInterOptionsSpaces = true;
            CalculateRegion();
        }
        private void SetupForConfigured(OMRRegionData regionData, Bitmap region = null)
        {
            //Size = new Size(450, 456);
            MinimumSize = new Size(450, 456);

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

            RegionData = regionData;

            RegionImage = region == null ? RegionImage : region;
            imageBox.Image = region;

            drawFields = true;
            drawOptions = true;
            CalculateRegion();
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

                Utilities.Functions.DrawBox(g, Color.CornflowerBlue, curDrawFieldRectF, imageBox.ZoomFactor, 84, 2, DashStyle.Dash);

                g.Restore(originalState);
            }
        }
        private void CalculateInterFieldsSpacesRects()
        {
            drawInterFieldsSpacesRects.Clear();

            RectangleF interFieldsSpaceRegionVertical = new RectangleF(new PointF(fieldsRegion.X + (fieldsRegion.Size.Width / 2f) - 2.7f, fieldsRegion.Y + fieldsRegion.Size.Height), new SizeF(20f, (float)interFieldsSpace));
            RectangleF interFieldsSpaceRegionHorizontal = new RectangleF(new PointF(fieldsRegion.X + fieldsRegion.Size.Width, fieldsRegion.Y + (fieldsRegion.Size.Height / 2f) - 2.7f), new SizeF((float)interFieldsSpace, 20f));

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

                Utilities.Functions.DrawBox(g, Color.CornflowerBlue, curDrawInterFieldSpaceRectF, imageBox.ZoomFactor, 200, 0);

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
                                            curOptionRectF = new RectangleF(new PointF(i == 0 ? optionsRegion.X + (float)interOptionsSpaces[0] : drawOptionsRects[i - 1].X + drawOptionsRects[i - 1].Width + (float)interOptionsSpaces[i], i0 == 0 ? (float)interFieldsSpaces[0] + optionsRegion.Y : lastFieldOptionRect.Y + (float)(fieldsRegion.Height + interFieldsSpaces[i0])), optionsRegion.Size);
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

                lastFieldOptionRect = drawOptionsRects.Count == 0? new RectangleF() : drawOptionsRects[drawOptionsRects.Count - 1];
            }
        }
        private void DrawOptions(Graphics g)
        {
            GraphicsState originalState;

            for (int i = 0; i < drawOptionsRects.Count; i++)
            {
                RectangleF curDrawOptionRectF = imageBox.GetOffsetRectangle(drawOptionsRects[i]);

                originalState = g.Save();

                Utilities.Functions.DrawBox(g, Color.Firebrick, curDrawOptionRectF, imageBox.ZoomFactor, 64, 1, DashStyle.Dash, DashCap.Round);

                g.Restore(originalState);
            }
        }
        private void CalculateInterOptionsSpacesRects()
        {
            drawInterOptionsSpacesRects.Clear();

            RectangleF interOptionsSpaceRegionVertical = new RectangleF(new PointF(optionsRegion.X + (optionsRegion.Size.Width / 2f) -1.35f, optionsRegion.Y + optionsRegion.Size.Height), new SizeF(10f, (float)interOptionsSpace));
            RectangleF interOptionsSpaceRegionHorizontal = new RectangleF(new PointF(optionsRegion.X + optionsRegion.Size.Width, optionsRegion.Y + (optionsRegion.Size.Height / 2f) - 1.35f), new SizeF((float)interOptionsSpace, 10f));

            float interSpaceVerticalWidth = 2.5f;
            float interSpaceHorizontalHeight = 2.5f;

            RectangleF lastFieldInterOptionRect = new RectangleF();
            for (int i0 = 0; i0 < totalFields; i0++)
            {
                RectangleF curInterOptionSpaceRectF = new RectangleF();

                for (int i = 0; i < totalOptions; i++)
                {
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
                                            curInterOptionSpaceRectF = new RectangleF(new PointF(i == 0 ? optionsRegion.X : drawOptionsRects[i - 1].Right, i0 == 0? interOptionsSpaceRegionHorizontal.Y + (float)interFieldsSpaces[0] : lastFieldInterOptionRect.Y + (float)(interFieldsSpaces[i0] + fieldsRegion.Height)), new SizeF((float)interOptionsSpaces[i], interSpaceHorizontalHeight));
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
                lastFieldInterOptionRect = curInterOptionSpaceRectF;
            }
        }
        private void DrawInterOptionsSpaces(Graphics g)
        {
            GraphicsState originalState;

            for (int i = 0; i < drawInterOptionsSpacesRects.Count; i++)
            {
                RectangleF curDrawInterOptionSpaceRectF = imageBox.GetOffsetRectangle(drawInterOptionsSpacesRects[i]);

                originalState = g.Save();

                Utilities.Functions.DrawBox(g, Color.Firebrick, curDrawInterOptionSpaceRectF, imageBox.ZoomFactor, 200, 0);

                g.Restore(originalState);
            }
        }

        private void CalculateFields()
        {
            CalculateFieldsRects();
            CalculateInterFieldsSpacesRects();

            imageBox.Invalidate();
        }
        private void CalculateOptions()
        {
            CalculateOptionsRects();
            CalculateInterOptionsSpacesRects();

            imageBox.Invalidate();
        }                 
        private void CalculateRegion()
        {
            CalculateFields();
            CalculateOptions();
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
                case ConfigurationState.SELECT_REGION_NAME:
                    stringValueStateTextBox.Text = "";
                    for (int i = 0; i < stringValueStateControlsLayoutPanel.ColumnCount; i++)
                    {
                        stringValueStateControlsLayoutPanel.ColumnStyles[i].SizeType = SizeType.Percent;
                        stringValueStateControlsLayoutPanel.ColumnStyles[i].Width = i == 0 ? 0 : i == 1 ? 100 : 0;
                    }

                    CurrentStatePanel = StringValueStatePanel;
                    break;
                case ConfigurationState.SELECT_REGION_ORIENTATION:
                    comboBoxStateComboBox.DataSource = EnumHelper.ToList(typeof(Orientation));
                    comboBoxStateComboBox.DisplayMember = "Value";
                    comboBoxStateComboBox.ValueMember = "Key";

                    CurrentStatePanel = ComboBoxStatePanel;
                    break;
                case ConfigurationState.SELECT_FIELD_REGION:
                    walkthroughDescriptionLabel.Text = "";

                    CurrentStatePanel = LabelStatePanel;
                    break;
                case ConfigurationState.PROVIDE_TOTAL_FIELDS:
                    integerStateValueTextBox.IntegerValue = 0;
                    for (int i = 0; i < integerStateControlsTablePanel.ColumnCount; i++)
                    {
                        integerStateControlsTablePanel.ColumnStyles[i].SizeType = SizeType.Percent;
                        integerStateControlsTablePanel.ColumnStyles[i].Width = i == 0? 0 : i == 1? 100 : 0;
                    }

                    CurrentStatePanel = IntegerValueStatePanel;
                    break;
                case ConfigurationState.SELECT_INTER_FIELDS_SPACE_TYPE:
                    comboBoxStateComboBox.DataSource = EnumHelper.ToList(typeof(InterSpaceType));
                    comboBoxStateComboBox.DisplayMember = "Value";
                    comboBoxStateComboBox.ValueMember = "Key";

                    CurrentStatePanel = ComboBoxStatePanel;
                    break;
                case ConfigurationState.SELECT_INTER_FIELDS_SPACE:
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

                case ConfigurationState.SELECT_OPTION_REGION:
                    CurrentStatePanel = LabelStatePanel;
                    break;
                case ConfigurationState.PROVIDE_TOTAL_OPTIONS:
                    integerStateValueTextBox.IntegerValue = 0;
                    for (int i = 0; i < integerStateControlsTablePanel.ColumnCount; i++)
                    {
                        integerStateControlsTablePanel.ColumnStyles[i].SizeType = SizeType.Percent;
                        integerStateControlsTablePanel.ColumnStyles[i].Width = i == 0 ? 0 : i == 1 ? 100 : 0;
                    }

                    CurrentStatePanel = IntegerValueStatePanel;
                    break;
                case ConfigurationState.SELECT_INTER_OPTIONS_SPACE_TYPE:
                    comboBoxStateComboBox.DataSource = EnumHelper.ToList(typeof(InterSpaceType));
                    comboBoxStateComboBox.DisplayMember = "Value";
                    comboBoxStateComboBox.ValueMember = "Key";

                    CurrentStatePanel = ComboBoxStatePanel;
                    break;
                case ConfigurationState.SELECT_INTER_OPTIONS_SPACE:
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
        private void SetupState(ConfigurationState walkthroughState)
        {
            InitializeStatePanel(walkthroughState);

            SelectionRegionChangedAction = null;
            SelectionRegionResizedAction = null;
            switch (walkthroughState)
            {
                case ConfigurationState.SELECT_REGION_NAME:
                    stringValueStateLabel.Text = "Region Name:";
                    stringValueStateTextBox.Text = string.IsNullOrEmpty(RegionName)? "OMR Region" : RegionName;

                    CurrentSetAction = new Action(SetRegionName);

                    nextBtn.Text = "NEXT";
                    nextBtn.BackColor = Color.LightSlateGray;
                    CurrentNextAction = new Action(NextState);
                    break;
                case ConfigurationState.SELECT_REGION_ORIENTATION:
                    comboBoxStateComboBox.Text = "Select Orientation";
                    comboBoxStateComboBox.SelectedIndex = RegionData == null ? -1 : (int)orientation;

                    CurrentSetAction = new Action(SetRegionOrientation);

                    nextBtn.Text = "NEXT";
                    nextBtn.BackColor = Color.LightSlateGray;
                    CurrentNextAction = new Action(NextState);
                    break;
                case ConfigurationState.SELECT_FIELD_REGION:
                    walkthroughDescriptionLabel.Text = "Please select the first field region.";

                    CurrentSetAction = new Action(SetFieldRegion);

                    nextBtn.Text = "NEXT";
                    nextBtn.BackColor = Color.LightSlateGray;
                    CurrentNextAction = new Action(NextState);
                    break;
                case ConfigurationState.PROVIDE_TOTAL_FIELDS:
                    integerStateLabel.Text = "Total Fields:";
                    integerStateValueTextBox.IntegerValue = RegionData == null ? 1 : totalFields;

                    CurrentSetAction = new Action(SetTotalFields);
                    break;
                case ConfigurationState.SELECT_INTER_FIELDS_SPACE_TYPE:
                    comboBoxStateComboBox.Text = "Inter Fields Space Type";
                    comboBoxStateComboBox.SelectedIndex = RegionData == null ? -1 : (int)RegionData.InterFieldsSpaceType;

                    CurrentSetAction = new Action(SetInterFieldsSpaceType);

                    nextBtn.Text = "NEXT";
                    nextBtn.BackColor = Color.LightSlateGray;
                    CurrentNextAction = new Action(NextState);
                    break;
                case ConfigurationState.SELECT_INTER_FIELDS_SPACE:
                    doubleStateLabel.Text = "Inter Fields Space:";
                    doubleStateValueTextBox.DoubleValue = RegionData == null ? 0.00 : RegionData.InterFieldsSpace;

                    DoubleStateComboAction = new Action(SetInterFieldsComboSpace);
                    CurrentSetAction = new Action(SetInterFieldsSpace);

                    SelectionRegionChangedAction = () =>
                    {
                        if (imageBox.SelectionRegion.IsEmpty)
                            return;

                        switch (orientation)
                        {
                            case Orientation.Horizontal:
                                SetInterFieldsSpace(imageBox.SelectionRegion.Height);
                                break;
                            case Orientation.Vertical:
                                SetInterFieldsSpace(imageBox.SelectionRegion.Width);
                                break;
                        }
                    };
                    SelectionRegionResizedAction = () =>
                    {
                        if (imageBox.SelectionRegion.IsEmpty)
                            return;

                        switch (orientation)
                        {
                            case Orientation.Horizontal:
                                SetInterFieldsSpace(imageBox.SelectionRegion.Height);
                                break;
                            case Orientation.Vertical:
                                SetInterFieldsSpace(imageBox.SelectionRegion.Width);
                                break;
                        }
                    };

                    nextBtn.Text = "NEXT";
                    nextBtn.BackColor = Color.LightSlateGray;
                    CurrentNextAction = new Action(NextState);
                    break;

                case ConfigurationState.SELECT_OPTION_REGION:
                    walkthroughDescriptionLabel.Text = "Please select the first option region.";

                    CurrentSetAction = new Action(SetOptionRegion);

                    nextBtn.Text = "NEXT";
                    nextBtn.BackColor = Color.LightSlateGray;
                    CurrentNextAction = new Action(NextState);
                    break;
                case ConfigurationState.PROVIDE_TOTAL_OPTIONS:
                    integerStateLabel.Text = "Total Options:";
                    integerStateValueTextBox.IntegerValue = RegionData == null ? 1 : totalOptions;

                    CurrentSetAction = new Action(SetTotalOptions);
                    break;
                case ConfigurationState.SELECT_INTER_OPTIONS_SPACE_TYPE:
                    comboBoxStateComboBox.Text = "Inter Options Space Type";
                    comboBoxStateComboBox.SelectedIndex = RegionData == null ? -1 : (int)RegionData.InterOptionsSpaceType;

                    CurrentSetAction = new Action(SetInterOptionsSpaceType);

                    nextBtn.Text = "NEXT";
                    nextBtn.BackColor = Color.LightSlateGray;
                    CurrentNextAction = new Action(NextState);
                    break;
                case ConfigurationState.SELECT_INTER_OPTIONS_SPACE:
                    doubleStateLabel.Text = "Inter Options Space:";
                    doubleStateValueTextBox.DoubleValue = RegionData == null ? 0.00 : RegionData.InterOptionsSpace;

                    DoubleStateComboAction = new Action(SetInterOptionsComboSpace);
                    CurrentSetAction = new Action(SetInterOptionsSpace);

                    SelectionRegionChangedAction = () =>
                    {
                        if (imageBox.SelectionRegion.IsEmpty)
                            return;

                        switch (orientation)
                        {
                            case Orientation.Horizontal:
                                SetInterOptionsSpace(imageBox.SelectionRegion.Width);
                                break;
                            case Orientation.Vertical:
                                SetInterOptionsSpace(imageBox.SelectionRegion.Height);
                                break;
                        }
                    };
                    SelectionRegionResizedAction = () =>
                    {
                        if (imageBox.SelectionRegion.IsEmpty)
                            return;

                        switch (orientation)
                        {
                            case Orientation.Horizontal:
                                SetInterOptionsSpace(imageBox.SelectionRegion.Width);
                                break;
                            case Orientation.Vertical:
                                SetInterOptionsSpace(imageBox.SelectionRegion.Height);
                                break;
                        }
                    };

                    nextBtn.Text = "FINISH";
                    nextBtn.BackColor = Color.MediumTurquoise;
                    CurrentNextAction = new Action(EndWalkthrough);
                    break;
            }
        }
        private void NextState()
        {
            if (!isCurrentSetActionCompleted && RegionData == null)
            {
                InvalidateState();

                return;
            }

            ValidateState();

            isCurrentSetActionCompleted = false;

            switch (ConfigWalkthroughState)
            {
                case ConfigurationState.SELECT_REGION_NAME:
                    break;
                case ConfigurationState.SELECT_REGION_ORIENTATION:
                    break;
                case ConfigurationState.SELECT_FIELD_REGION:
                    imageBox.SelectNone();
                    break;
                case ConfigurationState.PROVIDE_TOTAL_FIELDS:
                    break;
                case ConfigurationState.SELECT_INTER_FIELDS_SPACE_TYPE:
                    break;
                case ConfigurationState.SELECT_INTER_FIELDS_SPACE:
                    imageBox.SelectNone();

                    SelectionRegionChangedAction = null;
                    SelectionRegionResizedAction = null;
                    break;
                case ConfigurationState.SELECT_OPTION_REGION:
                    imageBox.SelectNone();
                    break;
                case ConfigurationState.PROVIDE_TOTAL_OPTIONS:
                    break;
                case ConfigurationState.SELECT_INTER_OPTIONS_SPACE_TYPE:
                    break;
                case ConfigurationState.SELECT_INTER_OPTIONS_SPACE:
                    imageBox.SelectNone();

                    SelectionRegionChangedAction = null;
                    SelectionRegionResizedAction = null;
                    break;
            }

            if (RegionData != null)
                selectStateComboBox.SelectedIndex = (int)ConfigWalkthroughState + 1;
            else
                ConfigWalkthroughState++;
        }
        private void ValidateState()
        {
            walkthroughIndexLabel.ForeColor = Color.FromArgb(255, 68, 68, 68);
            switch (ConfigWalkthroughState)
            {
                case ConfigurationState.SELECT_REGION_NAME:
                    stringValueStateTextBox.ForeColor = Color.FromArgb(255, 68, 68, 68);
                    stringValueStateLabel.ForeColor = Color.FromArgb(255, 68, 68, 68);
                    break;
                case ConfigurationState.SELECT_REGION_ORIENTATION:
                    comboBoxStateComboBox.ForeColor = Color.FromArgb(255, 68, 68, 68);
                    break;
                case ConfigurationState.SELECT_FIELD_REGION:
                    walkthroughDescriptionLabel.ForeColor = Color.FromArgb(255, 68, 68, 68);
                    break;
                case ConfigurationState.PROVIDE_TOTAL_FIELDS:
                    integerStateLabel.ForeColor = Color.FromArgb(255, 68, 68, 68);
                    break;
                case ConfigurationState.SELECT_INTER_FIELDS_SPACE_TYPE:
                    comboBoxStateComboBox.ForeColor = Color.FromArgb(255, 68, 68, 68);
                    break;
                case ConfigurationState.SELECT_INTER_FIELDS_SPACE:
                    doubleStateLabel.ForeColor = Color.FromArgb(255, 68, 68, 68);
                    doubleStateComboBox.ForeColor = Color.FromArgb(255, 68, 68, 68);
                    break;

                case ConfigurationState.SELECT_OPTION_REGION:
                    walkthroughDescriptionLabel.ForeColor = Color.FromArgb(255, 68, 68, 68);
                    break;
                case ConfigurationState.PROVIDE_TOTAL_OPTIONS:
                    integerStateLabel.ForeColor = Color.FromArgb(255, 68, 68, 68);
                    break;
                case ConfigurationState.SELECT_INTER_OPTIONS_SPACE_TYPE:
                    comboBoxStateComboBox.ForeColor = Color.FromArgb(255, 68, 68, 68);
                    break;
                case ConfigurationState.SELECT_INTER_OPTIONS_SPACE:
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
            switch (ConfigWalkthroughState)
            {
                case ConfigurationState.SELECT_REGION_NAME:
                    stringValueStateTextBox.ForeColor = Color.Crimson;
                    stringValueStateLabel.ForeColor = Color.Crimson;
                    break;
                case ConfigurationState.SELECT_REGION_ORIENTATION:
                    comboBoxStateComboBox.ForeColor = Color.Crimson;
                    break;
                case ConfigurationState.SELECT_FIELD_REGION:
                    walkthroughDescriptionLabel.ForeColor = Color.Crimson;
                    break;
                case ConfigurationState.PROVIDE_TOTAL_FIELDS:
                    integerStateLabel.ForeColor = Color.Crimson;
                    break;
                case ConfigurationState.SELECT_INTER_FIELDS_SPACE_TYPE:
                    comboBoxStateComboBox.ForeColor = Color.Crimson;
                    break;
                case ConfigurationState.SELECT_INTER_FIELDS_SPACE:
                    doubleStateLabel.ForeColor = Color.Crimson;
                    doubleStateComboBox.ForeColor = Color.Crimson;
                    break;

                case ConfigurationState.SELECT_OPTION_REGION:
                    walkthroughDescriptionLabel.ForeColor = Color.Crimson;
                    break;
                case ConfigurationState.PROVIDE_TOTAL_OPTIONS:
                    integerStateLabel.ForeColor = Color.Crimson;
                    break;
                case ConfigurationState.SELECT_INTER_OPTIONS_SPACE_TYPE:
                    comboBoxStateComboBox.ForeColor = Color.Crimson;
                    break;
                case ConfigurationState.SELECT_INTER_OPTIONS_SPACE:
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

            OMRRegionData regionData = new OMRRegionData(orientation, totalFields, fieldsRegion, interFieldsSpaceType, interFieldsSpace, interFieldsSpaces.ToArray(), totalOptions, optionsRegion, interOptionsSpaceType, interOptionsSpace, interOptionsSpaces.ToArray());
            OnConfigurationFinishedEvent?.Invoke(RegionName, orientation, regionData);
        }

        private bool ValidateName(string name)
        {
            bool isValid = true;

            if (name == "" || name[0] == ' ' || name[name.Length-1] == ' ')
                isValid = false;

            if (isValid)
                isValid = ConfigurationsManager.ValidateName(name);

            return isValid;
        }
        private void SetRegionName()
        {
            string name = stringValueStateTextBox.Text;
            if (!ValidateName(name))
            {
                InvalidateState();
                return;
            }
            else
            {
                ValidateState();

                RegionName = name;
            }
        }
        private void SetRegionOrientation()
        {
            if (comboBoxStateComboBox.SelectedValue == null)
            {
                InvalidateState();
                return;
            }
            else
            {
                Orientation value = (Orientation)comboBoxStateComboBox.SelectedValue;
                if (value < 0)
                {
                    InvalidateState();
                    return;
                }

                ValidateState();

                orientation = value;

                CalculateRegion();
            }
        }

        private void SetFieldRegion()
        {
            if (imageBox.SelectionRegion.IsEmpty && RegionData == null)
            {
                InvalidateState();
                return;
            }

            ValidateState();

            if (!imageBox.SelectionRegion.IsEmpty)
                fieldsRegion = imageBox.SelectionRegion;


            CalculateRegion();
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

            CalculateRegion();

            drawFields = true;
            drawInterFieldsSpaces = true;

            imageBox.Invalidate();
        }
        private void SetInterFieldsSpaceType()
        {
            if (comboBoxStateComboBox.SelectedValue == null)
            {
                InvalidateState();
                return;
            }
            else
            {
                InterSpaceType value = (InterSpaceType)comboBoxStateComboBox.SelectedValue;
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
                        if (RegionData != null) RegionData.InterFieldsSpaces = interFieldsSpaces.ToArray();
                        break;
                }

                if (RegionData != null) RegionData.InterFieldsSpaceType = interFieldsSpaceType;

                CalculateRegion();
            }
        }
        private void SetInterFieldsSpace()
        {
            SetInterFieldsSpace(-1);
        }
        private void SetInterFieldsSpace(float _value = -1)
        {
            double value = _value == -1 ? doubleStateValueTextBox.DoubleValue : _value;
            doubleStateValueTextBox.DoubleValue = value;
            int fieldIndex = doubleStateComboBox.SelectedIndex + 1;

            if (value < 0 || (fieldIndex <= 0 && interFieldsSpaceType == InterSpaceType.ARRAY))
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
                    if (RegionData != null) RegionData.InterFieldsSpace = interFieldsSpace;
                    break;
                case InterSpaceType.ARRAY:
                    interFieldsSpaces[fieldIndex - 1] = value;
                    if (RegionData != null) RegionData.InterFieldsSpaces[fieldIndex - 1] = value;
                    break;
                default:
                    break;
            }

            CalculateRegion();
        }

        private void SetInterFieldsComboSpace()
        {
            int curIndex = doubleStateComboBox.SelectedIndex;
            if (curIndex < 0 || RegionData == null || RegionData.InterFieldsSpaceType != InterSpaceType.ARRAY) return;

            double curInterFieldsSpace = RegionData.InterFieldsSpaces[curIndex];
            doubleStateValueTextBox.DoubleValue = curInterFieldsSpace;
        }

        private void SetOptionRegion()
        {
            if (imageBox.SelectionRegion.IsEmpty && RegionData == null)
            {
                InvalidateState();
                return;
            }

            ValidateState();

            if (!imageBox.SelectionRegion.IsEmpty)
                optionsRegion = imageBox.SelectionRegion;
        
            CalculateRegion();
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

            CalculateRegion();

            drawOptions = true;
            drawInterOptionsSpaces = true;

            imageBox.Invalidate();
        }
        private void SetInterOptionsSpaceType()
        {
            if (comboBoxStateComboBox.SelectedValue == null)
            {
                InvalidateState();
                return;
            }
            else
            {
                InterSpaceType value = (InterSpaceType)comboBoxStateComboBox.SelectedValue;
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
                        if (RegionData != null) RegionData.InterOptionsSpaces = interOptionsSpaces.ToArray();
                        break;
                }

                if (RegionData != null) RegionData.InterOptionsSpaceType = interOptionsSpaceType;
            }

            CalculateRegion();
        }
        private void SetInterOptionsSpace()
        {
            SetInterOptionsSpace(-1);
        }
        private void SetInterOptionsSpace(float _value = -1)
        {
            double value = _value == -1 ? doubleStateValueTextBox.DoubleValue : _value;
            doubleStateValueTextBox.DoubleValue = value;
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
                    if (RegionData != null) RegionData.InterOptionsSpace = value;
                    break;
                case InterSpaceType.ARRAY:
                    interOptionsSpaces[fieldIndex - 1] = value;
                    if (RegionData != null) RegionData.InterOptionsSpaces[fieldIndex - 1] = value;
                    break;
            }

            CalculateRegion();
        }

        private void SetInterOptionsComboSpace()
        {
            int curIndex = doubleStateComboBox.SelectedIndex;
            if (curIndex < 0 || RegionData == null || RegionData.InterOptionsSpaceType != InterSpaceType.ARRAY) return;

            double curInterOptionsSpace = RegionData.InterOptionsSpaces[curIndex];
            doubleStateValueTextBox.DoubleValue = curInterOptionsSpace;
        }

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
        #endregion

        #endregion
    }
} 