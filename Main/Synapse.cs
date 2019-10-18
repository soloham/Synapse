using Emgu.CV;
using Emgu.CV.Structure;
using Synapse.Core.Configurations;
using Synapse.Core.Engines.Data;
using Synapse.Core.Managers;
using Synapse.Core.Templates;
using Synapse.Modules;
using Synapse.Utilities;
using Synapse.Utilities.Objects;
using Syncfusion.Windows.Forms.Tools;
using Syncfusion.WinForms.DataGrid;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Dynamic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Syncfusion.WinForms.DataGridConverter;
using static Synapse.Core.Configurations.ConfigurationBase;
using static Synapse.Core.Templates.Template;
using Syncfusion.XlsIO;
using Synapse.Controls.Answer_Key;
using Synapse.Core;
using Synapse.Core.Keys;
using System.Linq;
using Synapse.Controls;
using System.Data;
using Synapse.Utilities.Attributes;
using Synapse.Utilities.Enums;
using Syncfusion.Windows.Forms;
using System.Globalization;
using System.Diagnostics;
using System.Reflection;

namespace Synapse
{
    public partial class SynapseMain : RibbonForm
    {
        #region Enums

        public enum StatusState
        {
            Red, Yellow, Green
        }

        public enum Themes
        {
            [EnumDescription("White")]
            WHITE,
            [EnumDescription("Colorful")]
            COLORFUL,
            [EnumDescription("Dark Gray")]
            DARK_GRAY,
            [EnumDescription("Black")]
            BLACK
        }
        #endregion

        #region Objects
        internal class OnTemplateConfig
        {
            internal ConfigurationBase Configuration;
            public RectangleF OffsetRectangle;
            public ColorStates ColorStates;

            internal OnTemplateConfig(ConfigurationBase configurationBase, ColorStates colorStates)
            {
                Configuration = configurationBase;
                ColorStates = colorStates;

                OffsetRectangle = configurationBase.GetConfigArea.ConfigRect;
            }
        }
        #endregion

        #region Properties
        public static SynapseMain GetSynapseMain { get => synapseMain; }
        private static SynapseMain synapseMain;

        internal static Template GetCurrentTemplate { get { return currentTemplate; } set { } }
        private static Template currentTemplate;

        public StatusState TemplateStatus { get { return templateStatus; } set { templateStatus = value; ToggleTemplateStatus(value); } }
        private StatusState templateStatus;
        public StatusState ConfigurationStatus { get { return configurationStatus; } set { configurationStatus = value; ToggleConfigurationDataStatus(value);  } }
        private StatusState configurationStatus;
        public StatusState AIStatus { get { return aiStatus; } set { aiStatus = value; ToggleAIStatus(value); } }
        private StatusState aiStatus;

        internal ProcessedDataRow? SelectedProcessedDataRow { get; set; }
        #endregion

        #region Variables
        private SynchronizationContext synchronizationContext;

        #region BackStage
        public Themes CurrentTheme;
        #endregion
        #region Configuration Panel
        private PointF curImageMouseLoc;

        private List<OnTemplateConfig> OnTemplateConfigs = new List<OnTemplateConfig>();
        private OnTemplateConfig SelectedTemplateConfig { get => selectedTemplateConfig; set { selectedTemplateConfig = value; SelectedTemplateConfigChanged(value); } }
        private OnTemplateConfig selectedTemplateConfig;
        private OnTemplateConfig InterestedTemplateConfig;

        public ColorStates OMRRegionColorStates;
        public ColorStates OBRRegionColorStates;
        public ColorStates ICRRegionColorStates;

        public bool IsMouseOverRegion { get => isMouseOverRegion; set { isMouseOverRegion = value; ToggleMouseOverRegion(value); } }
        private bool isMouseOverRegion;
        public bool IsMouseDownRegion { get => isMouseDownRegion; set { isMouseDownRegion = value; ToggleMouseDownRegion(value); } }
        private bool isMouseDownRegion;
        public bool IsMouseUpRegion { get => isMouseUpRegion; set { isMouseUpRegion = value; ToggleMouseUpRegion(value); } }
        private bool isMouseUpRegion;
        #endregion
        #region Reading Panel
        internal ProcessingManager MainProcessingManager { get; set; }
        public bool GetLocateOptionsToggle { get => locateOptionsToggle; private set { locateOptionsToggle = value; curOptionRects.Clear(); curMarkedOptionRects.Clear(); if (SelectedProcessedDataRow.HasValue) { UpdateImageSelection(SelectedProcessedDataRow.Value, value); } } }
        private bool locateOptionsToggle = false;

        private SheetsList loadedSheetsData = new SheetsList();
        private List<ProcessedDataRow> processedData = new List<ProcessedDataRow>();
        private ObservableCollection<dynamic> processedDataSource = new ObservableCollection<dynamic>();

        private List<string> gridColumns = new List<string>();
        private List<string> gridConfigOnlyColumns = new List<string>();
        private List<string> usedNonCollectiveDataLabels = new List<string>();

        public Dictionary<string, (int entryIndex, int fieldIndex)> GridCellsRepresentation = new Dictionary<string, (int entryIndex, int fieldIndex)>();

        private List<RectangleF> curOptionRects = new List<RectangleF>();
        private List<RectangleF> curMarkedOptionRects = new List<RectangleF>();
        Color primaryRectColor = Color.FromArgb(120, Color.DarkSlateGray);
        Color secondaryRectColor = Color.FromArgb(180, Color.MediumTurquoise);
        #endregion

        NumberFormatInfo NumberFormatInfo;

        Color manualDataCellBackColor = Color.Yellow;
        Color manualDataCellForeColor = Color.Empty;

        Color faultyDataCellBackColor = Color.OrangeRed;
        Color faultyDataCellForeColor = Color.White;

        Color incompatibleDataCellBackColor; 
        Color incompatibleDataCellForeColor;


        Color manualDataRowBackColor = Color.LightGoldenrodYellow;
        Color manualDataRowForeColor = Color.Empty;

        Color faultyDataRowBackColor;
        Color faultyDataRowForeColor;

        Color incompatibleDataRowBackColor = Color.Red;
        Color incompatibleDataRowForeColor = Color.White;
        #endregion

        #region Events 
        public event EventHandler<Mat> OnTemplateLoadedEvent;

        public event EventHandler<StatusState> OnTemplateStateChangedEvent;
        public event EventHandler<StatusState> OnConfigurationDataStateChangedEvent;
        public event EventHandler<StatusState> OnAIStateChangedEvent;
        #endregion

        #region Static Methods
        internal static void RunTemplate(Template template)
        {
            SynapseMain synapseMain = new SynapseMain(template);
            synapseMain.Text = "Synapse - " + template.GetTemplateName;
            synapseMain.Show();
        }
        internal static void UpdateMainStatus(string status)
        {
            GetSynapseMain.UpdateStatus(status);
        }
        #endregion

        #region Internal Methods
        internal SynapseMain(Template currentTemplate)
        {
            InitializeComponent();

            #region SetupComponents
            this.ribbonControl.Height = 220;

            this.templateConfigToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.templateConfigStatusToolStrip,
            this.toolStripSeparator1,
            this.templateToolStripBtn,
            this.toolStripSeparator5,
            this.eMarkingToolStripBtn});
            this.templateConfigToolStrip.Size = new System.Drawing.Size(326, 135);
            this.templateConfigStatusToolStrip.Padding = new Padding(3,20,2,2);

            this.dataConfigToolStripEx.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.dataConfigStatusToolStripPanel,
            this.toolStripSeparator3,
            this.toolStripButton1,
            this.configurationTestToolToolStripBtn,
            this.toolStripSeparator4,
            this.addAsOmrToolStripBtn,
            this.addAsBarcodeToolStripBtn,
            this.addAsICRToolStripBtn});
            this.dataConfigToolStripEx.Size = new System.Drawing.Size(565, 135);
            this.dataConfigStatusToolStripPanel.Padding = new Padding(2, 20, 2, 2);

            this.aiConfigToolStripEx.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aiConfigStatusToolStripPanel,
            this.toolStripSeparator2,
            this.configureNetworksToolStripBtn});
            this.aiConfigToolStripEx.Size = new System.Drawing.Size(202, 135);
            this.aiConfigStatusToolStripPanel.Padding = new Padding(2, 20, 2, 2);


            this.generalToolStripEx.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.answerKeyToolStripBtn,
            this.papersToolStripBtn});
            this.generalToolStripEx.Size = new System.Drawing.Size(186, 135);

            this.sheetsToolStripEx.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sheetsToolStripPanelItem,
            this.scanSheetsToolStripDropDownBtn,
            this.toolStripSeparator7});
            this.sheetsToolStripEx.Size = new System.Drawing.Size(291, 135);

            this.processingToolStripEx.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.startReadingToolStripBtn,
            this.stopReadingToolStripBtn});
            this.processingToolStripEx.Size = new System.Drawing.Size(146, 135);

            this.postOperationsToolStripEx.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.reReadFaultySheetsToolStripBtn,
            this.moveFaultySheetsToolStripBtn,
            this.locateOptionsToolStripBtn});
            this.postOperationsToolStripEx.Size = new System.Drawing.Size(280, 135);

            this.dataMiningToolStripEx.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.findDuplicatesToolStripBtn});
            this.dataMiningToolStripEx.Size = new System.Drawing.Size(101, 135);


            this.dataManipulationToolStripEx.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addFieldToolStripBtn,
            this.toolStripSeparator6,
            this.deleteAllToolStripBtn,
            this.deleteSelectedToolStripBtn,
            this.toolStripSeparator8,
            this.editValueToolStripBtn,
            this.markAsToolStripBtn});
            this.dataManipulationToolStripEx.Size = new System.Drawing.Size(423, 135);

            this.dataPointStorageToolStripEx.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.internalDataPointDropDownBtn,
            this.externalDataPointDropDownBtn});
            this.dataPointStorageToolStripEx.Size = new System.Drawing.Size(211, 135);

            this.exportToolStripEx.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exportCSVoolStripBtn,
            this.exportExcelToolStripBtn,
            this.SQLDatabaseExportToolStripBtn});
            this.exportToolStripEx.Size = new System.Drawing.Size(238, 135);
            #endregion
            synchronizationContext = SynchronizationContext.Current;
            synapseMain = this;

            SynapseMain.currentTemplate = currentTemplate;
            Awake();
        }

        #region Reading Tab
        internal void InitializeDataGrids(List<ProcessedDataEntry> processedDataEntries, (ObservableCollection<dynamic> processedDataSource, ObservableCollection<dynamic> manProcessedDataSource, ObservableCollection<dynamic> fauProcessedDataSource, ObservableCollection<dynamic> incProcessedDataSource) sources, int extraCols)
        {
            if (this.gridColumns.Count > 0)
            {
                var gridColumns = mainDataGrid.Columns;
                int dataDynamicTotalColumns = 0;
                for (int i = 0; i < processedDataEntries.Count; i++)
                {
                    dataDynamicTotalColumns += processedDataEntries[i].GetDataValues.Length;
                }
                if (gridColumns.Count != dataDynamicTotalColumns + extraCols)
                {

                }
                else
                {
                    bool areValid = false;
                    for (int i = 0; i < gridColumns.Count; i++)
                    {
                        areValid = gridColumns[i].HeaderText == this.gridColumns[i];
                    }

                    if (areValid)
                    {
                        #region DataDynamicColumnGeneration
                        //mainDataGrid.AutoGenerateColumns = false;
                        //mainDataGrid.Columns.Clear();
                        //for (int i3 = 0; i3 < processedDataEntries.Count; i3++)
                        //{
                        //    GridTextColumn col1 = new GridTextColumn();
                        //    col1.MappingName = processedDataEntries[i3].GetConfigurationBase.Title;
                        //    col1.HeaderText = processedDataEntries[i3].GetConfigurationBase.Title;
                        //    mainDataGrid.Columns.Add(col1);
                        //}
                        #endregion
                        mainDataGridPager.DataSource = sources.processedDataSource;
                        manualDataGridPager.DataSource = sources.manProcessedDataSource;
                        faultyDataGridPager.DataSource = sources.fauProcessedDataSource;
                        incompatibleDataGridPager.DataSource = sources.incProcessedDataSource;
                    }
                    else
                    {

                    }
                    #region TableSummary
                    //mainDataGrid.TableSummaryRows.Clear();
                    //GridTableSummaryRow tableSummaryRow = new GridTableSummaryRow();
                    //tableSummaryRow.Name = "TotalSheetsRow";
                    //tableSummaryRow.ShowSummaryInRow = true;
                    //tableSummaryRow.Title = "Total Processed Sheets: {TotalSheets}";
                    //tableSummaryRow.Position = Syncfusion.WinForms.DataGrid.Enums.VerticalPosition.Bottom;

                    //GridSummaryColumn summaryColumn = new GridSummaryColumn();
                    //summaryColumn.Name = "TotalSheets";
                    //summaryColumn.SummaryType = Syncfusion.Data.SummaryType.CountAggregate;
                    //summaryColumn.Format = "{Count}";
                    //summaryColumn.MappingName = mainDataGrid.Columns[1].MappingName;

                    //tableSummaryRow.SummaryColumns.Add(summaryColumn);
                    //mainDataGrid.TableSummaryRows.Add(tableSummaryRow);
                    #endregion
                }
            }
            else
            {
                #region DataDynamicColumnGeneration
                mainDataGrid.AutoGenerateColumns = false;
                mainDataGrid.Columns.Clear();
                for (int i3 = 0; i3 < processedDataEntries.Count; i3++)
                {
                    GridTextColumn col1 = new GridTextColumn();
                    col1.MappingName = processedDataEntries[i3].GetConfigurationBase.Title;
                    col1.HeaderText = processedDataEntries[i3].GetConfigurationBase.Title;
                    mainDataGrid.Columns.Add(col1);
                }
                #endregion
                mainDataGridPager.DataSource = sources.processedDataSource;
                manualDataGridPager.DataSource = sources.manProcessedDataSource;
                faultyDataGridPager.DataSource = sources.fauProcessedDataSource;
                incompatibleDataGridPager.DataSource = sources.incProcessedDataSource;
            }
            mainDataGrid.DataSource = mainDataGridPager.PagedSource;
            manualDataGrid.DataSource = manualDataGridPager.PagedSource;
            faultyDataGrid.DataSource = faultyDataGridPager.PagedSource;
            incompatibleDataGrid.DataSource = incompatibleDataGridPager.PagedSource;
        }
        #endregion

        #endregion

        #region Public Methods
        #region Configuration Tab
        public void AddRegionAsOMR(RectangleF region)
        {
            RectangleF configAreaRect = region;
            ConfigArea configArea = new ConfigArea(configAreaRect, (Bitmap)templateImageBox.GetSelectedImage());
            OMRConfigurationForm configurationForm = new OMRConfigurationForm(configArea);
            configurationForm.OnConfigurationFinishedEvent += async (string name, Orientation orientation, OMRRegionData regionData) =>
            {
                bool isSaved = false;
                Exception ex = new Exception();

                OMRConfiguration omrConfig = null;
                await Task.Run(() =>
                {
                    omrConfig = OMRConfiguration.CreateDefault(name, orientation, configArea, regionData, ConfigurationsManager.GetAllConfigurations.Count);
                    isSaved = OMRConfiguration.Save(omrConfig, out ex);
                });

                if (isSaved)
                {
                    configurationForm.Close();

                    ConfigurationsManager.AddConfiguration(omrConfig);
                    CalculateTemplateConfigs();
                }
                else
                    Messages.SaveFileException(ex);
            };
            configurationForm.OnFormInitializedEvent += (object sender, EventArgs args) =>
            {

            };
            configurationForm.ShowDialog();
        }
        public void AddRegionAsBarcode(RectangleF region)
        {
            RectangleF configAreaRect = region;
            ConfigArea configArea = new ConfigArea(configAreaRect, (Bitmap)templateImageBox.GetSelectedImage());
            BarcodeConfigurationForm configurationForm = new BarcodeConfigurationForm(configArea.ConfigBitmap);
            configurationForm.OnConfigurationFinishedEvent += async (string name) =>
            {
                bool isSaved = false;
                Exception ex = new Exception();

                OBRConfiguration barcodeConfig = null;
                await Task.Run(() =>
                {
                    barcodeConfig = OBRConfiguration.CreateDefault(name, configArea, ConfigurationsManager.GetAllConfigurations.Count);
                    isSaved = OBRConfiguration.Save(barcodeConfig, out ex);
                });

                if (isSaved)
                {
                    configurationForm.Close();

                    ConfigurationsManager.AddConfiguration(barcodeConfig);
                    CalculateTemplateConfigs();
                }
                else
                    Messages.SaveFileException(ex);
            };
            configurationForm.OnFormInitializedEvent += (object sender, EventArgs args) =>
            {

            };
            configurationForm.ShowDialog();
        }
        public void AddRegionAsICR(RectangleF region)
        {
            RectangleF configAreaRect = region;
            ConfigArea configArea = new ConfigArea(configAreaRect, (Bitmap)templateImageBox.GetSelectedImage());
            ICRConfigurationForm configurationForm = new ICRConfigurationForm(configArea.ConfigBitmap);
            configurationForm.OnConfigurationFinishedEvent += async (string name) =>
            {
                bool isSaved = false;
                Exception ex = new Exception();

                ICRConfiguration icrConfig = null;
                await Task.Run(() =>
                {
                    icrConfig = ICRConfiguration.CreateDefault(name, configArea, ConfigurationsManager.GetAllConfigurations.Count);
                    isSaved = ICRConfiguration.Save(icrConfig, out ex);
                });

                if (isSaved)
                {
                    configurationForm.Close();

                    ConfigurationsManager.AddConfiguration(icrConfig);
                    CalculateTemplateConfigs();
                }
                else
                    Messages.SaveFileException(ex);
            };
            configurationForm.OnFormInitializedEvent += (object sender, EventArgs args) =>
            {

            };
            configurationForm.ShowDialog();
        }

        public void StatusCheck()
        {
            //Template Status
            StatusState templateStatus = StatusState.Red;
            if (GetCurrentTemplate.GetTemplateImage != null && File.Exists(GetCurrentTemplate.GetTemplateImage.ImageLocation))
                templateStatus = StatusState.Yellow;
            if (GetCurrentTemplate.TemplateData.GetAlignmentPipeline != null && GetCurrentTemplate.TemplateData.GetAlignmentPipeline.Count > 0)
            {
                if (templateStatus == StatusState.Yellow) templateStatus = StatusState.Green;
            }
            TemplateStatus = templateStatus;

            //Configuration Status
            StatusState configStatus = StatusState.Red;
            if (ConfigurationsManager.GetAllConfigurations.Count > 0)
            {
                configStatus = StatusState.Green;
            }
            ConfigurationStatus = configStatus;

            //AI Status
        }
        public void ToggleTemplateStatus(StatusState status)
        {
            templateConfigStatusIndicator.Image = status == StatusState.Green? Properties.Resources.StatusOk : status == StatusState.Yellow ? Properties.Resources.StatusInOk : Properties.Resources.StatusNotOk;

            switch (status)
            {
                case StatusState.Red:
                    configurationTestToolToolStripBtn.Enabled = false;
                    addAsOmrToolStripBtn.Enabled = false;
                    addAsBarcodeToolStripBtn.Enabled = false;
                    addAsICRToolStripBtn.Enabled = false;

                    configureNetworksToolStripBtn.Enabled = false;
                    break;
                case StatusState.Yellow:
                    break;
                case StatusState.Green:
                    configurationTestToolToolStripBtn.Enabled = true;
                    addAsOmrToolStripBtn.Enabled = true;
                    addAsBarcodeToolStripBtn.Enabled = true;
                    addAsICRToolStripBtn.Enabled = true;

                    configureNetworksToolStripBtn.Enabled = true;
                    break;
            }

            OnTemplateStateChangedEvent?.Invoke(this, status);
        }
        public void ToggleConfigurationDataStatus(StatusState status)
        {
            dataConfigStatusIndicator.Image = status == StatusState.Green ? Properties.Resources.StatusOk : status == StatusState.Yellow ? Properties.Resources.StatusInOk : Properties.Resources.StatusNotOk;

            switch (status)
            {
                case StatusState.Red:
                    break;
                case StatusState.Yellow:
                    break;
                case StatusState.Green:
                    break;
            }

            OnConfigurationDataStateChangedEvent?.Invoke(this, status);
        }
        public void ToggleAIStatus(StatusState status)
        {
            aiConfigStatusIndicator.Image = status == StatusState.Green ? Properties.Resources.StatusOk : status == StatusState.Yellow ? Properties.Resources.StatusInOk : Properties.Resources.StatusNotOk;

            switch (status)
            {
                case StatusState.Red:
                    break;
                case StatusState.Yellow:
                    break;
                case StatusState.Green:
                    break;
            }

            OnAIStateChangedEvent?.Invoke(this, status);
        }

        internal Bitmap GetCurrentImage()
        {
            return (Bitmap)templateImageBox.Image;
        }
        internal void SetCurrentImage(Bitmap bitmap)
        {
            templateImageBox.Image = bitmap;
        }

        public void ToggleMouseOverRegion(bool isOver)
        {
            if (isOver)
            {
                Cursor.Current = Cursors.Hand;
            }
            else
            {
                Cursor.Current = Cursors.Arrow;
            }
        }
        public void ToggleMouseDownRegion(bool isDown)
        {

        }
        public void ToggleMouseUpRegion(bool isUp)
        {

        }
        #endregion
        #region Reading Tab
        public List<string> GetGridDataColumns(bool configOnly = false)
        {
            if (!configOnly)
            {
                if (gridColumns != null && gridColumns.Count > 0)
                    return new List<string>(gridColumns);
                else
                {
                    GenerateGridColumns();
                    return new List<string>(gridColumns);
                }
            }
            else
            {
                if (gridConfigOnlyColumns != null && gridConfigOnlyColumns.Count > 0)
                    return new List<string>(gridConfigOnlyColumns);
                else
                {
                    GenerateGridColumns();
                    return new List<string>(gridConfigOnlyColumns);
                }
            }
        }
        #endregion
        #region Data Tab
        #endregion


        public void UpdateStatus(string status)
        {
            try
            {
                synchronizationContext.Send(new SendOrPostCallback((object target) =>
                {
                    statusPanelStatusLabel.Text = status;
                }), null);
            }
            catch (Exception ex)
            {

            }
        }
        #endregion

        #region Private Methods
        #region Main
        private async void Awake()
        {
            #region BackStage
            bsSettingsThemeField.DataSource = EnumHelper.ToList(typeof(Themes));
            bsSettingsThemeField.DisplayMember = "Value";
            bsSettingsThemeField.ValueMember = "Key";
            #endregion

            MainProcessingManager = new ProcessingManager(GetCurrentTemplate);
            MainProcessingManager.OnSheetProcessed += OnSheetsProcessed;
            MainProcessingManager.OnDataSourceUpdated += MainProcessingManager_OnDataSourceUpdated;
            MainProcessingManager.OnProcessingComplete += MainProcessingManager_OnProcessingComplete;

            //Pre-Ops
            //-User Interface Setup
            //--Ribbon Tabs Setup
            readingTabPanel.Dock = DockStyle.Fill;
            configTabPanel.Dock = DockStyle.Fill;
            ribbonControl.SelectedTab = configToolStripTabItem;

            sheetsToolStripPanelItem.Height = 80;

            mainDockingManager.SetEnableDocking(configPropertiesPanel, true);
            mainDockingManager.DockControlInAutoHideMode(configPropertiesPanel, DockingStyle.Right, 300);
            mainDockingManager.SetMenuButtonVisibility(configPropertiesPanel, false);
            mainDockingManager.SetDockLabel(configPropertiesPanel, "Properties");

            mainDockingManager.SetEnableDocking(dataImageBoxPanel, true);
            mainDockingManager.DockControlInAutoHideMode(dataImageBoxPanel, DockingStyle.Right, 300);
            mainDockingManager.SetMenuButtonVisibility(dataImageBoxPanel, false);
            mainDockingManager.SetDockLabel(dataImageBoxPanel, "Image");
            if (!readingToolStripTabItem.Checked)
                mainDockingManager.SetDockVisibility(dataImageBoxPanel, false);

            mainDockingManager.SetEnableDocking(answerKeyPanel, true);
            mainDockingManager.SetMenuButtonVisibility(answerKeyPanel, false);
            mainDockingManager.SetAutoHideButtonVisibility(answerKeyPanel, false);
            mainDockingManager.SetCloseButtonVisibility(answerKeyPanel, true);
            mainDockingManager.SetDockLabel(answerKeyPanel, "Answer Key");
            mainDockingManager.SetDockVisibility(answerKeyPanel, false);

            readingTabManualTablePanel.Visible = false;
            readingTabFaultyTablePanel.Visible = false;
            readingTabIncompatibleTablePanel.Visible = false;
            
            mainDockingManager.SetEnableDocking(readingTabManualTablePanel, true);
            mainDockingManager.DockControlInAutoHideMode(readingTabManualTablePanel, DockingStyle.Bottom, 200);
            mainDockingManager.SetDockLabel(readingTabManualTablePanel, "MANUAL DATA");

            mainDockingManager.SetEnableDocking(readingTabFaultyTablePanel, true);
            mainDockingManager.DockControlInAutoHideMode(readingTabFaultyTablePanel, DockingStyle.Bottom, 200);
            mainDockingManager.SetDockLabel(readingTabFaultyTablePanel, "FAULTY DATA");

            mainDockingManager.SetEnableDocking(readingTabIncompatibleTablePanel, true);
            mainDockingManager.DockControlInAutoHideMode(readingTabIncompatibleTablePanel, DockingStyle.Bottom, 200);
            mainDockingManager.SetDockLabel(readingTabIncompatibleTablePanel, "INCOMPATIBLE DATA");

            readingTabMainTablePanel.Dock = DockStyle.Fill;

            mainDockingManager.AnimateAutoHiddenWindow = false;

            OMRRegionColorStates = new ColorStates(Color.FromArgb(55, Color.Firebrick), Color.FromArgb(95, Color.Firebrick), Color.FromArgb(85, Color.Firebrick), Color.FromArgb(110, Color.Firebrick));
            OBRRegionColorStates = new ColorStates(Color.FromArgb(55, Color.Black), Color.FromArgb(95, Color.Black), Color.FromArgb(85, Color.Black), Color.FromArgb(110, Color.Black));
            ICRRegionColorStates = new ColorStates(Color.FromArgb(55, Color.SlateGray), Color.FromArgb(95, Color.SlateGray), Color.FromArgb(85, Color.SlateGray), Color.FromArgb(110, Color.SlateGray));

            await GeneralManager.Initialize();
            await ConfigurationsManager.Initialize();
            ConfigurationsManager.OnConfigurationDeletedEvent += ConfigurationsManager_OnConfigurationDeletedEvent;
            CalculateTemplateConfigs();

            OnTemplateLoadedEvent += SynapseMain_OnTemplateLoadedEvent;

            StatusCheck();

            if (GetCurrentTemplate.GetTemplateImage != null && !string.IsNullOrEmpty(GetCurrentTemplate.GetTemplateImage.ImageLocation))
            {
                try
                {
                    //byte[] data = File.ReadAllBytes(GetCurrentTemplate.GetTemplateImage.ImageLocation);
                    //// Read in the data but do not close, before using the stream.
                    //Stream originalBinaryDataStream = new MemoryStream(data);
                    //Image tmpImage = Image.FromStream(originalBinaryDataStream);

                    var tmpImage = GetCurrentTemplate.GetTemplateImage.GetGrayImage.Mat;
                    templateImageBox.Image = tmpImage.Bitmap;
                    templateImageBox.ZoomToFit();

                    OnTemplateLoadedEvent?.Invoke(this, tmpImage);
                }
                catch (Exception ex)
                {
                    Messages.LoadFileException(ex);
                }
            }
            
            mainDataGrid.Style.ProgressBarStyle.ForegroundStyle = Syncfusion.WinForms.DataGrid.Enums.GridProgressBarStyle.Gradient;
            mainDataGrid.Style.ProgressBarStyle.GradientForegroundStartColor = Color.DeepSkyBlue;
            mainDataGrid.Style.ProgressBarStyle.GradientForegroundEndColor = Color.DodgerBlue;

            manualDataGrid.Style.ProgressBarStyle.ForegroundStyle = Syncfusion.WinForms.DataGrid.Enums.GridProgressBarStyle.Gradient;
            manualDataGrid.Style.ProgressBarStyle.GradientForegroundStartColor = Color.DeepSkyBlue;
            manualDataGrid.Style.ProgressBarStyle.GradientForegroundEndColor = Color.DodgerBlue;

            faultyDataGrid.Style.ProgressBarStyle.ForegroundStyle = Syncfusion.WinForms.DataGrid.Enums.GridProgressBarStyle.Gradient;
            faultyDataGrid.Style.ProgressBarStyle.GradientForegroundStartColor = Color.DeepSkyBlue;
            faultyDataGrid.Style.ProgressBarStyle.GradientForegroundEndColor = Color.DodgerBlue;

            incompatibleDataGrid.Style.ProgressBarStyle.ForegroundStyle = Syncfusion.WinForms.DataGrid.Enums.GridProgressBarStyle.Gradient;
            incompatibleDataGrid.Style.ProgressBarStyle.GradientForegroundStartColor = Color.DeepSkyBlue;
            incompatibleDataGrid.Style.ProgressBarStyle.GradientForegroundEndColor = Color.DodgerBlue;

            NumberFormatInfo = new NumberFormatInfo();
            NumberFormatInfo.NumberDecimalDigits = 0;

            if(currentTemplate.TemplateData.IsActivated)
                exportExcelToolStripBtn.Enabled = true;
        }

        double finalAverage = 0;
        double spsAvg = 0;
        TimeSpan totalTimeTaken = TimeSpan.Zero;
        int totalSheets = 0;
        void OnSheetsProcessed(object sender, (ProcessedDataRow dataRow, double runningAverage, double runningTotal) args)
        {
            int curIndex = MainProcessingManager.GetCurProcessingIndex + 1;
            try
            {
                int progressValue = (int)(curIndex / (float)totalSheets * 100);
                processingProgressBar.Value = progressValue;
            }
            catch (Exception ex)
            {

            }
            if (processingProgressBar.Value > 47)
                processingProgressBar.FontColor = Color.White;

            try
            {
                string timeLeft = TimeSpan.FromMilliseconds(args.runningAverage * (totalSheets - curIndex)).ToString(@"hh\:mm\:ss");
                processingTimeLeftLabel.Text = "ESTIMATED TIME: " + timeLeft;
            }
            catch (Exception ex)
            {

            }
            
            try
            {
                string status = "";
                spsAvg = 1 / (args.runningAverage / 1000);
                if (MainProcessingManager.IsPaused)
                {
                    status = $"[{MainProcessingManager.GetCurProcessingIndex + 1}/{totalSheets}] Processing Paused  |  AVG: {Math.Round(spsAvg, 1)} Sheets per second.";
                    statusPanelStatusLabel.Text = status;
                }
                else
                {
                    status = $"[{curIndex}/{totalSheets}] Processing...  |   AVG: {Math.Round(spsAvg, 1)} Sheets per second.";
                }
                statusPanelStatusLabel.Text = status;
            }
            catch (Exception ex)
            {

            }

            if (curIndex == totalSheets)
            {
                totalTimeTaken = TimeSpan.FromMilliseconds(args.runningTotal);
                finalAverage = args.runningAverage / 1000;
                finalAverage = Math.Round(finalAverage, 2);
            }
        }
        void MainProcessingManager_OnProcessingComplete(object sender, (bool cancelled, object result, Exception error) args)
        {
            stopReadingToolStripBtn.Enabled = false;
            startReadingToolStripBtn.Text = "Start";
            startReadingToolStripBtn.Image = Properties.Resources.startBtnIcon_ReadingTab;

            progressStatusTablePanel.Visible = false;

            processingTimeLeftLabel.Text = "TOTAL TIME: 00:00:00";
            processingProgressBar.Value = 0;

            if(args.cancelled)
                UpdateStatus($"Processing Stopped: {MainProcessingManager.GetCurProcessingIndex + 1} sheets in {totalTimeTaken.ToString(@"hh\:mm\:ss")} at an average of {finalAverage} seconds per sheet.");
            else
                UpdateStatus($"Processing Complete: {MainProcessingManager.GetCurProcessingIndex + 1} sheets in {totalTimeTaken.ToString(@"hh\:mm\:ss")} at an average of {finalAverage} seconds per sheet.");
        }
        private void MainProcessingManager_OnDataSourceUpdated(object sender, ProcessedDataType e)
        {
            SfDataGrid dataGrid = mainDataGrid;
            int rowCount = 0;

            rowCount = MainProcessingManager.GetTotalProcessedData;
            if (rowCount == 0)
                return;

            if (!normalDataTypePanel.Visible)
                normalDataTypePanel.Visible = true;
            totalMainDataLabel.Text = rowCount.ToString("N0", NumberFormatInfo);

            switch (e)
            {
                case ProcessedDataType.INCOMPATIBLE:
                    //dataGrid = incompatibleDataGrid;
                    rowCount = MainProcessingManager.GetTotalIncompatibleProcessedData;
                    if (rowCount == 0)
                        return;

                    if (!incompatibleDataStatusPanel.Visible)
                        incompatibleDataStatusPanel.Visible = true;
                    totalIncompatibleDataLabel.Text = rowCount.ToString("N0", NumberFormatInfo);
                    break;
                case ProcessedDataType.FAULTY:
                    //dataGrid = faultyDataGrid;
                    rowCount = MainProcessingManager.GetTotalFaultyProcessedData;
                    if (rowCount == 0)
                        return;

                    if (!faultyDataTypeStatusPanel.Visible)
                        faultyDataTypeStatusPanel.Visible = true;
                    totalFaultyDataLabel.Text = rowCount.ToString("N0", NumberFormatInfo);
                    break;
                case ProcessedDataType.MANUAL:
                    //dataGrid = manualDataGrid;
                    rowCount = MainProcessingManager.GetTotalManualProcessedData;
                    if (rowCount == 0)
                        return;

                    if (!manualDataTypeStatusPanel.Visible)
                        manualDataTypeStatusPanel.Visible = true;
                    totalManualDataLabel.Text = rowCount.ToString("N0", NumberFormatInfo);
                    break;
                case ProcessedDataType.NORMAL:
                    
                    break;
            }
        }

        #region Configuration Panel
        private void SynapseMain_OnTemplateLoadedEvent(object sender, Mat e)
        {
            templateConfigureToolStripMenuItem.Enabled = true;

            if(TemplateStatus == StatusState.Red)
                TemplateStatus = StatusState.Yellow;
            else if(TemplateStatus == StatusState.Green)
                templateLoadToolStripMenuItem.Enabled = false;
        }

        private void ConfigurationsManager_OnConfigurationDeletedEvent(object sender, ConfigurationBase e)
        {
            CalculateTemplateConfigs();
            templateImageBox.Invalidate(); 
        }

        private void DrawConfiguration(OnTemplateConfig onTemplateConfig, Graphics g)
        {
            ConfigArea configArea = onTemplateConfig.Configuration.GetConfigArea;
            MainConfigType mainConfigType = onTemplateConfig.Configuration.GetMainConfigType;

            ColorStates colorStates = onTemplateConfig.ColorStates;

            GraphicsState originalState;
            RectangleF curDrawFieldRectF = templateImageBox.GetOffsetRectangle(configArea.ConfigRect);
            onTemplateConfig.OffsetRectangle = curDrawFieldRectF;

            originalState = g.Save();

            switch (mainConfigType)
            {
                case MainConfigType.OMR:
                    Utilities.Functions.DrawBox(g, curDrawFieldRectF, templateImageBox.ZoomFactor, colorStates.CurrentColor, 0);
                    break;
                case MainConfigType.BARCODE:
                    Utilities.Functions.DrawBox(g, curDrawFieldRectF, templateImageBox.ZoomFactor, colorStates.CurrentColor, 0);
                    break;
                case MainConfigType.ICR:
                    //if (configArea.ConfigRect.Contains(curImageMouseLoc))
                    Utilities.Functions.DrawBox(g, curDrawFieldRectF, templateImageBox.ZoomFactor, colorStates.CurrentColor, 0);
                    break;
            }

            g.Restore(originalState);
        }
        private void CalculateTemplateConfigs()
        {
            SelectedTemplateConfig = null;

            OnTemplateConfigs = new List<OnTemplateConfig>();
            var allConfigs = ConfigurationsManager.GetAllConfigurations;
            for (int i = 0; i < allConfigs.Count; i++)
            {
                ColorStates colorStates = null;
                switch (allConfigs[i].GetMainConfigType)
                {
                    case MainConfigType.OMR:
                        colorStates = ColorStates.Copy(OMRRegionColorStates);
                        break;
                    case MainConfigType.BARCODE:
                        colorStates = ColorStates.Copy(OBRRegionColorStates);
                        break;
                    case MainConfigType.ICR:
                        colorStates = ColorStates.Copy(ICRRegionColorStates);
                        break;
                }

                OnTemplateConfig onTemplateConfig = new OnTemplateConfig(allConfigs[i], colorStates);
                OnTemplateConfigs.Add(onTemplateConfig);
            }
        }
        private void SelectedTemplateConfigChanged(OnTemplateConfig selectedTemplate)
        {
            configPropertyEditor.PropertyGrid.SelectedObject = selectedTemplate == null? null : selectedTemplate.Configuration?? null;
            if (configPropertyEditor.PropertyGrid.SelectedObject != null)
            {
                if (mainDockingManager.GetState(configPropertiesPanel) == DockState.Hidden || mainDockingManager.GetState(configPropertiesPanel) == DockState.AutoHidden)
                {
                    mainDockingManager.SetAutoHideMode(configPropertiesPanel, false);
                    mainDockingManager.DockControl(configPropertiesPanel, this, DockingStyle.Right, 400);
                }
            }
            else
                mainDockingManager.DockControlInAutoHideMode(configPropertiesPanel, DockingStyle.Right, 400);
        }

        private async void TemplateConfigurationForm_OnConfigurationFinishedEvent(TemplateConfigurationForm sender, Template.TemplateImage templateImage, List<Template.AlignmentMethod> alignmentMethods, Template.AlignmentPipelineResults alignmentPipelineResults)
        {
            if (templateImage.Size != Size.Empty && alignmentMethods.Count > 0)
            {
                templateImageBox.Image = templateImage.GetBitmap;
                GetCurrentTemplate.SetTemplateImage(templateImage);
                GetCurrentTemplate.SetAlignmentPipeline(alignmentMethods);

                bool isSaved = await Task.Run(() => Template.SaveTemplate(GetCurrentTemplate.TemplateData, string.IsNullOrEmpty(GetCurrentTemplate.GetTemplateImage.ImageLocation)));

                if (isSaved)
                {
                    TemplateStatus = StatusState.Green;
                    templateLoadToolStripMenuItem.Enabled = false;
                }
            }
            else
            {
                Messages.ShowError("Invalid template configuration.");
            }
        }
        #endregion
        #region Reading Panel
        public async Task<bool> InitializeSheetsToRead(string path, bool incSubDirs)
        {
            string error = "";
            bool success = false;

            loadedSheetsData = new SheetsList();
            success = await Task.Run(() => loadedSheetsData.Scan(path, incSubDirs, out error));

            //err = error;
            return success;
        }
        private void StartReadingToolStripBtn_Click(object sender, EventArgs e)
        {
            if (MainProcessingManager.IsProcessing && !MainProcessingManager.IsPaused)
            {
                MainProcessingManager.PauseProcessing();

                processingToolStripEx.Width = 161;
                startReadingToolStripBtn.Text = "Resume";
                startReadingToolStripBtn.Image = Properties.Resources.startBtnIcon_ReadingTab;

                string status = $"[{MainProcessingManager.GetCurProcessingIndex + 1}/{totalSheets}] Processing Paused   |  AVG: {Math.Round(spsAvg, 1)} Sheets per second.";
                statusPanelStatusLabel.Text = status;
            }
            else
            {
                processingToolStripEx.Width = 146;
                startReadingToolStripBtn.Text = "Pause";
                startReadingToolStripBtn.Image = Properties.Resources.PauseBtnIcon_ReadingTab;

                if (MainProcessingManager.IsPaused)
                {
                    MainProcessingManager.ResumeProcessing();
                    return;
                }

                if (!loadedSheetsData.SheetsLoaded)
                {
                    Messages.ShowError("Unable to start procesing as there are no sheets loaded for processing. \n\n Please load sheets in order to start processing");
                    return;
                }

                bool keepData = false;
                if (MainProcessingManager.DataExists())
                    keepData = Messages.ShowQuestion("Would you like to keep the current processed data?") == DialogResult.Yes;

                GenerateGridColumns();
                MainProcessingManager.LoadSheets(loadedSheetsData);

                totalSheets = loadedSheetsData.GetSheetsPath.Length;

                processingProgressBar.FontColor = CurrentTheme == Themes.COLORFUL || CurrentTheme == Themes.WHITE ? Color.Black : Color.WhiteSmoke;

                progressStatusTablePanel.Visible = true;
                MainProcessingManager.StartProcessing(keepData, gridConfigOnlyColumns);

                stopReadingToolStripBtn.Enabled = true;
            }
        }

        private void GenerateGridColumns()
        {
            gridColumns.Clear();
            gridConfigOnlyColumns.Clear();
            GridCellsRepresentation.Clear();
            usedNonCollectiveDataLabels.Clear();
            mainDataGrid.AutoGenerateColumns = false;
            mainDataGrid.Columns.Clear();
            manualDataGrid.AutoGenerateColumns = false;
            manualDataGrid.Columns.Clear();
            faultyDataGrid.AutoGenerateColumns = false;
            faultyDataGrid.Columns.Clear();
            incompatibleDataGrid.AutoGenerateColumns = false;
            incompatibleDataGrid.Columns.Clear();

            var allConfigs = ConfigurationsManager.GetAllConfigurations;
            for (int i = 0; i < allConfigs.Count; i++)
            {
                switch (allConfigs[i].GetMainConfigType)
                {
                    case MainConfigType.OMR:
                        OMRConfiguration omrConfiguration = (OMRConfiguration)allConfigs[i];

                        switch (omrConfiguration.ValueRepresentation)
                        {
                            case ValueRepresentation.Collective:
                                GridTextColumn omrCol = new GridTextColumn();
                                omrCol.MappingName = omrConfiguration.Title;
                                omrCol.HeaderText = omrConfiguration.Title;
                                mainDataGrid.Columns.Add(omrCol);
                                manualDataGrid.Columns.Add(omrCol);
                                faultyDataGrid.Columns.Add(omrCol);
                                incompatibleDataGrid.Columns.Add(omrCol);

                                gridColumns.Add(omrCol.HeaderText);
                                gridConfigOnlyColumns.Add(omrCol.HeaderText);
                                GridCellsRepresentation.Add(omrCol.HeaderText, (i, 0));
                                break;
                            case ValueRepresentation.Indiviual:
                                int totalIndFields = omrConfiguration.GetTotalFields;
                                string indConfigTitle = omrConfiguration.Title;
                                string indDataLabel = indConfigTitle[0] + "";
                                while (usedNonCollectiveDataLabels.Contains(indDataLabel))
                                {
                                    if (indDataLabel.Length == indConfigTitle.Length)
                                        indDataLabel = indConfigTitle + "_";
                                    else
                                        indDataLabel += indConfigTitle[indDataLabel.Length] + "";
                                }
                                usedNonCollectiveDataLabels.Add(indDataLabel);
                                for (int i1 = 0; i1 < totalIndFields; i1++)
                                {
                                    GridTextColumn omrIndCol = new GridTextColumn();
                                    omrIndCol.MappingName = indDataLabel + (i1+1).ToString();
                                    omrIndCol.HeaderText = omrIndCol.MappingName;
                                    mainDataGrid.Columns.Add(omrIndCol);
                                    manualDataGrid.Columns.Add(omrIndCol);
                                    faultyDataGrid.Columns.Add(omrIndCol);
                                    incompatibleDataGrid.Columns.Add(omrIndCol);

                                    gridColumns.Add(omrIndCol.HeaderText);
                                    gridConfigOnlyColumns.Add(omrIndCol.HeaderText);
                                    GridCellsRepresentation.Add(omrIndCol.HeaderText, (i, i1+1));
                                }
                                break;
                            case ValueRepresentation.CombineTwo:
                                int totalCom2Fields = omrConfiguration.GetTotalFields / 2;
                                if (totalCom2Fields % 2 == 0)
                                {
                                    string com2ConfigTitle = omrConfiguration.Title;
                                    string com2DataLabel = com2ConfigTitle[0] + "";
                                    while (usedNonCollectiveDataLabels.Contains(com2DataLabel))
                                    {
                                        if (com2DataLabel.Length == com2ConfigTitle.Length)
                                            com2DataLabel = com2ConfigTitle + "_";
                                        else
                                            com2DataLabel += com2ConfigTitle[com2DataLabel.Length] + "";
                                    }
                                    usedNonCollectiveDataLabels.Add(com2DataLabel);
                                    for (int i1 = 0; i1 < totalCom2Fields; i1++)
                                    {
                                        GridTextColumn omrCom2Col = new GridTextColumn();
                                        omrCom2Col.MappingName = com2DataLabel + (i1+1).ToString();
                                        omrCom2Col.HeaderText = omrCom2Col.MappingName;
                                        mainDataGrid.Columns.Add(omrCom2Col);
                                        manualDataGrid.Columns.Add(omrCom2Col);
                                        faultyDataGrid.Columns.Add(omrCom2Col);
                                        incompatibleDataGrid.Columns.Add(omrCom2Col);

                                        gridColumns.Add(omrCom2Col.HeaderText);
                                        gridConfigOnlyColumns.Add(omrCom2Col.HeaderText);
                                        GridCellsRepresentation.Add(omrCom2Col.HeaderText, (i, i1+1));
                                    }
                                }
                                else
                                {
                                    string _indConfigTitle = omrConfiguration.Title;
                                    string _indDataLabel = _indConfigTitle[0] + "";
                                    while (usedNonCollectiveDataLabels.Contains(_indDataLabel))
                                    {
                                        if (_indDataLabel.Length == _indConfigTitle.Length)
                                            _indDataLabel = _indConfigTitle + "_";
                                        else
                                            _indDataLabel += _indConfigTitle[_indDataLabel.Length] + "";
                                    }
                                    usedNonCollectiveDataLabels.Add(_indDataLabel);
                                    for (int i1 = 0; i1 < omrConfiguration.GetTotalFields; i1++)
                                    {
                                        GridTextColumn omrIndCol = new GridTextColumn();
                                        omrIndCol.MappingName = _indDataLabel + (i1+1).ToString();
                                        omrIndCol.HeaderText = omrIndCol.MappingName;
                                        mainDataGrid.Columns.Add(omrIndCol);
                                        manualDataGrid.Columns.Add(omrIndCol);
                                        faultyDataGrid.Columns.Add(omrIndCol);
                                        incompatibleDataGrid.Columns.Add(omrIndCol);

                                        gridColumns.Add(omrIndCol.HeaderText);
                                        gridConfigOnlyColumns.Add(omrIndCol.HeaderText);
                                        GridCellsRepresentation.Add(omrIndCol.HeaderText, (i, i1+1));
                                    }
                                }
                                break;
                        }
                        break;
                    case MainConfigType.BARCODE:
                        OBRConfiguration obrConfiguration = (OBRConfiguration)allConfigs[i];

                        GridTextColumn obrCol = new GridTextColumn();
                        obrCol.MappingName = obrConfiguration.Title;
                        obrCol.HeaderText = obrConfiguration.Title;
                        mainDataGrid.Columns.Add(obrCol);
                        manualDataGrid.Columns.Add(obrCol);
                        faultyDataGrid.Columns.Add(obrCol);
                        incompatibleDataGrid.Columns.Add(obrCol);

                        gridColumns.Add(obrCol.HeaderText);
                        gridConfigOnlyColumns.Add(obrCol.HeaderText);
                        GridCellsRepresentation.Add(obrCol.HeaderText, (i, 0));
                        break;
                    case MainConfigType.ICR:
                        ICRConfiguration icrConfiguration = (ICRConfiguration)allConfigs[i];

                        GridTextColumn icrCol = new GridTextColumn();
                        icrCol.MappingName = icrConfiguration.Title;
                        icrCol.HeaderText = icrConfiguration.Title;
                        mainDataGrid.Columns.Add(icrCol);
                        manualDataGrid.Columns.Add(icrCol);
                        faultyDataGrid.Columns.Add(icrCol);
                        incompatibleDataGrid.Columns.Add(icrCol);

                        gridColumns.Add(icrCol.HeaderText);
                        gridConfigOnlyColumns.Add(icrCol.HeaderText);
                        GridCellsRepresentation.Add(icrCol.HeaderText, (i, 0));
                        break;
                 }
                switch (allConfigs[i].GetMainConfigType)
                {
                    case MainConfigType.OMR:
                        OMRConfiguration omrConfig = (OMRConfiguration)allConfigs[i];
                        switch (omrConfig.OMRType)
                        {
                            case OMRType.Gradable:
                                //GridTextColumn omrScoreCol = new GridTextColumn();
                                GridProgressBarColumn omrScoreCol = new GridProgressBarColumn();
                                omrScoreCol.ValueMode = Syncfusion.WinForms.DataGrid.Enums.ProgressBarValueMode.Value;
                                omrScoreCol.MappingName = omrConfig.Title + " Score";
                                omrScoreCol.HeaderText = omrConfig.Title + " Score";
                                mainDataGrid.Columns.Add(omrScoreCol);
                                manualDataGrid.Columns.Add(omrScoreCol);
                                faultyDataGrid.Columns.Add(omrScoreCol);
                                incompatibleDataGrid.Columns.Add(omrScoreCol);

                                //GridTextColumn omrTotalCol = new GridTextColumn();
                                //omrTotalCol.MappingName = omrConfig.Title + " Total";
                                //omrTotalCol.HeaderText = omrConfig.Title + " Total";
                                //mainDataGrid.Columns.Add(omrTotalCol);

                                GridTextColumn omrPaperCol = new GridTextColumn();
                                omrPaperCol.MappingName = omrConfig.Title + " Paper";
                                omrPaperCol.HeaderText = omrConfig.Title + " Paper";
                                mainDataGrid.Columns.Add(omrPaperCol);
                                manualDataGrid.Columns.Add(omrPaperCol);
                                faultyDataGrid.Columns.Add(omrPaperCol);
                                incompatibleDataGrid.Columns.Add(omrPaperCol);

                                GridTextColumn omrKeyCol = new GridTextColumn();
                                omrKeyCol.MappingName = omrConfig.Title + " Key";
                                omrKeyCol.HeaderText = omrConfig.Title + " Key";
                                mainDataGrid.Columns.Add(omrKeyCol);
                                manualDataGrid.Columns.Add(omrKeyCol);
                                faultyDataGrid.Columns.Add(omrKeyCol);
                                incompatibleDataGrid.Columns.Add(omrKeyCol);

                                gridColumns.Add(omrScoreCol.HeaderText);
                                //gridColumns.Add(omrTotalCol.HeaderText);
                                gridColumns.Add(omrPaperCol.HeaderText);
                                gridColumns.Add(omrKeyCol.HeaderText);

                                switch (omrConfig.KeyType)
                                {
                                    case KeyType.General:
                                        break;
                                    case KeyType.ParameterBased:
                                        //GridTextColumn omrParameterCol = new GridTextColumn();
                                        //omrParameterCol.MappingName = omrConfig.Title + " Parameter";
                                        //omrParameterCol.HeaderText = omrConfig.Title + " Parameter";
                                        //mainDataGrid.Columns.Add(omrParameterCol);

                                        //gridColumns.Add(omrParameterCol.HeaderText);
                                        break;
                                    default:
                                        break;
                                }
                                break;
                            case OMRType.Parameter:
                                break;
                            default:
                                break;
                        }
                        break;
                    case MainConfigType.BARCODE:
                        break;
                    case MainConfigType.ICR:
                        break;
                }
            }

        }
        #endregion
        #region Data Panel
        private void ExportExcel()
        {
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                string path = "";
                if (folderBrowserDialog.SelectedPath != "")
                    path = folderBrowserDialog.SelectedPath;

                var options = new ExcelExportingOptions
                {
                    ExportBorders = true,
                    ExportStyle = false,
                    ExcelVersion = ExcelVersion.Excel2016
                };
                var excelEngine = mainDataGrid.ExportToExcel((ObservableCollection<object>)mainDataGridPager.DataSource, options);
                var workBook = excelEngine.Excel.Workbooks[0];
                workBook.SaveAs(path + "\\ProcessedData.xlsx");
            }
        }
        
        #endregion

        #endregion
        #region UI
        private void SynapseMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        #region BackStage
        private void bsSettingsThemeField_SelectedIndexChanged(object sender, EventArgs e)
        {
            Themes selectedTheme = (Themes)bsSettingsThemeField.SelectedIndex;
            switch (selectedTheme)
            {
                case Themes.WHITE:
                    ribbonControl.ThemeName = "Office2016White";
                    MetroStyleColorTable metroColorTable = new MetroStyleColorTable();
                    metroColorTable.NoButtonBackColor = Color.Red;
                    metroColorTable.YesButtonBackColor = Color.SkyBlue;
                    metroColorTable.OKButtonBackColor = Color.Green;
                    MessageBoxAdv.MetroColorTable = metroColorTable;
                    MessageBoxAdv.MessageBoxStyle = MessageBoxAdv.Style.Metro;

                    dataConfigToolStripEx.ThemeName = "Office2016White";
                    readingTabStatusBar.ThemeName = "Metro";
                    dataManipulationToolStripEx.ThemeName = "Metro";
                    exportToolStripEx.ThemeName = "Metro";
                    mainDockingManager.ThemeName = "Metro";
                    mainDataGrid.ThemeName = "Office2016White";
                    manualDataGrid.ThemeName = "Office2016White";
                    faultyDataGrid.ThemeName = "Office2016White";
                    incompatibleDataGrid.ThemeName = "Office2016White";
                    mainDataGridPager.ThemeName = "Office2016White";
                    processingProgressBar.ThemeName = "Office2016White";

                    processingProgressBar.FontColor = Color.Black;
                    processingProgressBar.BackGradientStartColor = Color.LightGray;
                    processingProgressBar.BackGradientEndColor = Color.White;
                    processingProgressBar.BorderColor = Color.FromArgb(10, 158, 200);
                    processingProgressBar.GradientStartColor = Color.DeepSkyBlue;
                    processingProgressBar.GradientEndColor = Color.DodgerBlue;

                    normalDataTypePanel.BackColor = Color.FromArgb(20, 148, 215);
                    manualDataTypeStatusPanel.BackColor = Color.FromArgb(30, 138, 215);
                    faultyDataTypeStatusPanel.BackColor = Color.FromArgb(50, 128, 215);
                    incompatibleDataStatusPanel.BackColor = Color.FromArgb(60, 115, 215);

                    mainDataGrid.BackColor = Color.White;
                    mainDataGrid.Style.ProgressBarStyle.GradientForegroundStartColor = Color.DeepSkyBlue;
                    mainDataGrid.Style.ProgressBarStyle.GradientForegroundEndColor = Color.DodgerBlue;
                    manualDataGrid.BackColor = Color.White;
                    manualDataGrid.Style.ProgressBarStyle.GradientForegroundStartColor = Color.DeepSkyBlue;
                    manualDataGrid.Style.ProgressBarStyle.GradientForegroundEndColor = Color.DodgerBlue;
                    faultyDataGrid.BackColor = Color.White;
                    faultyDataGrid.Style.ProgressBarStyle.GradientForegroundStartColor = Color.DeepSkyBlue;
                    faultyDataGrid.Style.ProgressBarStyle.GradientForegroundEndColor = Color.DodgerBlue;
                    incompatibleDataGrid.BackColor = Color.White;
                    incompatibleDataGrid.Style.ProgressBarStyle.GradientForegroundStartColor = Color.DeepSkyBlue;
                    incompatibleDataGrid.Style.ProgressBarStyle.GradientForegroundEndColor = Color.DodgerBlue;

                    templateImageBox.BackColor = Color.White;
                    templateImageBox.GridColor = Color.White;
                    templateImageBox.GridColorAlternate = Color.White;
                    templateImageBox.ForeColor = SystemColors.ControlText;

                    dataImageBox.BackColor = Color.White;
                    dataImageBox.GridColor = Color.White;
                    dataImageBox.GridColorAlternate = Color.White;
                    dataImageBox.ForeColor = SystemColors.ControlText;

                    configPropertyEditor.PropertyGrid.BackColor = Color.White;
                    configPropertyEditor.PropertyGrid.HelpBackColor = SystemColors.Control;
                    configPropertyEditor.PropertyGrid.ViewBackColor = SystemColors.Window;
                    configPropertyEditor.PropertyGrid.LineColor = SystemColors.InactiveBorder;
                    configPropertyEditor.PropertyGrid.SelectedItemWithFocusBackColor = SystemColors.Highlight;
                    configPropertyEditor.PropertyGrid.ViewForeColor = SystemColors.WindowText;
                    configPropertyEditor.PropertyGrid.HelpForeColor = SystemColors.ControlText;
                    configPropertyEditor.PropertyGrid.SelectedItemWithFocusForeColor = SystemColors.HighlightText;
                    configPropertyEditor.PropertyGrid.CategoryForeColor = SystemColors.ControlText;

                    backStageTab1.BackColor = Color.White;
                    bsSettingsTablePanel.BackColor = Color.White;
                    bsSettingsTabLabel.ForeColor = SystemColors.ControlText;
                    bsSettingsThemeLabel.ForeColor = SystemColors.ControlText;
                    bsSettingsThemeField.ForeColor = SystemColors.ControlText;
                    break;
                case Themes.COLORFUL:
                    ribbonControl.ThemeName = "Office2016Colorful";
                    MessageBoxAdv.Office2016Theme = Office2016Theme.Colorful;
                    MessageBoxAdv.MessageBoxStyle = MessageBoxAdv.Style.Office2016;

                    dataConfigToolStripEx.ThemeName = "Office2016Colorful";
                    readingTabStatusBar.ThemeName = "Metro";
                    dataManipulationToolStripEx.ThemeName = "Metro";
                    dataPointStorageToolStripEx.ThemeName = "Metro";
                    exportToolStripEx.ThemeName = "Metro";
                    mainDockingManager.ThemeName = "Office2016Colorful";
                    mainDataGrid.ThemeName = "Office2016Colorful";
                    manualDataGrid.ThemeName = "Office2016Colorful";
                    faultyDataGrid.ThemeName = "Office2016Colorful";
                    incompatibleDataGrid.ThemeName = "Office2016Colorful";
                    mainDataGridPager.ThemeName = "Office2016Colorful";
                    processingProgressBar.ThemeName = "Office2016Colorful";

                    processingProgressBar.FontColor = Color.White;
                    processingProgressBar.BackGradientStartColor = Color.LightGray;
                    processingProgressBar.BackGradientEndColor = Color.White;
                    processingProgressBar.BorderColor = Color.FromArgb(10, 158, 200);
                    processingProgressBar.GradientStartColor = Color.DeepSkyBlue;
                    processingProgressBar.GradientEndColor = Color.DodgerBlue;

                    normalDataTypePanel.BackColor = Color.FromArgb(20, 148, 215);
                    manualDataTypeStatusPanel.BackColor = Color.FromArgb(30, 138, 215);
                    faultyDataTypeStatusPanel.BackColor = Color.FromArgb(50, 128, 215);
                    incompatibleDataStatusPanel.BackColor = Color.FromArgb(60, 115, 215);

                    mainDataGrid.Style.ProgressBarStyle.GradientForegroundStartColor = Color.DeepSkyBlue;
                    mainDataGrid.Style.ProgressBarStyle.GradientForegroundEndColor = Color.DodgerBlue;
                    manualDataGrid.Style.ProgressBarStyle.GradientForegroundStartColor = Color.DeepSkyBlue;
                    manualDataGrid.Style.ProgressBarStyle.GradientForegroundEndColor = Color.DodgerBlue;
                    faultyDataGrid.Style.ProgressBarStyle.GradientForegroundStartColor = Color.DeepSkyBlue;
                    faultyDataGrid.Style.ProgressBarStyle.GradientForegroundEndColor = Color.DodgerBlue;
                    incompatibleDataGrid.Style.ProgressBarStyle.GradientForegroundStartColor = Color.DeepSkyBlue;
                    incompatibleDataGrid.Style.ProgressBarStyle.GradientForegroundEndColor = Color.DodgerBlue;

                    backStageTab1.BackColor = Color.FromArgb(243, 243, 243);
                    bsSettingsTablePanel.BackColor = Color.FromArgb(243, 243, 243);
                    bsSettingsTabLabel.ForeColor = SystemColors.ControlText;
                    bsSettingsThemeLabel.ForeColor = SystemColors.ControlText;
                    bsSettingsThemeField.ForeColor = SystemColors.ControlText;

                    templateImageBox.BackColor = Color.FromArgb(243, 243, 243);
                    templateImageBox.GridColor = Color.FromArgb(243, 243, 243);
                    templateImageBox.GridColorAlternate = Color.FromArgb(243, 243, 243);
                    templateImageBox.ForeColor = Color.WhiteSmoke;

                    dataImageBox.BackColor = Color.FromArgb(243, 243, 243);
                    dataImageBox.GridColor = Color.FromArgb(243, 243, 243);
                    dataImageBox.GridColorAlternate = Color.FromArgb(243, 243, 243);
                    dataImageBox.ForeColor = Color.WhiteSmoke;

                    configPropertyEditor.PropertyGrid.BackColor = Color.FromArgb(243, 243, 243);
                    configPropertyEditor.PropertyGrid.HelpBackColor = Color.FromArgb(243, 243, 243);
                    configPropertyEditor.PropertyGrid.ViewBackColor = Color.FromArgb(243, 243, 243);
                    configPropertyEditor.PropertyGrid.LineColor = SystemColors.InactiveBorder;
                    configPropertyEditor.PropertyGrid.SelectedItemWithFocusBackColor = SystemColors.Highlight;
                    configPropertyEditor.PropertyGrid.ViewForeColor = SystemColors.WindowText;
                    configPropertyEditor.PropertyGrid.HelpForeColor = SystemColors.ControlText;
                    configPropertyEditor.PropertyGrid.SelectedItemWithFocusForeColor = SystemColors.HighlightText;
                    configPropertyEditor.PropertyGrid.CategoryForeColor = SystemColors.ControlText;
                    break;
                case Themes.DARK_GRAY:
                    ribbonControl.ThemeName = "Office2016DarkGray";
                    MessageBoxAdv.Office2016Theme = Office2016Theme.DarkGray;
                    MessageBoxAdv.MessageBoxStyle = MessageBoxAdv.Style.Office2016;

                    backStageTab1.BackColor = Color.FromArgb(68, 68, 68);
                    bsSettingsTablePanel.BackColor = Color.FromArgb(68, 68, 68);
                    bsSettingsTabLabel.ForeColor = Color.WhiteSmoke;
                    bsSettingsThemeLabel.ForeColor = Color.WhiteSmoke;
                    bsSettingsThemeField.ForeColor = Color.WhiteSmoke;

                    dataConfigToolStripEx.ThemeName = "Office2016DarkGray";
                    readingTabStatusBar.ThemeName = "Office2016DarkGray";
                    dataManipulationToolStripEx.ThemeName = "Office2016DarkGray";
                    dataPointStorageToolStripEx.ThemeName = "Office2016DarkGray";
                    exportToolStripEx.ThemeName = "Office2016DarkGray";
                    mainDockingManager.ThemeName = "Office2016DarkGray";
                    mainDataGrid.ThemeName = "Office2016DarkGray";
                    manualDataGrid.ThemeName = "Office2016DarkGray";
                    faultyDataGrid.ThemeName = "Office2016DarkGray";
                    incompatibleDataGrid.ThemeName = "Office2016DarkGray";
                    mainDataGridPager.ThemeName = "Office2016DarkGray";
                    processingProgressBar.ThemeName = "Office2016DarkGray";

                    processingProgressBar.FontColor = Color.White;
                    processingProgressBar.BackGradientStartColor = Color.FromArgb(100, 100, 100);
                    processingProgressBar.BackGradientEndColor = Color.FromArgb(115, 115, 115);
                    processingProgressBar.BorderColor = Color.FromArgb(72, 72, 72);
                    processingProgressBar.GradientStartColor = Color.FromArgb(68, 68, 68);
                    processingProgressBar.GradientEndColor = Color.FromArgb(98, 98, 98);

                    normalDataTypePanel.BackColor = Color.FromArgb(94, 94, 94);
                    manualDataTypeStatusPanel.BackColor = Color.FromArgb(78, 78, 78);
                    faultyDataTypeStatusPanel.BackColor = Color.FromArgb(72, 72, 72);
                    incompatibleDataStatusPanel.BackColor = Color.FromArgb(66, 66, 66);

                    mainDataGrid.Style.ProgressBarStyle.GradientForegroundStartColor = Color.FromArgb(150, 150, 150);
                    mainDataGrid.Style.ProgressBarStyle.GradientForegroundEndColor = Color.FromArgb(190, 190, 190);
                    manualDataGrid.Style.ProgressBarStyle.GradientForegroundStartColor = Color.FromArgb(150, 150, 150);
                    manualDataGrid.Style.ProgressBarStyle.GradientForegroundEndColor = Color.FromArgb(190, 190, 190);
                    faultyDataGrid.Style.ProgressBarStyle.GradientForegroundStartColor = Color.FromArgb(150, 150, 150);
                    faultyDataGrid.Style.ProgressBarStyle.GradientForegroundEndColor = Color.FromArgb(190, 190, 190);
                    incompatibleDataGrid.Style.ProgressBarStyle.GradientForegroundStartColor = Color.FromArgb(150, 150, 150);
                    incompatibleDataGrid.Style.ProgressBarStyle.GradientForegroundEndColor = Color.FromArgb(190, 190, 190);

                    templateImageBox.BackColor = Color.FromArgb(68, 68, 68);
                    templateImageBox.GridColor = Color.FromArgb(68, 68, 68);
                    templateImageBox.GridColorAlternate = Color.FromArgb(68, 68, 68);
                    templateImageBox.ForeColor = Color.WhiteSmoke;

                    dataImageBox.BackColor = Color.FromArgb(68, 68, 68);
                    dataImageBox.GridColor = Color.FromArgb(68, 68, 68);
                    dataImageBox.GridColorAlternate = Color.FromArgb(68, 68, 68);
                    dataImageBox.ForeColor = Color.WhiteSmoke;

                    configPropertyEditor.PropertyGrid.BackColor = Color.FromArgb(68, 68, 68);
                    configPropertyEditor.PropertyGrid.HelpBackColor = Color.FromArgb(68, 68, 68);
                    configPropertyEditor.PropertyGrid.ViewBackColor = Color.FromArgb(68, 68, 68);
                    configPropertyEditor.PropertyGrid.LineColor = Color.FromArgb(88, 88, 88);
                    configPropertyEditor.PropertyGrid.SelectedItemWithFocusBackColor = SystemColors.Highlight;
                    configPropertyEditor.PropertyGrid.ViewForeColor = Color.WhiteSmoke;
                    configPropertyEditor.PropertyGrid.HelpForeColor = Color.WhiteSmoke;
                    configPropertyEditor.PropertyGrid.SelectedItemWithFocusForeColor = SystemColors.HighlightText;
                    configPropertyEditor.PropertyGrid.CategoryForeColor = Color.WhiteSmoke;

                    break;
                case Themes.BLACK:
                    ribbonControl.ThemeName = "Office2016Black";
                    MetroStyleColorTable _metroColorTable = new MetroStyleColorTable();
                    _metroColorTable.CaptionBarColor = Color.FromArgb(60, 60, 60);
                    _metroColorTable.CaptionForeColor = Color.White;
                    _metroColorTable.BackColor = Color.FromArgb(66, 66, 66);
                    _metroColorTable.ForeColor = Color.White;
                    _metroColorTable.NoButtonBackColor = Color.FromArgb(62, 62, 62);
                    _metroColorTable.NoButtonForeColor = Color.White;
                    _metroColorTable.YesButtonBackColor = Color.FromArgb(74, 74, 74);
                    _metroColorTable.YesButtonForeColor = Color.White;
                    _metroColorTable.OKButtonBackColor = Color.FromArgb(74, 74, 74);
                    _metroColorTable.OKButtonForeColor = Color.White;
                    MessageBoxAdv.MetroColorTable = _metroColorTable;
                    MessageBoxAdv.MessageBoxStyle = MessageBoxAdv.Style.Metro;

                    backStageTab1.BackColor = Color.FromArgb(64, 64, 64);
                    bsSettingsTablePanel.BackColor = Color.FromArgb(64, 64, 64);
                    bsSettingsTabLabel.ForeColor = Color.WhiteSmoke;
                    bsSettingsThemeLabel.ForeColor = Color.WhiteSmoke;
                    bsSettingsThemeField.ForeColor = Color.WhiteSmoke;

                    dataConfigToolStripEx.ThemeName = "Office2016Black";
                    readingTabStatusBar.ThemeName = "Office2016Black";
                    dataManipulationToolStripEx.ThemeName = "Office2016Black";
                    dataPointStorageToolStripEx.ThemeName = "Office2016Black";
                    exportToolStripEx.ThemeName = "Office2016Black";
                    mainDockingManager.ThemeName = "Office2016Black";
                    mainDataGrid.ThemeName = "Office2016Black";
                    manualDataGrid.ThemeName = "Office2016Black";
                    faultyDataGrid.ThemeName = "Office2016Black";
                    incompatibleDataGrid.ThemeName = "Office2016Black";
                    mainDataGridPager.ThemeName = "Office2016Black";
                    processingProgressBar.ThemeName = "Office2016Black";

                    processingProgressBar.FontColor = Color.White;
                    processingProgressBar.BackGradientStartColor = Color.FromArgb(95, 95, 95);
                    processingProgressBar.BackGradientEndColor = Color.FromArgb(105, 105, 105);
                    processingProgressBar.BorderColor = Color.FromArgb(60, 60, 60);
                    processingProgressBar.GradientStartColor = Color.FromArgb(54, 54, 54);
                    processingProgressBar.GradientEndColor = Color.FromArgb(84, 84, 84);
                    
                    normalDataTypePanel.BackColor = Color.FromArgb(74, 74, 74);
                    manualDataTypeStatusPanel.BackColor = Color.FromArgb(68, 68, 68);
                    faultyDataTypeStatusPanel.BackColor = Color.FromArgb(62, 62, 62);
                    incompatibleDataStatusPanel.BackColor = Color.FromArgb(56, 56, 56);

                    mainDataGrid.Style.ProgressBarStyle.GradientForegroundStartColor = Color.FromArgb(54, 54, 54);
                    mainDataGrid.Style.ProgressBarStyle.GradientForegroundEndColor = Color.FromArgb(84, 84, 84);
                    manualDataGrid.Style.ProgressBarStyle.GradientForegroundStartColor = Color.FromArgb(54, 54, 54);
                    manualDataGrid.Style.ProgressBarStyle.GradientForegroundEndColor = Color.FromArgb(84, 84, 84);
                    faultyDataGrid.Style.ProgressBarStyle.GradientForegroundStartColor = Color.FromArgb(54, 54, 54);
                    faultyDataGrid.Style.ProgressBarStyle.GradientForegroundEndColor = Color.FromArgb(84, 84, 84);
                    incompatibleDataGrid.Style.ProgressBarStyle.GradientForegroundStartColor = Color.FromArgb(54, 54, 54);
                    incompatibleDataGrid.Style.ProgressBarStyle.GradientForegroundEndColor = Color.FromArgb(84, 84, 84);

                    templateImageBox.BackColor = Color.FromArgb(64, 64, 64);
                    templateImageBox.GridColor = Color.FromArgb(64, 64, 64);
                    templateImageBox.GridColorAlternate = Color.FromArgb(64, 64, 64);
                    templateImageBox.ForeColor = Color.WhiteSmoke;

                    dataImageBox.BackColor = Color.FromArgb(64, 64, 64);
                    dataImageBox.GridColor = Color.FromArgb(64, 64, 64);
                    dataImageBox.GridColorAlternate = Color.FromArgb(64, 64, 64);
                    dataImageBox.ForeColor = Color.WhiteSmoke;

                    configPropertyEditor.PropertyGrid.BackColor = Color.FromArgb(54, 54, 54);
                    configPropertyEditor.PropertyGrid.HelpBackColor = Color.FromArgb(54, 54, 54);
                    configPropertyEditor.PropertyGrid.ViewBackColor = Color.FromArgb(54, 54, 54);
                    configPropertyEditor.PropertyGrid.LineColor = Color.FromArgb(74, 74, 74);
                    configPropertyEditor.PropertyGrid.SelectedItemWithFocusBackColor = SystemColors.Highlight;
                    configPropertyEditor.PropertyGrid.ViewForeColor = Color.WhiteSmoke;
                    configPropertyEditor.PropertyGrid.HelpForeColor = Color.WhiteSmoke;
                    configPropertyEditor.PropertyGrid.SelectedItemWithFocusForeColor = SystemColors.HighlightText;
                    configPropertyEditor.PropertyGrid.CategoryForeColor = Color.WhiteSmoke;
                    break;
            }
            CurrentTheme = selectedTheme;

        }
        #endregion
        #region Configuration Tab
        private void TemplateConfigureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TemplateConfigurationForm templateConfigurationForm = null;
            if(TemplateStatus == StatusState.Green)
                templateConfigurationForm = new TemplateConfigurationForm(GetCurrentTemplate);
            else
                templateConfigurationForm = new TemplateConfigurationForm((Bitmap)templateImageBox.Image);

            templateConfigurationForm.OnConfigurationFinishedEvent += TemplateConfigurationForm_OnConfigurationFinishedEvent;
            templateConfigurationForm.WindowState = FormWindowState.Maximized;
            templateConfigurationForm.ShowDialog();
        }
        private void ConfigToolStripTabItem_Click(object sender, EventArgs e)
        {
            if (configTabPanel.Visible)
                return;

            
        }
        private void ConfigToolStripTabItem_CheckedChanged(object sender, EventArgs e)
        {
            if (configToolStripTabItem.Checked)
            {
                configTabPanel.Visible = true;
                configTabPanel.BringToFront();
                //readingTabPanel.Visible = false;

                mainDockingManager.SetDockVisibility(configPropertiesPanel, true);
                mainDockingManager.SetDockVisibility(dataImageBoxPanel, false);

                mainDockingManager.SetDockVisibility(manualDataGrid, false);
                mainDockingManager.SetDockVisibility(faultyDataGrid, false);
                mainDockingManager.SetDockVisibility(incompatibleDataGrid, false);
            }
        }
        private void TmpLoadBrowseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ImageFileBrowser.ShowDialog() == DialogResult.OK)
            {
                string location = ImageFileBrowser.FileName;
                try
                {
                    Mat tmpImage = CvInvoke.Imread(location, Emgu.CV.CvEnum.ImreadModes.Grayscale);
                    templateImageBox.Image = tmpImage.Bitmap;
                    templateImageBox.ZoomToFit();

                    OnTemplateLoadedEvent?.Invoke(this, tmpImage);
                }
                catch (Exception ex)
                {
                    Messages.LoadFileException(ex);
                }
            }
        }

        private void ConfigureDataToolStripBtn_Click(object sender, EventArgs e)
        {
            DataConfigurationForm dataConfigurationForm = new DataConfigurationForm(ConfigurationsManager.GetAllConfigurations);
            dataConfigurationForm.ShowDialog();
        }
        private void ConfigurationTestToolToolStripBtn_Click(object sender, EventArgs e)
        {
            ConfigurationsTestForm configurationsTestForm = new ConfigurationsTestForm(ConfigurationsManager.GetAllConfigurations);
            configurationsTestForm.ShowDialog();
        }
        private void AddAsOmrToolStripBtn_Click(object sender, EventArgs e)
        {
            RectangleF selectedRegion = templateImageBox.SelectionRegion;
            if (selectedRegion.IsEmpty)
            {
                Messages.ShowError("Please select a reagion on the template to do this operation.");
                return;
            }

            AddRegionAsOMR(selectedRegion);
        }
        private void AddAsBarcodeToolStripBtn_Click(object sender, EventArgs e)
        {
            RectangleF selectedRegion = templateImageBox.SelectionRegion;
            if (selectedRegion.IsEmpty)
            {
                Messages.ShowError("Please select a reagion on the template to do this operation.");
                return;
            }

            AddRegionAsBarcode(selectedRegion);
        }
        private void AddAsICRToolStripBtn_Click(object sender, EventArgs e)
        {
            RectangleF selectedRegion = templateImageBox.SelectionRegion;
            if (selectedRegion.IsEmpty)
            {
                Messages.ShowError("Please select a reagion on the template to do this operation.");
                return;
            }

            AddRegionAsICR(selectedRegion);
        }

        private void TemplateImageBox_Paint(object sender, PaintEventArgs e)
        {
            if(ConfigurationStatus == StatusState.Green && templateImageBox.Image != null)
            {
                for (int i = 0; i < OnTemplateConfigs.Count; i++)
                {
                    DrawConfiguration(OnTemplateConfigs[i], e.Graphics);
                }
            }
        }
        private OnTemplateConfig[] lastPossibleInterestsConfigs;
        private void TemplateImageBox_MouseMove(object sender, MouseEventArgs e)
        {
            curImageMouseLoc = e.Location;

            if (ConfigurationStatus == StatusState.Green)
            {
                List<OnTemplateConfig> possibleInterests = new List<OnTemplateConfig>();
                for (int i = 0; i < OnTemplateConfigs.Count; i++)
                {
                    RectangleF offsetRect = OnTemplateConfigs[i].OffsetRectangle;

                    if (offsetRect.Contains(e.Location))
                    {                       
                        if (InterestedTemplateConfig == OnTemplateConfigs[i])
                            continue;

                        possibleInterests.Add(OnTemplateConfigs[i]); 
                    }
                    else
                    {
                        if (InterestedTemplateConfig == OnTemplateConfigs[i])
                            InterestedTemplateConfig = null;

                        if(SelectedTemplateConfig == OnTemplateConfigs[i])
                            continue;

                        switch (OnTemplateConfigs[i].Configuration.GetMainConfigType)
                        {
                            case MainConfigType.OMR:
                                OnTemplateConfigs[i].ColorStates.CurrentColor = OMRRegionColorStates.NormalColor;
                                IsMouseOverRegion = false;
                                break;
                            case MainConfigType.BARCODE:
                                OnTemplateConfigs[i].ColorStates.CurrentColor = OBRRegionColorStates.NormalColor;
                                IsMouseOverRegion = false;
                                break;
                            case MainConfigType.ICR:
                                OnTemplateConfigs[i].ColorStates.CurrentColor = ICRRegionColorStates.NormalColor;
                                IsMouseOverRegion = false;
                                break;
                        }
                    }

                }

                if(possibleInterests.Count > 0)
                {
                    if (lastPossibleInterestsConfigs != null && possibleInterests.TrueForAll(x => lastPossibleInterestsConfigs.Contains(x)) && lastPossibleInterestsConfigs.Contains(InterestedTemplateConfig))
                        return;

                    InterestedTemplateConfig = possibleInterests.Last();

                    if (SelectedTemplateConfig == InterestedTemplateConfig)
                        return;

                    switch (InterestedTemplateConfig.Configuration.GetMainConfigType)
                    {
                        case MainConfigType.OMR:
                            InterestedTemplateConfig.ColorStates.CurrentColor = OMRRegionColorStates.HighlightedColor;
                            IsMouseOverRegion = true;
                            break;
                        case MainConfigType.BARCODE:
                            InterestedTemplateConfig.ColorStates.CurrentColor = OBRRegionColorStates.HighlightedColor;
                            IsMouseOverRegion = true;
                            break;
                        case MainConfigType.ICR:
                            InterestedTemplateConfig.ColorStates.CurrentColor = ICRRegionColorStates.HighlightedColor;
                            IsMouseOverRegion = true;
                            break;
                    }
                }
                lastPossibleInterestsConfigs = possibleInterests.ToArray();

                templateImageBox.Invalidate();
            }
        }
        private void TemplateImageBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (ConfigurationStatus == StatusState.Green && e.Button == MouseButtons.Left)
            {
                if (InterestedTemplateConfig != null)
                {
                    switch (InterestedTemplateConfig.Configuration.GetMainConfigType)
                    {
                        case MainConfigType.OMR:
                            InterestedTemplateConfig.ColorStates.CurrentColor = OMRRegionColorStates.PressedColor;
                            IsMouseDownRegion = true;
                            break;
                        case MainConfigType.BARCODE:
                            InterestedTemplateConfig.ColorStates.CurrentColor = OBRRegionColorStates.PressedColor;
                            IsMouseDownRegion = true;
                            break;
                        case MainConfigType.ICR:
                            InterestedTemplateConfig.ColorStates.CurrentColor = ICRRegionColorStates.PressedColor;
                            IsMouseDownRegion = true;
                            break;
                    }

                    templateImageBox.Invalidate();
                }
            }
        }
        private void TemplateImageBox_MouseUp(object sender, MouseEventArgs e)
        {
            if (ConfigurationStatus == StatusState.Green && e.Button == MouseButtons.Left)
            {
                if (InterestedTemplateConfig != null)
                {
                    if (SelectedTemplateConfig != null)
                    {
                        switch (SelectedTemplateConfig.Configuration.GetMainConfigType)
                        {
                            case MainConfigType.OMR:
                                SelectedTemplateConfig.ColorStates.CurrentColor = SelectedTemplateConfig == InterestedTemplateConfig? OMRRegionColorStates.HighlightedColor : OMRRegionColorStates.NormalColor;
                                IsMouseUpRegion = false;
                                break;
                            case MainConfigType.BARCODE:
                                SelectedTemplateConfig.ColorStates.CurrentColor = SelectedTemplateConfig == InterestedTemplateConfig ? OBRRegionColorStates.HighlightedColor : OBRRegionColorStates.NormalColor;
                                IsMouseUpRegion = false;
                                break;
                            case MainConfigType.ICR:
                                SelectedTemplateConfig.ColorStates.CurrentColor = SelectedTemplateConfig == InterestedTemplateConfig ? ICRRegionColorStates.HighlightedColor : ICRRegionColorStates.NormalColor;
                                IsMouseUpRegion = false;
                                break;
                        }
                    }

                    if (SelectedTemplateConfig != InterestedTemplateConfig)
                    {
                        SelectedTemplateConfig = InterestedTemplateConfig;

                        switch (SelectedTemplateConfig.Configuration.GetMainConfigType)
                        {
                            case MainConfigType.OMR:
                                SelectedTemplateConfig.ColorStates.CurrentColor = OMRRegionColorStates.SelectedColor;
                                IsMouseUpRegion = true;
                                break;
                            case MainConfigType.BARCODE:
                                SelectedTemplateConfig.ColorStates.CurrentColor = OBRRegionColorStates.SelectedColor;
                                IsMouseUpRegion = true;
                                break;
                            case MainConfigType.ICR:
                                SelectedTemplateConfig.ColorStates.CurrentColor = ICRRegionColorStates.SelectedColor;
                                IsMouseUpRegion = true;
                                break;
                        }
                    }
                    else
                        SelectedTemplateConfig = null;

                    templateImageBox.Invalidate();
                }
                else
                {
                    if (SelectedTemplateConfig != null)
                    {
                        switch (SelectedTemplateConfig.Configuration.GetMainConfigType)
                        {
                            case MainConfigType.OMR:
                                SelectedTemplateConfig.ColorStates.CurrentColor = OMRRegionColorStates.NormalColor;
                                IsMouseUpRegion = false;
                                break;
                            case MainConfigType.BARCODE:
                                SelectedTemplateConfig.ColorStates.CurrentColor = OBRRegionColorStates.NormalColor;
                                IsMouseUpRegion = false;
                                break;
                            case MainConfigType.ICR:
                                SelectedTemplateConfig.ColorStates.CurrentColor = ICRRegionColorStates.NormalColor;
                                IsMouseUpRegion = false;
                                break;
                        }
                    }

                    SelectedTemplateConfig = null;
                }
            }
        }
        #endregion
        #region Reading Tab
        private void ReadingToolStripTabItem_Click(object sender, EventArgs e)
        {
            if (!configTabPanel.Visible)
                return;
        }
        private void ReadingToolStripTabItem_CheckedChanged(object sender, EventArgs e)
        {
            if(readingToolStripTabItem.Checked)
            {
                //readingTabPanel.Visible = true;
                configTabPanel.Visible = false;
                configTabPanel.SendToBack();

                if(!MainProcessingManager.IsProcessing)
                    GenerateGridColumns();

                mainDockingManager.SetDockVisibility(dataImageBoxPanel, true);
                mainDockingManager.SetDockVisibility(configPropertiesPanel, false);

                mainDockingManager.SetDockVisibility(manualDataGrid, true);
                mainDockingManager.SetDockVisibility(faultyDataGrid, true);
                mainDockingManager.SetDockVisibility(incompatibleDataGrid, true);
            }
        }
        private void AnswerKeyToolStripBtn_Click(object sender, EventArgs e)
        {
            if (mainDockingManager.GetDockVisibility(answerKeyPanel))
                mainDockingManager.SetDockVisibility(answerKeyPanel, false);
            else
            {
                answerKeyMainPanel.Visible = true;
                addAnswerKeyPanel.Visible = false;
                addAnswerKeyPanel.Dock = DockStyle.Fill;
                answerKeyMainPanel.Dock = DockStyle.Fill;

                var omrConfigs = ConfigurationsManager.GetConfigurations(MainConfigType.OMR, new Func<ConfigurationBase, bool>((ConfigurationBase x) => { OMRConfiguration omrX = (OMRConfiguration)x; return omrX.OMRType == OMRType.Gradable; })).ConvertAll(x => (OMRConfiguration)x);
                configurationComboBox.DataSource = omrConfigs;
                configurationComboBox.DisplayMember = "Title";

                PopulateAnswerKeyFields(omrConfigs);
            }
        }

        private async void PapersToolStripBtn_Click(object sender, EventArgs e)
        {
            await GeneralManager.Initialize();

            ExamPapersConfigurationForm examPapersConfigurationForm = new ExamPapersConfigurationForm(GeneralManager.GetExamPapers);
            examPapersConfigurationForm.ShowDialog();
        }
        #region Answer Key Panel
        async Task PopulateAnswerKeyFields(List<OMRConfiguration> omrConfigs)
        {
            if (answerKeyListFlowPanel.Controls.Count > 0)
            {
                for (int i = 0; i < answerKeyListFlowPanel.Controls.Count; i++)
                {
                    answerKeyListFlowPanel.Controls[i].Dispose();
                }
                answerKeyListFlowPanel.Controls.Clear();
            }

            await Task.Run(() =>
            {
                for (int i = 0; i < omrConfigs.Count; i++)
                {
                    var omrConfig = omrConfigs[i];
                    AnswerKey generalKey = omrConfig.GeneralAnswerKey;
                    if (generalKey != null)
                    {
                        AnswerKeyListItem answerKeyListItem = AnswerKeyListItem.Create(omrConfig.Title, generalKey, OnAnswerKeyListItemControlButtonPressed);
                        synchronizationContext.Post(new SendOrPostCallback((state) =>
                        {
                            answerKeyListFlowPanel.Controls.Add(answerKeyListItem);
                            answerKeyListItem.Dock = DockStyle.Top;
                            answerKeysEmptyListLabel.Visible = false;
                        }), null);
                    }

                    if (omrConfig.PB_AnswerKeys != null && omrConfig.PB_AnswerKeys.Count > 0)
                    {
                        var keys = omrConfig.PB_AnswerKeys.Values.ToList();
                        for (int i1 = 0; i1 < keys.Count; i1++)
                        {
                            AnswerKeyListItem answerKeyListItem = AnswerKeyListItem.Create(omrConfig.Title, keys[i1], OnAnswerKeyListItemControlButtonPressed);
                            synchronizationContext.Post(new SendOrPostCallback((state) =>
                            {
                                answerKeyListFlowPanel.Controls.Add(answerKeyListItem);
                                answerKeyListItem.Dock = DockStyle.Top;
                            }), null);
                        }
                        synchronizationContext.Post(new SendOrPostCallback((state) =>
                        {
                            answerKeysEmptyListLabel.Visible = false;
                        }), null);
                    }
                }
            });

            mainDockingManager.DockControl(answerKeyPanel, this, DockingStyle.Left, 350);
            mainDockingManager.SetDockVisibility(answerKeyPanel, true);
        }

        private void addAnswerKeyBtn_Click(object sender, EventArgs e)
        {
            answerKeyMainPanel.Visible = false;

            answerKeyTitleField.Text = "New Key Title";
            answerKeyParameterValueField.Text = "Value";
            addAnswerKeyPanel.Visible = true;
            addAnswerKeyPanel.Dock = DockStyle.Fill;
            //configurationComboBox.DataSource = ConfigurationsManager.GetConfigurations(MainConfigType.OMR, new Func<ConfigurationBase, bool>((ConfigurationBase x) => { OMRConfiguration omrX = (OMRConfiguration)x; return omrX.OMRType == OMRType.Gradable; }));
            //configurationComboBox.DisplayMember = "Title";

            var exmPapers = GeneralManager.GetExamPapers;
            answerKeyPaperComboBox.DataSource = exmPapers != null ? exmPapers.GetPapers : null;
            answerKeyPaperComboBox.DisplayMember = "Title";
        }

        private void configurationComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            OMRConfiguration selectedOMRConfig = (OMRConfiguration)configurationComboBox.SelectedItem;
            if (selectedOMRConfig == null)
                return;

            switch (selectedOMRConfig.KeyType)
            {
                case Core.Keys.KeyType.General:
                    answerKeyParameterTable.Visible = false;
                    answerKeyControlsSplitterContainer.SplitterDistance = 85;
                    break;
                case Core.Keys.KeyType.ParameterBased:
                    answerKeyParameterTable.Visible = true;
                    answerKeyControlsSplitterContainer.SplitterDistance = 125;

                    answerKeyParameterField.DataSource = ConfigurationsManager.GetConfigurations(MainConfigType.OMR, new Func<ConfigurationBase, bool>((ConfigurationBase x) => { OMRConfiguration omrX = (OMRConfiguration)x; return omrX.OMRType == OMRType.Parameter; }));
                    answerKeyParameterField.DisplayMember = "Title";
                    break;
            }

            SetupKeyFields(selectedOMRConfig);
        }

        private void SetupKeyFields(OMRConfiguration omrConfiguration)
        {
            int totalFields = omrConfiguration.GetTotalFields;
            int totalOptions = omrConfiguration.GetTotalOptions;
            Orientation orientation = omrConfiguration.Orientation;

            for (int i = 0; i < answerKeyFieldsTable.Controls.Count; i++)
            {
                answerKeyFieldsTable.Controls[i].Dispose();
            }
            answerKeyFieldsTable.Controls.Clear();
            answerKeyFieldsTable.ColumnStyles.Clear();
            answerKeyFieldsTable.RowStyles.Clear();
            answerKeyFieldsTable.Dock = DockStyle.None;
            answerKeyFieldsTable.Size = new Size(100, 100);
            answerKeyFieldsTable.Dock = DockStyle.Fill;

            switch (orientation)
            {
                case Orientation.Horizontal:
                    answerKeyFieldsTable.ColumnCount = 1;
                    answerKeyFieldsTable.RowCount = totalFields+1;

                    for (int i = 0; i < totalFields; i++)
                    {
                        answerKeyFieldsTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));
                        AnswerKeyFieldControl keyFieldControl = new AnswerKeyFieldControl();
                        answerKeyFieldsTable.Controls.Add(keyFieldControl);
                        answerKeyFieldsTable.SetRow(keyFieldControl, i);
                        keyFieldControl.Initialize(i+1);
                        keyFieldControl.TotalOptions = totalOptions;
                        keyFieldControl.FieldOrientation = orientation;
                        keyFieldControl.OptionsValueType = omrConfiguration.ValueDataType;
                        keyFieldControl.Dock = DockStyle.Fill;
                    }
                    break;
                case Orientation.Vertical:
                    answerKeyFieldsTable.RowCount = 1;
                    answerKeyFieldsTable.ColumnCount = totalFields+1;


                    for (int i = 0; i < totalFields; i++)
                    {
                        answerKeyFieldsTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
                        AnswerKeyFieldControl keyFieldControl = new AnswerKeyFieldControl();
                        answerKeyFieldsTable.Controls.Add(keyFieldControl);
                        answerKeyFieldsTable.SetColumn(keyFieldControl, i);
                        keyFieldControl.Initialize(i + 1);
                        keyFieldControl.TotalOptions = totalOptions;
                        keyFieldControl.FieldOrientation = orientation;
                        keyFieldControl.OptionsValueType = omrConfiguration.ValueDataType;
                        keyFieldControl.Dock = DockStyle.Fill;
                    }
                    break;
            }

        }
        private void PopulateFields(AnswerKey key)
        {
            int curTotalFields = answerKeyFieldsTable.Controls.Count;
            if(curTotalFields > 0)
            {
                AnswerKeyFieldControl keyFieldControl = (AnswerKeyFieldControl)answerKeyFieldsTable.Controls[0];
                int curTotalOptions = keyFieldControl.TotalOptions;

                List<int[]> _key = key.GetKey.ToList();
                if (_key.Count == curTotalFields && _key.TrueForAll(x => x.Length == curTotalOptions))
                {
                    for (int i = 0; i < curTotalFields; i++)
                    {
                        AnswerKeyFieldControl _keyFieldControl = (AnswerKeyFieldControl)answerKeyFieldsTable.Controls[i];
                        _keyFieldControl.SetOptions(_key[i]);
                    }
                }
            }
        }

        System.Action DeleteToEditKeyItem;
        private void DeleteAnswerKeyItem(OMRConfiguration omrConfig, AnswerKeyListItem keyListItem, bool ask = true)
        {
            if (omrConfig.GeneralAnswerKey != null && omrConfig.GeneralAnswerKey.Title == keyListItem.KeyTitle)
            {
                if (!ask || Messages.ShowQuestion($"Answer Key: {keyListItem.KeyTitle} - Configuration: {omrConfig.Title ?? "N/A"} \nAre you sure you want to delete this answer key?") == DialogResult.Yes)
                {
                    omrConfig.GeneralAnswerKey = null;
                    answerKeyListFlowPanel.Controls.Remove(keyListItem);
                    keyListItem.Dispose();
                }
            }
            else if (omrConfig.PB_AnswerKeys != null && omrConfig.PB_AnswerKeys.Count > 0 && omrConfig.PB_AnswerKeys.Values.Any(x => x.Title == keyListItem.KeyTitle))
            {
                var dicItem = omrConfig.PB_AnswerKeys.First(x => x.Value.Title == keyListItem.KeyTitle);
                if (!ask || Messages.ShowQuestion($"Answer Key: {keyListItem.KeyTitle} - Parameter: {dicItem.Key.parameterConfig.Title ?? "N/A"} - Configuration: {omrConfig.Title ?? "N/A"} \nAre you sure you want to delete this answer key?") == DialogResult.Yes)
                {
                    omrConfig.PB_AnswerKeys.Remove(dicItem.Key);
                    answerKeyListFlowPanel.Controls.Remove(keyListItem);
                    keyListItem.Dispose();
                }
            }
            if (answerKeyListFlowPanel.Controls.Count == 0)
                answerKeysEmptyListLabel.Visible = true;
        }
        private async void setBtn_Click(object sender, EventArgs e)
        {
            OMRConfiguration selectedOMRConfig = (OMRConfiguration)configurationComboBox.SelectedItem;
            if (selectedOMRConfig == null)
                return;

            Paper selectedPaper = (Paper)answerKeyPaperComboBox.SelectedItem;
            if(selectedPaper == null)
            {
                Messages.ShowError("Please select a valid paper or create one to link with the answer key.");
                return;
            }
            int totalFields = selectedPaper.GetFieldsCount;
            if(totalFields > selectedOMRConfig.GetTotalFields)
            {
                Messages.ShowError("Please select a valid paper or create one to link with the answer key. \n\nValidation Error: Paper fields cannot be greater than total fields of the selected gradable region.");
                return;
            }
            int totalOptions = selectedPaper.GetOptionsCount;

            string keyTitle = answerKeyTitleField.Text;
            int[][] answerKeyOptions = new int[totalFields][];

            for (int i = 0; i < totalFields; i++)
            {
                if (i > answerKeyFieldsTable.Controls.Count)
                {
                    Messages.ShowError("Couldn't add the answer key because not enough fields found to satisfy the paper.");
                    return;
                }

                var curField = (AnswerKeyFieldControl)answerKeyFieldsTable.Controls[i];
                int[] optionsValues = new int[totalOptions];
                var marked = curField.GetMarkedOptions();
                if (marked == null)
                {
                    Messages.ShowError("Couldn't add the answer key due to invalid answer key fields.");
                    return;
                }
                else if(marked.Length == 0)
                {
                    Messages.ShowError("Couldn't add the answer key due to invalid answer key fields, All fields must be filled.");
                    return;
                }
                for (int j = 0; j < marked.Length; j++)
                {
                    optionsValues[marked[j]] = 1;
                }
                answerKeyOptions[i] = optionsValues;
            }
            
            AnswerKey answerKey = new AnswerKey(keyTitle, selectedOMRConfig.Title, answerKeyOptions, selectedPaper);

            DeleteToEditKeyItem?.Invoke();
            DeleteToEditKeyItem = null;

            bool isSuccess = false;
            string err = "";
            switch (selectedOMRConfig.KeyType)
            {
                case KeyType.General:
                    isSuccess = selectedOMRConfig.SetGeneralAnswerKey(answerKey, out err);
                    if (!isSuccess && err != "User Denied")
                        Messages.ShowError("Couldn't add the answer key due to invalid answer key. \n\n Error: " + err);
                    break;
                case KeyType.ParameterBased:
                    var paramConfig = (ConfigurationBase)answerKeyParameterField.SelectedItem;
                    if (paramConfig == null)
                    {
                        Messages.ShowError("Couldn't add the answer key due to invalid answer key parameter. \n\n Please select a valid parameter to link with the answer key.");
                        return;
                    }
                    string paramValue = answerKeyParameterValueField.Text;
                    if (paramValue == "")
                    {
                        Messages.ShowError("Couldn't add the answer key due to invalid answer key parameter. \n\n Please select a valid parameter value to link with the answer key parameter.");
                        return;
                    }
                    Parameter keyParameter = new Parameter(paramConfig, paramValue);
                    isSuccess = selectedOMRConfig.AddPBAnswerKey(keyParameter, answerKey, out err);
                    if (!isSuccess && err != "User Denied")
                        Messages.ShowError("Couldn't add the key due to invalid answer key properties. \n\n Error: " + err);
                    break;
            }

            if(isSuccess)
            {
                await PopulateAnswerKeyFields((List<OMRConfiguration>)configurationComboBox.DataSource);

                addAnswerKeyPanel.Visible = false;
                answerKeyMainPanel.Visible = true;
                answerKeyMainPanel.Dock = DockStyle.Fill;
            }
        }
        private void OnAnswerKeyListItemControlButtonPressed(object sender, AnswerKeyListItem.ControlButton controlButton)
        {
            AnswerKeyListItem keyListItem = (AnswerKeyListItem)sender;
            OMRConfiguration omrConfig = (OMRConfiguration)ConfigurationsManager.GetConfiguration(keyListItem.ConfigTitle);
            switch (controlButton)
            {
                case AnswerKeyListItem.ControlButton.Delete:
                    DeleteAnswerKeyItem(omrConfig, keyListItem);
                    break;
                case AnswerKeyListItem.ControlButton.Configure:
                    DeleteToEditKeyItem = () => { DeleteAnswerKeyItem(omrConfig, keyListItem, false); };

                    answerKeyMainPanel.Visible = false;
                    addAnswerKeyPanel.Dock = DockStyle.Fill;
                    var exmPapers = GeneralManager.GetExamPapers;
                    answerKeyPaperComboBox.DataSource = exmPapers != null ? exmPapers.GetPapers : null;
                    answerKeyPaperComboBox.DisplayMember = "Title";
                    if (omrConfig.GeneralAnswerKey != null && omrConfig.GeneralAnswerKey.Title == keyListItem.KeyTitle)
                    {
                        AnswerKey answerKey = omrConfig.GeneralAnswerKey;

                        configurationComboBox.SelectedItem = omrConfig;
                        answerKeyTitleField.Text = answerKey.Title;

                        answerKeyPaperComboBox.SelectedItem = answerKey.GetPaper;

                        PopulateFields(answerKey);
                    }
                    else if (omrConfig.PB_AnswerKeys != null && omrConfig.PB_AnswerKeys.Count > 0 && omrConfig.PB_AnswerKeys.Values.Any(x => x.Title == keyListItem.KeyTitle))
                    {
                        var dicItem = omrConfig.PB_AnswerKeys.First(x => x.Value.Title == keyListItem.KeyTitle);

                        configurationComboBox.SelectedItem = omrConfig;
                        answerKeyTitleField.Text = keyListItem.KeyTitle;
                        answerKeyParameterValueField.Text = dicItem.Key.parameterValue;

                        answerKeyPaperComboBox.SelectedItem = dicItem.Value.GetPaper;

                        PopulateFields(dicItem.Value);
                    }
                    addAnswerKeyPanel.Visible = true;
                    break;
            }
        }
        #endregion
        private async void ScanDirectoryToolStripBtn_Click(object sender, EventArgs e)
        {
            if(folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                string selectedFolder = folderBrowserDialog.SelectedPath;
                bool includeSubDirs = includeSubFoldersToolStripCheckBox.Checked;

                await InitializeSheetsToRead(selectedFolder, includeSubDirs);
                if (loadedSheetsData.SheetsLoaded)
                {
                    UpdateStatus("Scan Directory Loaded, Successfully");
                }
                else
                {
                    UpdateStatus("Directory To Scan Failed To Load");
                }
            }
        }

        private void UpdateImageSelection(ProcessedDataRow processedDataRow, bool locateOptions = false)
        {
            if(!locateOptions)
            {
                dataImageBox.Image = new Bitmap(processedDataRow.RowSheetPath);
                return;
            }

            Mat alignedMat = null;
            if (!processedDataRow.GetAlignedImage(out alignedMat))
            {
                dataImageBox.Image = new Bitmap(processedDataRow.RowSheetPath);
                return; 
            }
            dataImageBox.Image = alignedMat.Bitmap;

            curOptionRects.Clear();
            curMarkedOptionRects.Clear();
            var entries = processedDataRow.GetProcessedDataEntries;
            for (int i = 0; i < entries.Count; i++)
            {
                if (entries[i].GetMainConfigType != MainConfigType.OMR)
                    continue;

                List<Point> markedRectIndexes = new List<Point>();
                var omrConfig = (OMRConfiguration)entries[i].GetConfigurationBase;
                var rawDataValues = ProcessedDataEntry.GenerateRawOMRDataValues(omrConfig, entries[i].GetFieldsOutputs, omrConfig.GetEscapeSymbols());
                int totalFields = omrConfig.GetTotalFields;
                int totalOptions = omrConfig.GetTotalOptions;
                for (int i1 = 0; i1 < totalFields; i1++)
                {
                    for (int j = 0; j < totalOptions; j++)
                    {
                        if (rawDataValues[i1, j] == 1)
                            markedRectIndexes.Add(new Point(i1, j));
                    }
                }
                var _curOptionRects = omrConfig.RegionData.GetOptionsRects;
                var _alignedCurOptionRects = new List<RectangleF>();
                var regionLocation = omrConfig.GetConfigArea.ConfigRect.Location;
                for (int i1 = 0; i1 < _curOptionRects.Count; i1++)
                {
                    var optionRect = _curOptionRects[i1];
                    optionRect.X += regionLocation.X;
                    optionRect.Y += regionLocation.Y;

                    curOptionRects.Add(optionRect);
                    _alignedCurOptionRects.Add(optionRect);
                }
                for (int i2 = 0; i2 < markedRectIndexes.Count; i2++)
                {
                    int index = markedRectIndexes[i2].X * totalOptions + markedRectIndexes[i2].Y;
                    curMarkedOptionRects.Add(_alignedCurOptionRects[index]);
                }
                curOptionRects.RemoveAll(x => curMarkedOptionRects.Contains(x));
            }
        }
        private void MainDataGrid_SelectionChanged(object sender, Syncfusion.WinForms.DataGrid.Events.SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0)
            {
                curOptionRects.Clear();
                curMarkedOptionRects.Clear();

                dataImageBox.Image = null;
                return;
            }

            dynamic lastItemData = (dynamic)e.AddedItems.Last();
            //if (e.AddedItems.RowType != Syncfusion.WinForms.DataGrid.Enums.RowType.DefaultRow)
            //    return;

            SelectedProcessedDataRow = (ProcessedDataRow)lastItemData.DataRowObject;
            UpdateImageSelection(SelectedProcessedDataRow.Value, GetLocateOptionsToggle);
        }
        private void mainDataGrid_QueryCellStyle(object sender, Syncfusion.WinForms.DataGrid.Events.QueryCellStyleEventArgs e)
        {
            dynamic dataObject = (dynamic)e.DataRow.RowData;
            string colText = e.Column.HeaderText;
            if (!GridCellsRepresentation.ContainsKey(colText))
                return;

            ProcessedDataType fieldDataType = ProcessedDataType.NORMAL;

            try
            {
                (int entryIndex, int fieldIndex) cellRepresentation = GridCellsRepresentation[colText];
                fieldDataType = dataObject.DataRowObject.GetProcessedDataEntries[cellRepresentation.entryIndex].DataEntriesResultType[cellRepresentation.fieldIndex];
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}\nColumn: {e.Column.HeaderText} - {e.DisplayText}\nRow: {e.RowIndex}");
            }

            switch (fieldDataType)
            {
                case ProcessedDataType.INCOMPATIBLE:
                    if (incompatibleDataCellBackColor != Color.Empty) e.Style.BackColor = incompatibleDataCellBackColor;
                    if (incompatibleDataCellForeColor != Color.Empty) e.Style.TextColor = incompatibleDataCellForeColor;
                    break;
                case ProcessedDataType.FAULTY:
                    if (faultyDataCellBackColor != Color.Empty) e.Style.BackColor = faultyDataCellBackColor;
                    if (faultyDataCellForeColor != Color.Empty) e.Style.TextColor = faultyDataCellForeColor;
                    break;
                case ProcessedDataType.MANUAL:
                    if (manualDataCellBackColor != Color.Empty) e.Style.BackColor = manualDataCellBackColor;
                    if (manualDataCellForeColor != Color.Empty) e.Style.TextColor = manualDataCellForeColor;
                    break;
                case ProcessedDataType.NORMAL:
                    break;
            }
        }
        private void MainDataGrid_QueryRowStyle(object sender, Syncfusion.WinForms.DataGrid.Events.QueryRowStyleEventArgs e)
        {
            e.Style.HorizontalAlignment = HorizontalAlignment.Center;
            if (e.RowType != Syncfusion.WinForms.DataGrid.Enums.RowType.DefaultRow)
                return;

            dynamic dataObject = (dynamic)e.RowData;

            ProcessedDataType fieldDataType = ProcessedDataType.NORMAL;

            try
            {
                fieldDataType = dataObject.DataRowObject.DataRowResultType;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}\nRow: {e.RowIndex}");
            }

            switch (fieldDataType)
            {
                case ProcessedDataType.INCOMPATIBLE:
                    if (incompatibleDataRowBackColor != Color.Empty) e.Style.BackColor = incompatibleDataRowBackColor;
                    if (incompatibleDataRowForeColor != Color.Empty) e.Style.TextColor = incompatibleDataRowForeColor;
                    break;
                case ProcessedDataType.FAULTY:
                    if (faultyDataRowBackColor != Color.Empty) e.Style.BackColor = faultyDataRowBackColor;
                    if (faultyDataRowForeColor != Color.Empty) e.Style.TextColor = faultyDataRowForeColor;
                    break;
                case ProcessedDataType.MANUAL:
                    if (manualDataRowBackColor != Color.Empty) e.Style.BackColor = manualDataRowBackColor;
                    if (manualDataRowForeColor != Color.Empty) e.Style.TextColor = manualDataRowForeColor;
                    break;
                case ProcessedDataType.NORMAL:
                    break;
            }
        }
        private void mainDataGrid_QueryProgressBarCellStyle(object sender, Syncfusion.WinForms.DataGrid.Events.QueryProgressBarCellStyleEventArgs e)
        {
            if (!e.Column.HeaderText.Contains(" Score"))
                return;

            var dataObject = (dynamic)e.Record;
            AnswerKey ansKey = Functions.GetProperty(dataObject, "AnswerKey");
            if (ansKey == null)
                return;

            int maximum = ansKey.GetPaper.GetCorrectOptionValue * ansKey.GetKey.Length;
            e.Maximum = maximum == 0 ? 100 : maximum;

            e.Style.ProgressTextColor = (CurrentTheme == Themes.BLACK || CurrentTheme == Themes.DARK_GRAY) ? Color.White : e.Style.ProgressTextColor;
            e.Style.BackgroundColor = (CurrentTheme == Themes.DARK_GRAY) ? Color.FromArgb(210, 210, 210) : e.Style.BackgroundColor;
            //mainDataGrid.View.
        }

        private void manualDataGrid_SelectionChanged(object sender, Syncfusion.WinForms.DataGrid.Events.SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0)
            {
                curOptionRects.Clear();
                curMarkedOptionRects.Clear();

                dataImageBox.Image = null;
                return;
            }

            dynamic lastItemData = (dynamic)e.AddedItems.Last();
            //if (e.AddedItems.RowType != Syncfusion.WinForms.DataGrid.Enums.RowType.DefaultRow)
            //    return;

            SelectedProcessedDataRow = (ProcessedDataRow)lastItemData.DataRowObject;
            UpdateImageSelection(SelectedProcessedDataRow.Value, GetLocateOptionsToggle);
        }
        private void manualDataGrid_QueryCellStyle(object sender, Syncfusion.WinForms.DataGrid.Events.QueryCellStyleEventArgs e)
        {
            dynamic dataObject = (dynamic)e.DataRow.RowData;
            string colText = e.Column.HeaderText;
            if (!GridCellsRepresentation.ContainsKey(colText))
                return;

            ProcessedDataType fieldDataType = ProcessedDataType.NORMAL;

            try
            {
                (int entryIndex, int fieldIndex) cellRepresentation = GridCellsRepresentation[colText];
                fieldDataType = dataObject.DataRowObject.GetProcessedDataEntries[cellRepresentation.entryIndex].DataEntriesResultType[cellRepresentation.fieldIndex];
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}\nColumn: {e.Column.HeaderText} - {e.DisplayText}\nRow: {e.RowIndex}");
            }

            switch (fieldDataType)
            {
                case ProcessedDataType.INCOMPATIBLE:
                    if (incompatibleDataCellBackColor != Color.Empty) e.Style.BackColor = incompatibleDataCellBackColor;
                    if (incompatibleDataCellForeColor != Color.Empty) e.Style.TextColor = incompatibleDataCellForeColor;
                    break;
                case ProcessedDataType.FAULTY:
                    if (faultyDataCellBackColor != Color.Empty) e.Style.BackColor = faultyDataCellBackColor;
                    if (faultyDataCellForeColor != Color.Empty) e.Style.TextColor = faultyDataCellForeColor;
                    break;
                case ProcessedDataType.MANUAL:
                    if (manualDataCellBackColor != Color.Empty) e.Style.BackColor = manualDataCellBackColor;
                    if (manualDataCellForeColor != Color.Empty) e.Style.TextColor = manualDataCellForeColor;
                    break;
                case ProcessedDataType.NORMAL:
                    break;
            }
        }
        private void manualDataGrid_QueryRowStyle(object sender, Syncfusion.WinForms.DataGrid.Events.QueryRowStyleEventArgs e)
        {
            e.Style.HorizontalAlignment = HorizontalAlignment.Center;
        }
        private void manualDataGrid_QueryProgressBarCellStyle(object sender, Syncfusion.WinForms.DataGrid.Events.QueryProgressBarCellStyleEventArgs e)
        {
            if (!e.Column.HeaderText.Contains(" Score"))
                return;

            var dataObject = (dynamic)e.Record;
            AnswerKey ansKey = Functions.GetProperty(dataObject, "AnswerKey");
            if (ansKey == null)
                return;

            int maximum = ansKey.GetPaper.GetCorrectOptionValue * ansKey.GetKey.Length;
            e.Maximum = maximum == 0 ? 100 : maximum;

            e.Style.ProgressTextColor = (CurrentTheme == Themes.BLACK || CurrentTheme == Themes.DARK_GRAY) ? Color.White : e.Style.ProgressTextColor;
            e.Style.BackgroundColor = (CurrentTheme == Themes.DARK_GRAY) ? Color.FromArgb(210, 210, 210) : e.Style.BackgroundColor;
            //mainDataGrid.View.
        }

        private void faultyDataGrid_SelectionChanged(object sender, Syncfusion.WinForms.DataGrid.Events.SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0)
            {
                curOptionRects.Clear();
                curMarkedOptionRects.Clear();

                dataImageBox.Image = null;
                return;
            }

            dynamic lastItemData = (dynamic)e.AddedItems.Last();
            //if (e.AddedItems.RowType != Syncfusion.WinForms.DataGrid.Enums.RowType.DefaultRow)
            //    return;

            SelectedProcessedDataRow = (ProcessedDataRow)lastItemData.DataRowObject;
            UpdateImageSelection(SelectedProcessedDataRow.Value, GetLocateOptionsToggle);
        }
        private void faultyDataGrid_QueryCellStyle(object sender, Syncfusion.WinForms.DataGrid.Events.QueryCellStyleEventArgs e)
        {
            dynamic dataObject = (dynamic)e.DataRow.RowData;
            string colText = e.Column.HeaderText;
            if (!GridCellsRepresentation.ContainsKey(colText))
                return;

            ProcessedDataType fieldDataType = ProcessedDataType.NORMAL;

            try
            {
                (int entryIndex, int fieldIndex) cellRepresentation = GridCellsRepresentation[colText];
                fieldDataType = dataObject.DataRowObject.GetProcessedDataEntries[cellRepresentation.entryIndex].DataEntriesResultType[cellRepresentation.fieldIndex];
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}\nColumn: {e.Column.HeaderText} - {e.DisplayText}\nRow: {e.RowIndex}");
            }

            switch (fieldDataType)
            {
                case ProcessedDataType.INCOMPATIBLE:
                    if (incompatibleDataCellBackColor != Color.Empty) e.Style.BackColor = incompatibleDataCellBackColor;
                    if (incompatibleDataCellForeColor != Color.Empty) e.Style.TextColor = incompatibleDataCellForeColor;
                    break;
                case ProcessedDataType.FAULTY:
                    if (faultyDataCellBackColor != Color.Empty) e.Style.BackColor = faultyDataCellBackColor;
                    if (faultyDataCellForeColor != Color.Empty) e.Style.TextColor = faultyDataCellForeColor;
                    break;
                case ProcessedDataType.MANUAL:
                    if (manualDataCellBackColor != Color.Empty) e.Style.BackColor = manualDataCellBackColor;
                    if (manualDataCellForeColor != Color.Empty) e.Style.TextColor = manualDataCellForeColor;
                    break;
                case ProcessedDataType.NORMAL:
                    break;
            }
        }
        private void faultyDataGrid_QueryRowStyle(object sender, Syncfusion.WinForms.DataGrid.Events.QueryRowStyleEventArgs e)
        {
            e.Style.HorizontalAlignment = HorizontalAlignment.Center;
        }
        private void faultyDataGrid_QueryProgressBarCellStyle(object sender, Syncfusion.WinForms.DataGrid.Events.QueryProgressBarCellStyleEventArgs e)
        {
            if (!e.Column.HeaderText.Contains(" Score"))
                return;

            var dataObject = (dynamic)e.Record;
            AnswerKey ansKey = Functions.GetProperty(dataObject, "AnswerKey");
            if (ansKey == null)
                return;

            int maximum = ansKey.GetPaper.GetCorrectOptionValue * ansKey.GetKey.Length;
            e.Maximum = maximum == 0 ? 100 : maximum;

            e.Style.ProgressTextColor = (CurrentTheme == Themes.BLACK || CurrentTheme == Themes.DARK_GRAY) ? Color.White : e.Style.ProgressTextColor;
            e.Style.BackgroundColor = (CurrentTheme == Themes.DARK_GRAY) ? Color.FromArgb(210, 210, 210) : e.Style.BackgroundColor;
            //mainDataGrid.View.
        }

        private void incompatibleDataGrid_SelectionChanged(object sender, Syncfusion.WinForms.DataGrid.Events.SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0)
            {
                curOptionRects.Clear();
                curMarkedOptionRects.Clear();

                dataImageBox.Image = null;
                return;
            }

            dynamic lastItemData = (dynamic)e.AddedItems.Last();
            //if (e.AddedItems.RowType != Syncfusion.WinForms.DataGrid.Enums.RowType.DefaultRow)
            //    return;

            SelectedProcessedDataRow = (ProcessedDataRow)lastItemData.DataRowObject;
            UpdateImageSelection(SelectedProcessedDataRow.Value, GetLocateOptionsToggle);
        }
        private void incompatibleDataGrid_QueryCellStyle(object sender, Syncfusion.WinForms.DataGrid.Events.QueryCellStyleEventArgs e)
        {
            dynamic dataObject = (dynamic)e.DataRow.RowData;
            string colText = e.Column.HeaderText;
            if (!GridCellsRepresentation.ContainsKey(colText))
                return;

            ProcessedDataType fieldDataType = ProcessedDataType.NORMAL;

            try
            {
                (int entryIndex, int fieldIndex) cellRepresentation = GridCellsRepresentation[colText];
                fieldDataType = dataObject.DataRowObject.GetProcessedDataEntries[cellRepresentation.entryIndex].DataEntriesResultType[cellRepresentation.fieldIndex];
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}\nColumn: {e.Column.HeaderText} - {e.DisplayText}\nRow: {e.RowIndex}");
            }

            switch (fieldDataType)
            {
                case ProcessedDataType.INCOMPATIBLE:
                    if (incompatibleDataCellBackColor != Color.Empty) e.Style.BackColor = incompatibleDataCellBackColor;
                    if (incompatibleDataCellForeColor != Color.Empty) e.Style.TextColor = incompatibleDataCellForeColor;
                    break;
                case ProcessedDataType.FAULTY:
                    if (faultyDataCellBackColor != Color.Empty) e.Style.BackColor = faultyDataCellBackColor;
                    if (faultyDataCellForeColor != Color.Empty) e.Style.TextColor = faultyDataCellForeColor;
                    break;
                case ProcessedDataType.MANUAL:
                    if (manualDataCellBackColor != Color.Empty) e.Style.BackColor = manualDataCellBackColor;
                    if (manualDataCellForeColor != Color.Empty) e.Style.TextColor = manualDataCellForeColor;
                    break;
                case ProcessedDataType.NORMAL:
                    break;
            }
        }
        private void incompatibleDataGrid_QueryRowStyle(object sender, Syncfusion.WinForms.DataGrid.Events.QueryRowStyleEventArgs e)
        {
            e.Style.HorizontalAlignment = HorizontalAlignment.Center;
        }
        private void incompatibleDataGrid_QueryProgressBarCellStyle(object sender, Syncfusion.WinForms.DataGrid.Events.QueryProgressBarCellStyleEventArgs e)
        {
            if (!e.Column.HeaderText.Contains(" Score"))
                return;

            var dataObject = (dynamic)e.Record;
            AnswerKey ansKey = Functions.GetProperty(dataObject, "AnswerKey");
            if (ansKey == null)
                return;

            int maximum = ansKey.GetPaper.GetCorrectOptionValue * ansKey.GetKey.Length;
            e.Maximum = maximum == 0 ? 100 : maximum;

            e.Style.ProgressTextColor = (CurrentTheme == Themes.BLACK || CurrentTheme == Themes.DARK_GRAY) ? Color.White : e.Style.ProgressTextColor;
            e.Style.BackgroundColor = (CurrentTheme == Themes.DARK_GRAY) ? Color.FromArgb(210, 210, 210) : e.Style.BackgroundColor;
            //mainDataGrid.View.
        }

        private void DataImageBox_Paint(object sender, PaintEventArgs e)
        {
            if (curOptionRects.Count > 0)
            {
                for (int i = 0; i < curOptionRects.Count; i++)
                {
                    Functions.DrawBox(e.Graphics, dataImageBox.GetOffsetRectangle(curOptionRects[i]), dataImageBox.ZoomFactor, primaryRectColor, 1);
                }
            }
            if (curMarkedOptionRects.Count > 0)
            {
                for (int i = 0; i < curMarkedOptionRects.Count; i++)
                {
                    Functions.DrawBox(e.Graphics, dataImageBox.GetOffsetRectangle(curMarkedOptionRects[i]), dataImageBox.ZoomFactor, secondaryRectColor, 1);
                }
            }
        }
        #endregion
        #region Data Tab
        private void ExportExcelToolStripBtn_Click(object sender, EventArgs e)
        {
            ExportExcel();
        }
        private void ReadingTabStatusBar_SizeChanged(object sender, EventArgs e)
        {
            statusGeneralPanel.Width = Size.Width-35;
            statusPanelStatusLabel.Width = statusTextStatusPanel.Width - 48;
        }




        #endregion

        #endregion

        #endregion

        private void locateOptionsToolStripBtn_Click(object sender, EventArgs e)
        {
            GetLocateOptionsToggle = !GetLocateOptionsToggle;
            locateOptionsToolStripBtn.CheckState = GetLocateOptionsToggle? CheckState.Checked : CheckState.Unchecked;
        }

        private void stopReadingToolStripBtn_Click(object sender, EventArgs e)
        {
            MainProcessingManager.StopProcessing();

            startReadingToolStripBtn.Text = "Start";
            startReadingToolStripBtn.Image = Properties.Resources.startBtnIcon_ReadingTab;
        }

        private void manualDataGrid_CurrentCellEndEdit(object sender, Syncfusion.WinForms.DataGrid.Events.CurrentCellEndEditEventArgs e)
        {

        }

        private void manualDataGrid_CellDoubleClick(object sender, Syncfusion.WinForms.DataGrid.Events.CellClickEventArgs e)
        {
            using (DataEditForm dataEditForm = new DataEditForm(OnTemplateConfigs[e.DataColumn.ColumnIndex], e.DataRow.RowData, e.DataColumn.GridColumn.MappingName, e.DataColumn.ColumnIndex))
                dataEditForm.ShowDialog();
        }

        private void manualDataGrid_MouseClick(object sender, MouseEventArgs e)
        {
            
        }
            
        private void showInExplorerContextBtn_Click(object sender, EventArgs e)
        {
            dynamic selectedRow = manualDataGrid.SelectedItems.Last();

            string path = selectedRow.DataRowObject.RowSheetPath;
            string cmd = "explorer.exe";
            string arg = $"/select,  {path}";

            Process.Start(cmd, arg);
        }

        private void normallyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<dynamic> selectedRows = manualDataGrid.SelectedItems.ToList();

            if (MainProcessingManager.IsProcessing && !MainProcessingManager.IsPaused)
                MainProcessingManager.PauseProcessing();

            MainProcessingManager.ReprocessData(selectedRows, ProcessingManager.RereadType.NORMAL);

            if (MainProcessingManager.IsProcessing)
                MainProcessingManager.ResumeProcessing();
        }

        private void manualDataGrid_CellClick(object sender, Syncfusion.WinForms.DataGrid.Events.CellClickEventArgs e)
        {
            if (e.MouseEventArgs.Button == MouseButtons.Right)
            {
                manualContext.Show(Cursor.Position);
            }
        }

        private void rotateXBy90ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<dynamic> selectedRows = manualDataGrid.SelectedItems.ToList();

            if (MainProcessingManager.IsProcessing && !MainProcessingManager.IsPaused)
                MainProcessingManager.PauseProcessing();

            MainProcessingManager.ReprocessData(selectedRows, ProcessingManager.RereadType.ROTATE_C_90);

            if (MainProcessingManager.IsProcessing)
                MainProcessingManager.ResumeProcessing();
        }

        private void rotateX180ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<dynamic> selectedRows = manualDataGrid.SelectedItems.ToList();

            if (MainProcessingManager.IsProcessing && !MainProcessingManager.IsPaused)
                MainProcessingManager.PauseProcessing();

            MainProcessingManager.ReprocessData(selectedRows, ProcessingManager.RereadType.ROTATE_AC_90);

            if (MainProcessingManager.IsProcessing)
                MainProcessingManager.ResumeProcessing();
        }

        private void rotateY90ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<dynamic> selectedRows = manualDataGrid.SelectedItems.ToList();

            if (MainProcessingManager.IsProcessing && !MainProcessingManager.IsPaused)
                MainProcessingManager.PauseProcessing();

            MainProcessingManager.ReprocessData(selectedRows, ProcessingManager.RereadType.ROTATE_180);

            if (MainProcessingManager.IsProcessing)
                MainProcessingManager.ResumeProcessing();
        }
    }
}