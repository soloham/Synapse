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

namespace Synapse
{
    public partial class SynapseMain : RibbonForm
    {
        #region Enums

        public enum StatusState
        {
            Red, Yellow, Green
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

        #endregion

        #region Variables
        private SynchronizationContext synchronizationContext;

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

        private SheetsList loadedSheetsData = new SheetsList();
        private List<ProcessedDataRow> processedData = new List<ProcessedDataRow>();
        private ObservableCollection<dynamic> processedDataSource = new ObservableCollection<dynamic>();

        private List<string> gridColumns = new List<string>();
        private List<string> usedNonCollectiveDataLabels = new List<string>();

        private List<RectangleF> curOptionRects = new List<RectangleF>();
        private List<RectangleF> curMarkedOptionRects = new List<RectangleF>();
        Color primaryRectColor = Color.FromArgb(120, Color.DarkSlateGray);
        Color secondaryRectColor = Color.FromArgb(180, Color.MediumTurquoise);
        #endregion
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
            synchronizationContext = SynchronizationContext.Current;
            synapseMain = this;

            SynapseMain.currentTemplate = currentTemplate;

            Awake();
        }
        #endregion

        #region Public Methods
        public void AddRegionAsOMR(RectangleF region)
        {
            RectangleF configAreaRect = region;
            ConfigArea configArea = new ConfigArea(configAreaRect, (Bitmap)templateImageBox.GetSelectedImage());
            OMRConfigurationForm configurationForm = new OMRConfigurationForm(configArea.ConfigBitmap);
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

        public void UpdateStatus(string status)
        {
            synchronizationContext.Send(new SendOrPostCallback((object target) =>
            {
                statusPanelStatusLabel.Text = status;
            }), null);
        }
        #endregion

        #region Private Methods
        #region Main
        private async void Awake()
        {
            //Pre-Ops
            //-User Interface Setup
            //--Ribbon Tabs Setup
            readingTabPanel.Dock = DockStyle.Fill;
            configTabPanel.Dock = DockStyle.Fill;
            ribbonControl.SelectedTab = configToolStripTabItem;

            mainDockingManager.SetEnableDocking(configPropertiesPanel, true);
            mainDockingManager.DockControlInAutoHideMode(configPropertiesPanel, DockingStyle.Right, 400);
            mainDockingManager.SetMenuButtonVisibility(configPropertiesPanel, false);
            mainDockingManager.SetDockLabel(configPropertiesPanel, "Properties");

            mainDockingManager.SetEnableDocking(dataImageBoxPanel, true);
            mainDockingManager.DockControlInAutoHideMode(dataImageBoxPanel, DockingStyle.Right, 450);
            mainDockingManager.SetMenuButtonVisibility(dataImageBoxPanel, false);
            mainDockingManager.SetDockLabel(dataImageBoxPanel, "Image");

            OMRRegionColorStates = new ColorStates(Color.FromArgb(55, Color.Firebrick), Color.FromArgb(95, Color.Firebrick), Color.FromArgb(85, Color.Firebrick), Color.FromArgb(110, Color.Firebrick));
            ICRRegionColorStates = new ColorStates(Color.FromArgb(55, Color.SlateGray), Color.FromArgb(95, Color.SlateGray), Color.FromArgb(85, Color.SlateGray), Color.FromArgb(110, Color.SlateGray));

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

            MainProcessingManager = new ProcessingManager(GetCurrentTemplate);
            MainProcessingManager.OnDataSourceUpdated += MainProcessingManager_OnDataSourceUpdated;
        }

        private void MainProcessingManager_OnDataSourceUpdated(object sender, ProcessedDataType e)
        {
            SfDataGrid dataGrid = mainDataGrid;
            int rowCount = 0;
            switch (e)
            {
                case ProcessedDataType.INCOMPATIBLE:
                    //dataGrid = incompatibleDataGrid;
                    rowCount = MainProcessingManager.GetTotalIncompatibleProcessedData;
                    if (rowCount == 0)
                        return;

                    if (!incompatibleDataStatusPanel.Visible)
                        incompatibleDataStatusPanel.Visible = true;
                    totalIncompatibleDataLabel.Text = rowCount + "";
                    break;
                case ProcessedDataType.FAULTY:
                    //dataGrid = faultyDataGrid;
                    rowCount = MainProcessingManager.GetTotalFaultyProcessedData;
                    if (rowCount == 0)
                        return;

                    if (!faultyDataTypeStatusPanel.Visible)
                        faultyDataTypeStatusPanel.Visible = true;
                    totalFaultyDataLabel.Text = rowCount + "";
                    break;
                case ProcessedDataType.MANUAL:
                    //dataGrid = manualDataGrid;
                    rowCount = MainProcessingManager.GetTotalManualProcessedData;
                    if (rowCount == 0)
                        return;

                    if (!manualDataTypeStatusPanel.Visible)
                        manualDataTypeStatusPanel.Visible = true;
                    totalManualDataLabel.Text = rowCount + "";
                    break;
                case ProcessedDataType.NORMAL:
                    //dataGrid = mainDataGrid;
                    rowCount = MainProcessingManager.GetTotalProcessedData;
                    if (rowCount == 0)
                        return;

                    if (!normalDataTypePanel.Visible)
                        normalDataTypePanel.Visible = true;
                    totalMainDataLabel.Text = rowCount + "";
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
        private async void StartReadingToolStripBtn_Click(object sender, EventArgs e)
        {
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
            int totalSheets = loadedSheetsData.GetSheetsPath.Length;
            TimeSpan totalTimeTaken = TimeSpan.Zero;
            double finalAverage = 0;
            Action<ProcessedDataRow, double, double> OnSheetsProcessed = (ProcessedDataRow dataRow, double runningAverage, double runningTotal) =>
            {
                int curIndex = MainProcessingManager.GetCurProcessingIndex+1;
                int progressValue = (int)(curIndex / (float)totalSheets * 100);
                processingProgressBar.Value = progressValue;
                if(processingProgressBar.Value > 47) 
                    processingProgressBar.FontColor = Color.White;  

                string timeLeft = TimeSpan.FromMilliseconds(runningAverage * (totalSheets - curIndex)).ToString(@"hh\:mm\:ss");
                processingTimeLeftLabel.Text = "TOTAL TIME: " + timeLeft;

                double spsAvg = 1/(runningAverage / 1000);
                string status = $"[{curIndex}/{totalSheets}] Processing...     AVG: {Math.Round(spsAvg, 1)} Sheets per second.";
                UpdateStatus(status);

                if (curIndex == totalSheets)
                {
                    totalTimeTaken = TimeSpan.FromMilliseconds(runningTotal);
                    finalAverage = runningAverage / 1000;
                    finalAverage = Math.Round(finalAverage, 2);
                }
            };
            processingProgressBar.FontColor = Color.Black;

            progressStatusTablePanel.Visible = true;
            await MainProcessingManager.StartProcessing(keepData, OnSheetsProcessed, gridColumns);
            progressStatusTablePanel.Visible = false;

            processingTimeLeftLabel.Text = "TOTAL TIME: 00:00:00";
            processingProgressBar.Value = 0;

            UpdateStatus($"Processing Complete: {totalSheets} sheets in {totalTimeTaken.ToString(@"hh\:mm\:ss")} at an average of {finalAverage} seconds per sheet.");
        }

        private void GenerateGridColumns()
        {
            gridColumns.Clear();
            usedNonCollectiveDataLabels.Clear();
            mainDataGrid.AutoGenerateColumns = false;
            mainDataGrid.Columns.Clear();

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

                                gridColumns.Add(omrCol.HeaderText);
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

                                    gridColumns.Add(omrIndCol.HeaderText);
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

                                        gridColumns.Add(omrCom2Col.HeaderText);
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

                                        gridColumns.Add(omrIndCol.HeaderText);
                                    }
                                }
                                break;
                        }
                        break;
                    case MainConfigType.BARCODE:
                        OMRConfiguration obrConfiguration = (OMRConfiguration)allConfigs[i];

                        GridTextColumn obrCol = new GridTextColumn();
                        obrCol.MappingName = obrConfiguration.Title;
                        obrCol.HeaderText = obrConfiguration.Title;
                        mainDataGrid.Columns.Add(obrCol);

                        gridColumns.Add(obrCol.HeaderText);
                        break;
                    case MainConfigType.ICR:
                        ICRConfiguration icrConfiguration = (ICRConfiguration)allConfigs[i];

                        GridTextColumn icrCol = new GridTextColumn();
                        icrCol.MappingName = icrConfiguration.Title;
                        icrCol.HeaderText = icrConfiguration.Title;
                        mainDataGrid.Columns.Add(icrCol);

                        gridColumns.Add(icrCol.HeaderText);
                        break;
                 }
            }
        }
        internal void InitializeMainDataGrid(List<ProcessedDataEntry> processedDataEntries, ObservableCollection<dynamic> processedDataSource)
        {
            if (this.gridColumns.Count > 0)
            {
                var gridColumns = mainDataGrid.Columns;
                int dataDynamicTotalColumns = 0;
                for (int i = 0; i < processedDataEntries.Count; i++)
                {
                    dataDynamicTotalColumns += processedDataEntries[i].GetDataValues.Length;
                }
                if (gridColumns.Count != dataDynamicTotalColumns)
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
                        mainDataGridPager.DataSource = processedDataSource;
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
                mainDataGridPager.DataSource = processedDataSource;
            }
            mainDataGrid.DataSource = mainDataGridPager.PagedSource;
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
                var excelEngine = mainDataGrid.ExportToExcel(mainDataGrid.View, options);
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

            configTabPanel.Visible = true;
            configTabPanel.BringToFront();
            //readingTabPanel.Visible = false;

            mainDockingManager.SetDockVisibility(configPropertiesPanel, true);
            mainDockingManager.SetDockVisibility(dataImageBoxPanel, false);
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
        private void TemplateImageBox_MouseMove(object sender, MouseEventArgs e)
        {
            curImageMouseLoc = e.Location;

            if (ConfigurationStatus == StatusState.Green)
            {
                for (int i = 0; i < OnTemplateConfigs.Count; i++)
                {
                    RectangleF offsetRect = OnTemplateConfigs[i].OffsetRectangle;

                    if (offsetRect.Contains(e.Location))
                    {
                        InterestedTemplateConfig = OnTemplateConfigs[i];

                        if (SelectedTemplateConfig == InterestedTemplateConfig)
                            return;

                        switch (OnTemplateConfigs[i].Configuration.GetMainConfigType)
                        {
                            case MainConfigType.OMR:
                                OnTemplateConfigs[i].ColorStates.CurrentColor = OMRRegionColorStates.HighlightedColor;
                                IsMouseOverRegion = true;
                                break;
                            case MainConfigType.BARCODE:
                                OnTemplateConfigs[i].ColorStates.CurrentColor = OBRRegionColorStates.HighlightedColor;
                                IsMouseOverRegion = true;
                                break;
                            case MainConfigType.ICR:
                                OnTemplateConfigs[i].ColorStates.CurrentColor = ICRRegionColorStates.HighlightedColor;
                                IsMouseOverRegion = true;
                                break;
                        }
                        
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

            //readingTabPanel.Visible = true;
            configTabPanel.Visible = false;
            configTabPanel.SendToBack();

            mainDockingManager.SetDockVisibility(dataImageBoxPanel, true);
            mainDockingManager.SetDockVisibility(configPropertiesPanel, false);

            GenerateGridColumns();
        }
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

        private void MainDataGrid_SelectionChanged(object sender, Syncfusion.WinForms.DataGrid.Events.SelectionChangedEventArgs e)
        {

        }
        private void MainDataGrid_CellClick(object sender, Syncfusion.WinForms.DataGrid.Events.CellClickEventArgs e)
        {
            if (e.DataRow.RowType != Syncfusion.WinForms.DataGrid.Enums.RowType.DefaultRow)
                return;

            var dataObject = (dynamic)e.DataRow.RowData;
            var processedDataRow = (ProcessedDataRow)dataObject.DataRowObject;

            dataImageBox.Image = processedDataRow.GetAlignedImage().Bitmap;

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
        private void MainDataGrid_QueryRowStyle(object sender, Syncfusion.WinForms.DataGrid.Events.QueryRowStyleEventArgs e)
        {
            e.Style.HorizontalAlignment = HorizontalAlignment.Center;
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
    }
}