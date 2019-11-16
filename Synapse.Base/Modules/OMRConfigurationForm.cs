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
using Synapse.Utilities.Objects;
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

            [EnumDescription("Add Instances")]
            SELECT_INSTANCES_COUNT,
            [EnumDescription("Select Instances Orientation")]
            SELECT_INSTANCES_ORIENTATION,
            [EnumDescription("Select Inter Instances Space Type")]
            SELECT_INTER_INSTANCES_SPACE_TYPE,
            [EnumDescription("Select Inter Instances Space")]
            SELECT_INTER_INSTANCES_SPACE,
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

        private ConfigurationBase.ConfigArea configArea;
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

        #region Instances Variables
        private int totalInstances = 1;
        private InterSpaceType interInstancesSpaceType;
        private double interInstancesSpace;
        private List<double> interInstancesSpaces = new List<double>() { 0 };
        private List<Orientation> instancesOrientations = new List<Orientation>() { Orientation.Horizontal };

        private bool drawInterInstancesSpaces = false;
        private bool drawInstances = false;
        private List<RectangleF> drawInstancesRects = new List<RectangleF>();
        private List<RectangleF> drawInterInstancesSpacesRects = new List<RectangleF>();
        #endregion

        #endregion

        #region Public Methods
        public OMRConfigurationForm(ConfigurationBase.ConfigArea region)
        {
            configArea = region;
            Initialize(configArea.ConfigBitmap);
        }
        public OMRConfigurationForm(OMRConfiguration omrConfig, Bitmap regionImage)
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
            statePanels.Add(ComboValueStatePanel);

            LastStateAction = SetInterInstancesSpace;

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
            statePanels.Add(ComboValueStatePanel);

            LastStateAction = SetInterInstancesSpace;

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
            configArea = omrConfig.GetConfigArea;
            totalFields = RegionData.TotalFields;
            totalOptions = RegionData.TotalOptions;
            totalInstances = RegionData.TotalInstances;
            orientation = omrConfig.Orientation;
            fieldsRegion = omrConfig.RegionData.FieldsRegion;
            optionsRegion = omrConfig.RegionData.OptionsRegion;

            instancesOrientations = omrConfig.RegionData.InstancesOrientations.ToList();

            interOptionsSpaceType = omrConfig.RegionData.InterOptionsSpaceType;
            interOptionsSpace = omrConfig.RegionData.InterOptionsSpace;
            interOptionsSpaces = omrConfig.RegionData.InterOptionsSpaces.ToList();

            interFieldsSpaceType = omrConfig.RegionData.InterFieldsSpaceType;
            interFieldsSpace = omrConfig.RegionData.InterFieldsSpace;
            interFieldsSpaces = omrConfig.RegionData.InterFieldsSpaces.ToList();

            interInstancesSpaceType = omrConfig.RegionData.InterInstancesSpaceType;
            interInstancesSpace = omrConfig.RegionData.InterInstancesSpace;
            interInstancesSpaces = omrConfig.RegionData.InterInstancesSpaces.ToList();

            RegionImage = region == null? RegionImage : region;
            imageBox.Image = region;

            drawInstances = false;
            drawInterInstancesSpaces = false;
            drawFields = true;
            drawInterFieldsSpaces = true;
            drawOptions = true;
            drawInterOptionsSpaces = true;
            CalculateRegions();
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

            drawInstances = false;
            drawInterInstancesSpaces = false;
            drawFields = true;
            drawOptions = true;
            CalculateRegions();
        }

        private void CalculateFieldsRects()
        {
            drawFieldsRects.Clear();

            float initialRectX = fieldsRegion.X;
            float initialRectY = fieldsRegion.Y;
            for (int i0 = 0; i0 < totalInstances; i0++)
            {
                switch (instancesOrientations[i0])
                {
                    case Orientation.Horizontal:
                        initialRectX = (drawInstancesRects[i0].Left - configArea.ConfigRect.Left) + fieldsRegion.Left;
                        break;
                    case Orientation.Vertical:
                        initialRectY = (drawInstancesRects[i0].Top - configArea.ConfigRect.Top) + fieldsRegion.Top;
                        break;
                }
            
                for (int i = 0; i < totalFields; i++)
                {
                    RectangleF curFieldRectF = new RectangleF();

                    switch (orientation)
                    {
                        case Orientation.Horizontal:
                            switch (interFieldsSpaceType)
                            {
                                case InterSpaceType.CONSTANT:
                                    curFieldRectF = new RectangleF(new PointF(initialRectX, initialRectY + (float)(i * (fieldsRegion.Height + interFieldsSpace))), fieldsRegion.Size);
                                    break;
                                case InterSpaceType.ARRAY:
                                    curFieldRectF = new RectangleF(new PointF(initialRectX, i == 0 ? initialRectY + (float)interFieldsSpaces[0] : drawFieldsRects[i - 1].Bottom + (float)interFieldsSpaces[i]), fieldsRegion.Size);
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case Orientation.Vertical:
                            switch (interFieldsSpaceType)
                            {
                                case InterSpaceType.CONSTANT:
                                    curFieldRectF = new RectangleF(new PointF(initialRectX + (float)(i * (fieldsRegion.Width + interFieldsSpace)), initialRectY), fieldsRegion.Size);
                                    break;
                                case InterSpaceType.ARRAY:
                                    curFieldRectF = new RectangleF(new PointF(i == 0 ? initialRectX + (float)interFieldsSpaces[0] : drawFieldsRects[i - 1].Right + (float)interFieldsSpaces[i], initialRectY), fieldsRegion.Size);
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
        }
        private void DrawFields(Graphics g)
        {
            GraphicsState originalState;

            int totalToDrawFields = drawInstances == true ? drawFieldsRects.Count : totalFields;
            for (int i = 0; i < totalToDrawFields; i++)
            {
                RectangleF fieldRectArea = drawFieldsRects[i];
                RectangleF curDrawFieldRectF = RectangleF.Empty;
                if (drawInstances)
                    curDrawFieldRectF = imageBox.GetOffsetRectangle(new RectangleF(configArea.ConfigRect.Left + fieldRectArea.Left, configArea.ConfigRect.Top + fieldRectArea.Top, fieldRectArea.Width, fieldRectArea.Height));
                else
                    curDrawFieldRectF = imageBox.GetOffsetRectangle(fieldRectArea);

                SizeF rationSize = new SizeF(RegionImage.Size.Width / configArea.ConfigRect.Size.Width, RegionImage.Size.Height / configArea.ConfigRect.Size.Height);
                curDrawFieldRectF.Width *= rationSize.Width;
                curDrawFieldRectF.Height *= rationSize.Height;

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

            for (int i0 = 0; i0 < totalInstances; i0++)
            {
                switch (instancesOrientations[i0])
                {
                    case Orientation.Horizontal:
                        interFieldsSpaceRegionVertical.X = (drawInstancesRects[i0].Left - configArea.ConfigRect.Left) + fieldsRegion.X + (fieldsRegion.Size.Width / 2f) - 2.7f;
                        interFieldsSpaceRegionHorizontal.X = (drawInstancesRects[i0].Left - configArea.ConfigRect.Left) + fieldsRegion.X + fieldsRegion.Size.Width;
                        break;
                    case Orientation.Vertical:
                        interFieldsSpaceRegionVertical.Y = (drawInstancesRects[i0].Top - configArea.ConfigRect.Top) + fieldsRegion.Y + fieldsRegion.Size.Height;
                        interFieldsSpaceRegionHorizontal.Y = (drawInstancesRects[i0].Top - configArea.ConfigRect.Top) + fieldsRegion.Y + (fieldsRegion.Size.Height / 2f) - 2.7f;
                        break;
                }

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
        }
        private void DrawInterFieldsSpaces(Graphics g)
        {
            GraphicsState originalState;

            int totalToDrawInterFieldsSpacesRects = drawInstances == true ? drawInterFieldsSpacesRects.Count : interFieldsSpaceType == InterSpaceType.CONSTANT? totalFields-1 : totalFields;
            for (int i = 0; i < totalToDrawInterFieldsSpacesRects; i++)
            {
                RectangleF curDrawInterFieldSpaceRectF = RectangleF.Empty;
                if (drawInstances)
                    curDrawInterFieldSpaceRectF = imageBox.GetOffsetRectangle(new RectangleF(configArea.ConfigRect.Left + drawInterFieldsSpacesRects[i].Left, configArea.ConfigRect.Top + drawInterFieldsSpacesRects[i].Top, drawInterFieldsSpacesRects[i].Width, drawInterFieldsSpacesRects[i].Height));
                else
                    curDrawInterFieldSpaceRectF = imageBox.GetOffsetRectangle(drawInterFieldsSpacesRects[i]);

                originalState = g.Save();

                Utilities.Functions.DrawBox(g, Color.CornflowerBlue, curDrawInterFieldSpaceRectF, imageBox.ZoomFactor, 200, 0);

                g.Restore(originalState);
            }
        }

        private void CalculateOptionsRects()
        {
            drawOptionsRects.Clear();

            RectangleF lastFieldOptionRect = new RectangleF();

            float initialRectX = optionsRegion.X;
            float initialRectY = optionsRegion.Y;
            for (int i0 = 0; i0 < totalInstances; i0++)
            {
                switch (instancesOrientations[i0])
                {
                    case Orientation.Horizontal:
                        initialRectX = (drawInstancesRects[i0].Left - configArea.ConfigRect.Left) + optionsRegion.Left;
                        break;
                    case Orientation.Vertical:
                        initialRectY = (drawInstancesRects[i0].Top - configArea.ConfigRect.Top) + optionsRegion.Top;
                        break;
                }

                for (int i1 = 0; i1 < totalFields; i1++)
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
                                                curOptionRectF = new RectangleF(new PointF(initialRectX + (float)(i * (optionsRegion.Width + interOptionsSpace)), initialRectY + (float)(i1 * (fieldsRegion.Height + interFieldsSpace))), optionsRegion.Size);
                                                break;
                                            case InterSpaceType.ARRAY:
                                                curOptionRectF = new RectangleF(new PointF(initialRectX + (float)(i * (optionsRegion.Width + interOptionsSpace)), i1 == 0 ? (float)interFieldsSpaces[0] + initialRectY : (lastFieldOptionRect.Y + lastFieldOptionRect.Height) + (float)(fieldsRegion.Height + interFieldsSpaces[i1])), optionsRegion.Size);
                                                break;
                                            default:
                                                break;
                                        }
                                        break;
                                    case InterSpaceType.ARRAY:
                                        switch (interFieldsSpaceType)
                                        {
                                            case InterSpaceType.CONSTANT:
                                                curOptionRectF = new RectangleF(new PointF(i == 0 ? initialRectX + (float)interOptionsSpaces[0] : drawOptionsRects[i - 1].X + drawOptionsRects[i - 1].Width + (float)interOptionsSpaces[i], initialRectY + (float)(i1 * (fieldsRegion.Height + interFieldsSpace))), optionsRegion.Size);
                                                break;
                                            case InterSpaceType.ARRAY:
                                                curOptionRectF = new RectangleF(new PointF(i == 0 ? initialRectX + (float)interOptionsSpaces[0] : drawOptionsRects[i - 1].X + drawOptionsRects[i - 1].Width + (float)interOptionsSpaces[i], i1 == 0 ? (float)interFieldsSpaces[0] + initialRectY : lastFieldOptionRect.Y + (float)(fieldsRegion.Height + interFieldsSpaces[i1])), optionsRegion.Size);
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
                                                curOptionRectF = new RectangleF(new PointF(initialRectX + (float)(i1 * (fieldsRegion.Width + interFieldsSpace)), initialRectY + (float)(i * (optionsRegion.Height + interOptionsSpace))), optionsRegion.Size);
                                                break;
                                            case InterSpaceType.ARRAY:
                                                curOptionRectF = new RectangleF(new PointF(i1 == 0 ? (float)interFieldsSpaces[0] + initialRectX : lastFieldOptionRect.X + fieldsRegion.Width + (float)interFieldsSpaces[i1], initialRectY + (float)(i * (optionsRegion.Height + interOptionsSpace))), optionsRegion.Size);
                                                break;
                                        }
                                        break;
                                    case InterSpaceType.ARRAY:
                                        switch (interFieldsSpaceType)
                                        {
                                            case InterSpaceType.CONSTANT:
                                                curOptionRectF = new RectangleF(new PointF(initialRectX + (float)(i1 * (fieldsRegion.Width + interFieldsSpace)), i == 0 ? (float)interOptionsSpaces[0] + initialRectY : drawOptionsRects[i - 1].Bottom + (float)interOptionsSpaces[i]), optionsRegion.Size);
                                                break;
                                            case InterSpaceType.ARRAY:
                                                curOptionRectF = new RectangleF(new PointF(i1 == 0 ? (float)interFieldsSpaces[0] + initialRectX : lastFieldOptionRect.X + fieldsRegion.Width + (float)interFieldsSpaces[i1], i == 0 ? (float)interOptionsSpaces[0] + initialRectY : drawOptionsRects[i - 1].Bottom + (float)interOptionsSpaces[i]), optionsRegion.Size);
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

                    lastFieldOptionRect = drawOptionsRects.Count == 0 ? new RectangleF() : drawOptionsRects[drawOptionsRects.Count - 1];
                }
            }
        }
        private void DrawOptions(Graphics g)
        {
            GraphicsState originalState;

            int totalToDrawOptions = drawInstances == true ? drawOptionsRects.Count : totalOptions * totalFields;
            for (int i = 0; i < totalToDrawOptions; i++)
            {
                RectangleF curDrawOptionRectF = Rectangle.Empty;
                RectangleF fieldRectArea = drawOptionsRects[i];
                if (drawInstances)
                    curDrawOptionRectF = imageBox.GetOffsetRectangle(new RectangleF(configArea.ConfigRect.Left + fieldRectArea.Left, configArea.ConfigRect.Top + fieldRectArea.Top, fieldRectArea.Width, fieldRectArea.Height));
                else
                    curDrawOptionRectF = imageBox.GetOffsetRectangle(fieldRectArea);

                originalState = g.Save();

                SizeF rationSize = new SizeF(RegionImage.Size.Width / configArea.ConfigRect.Size.Width, RegionImage.Size.Height / configArea.ConfigRect.Size.Height);
                curDrawOptionRectF.Width *= rationSize.Width;
                curDrawOptionRectF.Height *= rationSize.Height;

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

            for (int i0 = 0; i0 < totalInstances; i0++)
            {
                switch (instancesOrientations[i0])
                {
                    case Orientation.Horizontal:
                        interOptionsSpaceRegionVertical.X = (drawInstancesRects[i0].Left - configArea.ConfigRect.Left) + optionsRegion.X + (optionsRegion.Size.Width / 2f) - 1.35f;
                        interOptionsSpaceRegionHorizontal.X = (drawInstancesRects[i0].Left - configArea.ConfigRect.Left) + optionsRegion.X + optionsRegion.Size.Width;
                        break;
                    case Orientation.Vertical:
                        interOptionsSpaceRegionVertical.Y = (drawInstancesRects[i0].Top - configArea.ConfigRect.Top) + optionsRegion.Y + optionsRegion.Size.Height;
                        interOptionsSpaceRegionHorizontal.Y = (drawInstancesRects[i0].Top - configArea.ConfigRect.Top) + optionsRegion.Y + (optionsRegion.Size.Height / 2f) - 1.35f;
                        break;
                }

                RectangleF lastFieldInterOptionRect = new RectangleF();
                for (int i1 = 0; i1 < totalFields; i1++)
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
                                                curInterOptionSpaceRectF = new RectangleF(new PointF(interOptionsSpaceRegionHorizontal.X + (float)(i * (interOptionsSpace + optionsRegion.Width)), interOptionsSpaceRegionHorizontal.Y + (float)(i1 * (interFieldsSpace + fieldsRegion.Height))), new SizeF((float)interOptionsSpace, interSpaceHorizontalHeight));
                                                break;
                                            case InterSpaceType.ARRAY:
                                                curInterOptionSpaceRectF = new RectangleF(new PointF(interOptionsSpaceRegionHorizontal.X + (float)(i * (interOptionsSpace + optionsRegion.Width)), interOptionsSpaceRegionHorizontal.Y + (float)(i1 * (interFieldsSpaces[i1] + fieldsRegion.Height))), new SizeF((float)interOptionsSpace, interSpaceHorizontalHeight));
                                                break;
                                        }
                                        break;
                                    case InterSpaceType.ARRAY:
                                        switch (interFieldsSpaceType)
                                        {
                                            case InterSpaceType.CONSTANT:
                                                curInterOptionSpaceRectF = new RectangleF(new PointF(i == 0 ? optionsRegion.X : drawOptionsRects[i - 1].Right, interOptionsSpaceRegionHorizontal.Y + (float)(i1 * (interFieldsSpace + fieldsRegion.Height))), new SizeF((float)interOptionsSpaces[i], interSpaceHorizontalHeight));
                                                break;
                                            case InterSpaceType.ARRAY:
                                                curInterOptionSpaceRectF = new RectangleF(new PointF(i == 0 ? optionsRegion.X : drawOptionsRects[i - 1].Right, i1 == 0 ? interOptionsSpaceRegionHorizontal.Y + (float)interFieldsSpaces[0] : lastFieldInterOptionRect.Y + (float)(interFieldsSpaces[i1] + fieldsRegion.Height)), new SizeF((float)interOptionsSpaces[i], interSpaceHorizontalHeight));
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
                                                curInterOptionSpaceRectF = new RectangleF(new PointF(interOptionsSpaceRegionVertical.X + (float)(i1 * (interFieldsSpace + fieldsRegion.Width)), interOptionsSpaceRegionVertical.Y + (float)(i * (interOptionsSpace + optionsRegion.Height))), new SizeF(interSpaceVerticalWidth, (float)interOptionsSpace));
                                                break;
                                            case InterSpaceType.ARRAY:
                                                curInterOptionSpaceRectF = new RectangleF(new PointF(interOptionsSpaceRegionVertical.X + (float)(i1 * (interFieldsSpaces[i1] + fieldsRegion.Width)), interOptionsSpaceRegionVertical.Y + (float)(i * (interOptionsSpace + optionsRegion.Height))), new SizeF(interSpaceVerticalWidth, (float)interOptionsSpace));
                                                break;
                                        }
                                        break;
                                    case InterSpaceType.ARRAY:
                                        switch (interFieldsSpaceType)
                                        {
                                            case InterSpaceType.CONSTANT:
                                                curInterOptionSpaceRectF = new RectangleF(new PointF(interOptionsSpaceRegionVertical.X + (float)(i1 * (interFieldsSpace + fieldsRegion.Width)), i == 0 ? optionsRegion.Y : drawOptionsRects[i - 1].Bottom), new SizeF(interSpaceVerticalWidth, (float)interOptionsSpaces[i]));
                                                break;
                                            case InterSpaceType.ARRAY:
                                                curInterOptionSpaceRectF = new RectangleF(new PointF(interOptionsSpaceRegionVertical.X + (float)(i1 * (interFieldsSpaces[i1] + fieldsRegion.Width)), i == 0 ? optionsRegion.Y : drawOptionsRects[i - 1].Bottom), new SizeF(interSpaceVerticalWidth, (float)interOptionsSpaces[i]));
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
        }
        private void DrawInterOptionsSpaces(Graphics g)
        {
            GraphicsState originalState;

            int totalToDrawInterOptionsSpacesRects = drawInstances == true ? drawInterOptionsSpacesRects.Count : interOptionsSpaceType == InterSpaceType.CONSTANT ? totalOptions - 1 : totalOptions;
            for (int i = 0; i < totalToDrawInterOptionsSpacesRects; i++)
            {
                RectangleF curDrawInterOptionSpaceRectF = RectangleF.Empty;
                if (drawInstances)
                    curDrawInterOptionSpaceRectF = imageBox.GetOffsetRectangle(new RectangleF(configArea.ConfigRect.Left + drawInterOptionsSpacesRects[i].Left, configArea.ConfigRect.Top + drawInterOptionsSpacesRects[i].Top, drawInterOptionsSpacesRects[i].Width, drawInterOptionsSpacesRects[i].Height));
                else
                    curDrawInterOptionSpaceRectF = imageBox.GetOffsetRectangle(drawInterOptionsSpacesRects[i]);

                originalState = g.Save();

                Utilities.Functions.DrawBox(g, Color.Firebrick, curDrawInterOptionSpaceRectF, imageBox.ZoomFactor, 200, 0);

                g.Restore(originalState);
            }
        }

        private void CalculateInstancesRects()
        {
            drawInstancesRects.Clear();

            RectangleF curInstanceRect = RectangleF.Empty;
            switch (interInstancesSpaceType)
            {
                case InterSpaceType.CONSTANT:
                    for (int i = 0; i < totalInstances; i++)
                    {
                        curInstanceRect = i == 0? configArea.ConfigRect : drawInstancesRects.Last();
                        if (i != 0)
                        {
                            switch (instancesOrientations[i])
                            {
                                case Orientation.Horizontal:
                                    curInstanceRect.Offset(curInstanceRect.Width + (float)interInstancesSpace, 0F);
                                    break;
                                case Orientation.Vertical:
                                    curInstanceRect.Offset(0F, curInstanceRect.Height + (float)interInstancesSpace);
                                    break;
                            }
                        }
                        drawInstancesRects.Add(curInstanceRect);
                    }
                    break;
                case InterSpaceType.ARRAY:
                    for (int i = 0; i < totalInstances; i++)
                    {
                        curInstanceRect = i == 0 ? configArea.ConfigRect : drawInstancesRects.Last();
                        if (i == 0)
                        {
                            switch (instancesOrientations[i])
                            {
                                case Orientation.Horizontal:
                                    curInstanceRect.Offset(curInstanceRect.Width + (float)interInstancesSpaces[i], 0F);
                                    break;
                                case Orientation.Vertical:
                                    curInstanceRect.Offset(0F, curInstanceRect.Height + (float)interInstancesSpaces[i]);
                                    break;
                            }
                        }
                       drawInstancesRects.Add(curInstanceRect);
                    }
                    break;
            }
        }
        private void DrawInstancesRects(Graphics g)
        {
            GraphicsState originalState;

            for (int i = 0; i < drawInstancesRects.Count; i++)
            {
                RectangleF curDrawInstanceRectF = imageBox.GetOffsetRectangle(drawInstancesRects[i]);

                originalState = g.Save();

                Utilities.Functions.DrawBox(g, Color.Firebrick, curDrawInstanceRectF, imageBox.ZoomFactor, 64, 1, DashStyle.Dash, DashCap.Round);

                g.Restore(originalState);
            }
        }
        private void CalculateInterInstancesSpacesRects()
        {
            drawInterInstancesSpacesRects.Clear();

            RectangleF curInterInstanceSpaceRect = RectangleF.Empty;
            RectangleF lastInstanceRect = RectangleF.Empty;
            switch (interInstancesSpaceType)
            {
                case InterSpaceType.CONSTANT:
                    for (int i = 1; i < totalInstances; i++)
                    {
                        lastInstanceRect = drawInstancesRects[i-1];
                        switch (instancesOrientations[i])
                        {
                            case Orientation.Horizontal:
                                curInterInstanceSpaceRect = new RectangleF(new PointF(lastInstanceRect.Right, lastInstanceRect.Y + ((lastInstanceRect.Height / 2f)-10)), new SizeF((float)interInstancesSpace, 20));
                                break;
                            case Orientation.Vertical:
                                curInterInstanceSpaceRect = new RectangleF(new PointF(lastInstanceRect.X + ((lastInstanceRect.Width / 2f)-10), lastInstanceRect.Bottom), new SizeF(20, (float)interInstancesSpace));
                                break;
                        }
                        drawInterInstancesSpacesRects.Add(curInterInstanceSpaceRect);
                    }
                    break;
                case InterSpaceType.ARRAY:
                    for (int i = 0; i < totalInstances; i++)
                    {
                        lastInstanceRect = drawInstancesRects[i];
                        if(i == 0)
                        {
                            switch (instancesOrientations[i])
                            {
                                case Orientation.Horizontal:
                                    curInterInstanceSpaceRect = new RectangleF(new PointF(lastInstanceRect.Left, lastInstanceRect.Y + ((lastInstanceRect.Height / 2f)-10)), new SizeF((float)interInstancesSpaces[i], 20));
                                    break;
                                case Orientation.Vertical:
                                    curInterInstanceSpaceRect = new RectangleF(new PointF(lastInstanceRect.X + ((lastInstanceRect.Width / 2f)-10), lastInstanceRect.Bottom), new SizeF(20, (float)interInstancesSpaces[i]));
                                    break;
                            }
                            continue;
                        }
                        switch (instancesOrientations[i])
                        {
                            case Orientation.Horizontal:
                                curInterInstanceSpaceRect = new RectangleF(new PointF(lastInstanceRect.Right, lastInstanceRect.Y + ((lastInstanceRect.Height / 2f)-10)), new SizeF((float)interInstancesSpaces[i], 20));
                                break;
                            case Orientation.Vertical:
                                curInterInstanceSpaceRect = new RectangleF(new PointF(lastInstanceRect.X + ((lastInstanceRect.Width / 2f)-10), lastInstanceRect.Top), new SizeF(20, (float)interInstancesSpaces[i]));
                                break;
                        }
                        drawInterInstancesSpacesRects.Add(curInterInstanceSpaceRect);
                    }
                    break;
            }
        }
        private void DrawInterInstancesSpaces(Graphics g)
        {
            GraphicsState originalState;

            for (int i = 0; i < drawInterInstancesSpacesRects.Count; i++)
            {
                RectangleF curDrawInterInstanceSpaceRectF = imageBox.GetOffsetRectangle(drawInterInstancesSpacesRects[i]);

                originalState = g.Save();

                Utilities.Functions.DrawBox(g, Color.Firebrick, curDrawInterInstanceSpaceRectF, imageBox.ZoomFactor, 200, 0);

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
        private void CalculateRegions()
        {
            CalculateInstancesRegion();
            CalculateFields();
            CalculateOptions();
        }
        private void CalculateInstancesRegion()
        {
            CalculateInstancesRects();
            CalculateInterInstancesSpacesRects();

            imageBox.Invalidate();
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

            if (drawInstances)
            {
                DrawInstancesRects(e.Graphics);

                if (drawInterInstancesSpaces)
                    DrawInterInstancesSpaces(e.Graphics);
            }
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
                case ConfigurationState.SELECT_INSTANCES_COUNT:
                    integerStateValueTextBox.IntegerValue = 0;
                    for (int i = 0; i < integerStateControlsTablePanel.ColumnCount; i++)
                    {
                        integerStateControlsTablePanel.ColumnStyles[i].SizeType = SizeType.Percent;
                        integerStateControlsTablePanel.ColumnStyles[i].Width = i == 0 ? 0 : i == 1 ? 100 : 0;
                    }

                    CurrentStatePanel = IntegerValueStatePanel;
                    break;
                case ConfigurationState.SELECT_INSTANCES_ORIENTATION:
                    comboValueStateValueComboBox.DataSource = EnumHelper.ToList(typeof(Orientation));
                    comboValueStateValueComboBox.DisplayMember = "Value";
                    comboValueStateValueComboBox.ValueMember = "Key";

                    comboValueStateComboBox.Items.Clear();
                    for (int i = 0; i < totalInstances; i++)
                    {
                        comboValueStateComboBox.Items.Add(i + 1);
                    }
                    comboValueStateComboBox.SelectedIndex = 0;
                    for (int i = 0; i < comboValueStateTablePanel.ColumnCount; i++)
                    {
                        comboValueStateTablePanel.ColumnStyles[i].SizeType = SizeType.Percent;
                        comboValueStateTablePanel.ColumnStyles[i].Width = i == 0 ? 50 : i == 1 ? 50 : 0;
                    }

                    CurrentStatePanel = ComboValueStatePanel;
                    break;
                case ConfigurationState.SELECT_INTER_INSTANCES_SPACE_TYPE:
                    comboBoxStateComboBox.DataSource = EnumHelper.ToList(typeof(InterSpaceType));
                    comboBoxStateComboBox.DisplayMember = "Value";
                    comboBoxStateComboBox.ValueMember = "Key";

                    CurrentStatePanel = ComboBoxStatePanel;
                    break;
                case ConfigurationState.SELECT_INTER_INSTANCES_SPACE:
                    doubleStateValueTextBox.DoubleValue = 0.00;
                    switch (interInstancesSpaceType)
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
                            for (int i = 0; i < totalInstances; i++)
                            {
                                doubleStateComboBox.Items.Add(i + 1);
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

                    nextBtn.Text = "NEXT";
                    nextBtn.BackColor = Color.LightSlateGray;
                    CurrentNextAction = new Action(NextState);
                    break;
                case ConfigurationState.SELECT_INSTANCES_COUNT:
                    imageBox.Image = SynapseMain.GetSynapseMain.GetCurrentImage();
                    drawInstances = true;
                    drawInterInstancesSpaces = true;
                    imageBox.Invalidate();

                    totalInstances = totalInstances == 0 ? 1 : totalInstances; 
                    integerStateLabel.Text = "Total Instances:";
                    integerStateValueTextBox.IntegerValue = totalInstances;
                    CalculateRegions();

                    CurrentSetAction = new Action(SetTotalInstances);
                    break;
                case ConfigurationState.SELECT_INSTANCES_ORIENTATION:
                    comboValueStateLabel.Text = "Select Orientations";

                    comboValueStateValueComboBox.Text = "Select Instances Orientations";
                    comboValueStateValueComboBox.SelectedIndex = RegionData == null ? -1 : (int)instancesOrientations[comboValueStateComboBox.SelectedIndex];

                    CurrentSetAction = new Action(SetInstancesOrientation);

                    nextBtn.Text = "NEXT";
                    nextBtn.BackColor = Color.LightSlateGray;
                    CurrentNextAction = new Action(NextState);
                    break;
                case ConfigurationState.SELECT_INTER_INSTANCES_SPACE_TYPE:
                    comboBoxStateComboBox.Text = "Inter Instances Space Type";
                    comboBoxStateComboBox.SelectedIndex = RegionData == null ? -1 : (int)RegionData.InterInstancesSpaceType;

                    CurrentSetAction = new Action(SetInterInstancesSpaceType);

                    nextBtn.Text = "NEXT";
                    nextBtn.BackColor = Color.LightSlateGray;
                    CurrentNextAction = new Action(NextState);
                    break;
                case ConfigurationState.SELECT_INTER_INSTANCES_SPACE:
                    doubleStateLabel.Text = "Inter Instances Space:";
                    doubleStateValueTextBox.DoubleValue = RegionData == null ? 0.00 : RegionData.InterInstancesSpace;

                    DoubleStateComboAction = new Action(SetInterInstancesComboSpace);
                    CurrentSetAction = new Action(SetInterInstancesSpace);

                    SelectionRegionChangedAction = () =>
                    {
                        if (imageBox.SelectionRegion.IsEmpty)
                            return;

                        switch (instancesOrientations[0])
                        {
                            case Orientation.Horizontal:
                                SetInterInstancesSpace(imageBox.SelectionRegion.Width);
                                break;
                            case Orientation.Vertical:
                                SetInterInstancesSpace(imageBox.SelectionRegion.Height);
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
                                SetInterInstancesSpace(imageBox.SelectionRegion.Width);
                                break;
                            case Orientation.Vertical:
                                SetInterInstancesSpace(imageBox.SelectionRegion.Height);
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
                case ConfigurationState.SELECT_INSTANCES_COUNT:
                    if (totalInstances == 1)
                        EndWalkthrough();
                    break;
                case ConfigurationState.SELECT_INSTANCES_ORIENTATION:
                    break;
                case ConfigurationState.SELECT_INTER_INSTANCES_SPACE_TYPE:
                    break;
                case ConfigurationState.SELECT_INTER_INSTANCES_SPACE:
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
                case ConfigurationState.SELECT_INSTANCES_COUNT:
                    integerStateLabel.ForeColor = Color.FromArgb(255, 68, 68, 68);
                    break;
                case ConfigurationState.SELECT_INSTANCES_ORIENTATION:
                    comboValueStateValueComboBox.ForeColor = Color.FromArgb(255, 68, 68, 68);
                    break;
                case ConfigurationState.SELECT_INTER_INSTANCES_SPACE_TYPE:
                    comboBoxStateComboBox.ForeColor = Color.FromArgb(255, 68, 68, 68);
                    break;
                case ConfigurationState.SELECT_INTER_INSTANCES_SPACE:
                    doubleStateLabel.ForeColor = Color.FromArgb(255, 68, 68, 68);
                    doubleStateComboBox.ForeColor = Color.FromArgb(255, 68, 68, 68);
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
                case ConfigurationState.SELECT_INSTANCES_COUNT:
                    integerStateLabel.ForeColor = Color.Crimson;
                    break;
                case ConfigurationState.SELECT_INSTANCES_ORIENTATION:
                    comboValueStateValueComboBox.ForeColor = Color.Crimson;
                    break;
                case ConfigurationState.SELECT_INTER_INSTANCES_SPACE_TYPE:
                    comboBoxStateComboBox.ForeColor = Color.Crimson;
                    break;
                case ConfigurationState.SELECT_INTER_INSTANCES_SPACE:
                    doubleStateLabel.ForeColor = Color.Crimson;
                    doubleStateComboBox.ForeColor = Color.Crimson;
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

            OMRRegionData regionData = new OMRRegionData(orientation, totalFields, fieldsRegion, interFieldsSpaceType, interFieldsSpace, interFieldsSpaces.ToArray(), totalOptions, optionsRegion, interOptionsSpaceType, interOptionsSpace, interOptionsSpaces.ToArray(), totalInstances, configArea.ConfigRect, instancesOrientations.ToArray(), interInstancesSpaceType, interInstancesSpace, interInstancesSpaces.ToArray());
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

                CalculateRegions();
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


            CalculateRegions();
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

            CalculateRegions();

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

                CalculateRegions();
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

            CalculateRegions();
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
        
            CalculateRegions();
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

            CalculateRegions();

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

            CalculateRegions();
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

            CalculateRegions();
        }
        private void SetInterOptionsComboSpace()
        {
            int curIndex = doubleStateComboBox.SelectedIndex;
            if (curIndex < 0 || RegionData == null || RegionData.InterOptionsSpaceType != InterSpaceType.ARRAY) return;

            double curInterOptionsSpace = RegionData.InterOptionsSpaces[curIndex];
            doubleStateValueTextBox.DoubleValue = curInterOptionsSpace;
        }

        private void SetTotalInstances()
        {
            int value = (int)integerStateValueTextBox.IntegerValue;
            if (value <= 0)
            {
                //InvalidateState();
                return;
            }
            else
                ValidateState();

            totalInstances = value;
            int diff = totalInstances - instancesOrientations.Count;
            if (diff > 0)
                instancesOrientations.AddRange(Enumerable.Repeat(Orientation.Horizontal, diff).ToList());
            else
                instancesOrientations.RemoveRange(instancesOrientations.Count - Math.Abs(diff), Math.Abs(diff));

            CalculateRegions();

            drawInstances = true;
            drawInterInstancesSpaces = true;

            imageBox.Invalidate();
        }
        private void SetInstancesOrientation()
        {
            if (comboValueStateValueComboBox.SelectedValue == null)
            {
                InvalidateState();
                return;
            }
            else
            {
                Orientation value = (Orientation)comboValueStateValueComboBox.SelectedValue;
                if (value < 0 || comboValueStateComboBox.SelectedIndex < 0)
                {
                    InvalidateState();
                    return;
                }

                ValidateState();

                instancesOrientations[comboValueStateComboBox.SelectedIndex] = value;
                if (RegionData != null) RegionData.InstancesOrientations = instancesOrientations.ToArray();

                CalculateRegions();
            }
        }
        private void SetInterInstancesSpaceType()
        {
            if (comboValueStateValueComboBox.SelectedValue == null)
            {
                InvalidateState();
                return;
            }
            else
            {
                InterSpaceType value = (InterSpaceType)comboValueStateValueComboBox.SelectedValue;
                if (value < 0)
                {
                    InvalidateState();
                    return;
                }

                ValidateState();

                interInstancesSpaceType = value;

                switch (interInstancesSpaceType)
                {
                    case InterSpaceType.CONSTANT:
                        break;
                    case InterSpaceType.ARRAY:
                        int toAddCount = totalInstances - interInstancesSpaces.Count;
                        for (int i = 0; i < toAddCount; i++)
                            interInstancesSpaces.Add(0.1);
                        if (RegionData != null) RegionData.InterInstancesSpaces = interInstancesSpaces.ToArray();
                        break;
                }

                if (RegionData != null) RegionData.InterInstancesSpaceType = interInstancesSpaceType;
            }

            CalculateRegions();
        }
        private void SetInterInstancesSpace()
        {
            SetInterInstancesSpace(-1);
        }
        private void SetInterInstancesSpace(float _value = -1)
        {
            double value = _value == -1 ? doubleStateValueTextBox.DoubleValue : _value;
            doubleStateValueTextBox.DoubleValue = value;
            int fieldIndex = doubleStateComboBox.SelectedIndex + 1;

            if (value < 0 || (fieldIndex <= 0 && interInstancesSpaceType == InterSpaceType.ARRAY))
            {
                InvalidateState();
                return;
            }
            else
                ValidateState();

            switch (interInstancesSpaceType)
            {
                case InterSpaceType.CONSTANT:
                    interInstancesSpace = value;
                    if (RegionData != null) RegionData.InterInstancesSpace = value;
                    break;
                case InterSpaceType.ARRAY:
                    interInstancesSpaces[fieldIndex - 1] = value;
                    if (RegionData != null) RegionData.InterInstancesSpaces[fieldIndex - 1] = value;
                    break;
            }

            CalculateRegions();
        }
        private void SetInterInstancesComboSpace()
        {
            int curIndex = doubleStateComboBox.SelectedIndex;
            if (curIndex < 0 || RegionData == null || RegionData.InterInstancesSpaceType != InterSpaceType.ARRAY) return;

            double curInterInstancesSpace = RegionData.InterInstancesSpaces[curIndex];
            doubleStateValueTextBox.DoubleValue = curInterInstancesSpace;

            CalculateRegions();
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