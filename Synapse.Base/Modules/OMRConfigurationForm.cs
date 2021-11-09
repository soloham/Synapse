using static Synapse.Core.Configurations.OMRRegionData;

namespace Synapse.Modules
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Globalization;
    using System.Linq;
    using System.Threading;
    using System.Windows.Forms;

    using Synapse.Core.Configurations;
    using Synapse.Core.Managers;
    using Synapse.Utilities;
    using Synapse.Utilities.Attributes;
    using Synapse.Utilities.Enums;

    using Syncfusion.WinForms.Controls;

    using WinFormAnimation;

    public partial class OMRConfigurationForm : SfForm
    {
        #region Enums

        public enum ConfigurationState
        {
            [EnumDescription("Select Region Name")]
            SELECT_REGION_NAME,

            [EnumDescription("Select Designer Mode")]
            SELECT_DESIGNER_MODE,

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

            [EnumDescription("Add Instances")] SELECT_INSTANCES_COUNT,

            [EnumDescription("Select Instances Orientation")]
            SELECT_INSTANCES_ORIENTATION,

            [EnumDescription("Select Inter Instances Space Type")]
            SELECT_INTER_INSTANCES_SPACE_TYPE,

            [EnumDescription("Select Inter Instances Space")]
            SELECT_INTER_INSTANCES_SPACE
        }

        #endregion

        #region Events

        public delegate void OnConfigurationFinshed(string regionName, Orientation regionOrientaion,
            OMRRegionData regionData);

        public event OnConfigurationFinshed OnConfigurationFinishedEvent;

        public event EventHandler OnFormInitializedEvent;

        #endregion

        #region Properties

        public string RegionName { get; set; }
        public OMRRegionData RegionData { get; set; }
        public Bitmap RegionImage { get; set; }

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

        private Orientation orientation;
        private DesignerModeType designerMode;

        private ConfigurationBase.ConfigArea configArea;

        #region Fields Variables

        private RectangleF fieldsRegion;
        private int totalFields;
        private InterSpaceType interFieldsSpaceType;
        private double interFieldsSpace;
        private List<double> interFieldsSpaces = new List<double>();

        private bool drawFields;
        private readonly List<RectangleF> drawFieldsRects = new List<RectangleF>();

        private bool drawInterFieldsSpaces;
        private readonly List<RectangleF> drawInterFieldsSpacesRects = new List<RectangleF>();

        #endregion

        #region Options Variables

        private RectangleF optionsRegion;
        private Dictionary<int, List<RectangleF>> freeOptionsRegions;
        private int totalOptions;
        private InterSpaceType interOptionsSpaceType;
        private double interOptionsSpace;
        private List<double> interOptionsSpaces = new List<double>();

        private bool drawInterOptionsSpaces;
        private bool drawOptions;
        private readonly List<RectangleF> drawOptionsRects = new List<RectangleF>();
        private readonly List<RectangleF> drawInterOptionsSpacesRects = new List<RectangleF>();

        #endregion

        #region Instances Variables

        private int totalInstances = 1;
        private InterSpaceType interInstancesSpaceType;
        private double interInstancesSpace;
        private List<double> interInstancesSpaces = new List<double> { 0 };
        private List<Orientation> instancesOrientations = new List<Orientation> { Orientation.Horizontal };

        private bool drawInterInstancesSpaces;
        private bool drawInstances;
        private readonly List<RectangleF> drawInstancesRects = new List<RectangleF>();
        private readonly List<RectangleF> drawInterInstancesSpacesRects = new List<RectangleF>();

        #endregion

        #endregion

        #region Public Methods

        public OMRConfigurationForm(ConfigurationBase.ConfigArea region)
        {
            configArea = region;
            this.Initialize(configArea.ConfigBitmap);
        }

        public OMRConfigurationForm(OMRConfiguration omrConfig, Bitmap regionImage)
        {
            this.Initialize(omrConfig, regionImage);
        }

        #endregion

        #region Private Methods

        private void Initialize(OMRConfiguration omrConfig, Bitmap regionImage)
        {
            synchronizationContext = SynchronizationContext.Current;

            this.InitializeComponent();

            statePanels.Add(LabelStatePanel);
            statePanels.Add(IntegerValueStatePanel);
            statePanels.Add(ComboBoxStatePanel);
            statePanels.Add(DoubleValueStatePanel);
            statePanels.Add(ComboValueStatePanel);

            LastStateAction = this.SetInterInstancesSpace;

            this.SetupForConfigured(omrConfig, regionImage);

            this.OnConfigurationFinishedEvent += this.OnConfigurationFinishedCallback;

            this.OnFormInitializedEvent?.Invoke(this, EventArgs.Empty);
        }

        private void Initialize(Bitmap regionImage)
        {
            synchronizationContext = SynchronizationContext.Current;

            this.InitializeComponent();

            statePanels.Add(LabelStatePanel);
            statePanels.Add(IntegerValueStatePanel);
            statePanels.Add(ComboBoxStatePanel);
            statePanels.Add(DoubleValueStatePanel);
            statePanels.Add(ComboValueStatePanel);

            LastStateAction = this.SetInterInstancesSpace;

            this.SetupForConfiguration(regionImage);

            this.OnConfigurationFinishedEvent += this.OnConfigurationFinishedCallback;

            this.OnFormInitializedEvent?.Invoke(this, EventArgs.Empty);
        }

        private void OnConfigurationFinishedCallback(string name, Orientation orientation, OMRRegionData _regionData)
        {
            this.SetupForConfigured(_regionData, this.RegionImage);
        }

        private void SetupForConfiguration(Bitmap region = null)
        {
            if (this.RegionData == null)
            {
                //Size = new Size(450, 505);
                this.MinimumSize = new Size(450, 505);

                MainLayoutPanel.RowStyles[1].SizeType = SizeType.Absolute;
                MainLayoutPanel.RowStyles[1].Height = 115;

                for (var i = 0; i < configureStatesPanel.RowCount; i++)
                {
                    configureStatesPanel.RowStyles[i].SizeType = SizeType.Percent;
                    configureStatesPanel.RowStyles[i].Height = i == 0 ? 0 : i == 1 ? 0 : i == 2 ? 50 : i == 3 ? 50 : 0;
                }
            }
            else
            {
                //Size = new Size(450, 570);
                this.MinimumSize = new Size(450, 505);

                MainLayoutPanel.RowStyles[1].SizeType = SizeType.Absolute;
                MainLayoutPanel.RowStyles[1].Height = 185;

                for (var i = 0; i < configureStatesPanel.RowCount; i++)
                {
                    configureStatesPanel.RowStyles[i].SizeType = SizeType.Percent;
                    configureStatesPanel.RowStyles[i].Height = i == 0 ? 0 : i == 1 ? 30 : i == 2 ? 35 : i == 3 ? 35 : 0;
                }

                selectStateComboBox.Visible = true;

                this.ConfigWalkthroughState = 0;
            }

            if (!curConfigureStatePanel.Controls.Contains(statePanelsPanel))
            {
                curConfigureStatePanel.Controls.Add(statePanelsPanel, 1, 0);
            }

            statePanelsPanel.Dock = DockStyle.Fill;
            statePanelsPanel.Visible = true;

            configureStatesPanel.Visible = true;

            this.RegionImage = region ?? this.RegionImage;
            imageBox.Image = this.RegionImage;

            this.StartWalkThrough();
        }

        private void SetupForConfigured(OMRConfiguration omrConfig, Bitmap region = null)
        {
            //Size = new Size(450, 456);
            this.MinimumSize = new Size(450, 456);

            MainLayoutPanel.RowStyles[1].SizeType = SizeType.Absolute;
            MainLayoutPanel.RowStyles[1].Height = 58;

            for (var i = 0; i < configureStatesPanel.RowCount; i++)
            {
                configureStatesPanel.RowStyles[i].SizeType = SizeType.Percent;
                configureStatesPanel.RowStyles[i].Height = i == 0 ? 100 : i == 1 ? 0 : i == 2 ? 0 : i == 3 ? 0 : 0;
            }

            selectStateComboBox.DataSource = EnumHelper.ToList(typeof(ConfigurationState));
            selectStateComboBox.DisplayMember = "Value";
            selectStateComboBox.ValueMember = "Key";

            configureStatesPanel.Visible = true;
            CurrentStatePanel = LabelStatePanel;

            this.RegionName = omrConfig.Title;
            this.RegionData = omrConfig.RegionData;
            configArea = omrConfig.GetConfigArea;
            totalFields = this.RegionData.TotalFields;
            totalOptions = this.RegionData.TotalOptions;
            totalInstances = this.RegionData.TotalInstances;
            orientation = omrConfig.Orientation;
            designerMode = omrConfig.RegionData.DesignerMode;
            fieldsRegion = omrConfig.RegionData.FieldsRegion;
            freeOptionsRegions = omrConfig.RegionData.FreeOptionsRegion;
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

            this.RegionImage = region == null ? this.RegionImage : region;
            imageBox.Image = region;

            drawInstances = false;
            drawInterInstancesSpaces = false;
            drawFields = true;
            drawInterFieldsSpaces = true;
            drawOptions = true;
            drawInterOptionsSpaces = true;
            this.CalculateRegions();
        }

        private void SetupForConfigured(OMRRegionData regionData, Bitmap region = null)
        {
            //Size = new Size(450, 456);
            this.MinimumSize = new Size(450, 456);

            MainLayoutPanel.RowStyles[1].SizeType = SizeType.Absolute;
            MainLayoutPanel.RowStyles[1].Height = 58;

            for (var i = 0; i < configureStatesPanel.RowCount; i++)
            {
                configureStatesPanel.RowStyles[i].SizeType = SizeType.Percent;
                configureStatesPanel.RowStyles[i].Height = i == 0 ? 100 : i == 1 ? 0 : i == 2 ? 0 : i == 3 ? 0 : 0;
            }

            selectStateComboBox.DataSource = EnumHelper.ToList(typeof(ConfigurationState));
            selectStateComboBox.DisplayMember = "Value";
            selectStateComboBox.ValueMember = "Key";

            configureStatesPanel.Visible = true;
            CurrentStatePanel = LabelStatePanel;

            this.RegionData = regionData;

            this.RegionImage = region == null ? this.RegionImage : region;
            imageBox.Image = region;

            drawInstances = false;
            drawInterInstancesSpaces = false;
            drawFields = true;
            drawOptions = true;
            this.CalculateRegions();
        }

        private void CalculateDynamicFieldsRects()
        {
            drawFieldsRects.Clear();

            var initialRectX = fieldsRegion.X;
            var initialRectY = fieldsRegion.Y;
            for (var i0 = 0; i0 < totalInstances; i0++)
            {
                switch (instancesOrientations[i0])
                {
                    case Orientation.Horizontal:
                        initialRectX = drawInstancesRects[i0].Left - configArea.ConfigRect.Left + fieldsRegion.Left;
                        break;

                    case Orientation.Vertical:
                        initialRectY = drawInstancesRects[i0].Top - configArea.ConfigRect.Top + fieldsRegion.Top;
                        break;
                }

                for (var i = 0; i < totalFields; i++)
                {
                    var curFieldRectF = new RectangleF();

                    switch (orientation)
                    {
                        case Orientation.Horizontal:
                            switch (interFieldsSpaceType)
                            {
                                case InterSpaceType.CONSTANT:
                                    curFieldRectF =
                                        new RectangleF(
                                            new PointF(initialRectX,
                                                initialRectY + (float)(i * (fieldsRegion.Height + interFieldsSpace))),
                                            fieldsRegion.Size);
                                    break;

                                case InterSpaceType.ARRAY:
                                    curFieldRectF =
                                        new RectangleF(
                                            new PointF(initialRectX,
                                                i == 0
                                                    ? initialRectY + (float)interFieldsSpaces[0]
                                                    : drawFieldsRects[i - 1].Bottom + (float)interFieldsSpaces[i]),
                                            fieldsRegion.Size);
                                    break;
                            }

                            break;

                        case Orientation.Vertical:
                            switch (interFieldsSpaceType)
                            {
                                case InterSpaceType.CONSTANT:
                                    curFieldRectF =
                                        new RectangleF(
                                            new PointF(
                                                initialRectX + (float)(i * (fieldsRegion.Width + interFieldsSpace)),
                                                initialRectY), fieldsRegion.Size);
                                    break;

                                case InterSpaceType.ARRAY:
                                    curFieldRectF =
                                        new RectangleF(
                                            new PointF(
                                                i == 0
                                                    ? initialRectX + (float)interFieldsSpaces[0]
                                                    : drawFieldsRects[i - 1].Right + (float)interFieldsSpaces[i],
                                                initialRectY), fieldsRegion.Size);
                                    break;
                            }

                            break;
                    }

                    drawFieldsRects.Add(curFieldRectF);
                }
            }
        }

        private void CalculateFreeOptionsRects()
        {
            drawOptionsRects.Clear();
            if (!(freeOptionsRegions?.Any() ?? false))
            {
                return;
            }

            var allOptions = freeOptionsRegions.Values.SelectMany(x => x).ToList();

            for (var i0 = 0; i0 < totalInstances; i0++)
                switch (instancesOrientations[i0])
                {
                    case Orientation.Horizontal:
                    {
                        var rects = allOptions.Select(x =>
                            new RectangleF(drawInstancesRects[i0].Left - configArea.ConfigRect.Left + x.Left, x.Y,
                                x.Width, x.Height)).ToList();

                        drawOptionsRects.AddRange(rects);
                    }
                        break;

                    case Orientation.Vertical:
                    {
                        var rects = allOptions.Select(x =>
                            new RectangleF(x.X, drawInstancesRects[i0].Top - configArea.ConfigRect.Top + x.Top,
                                x.Width, x.Height)).ToList();

                        drawOptionsRects.AddRange(rects);
                    }
                        break;
                }
        }

        private void DrawFields(Graphics g)
        {
            if (!drawFieldsRects.Any())
            {
                return;
            }

            GraphicsState originalState;

            var totalToDrawFields = drawInstances ? drawFieldsRects.Count : totalFields;
            for (var i = 0; i < totalToDrawFields; i++)
            {
                var fieldRectArea = drawFieldsRects[i];
                var curDrawFieldRectF = RectangleF.Empty;
                if (drawInstances)
                {
                    curDrawFieldRectF = imageBox.GetOffsetRectangle(new RectangleF(
                        configArea.ConfigRect.Left + fieldRectArea.Left, configArea.ConfigRect.Top + fieldRectArea.Top,
                        fieldRectArea.Width, fieldRectArea.Height));
                }
                else
                {
                    curDrawFieldRectF = imageBox.GetOffsetRectangle(fieldRectArea);
                }

                var rationSize = new SizeF(this.RegionImage.Size.Width / configArea.ConfigRect.Size.Width,
                    this.RegionImage.Size.Height / configArea.ConfigRect.Size.Height);
                curDrawFieldRectF.Width *= rationSize.Width;
                curDrawFieldRectF.Height *= rationSize.Height;

                originalState = g.Save();

                Functions.DrawBox(g, Color.CornflowerBlue, curDrawFieldRectF, imageBox.ZoomFactor, 84, 2,
                    DashStyle.Dash);

                g.Restore(originalState);
            }
        }

        private void CalculateInterFieldsSpacesRects()
        {
            drawInterFieldsSpacesRects.Clear();

            var interFieldsSpaceRegionVertical =
                new RectangleF(
                    new PointF(fieldsRegion.X + fieldsRegion.Size.Width / 2f - 2.7f,
                        fieldsRegion.Y + fieldsRegion.Size.Height), new SizeF(20f, (float)interFieldsSpace));
            var interFieldsSpaceRegionHorizontal = new RectangleF(
                new PointF(fieldsRegion.X + fieldsRegion.Size.Width,
                    fieldsRegion.Y + fieldsRegion.Size.Height / 2f - 2.7f), new SizeF((float)interFieldsSpace, 20f));

            var interSpaceVerticalWidth = 5f;
            var interSpaceHorizontalHeight = 5f;

            for (var i0 = 0; i0 < totalInstances; i0++)
            {
                switch (instancesOrientations[i0])
                {
                    case Orientation.Horizontal:
                        interFieldsSpaceRegionVertical.X = drawInstancesRects[i0].Left - configArea.ConfigRect.Left +
                            fieldsRegion.X + fieldsRegion.Size.Width / 2f - 2.7f;
                        interFieldsSpaceRegionHorizontal.X = drawInstancesRects[i0].Left - configArea.ConfigRect.Left +
                                                             fieldsRegion.X + fieldsRegion.Size.Width;
                        break;

                    case Orientation.Vertical:
                        interFieldsSpaceRegionVertical.Y = drawInstancesRects[i0].Top - configArea.ConfigRect.Top +
                                                           fieldsRegion.Y + fieldsRegion.Size.Height;
                        interFieldsSpaceRegionHorizontal.Y = drawInstancesRects[i0].Top - configArea.ConfigRect.Top +
                            fieldsRegion.Y + fieldsRegion.Size.Height / 2f - 2.7f;
                        break;
                }

                for (var i = 0; i < totalFields; i++)
                {
                    var curInterFieldSpaceRectF = new RectangleF();

                    switch (orientation)
                    {
                        case Orientation.Horizontal:
                            switch (interFieldsSpaceType)
                            {
                                case InterSpaceType.CONSTANT:
                                    if (i == totalFields - 1)
                                    {
                                        break;
                                    }

                                    curInterFieldSpaceRectF = new RectangleF(
                                        new PointF(interFieldsSpaceRegionVertical.X,
                                            interFieldsSpaceRegionVertical.Y +
                                            i * ((float)interFieldsSpace + fieldsRegion.Height)),
                                        new SizeF(interSpaceVerticalWidth, (float)interFieldsSpace));
                                    break;

                                case InterSpaceType.ARRAY:
                                    curInterFieldSpaceRectF = new RectangleF(
                                        new PointF(interFieldsSpaceRegionVertical.X,
                                            i == 0 ? fieldsRegion.Y : drawFieldsRects[i - 1].Bottom),
                                        new SizeF(interSpaceVerticalWidth, (float)interFieldsSpaces[i]));
                                    break;
                            }

                            break;

                        case Orientation.Vertical:
                            switch (interFieldsSpaceType)
                            {
                                case InterSpaceType.CONSTANT:
                                    if (i == totalFields - 1)
                                    {
                                        break;
                                    }

                                    curInterFieldSpaceRectF = new RectangleF(
                                        new PointF(
                                            interFieldsSpaceRegionHorizontal.X +
                                            i * ((float)interFieldsSpace + fieldsRegion.Width),
                                            interFieldsSpaceRegionHorizontal.Y),
                                        new SizeF((float)interFieldsSpace, interSpaceHorizontalHeight));
                                    break;

                                case InterSpaceType.ARRAY:
                                    curInterFieldSpaceRectF = new RectangleF(
                                        new PointF(i == 0 ? fieldsRegion.X : drawFieldsRects[i - 1].Right,
                                            interFieldsSpaceRegionHorizontal.Y),
                                        new SizeF((float)interFieldsSpaces[i], interSpaceHorizontalHeight));
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
            if (!drawInterFieldsSpacesRects.Any() || designerMode == DesignerModeType.FREE)
            {
                return;
            }

            GraphicsState originalState;

            var totalToDrawInterFieldsSpacesRects = drawInstances ? drawInterFieldsSpacesRects.Count :
                interFieldsSpaceType == InterSpaceType.CONSTANT ? totalFields - 1 : totalFields;
            for (var i = 0; i < totalToDrawInterFieldsSpacesRects; i++)
            {
                var curDrawInterFieldSpaceRectF = RectangleF.Empty;
                if (drawInstances)
                {
                    curDrawInterFieldSpaceRectF = imageBox.GetOffsetRectangle(new RectangleF(
                        configArea.ConfigRect.Left + drawInterFieldsSpacesRects[i].Left,
                        configArea.ConfigRect.Top + drawInterFieldsSpacesRects[i].Top,
                        drawInterFieldsSpacesRects[i].Width, drawInterFieldsSpacesRects[i].Height));
                }
                else
                {
                    curDrawInterFieldSpaceRectF = imageBox.GetOffsetRectangle(drawInterFieldsSpacesRects[i]);
                }

                originalState = g.Save();

                Functions.DrawBox(g, Color.CornflowerBlue, curDrawInterFieldSpaceRectF, imageBox.ZoomFactor, 200, 0);

                g.Restore(originalState);
            }
        }

        private void CalculateDynamicOptionsRects()
        {
            drawOptionsRects.Clear();

            var lastFieldOptionRect = new RectangleF();

            var initialRectX = optionsRegion.X;
            var initialRectY = optionsRegion.Y;
            for (var i0 = 0; i0 < totalInstances; i0++)
            {
                switch (instancesOrientations[i0])
                {
                    case Orientation.Horizontal:
                        initialRectX = drawInstancesRects[i0].Left - configArea.ConfigRect.Left + optionsRegion.Left;
                        break;

                    case Orientation.Vertical:
                        initialRectY = drawInstancesRects[i0].Top - configArea.ConfigRect.Top + optionsRegion.Top;
                        break;
                }

                for (var i1 = 0; i1 < totalFields; i1++)
                {
                    for (var i = 0; i < totalOptions; i++)
                    {
                        var curOptionRectF = new RectangleF();

                        switch (orientation)
                        {
                            case Orientation.Horizontal:
                                switch (interOptionsSpaceType)
                                {
                                    case InterSpaceType.CONSTANT:
                                        switch (interFieldsSpaceType)
                                        {
                                            case InterSpaceType.CONSTANT:
                                                curOptionRectF = new RectangleF(
                                                    new PointF(
                                                        initialRectX +
                                                        (float)(i * (optionsRegion.Width + interOptionsSpace)),
                                                        initialRectY +
                                                        (float)(i1 * (fieldsRegion.Height + interFieldsSpace))),
                                                    optionsRegion.Size);
                                                break;

                                            case InterSpaceType.ARRAY:
                                                curOptionRectF = new RectangleF(
                                                    new PointF(
                                                        initialRectX +
                                                        (float)(i * (optionsRegion.Width + interOptionsSpace)),
                                                        i1 == 0
                                                            ? (float)interFieldsSpaces[0] + initialRectY
                                                            : lastFieldOptionRect.Y + lastFieldOptionRect.Height +
                                                              (float)(fieldsRegion.Height + interFieldsSpaces[i1])),
                                                    optionsRegion.Size);
                                                break;
                                        }

                                        break;

                                    case InterSpaceType.ARRAY:
                                        switch (interFieldsSpaceType)
                                        {
                                            case InterSpaceType.CONSTANT:
                                                curOptionRectF = new RectangleF(
                                                    new PointF(
                                                        i == 0
                                                            ? initialRectX + (float)interOptionsSpaces[0]
                                                            : drawOptionsRects[i - 1].X +
                                                              drawOptionsRects[i - 1].Width +
                                                              (float)interOptionsSpaces[i],
                                                        initialRectY +
                                                        (float)(i1 * (fieldsRegion.Height + interFieldsSpace))),
                                                    optionsRegion.Size);
                                                break;

                                            case InterSpaceType.ARRAY:
                                                curOptionRectF = new RectangleF(
                                                    new PointF(
                                                        i == 0
                                                            ? initialRectX + (float)interOptionsSpaces[0]
                                                            : drawOptionsRects[i - 1].X +
                                                              drawOptionsRects[i - 1].Width +
                                                              (float)interOptionsSpaces[i],
                                                        i1 == 0
                                                            ? (float)interFieldsSpaces[0] + initialRectY
                                                            : lastFieldOptionRect.Y +
                                                              (float)(fieldsRegion.Height + interFieldsSpaces[i1])),
                                                    optionsRegion.Size);
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
                                                curOptionRectF = new RectangleF(
                                                    new PointF(
                                                        initialRectX +
                                                        (float)(i1 * (fieldsRegion.Width + interFieldsSpace)),
                                                        initialRectY +
                                                        (float)(i * (optionsRegion.Height + interOptionsSpace))),
                                                    optionsRegion.Size);
                                                break;

                                            case InterSpaceType.ARRAY:
                                                curOptionRectF = new RectangleF(
                                                    new PointF(
                                                        i1 == 0
                                                            ? (float)interFieldsSpaces[0] + initialRectX
                                                            : lastFieldOptionRect.X + fieldsRegion.Width +
                                                              (float)interFieldsSpaces[i1],
                                                        initialRectY +
                                                        (float)(i * (optionsRegion.Height + interOptionsSpace))),
                                                    optionsRegion.Size);
                                                break;
                                        }

                                        break;

                                    case InterSpaceType.ARRAY:
                                        switch (interFieldsSpaceType)
                                        {
                                            case InterSpaceType.CONSTANT:
                                                curOptionRectF = new RectangleF(
                                                    new PointF(
                                                        initialRectX +
                                                        (float)(i1 * (fieldsRegion.Width + interFieldsSpace)),
                                                        i == 0
                                                            ? (float)interOptionsSpaces[0] + initialRectY
                                                            : drawOptionsRects[i - 1].Bottom +
                                                              (float)interOptionsSpaces[i]), optionsRegion.Size);
                                                break;

                                            case InterSpaceType.ARRAY:
                                                curOptionRectF = new RectangleF(
                                                    new PointF(
                                                        i1 == 0
                                                            ? (float)interFieldsSpaces[0] + initialRectX
                                                            : lastFieldOptionRect.X + fieldsRegion.Width +
                                                              (float)interFieldsSpaces[i1],
                                                        i == 0
                                                            ? (float)interOptionsSpaces[0] + initialRectY
                                                            : drawOptionsRects[i - 1].Bottom +
                                                              (float)interOptionsSpaces[i]), optionsRegion.Size);
                                                break;
                                        }

                                        break;
                                }

                                break;
                        }

                        drawOptionsRects.Add(curOptionRectF);
                    }

                    lastFieldOptionRect = drawOptionsRects.Count == 0
                        ? new RectangleF()
                        : drawOptionsRects[drawOptionsRects.Count - 1];
                }
            }
        }

        private void DrawOptions(Graphics g)
        {
            GraphicsState originalState;

            var totalToDrawOptions = drawInstances ? drawOptionsRects.Count : totalOptions * totalFields;
            for (var i = 0; i < totalToDrawOptions; i++)
            {
                RectangleF curDrawOptionRectF = Rectangle.Empty;
                var fieldRectArea = drawOptionsRects[i];
                if (drawInstances)
                {
                    curDrawOptionRectF = imageBox.GetOffsetRectangle(new RectangleF(
                        configArea.ConfigRect.Left + fieldRectArea.Left, configArea.ConfigRect.Top + fieldRectArea.Top,
                        fieldRectArea.Width, fieldRectArea.Height));
                }
                else
                {
                    curDrawOptionRectF = imageBox.GetOffsetRectangle(fieldRectArea);
                }

                originalState = g.Save();

                var rationSize = new SizeF(this.RegionImage.Size.Width / configArea.ConfigRect.Size.Width,
                    this.RegionImage.Size.Height / configArea.ConfigRect.Size.Height);
                curDrawOptionRectF.Width *= rationSize.Width;
                curDrawOptionRectF.Height *= rationSize.Height;

                Functions.DrawBox(g, Color.Firebrick, curDrawOptionRectF, imageBox.ZoomFactor, 64, 1, DashStyle.Dash,
                    DashCap.Round);

                g.Restore(originalState);
            }
        }

        private void CalculateInterOptionsSpacesRects()
        {
            drawInterOptionsSpacesRects.Clear();

            var interOptionsSpaceRegionVertical =
                new RectangleF(
                    new PointF(optionsRegion.X + optionsRegion.Size.Width / 2f - 1.35f,
                        optionsRegion.Y + optionsRegion.Size.Height), new SizeF(10f, (float)interOptionsSpace));
            var interOptionsSpaceRegionHorizontal = new RectangleF(
                new PointF(optionsRegion.X + optionsRegion.Size.Width,
                    optionsRegion.Y + optionsRegion.Size.Height / 2f - 1.35f),
                new SizeF((float)interOptionsSpace, 10f));

            var interSpaceVerticalWidth = 2.5f;
            var interSpaceHorizontalHeight = 2.5f;

            for (var i0 = 0; i0 < totalInstances; i0++)
            {
                switch (instancesOrientations[i0])
                {
                    case Orientation.Horizontal:
                        interOptionsSpaceRegionVertical.X = drawInstancesRects[i0].Left - configArea.ConfigRect.Left +
                            optionsRegion.X + optionsRegion.Size.Width / 2f - 1.35f;
                        interOptionsSpaceRegionHorizontal.X = drawInstancesRects[i0].Left - configArea.ConfigRect.Left +
                                                              optionsRegion.X + optionsRegion.Size.Width;
                        break;

                    case Orientation.Vertical:
                        interOptionsSpaceRegionVertical.Y = drawInstancesRects[i0].Top - configArea.ConfigRect.Top +
                                                            optionsRegion.Y + optionsRegion.Size.Height;
                        interOptionsSpaceRegionHorizontal.Y = drawInstancesRects[i0].Top - configArea.ConfigRect.Top +
                            optionsRegion.Y + optionsRegion.Size.Height / 2f - 1.35f;
                        break;
                }

                var lastFieldInterOptionRect = new RectangleF();
                for (var i1 = 0; i1 < totalFields; i1++)
                {
                    var curInterOptionSpaceRectF = new RectangleF();

                    for (var i = 0; i < totalOptions; i++)
                    {
                        switch (orientation)
                        {
                            case Orientation.Horizontal:
                                switch (interOptionsSpaceType)
                                {
                                    case InterSpaceType.CONSTANT:
                                        if (i == totalOptions - 1)
                                        {
                                            break;
                                        }

                                        switch (interFieldsSpaceType)
                                        {
                                            case InterSpaceType.CONSTANT:
                                                curInterOptionSpaceRectF = new RectangleF(
                                                    new PointF(
                                                        interOptionsSpaceRegionHorizontal.X +
                                                        (float)(i * (interOptionsSpace + optionsRegion.Width)),
                                                        interOptionsSpaceRegionHorizontal.Y +
                                                        (float)(i1 * (interFieldsSpace + fieldsRegion.Height))),
                                                    new SizeF((float)interOptionsSpace, interSpaceHorizontalHeight));
                                                break;

                                            case InterSpaceType.ARRAY:
                                                curInterOptionSpaceRectF = new RectangleF(
                                                    new PointF(
                                                        interOptionsSpaceRegionHorizontal.X +
                                                        (float)(i * (interOptionsSpace + optionsRegion.Width)),
                                                        interOptionsSpaceRegionHorizontal.Y +
                                                        (float)(i1 * (interFieldsSpaces[i1] + fieldsRegion.Height))),
                                                    new SizeF((float)interOptionsSpace, interSpaceHorizontalHeight));
                                                break;
                                        }

                                        break;

                                    case InterSpaceType.ARRAY:
                                        switch (interFieldsSpaceType)
                                        {
                                            case InterSpaceType.CONSTANT:
                                                curInterOptionSpaceRectF = new RectangleF(
                                                    new PointF(i == 0 ? optionsRegion.X : drawOptionsRects[i - 1].Right,
                                                        interOptionsSpaceRegionHorizontal.Y +
                                                        (float)(i1 * (interFieldsSpace + fieldsRegion.Height))),
                                                    new SizeF((float)interOptionsSpaces[i],
                                                        interSpaceHorizontalHeight));
                                                break;

                                            case InterSpaceType.ARRAY:
                                                curInterOptionSpaceRectF = new RectangleF(
                                                    new PointF(i == 0 ? optionsRegion.X : drawOptionsRects[i - 1].Right,
                                                        i1 == 0
                                                            ? interOptionsSpaceRegionHorizontal.Y +
                                                              (float)interFieldsSpaces[0]
                                                            : lastFieldInterOptionRect.Y +
                                                              (float)(interFieldsSpaces[i1] + fieldsRegion.Height)),
                                                    new SizeF((float)interOptionsSpaces[i],
                                                        interSpaceHorizontalHeight));
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
                                        {
                                            break;
                                        }

                                        switch (interFieldsSpaceType)
                                        {
                                            case InterSpaceType.CONSTANT:
                                                curInterOptionSpaceRectF = new RectangleF(
                                                    new PointF(
                                                        interOptionsSpaceRegionVertical.X +
                                                        (float)(i1 * (interFieldsSpace + fieldsRegion.Width)),
                                                        interOptionsSpaceRegionVertical.Y +
                                                        (float)(i * (interOptionsSpace + optionsRegion.Height))),
                                                    new SizeF(interSpaceVerticalWidth, (float)interOptionsSpace));
                                                break;

                                            case InterSpaceType.ARRAY:
                                                curInterOptionSpaceRectF = new RectangleF(
                                                    new PointF(
                                                        interOptionsSpaceRegionVertical.X +
                                                        (float)(i1 * (interFieldsSpaces[i1] + fieldsRegion.Width)),
                                                        interOptionsSpaceRegionVertical.Y +
                                                        (float)(i * (interOptionsSpace + optionsRegion.Height))),
                                                    new SizeF(interSpaceVerticalWidth, (float)interOptionsSpace));
                                                break;
                                        }

                                        break;

                                    case InterSpaceType.ARRAY:
                                        switch (interFieldsSpaceType)
                                        {
                                            case InterSpaceType.CONSTANT:
                                                curInterOptionSpaceRectF = new RectangleF(
                                                    new PointF(
                                                        interOptionsSpaceRegionVertical.X +
                                                        (float)(i1 * (interFieldsSpace + fieldsRegion.Width)),
                                                        i == 0 ? optionsRegion.Y : drawOptionsRects[i - 1].Bottom),
                                                    new SizeF(interSpaceVerticalWidth, (float)interOptionsSpaces[i]));
                                                break;

                                            case InterSpaceType.ARRAY:
                                                curInterOptionSpaceRectF = new RectangleF(
                                                    new PointF(
                                                        interOptionsSpaceRegionVertical.X +
                                                        (float)(i1 * (interFieldsSpaces[i1] + fieldsRegion.Width)),
                                                        i == 0 ? optionsRegion.Y : drawOptionsRects[i - 1].Bottom),
                                                    new SizeF(interSpaceVerticalWidth, (float)interOptionsSpaces[i]));
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
            if (!drawInterOptionsSpacesRects.Any() || designerMode == DesignerModeType.FREE)
            {
                return;
            }

            GraphicsState originalState;

            var totalToDrawInterOptionsSpacesRects = drawInstances ? drawInterOptionsSpacesRects.Count :
                interOptionsSpaceType == InterSpaceType.CONSTANT ? totalOptions - 1 : totalOptions;
            for (var i = 0; i < totalToDrawInterOptionsSpacesRects; i++)
            {
                var curDrawInterOptionSpaceRectF = RectangleF.Empty;
                if (drawInstances)
                {
                    curDrawInterOptionSpaceRectF = imageBox.GetOffsetRectangle(new RectangleF(
                        configArea.ConfigRect.Left + drawInterOptionsSpacesRects[i].Left,
                        configArea.ConfigRect.Top + drawInterOptionsSpacesRects[i].Top,
                        drawInterOptionsSpacesRects[i].Width, drawInterOptionsSpacesRects[i].Height));
                }
                else
                {
                    curDrawInterOptionSpaceRectF = imageBox.GetOffsetRectangle(drawInterOptionsSpacesRects[i]);
                }

                originalState = g.Save();

                Functions.DrawBox(g, Color.Firebrick, curDrawInterOptionSpaceRectF, imageBox.ZoomFactor, 200, 0);

                g.Restore(originalState);
            }
        }

        private void CalculateInstancesRects()
        {
            drawInstancesRects.Clear();

            var curInstanceRect = RectangleF.Empty;
            switch (interInstancesSpaceType)
            {
                case InterSpaceType.CONSTANT:
                    for (var i = 0; i < totalInstances; i++)
                    {
                        curInstanceRect = i == 0 ? configArea.ConfigRect : drawInstancesRects.Last();
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
                    for (var i = 0; i < totalInstances; i++)
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

            for (var i = 0; i < drawInstancesRects.Count; i++)
            {
                var curDrawInstanceRectF = imageBox.GetOffsetRectangle(drawInstancesRects[i]);

                originalState = g.Save();

                Functions.DrawBox(g, Color.Firebrick, curDrawInstanceRectF, imageBox.ZoomFactor, 64, 1, DashStyle.Dash,
                    DashCap.Round);

                g.Restore(originalState);
            }
        }

        private void CalculateInterInstancesSpacesRects()
        {
            drawInterInstancesSpacesRects.Clear();

            var curInterInstanceSpaceRect = RectangleF.Empty;
            var lastInstanceRect = RectangleF.Empty;
            switch (interInstancesSpaceType)
            {
                case InterSpaceType.CONSTANT:
                    for (var i = 1; i < totalInstances; i++)
                    {
                        lastInstanceRect = drawInstancesRects[i - 1];
                        switch (instancesOrientations[i])
                        {
                            case Orientation.Horizontal:
                                curInterInstanceSpaceRect =
                                    new RectangleF(
                                        new PointF(lastInstanceRect.Right,
                                            lastInstanceRect.Y + (lastInstanceRect.Height / 2f - 10)),
                                        new SizeF((float)interInstancesSpace, 20));
                                break;

                            case Orientation.Vertical:
                                curInterInstanceSpaceRect =
                                    new RectangleF(
                                        new PointF(lastInstanceRect.X + (lastInstanceRect.Width / 2f - 10),
                                            lastInstanceRect.Bottom), new SizeF(20, (float)interInstancesSpace));
                                break;
                        }

                        drawInterInstancesSpacesRects.Add(curInterInstanceSpaceRect);
                    }

                    break;

                case InterSpaceType.ARRAY:
                    for (var i = 0; i < totalInstances; i++)
                    {
                        lastInstanceRect = drawInstancesRects[i];
                        if (i == 0)
                        {
                            switch (instancesOrientations[i])
                            {
                                case Orientation.Horizontal:
                                    curInterInstanceSpaceRect = new RectangleF(
                                        new PointF(lastInstanceRect.Left,
                                            lastInstanceRect.Y + (lastInstanceRect.Height / 2f - 10)),
                                        new SizeF((float)interInstancesSpaces[i], 20));
                                    break;

                                case Orientation.Vertical:
                                    curInterInstanceSpaceRect = new RectangleF(
                                        new PointF(lastInstanceRect.X + (lastInstanceRect.Width / 2f - 10),
                                            lastInstanceRect.Bottom), new SizeF(20, (float)interInstancesSpaces[i]));
                                    break;
                            }

                            continue;
                        }

                        switch (instancesOrientations[i])
                        {
                            case Orientation.Horizontal:
                                curInterInstanceSpaceRect =
                                    new RectangleF(
                                        new PointF(lastInstanceRect.Right,
                                            lastInstanceRect.Y + (lastInstanceRect.Height / 2f - 10)),
                                        new SizeF((float)interInstancesSpaces[i], 20));
                                break;

                            case Orientation.Vertical:
                                curInterInstanceSpaceRect =
                                    new RectangleF(
                                        new PointF(lastInstanceRect.X + (lastInstanceRect.Width / 2f - 10),
                                            lastInstanceRect.Top), new SizeF(20, (float)interInstancesSpaces[i]));
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

            for (var i = 0; i < drawInterInstancesSpacesRects.Count; i++)
            {
                var curDrawInterInstanceSpaceRectF = imageBox.GetOffsetRectangle(drawInterInstancesSpacesRects[i]);

                originalState = g.Save();

                Functions.DrawBox(g, Color.Firebrick, curDrawInterInstanceSpaceRectF, imageBox.ZoomFactor, 200, 0);

                g.Restore(originalState);
            }
        }

        private void CalculateFields()
        {
            switch (designerMode)
            {
                case DesignerModeType.DYNAMIC:
                {
                    this.CalculateDynamicFieldsRects();
                    this.CalculateInterFieldsSpacesRects();
                }
                    break;
            }

            imageBox.Invalidate();
        }

        private void CalculateOptions()
        {
            if (designerMode == DesignerModeType.DYNAMIC)
            {
                this.CalculateDynamicOptionsRects();
                this.CalculateInterOptionsSpacesRects();
            }
            else if (designerMode == DesignerModeType.FREE)
            {
                this.CalculateFreeOptionsRects();
            }

            imageBox.Invalidate();
        }

        private void CalculateRegions()
        {
            this.CalculateInstancesRegion();
            this.CalculateFields();
            this.CalculateOptions();
        }

        private void CalculateInstancesRegion()
        {
            this.CalculateInstancesRects();
            this.CalculateInterInstancesSpacesRects();

            imageBox.Invalidate();
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

            if (drawInstances)
            {
                this.DrawInstancesRects(e.Graphics);

                if (drawInterInstancesSpaces)
                {
                    this.DrawInterInstancesSpaces(e.Graphics);
                }
            }

            if (drawFields)
            {
                this.DrawFields(e.Graphics);

                if (drawInterFieldsSpaces)
                {
                    this.DrawInterFieldsSpaces(e.Graphics);
                }
            }

            if (drawOptions)
            {
                this.DrawOptions(e.Graphics);

                if (drawInterOptionsSpaces)
                {
                    this.DrawInterOptionsSpaces(e.Graphics);
                }
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

        private void StartWalkThrough()
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
            new Animator2D(new Path2D(walkthroughIndexLabel.Location.ToFloat2D(), new Float2D(0, 50), 160)).Play(
                walkthroughIndexLabel, Animator2D.KnownProperties.Location, new SafeInvoker(() =>
                {
                    synchronizationContext.Send(
                        delegate { walkthroughIndexLabel.Text = (int)(this.ConfigWalkthroughState + 1) + "."; },
                        null);

                    new Animator2D(new Path2D(walkthroughIndexLabel.Location.ToFloat2D(), new Float2D(0, 0), 160)).Play(
                        walkthroughIndexLabel, Animator2D.KnownProperties.Location, new SafeInvoker(() =>
                        {
                            synchronizationContext.Send(delegate { walkthroughIndexLabel.Dock = DockStyle.Fill; },
                                null);
                        }));
                }));

            if (CurrentStatePanel != null)
            {
                var curStatePanel = CurrentStatePanel;
                curStatePanel.Dock = DockStyle.None;
                curStatePanel.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;
                curStatePanel.Visible = false;
                //new Animator2D(new Path2D(CurrentStatePanel.Location.ToFloat2D(), new Float2D(-400, 0), 250)).Play(
                //    CurrentStatePanel, Animator2D.KnownProperties.Location, new SafeInvoker(() =>
                //    {
                //        synchronizationContext.Send(delegate { curStatePanel.Visible = false; },
                //            null);
                //    }));
            }

            switch (state)
            {
                case ConfigurationState.SELECT_REGION_NAME:
                    stringValueStateTextBox.Text = "";
                    for (var i = 0; i < stringValueStateControlsLayoutPanel.ColumnCount; i++)
                    {
                        stringValueStateControlsLayoutPanel.ColumnStyles[i].SizeType = SizeType.Percent;
                        stringValueStateControlsLayoutPanel.ColumnStyles[i].Width = i == 0 ? 0 : i == 1 ? 100 : 0;
                    }

                    CurrentStatePanel = StringValueStatePanel;
                    break;

                case ConfigurationState.SELECT_DESIGNER_MODE:
                    comboBoxStateComboBox.DataSource = EnumHelper.ToList(typeof(DesignerModeType));
                    comboBoxStateComboBox.DisplayMember = "Value";
                    comboBoxStateComboBox.ValueMember = "Key";

                    CurrentStatePanel = ComboBoxStatePanel;
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
                    for (var i = 0; i < integerStateControlsTablePanel.ColumnCount; i++)
                    {
                        integerStateControlsTablePanel.ColumnStyles[i].SizeType = SizeType.Percent;
                        integerStateControlsTablePanel.ColumnStyles[i].Width = i == 0 ? 0 : i == 1 ? 100 : 0;
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
                            for (var i = 0; i < doubleStageControlsPanel.ColumnCount; i++)
                            {
                                doubleStageControlsPanel.ColumnStyles[i].SizeType = SizeType.Percent;
                                doubleStageControlsPanel.ColumnStyles[i].Width = i == 0 ? 0 : i == 1 ? 100 : 0;
                            }

                            break;

                        case InterSpaceType.ARRAY:
                            doubleStateComboBox.Items.Clear();
                            for (var i = 0; i < totalFields; i++) doubleStateComboBox.Items.Add(i + 1);
                            doubleStateComboBox.SelectedIndex = 0;
                            for (var i = 0; i < doubleStageControlsPanel.ColumnCount; i++)
                            {
                                doubleStageControlsPanel.ColumnStyles[i].SizeType = SizeType.Percent;
                                doubleStageControlsPanel.ColumnStyles[i].Width = i == 0 ? 50 : i == 1 ? 50 : 0;
                            }

                            break;
                    }

                    CurrentStatePanel = DoubleValueStatePanel;
                    break;

                case ConfigurationState.SELECT_OPTION_REGION:
                    switch (designerMode)
                    {
                        case DesignerModeType.DYNAMIC:
                            CurrentStatePanel = LabelStatePanel;
                            break;

                        case DesignerModeType.FREE:
                            multiComboStateLabel.Text = "Select Option Regions:";
                            multiComboStateFirstCombo.DataSource = Enumerable.Range(1, totalFields).ToList();
                            multiComboStateSecondCombo.DataSource = Enumerable.Range(1, totalOptions).ToList();
                            CurrentStatePanel = MultiComboStatePanel;
                            break;
                    }

                    break;

                case ConfigurationState.PROVIDE_TOTAL_OPTIONS:
                    integerStateValueTextBox.IntegerValue = 0;
                    for (var i = 0; i < integerStateControlsTablePanel.ColumnCount; i++)
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
                            for (var i = 0; i < doubleStageControlsPanel.ColumnCount; i++)
                            {
                                doubleStageControlsPanel.ColumnStyles[i].SizeType = SizeType.Percent;
                                doubleStageControlsPanel.ColumnStyles[i].Width = i == 0 ? 0 : i == 1 ? 100 : 0;
                            }

                            break;

                        case InterSpaceType.ARRAY:
                            doubleStateComboBox.Items.Clear();
                            for (var i = 0; i < totalOptions; i++) doubleStateComboBox.Items.Add(i + 1);
                            doubleStateComboBox.SelectedIndex = 0;
                            for (var i = 0; i < doubleStageControlsPanel.ColumnCount; i++)
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
                    for (var i = 0; i < integerStateControlsTablePanel.ColumnCount; i++)
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
                    for (var i = 0; i < totalInstances; i++) comboValueStateComboBox.Items.Add(i + 1);
                    comboValueStateComboBox.SelectedIndex = 0;
                    for (var i = 0; i < comboValueStateTablePanel.ColumnCount; i++)
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
                            for (var i = 0; i < doubleStageControlsPanel.ColumnCount; i++)
                            {
                                doubleStageControlsPanel.ColumnStyles[i].SizeType = SizeType.Percent;
                                doubleStageControlsPanel.ColumnStyles[i].Width = i == 0 ? 0 : i == 1 ? 100 : 0;
                            }

                            break;

                        case InterSpaceType.ARRAY:
                            doubleStateComboBox.Items.Clear();
                            for (var i = 0; i < totalInstances; i++) doubleStateComboBox.Items.Add(i + 1);
                            doubleStateComboBox.SelectedIndex = 0;
                            for (var i = 0; i < doubleStageControlsPanel.ColumnCount; i++)
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
            CurrentStatePanel.Dock = DockStyle.Fill;
            //new Animator2D(new Path2D(CurrentStatePanel.Location.ToFloat2D(), new Float2D(0, 0), 250)).Play(
            //    CurrentStatePanel, Animator2D.KnownProperties.Location, new SafeInvoker(() =>
            //    {
            //        synchronizationContext.Send(delegate { CurrentStatePanel.Dock = DockStyle.Fill; },
            //            null);
            //    }));
        }

        private void SetupState(ConfigurationState walkThroughState)
        {
            this.InitializeStatePanel(walkThroughState);

            SelectionRegionChangedAction = null;
            SelectionRegionResizedAction = null;
            switch (walkThroughState)
            {
                case ConfigurationState.SELECT_REGION_NAME:
                    stringValueStateLabel.Text = "Region Name:";
                    stringValueStateTextBox.Text =
                        string.IsNullOrEmpty(this.RegionName) ? "OMR Region" : this.RegionName;

                    CurrentSetAction = this.SetRegionName;

                    nextBtn.Text = "NEXT";
                    nextBtn.BackColor = Color.LightSlateGray;
                    CurrentNextAction = this.NextState;
                    break;

                case ConfigurationState.SELECT_DESIGNER_MODE:
                    comboBoxStateComboBox.Text = "Select Designer Mode";
                    comboBoxStateComboBox.SelectedIndex = this.RegionData == null ? -1 : (int)designerMode;

                    CurrentSetAction = this.SetRegionDesignerMode;

                    nextBtn.Text = "NEXT";
                    nextBtn.BackColor = Color.LightSlateGray;
                    CurrentNextAction = this.NextState;
                    break;

                case ConfigurationState.SELECT_REGION_ORIENTATION:
                    comboBoxStateComboBox.Text = "Select Orientation";
                    comboBoxStateComboBox.SelectedIndex = this.RegionData == null ? -1 : (int)orientation;

                    CurrentSetAction = this.SetRegionOrientation;

                    nextBtn.Text = "NEXT";
                    nextBtn.BackColor = Color.LightSlateGray;
                    CurrentNextAction = this.NextState;
                    break;

                case ConfigurationState.SELECT_FIELD_REGION:
                    walkthroughDescriptionLabel.Text = "Please select the first field region.";

                    CurrentSetAction = this.SetFieldRegion;

                    nextBtn.Text = "NEXT";
                    nextBtn.BackColor = Color.LightSlateGray;
                    CurrentNextAction = this.NextState;
                    break;

                case ConfigurationState.PROVIDE_TOTAL_FIELDS:
                    integerStateLabel.Text = "Total Fields:";
                    integerStateValueTextBox.IntegerValue = this.RegionData == null ? 1 : totalFields;

                    CurrentSetAction = this.SetTotalFields;
                    CurrentNextAction = this.NextState;
                    break;

                case ConfigurationState.SELECT_INTER_FIELDS_SPACE_TYPE:
                    comboBoxStateComboBox.Text = "Inter Fields Space Type";
                    comboBoxStateComboBox.SelectedIndex =
                        this.RegionData == null ? -1 : (int)this.RegionData.InterFieldsSpaceType;

                    CurrentSetAction = this.SetInterFieldsSpaceType;

                    nextBtn.Text = "NEXT";
                    nextBtn.BackColor = Color.LightSlateGray;
                    CurrentNextAction = this.NextState;
                    break;

                case ConfigurationState.SELECT_INTER_FIELDS_SPACE:
                    doubleStateLabel.Text = "Inter Fields Space:";
                    doubleStateValueTextBox.DoubleValue =
                        this.RegionData == null ? 0.00 : this.RegionData.InterFieldsSpace;

                    DoubleStateComboAction = this.SetInterFieldsComboSpace;
                    CurrentSetAction = this.SetInterFieldsSpace;

                    SelectionRegionChangedAction = () =>
                    {
                        if (imageBox.SelectionRegion.IsEmpty)
                        {
                            return;
                        }

                        switch (orientation)
                        {
                            case Orientation.Horizontal:
                                this.SetInterFieldsSpace(imageBox.SelectionRegion.Height);
                                break;

                            case Orientation.Vertical:
                                this.SetInterFieldsSpace(imageBox.SelectionRegion.Width);
                                break;
                        }
                    };
                    SelectionRegionResizedAction = () =>
                    {
                        if (imageBox.SelectionRegion.IsEmpty)
                        {
                            return;
                        }

                        switch (orientation)
                        {
                            case Orientation.Horizontal:
                                this.SetInterFieldsSpace(imageBox.SelectionRegion.Height);
                                break;

                            case Orientation.Vertical:
                                this.SetInterFieldsSpace(imageBox.SelectionRegion.Width);
                                break;
                        }
                    };

                    nextBtn.Text = "NEXT";
                    nextBtn.BackColor = Color.LightSlateGray;
                    CurrentNextAction = this.NextState;
                    break;

                case ConfigurationState.SELECT_OPTION_REGION:
                    switch (designerMode)
                    {
                        case DesignerModeType.DYNAMIC:
                            walkthroughDescriptionLabel.Text = "Please select the first option region.";

                            CurrentSetAction = this.SetDynamicOptionRegion;
                            break;

                        case DesignerModeType.FREE:
                            CurrentSetAction = this.SetFreeOptionRegion;
                            break;
                    }

                    nextBtn.Text = "NEXT";
                    nextBtn.BackColor = Color.LightSlateGray;
                    CurrentNextAction = this.NextState;
                    break;

                case ConfigurationState.PROVIDE_TOTAL_OPTIONS:
                    integerStateLabel.Text = "Total Options:";
                    integerStateValueTextBox.IntegerValue = this.RegionData == null ? 1 : totalOptions;

                    CurrentSetAction = this.SetTotalOptions;
                    break;

                case ConfigurationState.SELECT_INTER_OPTIONS_SPACE_TYPE:
                    comboBoxStateComboBox.Text = "Inter Options Space Type";
                    comboBoxStateComboBox.SelectedIndex =
                        this.RegionData == null ? -1 : (int)this.RegionData.InterOptionsSpaceType;

                    CurrentSetAction = this.SetInterOptionsSpaceType;

                    nextBtn.Text = "NEXT";
                    nextBtn.BackColor = Color.LightSlateGray;
                    CurrentNextAction = this.NextState;
                    break;

                case ConfigurationState.SELECT_INTER_OPTIONS_SPACE:
                    doubleStateLabel.Text = "Inter Options Space:";
                    doubleStateValueTextBox.DoubleValue =
                        this.RegionData == null ? 0.00 : this.RegionData.InterOptionsSpace;

                    DoubleStateComboAction = this.SetInterOptionsComboSpace;
                    CurrentSetAction = this.SetInterOptionsSpace;

                    SelectionRegionChangedAction = () =>
                    {
                        if (imageBox.SelectionRegion.IsEmpty)
                        {
                            return;
                        }

                        switch (orientation)
                        {
                            case Orientation.Horizontal:
                                this.SetInterOptionsSpace(imageBox.SelectionRegion.Width);
                                break;

                            case Orientation.Vertical:
                                this.SetInterOptionsSpace(imageBox.SelectionRegion.Height);
                                break;
                        }
                    };
                    SelectionRegionResizedAction = () =>
                    {
                        if (imageBox.SelectionRegion.IsEmpty)
                        {
                            return;
                        }

                        switch (orientation)
                        {
                            case Orientation.Horizontal:
                                this.SetInterOptionsSpace(imageBox.SelectionRegion.Width);
                                break;

                            case Orientation.Vertical:
                                this.SetInterOptionsSpace(imageBox.SelectionRegion.Height);
                                break;
                        }
                    };

                    nextBtn.Text = "NEXT";
                    nextBtn.BackColor = Color.LightSlateGray;
                    CurrentNextAction = this.NextState;
                    break;

                case ConfigurationState.SELECT_INSTANCES_COUNT:
                    imageBox.Image = SynapseMain.GetSynapseMain.GetCurrentImage();
                    drawInstances = true;
                    drawInterInstancesSpaces = true;
                    imageBox.Invalidate();

                    totalInstances = totalInstances == 0 ? 1 : totalInstances;
                    integerStateLabel.Text = "Total Instances:";
                    integerStateValueTextBox.IntegerValue = totalInstances;
                    this.CalculateRegions();

                    CurrentSetAction = this.SetTotalInstances;
                    break;

                case ConfigurationState.SELECT_INSTANCES_ORIENTATION:
                    comboValueStateLabel.Text = "Select Orientations";

                    comboValueStateValueComboBox.Text = "Select Instances Orientations";
                    comboValueStateValueComboBox.SelectedIndex = this.RegionData == null
                        ? -1
                        : (int)instancesOrientations[comboValueStateComboBox.SelectedIndex];

                    CurrentSetAction = this.SetInstancesOrientation;

                    nextBtn.Text = "NEXT";
                    nextBtn.BackColor = Color.LightSlateGray;
                    CurrentNextAction = this.NextState;
                    break;

                case ConfigurationState.SELECT_INTER_INSTANCES_SPACE_TYPE:
                    comboBoxStateComboBox.Text = "Inter Instances Space Type";
                    comboBoxStateComboBox.SelectedIndex =
                        this.RegionData == null ? -1 : (int)this.RegionData.InterInstancesSpaceType;

                    CurrentSetAction = this.SetInterInstancesSpaceType;

                    nextBtn.Text = "NEXT";
                    nextBtn.BackColor = Color.LightSlateGray;
                    CurrentNextAction = this.NextState;
                    break;

                case ConfigurationState.SELECT_INTER_INSTANCES_SPACE:
                    doubleStateLabel.Text = "Inter Instances Space:";
                    doubleStateValueTextBox.DoubleValue =
                        this.RegionData == null ? 0.00 : this.RegionData.InterInstancesSpace;

                    DoubleStateComboAction = this.SetInterInstancesComboSpace;
                    CurrentSetAction = this.SetInterInstancesSpace;

                    SelectionRegionChangedAction = () =>
                    {
                        if (imageBox.SelectionRegion.IsEmpty)
                        {
                            return;
                        }

                        switch (instancesOrientations[0])
                        {
                            case Orientation.Horizontal:
                                this.SetInterInstancesSpace(imageBox.SelectionRegion.Width);
                                break;

                            case Orientation.Vertical:
                                this.SetInterInstancesSpace(imageBox.SelectionRegion.Height);
                                break;
                        }
                    };
                    SelectionRegionResizedAction = () =>
                    {
                        if (imageBox.SelectionRegion.IsEmpty)
                        {
                            return;
                        }

                        switch (orientation)
                        {
                            case Orientation.Horizontal:
                                this.SetInterInstancesSpace(imageBox.SelectionRegion.Width);
                                break;

                            case Orientation.Vertical:
                                this.SetInterInstancesSpace(imageBox.SelectionRegion.Height);
                                break;
                        }
                    };

                    nextBtn.Text = "FINISH";
                    nextBtn.BackColor = Color.MediumTurquoise;
                    CurrentNextAction = this.EndWalkThrough;
                    break;
            }
        }

        private void NextState()
        {
            if (!isCurrentSetActionCompleted && this.RegionData == null)
            {
                this.InvalidateState();

                return;
            }

            this.ValidateState();

            isCurrentSetActionCompleted = false;

            var nextState = this.ConfigWalkthroughState + 1;

            switch (this.ConfigWalkthroughState)
            {
                case ConfigurationState.SELECT_REGION_NAME:
                    break;

                case ConfigurationState.SELECT_DESIGNER_MODE:
                    if (designerMode == DesignerModeType.FREE)
                    {
                        nextState = ConfigurationState.PROVIDE_TOTAL_FIELDS;
                    }

                    break;

                case ConfigurationState.SELECT_REGION_ORIENTATION:
                    break;

                case ConfigurationState.SELECT_FIELD_REGION:
                    imageBox.SelectNone();
                    break;

                case ConfigurationState.PROVIDE_TOTAL_FIELDS:
                    if (designerMode == DesignerModeType.FREE)
                    {
                        nextState = ConfigurationState.PROVIDE_TOTAL_OPTIONS;
                    }

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
                    switch (designerMode)
                    {
                        case DesignerModeType.FREE:
                        {
                            nextState = ConfigurationState.SELECT_INSTANCES_COUNT;
                        }
                            break;
                    }

                    break;

                case ConfigurationState.PROVIDE_TOTAL_OPTIONS:
                    switch (designerMode)
                    {
                        case DesignerModeType.FREE:
                        {
                            nextState = ConfigurationState.SELECT_OPTION_REGION;
                        }
                            break;
                    }

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
                    {
                        this.EndWalkThrough();
                    }

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

            if (this.RegionData != null)
            {
                selectStateComboBox.SelectedIndex = (int)nextState;
            }
            else
            {
                this.ConfigWalkthroughState = nextState;
            }
        }

        private void ValidateState()
        {
            walkthroughIndexLabel.ForeColor = Color.FromArgb(255, 68, 68, 68);
            switch (this.ConfigWalkthroughState)
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
            switch (this.ConfigWalkthroughState)
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

        private void EndWalkThrough()
        {
            LastStateAction();

            if (!isCurrentSetActionCompleted)
            {
                return;
            }

            imageBox.SelectNone();

            var regionData = new OMRRegionData(orientation, totalFields, fieldsRegion, interFieldsSpaceType,
                interFieldsSpace, interFieldsSpaces.ToArray(), totalOptions, optionsRegion, interOptionsSpaceType,
                interOptionsSpace, interOptionsSpaces.ToArray(), totalInstances, configArea.ConfigRect,
                instancesOrientations.ToArray(), interInstancesSpaceType, interInstancesSpace,
                interInstancesSpaces.ToArray(), designerMode, freeOptionsRegions);
            this.OnConfigurationFinishedEvent?.Invoke(this.RegionName, orientation, regionData);
        }

        private bool ValidateName(string name)
        {
            var isValid = true;

            if (name == "" || name[0] == ' ' || name[name.Length - 1] == ' ')
            {
                isValid = false;
            }

            if (isValid)
            {
                isValid = ConfigurationsManager.ValidateName(name);
            }

            return isValid;
        }

        private void SetRegionName()
        {
            var name = stringValueStateTextBox.Text;
            if (!this.ValidateName(name))
            {
                this.InvalidateState();
                return;
            }

            this.ValidateState();

            this.RegionName = name;
        }

        private void SetRegionOrientation()
        {
            if (comboBoxStateComboBox.SelectedValue == null)
            {
                this.InvalidateState();
                return;
            }

            var value = (Orientation)comboBoxStateComboBox.SelectedValue;
            if (value < 0)
            {
                this.InvalidateState();
                return;
            }

            this.ValidateState();

            orientation = value;

            this.CalculateRegions();
        }

        private void SetRegionDesignerMode()
        {
            if (comboBoxStateComboBox.SelectedValue == null)
            {
                this.InvalidateState();
                return;
            }

            var value = (DesignerModeType)comboBoxStateComboBox.SelectedValue;
            if (value < 0)
            {
                this.InvalidateState();
                return;
            }

            this.ValidateState();

            designerMode = value;

            this.CalculateRegions();
        }

        private void SetFieldRegion()
        {
            if (imageBox.SelectionRegion.IsEmpty && this.RegionData == null)
            {
                this.InvalidateState();
                return;
            }

            this.ValidateState();

            if (!imageBox.SelectionRegion.IsEmpty)
            {
                fieldsRegion = imageBox.SelectionRegion;
            }


            this.CalculateRegions();
        }

        private void SetTotalFields()
        {
            var value = (int)integerStateValueTextBox.IntegerValue;
            if (value <= 0)
            {
                this.InvalidateState();
                return;
            }

            this.ValidateState();

            totalFields = value;

            this.CalculateRegions();

            drawFields = designerMode == DesignerModeType.DYNAMIC;
            drawInterFieldsSpaces = designerMode == DesignerModeType.DYNAMIC;

            imageBox.Invalidate();
        }

        private void SetInterFieldsSpaceType()
        {
            if (comboBoxStateComboBox.SelectedValue == null)
            {
                this.InvalidateState();
                return;
            }

            var value = (InterSpaceType)comboBoxStateComboBox.SelectedValue;
            if (value < 0)
            {
                this.InvalidateState();
                return;
            }

            this.ValidateState();

            interFieldsSpaceType = value;

            switch (interFieldsSpaceType)
            {
                case InterSpaceType.CONSTANT:
                    break;

                case InterSpaceType.ARRAY:
                    var toAddCount = totalFields - interFieldsSpaces.Count;
                    for (var i = 0; i < toAddCount; i++)
                        interFieldsSpaces.Add(0.1);
                    if (this.RegionData != null)
                    {
                        this.RegionData.InterFieldsSpaces = interFieldsSpaces.ToArray();
                    }

                    break;
            }

            if (this.RegionData != null)
            {
                this.RegionData.InterFieldsSpaceType = interFieldsSpaceType;
            }

            this.CalculateRegions();
        }

        private void SetInterFieldsSpace()
        {
            this.SetInterFieldsSpace(-1);
        }

        private void SetInterFieldsSpace(float _value = -1)
        {
            var value = _value == -1 ? doubleStateValueTextBox.DoubleValue : _value;
            doubleStateValueTextBox.DoubleValue = value;
            var fieldIndex = doubleStateComboBox.SelectedIndex + 1;

            if (value < 0 || fieldIndex <= 0 && interFieldsSpaceType == InterSpaceType.ARRAY)
            {
                this.InvalidateState();
                return;
            }

            this.ValidateState();

            switch (interFieldsSpaceType)
            {
                case InterSpaceType.CONSTANT:
                    interFieldsSpace = value;
                    if (this.RegionData != null)
                    {
                        this.RegionData.InterFieldsSpace = interFieldsSpace;
                    }

                    break;

                case InterSpaceType.ARRAY:
                    interFieldsSpaces[fieldIndex - 1] = value;
                    if (this.RegionData != null)
                    {
                        this.RegionData.InterFieldsSpaces[fieldIndex - 1] = value;
                    }

                    break;
            }

            this.CalculateRegions();
        }

        private void SetInterFieldsComboSpace()
        {
            var curIndex = doubleStateComboBox.SelectedIndex;
            if (curIndex < 0 || this.RegionData == null || this.RegionData.InterFieldsSpaceType != InterSpaceType.ARRAY)
            {
                return;
            }

            var curInterFieldsSpace = this.RegionData.InterFieldsSpaces[curIndex];
            doubleStateValueTextBox.DoubleValue = curInterFieldsSpace;
        }

        private void SetDynamicOptionRegion()
        {
            if (imageBox.SelectionRegion.IsEmpty && this.RegionData == null)
            {
                this.InvalidateState();
                return;
            }

            this.ValidateState();

            if (!imageBox.SelectionRegion.IsEmpty)
            {
                optionsRegion = imageBox.SelectionRegion;
            }

            this.CalculateRegions();
        }

        private void SetFreeOptionRegion()
        {
            if (imageBox.SelectionRegion.IsEmpty && this.RegionData == null)
            {
                this.InvalidateState();
                return;
            }

            this.ValidateState();

            if (!imageBox.SelectionRegion.IsEmpty)
            {
                var selectedField = int.Parse(multiComboStateFirstCombo.SelectedValue.ToString(),
                    CultureInfo.InvariantCulture) - 1;
                var selectedOption = int.Parse(multiComboStateSecondCombo.SelectedValue.ToString(),
                    CultureInfo.InvariantCulture) - 1;
                freeOptionsRegions[selectedField][selectedOption] = imageBox.SelectionRegion;
            }

            this.CalculateRegions();
        }

        private void SetTotalOptions()
        {
            var value = (int)integerStateValueTextBox.IntegerValue;
            if (value <= 0)
            {
                this.InvalidateState();
                return;
            }

            this.ValidateState();

            totalOptions = value;

            if (designerMode == DesignerModeType.FREE)
            {
                freeOptionsRegions = Enumerable.Range(0, totalFields)
                    .Select(x => (Field: x,
                        Options: Enumerable.Repeat(new RectangleF(0, 0, 0, 0), totalOptions).ToList()))
                    .ToDictionary(x => x.Field, x => x.Options);
            }

            this.CalculateRegions();

            drawOptions = true;
            drawInterOptionsSpaces = designerMode == DesignerModeType.DYNAMIC;

            imageBox.Invalidate();
        }

        private void SetInterOptionsSpaceType()
        {
            if (comboBoxStateComboBox.SelectedValue == null)
            {
                this.InvalidateState();
                return;
            }

            var value = (InterSpaceType)comboBoxStateComboBox.SelectedValue;
            if (value < 0)
            {
                this.InvalidateState();
                return;
            }

            this.ValidateState();

            interOptionsSpaceType = value;

            switch (interOptionsSpaceType)
            {
                case InterSpaceType.CONSTANT:
                    break;

                case InterSpaceType.ARRAY:
                    var toAddCount = totalOptions - interOptionsSpaces.Count;
                    for (var i = 0; i < toAddCount; i++)
                        interOptionsSpaces.Add(0.1);
                    if (this.RegionData != null)
                    {
                        this.RegionData.InterOptionsSpaces = interOptionsSpaces.ToArray();
                    }

                    break;
            }

            if (this.RegionData != null)
            {
                this.RegionData.InterOptionsSpaceType = interOptionsSpaceType;
            }

            this.CalculateRegions();
        }

        private void SetInterOptionsSpace()
        {
            this.SetInterOptionsSpace(-1);
        }

        private void SetInterOptionsSpace(float _value = -1)
        {
            var value = _value == -1 ? doubleStateValueTextBox.DoubleValue : _value;
            doubleStateValueTextBox.DoubleValue = value;
            var fieldIndex = doubleStateComboBox.SelectedIndex + 1;

            if (value < 0 || fieldIndex <= 0 && interOptionsSpaceType == InterSpaceType.ARRAY)
            {
                this.InvalidateState();
                return;
            }

            this.ValidateState();

            switch (interOptionsSpaceType)
            {
                case InterSpaceType.CONSTANT:
                    interOptionsSpace = value;
                    if (this.RegionData != null)
                    {
                        this.RegionData.InterOptionsSpace = value;
                    }

                    break;

                case InterSpaceType.ARRAY:
                    interOptionsSpaces[fieldIndex - 1] = value;
                    if (this.RegionData != null)
                    {
                        this.RegionData.InterOptionsSpaces[fieldIndex - 1] = value;
                    }

                    break;
            }

            this.CalculateRegions();
        }

        private void SetInterOptionsComboSpace()
        {
            var curIndex = doubleStateComboBox.SelectedIndex;
            if (curIndex < 0 || this.RegionData == null ||
                this.RegionData.InterOptionsSpaceType != InterSpaceType.ARRAY)
            {
                return;
            }

            var curInterOptionsSpace = this.RegionData.InterOptionsSpaces[curIndex];
            doubleStateValueTextBox.DoubleValue = curInterOptionsSpace;
        }

        private void SetTotalInstances()
        {
            var value = (int)integerStateValueTextBox.IntegerValue;
            if (value <= 0)
            {
                //InvalidateState();
                return;
            }

            this.ValidateState();

            totalInstances = value;
            var diff = totalInstances - instancesOrientations.Count;
            if (diff > 0)
            {
                instancesOrientations.AddRange(Enumerable.Repeat(Orientation.Horizontal, diff).ToList());
            }
            else
            {
                instancesOrientations.RemoveRange(instancesOrientations.Count - Math.Abs(diff), Math.Abs(diff));
            }

            this.CalculateRegions();

            drawInstances = true;
            drawInterInstancesSpaces = true;

            imageBox.Invalidate();
        }

        private void SetInstancesOrientation()
        {
            if (comboValueStateValueComboBox.SelectedValue == null)
            {
                this.InvalidateState();
                return;
            }

            var value = (Orientation)comboValueStateValueComboBox.SelectedValue;
            if (value < 0 || comboValueStateComboBox.SelectedIndex < 0)
            {
                this.InvalidateState();
                return;
            }

            this.ValidateState();

            instancesOrientations[comboValueStateComboBox.SelectedIndex] = value;
            if (this.RegionData != null)
            {
                this.RegionData.InstancesOrientations = instancesOrientations.ToArray();
            }

            this.CalculateRegions();
        }

        private void SetInterInstancesSpaceType()
        {
            if (comboValueStateValueComboBox.SelectedValue == null)
            {
                this.InvalidateState();
                return;
            }

            var value = (InterSpaceType)comboValueStateValueComboBox.SelectedValue;
            if (value < 0)
            {
                this.InvalidateState();
                return;
            }

            this.ValidateState();

            interInstancesSpaceType = value;

            switch (interInstancesSpaceType)
            {
                case InterSpaceType.CONSTANT:
                    break;

                case InterSpaceType.ARRAY:
                    var toAddCount = totalInstances - interInstancesSpaces.Count;
                    for (var i = 0; i < toAddCount; i++)
                        interInstancesSpaces.Add(0.1);
                    if (this.RegionData != null)
                    {
                        this.RegionData.InterInstancesSpaces = interInstancesSpaces.ToArray();
                    }

                    break;
            }

            if (this.RegionData != null)
            {
                this.RegionData.InterInstancesSpaceType = interInstancesSpaceType;
            }

            this.CalculateRegions();
        }

        private void SetInterInstancesSpace()
        {
            this.SetInterInstancesSpace(-1);
        }

        private void SetInterInstancesSpace(float _value = -1)
        {
            var value = _value == -1 ? doubleStateValueTextBox.DoubleValue : _value;
            doubleStateValueTextBox.DoubleValue = value;
            var fieldIndex = doubleStateComboBox.SelectedIndex + 1;

            if (value < 0 || fieldIndex <= 0 && interInstancesSpaceType == InterSpaceType.ARRAY)
            {
                this.InvalidateState();
                return;
            }

            this.ValidateState();

            switch (interInstancesSpaceType)
            {
                case InterSpaceType.CONSTANT:
                    interInstancesSpace = value;
                    if (this.RegionData != null)
                    {
                        this.RegionData.InterInstancesSpace = value;
                    }

                    break;

                case InterSpaceType.ARRAY:
                    interInstancesSpaces[fieldIndex - 1] = value;
                    if (this.RegionData != null)
                    {
                        this.RegionData.InterInstancesSpaces[fieldIndex - 1] = value;
                    }

                    break;
            }

            this.CalculateRegions();
        }

        private void SetInterInstancesComboSpace()
        {
            var curIndex = doubleStateComboBox.SelectedIndex;
            if (curIndex < 0 || this.RegionData == null ||
                this.RegionData.InterInstancesSpaceType != InterSpaceType.ARRAY)
            {
                return;
            }

            var curInterInstancesSpace = this.RegionData.InterInstancesSpaces[curIndex];
            doubleStateValueTextBox.DoubleValue = curInterInstancesSpace;

            this.CalculateRegions();
        }


        private void ReconfigureBtn_Click(object sender, EventArgs e)
        {
            this.SetupForConfiguration();
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
            var configurationState = (ConfigurationState)selectStateComboBox.SelectedIndex;
            this.ConfigWalkthroughState = configurationState;
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