using static Synapse.Core.Configurations.ConfigurationBase;


namespace Synapse
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Management;
    using System.Net;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Forms;

    using Emgu.CV;
    using Emgu.CV.CvEnum;

    using Synapse.Controls;
    using Synapse.Controls.Answer_Key;
    using Synapse.Core;
    using Synapse.Core.Configurations;
    using Synapse.Core.Engines;
    using Synapse.Core.Engines.Data;
    using Synapse.Core.Keys;
    using Synapse.Core.Managers;
    using Synapse.Core.Templates;
    using Synapse.DeCore.Engines.Data;
    using Synapse.Modules;
    using Synapse.Properties;
    using Synapse.Shared.Enums;
    using Synapse.Shared.Utilities.Extensions;
    using Synapse.Shared.Utilities.Objects;
    using Synapse.Utilities;
    using Synapse.Utilities.Attributes;
    using Synapse.Utilities.Enums;
    using Synapse.Utilities.Memory;
    using Synapse.Utilities.Objects;

    using Syncfusion.Windows.Forms;
    using Syncfusion.Windows.Forms.Tools;
    using Syncfusion.WinForms.DataGrid;
    using Syncfusion.WinForms.DataGrid.Enums;
    using Syncfusion.WinForms.DataGrid.Events;
    using Syncfusion.WinForms.DataGridConverter;
    using Syncfusion.XlsIO;

    using Action = System.Action;
    using Orientation = System.Windows.Forms.Orientation;

    public partial class SynapseMain : RibbonForm
    {
        public static bool IsMainDashing { get; set; }

        #region Enums

        public enum StatusState
        {
            Red,
            Yellow,
            Green
        }

        public enum Themes
        {
            [EnumDescription("White")] WHITE,
            [EnumDescription("Colorful")] COLORFUL,
            [EnumDescription("Dark Gray")] DARK_GRAY,
            [EnumDescription("Black")] BLACK
        }

        #endregion

        #region Objects

        public class OnTemplateConfig
        {
            public ConfigurationBase Configuration;
            public RectangleF OffsetRectangle;
            public ColorStates ColorStates;

            public OnTemplateConfig(ConfigurationBase configurationBase, ColorStates colorStates)
            {
                Configuration = configurationBase;
                ColorStates = colorStates;

                OffsetRectangle = configurationBase.GetConfigArea.ConfigRect;
            }
        }

        #endregion

        #region Constants

        private const string DATAPOINT_FILTER = "Data Point (*.sdp) | *.sdp";

        #endregion

        #region Properties

        public static SynapseMain GetSynapseMain { get; private set; }

        public static Template GetCurrentTemplate
        {
            get => currentTemplate;
            set { }
        }

        private static Template currentTemplate;

        public StatusState TemplateStatus
        {
            get => templateStatus;
            set
            {
                templateStatus = value;
                this.ToggleTemplateStatus(value);
            }
        }

        private StatusState templateStatus;

        public StatusState ConfigurationStatus
        {
            get => configurationStatus;
            set
            {
                configurationStatus = value;
                this.ToggleConfigurationDataStatus(value);
            }
        }

        private StatusState configurationStatus;

        public StatusState AIStatus
        {
            get => aiStatus;
            set
            {
                aiStatus = value;
                this.ToggleAIStatus(value);
            }
        }

        private StatusState aiStatus;

        public ProcessedDataRow? SelectedProcessedDataRow { get; set; }

        #endregion

        #region Variables

        private readonly SynchronizationContext synchronizationContext;

        #region BackStage

        public Themes CurrentTheme;

        #endregion

        #region Configuration Panel

        private PointF curImageMouseLoc;

        private List<OnTemplateConfig> OnTemplateConfigs = new List<OnTemplateConfig>();

        private OnTemplateConfig SelectedTemplateConfig
        {
            get => selectedTemplateConfig;
            set
            {
                selectedTemplateConfig = value;
                this.SelectedTemplateConfigChanged(value);
            }
        }

        private OnTemplateConfig selectedTemplateConfig;
        private OnTemplateConfig InterestedTemplateConfig;

        public ColorStates OMRRegionColorStates;
        public ColorStates OBRRegionColorStates;
        public ColorStates ICRRegionColorStates;

        public bool IsMouseOverRegion
        {
            get => isMouseOverRegion;
            set
            {
                isMouseOverRegion = value;
                this.ToggleMouseOverRegion(value);
            }
        }

        private bool isMouseOverRegion;

        public bool IsMouseDownRegion
        {
            get => isMouseDownRegion;
            set
            {
                isMouseDownRegion = value;
                this.ToggleMouseDownRegion(value);
            }
        }

        private bool isMouseDownRegion;

        public bool IsMouseUpRegion
        {
            get => isMouseUpRegion;
            set
            {
                isMouseUpRegion = value;
                this.ToggleMouseUpRegion(value);
            }
        }

        private bool isMouseUpRegion;

        #endregion

        #region Reading Panel

        public ProcessingManager MainProcessingManager { get; set; }

        public bool GetLocateRegionsToggle
        {
            get => locateRegionsToggle;
            private set
            {
                locateRegionsToggle = value;
                curOptionRects.Clear();
                curMarkedOptionRects.Clear();
                if (this.SelectedProcessedDataRow.HasValue)
                {
                    this.UpdateImageSelection(this.SelectedProcessedDataRow.Value, value);
                }
            }
        }

        private bool locateRegionsToggle;

        private SheetsList loadedSheetsData = new SheetsList(UpdateMainStatus);
        private List<ProcessedDataRow> processedData = new List<ProcessedDataRow>();
        private ObservableCollection<dynamic> processedDataSource = new ObservableCollection<dynamic>();

        private readonly List<string> gridColumns = new List<string>();
        private readonly List<string> gridConfigOnlyColumns = new List<string>();
        private readonly List<string> usedNonCollectiveDataLabels = new List<string>();

        public Dictionary<string, (int entryIndex, int fieldIndex)> GridCellsRepresentation =
            new Dictionary<string, (int entryIndex, int fieldIndex)>();

        private readonly List<RectangleF> curOptionRects = new List<RectangleF>();
        private readonly List<RectangleF> curBackOptionRects = new List<RectangleF>();
        private readonly List<RectangleF> curMarkedOptionRects = new List<RectangleF>();
        private readonly List<RectangleF> curMarkedBackOptionRects = new List<RectangleF>();
        private readonly Color primaryRectColor = Color.FromArgb(120, Color.DarkSlateGray);
        private readonly Color secondaryRectColor = Color.FromArgb(180, Color.MediumTurquoise);

        #endregion

        private NumberFormatInfo NumberFormatInfo;

        private readonly Color editedDataCellBackColor = Color.OldLace;
        private readonly Color editedDataCellForeColor = Color.Empty;

        private readonly Color manualDataCellBackColor = Color.FromArgb(255, 250, 102);
        private readonly Color manualDataCellForeColor = Color.Empty;

        private readonly Color faultyDataCellBackColor = Color.OrangeRed;
        private readonly Color faultyDataCellForeColor = Color.White;

        private Color incompatibleDataCellBackColor;
        private Color incompatibleDataCellForeColor;

        private readonly Color editedDataRowBackColor = Color.LightGoldenrodYellow;
        private readonly Color editedDataRowForeColor = Color.Empty;

        private readonly Color manualDataRowBackColor = Color.FromArgb(255, 250, 224);
        private readonly Color manualDataRowForeColor = Color.Empty;

        private Color faultyDataRowBackColor;
        private Color faultyDataRowForeColor;

        private readonly Color incompatibleDataRowBackColor = Color.Red;
        private readonly Color incompatibleDataRowForeColor = Color.White;

        #endregion

        #region Events

        public event EventHandler<Mat> OnTemplateLoadedEvent;

        public event EventHandler<StatusState> OnTemplateStateChangedEvent;
        public event EventHandler<StatusState> OnConfigurationDataStateChangedEvent;
        public event EventHandler<StatusState> OnAIStateChangedEvent;

        #endregion

        #region Static Methods

        public static void RunTemplate(Template template)
        {
            var synapseMain = new SynapseMain(template);
            synapseMain.Text = "Synapse - " + template.GetTemplateName;
            Program.DefaultSplashScreen.Hide();
            synapseMain.Show();
        }

        public static void UpdateMainStatus(string status)
        {
            GetSynapseMain.UpdateStatus(status);
        }

        #endregion

        #region public Methods

        public async
            Task<(string systemKey, string processorID, string driveSignature, string biosSerial, string biosVersion)>
            GetHardwareID()
        {
            var bytes = Array.Empty<byte>();
            var hashedBytes = Array.Empty<byte>();
            var sb = new StringBuilder();

            var cpuID = string.Empty;
            var driveSig = string.Empty;
            var biosSerial = string.Empty;
            var biosVersion = string.Empty;

            var GetIDTask = Task.Run(() =>
            {
                ManagementObjectSearcher searcher;
                ManagementObjectCollection collection;

                #region ProcessorID

                searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Processor");
                collection = searcher.Get();

                foreach (ManagementObject processor in collection)
                    if (cpuID == "")
                    {
                        //Get only the first CPU's ID
                        cpuID = processor.Properties["processorID"].Value.ToString();
                        break;
                    }

                sb.Append(cpuID.Trim() + "-");

                #endregion

                #region DriveSerial

                var hdCollection = new List<HardDrive>();
                searcher = new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive");
                collection = searcher.Get();

                foreach (ManagementObject wmi_HD in collection)
                {
                    var mediaType = wmi_HD["MediaType"].ToString();
                    if (wmi_HD["InterfaceType"].ToString() == "USB" || mediaType == "Removable Media")
                    {
                        continue;
                    }

                    if (mediaType.IndexOf("fixed", StringComparison.OrdinalIgnoreCase) == -1)
                    {
                        continue;
                    }

                    var hd = new HardDrive();

                    // get the hardware serial no.
                    if (wmi_HD["SerialNumber"] == null)
                    {
                        hd.SerialNo = "None";
                    }
                    else
                    {
                        hd.SerialNo = wmi_HD["SerialNumber"].ToString();
                    }

                    if (wmi_HD["Signature"] == null)
                    {
                        hd.Signature = "None";
                    }
                    else
                    {
                        hd.Signature = wmi_HD["Signature"].ToString();
                    }

                    hdCollection.Add(hd);
                }

                driveSig = hdCollection[0].Signature.Trim();
                sb.Append(driveSig + "-");

                #endregion

                #region BIOS

                searcher = new ManagementObjectSearcher("SELECT * FROM Win32_BIOS");
                collection = searcher.Get();
                foreach (ManagementObject wmi_BIOS in collection)
                {
                    if (wmi_BIOS["SerialNumber"] != null)
                    {
                        biosSerial = wmi_BIOS["SerialNumber"].ToString();
                    }

                    if (wmi_BIOS["Version"] != null)
                    {
                        biosVersion = wmi_BIOS["Version"].ToString();
                    }
                }

                sb.Append(string.IsNullOrEmpty(biosSerial) ? biosVersion : biosSerial);

                #endregion
            });
            Task.WaitAll(GetIDTask);
            bytes = Encoding.UTF8.GetBytes(sb.ToString());
            using (var sha256 = SHA256.Create())
            {
                hashedBytes = sha256.ComputeHash(bytes);
            }

            var hashedString = Convert.ToBase64String(hashedBytes);
            return (await Task.FromResult(hashedString.Substring(25)), cpuID, driveSig, biosSerial, biosVersion);
        }

        public SynapseMain(Template currentTemplate)
        {
            this.Hide();

            Program.DefaultSplashScreen.SetSplashText("Initializing Interface...");
            var hardwareID = this.GetHardwareID().Result;
            using (var wc = new WebClient())
            {
                #region PublicIP

                var publicIP = wc.DownloadString("http://icanhazip.com");

                #endregion

                string contents;
                try
                {
                    #region VerifySystem

                    var verifyUri =
                        $"http://synapse-terminal.cf/VerifySystem?SystemKey={hardwareID.systemKey}&MachineName={Environment.MachineName}&DriveSerial={hardwareID.driveSignature}&ProcessorID={hardwareID.processorID}&PublicIP={publicIP.Trim()}&BIOS={(string.IsNullOrEmpty(hardwareID.biosSerial) ? hardwareID.biosVersion : hardwareID.biosSerial)}";
                    contents = wc.DownloadString(new Uri(verifyUri));
                    if (contents == "Verified")
                    {
                        IsMainDashing = true;
                    }
                    else if (contents == "Not Found")
                    {
                        #region AddSystem

                        var uriString =
                            $"https://synapse-terminal.cf/AddSystem?SystemKey={hardwareID.systemKey}&MachineName={Environment.MachineName}&DriveSerial={hardwareID.driveSignature}&ProcessorID={hardwareID.processorID}&PublicIP={publicIP.Trim()}&BIOS={(string.IsNullOrEmpty(hardwareID.biosSerial) ? hardwareID.biosVersion : hardwareID.biosSerial)}";
                        contents = wc.DownloadString(new Uri(uriString));
                        if (contents == "Success")
                        {
                            IsMainDashing = false;
                        }
                        else
                        {
                            IsMainDashing = false;
                        }

                        #endregion
                    }
                    else
                    {
                        IsMainDashing = false;
                    }

                    Program.ValidateLicenseKey(LSTM.LoadLicenseKey(), isValid =>
                    {
                        if (!isValid)
                        {
                            Messages.ShowError("Invalid License Key.");
                            Application.Exit();
                        }
                    });
                }
                catch
                {
                    IsMainDashing = true;
                }

                #endregion
            }

            IsMainDashing = true;
            this.InitializeComponent();

            Program.DefaultSplashScreen.SetSplashText("Setting Up Components...");

            #region SetupComponents

            ribbonControl.Height = 220;

            templateConfigToolStrip.Items.AddRange(new ToolStripItem[]
            {
                templateConfigStatusToolStrip,
                toolStripSeparator1,
                templateToolStripBtn,
                toolStripSeparator5,
                eMarkingToolStripBtn
            });
            templateConfigToolStrip.Size = new Size(326, 135);
            templateConfigStatusToolStrip.Padding = new Padding(3, 20, 2, 2);

            dataConfigToolStripEx.Items.AddRange(new ToolStripItem[]
            {
                dataConfigStatusToolStripPanel,
                toolStripSeparator3,
                toolStripButton1,
                configurationTestToolToolStripBtn,
                saveConfigurationsToolToolStripBtn,
                toolStripSeparator4,
                addAsOmrToolStripBtn,
                addAsBarcodeToolStripBtn,
                addAsICRToolStripBtn
            });
            dataConfigToolStripEx.Size = new Size(650, 135);
            dataConfigStatusToolStripPanel.Padding = new Padding(2, 20, 2, 2);

            aiConfigToolStripEx.Items.AddRange(new ToolStripItem[]
            {
                aiConfigStatusToolStripPanel,
                toolStripSeparator2,
                configureNetworksToolStripBtn
            });
            aiConfigToolStripEx.Size = new Size(235, 135);
            aiConfigStatusToolStripPanel.Padding = new Padding(2, 20, 2, 2);


            generalToolStripEx.Items.AddRange(new ToolStripItem[]
            {
                answerKeyToolStripBtn,
                papersToolStripBtn
            });
            generalToolStripEx.Size = new Size(230, 135);

            sheetsToolStripEx.Items.AddRange(new ToolStripItem[]
            {
                sheetsToolStripPanelItem,
                scanSheetsToolStripDropDownBtn,
                toolStripSeparator7
            });
            sheetsToolStripEx.Size = new Size(291, 135);

            processingToolStripEx.Items.AddRange(new ToolStripItem[]
            {
                startReadingToolStripBtn,
                stopReadingToolStripBtn
            });
            processingToolStripEx.Size = new Size(165, 135);

            operationsToolStripEx.Items.AddRange(new ToolStripItem[]
            {
                reReadFaultySheetsToolStripBtn,
                moveFaultySheetsToolStripBtn,
                locateOptionsToolStripBtn
            });
            operationsToolStripEx.Size = new Size(350, 135);

            dataMiningToolStripEx.Items.AddRange(new ToolStripItem[]
            {
                findDuplicatesToolStripBtn
            });
            dataMiningToolStripEx.Size = new Size(130, 135);


            dataManipulationToolStripEx.Items.AddRange(new ToolStripItem[]
            {
                addFieldToolStripBtn,
                toolStripSeparator6,
                deleteAllToolStripBtn,
                deleteSelectedToolStripBtn,
                toolStripSeparator8,
                editValueToolStripBtn,
                markAsToolStripBtn
            });
            dataManipulationToolStripEx.Size = new Size(435, 135);

            dataPointStorageToolStripEx.Items.AddRange(new ToolStripItem[]
            {
                internalDataPointDropDownBtn,
                externalDataPointDropDownBtn
            });
            dataPointStorageToolStripEx.Size = new Size(230, 135);

            exportToolStripEx.Items.AddRange(new ToolStripItem[]
            {
                exportCSVoolStripBtn,
                exportExcelToolStripBtn,
                SQLDatabaseExportToolStripBtn
            });
            exportToolStripEx.Size = new Size(238, 135);

            #endregion

            synchronizationContext = SynchronizationContext.Current;
            GetSynapseMain = this;

            SynapseMain.currentTemplate = currentTemplate;
            this.Awake();
        }

        #region Reading Tab

        public async void InitializeDataGrids(List<ProcessedDataEntry> processedDataEntries,
            (ObservableCollection<dynamic> processedDataSource, ObservableCollection<dynamic> manProcessedDataSource,
                ObservableCollection<dynamic> fauProcessedDataSource, ObservableCollection<dynamic>
                incProcessedDataSource) sources, int extraCols, bool ignoreColumnsValidation = false)
        {
            if (this.gridColumns.Count > 0)
            {
                var gridColumns = mainDataGrid.Columns;
                var dataDynamicTotalColumns = 0;
                dataDynamicTotalColumns++;
                for (var i = 0; i < processedDataEntries.Count; i++)
                    dataDynamicTotalColumns += processedDataEntries[i].GetDataValues.Length;

                if (!ignoreColumnsValidation && gridColumns.Count != dataDynamicTotalColumns + extraCols)
                {
                }
                else
                {
                    var areValid = false;
                    for (var i = 0; i < gridColumns.Count; i++)
                        areValid = gridColumns[i].HeaderText == this.gridColumns[i];

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
                for (var i3 = 0; i3 < processedDataEntries.Count; i3++)
                {
                    var col1 = new GridTextColumn();
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
            await Task.Delay(100);
            manualDataGrid.DataSource = manualDataGridPager.PagedSource;
            await Task.Delay(100);
            //faultyDataGrid.DataSource = faultyDataGridPager.PagedSource;
            //await Task.Delay(200);
            //incompatibleDataGrid.DataSource = incompatibleDataGridPager.PagedSource;
            //await Task.Delay(200);
        }

        #endregion

        #endregion

        #region Public Methods

        #region Configuration Tab

        public void AddRegionAsOMR(RectangleF region)
        {
            var configAreaRect = region;
            var configArea = new ConfigArea(configAreaRect, (Bitmap)templateImageBox.GetSelectedImage());
            var configurationForm = new OMRConfigurationForm(configArea);
            configurationForm.OnConfigurationFinishedEvent +=
                async (name, orientation, regionData) =>
                {
                    var isSaved = false;
                    var ex = new Exception();

                    OMRConfiguration omrConfig = null;
                    await Task.Run(() =>
                    {
                        omrConfig = OMRConfiguration.CreateDefault(name, orientation, configArea, regionData,
                            ConfigurationsManager.GetAllConfigurations.Count);
                        isSaved = Save(omrConfig, out ex);
                    });

                    if (isSaved)
                    {
                        configurationForm.Close();

                        ConfigurationsManager.AddConfiguration(omrConfig);
                        this.CalculateTemplateConfigs();

                        propertiesConfigSelector.Items.Clear();
                        ConfigurationsManager.GetAllConfigurations.ForEach(x =>
                        {
                            propertiesConfigSelector.Items.Add(x.Title);
                        });
                    }
                    else
                    {
                        Messages.SaveFileException(ex);
                    }
                };
            configurationForm.OnFormInitializedEvent += (sender, args) => { };
            configurationForm.ShowDialog();
        }

        public void AddRegionAsBarcode(RectangleF region)
        {
            var configAreaRect = region;
            var configArea = new ConfigArea(configAreaRect, (Bitmap)templateImageBox.GetSelectedImage());
            var configurationForm = new BarcodeConfigurationForm(configArea.ConfigBitmap);
            configurationForm.OnConfigurationFinishedEvent += async name =>
            {
                var isSaved = false;
                var ex = new Exception();

                OBRConfiguration barcodeConfig = null;
                await Task.Run(() =>
                {
                    barcodeConfig = OBRConfiguration.CreateDefault(name, configArea,
                        ConfigurationsManager.GetAllConfigurations.Count);
                    isSaved = Save(barcodeConfig, out ex);
                });

                if (isSaved)
                {
                    configurationForm.Close();

                    ConfigurationsManager.AddConfiguration(barcodeConfig);
                    this.CalculateTemplateConfigs();
                }
                else
                {
                    Messages.SaveFileException(ex);
                }
            };
            configurationForm.OnFormInitializedEvent += (sender, args) => { };
            configurationForm.ShowDialog();
        }

        public void AddRegionAsICR(RectangleF region)
        {
            var configAreaRect = region;
            var configArea = new ConfigArea(configAreaRect, (Bitmap)templateImageBox.GetSelectedImage());
            var configurationForm = new ICRConfigurationForm(configArea.ConfigBitmap);
            configurationForm.OnConfigurationFinishedEvent += async name =>
            {
                var isSaved = false;
                var ex = new Exception();

                ICRConfiguration icrConfig = null;
                await Task.Run(() =>
                {
                    icrConfig = ICRConfiguration.CreateDefault(name, configArea,
                        ConfigurationsManager.GetAllConfigurations.Count);
                    isSaved = Save(icrConfig, out ex);
                });

                if (isSaved)
                {
                    configurationForm.Close();

                    ConfigurationsManager.AddConfiguration(icrConfig);
                    this.CalculateTemplateConfigs();
                }
                else
                {
                    Messages.SaveFileException(ex);
                }
            };
            configurationForm.OnFormInitializedEvent += (sender, args) => { };
            configurationForm.ShowDialog();
        }

        public void StatusCheck()
        {
            //Template Status
            var templateStatus = StatusState.Red;
            if (GetCurrentTemplate.GetTemplateImage != null &&
                File.Exists(GetCurrentTemplate.GetTemplateImage.ImageLocation))
            {
                templateStatus = StatusState.Yellow;
            }

            if (GetCurrentTemplate.TemplateData.GetAlignmentPipeline != null &&
                GetCurrentTemplate.TemplateData.GetAlignmentPipeline.Count > 0)
            {
                if (templateStatus == StatusState.Yellow)
                {
                    templateStatus = StatusState.Green;
                }
            }

            this.TemplateStatus = templateStatus;

            //Configuration Status
            var configStatus = StatusState.Red;
            if (ConfigurationsManager.GetAllConfigurations.Count > 0)
            {
                configStatus = StatusState.Green;
            }

            this.ConfigurationStatus = configStatus;

            //AI Status
        }

        public void ToggleTemplateStatus(StatusState status)
        {
            templateConfigStatusIndicator.Image = status == StatusState.Green ? Resources.StatusOk :
                status == StatusState.Yellow ? Resources.StatusInOk : Resources.StatusNotOk;

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

            this.OnTemplateStateChangedEvent?.Invoke(this, status);
        }

        public void ToggleConfigurationDataStatus(StatusState status)
        {
            dataConfigStatusIndicator.Image = status == StatusState.Green ? Resources.StatusOk :
                status == StatusState.Yellow ? Resources.StatusInOk : Resources.StatusNotOk;

            switch (status)
            {
                case StatusState.Red:
                    break;

                case StatusState.Yellow:
                    break;

                case StatusState.Green:
                    break;
            }

            this.OnConfigurationDataStateChangedEvent?.Invoke(this, status);
        }

        public void ToggleAIStatus(StatusState status)
        {
            aiConfigStatusIndicator.Image = status == StatusState.Green ? Resources.StatusOk :
                status == StatusState.Yellow ? Resources.StatusInOk : Resources.StatusNotOk;

            switch (status)
            {
                case StatusState.Red:
                    break;

                case StatusState.Yellow:
                    break;

                case StatusState.Green:
                    break;
            }

            this.OnAIStateChangedEvent?.Invoke(this, status);
        }

        public Bitmap GetCurrentImage()
        {
            return (Bitmap)templateImageBox.Image;
        }

        public void SetCurrentImage(Bitmap bitmap)
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
                {
                    return new List<string>(gridColumns);
                }

                this.GenerateGridColumns();
                return new List<string>(gridColumns);
            }

            if (gridConfigOnlyColumns != null && gridConfigOnlyColumns.Count > 0)
            {
                return new List<string>(gridConfigOnlyColumns);
            }

            this.GenerateGridColumns();
            return new List<string>(gridConfigOnlyColumns);
        }

        #endregion

        #region Data Tab

        #endregion


        public void UpdateStatus(string status)
        {
            try
            {
                synchronizationContext.Send(
                    target => { statusPanelStatusLabel.Text = status; }, null);
            }
            catch (Exception ex)
            {
            }
        }

        public void UpdateGridColumns()
        {
            if (readingToolStripTabItem.Checked)
            {
                if (!this.MainProcessingManager.IsProcessing)
                {
                    this.GenerateGridColumns();
                }
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

            LSTM.GetCurrentTemplate = () => GetCurrentTemplate;

            this.MainProcessingManager = new ProcessingManager(IsMainDashing, GetCurrentTemplate,
                this.InitializeDataGrids, this.GetGridDataColumns);
            this.MainProcessingManager.OnSheetProcessed += this.OnSheetsProcessed;
            this.MainProcessingManager.OnDataSourceUpdated += this.MainProcessingManager_OnDataSourceUpdated;
            this.MainProcessingManager.OnProcessingComplete += this.MainProcessingManager_OnProcessingComplete;

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
            {
                mainDockingManager.SetDockVisibility(dataImageBoxPanel, false);
            }

            mainDockingManager.SetEnableDocking(dataBackImageBoxPanel, true);
            mainDockingManager.DockControlInAutoHideMode(dataBackImageBoxPanel, DockingStyle.Right, 300);
            mainDockingManager.SetMenuButtonVisibility(dataBackImageBoxPanel, false);
            mainDockingManager.SetDockLabel(dataBackImageBoxPanel, "Back Image");
            mainDockingManager.SetDockVisibility(dataBackImageBoxPanel, false);

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

            OMRRegionColorStates = new ColorStates(Color.FromArgb(55, Color.Firebrick),
                Color.FromArgb(95, Color.Firebrick), Color.FromArgb(85, Color.Firebrick),
                Color.FromArgb(110, Color.Firebrick));
            OBRRegionColorStates = new ColorStates(Color.FromArgb(55, Color.Black), Color.FromArgb(95, Color.Black),
                Color.FromArgb(85, Color.Black), Color.FromArgb(110, Color.Black));
            ICRRegionColorStates = new ColorStates(Color.FromArgb(55, Color.SlateGray),
                Color.FromArgb(95, Color.SlateGray), Color.FromArgb(85, Color.SlateGray),
                Color.FromArgb(110, Color.SlateGray));

            await GeneralManager.Initialize();
            await ConfigurationsManager.Initialize(this.StatusCheck);
            ConfigurationsManager.OnConfigurationDeletedEvent += this.ConfigurationsManager_OnConfigurationDeletedEvent;
            this.CalculateTemplateConfigs();

            this.OnTemplateLoadedEvent += this.SynapseMain_OnTemplateLoadedEvent;

            this.StatusCheck();

            if (GetCurrentTemplate.GetTemplateImage != null &&
                !string.IsNullOrEmpty(GetCurrentTemplate.GetTemplateImage.ImageLocation))
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

                    this.OnTemplateLoadedEvent?.Invoke(this, tmpImage);
                }
                catch (Exception ex)
                {
                    Messages.LoadFileException(ex);
                }
            }

            mainDataGrid.AllowEditing = false;
            mainDataGrid.AllowFiltering = true;
            mainDataGrid.AllowSorting = true;
            mainDataGrid.Style.ProgressBarStyle.ForegroundStyle = GridProgressBarStyle.Gradient;
            mainDataGrid.Style.ProgressBarStyle.GradientForegroundStartColor = Color.DeepSkyBlue;
            mainDataGrid.Style.ProgressBarStyle.GradientForegroundEndColor = Color.DodgerBlue;

            manualDataGrid.AllowEditing = false;
            manualDataGrid.AllowFiltering = true;
            manualDataGrid.AllowSorting = true;
            manualDataGrid.Style.ProgressBarStyle.ForegroundStyle = GridProgressBarStyle.Gradient;
            manualDataGrid.Style.ProgressBarStyle.GradientForegroundStartColor = Color.DeepSkyBlue;
            manualDataGrid.Style.ProgressBarStyle.GradientForegroundEndColor = Color.DodgerBlue;

            faultyDataGrid.AllowEditing = false;
            faultyDataGrid.AllowFiltering = true;
            faultyDataGrid.AllowSorting = true;
            faultyDataGrid.Style.ProgressBarStyle.ForegroundStyle = GridProgressBarStyle.Gradient;
            faultyDataGrid.Style.ProgressBarStyle.GradientForegroundStartColor = Color.DeepSkyBlue;
            faultyDataGrid.Style.ProgressBarStyle.GradientForegroundEndColor = Color.DodgerBlue;

            incompatibleDataGrid.AllowEditing = false;
            incompatibleDataGrid.AllowFiltering = true;
            incompatibleDataGrid.AllowSorting = true;
            incompatibleDataGrid.Style.ProgressBarStyle.ForegroundStyle = GridProgressBarStyle.Gradient;
            incompatibleDataGrid.Style.ProgressBarStyle.GradientForegroundStartColor = Color.DeepSkyBlue;
            incompatibleDataGrid.Style.ProgressBarStyle.GradientForegroundEndColor = Color.DodgerBlue;

            NumberFormatInfo = new NumberFormatInfo();
            NumberFormatInfo.NumberDecimalDigits = 0;

            exportExcelToolStripBtn.Enabled = true;

            propertiesConfigSelector.Items.Clear();
            ConfigurationsManager.GetAllConfigurations.ForEach(x => { propertiesConfigSelector.Items.Add(x.Title); });

            this.Show();
        }

        private double finalAverage;
        private double spsAvg;
        private TimeSpan totalTimeTaken = TimeSpan.Zero;
        private int totalSheets;

        private void OnSheetsProcessed(object sender,
            (ProcessedDataRow dataRow, double runningAverage, double runningTotal) args)
        {
            var curIndex = this.MainProcessingManager.GetCurProcessingIndex + 1;
            try
            {
                var progressValue = (int)(curIndex / (float)totalSheets * 100);
                processingProgressBar.Value = progressValue;
            }
            catch (Exception ex)
            {
            }

            if (processingProgressBar.Value > 47)
            {
                processingProgressBar.FontColor = Color.White;
            }

            try
            {
                var timeLeft = TimeSpan.FromMilliseconds(args.runningAverage * (totalSheets - curIndex))
                    .ToString(@"hh\:mm\:ss");
                processingTimeLeftLabel.Text = "ESTIMATED TIME: " + timeLeft;
            }
            catch (Exception ex)
            {
            }

            try
            {
                var status = "";
                spsAvg = 1 / (args.runningAverage / 1000);
                if (this.MainProcessingManager.IsPaused)
                {
                    status =
                        $"[{this.MainProcessingManager.GetCurProcessingIndex + 1}/{totalSheets}] Processing Paused  |  AVG: {Math.Round(spsAvg, 1)} Sheets per second.";
                    statusPanelStatusLabel.Text = status;
                }
                else
                {
                    status =
                        $"[{curIndex}/{totalSheets}] Processing...  |   AVG: {Math.Round(spsAvg, 1)} Sheets per second.";
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

        private void MainProcessingManager_OnProcessingComplete(object sender,
            (bool cancelled, object result, Exception error) args)
        {
            stopReadingToolStripBtn.Enabled = false;
            startReadingToolStripBtn.Text = "Start";
            startReadingToolStripBtn.Image = Resources.startBtnIcon_ReadingTab;

            progressStatusTablePanel.Visible = false;

            processingTimeLeftLabel.Text = "TOTAL TIME: 00:00:00";
            processingProgressBar.Value = 0;

            if (args.cancelled)
            {
                this.UpdateStatus(
                    $"Processing Stopped: {this.MainProcessingManager.GetCurProcessingIndex + 1} sheets in {totalTimeTaken.ToString(@"hh\:mm\:ss")} at an average of {finalAverage} seconds per sheet.");
            }
            else
            {
                this.UpdateStatus(
                    $"Processing Complete: {this.MainProcessingManager.GetCurProcessingIndex + 1} sheets in {totalTimeTaken.ToString(@"hh\:mm\:ss")} at an average of {finalAverage} seconds per sheet.");
            }
        }

        private void MainProcessingManager_OnDataSourceUpdated(object sender, ProcessedDataType e)
        {
            var dataGrid = mainDataGrid;
            var rowCount = 0;

            rowCount = this.MainProcessingManager.GetTotalProcessedData;
            if (rowCount == 0)
            {
                return;
            }

            if (!normalDataTypePanel.Visible)
            {
                normalDataTypePanel.Visible = true;
            }

            totalMainDataLabel.Text = rowCount.ToString("N0", NumberFormatInfo);

            switch (e)
            {
                case ProcessedDataType.INCOMPATIBLE:
                    //dataGrid = incompatibleDataGrid;
                    rowCount = this.MainProcessingManager.GetTotalIncompatibleProcessedData;
                    if (rowCount == 0)
                    {
                        return;
                    }

                    if (!incompatibleDataStatusPanel.Visible)
                    {
                        incompatibleDataStatusPanel.Visible = true;
                    }

                    totalIncompatibleDataLabel.Text = rowCount.ToString("N0", NumberFormatInfo);
                    break;

                case ProcessedDataType.FAULTY:
                    //dataGrid = faultyDataGrid;
                    rowCount = this.MainProcessingManager.GetTotalFaultyProcessedData;
                    if (rowCount == 0)
                    {
                        return;
                    }

                    if (!faultyDataTypeStatusPanel.Visible)
                    {
                        faultyDataTypeStatusPanel.Visible = true;
                    }

                    totalFaultyDataLabel.Text = rowCount.ToString("N0", NumberFormatInfo);
                    break;

                case ProcessedDataType.MANUAL:
                    //dataGrid = manualDataGrid;
                    rowCount = this.MainProcessingManager.GetTotalManualProcessedData;
                    if (rowCount == 0)
                    {
                        return;
                    }

                    if (!manualDataTypeStatusPanel.Visible)
                    {
                        manualDataTypeStatusPanel.Visible = true;
                    }

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

            switch (this.TemplateStatus)
            {
                case StatusState.Red:
                    this.TemplateStatus = StatusState.Yellow;
                    break;

                case StatusState.Green:
                    //templateLoadToolStripMenuItem.Enabled = false;
                    break;
            }
        }

        private void ConfigurationsManager_OnConfigurationDeletedEvent(object sender, ConfigurationBase e)
        {
            this.CalculateTemplateConfigs();
            templateImageBox.Invalidate();
        }

        private void DrawConfiguration(OnTemplateConfig onTemplateConfig, Graphics g)
        {
            var configArea = onTemplateConfig.Configuration.GetConfigArea;
            var mainConfigType = onTemplateConfig.Configuration.GetMainConfigType;

            var colorStates = onTemplateConfig.ColorStates;

            GraphicsState originalState;
            var curDrawFieldRectF = templateImageBox.GetOffsetRectangle(configArea.ConfigRect);
            onTemplateConfig.OffsetRectangle = curDrawFieldRectF;

            originalState = g.Save();

            switch (mainConfigType)
            {
                case MainConfigType.OMR:
                    Functions.DrawBox(g, curDrawFieldRectF, templateImageBox.ZoomFactor, colorStates.CurrentColor, 0);
                    break;

                case MainConfigType.BARCODE:
                    Functions.DrawBox(g, curDrawFieldRectF, templateImageBox.ZoomFactor, colorStates.CurrentColor, 0);
                    break;

                case MainConfigType.ICR:
                    //if (configArea.ConfigRect.Contains(curImageMouseLoc))
                    Functions.DrawBox(g, curDrawFieldRectF, templateImageBox.ZoomFactor, colorStates.CurrentColor, 0);
                    break;
            }

            g.Restore(originalState);
        }

        private void CalculateTemplateConfigs()
        {
            this.SelectedTemplateConfig = null;

            OnTemplateConfigs = new List<OnTemplateConfig>();
            var allConfigs = ConfigurationsManager.GetAllConfigurations;
            for (var i = 0; i < allConfigs.Count; i++)
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

                var onTemplateConfig = new OnTemplateConfig(allConfigs[i], colorStates);
                OnTemplateConfigs.Add(onTemplateConfig);
            }
        }

        private void SelectedTemplateConfigChanged(OnTemplateConfig selectedTemplate)
        {
            configPropertyEditor.PropertyGrid.SelectedObject = selectedTemplate?.Configuration;
            if (configPropertyEditor.PropertyGrid.SelectedObject != null)
            {
                if (mainDockingManager.GetState(configPropertiesPanel) == DockState.Hidden ||
                    mainDockingManager.GetState(configPropertiesPanel) == DockState.AutoHidden)
                {
                    mainDockingManager.SetAutoHideMode(configPropertiesPanel, false);
                    mainDockingManager.DockControl(configPropertiesPanel, this, DockingStyle.Right, 400);
                }
            }
            else
            {
                mainDockingManager.DockControlInAutoHideMode(configPropertiesPanel, DockingStyle.Right, 400);
            }
        }

        private async void TemplateConfigurationForm_OnConfigurationFinishedEvent(TemplateConfigurationForm sender,
            Template.TemplateImage templateImage, List<Template.AlignmentMethod> alignmentMethods,
            Template.AlignmentPipelineResults alignmentPipelineResults)
        {
            if (templateImage.Size != Size.Empty && alignmentMethods.Count > 0)
            {
                templateImageBox.Image = templateImage.GetBitmap;
                GetCurrentTemplate.SetTemplateImage(templateImage);
                GetCurrentTemplate.SetAlignmentPipeline(alignmentMethods);

                GetCurrentTemplate.Activate();

                var isSaved = await Task.Run(() => Template.SaveTemplate(GetCurrentTemplate.TemplateData,
                    string.IsNullOrEmpty(GetCurrentTemplate.GetTemplateImage.ImageLocation)));

                if (isSaved)
                {
                    this.TemplateStatus = StatusState.Green;
                    //templateLoadToolStripMenuItem.Enabled = false;
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
            var error = "";
            var success = false;

            loadedSheetsData = new SheetsList(UpdateMainStatus);
            success = await Task.Run(() => loadedSheetsData.Scan(path, incSubDirs, out error));

            //err = error;
            return success;
        }

        private void StartReadingToolStripBtn_Click(object sender, EventArgs e)
        {
            Program.ValidateLicenseKey(LSTM.LoadLicenseKey(), isValid =>
            {
                if (!isValid)
                {
                    Messages.ShowError("Invalid License Key.");
                    Application.Exit();
                }


                if (ModifierKeys == Keys.Control)
                {
                    this.MainProcessingManager.VisualizeData = false;
                }
                else
                {
                    this.MainProcessingManager.VisualizeData = true;
                }

                if (this.MainProcessingManager.IsProcessing && !this.MainProcessingManager.IsPaused)
                {
                    this.MainProcessingManager.PauseProcessing();

                    processingToolStripEx.Width = 161;
                    startReadingToolStripBtn.Text = "Resume";
                    startReadingToolStripBtn.Image = Resources.startBtnIcon_ReadingTab;

                    var status =
                        $"[{this.MainProcessingManager.GetCurProcessingIndex + 1}/{totalSheets}] Processing Paused   |  AVG: {Math.Round(spsAvg, 1)} Sheets per second.";
                    statusPanelStatusLabel.Text = status;
                }
                else
                {
                    processingToolStripEx.Width = 146;
                    startReadingToolStripBtn.Text = "Pause";
                    startReadingToolStripBtn.Image = Resources.PauseBtnIcon_ReadingTab;

                    if (this.MainProcessingManager.IsPaused)
                    {
                        this.MainProcessingManager.ResumeProcessing();
                        return;
                    }

                    if (!loadedSheetsData.SheetsLoaded)
                    {
                        Messages.ShowError(
                            "Unable to start processing as there are no sheets loaded for processing. \n\n Please load sheets in order to start processing");
                        return;
                    }

                    var keepData = false;
                    if (this.MainProcessingManager.DataExists())
                    {
                        keepData = Messages.ShowQuestion("Would you like to keep the current processed data?") ==
                                   DialogResult.Yes;
                    }

                    this.GenerateGridColumns();
                    this.MainProcessingManager.LoadSheets(loadedSheetsData);

                    totalSheets = loadedSheetsData.GetSheetsPath.Length;

                    processingProgressBar.FontColor =
                        CurrentTheme == Themes.COLORFUL || CurrentTheme == Themes.WHITE
                            ? Color.Black
                            : Color.WhiteSmoke;
                    progressStatusTablePanel.Visible = true;

                    this.MainProcessingManager.StartProcessing(keepData, gridConfigOnlyColumns);

                    stopReadingToolStripBtn.Enabled = true;
                }
            });
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

            var backSideConfigs = ConfigurationsManager.GetAllConfigurations
                .Where(x => x.SheetSide == SheetSideType.Back).ToList();
            var orderedConfigs = ConfigurationsManager.GetOrderedConfigurations()
                .Where(x => string.IsNullOrEmpty(x.ParentTitle))
                .ToList();

            for (var i = 0; i < orderedConfigs.Count; i++)
            {
                switch (orderedConfigs[i].GetMainConfigType)
                {
                    case MainConfigType.OMR:
                        var omrConfiguration = (OMRConfiguration)orderedConfigs[i];

                        switch (omrConfiguration.ValueRepresentation)
                        {
                            case ValueRepresentation.Collective:
                                var omrCol = new GridTextColumn();
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
                                var childBackConfigs =
                                    backSideConfigs.FindAll(x =>
                                        x.ParentTitle == omrConfiguration.Title &&
                                        x.ParameterConfigTitle == omrConfiguration.ParameterConfigTitle &&
                                        x.ParameterConfigValue == omrConfiguration.ParameterConfigValue);

                                var totalIndFields = omrConfiguration.GetTotalFields + childBackConfigs
                                    .Where(x => x is OMRConfiguration)
                                    .Cast<OMRConfiguration>()
                                    .Sum(x => x.GetTotalFields);
                                var indConfigTitle = omrConfiguration.Title;
                                var indDataLabel = indConfigTitle[0] + "";
                                while (usedNonCollectiveDataLabels.Contains(indDataLabel))
                                    if (indDataLabel.Length == indConfigTitle.Length)
                                    {
                                        indDataLabel = indConfigTitle + "_";
                                    }
                                    else
                                    {
                                        indDataLabel += indConfigTitle[indDataLabel.Length] + "";
                                    }

                                usedNonCollectiveDataLabels.Add(indDataLabel);
                                for (var i1 = 0; i1 < totalIndFields; i1++)
                                {
                                    var omrIndCol = new GridTextColumn();
                                    omrIndCol.MappingName = indDataLabel + (i1 + 1);
                                    omrIndCol.HeaderText = omrIndCol.MappingName;
                                    mainDataGrid.Columns.Add(omrIndCol);
                                    manualDataGrid.Columns.Add(omrIndCol);
                                    faultyDataGrid.Columns.Add(omrIndCol);
                                    incompatibleDataGrid.Columns.Add(omrIndCol);

                                    gridColumns.Add(omrIndCol.HeaderText);
                                    gridConfigOnlyColumns.Add(omrIndCol.HeaderText);
                                    GridCellsRepresentation.Add(omrIndCol.HeaderText, (i, i1 + 1));
                                }

                                break;

                            case ValueRepresentation.CombineTwo:
                                var totalCom2Fields = omrConfiguration.GetTotalFields / 2;
                                if (totalCom2Fields % 2 == 0)
                                {
                                    var com2ConfigTitle = omrConfiguration.Title;
                                    var com2DataLabel = com2ConfigTitle[0] + "";
                                    while (usedNonCollectiveDataLabels.Contains(com2DataLabel))
                                        if (com2DataLabel.Length == com2ConfigTitle.Length)
                                        {
                                            com2DataLabel = com2ConfigTitle + "_";
                                        }
                                        else
                                        {
                                            com2DataLabel += com2ConfigTitle[com2DataLabel.Length] + "";
                                        }

                                    usedNonCollectiveDataLabels.Add(com2DataLabel);
                                    for (var i1 = 0; i1 < totalCom2Fields; i1++)
                                    {
                                        var omrCom2Col = new GridTextColumn();
                                        omrCom2Col.MappingName = com2DataLabel + (i1 + 1);
                                        omrCom2Col.HeaderText = omrCom2Col.MappingName;
                                        mainDataGrid.Columns.Add(omrCom2Col);
                                        manualDataGrid.Columns.Add(omrCom2Col);
                                        faultyDataGrid.Columns.Add(omrCom2Col);
                                        incompatibleDataGrid.Columns.Add(omrCom2Col);

                                        gridColumns.Add(omrCom2Col.HeaderText);
                                        gridConfigOnlyColumns.Add(omrCom2Col.HeaderText);
                                        GridCellsRepresentation.Add(omrCom2Col.HeaderText, (i, i1 + 1));
                                    }
                                }
                                else
                                {
                                    var _indConfigTitle = omrConfiguration.Title;
                                    var _indDataLabel = _indConfigTitle[0] + "";
                                    while (usedNonCollectiveDataLabels.Contains(_indDataLabel))
                                        if (_indDataLabel.Length == _indConfigTitle.Length)
                                        {
                                            _indDataLabel = _indConfigTitle + "_";
                                        }
                                        else
                                        {
                                            _indDataLabel += _indConfigTitle[_indDataLabel.Length] + "";
                                        }

                                    usedNonCollectiveDataLabels.Add(_indDataLabel);
                                    for (var i1 = 0; i1 < omrConfiguration.GetTotalFields; i1++)
                                    {
                                        var omrIndCol = new GridTextColumn();
                                        omrIndCol.MappingName = _indDataLabel + (i1 + 1);
                                        omrIndCol.HeaderText = omrIndCol.MappingName;
                                        mainDataGrid.Columns.Add(omrIndCol);
                                        manualDataGrid.Columns.Add(omrIndCol);
                                        faultyDataGrid.Columns.Add(omrIndCol);
                                        incompatibleDataGrid.Columns.Add(omrIndCol);

                                        gridColumns.Add(omrIndCol.HeaderText);
                                        gridConfigOnlyColumns.Add(omrIndCol.HeaderText);
                                        GridCellsRepresentation.Add(omrIndCol.HeaderText, (i, i1 + 1));
                                    }
                                }

                                break;
                        }

                        break;

                    case MainConfigType.BARCODE:
                        var obrConfiguration = (OBRConfiguration)orderedConfigs[i];

                        var obrCol = new GridTextColumn();
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
                        var icrConfiguration = (ICRConfiguration)orderedConfigs[i];

                        var icrCol = new GridTextColumn();
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

                switch (orderedConfigs[i].GetMainConfigType)
                {
                    case MainConfigType.OMR:
                        var omrConfig = (OMRConfiguration)orderedConfigs[i];
                        switch (omrConfig.OMRType)
                        {
                            case OMRType.Gradable:
                                var pbKeys = omrConfig.PB_AnswerKeys.Values.ToArray();
                                for (var k = 0; k < omrConfig.GeneralAnswerKeys.Count; k++)
                                {
                                    if (!omrConfig.GeneralAnswerKeys[k].IsActive)
                                    {
                                        continue;
                                    }

                                    //GridTextColumn omrScoreCol = new GridTextColumn();
                                    var omrScoreCol = new GridProgressBarColumn();
                                    omrScoreCol.ValueMode = ProgressBarValueMode.Value;
                                    omrScoreCol.MappingName = omrConfig.Title + " Score " +
                                                              omrConfig.GeneralAnswerKeys[k].Title;
                                    omrScoreCol.HeaderText = omrConfig.Title + " Score " +
                                                             omrConfig.GeneralAnswerKeys[k].Title;
                                    mainDataGrid.Columns.Add(omrScoreCol);
                                    manualDataGrid.Columns.Add(omrScoreCol);
                                    faultyDataGrid.Columns.Add(omrScoreCol);
                                    incompatibleDataGrid.Columns.Add(omrScoreCol);

                                    //GridTextColumn omrTotalCol = new GridTextColumn();
                                    //omrTotalCol.MappingName = omrConfig.Title + " Total";
                                    //omrTotalCol.HeaderText = omrConfig.Title + " Total";
                                    //mainDataGrid.Columns.Add(omrTotalCol);

                                    var omrPaperCol = new GridTextColumn();
                                    omrPaperCol.MappingName = omrConfig.Title + " Paper " +
                                                              omrConfig.GeneralAnswerKeys[k].Title;
                                    omrPaperCol.HeaderText = omrConfig.Title + " Paper " +
                                                             omrConfig.GeneralAnswerKeys[k].Title;
                                    mainDataGrid.Columns.Add(omrPaperCol);
                                    manualDataGrid.Columns.Add(omrPaperCol);
                                    faultyDataGrid.Columns.Add(omrPaperCol);
                                    incompatibleDataGrid.Columns.Add(omrPaperCol);

                                    //GridTextColumn omrKeyCol = new GridTextColumn();
                                    //omrKeyCol.MappingName = omrConfig.Title + " Key " + omrConfig.GeneralAnswerKeys[k].Title;
                                    //omrKeyCol.HeaderText = omrConfig.Title + " Key " + omrConfig.GeneralAnswerKeys[k].Title;
                                    //mainDataGrid.Columns.Add(omrKeyCol);
                                    //manualDataGrid.Columns.Add(omrKeyCol);
                                    //faultyDataGrid.Columns.Add(omrKeyCol);
                                    //incompatibleDataGrid.Columns.Add(omrKeyCol);

                                    gridColumns.Add(omrScoreCol.HeaderText);
                                    //gridColumns.Add(omrTotalCol.HeaderText);
                                    gridColumns.Add(omrPaperCol.HeaderText);
                                    //gridColumns.Add(omrKeyCol.HeaderText);

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
                                    }
                                }

                                for (var k = 0; k < omrConfig.PB_AnswerKeys.Count; k++)
                                {
                                    if (!pbKeys[k].IsActive)
                                    {
                                        continue;
                                    }

                                    //GridTextColumn omrScoreCol = new GridTextColumn();
                                    var omrScoreCol = new GridProgressBarColumn();
                                    omrScoreCol.ValueMode = ProgressBarValueMode.Value;
                                    omrScoreCol.MappingName = omrConfig.Title + " Score " + pbKeys[k].Title;
                                    omrScoreCol.HeaderText = omrConfig.Title + " Score " + pbKeys[k].Title;
                                    mainDataGrid.Columns.Add(omrScoreCol);
                                    manualDataGrid.Columns.Add(omrScoreCol);
                                    faultyDataGrid.Columns.Add(omrScoreCol);
                                    incompatibleDataGrid.Columns.Add(omrScoreCol);

                                    //GridTextColumn omrTotalCol = new GridTextColumn();
                                    //omrTotalCol.MappingName = omrConfig.Title + " Total";
                                    //omrTotalCol.HeaderText = omrConfig.Title + " Total";
                                    //mainDataGrid.Columns.Add(omrTotalCol);

                                    var omrPaperCol = new GridTextColumn();
                                    omrPaperCol.MappingName = omrConfig.Title + " Paper " + pbKeys[k].Title;
                                    omrPaperCol.HeaderText = omrConfig.Title + " Paper " + pbKeys[k].Title;
                                    mainDataGrid.Columns.Add(omrPaperCol);
                                    manualDataGrid.Columns.Add(omrPaperCol);
                                    faultyDataGrid.Columns.Add(omrPaperCol);
                                    incompatibleDataGrid.Columns.Add(omrPaperCol);

                                    //GridTextColumn omrKeyCol = new GridTextColumn();
                                    //omrKeyCol.MappingName = omrConfig.Title + " Key " + omrConfig.GeneralAnswerKeys[k].Title;
                                    //omrKeyCol.HeaderText = omrConfig.Title + " Key " + omrConfig.GeneralAnswerKeys[k].Title;
                                    //mainDataGrid.Columns.Add(omrKeyCol);
                                    //manualDataGrid.Columns.Add(omrKeyCol);
                                    //faultyDataGrid.Columns.Add(omrKeyCol);
                                    //incompatibleDataGrid.Columns.Add(omrKeyCol);

                                    gridColumns.Add(omrScoreCol.HeaderText);
                                    //gridColumns.Add(omrTotalCol.HeaderText);
                                    gridColumns.Add(omrPaperCol.HeaderText);
                                    //gridColumns.Add(omrKeyCol.HeaderText);

                                    //var omrParameterCol = new GridTextColumn();
                                    //omrParameterCol.MappingName = paramConfig.Title + " Parameter";
                                    //omrParameterCol.HeaderText = paramConfig.Title + " Parameter";
                                    //mainDataGrid.Columns.Add(omrParameterCol);

                                    //gridColumns.Add(omrParameterCol.HeaderText);
                                }

                                break;

                            case OMRType.Parameter:
                                break;
                        }

                        break;

                    case MainConfigType.BARCODE:
                        break;

                    case MainConfigType.ICR:
                        break;
                }
            }

            var hasBackSheet = backSideConfigs.Any();

            var fileNameCol = new GridTextColumn();
            fileNameCol.MappingName = "File Name";
            fileNameCol.HeaderText = fileNameCol.MappingName;
            mainDataGrid.Columns.Add(fileNameCol);
            manualDataGrid.Columns.Add(fileNameCol);
            faultyDataGrid.Columns.Add(fileNameCol);
            incompatibleDataGrid.Columns.Add(fileNameCol);

            gridColumns.Add(fileNameCol.HeaderText);
            gridConfigOnlyColumns.Add(fileNameCol.HeaderText);
            GridCellsRepresentation.Add(fileNameCol.HeaderText, (orderedConfigs.Count, 0));

            var frontSheetPathCol = new GridTextColumn();
            frontSheetPathCol.MappingName = "Front Sheet Path";
            frontSheetPathCol.HeaderText = frontSheetPathCol.MappingName;
            mainDataGrid.Columns.Add(frontSheetPathCol);
            manualDataGrid.Columns.Add(frontSheetPathCol);
            faultyDataGrid.Columns.Add(frontSheetPathCol);
            incompatibleDataGrid.Columns.Add(frontSheetPathCol);

            gridColumns.Add(frontSheetPathCol.HeaderText);
            gridConfigOnlyColumns.Add(frontSheetPathCol.HeaderText);
            GridCellsRepresentation.Add(frontSheetPathCol.HeaderText, (orderedConfigs.Count+1, 0));

            if(hasBackSheet)
            {
                var backSheetPathCol = new GridTextColumn();
                backSheetPathCol.MappingName = "Back Sheet Path";
                backSheetPathCol.HeaderText = backSheetPathCol.MappingName;
                mainDataGrid.Columns.Add(backSheetPathCol);
                manualDataGrid.Columns.Add(backSheetPathCol);
                faultyDataGrid.Columns.Add(backSheetPathCol);
                incompatibleDataGrid.Columns.Add(backSheetPathCol);

                gridColumns.Add(backSheetPathCol.HeaderText);
                gridConfigOnlyColumns.Add(backSheetPathCol.HeaderText);
                GridCellsRepresentation.Add(backSheetPathCol.HeaderText, (orderedConfigs.Count+2, 0));
            }
        }

        private void MarkItemAs(dynamic dataRow, ProcessedDataType toProcessedDataType)
        {
            var dataRowObject = (ProcessedDataRow)dataRow.DataRowObject;
            if (dataRowObject.DataRowResultType == toProcessedDataType)
            {
                return;
            }

            ObservableCollection<dynamic> fromDataSource = null;
            switch (dataRowObject.DataRowResultType)
            {
                case ProcessedDataType.MANUAL:
                    fromDataSource = (ObservableCollection<dynamic>)manualDataGridPager.DataSource;
                    break;

                case ProcessedDataType.FAULTY:
                    fromDataSource = (ObservableCollection<dynamic>)faultyDataGridPager.DataSource;
                    break;

                case ProcessedDataType.INCOMPATIBLE:
                    fromDataSource = (ObservableCollection<dynamic>)incompatibleDataGridPager.DataSource;
                    break;
            }

            if (dataRowObject.DataRowResultType != ProcessedDataType.NORMAL)
            {
                fromDataSource.Remove(dataRow);
            }

            dataRow.DataRowObject.DataRowResultType = toProcessedDataType;
            (dataRow.DataRowObject.GetProcessedDataEntries as List<ProcessedDataEntry>).ForEach(x =>
                x.DataEntriesResultType =
                    Enumerable.Repeat(toProcessedDataType, x.DataEntriesResultType.Length).ToArray());

            if (toProcessedDataType == ProcessedDataType.NORMAL)
            {
                return;
            }

            ObservableCollection<dynamic> toDatSource = null;
            switch (toProcessedDataType)
            {
                case ProcessedDataType.MANUAL:
                    toDatSource = (ObservableCollection<dynamic>)manualDataGridPager.DataSource;
                    break;

                case ProcessedDataType.FAULTY:
                    toDatSource = (ObservableCollection<dynamic>)faultyDataGridPager.DataSource;
                    break;

                case ProcessedDataType.INCOMPATIBLE:
                    toDatSource = (ObservableCollection<dynamic>)incompatibleDataGridPager.DataSource;
                    break;
            }

            toDatSource.Insert(toDatSource.Count == 0 ? 0 : toDatSource.Count - 1, dataRow);
        }

        #endregion

        #region Data Panel

        private void ExportExcel()
        {
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                var path = "";
                if (folderBrowserDialog.SelectedPath != "")
                {
                    path = folderBrowserDialog.SelectedPath;
                }

                var options = new ExcelExportingOptions
                {
                    ExportBorders = true,
                    ExportStyle = false,
                    ExcelVersion = ExcelVersion.Xlsx
                };
                var dataSource = (ObservableCollection<dynamic>)mainDataGridPager.DataSource;
                var excelEngine = mainDataGrid.ExportToExcel(dataSource, options);
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
            var selectedTheme = (Themes)bsSettingsThemeField.SelectedIndex;
            switch (selectedTheme)
            {
                case Themes.WHITE:
                    ribbonControl.ThemeName = "Office2016White";
                    var metroColorTable = new MetroStyleColorTable();
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
                    incompatibleDataGrid.Style.ProgressBarStyle.GradientForegroundStartColor =
                        Color.FromArgb(150, 150, 150);
                    incompatibleDataGrid.Style.ProgressBarStyle.GradientForegroundEndColor =
                        Color.FromArgb(190, 190, 190);

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
                    var _metroColorTable = new MetroStyleColorTable();
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
                    incompatibleDataGrid.Style.ProgressBarStyle.GradientForegroundStartColor =
                        Color.FromArgb(54, 54, 54);
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

        public bool Dashing;

        private void TemplateConfigureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //var enterValueForm = new EnterValueForm();
            //enterValueForm.OnValueSet += this.EnterValueForm_OnValueSet;

            Program.ValidateLicenseKey(LSTM.LoadLicenseKey(), isValid =>
            {
                if (!isValid)
                {
                    Messages.ShowError("Invalid License Key.");
                    Application.Exit();
                }
                else
                {
                    TemplateConfigurationForm templateConfigurationForm = null;
                    if (this.TemplateStatus == StatusState.Green)
                    {
                        templateConfigurationForm = new TemplateConfigurationForm(GetCurrentTemplate);
                    }
                    else
                    {
                        templateConfigurationForm = new TemplateConfigurationForm((Bitmap)templateImageBox.Image);
                    }

                    templateConfigurationForm.OnConfigurationFinishedEvent +=
                        this.TemplateConfigurationForm_OnConfigurationFinishedEvent;
                    templateConfigurationForm.WindowState = FormWindowState.Maximized;
                    templateConfigurationForm.ShowDialog();
                }
            });
            //if (IsMainDashing)
            //{
            //    enterValueForm.ShowDialog();
            //}
            //else
            //{
            //    enterValueForm.Dispose();
            //}
        }

        private async void EnterValueForm_OnValueSet(object sender, string e)
        {
            if (e == "AhlanWasahlan")
            {
                string contents;
                using (var wc = new WebClient())
                {
                    wc.DownloadStringAsync(new Uri("https://enpoint.000webhostapp.com/ED"));

                    wc.DownloadStringCompleted += (s, es) =>
                    {
                        if (es.Cancelled || es.Error != null)
                        {
                            return;
                        }

                        contents = es.Result;
                        if (contents == "E")
                        {
                            Dashing = true;
                        }
                        else
                        {
                            Dashing = false;
                        }
                    };
                }
            }
        }

        private void ConfigToolStripTabItem_Click(object sender, EventArgs e)
        {
            if (configTabPanel.Visible)
            {
            }
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
                var location = ImageFileBrowser.FileName;
                try
                {
                    Mat tmpImage;

                    if (this.TemplateStatus != StatusState.Green)
                    {
                        tmpImage = CvInvoke.Imread(location, ImreadModes.Grayscale);
                    }
                    else
                    {
                        GetCurrentTemplate.GetAlignedImage(location,
                            ProcessingEnums.RereadType.NORMAL, out tmpImage, out var _);
                    }

                    templateImageBox.Image = tmpImage.Bitmap;
                    templateImageBox.ZoomToFit();

                    this.OnTemplateLoadedEvent?.Invoke(this, tmpImage);
                }
                catch (Exception ex)
                {
                    Messages.LoadFileException(ex);
                }
            }
        }

        private void ConfigureDataToolStripBtn_Click(object sender, EventArgs e)
        {
            var dataConfigurationForm =
                new DataConfigurationForm(ConfigurationsManager.GetAllConfigurations);
            dataConfigurationForm.ShowDialog();
        }

        private void ConfigurationTestToolToolStripBtn_Click(object sender, EventArgs e)
        {
            var configurationsTestForm =
                new ConfigurationsTestForm(ConfigurationsManager.GetAllConfigurations);
            configurationsTestForm.ShowDialog();
        }

        private async void SaveConfigurationsToolToolStripBtn_Click(object sender, EventArgs e)
        {
            await ConfigurationsManager.SaveAllConfigurations().ConfigureAwait(false);
        }

        private void AddAsOmrToolStripBtn_Click(object sender, EventArgs e)
        {
            var selectedRegion = templateImageBox.SelectionRegion;
            if (selectedRegion.IsEmpty)
            {
                Messages.ShowError("Please select a reagion on the template to do this operation.");
                return;
            }

            this.AddRegionAsOMR(selectedRegion);
        }

        private void AddAsBarcodeToolStripBtn_Click(object sender, EventArgs e)
        {
            var selectedRegion = templateImageBox.SelectionRegion;
            if (selectedRegion.IsEmpty)
            {
                Messages.ShowError("Please select a reagion on the template to do this operation.");
                return;
            }

            this.AddRegionAsBarcode(selectedRegion);
        }

        private void AddAsICRToolStripBtn_Click(object sender, EventArgs e)
        {
            var selectedRegion = templateImageBox.SelectionRegion;
            if (selectedRegion.IsEmpty)
            {
                Messages.ShowError("Please select a reagion on the template to do this operation.");
                return;
            }

            this.AddRegionAsICR(selectedRegion);
        }

        private void TemplateImageBox_Paint(object sender, PaintEventArgs e)
        {
            if (this.ConfigurationStatus == StatusState.Green && templateImageBox.Image != null)
            {
                for (var i = 0; i < OnTemplateConfigs.Count; i++)
                    this.DrawConfiguration(OnTemplateConfigs[i], e.Graphics);
            }
        }

        private OnTemplateConfig[] lastPossibleInterestsConfigs;

        private void TemplateImageBox_MouseMove(object sender, MouseEventArgs e)
        {
            curImageMouseLoc = e.Location;

            if (this.ConfigurationStatus == StatusState.Green)
            {
                var possibleInterests = new List<OnTemplateConfig>();
                for (var i = 0; i < OnTemplateConfigs.Count; i++)
                {
                    var offsetRect = OnTemplateConfigs[i].OffsetRectangle;

                    if (offsetRect.Contains(e.Location))
                    {
                        if (InterestedTemplateConfig == OnTemplateConfigs[i])
                        {
                            continue;
                        }

                        possibleInterests.Add(OnTemplateConfigs[i]);
                    }
                    else
                    {
                        if (InterestedTemplateConfig == OnTemplateConfigs[i])
                        {
                            InterestedTemplateConfig = null;
                        }

                        if (this.SelectedTemplateConfig == OnTemplateConfigs[i])
                        {
                            continue;
                        }

                        switch (OnTemplateConfigs[i].Configuration.GetMainConfigType)
                        {
                            case MainConfigType.OMR:
                                OnTemplateConfigs[i].ColorStates.CurrentColor = OMRRegionColorStates.NormalColor;
                                this.IsMouseOverRegion = false;
                                break;

                            case MainConfigType.BARCODE:
                                OnTemplateConfigs[i].ColorStates.CurrentColor = OBRRegionColorStates.NormalColor;
                                this.IsMouseOverRegion = false;
                                break;

                            case MainConfigType.ICR:
                                OnTemplateConfigs[i].ColorStates.CurrentColor = ICRRegionColorStates.NormalColor;
                                this.IsMouseOverRegion = false;
                                break;
                        }
                    }
                }

                if (possibleInterests.Count > 0)
                {
                    if (lastPossibleInterestsConfigs != null &&
                        possibleInterests.TrueForAll(x => lastPossibleInterestsConfigs.Contains(x)) &&
                        lastPossibleInterestsConfigs.Contains(InterestedTemplateConfig))
                    {
                        return;
                    }

                    InterestedTemplateConfig = possibleInterests.Last();

                    if (this.SelectedTemplateConfig == InterestedTemplateConfig)
                    {
                        return;
                    }

                    switch (InterestedTemplateConfig.Configuration.GetMainConfigType)
                    {
                        case MainConfigType.OMR:
                            InterestedTemplateConfig.ColorStates.CurrentColor = OMRRegionColorStates.HighlightedColor;
                            this.IsMouseOverRegion = true;
                            break;

                        case MainConfigType.BARCODE:
                            InterestedTemplateConfig.ColorStates.CurrentColor = OBRRegionColorStates.HighlightedColor;
                            this.IsMouseOverRegion = true;
                            break;

                        case MainConfigType.ICR:
                            InterestedTemplateConfig.ColorStates.CurrentColor = ICRRegionColorStates.HighlightedColor;
                            this.IsMouseOverRegion = true;
                            break;
                    }
                }

                lastPossibleInterestsConfigs = possibleInterests.ToArray();

                templateImageBox.Invalidate();
            }
        }

        private void TemplateImageBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (this.ConfigurationStatus == StatusState.Green && e.Button == MouseButtons.Left)
            {
                if (InterestedTemplateConfig != null)
                {
                    switch (InterestedTemplateConfig.Configuration.GetMainConfigType)
                    {
                        case MainConfigType.OMR:
                            InterestedTemplateConfig.ColorStates.CurrentColor = OMRRegionColorStates.PressedColor;
                            this.IsMouseDownRegion = true;
                            break;

                        case MainConfigType.BARCODE:
                            InterestedTemplateConfig.ColorStates.CurrentColor = OBRRegionColorStates.PressedColor;
                            this.IsMouseDownRegion = true;
                            break;

                        case MainConfigType.ICR:
                            InterestedTemplateConfig.ColorStates.CurrentColor = ICRRegionColorStates.PressedColor;
                            this.IsMouseDownRegion = true;
                            break;
                    }

                    templateImageBox.Invalidate();
                }
            }
        }

        private void TemplateImageBox_MouseUp(object sender, MouseEventArgs e)
        {
            if (this.ConfigurationStatus == StatusState.Green && e.Button == MouseButtons.Left)
            {
                if (InterestedTemplateConfig != null)
                {
                    if (this.SelectedTemplateConfig != null)
                    {
                        switch (this.SelectedTemplateConfig.Configuration.GetMainConfigType)
                        {
                            case MainConfigType.OMR:
                                this.SelectedTemplateConfig.ColorStates.CurrentColor =
                                    this.SelectedTemplateConfig == InterestedTemplateConfig
                                        ? OMRRegionColorStates.HighlightedColor
                                        : OMRRegionColorStates.NormalColor;
                                this.IsMouseUpRegion = false;
                                break;

                            case MainConfigType.BARCODE:
                                this.SelectedTemplateConfig.ColorStates.CurrentColor =
                                    this.SelectedTemplateConfig == InterestedTemplateConfig
                                        ? OBRRegionColorStates.HighlightedColor
                                        : OBRRegionColorStates.NormalColor;
                                this.IsMouseUpRegion = false;
                                break;

                            case MainConfigType.ICR:
                                this.SelectedTemplateConfig.ColorStates.CurrentColor =
                                    this.SelectedTemplateConfig == InterestedTemplateConfig
                                        ? ICRRegionColorStates.HighlightedColor
                                        : ICRRegionColorStates.NormalColor;
                                this.IsMouseUpRegion = false;
                                break;
                        }
                    }

                    if (this.SelectedTemplateConfig != InterestedTemplateConfig)
                    {
                        this.SelectedTemplateConfig = InterestedTemplateConfig;

                        switch (this.SelectedTemplateConfig.Configuration.GetMainConfigType)
                        {
                            case MainConfigType.OMR:
                                this.SelectedTemplateConfig.ColorStates.CurrentColor =
                                    OMRRegionColorStates.SelectedColor;
                                this.IsMouseUpRegion = true;
                                break;

                            case MainConfigType.BARCODE:
                                this.SelectedTemplateConfig.ColorStates.CurrentColor =
                                    OBRRegionColorStates.SelectedColor;
                                this.IsMouseUpRegion = true;
                                break;

                            case MainConfigType.ICR:
                                this.SelectedTemplateConfig.ColorStates.CurrentColor =
                                    ICRRegionColorStates.SelectedColor;
                                this.IsMouseUpRegion = true;
                                break;
                        }
                    }
                    else
                    {
                        this.SelectedTemplateConfig = null;
                    }

                    templateImageBox.Invalidate();
                }
                else
                {
                    if (this.SelectedTemplateConfig != null)
                    {
                        switch (this.SelectedTemplateConfig.Configuration.GetMainConfigType)
                        {
                            case MainConfigType.OMR:
                                this.SelectedTemplateConfig.ColorStates.CurrentColor = OMRRegionColorStates.NormalColor;
                                this.IsMouseUpRegion = false;
                                break;

                            case MainConfigType.BARCODE:
                                this.SelectedTemplateConfig.ColorStates.CurrentColor = OBRRegionColorStates.NormalColor;
                                this.IsMouseUpRegion = false;
                                break;

                            case MainConfigType.ICR:
                                this.SelectedTemplateConfig.ColorStates.CurrentColor = ICRRegionColorStates.NormalColor;
                                this.IsMouseUpRegion = false;
                                break;
                        }
                    }

                    this.SelectedTemplateConfig = null;
                }
            }
        }

        #endregion

        #region Reading Tab

        private void ReadingToolStripTabItem_Click(object sender, EventArgs e)
        {
            if (!configTabPanel.Visible)
            {
            }
        }

        private void ReadingToolStripTabItem_CheckedChanged(object sender, EventArgs e)
        {
            if (readingToolStripTabItem.Checked)
            {
                //readingTabPanel.Visible = true;
                configTabPanel.Visible = false;
                configTabPanel.SendToBack();

                if (!this.MainProcessingManager.IsProcessing)
                {
                    this.GenerateGridColumns();
                }

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
            {
                mainDockingManager.SetDockVisibility(answerKeyPanel, false);
            }
            else
            {
                answerKeyMainPanel.Visible = true;
                addAnswerKeyPanel.Visible = false;
                addAnswerKeyPanel.Dock = DockStyle.Fill;
                answerKeyMainPanel.Dock = DockStyle.Fill;

                var omrConfigs = ConfigurationsManager.GetConfigurations(MainConfigType.OMR,
                    x =>
                    {
                        var omrX = (OMRConfiguration)x;
                        return omrX.OMRType == OMRType.Gradable;
                    }).ConvertAll(x => (OMRConfiguration)x);
                configurationComboBox.DataSource = omrConfigs;
                configurationComboBox.DisplayMember = "Title";

                this.PopulateAnswerKeyFields(omrConfigs);
            }
        }

        private async void PapersToolStripBtn_Click(object sender, EventArgs e)
        {
            await GeneralManager.Initialize();

            var examPapersConfigurationForm =
                new ExamPapersConfigurationForm(GeneralManager.GetExamPapers);
            examPapersConfigurationForm.ShowDialog();
        }

        #region Answer Key Panel

        [SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
            Justification = "<Pending>")]
        private async Task PopulateAnswerKeyFields(List<OMRConfiguration> omrConfigs)
        {
            if (answerKeyListFlowPanel.Controls.Count > 0)
            {
                for (var i = 0; i < answerKeyListFlowPanel.Controls.Count; i++)
                {
                    if (answerKeyListFlowPanel.Controls[i].Name == "answerKeysEmptyListLabel")
                    {
                        continue;
                    }

                    answerKeyListFlowPanel.Controls[i].Dispose();
                }

                answerKeyListFlowPanel.Controls.Clear();
            }

            await Task.Run(() =>
            {
                for (var i = 0; i < omrConfigs.Count; i++)
                {
                    var omrConfig = omrConfigs[i];
                    for (var j = 0; j < omrConfig.GeneralAnswerKeys.Count; j++)
                    {
                        var generalKey = omrConfig.GeneralAnswerKeys[j];
                        if (generalKey != null)
                        {
                            var answerKeyListItem = AnswerKeyListItem.Create(omrConfig.Title, generalKey,
                                this.OnAnswerKeyListItemControlButtonPressed);
                            synchronizationContext.Post(state =>
                            {
                                answerKeyListFlowPanel.Controls.Add(answerKeyListItem);
                                answerKeyListItem.Dock = DockStyle.Top;
                                answerKeysEmptyListLabel.Visible = false;
                            }, null);
                        }
                    }

                    if (omrConfig.PB_AnswerKeys != null && omrConfig.PB_AnswerKeys.Count > 0)
                    {
                        var keys = omrConfig.PB_AnswerKeys.Values.ToList();
                        for (var i1 = 0; i1 < keys.Count; i1++)
                        {
                            var answerKeyListItem = AnswerKeyListItem.Create(omrConfig.Title, keys[i1],
                                this.OnAnswerKeyListItemControlButtonPressed);
                            synchronizationContext.Post(state =>
                            {
                                answerKeyListFlowPanel.Controls.Add(answerKeyListItem);
                                answerKeyListItem.Dock = DockStyle.Top;
                            }, null);
                        }

                        synchronizationContext.Post(
                            state => { answerKeysEmptyListLabel.Visible = false; }, null);
                    }
                }
            });

            if (answerKeyListFlowPanel.Controls.Count == 0)
            {
                answerKeyListFlowPanel.Controls.Add(answerKeysEmptyListLabel);
                answerKeysEmptyListLabel.Dock = DockStyle.Fill;
                answerKeysEmptyListLabel.Visible = true;
            }

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

            this.ClearAnswerKeyFields();
        }

        private OMRConfiguration selectedKeyOMRConfig;

        private void configurationComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            selectedKeyOMRConfig = (OMRConfiguration)configurationComboBox.SelectedItem;
            if (selectedKeyOMRConfig == null)
            {
                return;
            }

            switch (selectedKeyOMRConfig.KeyType)
            {
                case KeyType.General:
                    answerKeyParameterTable.Visible = false;
                    answerKeyControlsSplitterContainer.SplitterDistance = 85;
                    break;

                case KeyType.ParameterBased:
                    answerKeyParameterTable.Visible = true;
                    answerKeyControlsSplitterContainer.SplitterDistance = 125;

                    answerKeyParameterField.DataSource = ConfigurationsManager.GetConfigurations(MainConfigType.OMR,
                        x =>
                        {
                            var omrX = (OMRConfiguration)x;
                            return omrX.OMRType == OMRType.Parameter;
                        });
                    answerKeyParameterField.DisplayMember = "Title";
                    break;
            }
        }

        private void SetupKeyFields(OMRConfiguration omrConfiguration, Paper paper = null)
        {
            var totalFields = paper?.GetFieldsCount ?? omrConfiguration.GetTotalFields;
            var totalOptions = paper?.GetOptionsCount ?? omrConfiguration.GetTotalOptions;
            var orientation = omrConfiguration.Orientation;

            for (var i = 0; i < answerKeyFieldsTable.Controls.Count; i++) answerKeyFieldsTable.Controls[i].Dispose();

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
                    answerKeyFieldsTable.RowCount = totalFields + 1;

                    for (var i = 0; i < totalFields; i++)
                    {
                        answerKeyFieldsTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));
                        var keyFieldControl = new AnswerKeyFieldControl();
                        answerKeyFieldsTable.Controls.Add(keyFieldControl);
                        answerKeyFieldsTable.SetRow(keyFieldControl, i);
                        keyFieldControl.Initialize(i + 1);
                        keyFieldControl.TotalOptions = totalOptions;
                        keyFieldControl.FieldOrientation = orientation;
                        keyFieldControl.OptionsValueType = omrConfiguration.ValueDataType;
                        keyFieldControl.Dock = DockStyle.Fill;
                    }

                    break;

                case Orientation.Vertical:
                    answerKeyFieldsTable.RowCount = 1;
                    answerKeyFieldsTable.ColumnCount = totalFields + 1;


                    for (var i = 0; i < totalFields; i++)
                    {
                        answerKeyFieldsTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
                        var keyFieldControl = new AnswerKeyFieldControl();
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

        private void PopulateKeyFields(AnswerKey key)
        {
            var curTotalFields = answerKeyFieldsTable.Controls.Count;
            if (curTotalFields > 0)
            {
                var keyFieldControl = (AnswerKeyFieldControl)answerKeyFieldsTable.Controls[0];
                var curTotalOptions = keyFieldControl.TotalOptions;

                var _key = key.GetKey.ToList();
                if (_key.Count == curTotalFields && _key.TrueForAll(x => x.Length == curTotalOptions + 1))
                {
                    for (var i = 0; i < curTotalFields; i++)
                    {
                        var _keyFieldControl =
                            (AnswerKeyFieldControl)answerKeyFieldsTable.Controls[i];
                        _keyFieldControl.SetOptions(_key[i]);
                    }
                }
            }
        }

        private void ClearAnswerKeyFields()
        {
            var curTotalFields = answerKeyFieldsTable.Controls.Count;

            for (var i = 0; i < curTotalFields; i++)
            {
                var _keyFieldControl = (AnswerKeyFieldControl)answerKeyFieldsTable.Controls[i];
                _keyFieldControl.ClearOptions();
            }
        }

        private Action DeleteToEditKeyItem;

        private void DeleteAnswerKeyItem(OMRConfiguration omrConfig, AnswerKeyListItem keyListItem, bool ask = true)
        {
            if (omrConfig.GeneralAnswerKeys != null &&
                omrConfig.GeneralAnswerKeys.Exists(x => x.Title == keyListItem.KeyTitle))
            {
                if (!ask || Messages.ShowQuestion(
                        $"Answer Key: {keyListItem.KeyTitle} - Configuration: {omrConfig.Title ?? "N/A"} \nAre you sure you want to delete this answer key?") ==
                    DialogResult.Yes)
                {
                    omrConfig.GeneralAnswerKeys.Remove(
                        omrConfig.GeneralAnswerKeys.First(x => x.Title == keyListItem.KeyTitle));
                    answerKeyListFlowPanel.Controls.Remove(keyListItem);
                    keyListItem.Dispose();
                }
            }
            else if (omrConfig.PB_AnswerKeys != null && omrConfig.PB_AnswerKeys.Count > 0 &&
                     omrConfig.PB_AnswerKeys.Values.Any(x => x.Title == keyListItem.KeyTitle))
            {
                var dicItem = omrConfig.PB_AnswerKeys.First(x => x.Value.Title == keyListItem.KeyTitle);
                if (!ask || Messages.ShowQuestion(
                        $"Answer Key: {keyListItem.KeyTitle} - Parameter: {dicItem.Key.parameterConfig.Title ?? "N/A"} - Configuration: {omrConfig.Title ?? "N/A"} \nAre you sure you want to delete this answer key?") ==
                    DialogResult.Yes)
                {
                    omrConfig.PB_AnswerKeys.Remove(dicItem.Key);
                    answerKeyListFlowPanel.Controls.Remove(keyListItem);
                    keyListItem.Dispose();
                }
            }

            if (answerKeyListFlowPanel.Controls.Count == 0)
            {
                answerKeyListFlowPanel.Controls.Add(answerKeysEmptyListLabel);
                answerKeysEmptyListLabel.Dock = DockStyle.Fill;
                answerKeysEmptyListLabel.Visible = true;
            }
        }

        private async void setBtn_Click(object sender, EventArgs e)
        {
            var selectedOMRConfig = (OMRConfiguration)configurationComboBox.SelectedItem;
            if (selectedOMRConfig == null)
            {
                return;
            }

            var selectedPaper = (Paper)answerKeyPaperComboBox.SelectedItem;
            if (selectedPaper == null)
            {
                Messages.ShowError("Please select a valid paper or create one to link with the answer key.");
                return;
            }

            var totalFields = selectedPaper.GetFieldsCount;
            //if (totalFields > selectedOMRConfig.GetTotalFields)
            //{
            //    Messages.ShowError(
            //        "Please select a valid paper or create one to link with the answer key. \n\nValidation Error: Paper fields cannot be greater than total fields of the selected gradable region.");
            //    return;
            //}

            var totalOptions = selectedPaper.GetOptionsCount;

            var keyTitle = answerKeyTitleField.Text;
            var answerKeyOptions = new int[totalFields][];

            for (var i = 0; i < totalFields; i++)
            {
                if (i > answerKeyFieldsTable.Controls.Count)
                {
                    Messages.ShowError(
                        "Couldn't add the answer key because not enough fields found to satisfy the paper.");
                    return;
                }

                var curField = (AnswerKeyFieldControl)answerKeyFieldsTable.Controls[i];
                var optionsValues = new int[totalOptions + 1];
                var marked = curField.GetMarkedOptions();
                if (marked == null)
                {
                    Messages.ShowError("Couldn't add the answer key due to invalid answer key fields.");
                    return;
                }

                if (marked.Length == 0)
                {
                    Messages.ShowError(
                        "Couldn't add the answer key due to invalid answer key fields, All fields must be filled.");
                    return;
                }

                for (var j = 0; j < marked.Length; j++) optionsValues[marked[j]] = 1;

                answerKeyOptions[i] = optionsValues;
            }

            var answerKey = new AnswerKey(keyTitle, selectedOMRConfig.Title, answerKeyOptions, selectedPaper);

            DeleteToEditKeyItem?.Invoke();
            DeleteToEditKeyItem = null;

            var isSuccess = false;
            var err = "";
            switch (selectedOMRConfig.KeyType)
            {
                case KeyType.General:
                    isSuccess = selectedOMRConfig.SetGeneralAnswerKey(answerKey, out err);
                    if (!isSuccess && err != "User Denied")
                    {
                        Messages.ShowError("Couldn't add the answer key due to invalid answer key. \n\n Error: " + err);
                    }

                    break;

                case KeyType.ParameterBased:
                    var paramConfig = (ConfigurationBase)answerKeyParameterField.SelectedItem;
                    if (paramConfig == null)
                    {
                        Messages.ShowError(
                            "Couldn't add the answer key due to invalid answer key parameter. \n\n Please select a valid parameter to link with the answer key.");
                        return;
                    }

                    var paramValue = answerKeyParameterValueField.Text;
                    if (paramValue == "")
                    {
                        Messages.ShowError(
                            "Couldn't add the answer key due to invalid answer key parameter. \n\n Please select a valid parameter value to link with the answer key parameter.");
                        return;
                    }

                    var keyParameter = new Parameter(paramConfig, paramValue);
                    isSuccess = selectedOMRConfig.AddPBAnswerKey(keyParameter, answerKey, out err);
                    if (!isSuccess && err != "User Denied")
                    {
                        Messages.ShowError("Couldn't add the key due to invalid answer key properties. \n\n Error: " +
                                           err);
                    }

                    break;
            }

            if (isSuccess)
            {
                await this.PopulateAnswerKeyFields((List<OMRConfiguration>)configurationComboBox.DataSource);

                addAnswerKeyPanel.Visible = false;
                answerKeyMainPanel.Visible = true;
                answerKeyMainPanel.Dock = DockStyle.Fill;
            }
        }

        private void OnAnswerKeyListItemControlButtonPressed(object sender,
            AnswerKeyListItem.ControlButton controlButton)
        {
            var keyListItem = (AnswerKeyListItem)sender;
            var omrConfig =
                (OMRConfiguration)ConfigurationsManager.GetConfiguration(keyListItem.ConfigTitle);
            switch (controlButton)
            {
                case AnswerKeyListItem.ControlButton.Delete:
                    this.DeleteAnswerKeyItem(omrConfig, keyListItem);
                    break;

                case AnswerKeyListItem.ControlButton.Configure:
                    DeleteToEditKeyItem = () => { this.DeleteAnswerKeyItem(omrConfig, keyListItem, false); };

                    answerKeyMainPanel.Visible = false;
                    addAnswerKeyPanel.Dock = DockStyle.Fill;
                    var exmPapers = GeneralManager.GetExamPapers;
                    answerKeyPaperComboBox.DataSource = exmPapers != null ? exmPapers.GetPapers : null;
                    answerKeyPaperComboBox.DisplayMember = "Title";
                    if (omrConfig.GeneralAnswerKeys != null &&
                        omrConfig.GeneralAnswerKeys.Exists(x => x.Title == keyListItem.KeyTitle))
                    {
                        var answerKey = omrConfig.GeneralAnswerKeys.Find(x => x.Title == keyListItem.KeyTitle);

                        configurationComboBox.SelectedItem = omrConfig;
                        answerKeyTitleField.Text = answerKey.Title;

                        answerKeyPaperComboBox.SelectedItem = answerKey.GetPaper;

                        this.PopulateKeyFields(answerKey);
                    }
                    else if (omrConfig.PB_AnswerKeys != null && omrConfig.PB_AnswerKeys.Count > 0 &&
                             omrConfig.PB_AnswerKeys.Values.Any(x => x.Title == keyListItem.KeyTitle))
                    {
                        var dicItem = omrConfig.PB_AnswerKeys.First(x => x.Value.Title == keyListItem.KeyTitle);

                        configurationComboBox.SelectedItem = omrConfig;
                        answerKeyTitleField.Text = keyListItem.KeyTitle;
                        answerKeyParameterField.Text = dicItem.Key.parameterConfig.Title;
                        answerKeyParameterValueField.Text = dicItem.Key.parameterValue;

                        answerKeyPaperComboBox.SelectedItem = dicItem.Value.GetPaper;

                        this.PopulateKeyFields(dicItem.Value);
                    }

                    addAnswerKeyPanel.Visible = true;
                    break;

                case AnswerKeyListItem.ControlButton.Active:
                    if (omrConfig.GeneralAnswerKeys != null &&
                        omrConfig.GeneralAnswerKeys.Exists(x => x.Title == keyListItem.KeyTitle))
                    {
                        omrConfig.GeneralAnswerKeys.Find(x => x.Title == keyListItem.KeyTitle).IsActive = true;
                    }
                    else if (omrConfig.PB_AnswerKeys != null && omrConfig.PB_AnswerKeys.Count > 0 &&
                             omrConfig.PB_AnswerKeys.Values.Any(x => x.Title == keyListItem.KeyTitle))
                    {
                        omrConfig.PB_AnswerKeys.First(x => x.Value.Title == keyListItem.KeyTitle).Value.IsActive = true;
                    }

                    this.UpdateGridColumns();
                    break;

                case AnswerKeyListItem.ControlButton.Inactive:
                    if (omrConfig.GeneralAnswerKeys != null &&
                        omrConfig.GeneralAnswerKeys.Exists(x => x.Title == keyListItem.KeyTitle))
                    {
                        omrConfig.GeneralAnswerKeys.Find(x => x.Title == keyListItem.KeyTitle).IsActive = false;
                    }
                    else if (omrConfig.PB_AnswerKeys != null && omrConfig.PB_AnswerKeys.Count > 0 &&
                             omrConfig.PB_AnswerKeys.Values.Any(x => x.Title == keyListItem.KeyTitle))
                    {
                        omrConfig.PB_AnswerKeys.First(x => x.Value.Title == keyListItem.KeyTitle).Value.IsActive =
                            false;
                    }

                    this.UpdateGridColumns();
                    break;
            }
        }

        private void markAsNormalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            selectedDataRows.ForEach(row => { this.MarkItemAs(row, ProcessedDataType.NORMAL); });
        }

        private void markAsManualToolStripMenuItem_Click(object sender, EventArgs e)
        {
            selectedDataRows.ForEach(row => { this.MarkItemAs(row, ProcessedDataType.MANUAL); });
        }

        private void markAsFaultyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            selectedDataRows.ForEach(row => { this.MarkItemAs(row, ProcessedDataType.FAULTY); });
        }

        private void markAsIncompatibleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            selectedDataRows.ForEach(row => { this.MarkItemAs(row, ProcessedDataType.INCOMPATIBLE); });
        }

        #endregion

        private async void ScanDirectoryToolStripBtn_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                var selectedFolder = folderBrowserDialog.SelectedPath;
                var includeSubDirs = includeSubFoldersToolStripCheckBox.Checked;

                await this.InitializeSheetsToRead(selectedFolder, includeSubDirs);
                if (loadedSheetsData.SheetsLoaded)
                {
                    this.UpdateStatus("Scan Directory Loaded, Successfully");
                }
                else
                {
                    this.UpdateStatus("Directory To Scan Failed To Load");
                }
            }
        }

        private void UpdateImageSelection(ProcessedDataRow processedDataRow, bool locateRegions = false)
        {
            var showBackImage = !string.IsNullOrEmpty(processedDataRow.RowBackSheetPath);
            if (showBackImage)
            {
                mainDockingManager.SetDockLabel(dataImageBoxPanel,
                    $"Front Image - {Path.GetFileName(processedDataRow.RowSheetPath)} - Row: {processedDataRow.GetRowIndex + 1}");

                mainDockingManager.SetDockLabel(dataBackImageBoxPanel,
                    $"Back Image - {Path.GetFileName(processedDataRow.RowBackSheetPath)} - Row: {processedDataRow.GetRowIndex + 1}");
                mainDockingManager.SetDockVisibility(dataBackImageBoxPanel, true);

                if (!locateRegions)
                {
                    dataBackImageBox.Image = new Bitmap(processedDataRow.RowBackSheetPath);
                }
                else
                {
                    Mat backAlignedMat = null;
                    if (!GetCurrentTemplate.GetAlignedImage(processedDataRow.RowBackSheetPath,
                        processedDataRow.RereadType,
                        out backAlignedMat, out var _))
                    {
                        dataBackImageBox.Image = new Bitmap(processedDataRow.RowBackSheetPath);
                        return;
                    }

                    dataBackImageBox.Image = backAlignedMat.Bitmap;
                }
            }
            else
            {
                mainDockingManager.SetDockLabel(dataImageBoxPanel,
                    $"Image - {Path.GetFileName(processedDataRow.RowSheetPath)} - Row: {processedDataRow.GetRowIndex + 1}");

                mainDockingManager.SetDockVisibility(dataBackImageBoxPanel, false);
            }

            if (!locateRegions)
            {
                dataImageBox.Image = new Bitmap(processedDataRow.RowSheetPath);
                return;
            }

            Mat alignedMat = null;
            if (!GetCurrentTemplate.GetAlignedImage(processedDataRow.RowSheetPath, processedDataRow.RereadType,
                out alignedMat, out var _))
            {
                dataImageBox.Image = new Bitmap(processedDataRow.RowSheetPath);
                return;
            }

            dataImageBox.Image = alignedMat.Bitmap;

            curOptionRects.Clear();
            curMarkedOptionRects.Clear();
            curBackOptionRects.Clear();
            curMarkedBackOptionRects.Clear();
            var entries = processedDataRow.GetProcessedDataEntries;
            for (var i = 0; i < entries.Count; i++)
            {
                if (entries[i].GetFieldsOutputs.All(x => x == '-'))
                {
                    continue;
                }

                switch (entries[i].GetMainConfigType)
                {
                    case MainConfigType.OMR:
                        var markedRectIndexes = new List<Point>();
                        var omrConfig = (OMRConfiguration)entries[i].GetConfigurationBase;
                        var rawDataValues = ProcessedDataEntry.GenerateRawOMRDataValues(omrConfig,
                            entries[i].GetFieldsOutputs, omrConfig.GetEscapeSymbols());
                        var totalFields = omrConfig.GetTotalFields;
                        var totalOptions = omrConfig.GetTotalOptions;
                        for (var i1 = 0; i1 < totalFields; i1++)
                        for (var j = 0; j < totalOptions; j++)
                            if (rawDataValues[i1, j] == 1)
                            {
                                markedRectIndexes.Add(new Point(i1, j));
                            }

                        var _curOptionRects = omrConfig.RegionData.GetOptionsRects;
                        var _alignedCurOptionRects = new List<RectangleF>();
                        var regionLocation = omrConfig.GetConfigArea.ConfigRect.Location;
                        for (var i1 = 0; i1 < _curOptionRects.Count; i1++)
                        {
                            var optionRect = _curOptionRects[i1];
                            optionRect.X += regionLocation.X;
                            optionRect.Y += regionLocation.Y;

                            curOptionRects.Add(optionRect);
                            _alignedCurOptionRects.Add(optionRect);
                        }

                        for (var i2 = 0; i2 < markedRectIndexes.Count; i2++)
                        {
                            var index = markedRectIndexes[i2].X * totalOptions + markedRectIndexes[i2].Y;
                            curMarkedOptionRects.Add(_alignedCurOptionRects[index]);
                        }

                        curOptionRects.RemoveAll(x => curMarkedOptionRects.Contains(x));

                        if (!string.IsNullOrEmpty(entries[i].BackConfigurationTitle) && showBackImage)
                        {
                            var omrBackConfig = (OMRConfiguration)entries[i].GetBackConfigurationBase;
                            markedRectIndexes = new List<Point>();
                            var totalBackFields = omrBackConfig.GetTotalFields;
                            var totalBackOptions = omrBackConfig.GetTotalOptions;
                            var fieldOutputs = new char[totalBackFields];
                            for (var j = totalFields; j < entries[i].GetFieldsOutputs.Length; j++)
                                fieldOutputs[j - totalFields] = entries[i].GetFieldsOutputs[j];
                            rawDataValues = ProcessedDataEntry.GenerateRawOMRDataValues(omrBackConfig, fieldOutputs,
                                omrBackConfig.GetEscapeSymbols());

                            for (var i1 = 0; i1 < totalBackFields; i1++)
                            for (var j = 0; j < totalBackOptions; j++)
                            {
                                if (i1 >= rawDataValues.GetLength(0) || j >= rawDataValues.GetLength(1))
                                {
                                    continue;
                                }

                                if (rawDataValues[i1, j] == 1)
                                {
                                    markedRectIndexes.Add(new Point(i1, j));
                                }
                            }

                            _curOptionRects = omrBackConfig.RegionData.GetOptionsRects;
                            _alignedCurOptionRects = new List<RectangleF>();
                            regionLocation = omrBackConfig.GetConfigArea.ConfigRect.Location;

                            for (var i1 = 0; i1 < _curOptionRects.Count; i1++)
                            {
                                var optionRect = _curOptionRects[i1];
                                optionRect.X += regionLocation.X;
                                optionRect.Y += regionLocation.Y;

                                curBackOptionRects.Add(optionRect);
                                _alignedCurOptionRects.Add(optionRect);
                            }

                            for (var i2 = 0; i2 < markedRectIndexes.Count; i2++)
                            {
                                var index = markedRectIndexes[i2].X * totalOptions + markedRectIndexes[i2].Y;
                                curMarkedBackOptionRects.Add(_alignedCurOptionRects[index]);
                            }

                            curBackOptionRects.RemoveAll(x => curMarkedBackOptionRects.Contains(x));
                        }

                        break;

                    case MainConfigType.BARCODE:
                        if (entries[i].BarcodesResult == null || entries[i].BarcodesResult.Length == 0)
                        {
                            continue;
                        }

                        var barcodeConfig = (OBRConfiguration)entries[i].GetConfigurationBase;

                        var barcodeRegionLocation = barcodeConfig.GetConfigArea.ConfigRect.Location;
                        RectangleF barcodeRect = entries[i].BarcodesResult[0].Rectangle;
                        barcodeRect.X += barcodeRegionLocation.X;
                        barcodeRect.Y += barcodeRegionLocation.Y;

                        curOptionRects.Add(barcodeRect);
                        break;

                    case MainConfigType.ICR:
                        break;
                }
            }
        }

        private void MainDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0)
            {
                curOptionRects.Clear();
                curMarkedOptionRects.Clear();

                dataImageBox.Image = null;
                return;
            }

            var lastItemData = (dynamic)e.AddedItems.Last();
            //if (e.AddedItems.RowType != Syncfusion.WinForms.DataGrid.Enums.RowType.DefaultRow)
            //    return;
            if (lastItemData == null)
            {
                return;
            }

            this.SelectedProcessedDataRow = (ProcessedDataRow)lastItemData.DataRowObject;
            selectedDataRows = mainDataGrid.SelectedItems.Cast<dynamic>().ToList();
            this.UpdateImageSelection(this.SelectedProcessedDataRow.Value, this.GetLocateRegionsToggle);
        }

        private void mainDataGrid_QueryCellStyle(object sender, QueryCellStyleEventArgs e)
        {
            var dataObject = (dynamic)e.DataRow.RowData;
            var colText = e.Column.HeaderText;
            if (!GridCellsRepresentation.ContainsKey(colText))
            {
                return;
            }

            var fieldDataType = ProcessedDataType.NORMAL;

            (int entryIndex, int fieldIndex) cellRepresentation;
            ProcessedDataEntry? _processedDataEntry = null;
            try
            {
                cellRepresentation = GridCellsRepresentation[colText];
                _processedDataEntry = dataObject.DataRowObject.GetProcessedDataEntries[cellRepresentation.entryIndex];
                if (!_processedDataEntry.HasValue)
                {
                    return;
                }

                var ambigiousFieldType = _processedDataEntry.Value.GetRegionDataType();
                fieldDataType = ambigiousFieldType.HasValue
                    ? ambigiousFieldType.Value
                    : _processedDataEntry.Value.DataEntriesResultType[
                        cellRepresentation.fieldIndex == 0 ? 0 : cellRepresentation.fieldIndex - 1];
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}\nColumn: {e.Column.HeaderText} - {e.DisplayText}\nRow: {e.RowIndex}");
                return;
            }

            var processedDataEntry = _processedDataEntry.Value;
            switch (fieldDataType)
            {
                case ProcessedDataType.INCOMPATIBLE:
                    if (incompatibleDataCellBackColor != Color.Empty)
                    {
                        e.Style.BackColor = CurrentTheme == Themes.BLACK
                            ? Color.FromArgb(45, incompatibleDataCellBackColor)
                            : incompatibleDataCellBackColor;
                    }

                    if (incompatibleDataCellForeColor != Color.Empty)
                    {
                        e.Style.TextColor = incompatibleDataCellForeColor;
                    }

                    break;

                case ProcessedDataType.FAULTY:
                    if (faultyDataCellBackColor != Color.Empty)
                    {
                        e.Style.BackColor = CurrentTheme == Themes.BLACK
                            ? Color.FromArgb(45, faultyDataCellBackColor)
                            : faultyDataCellBackColor;
                    }

                    if (faultyDataCellForeColor != Color.Empty)
                    {
                        e.Style.TextColor = faultyDataCellForeColor;
                    }

                    break;

                case ProcessedDataType.MANUAL:
                    if (manualDataCellBackColor != Color.Empty)
                    {
                        e.Style.BackColor = CurrentTheme == Themes.BLACK
                            ? Color.FromArgb(45, manualDataCellBackColor)
                            : manualDataCellBackColor;
                    }

                    if (manualDataCellForeColor != Color.Empty)
                    {
                        e.Style.TextColor = manualDataCellForeColor;
                    }

                    break;

                case ProcessedDataType.NORMAL:
                    break;
            }

            if (processedDataEntry.SpecialCells.Exists(x =>
                x.cell.entryIndex == cellRepresentation.entryIndex &&
                x.cell.fieldIndex == cellRepresentation.fieldIndex))
            {
                var specialCell = processedDataEntry.SpecialCells.Find(x =>
                    x.cell.entryIndex == cellRepresentation.entryIndex &&
                    x.cell.fieldIndex == cellRepresentation.fieldIndex);

                e.Style.BackColor = specialCell.cellBackColor.Blend(e.Style.BackColor, 0.6);
                e.Style.TextColor = specialCell.cellForeColor;
            }
        }

        private void MainDataGrid_QueryRowStyle(object sender, QueryRowStyleEventArgs e)
        {
            e.Style.HorizontalAlignment = HorizontalAlignment.Center;
            if (e.RowType != RowType.DefaultRow)
            {
                return;
            }

            var dataObject = (dynamic)e.RowData;

            var fieldDataType = ProcessedDataType.NORMAL;

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
                    if (incompatibleDataRowBackColor != Color.Empty)
                    {
                        e.Style.BackColor = CurrentTheme == Themes.BLACK
                            ? Color.FromArgb(1, incompatibleDataCellBackColor)
                            : incompatibleDataRowBackColor;
                    }

                    if (incompatibleDataRowForeColor != Color.Empty)
                    {
                        e.Style.TextColor = incompatibleDataRowForeColor;
                    }

                    break;

                case ProcessedDataType.FAULTY:
                    if (faultyDataRowBackColor != Color.Empty)
                    {
                        e.Style.BackColor = CurrentTheme == Themes.BLACK
                            ? Color.FromArgb(1, faultyDataRowBackColor)
                            : faultyDataRowBackColor;
                    }

                    if (faultyDataRowForeColor != Color.Empty)
                    {
                        e.Style.TextColor = faultyDataRowForeColor;
                    }

                    break;

                case ProcessedDataType.MANUAL:
                    if (manualDataRowBackColor != Color.Empty)
                    {
                        e.Style.BackColor = CurrentTheme == Themes.BLACK
                            ? Color.FromArgb(1, manualDataRowBackColor)
                            : manualDataRowBackColor;
                    }

                    if (manualDataRowForeColor != Color.Empty)
                    {
                        e.Style.TextColor = manualDataRowForeColor;
                    }

                    break;

                case ProcessedDataType.NORMAL:
                    break;
            }
        }

        private void mainDataGrid_QueryProgressBarCellStyle(object sender, QueryProgressBarCellStyleEventArgs e)
        {
            if (!e.Column.HeaderText.Contains(" Score"))
            {
                return;
            }

            var dataObject = (dynamic)e.Record;
            AnswerKey ansKey = Functions.GetProperty(dataObject, "AnswerKey");
            if (ansKey == null)
            {
                return;
            }

            var maximum = ansKey.GetPaper.GetCorrectOptionValue * ansKey.GetKey.Length;
            e.Maximum = maximum == 0 ? 100 : maximum;

            e.Style.ProgressTextColor = CurrentTheme == Themes.BLACK || CurrentTheme == Themes.DARK_GRAY
                ? Color.White
                : e.Style.ProgressTextColor;
            e.Style.BackgroundColor = CurrentTheme == Themes.DARK_GRAY
                ? Color.FromArgb(210, 210, 210)
                : e.Style.BackgroundColor;
            //mainDataGrid.View.
        }

        private void manualDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0)
            {
                curOptionRects.Clear();
                curMarkedOptionRects.Clear();

                dataImageBox.Image = null;
                return;
            }

            var lastItemData = (dynamic)e.AddedItems.Last();
            //if (e.AddedItems.RowType != Syncfusion.WinForms.DataGrid.Enums.RowType.DefaultRow)
            //    return;
            if (lastItemData == null)
            {
                return;
            }

            this.SelectedProcessedDataRow = (ProcessedDataRow)lastItemData.DataRowObject;
            selectedDataRows = manualDataGrid.SelectedItems.Cast<dynamic>().ToList();
            this.UpdateImageSelection(this.SelectedProcessedDataRow.Value, this.GetLocateRegionsToggle);
        }

        private void manualDataGrid_QueryCellStyle(object sender, QueryCellStyleEventArgs e)
        {
            var dataObject = (dynamic)e.DataRow.RowData;
            var colText = e.Column.HeaderText;
            if (!GridCellsRepresentation.ContainsKey(colText))
            {
                return;
            }

            var fieldDataType = ProcessedDataType.NORMAL;
            ProcessedDataRow processedDataRow = dataObject.DataRowObject;

            (int entryIndex, int fieldIndex) cellRepresentation = (0, 0);
            ProcessedDataEntry? _processedDataEntry = null;
            try
            {
                cellRepresentation = GridCellsRepresentation[colText];
                _processedDataEntry = processedDataRow.GetProcessedDataEntries[cellRepresentation.entryIndex];
                if (!_processedDataEntry.HasValue)
                {
                    return;
                }

                var ambigiousFieldType = _processedDataEntry.Value.GetRegionDataType();
                fieldDataType = ambigiousFieldType.HasValue
                    ? ambigiousFieldType.Value
                    : _processedDataEntry.Value.DataEntriesResultType[
                        cellRepresentation.fieldIndex == 0 ? 0 : cellRepresentation.fieldIndex - 1];
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}\nColumn: {e.Column.HeaderText} - {e.DisplayText}\nRow: {e.RowIndex}");
                return;
            }

            var processedDataEntry = _processedDataEntry.Value;
            switch (fieldDataType)
            {
                case ProcessedDataType.INCOMPATIBLE:
                    if (incompatibleDataCellBackColor != Color.Empty)
                    {
                        e.Style.BackColor = CurrentTheme == Themes.BLACK
                            ? Color.FromArgb(45, incompatibleDataCellBackColor)
                            : incompatibleDataCellBackColor;
                    }

                    if (incompatibleDataCellForeColor != Color.Empty)
                    {
                        e.Style.TextColor = incompatibleDataCellForeColor;
                    }

                    break;

                case ProcessedDataType.FAULTY:
                    if (faultyDataCellBackColor != Color.Empty)
                    {
                        e.Style.BackColor = CurrentTheme == Themes.BLACK
                            ? Color.FromArgb(45, faultyDataCellBackColor)
                            : faultyDataCellBackColor;
                    }

                    if (faultyDataCellForeColor != Color.Empty)
                    {
                        e.Style.TextColor = faultyDataCellForeColor;
                    }

                    break;

                case ProcessedDataType.MANUAL:
                    if (manualDataCellBackColor != Color.Empty)
                    {
                        e.Style.BackColor = CurrentTheme == Themes.BLACK
                            ? Color.FromArgb(45, manualDataCellBackColor)
                            : manualDataCellBackColor;
                    }

                    if (manualDataCellForeColor != Color.Empty)
                    {
                        e.Style.TextColor = manualDataCellForeColor;
                    }

                    break;

                case ProcessedDataType.NORMAL:
                    break;
            }

            if (processedDataEntry.IsEdited)
            {
                if (editedDataCellBackColor != Color.Empty)
                {
                    e.Style.BackColor = editedDataCellBackColor;
                }

                if (editedDataCellForeColor != Color.Empty)
                {
                    e.Style.TextColor = editedDataCellForeColor;
                }
            }

            if (processedDataEntry.SpecialCells.Exists(x =>
                x.cell.entryIndex == cellRepresentation.entryIndex &&
                x.cell.fieldIndex == cellRepresentation.fieldIndex))
            {
                var specialCell = processedDataEntry.SpecialCells.Find(x =>
                    x.cell.entryIndex == cellRepresentation.entryIndex &&
                    x.cell.fieldIndex == cellRepresentation.fieldIndex);

                e.Style.BackColor = specialCell.cellBackColor.Blend(e.Style.BackColor, 0.6);
                e.Style.TextColor = specialCell.cellForeColor;
            }
        }

        private void manualDataGrid_QueryRowStyle(object sender, QueryRowStyleEventArgs e)
        {
            e.Style.HorizontalAlignment = HorizontalAlignment.Center;

            if (e.RowType != RowType.DefaultRow)
            {
                return;
            }

            var dataObject = (dynamic)e.RowData;
            ProcessedDataRow processedDataRow = dataObject.DataRowObject;

            if (processedDataRow.IsEdited)
            {
                if (editedDataRowBackColor != Color.Empty)
                {
                    e.Style.BackColor = editedDataRowBackColor;
                }

                if (editedDataRowForeColor != Color.Empty)
                {
                    e.Style.TextColor = editedDataRowForeColor;
                }
            }
        }

        private void manualDataGrid_QueryProgressBarCellStyle(object sender, QueryProgressBarCellStyleEventArgs e)
        {
            if (!e.Column.HeaderText.Contains(" Score"))
            {
                return;
            }

            var dataObject = (dynamic)e.Record;
            AnswerKey ansKey = Functions.GetProperty(dataObject, "AnswerKey");
            if (ansKey == null)
            {
                return;
            }

            var maximum = ansKey.GetPaper.GetCorrectOptionValue * ansKey.GetKey.Length;
            e.Maximum = maximum == 0 ? 100 : maximum;

            e.Style.ProgressTextColor = CurrentTheme == Themes.BLACK || CurrentTheme == Themes.DARK_GRAY
                ? Color.White
                : e.Style.ProgressTextColor;
            e.Style.BackgroundColor = CurrentTheme == Themes.DARK_GRAY
                ? Color.FromArgb(210, 210, 210)
                : e.Style.BackgroundColor;
        }

        private void faultyDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0)
            {
                curOptionRects.Clear();
                curMarkedOptionRects.Clear();

                dataImageBox.Image = null;
                return;
            }

            var lastItemData = (dynamic)e.AddedItems.Last();
            //if (e.AddedItems.RowType != Syncfusion.WinForms.DataGrid.Enums.RowType.DefaultRow)
            //    return;
            if (lastItemData == null)
            {
                return;
            }

            this.SelectedProcessedDataRow = (ProcessedDataRow)lastItemData.DataRowObject;
            selectedDataRows = faultyDataGrid.SelectedItems.Cast<dynamic>().ToList();
            this.UpdateImageSelection(this.SelectedProcessedDataRow.Value, this.GetLocateRegionsToggle);
        }

        private void faultyDataGrid_QueryCellStyle(object sender, QueryCellStyleEventArgs e)
        {
            var dataObject = (dynamic)e.DataRow.RowData;
            var colText = e.Column.HeaderText;
            if (!GridCellsRepresentation.ContainsKey(colText))
            {
                return;
            }

            var fieldDataType = ProcessedDataType.NORMAL;

            try
            {
                var cellRepresentation = GridCellsRepresentation[colText];
                fieldDataType = dataObject.DataRowObject.GetProcessedDataEntries[cellRepresentation.entryIndex]
                    .DataEntriesResultType[cellRepresentation.fieldIndex];
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}\nColumn: {e.Column.HeaderText} - {e.DisplayText}\nRow: {e.RowIndex}");
                return;
            }

            switch (fieldDataType)
            {
                case ProcessedDataType.INCOMPATIBLE:
                    if (incompatibleDataCellBackColor != Color.Empty)
                    {
                        e.Style.BackColor = CurrentTheme == Themes.BLACK
                            ? Color.FromArgb(45, incompatibleDataCellBackColor)
                            : incompatibleDataCellBackColor;
                    }

                    if (incompatibleDataCellForeColor != Color.Empty)
                    {
                        e.Style.TextColor = incompatibleDataCellForeColor;
                    }

                    break;

                case ProcessedDataType.FAULTY:
                    if (faultyDataCellBackColor != Color.Empty)
                    {
                        e.Style.BackColor = CurrentTheme == Themes.BLACK
                            ? Color.FromArgb(45, faultyDataCellBackColor)
                            : faultyDataCellBackColor;
                    }

                    if (faultyDataCellForeColor != Color.Empty)
                    {
                        e.Style.TextColor = faultyDataCellForeColor;
                    }

                    break;

                case ProcessedDataType.MANUAL:
                    if (manualDataCellBackColor != Color.Empty)
                    {
                        e.Style.BackColor = CurrentTheme == Themes.BLACK
                            ? Color.FromArgb(45, manualDataCellBackColor)
                            : manualDataCellBackColor;
                    }

                    if (manualDataCellForeColor != Color.Empty)
                    {
                        e.Style.TextColor = manualDataCellForeColor;
                    }

                    break;

                case ProcessedDataType.NORMAL:
                    break;
            }
        }

        private void faultyDataGrid_QueryRowStyle(object sender, QueryRowStyleEventArgs e)
        {
            e.Style.HorizontalAlignment = HorizontalAlignment.Center;
        }

        private void faultyDataGrid_QueryProgressBarCellStyle(object sender, QueryProgressBarCellStyleEventArgs e)
        {
            if (!e.Column.HeaderText.Contains(" Score"))
            {
                return;
            }

            var dataObject = (dynamic)e.Record;
            AnswerKey ansKey = Functions.GetProperty(dataObject, "AnswerKey");
            if (ansKey == null)
            {
                return;
            }

            var maximum = ansKey.GetPaper.GetCorrectOptionValue * ansKey.GetKey.Length;
            e.Maximum = maximum == 0 ? 100 : maximum;

            e.Style.ProgressTextColor = CurrentTheme == Themes.BLACK || CurrentTheme == Themes.DARK_GRAY
                ? Color.White
                : e.Style.ProgressTextColor;
            e.Style.BackgroundColor = CurrentTheme == Themes.DARK_GRAY
                ? Color.FromArgb(210, 210, 210)
                : e.Style.BackgroundColor;
            //mainDataGrid.View.
        }

        private void incompatibleDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0)
            {
                curOptionRects.Clear();
                curMarkedOptionRects.Clear();

                dataImageBox.Image = null;
                return;
            }

            var lastItemData = (dynamic)e.AddedItems.Last();
            //if (e.AddedItems.RowType != Syncfusion.WinForms.DataGrid.Enums.RowType.DefaultRow)
            //    return;
            if (lastItemData == null)
            {
                return;
            }

            this.SelectedProcessedDataRow = (ProcessedDataRow)lastItemData.DataRowObject;
            selectedDataRows = incompatibleDataGrid.SelectedItems.Cast<dynamic>().ToList();
            this.UpdateImageSelection(this.SelectedProcessedDataRow.Value, this.GetLocateRegionsToggle);
        }

        private void incompatibleDataGrid_QueryCellStyle(object sender, QueryCellStyleEventArgs e)
        {
            var dataObject = (dynamic)e.DataRow.RowData;
            var colText = e.Column.HeaderText;
            if (!GridCellsRepresentation.ContainsKey(colText))
            {
                return;
            }

            var fieldDataType = ProcessedDataType.NORMAL;

            try
            {
                var cellRepresentation = GridCellsRepresentation[colText];
                fieldDataType = dataObject.DataRowObject.GetProcessedDataEntries[cellRepresentation.entryIndex]
                    .DataEntriesResultType[cellRepresentation.fieldIndex];
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}\nColumn: {e.Column.HeaderText} - {e.DisplayText}\nRow: {e.RowIndex}");
                return;
            }

            switch (fieldDataType)
            {
                case ProcessedDataType.INCOMPATIBLE:
                    if (incompatibleDataCellBackColor != Color.Empty)
                    {
                        e.Style.BackColor = CurrentTheme == Themes.BLACK
                            ? Color.FromArgb(45, incompatibleDataCellBackColor)
                            : incompatibleDataCellBackColor;
                    }

                    if (incompatibleDataCellForeColor != Color.Empty)
                    {
                        e.Style.TextColor = incompatibleDataCellForeColor;
                    }

                    break;

                case ProcessedDataType.FAULTY:
                    if (faultyDataCellBackColor != Color.Empty)
                    {
                        e.Style.BackColor = CurrentTheme == Themes.BLACK
                            ? Color.FromArgb(45, faultyDataCellBackColor)
                            : faultyDataCellBackColor;
                    }

                    if (faultyDataCellForeColor != Color.Empty)
                    {
                        e.Style.TextColor = faultyDataCellForeColor;
                    }

                    break;

                case ProcessedDataType.MANUAL:
                    if (manualDataCellBackColor != Color.Empty)
                    {
                        e.Style.BackColor = CurrentTheme == Themes.BLACK
                            ? Color.FromArgb(45, manualDataCellBackColor)
                            : manualDataCellBackColor;
                    }

                    if (manualDataCellForeColor != Color.Empty)
                    {
                        e.Style.TextColor = manualDataCellForeColor;
                    }

                    break;

                case ProcessedDataType.NORMAL:
                    break;
            }
        }

        private void incompatibleDataGrid_QueryRowStyle(object sender, QueryRowStyleEventArgs e)
        {
            e.Style.HorizontalAlignment = HorizontalAlignment.Center;
        }

        private void incompatibleDataGrid_QueryProgressBarCellStyle(object sender, QueryProgressBarCellStyleEventArgs e)
        {
            if (!e.Column.HeaderText.Contains(" Score"))
            {
                return;
            }

            var dataObject = (dynamic)e.Record;
            AnswerKey ansKey = Functions.GetProperty(dataObject, "AnswerKey");
            if (ansKey == null)
            {
                return;
            }

            var maximum = ansKey.GetPaper.GetCorrectOptionValue * ansKey.GetKey.Length;
            e.Maximum = maximum == 0 ? 100 : maximum;

            e.Style.ProgressTextColor = CurrentTheme == Themes.BLACK || CurrentTheme == Themes.DARK_GRAY
                ? Color.White
                : e.Style.ProgressTextColor;
            e.Style.BackgroundColor = CurrentTheme == Themes.DARK_GRAY
                ? Color.FromArgb(210, 210, 210)
                : e.Style.BackgroundColor;
            //mainDataGrid.View.
        }

        private void DataImageBox_Paint(object sender, PaintEventArgs e)
        {
            if (curOptionRects.Count > 0)
            {
                for (var i = 0; i < curOptionRects.Count; i++)
                    Functions.DrawBox(e.Graphics, dataImageBox.GetOffsetRectangle(curOptionRects[i]),
                        dataImageBox.ZoomFactor, primaryRectColor, 1);
            }

            if (curMarkedOptionRects.Count > 0)
            {
                for (var i = 0; i < curMarkedOptionRects.Count; i++)
                    Functions.DrawBox(e.Graphics, dataImageBox.GetOffsetRectangle(curMarkedOptionRects[i]),
                        dataImageBox.ZoomFactor, secondaryRectColor, 1);
            }
        }

        private void DataBackImageBox_Paint(object sender, PaintEventArgs e)
        {
            if (curBackOptionRects.Count > 0)
            {
                for (var i = 0; i < curBackOptionRects.Count; i++)
                    Functions.DrawBox(e.Graphics, dataBackImageBox.GetOffsetRectangle(curBackOptionRects[i]),
                        dataBackImageBox.ZoomFactor, primaryRectColor, 1);
            }

            if (curMarkedBackOptionRects.Count > 0)
            {
                for (var i = 0; i < curMarkedBackOptionRects.Count; i++)
                    Functions.DrawBox(e.Graphics, dataBackImageBox.GetOffsetRectangle(curMarkedBackOptionRects[i]),
                        dataBackImageBox.ZoomFactor, secondaryRectColor, 1);
            }
        }

        #endregion

        #region Data Tab

        private void ExportExcelToolStripBtn_Click(object sender, EventArgs e)
        {
            this.ExportExcel();
        }

        private void ReadingTabStatusBar_SizeChanged(object sender, EventArgs e)
        {
            statusGeneralPanel.Width = this.Size.Width - 35;
            statusPanelStatusLabel.Width = statusTextStatusPanel.Width - 48;
        }

        #endregion

        #endregion

        #endregion

        private void locateOptionsToolStripBtn_Click(object sender, EventArgs e)
        {
            this.GetLocateRegionsToggle = !this.GetLocateRegionsToggle;
            locateOptionsToolStripBtn.CheckState =
                this.GetLocateRegionsToggle ? CheckState.Checked : CheckState.Unchecked;
        }

        private void stopReadingToolStripBtn_Click(object sender, EventArgs e)
        {
            this.MainProcessingManager.StopProcessing();

            startReadingToolStripBtn.Text = "Start";
            startReadingToolStripBtn.Image = Resources.startBtnIcon_ReadingTab;
        }

        private void manualDataGrid_CellDoubleClick(object sender, CellClickEventArgs e)
        {
            if (ModifierKeys == Keys.Control)
            {
                var cellRepresentation =
                    GridCellsRepresentation[e.DataColumn.GridColumn.MappingName];
                var configTitle = (e.DataRow.RowData as dynamic).DataRowObject
                    .GetProcessedDataEntries[cellRepresentation.entryIndex].ConfigurationTitle;
                var config = OnTemplateConfigs.Find(x => x.Configuration.Title == configTitle);

                if (config.Configuration.ValueEditType == ValueEditType.ReadOnly)
                {
                    return;
                }

                using (var dataEditForm =
                    new DataEditForm(config,
                        e.DataRow.RowData, e.DataColumn.GridColumn.MappingName, e.DataColumn.ColumnIndex))
                {
                    dataEditForm.ShowDialog();
                }
            }
        }

        private void showInExplorerContextBtn_Click(object sender, EventArgs e)
        {
            dynamic selectedRow = manualDataGrid.SelectedItems.Last();

            string path = selectedRow.DataRowObject.RowSheetPath;
            var cmd = "explorer.exe";
            var arg = $"/select,  {path}";

            Process.Start(cmd, arg);
        }

        private void normallyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<dynamic> selectedRows = manualDataGrid.SelectedItems.ToList();

            if (this.MainProcessingManager.IsProcessing && !this.MainProcessingManager.IsPaused)
            {
                this.MainProcessingManager.PauseProcessing();
            }

            this.MainProcessingManager.ReprocessData(selectedRows, ProcessingEnums.RereadType.NORMAL);

            if (this.MainProcessingManager.IsProcessing)
            {
                this.MainProcessingManager.ResumeProcessing();
            }
        }

        private List<dynamic> selectedDataRows;

        private void manualDataGrid_CellClick(object sender, CellClickEventArgs e)
        {
            if (e.MouseEventArgs.Button == MouseButtons.Right)
            {
                manualContext.Show(Cursor.Position);
            }
        }

        private void rotateXBy90ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<dynamic> selectedRows = manualDataGrid.SelectedItems.ToList();

            if (this.MainProcessingManager.IsProcessing && !this.MainProcessingManager.IsPaused)
            {
                this.MainProcessingManager.PauseProcessing();
            }

            this.MainProcessingManager.ReprocessData(selectedRows, ProcessingEnums.RereadType.ROTATE_C_90);

            if (this.MainProcessingManager.IsProcessing)
            {
                this.MainProcessingManager.ResumeProcessing();
            }
        }

        private void rotateX180ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<dynamic> selectedRows = manualDataGrid.SelectedItems.ToList();

            if (this.MainProcessingManager.IsProcessing && !this.MainProcessingManager.IsPaused)
            {
                this.MainProcessingManager.PauseProcessing();
            }

            this.MainProcessingManager.ReprocessData(selectedRows, ProcessingEnums.RereadType.ROTATE_AC_90);

            if (this.MainProcessingManager.IsProcessing)
            {
                this.MainProcessingManager.ResumeProcessing();
            }
        }

        private void rotateY90ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<dynamic> selectedRows = manualDataGrid.SelectedItems.ToList();

            if (this.MainProcessingManager.IsProcessing && !this.MainProcessingManager.IsPaused)
            {
                this.MainProcessingManager.PauseProcessing();
            }

            this.MainProcessingManager.ReprocessData(selectedRows, ProcessingEnums.RereadType.ROTATE_180);

            if (this.MainProcessingManager.IsProcessing)
            {
                this.MainProcessingManager.ResumeProcessing();
            }
        }

        private void answerKeyPaperComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            var selectedPaper = (Paper)answerKeyPaperComboBox.SelectedValue;
            this.SetupKeyFields(selectedKeyOMRConfig, selectedPaper);
        }

        private void mainDataGrid_CurrentCellEndEdit(object sender, CurrentCellEndEditEventArgs e)
        {
            var dataColumnName = e.DataColumn.GridColumn.MappingName;
            var dataRowObject = e.DataRow.RowData as dynamic;
            var cellRepresentation = GridCellsRepresentation[dataColumnName];
            ProcessedDataRow processedDataRow = dataRowObject.DataRowObject;
            ConfigurationBase configurationBase = null;
            try
            {
                configurationBase = processedDataRow.GetProcessedDataEntries[cellRepresentation.entryIndex]
                    .GetConfigurationBase;
            }
            catch (Exception ex)
            {
                return;
            }

            string editValue = Functions.GetProperty(dataRowObject, dataColumnName);
            if (editValue == "" || configurationBase == null ||
                configurationBase.ValueEditType == ValueEditType.ReadOnly)
            {
                return;
            }

            var err = string.Empty;
            var value = editValue;
            switch (configurationBase.ValueDataType)
            {
                case ValueDataType.String:
                    break;

                case ValueDataType.Text:
                    if (!value.All(char.IsLetter))
                    {
                        err = "Invalid value. Text was expected.";
                    }

                    break;

                case ValueDataType.Alphabet:
                    if (!value.All(char.IsLetter))
                    {
                        err = "Invalid value. Text was expected.";
                    }

                    break;

                case ValueDataType.WholeNumber:
                    if (!value.All(char.IsDigit))
                    {
                        err = "Invalid value. Whole number was expected.";
                    }

                    break;

                case ValueDataType.NaturalNumber:
                    if (!value.All(char.IsDigit) || value == "0")
                    {
                        err = "Invalid value, Natural number was expected.";
                    }

                    break;

                case ValueDataType.Integer:
                    if (!value.All(char.IsDigit))
                    {
                        err = "Invalid value. Integer was expected.";
                    }

                    break;
            }

            switch (configurationBase.GetMainConfigType)
            {
                case MainConfigType.OMR:

                    break;

                case MainConfigType.BARCODE:

                    break;

                case MainConfigType.ICR:

                    break;
            }

            var entry = processedDataRow.GetProcessedDataEntries[cellRepresentation.entryIndex];
            if (!string.IsNullOrEmpty(err))
            {
                Functions.AddProperty(dataRowObject, dataColumnName,
                    entry.GetDataValues[cellRepresentation.fieldIndex]);
                Messages.ShowError(err);
                return;
            }

            entry.IsEdited = true;
            entry.DataEntriesResultType[cellRepresentation.fieldIndex] = ProcessedDataType.NORMAL;
            Functions.AddProperty(dataRowObject, dataColumnName, editValue);
            entry.GetDataValues[cellRepresentation.fieldIndex] = editValue;

            processedDataRow.IsEdited = true;
            processedDataRow.GetProcessedDataEntries[cellRepresentation.entryIndex] = entry;
            Functions.AddProperty(dataRowObject, "DataRowObject", processedDataRow);
        }

        private void manualDataGrid_CurrentCellEndEdit(object sender, CurrentCellEndEditEventArgs e)
        {
            var dataColumnName = e.DataColumn.GridColumn.MappingName;
            var dataRowObject = e.DataRow.RowData as dynamic;
            var cellRepresentation = GridCellsRepresentation[dataColumnName];
            ProcessedDataRow processedDataRow = dataRowObject.DataRowObject;
            ConfigurationBase configurationBase = null;
            try
            {
                configurationBase = processedDataRow.GetProcessedDataEntries[cellRepresentation.entryIndex]
                    .GetConfigurationBase;
            }
            catch (Exception ex)
            {
                return;
            }

            string editValue = Functions.GetProperty(dataRowObject, dataColumnName);
            if (editValue == "" || configurationBase == null ||
                configurationBase.ValueEditType == ValueEditType.ReadOnly)
            {
                return;
            }

            var err = string.Empty;
            var value = editValue;
            switch (configurationBase.ValueDataType)
            {
                case ValueDataType.String:
                    break;

                case ValueDataType.Text:
                    if (!value.All(char.IsLetter))
                    {
                        err = "Invalid value. Text was expected.";
                    }

                    break;

                case ValueDataType.Alphabet:
                    if (!value.All(char.IsLetter))
                    {
                        err = "Invalid value. Text was expected.";
                    }

                    break;

                case ValueDataType.WholeNumber:
                    if (!value.All(char.IsDigit))
                    {
                        err = "Invalid value. Whole number was expected.";
                    }

                    break;

                case ValueDataType.NaturalNumber:
                    if (!value.All(char.IsDigit) || value == "0")
                    {
                        err = "Invalid value, Natural number was expected.";
                    }

                    break;

                case ValueDataType.Integer:
                    if (!value.All(char.IsDigit))
                    {
                        err = "Invalid value. Integer was expected.";
                    }

                    break;
            }

            var processedDataEntry =
                processedDataRow.GetProcessedDataEntries[cellRepresentation.entryIndex];

            if (!string.IsNullOrEmpty(err))
            {
                Functions.AddProperty(dataRowObject, dataColumnName,
                    processedDataEntry.GetDataValues[cellRepresentation.fieldIndex]);
                Messages.ShowError(err);
                return;
            }

            var fieldIndex = cellRepresentation.fieldIndex == 0 ? 0 : cellRepresentation.fieldIndex - 1;
            processedDataEntry.IsEdited = true;
            processedDataEntry.DataEntriesResultType[fieldIndex] = ProcessedDataType.NORMAL;
            Functions.AddProperty(dataRowObject, dataColumnName, editValue);
            processedDataEntry.GetDataValues[fieldIndex] = editValue;
            processedDataEntry.GetFieldsOutputs[fieldIndex] = editValue.ToCharArray()[0];

            switch (configurationBase.GetMainConfigType)
            {
                case MainConfigType.OMR:
                    var omrConfig = (OMRConfiguration)configurationBase;

                    switch (omrConfig.OMRType)
                    {
                        case OMRType.Gradable:
                            switch (omrConfig.KeyType)
                            {
                                case KeyType.General:
                                    for (var k = 0; k < omrConfig.GeneralAnswerKeys.Count; k++)
                                        try
                                        {
                                            var generalKey = omrConfig.GeneralAnswerKeys[k];
                                            if (generalKey.IsActive == false)
                                            {
                                                continue;
                                            }

                                            if (processedDataEntry.GetFieldsOutputs.Length >
                                                generalKey.GetPaper.GetFieldsCount)
                                            {
                                                var startIndex = generalKey.GetPaper.GetFieldsCount;
                                                var curTotal = processedDataEntry.GetFieldsOutputs.Length;
                                                //for (int j = startIndex; j < curTotal; j++)
                                                //{
                                                //    //processedDataEntry.GetFieldsOutputs[j] = '—';
                                                //    processedDataEntry.DataEntriesResultType[j] = ProcessedDataType.NORMAL;
                                                //    processedDataEntry.SpecialCells.Add(new ProcessedDataEntry.SpecialCell((i1, j + 1), Color.FromArgb(220, 238, 245), Color.Black));
                                                //    //** IMPOSSIBLE??
                                                //    //string dataTitle = dataColumns != null && dataColumns.Count > 0 ? dataColumns[(lastDataColumnsIndex + 1) + j] : allConfigurations[i1].Title[0] + (j + 1).ToString();
                                                //    //Functions.AddProperty(dynamicDataRow, dataTitle, "—");
                                                //}
                                            }

                                            var rawValues = ProcessedDataEntry.GenerateRawOMRDataValues(omrConfig,
                                                processedDataEntry.GetFieldsOutputs, omrConfig.GetEscapeSymbols());
                                            var gradeResult = OMREngine.GradeSheet(generalKey, rawValues,
                                                omrConfig.MultiMarkAction);
                                            Functions.AddProperty(dataRowObject, "AnswerKey", generalKey);

                                            for (var i2 = 0; i2 < 2; i2++)
                                            {
                                                var dataTitle = i2 == 0
                                                    ? omrConfig.Title + " Score " + omrConfig.GeneralAnswerKeys[k].Title
                                                    : i2 == 1
                                                        ? omrConfig.Title + " Paper " +
                                                          omrConfig.GeneralAnswerKeys[k].Title
                                                        : omrConfig.Title + $" x{i2}";
                                                Functions.AddProperty(dataRowObject, dataTitle,
                                                    i2 == 0 ? gradeResult.obtainedMarks + "" :
                                                    i2 == 1 ? generalKey.GetPaper.Title : generalKey.Title);

                                                //lastDataColumnsIndexEx++;
                                                //extraColumns++;
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            for (var i2 = 0; i2 < 2; i2++)
                                            {
                                                var dataTitle = i2 == 0
                                                    ? omrConfig.Title + " Score " + omrConfig.GeneralAnswerKeys[k].Title
                                                    : i2 == 1
                                                        ? omrConfig.Title + " Paper " +
                                                          omrConfig.GeneralAnswerKeys[k].Title
                                                        : omrConfig.Title + $" x{i2}";
                                                Functions.AddProperty(dataRowObject, dataTitle, "—");

                                                //lastDataColumnsIndexEx++;
                                                //extraColumns++;
                                            }
                                        }

                                    break;

                                case KeyType.ParameterBased:
                                    break;
                            }

                            break;

                        case OMRType.Parameter:
                            var gradingKeys = ConfigurationsManager
                                .GetConfigurations(MainConfigType.OMR, null)
                                .Cast<OMRConfiguration>()
                                .SelectMany(config => config.PB_AnswerKeys.ToList())
                                .Where(keyValue => keyValue.Key.parameterConfig.Title == omrConfig.Title
                                                   && keyValue.Key.parameterValue == value)
                                .ToList();

                            gradingKeys.ForEach(gradingKey =>
                            {
                                var answerKey = omrConfig.PB_AnswerKeys[gradingKey.Key];
                                var answerKeyConfig =
                                    (OMRConfiguration)ConfigurationsManager.GetConfiguration(answerKey.GetConfigName);
                                var answersProcessedDataEntry =
                                    processedDataRow.GetProcessedDataEntries.Single(x =>
                                        x.ConfigurationTitle == answerKeyConfig.Title);
                                var rawValues = ProcessedDataEntry.GenerateRawOMRDataValues(answerKeyConfig,
                                    answersProcessedDataEntry.GetFieldsOutputs, answerKeyConfig.GetEscapeSymbols());
                                var gradingResult =
                                    OMREngine.GradeSheet(answerKey, rawValues, answerKeyConfig.MultiMarkAction);

                                Functions.AddProperty(dataRowObject, "AnswerKey", answerKey);

                                for (var k = 0; k < answerKeyConfig.PB_AnswerKeys.Count; k++)
                                for (var i2 = 0; i2 < 2; i2++)
                                {
                                    var dataTitle = i2 == 0
                                        ? (answerKeyConfig.ParentTitle ?? answerKeyConfig.Title) + " Score " +
                                          answerKeyConfig.PB_AnswerKeys.Values.ToArray()[k].Title
                                        : i2 == 1
                                            ? (answerKeyConfig.ParentTitle ?? answerKeyConfig.Title) + " Paper " +
                                              answerKeyConfig.PB_AnswerKeys.Values.ToArray()[k].Title
                                            : (answerKeyConfig.ParentTitle ?? answerKeyConfig.Title) + $" x{i2}";
                                    Functions.AddProperty(dataRowObject, dataTitle,
                                        i2 == 0 ? gradingResult.obtainedMarks + "" :
                                        i2 == 1 ? answerKey.GetPaper.Code : answerKey.Title);
                                }
                            });
                            break;
                    }

                    break;

                case MainConfigType.BARCODE:

                    break;

                case MainConfigType.ICR:

                    break;
            }

            processedDataRow.IsEdited = true;
            processedDataRow.GetProcessedDataEntries[cellRepresentation.entryIndex] = processedDataEntry;
            Functions.AddProperty(dataRowObject, "DataRowObject", processedDataRow);
        }

        private void externalSaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mainDataGrid.DataSource == null || this.MainProcessingManager.GetTotalProcessedData == 0)
            {
                Messages.ShowError(
                    "No processed data found to create a data point.\n\nConsider processing some data before creating a data point.");
                return;
            }

            if (this.MainProcessingManager.IsProcessing)
            {
                MessageBoxAdv.Show(
                    "This operation cannot be performed while processing data.\n\nStop processing in order to create a data point.",
                    "Hold On", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var mainData = (ObservableCollection<dynamic>)mainDataGridPager.DataSource;
            var mainSerializeData =
                new ObservableCollection<Dictionary<string, object>>();
            for (var i = 0; i < mainData.Count; i++)
                mainSerializeData.Add(new Dictionary<string, object>((IDictionary<string, object>)mainData[i]));

            var manData = (ObservableCollection<dynamic>)manualDataGridPager.DataSource;
            var manSerializeData =
                new ObservableCollection<Dictionary<string, object>>();
            for (var i = 0; i < manData.Count; i++)
                manSerializeData.Add(new Dictionary<string, object>((IDictionary<string, object>)manData[i]));

            var fauData = (ObservableCollection<dynamic>)faultyDataGridPager.DataSource;
            var faultySerializeData =
                new ObservableCollection<Dictionary<string, object>>();
            for (var i = 0; i < fauData.Count; i++)
                faultySerializeData.Add(new Dictionary<string, object>((IDictionary<string, object>)fauData[i]));

            var incData = (ObservableCollection<dynamic>)incompatibleDataGridPager.DataSource;
            var incSerializeData =
                new ObservableCollection<Dictionary<string, object>>();
            for (var i = 0; i < incData.Count; i++)
                incSerializeData.Add(new Dictionary<string, object>((IDictionary<string, object>)incData[i]));

            var externalDataPoint =
                new DataPoint(mainSerializeData, manSerializeData, faultySerializeData, incSerializeData);

            saveFileDialog.FileName = $"DP_{DateTime.Now.ToShortDateString()}";
            saveFileDialog.Filter = DATAPOINT_FILTER;
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                var success = DataPoint.Save(externalDataPoint, saveFileDialog.FileName, out var ex);

                if (!success)
                {
                    Messages.ShowError($"Failed to save External Data Point\n\nError: {ex.Message}");
                }
                else
                {
                    MessageBoxAdv.Show(
                        $"External data point: {Path.GetFileNameWithoutExtension(saveFileDialog.FileName)} saved successfully.");
                }
            }
        }

        private void externalLoadToolStripMenuItem_Click(object sender, EventArgs e)
        {
#pragma warning disable CA1303 // Do not pass literals as localized parameters
            openFileDialog.Filter = DATAPOINT_FILTER;
#pragma warning restore CA1303 // Do not pass literals as localized parameters
            try
            {
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    var dpPath = openFileDialog.FileName;
                    var loadedExternalDataPoint = DataPoint.Load(dpPath, out var ex);
                    if (loadedExternalDataPoint == null)
                    {
                        Messages.ShowError($"An error occured while loading the data point\n\nError: {ex.Message}");
                    }
                    else
                    {
                        var dataObj =
                            (dynamic)loadedExternalDataPoint.ProcessedData
                                .processedDataSource[0].ToExpando();
                        var data =
                            loadedExternalDataPoint.ReturnProcessedData();

                        //InitializeDataGrids(dataObj.DataRowObject.GetProcessedDataEntries[0], (data.processedDataSource, data.manProcessedDataSource, data.fauProcessedDataSource, data.incProcessedDataSource), 0);
                        this.MainProcessingManager.SetData(data.processedDataSource, data.manProcessedDataSource,
                            data.fauProcessedDataSource, data.incProcessedDataSource);
                    }
                }
            }
            catch (Exception ex)
            {
                Messages.ShowError($"Failed to load the data point\n\nError: {ex.Message}");
            }
        }

        private void SQLDatabaseExportToolStripBtn_Click(object sender, EventArgs e)
        {
            if (this.MainProcessingManager.IsProcessing || this.MainProcessingManager.IsPaused)
            {
                Messages.ShowError("Cannot use this feature during an active processing session");
            }

            try
            {
                var allData = (ObservableCollection<dynamic>)mainDataGridPager.DataSource;
                if (allData == null || allData.Count == 0)
                {
                    allData = this.MainProcessingManager.GetAllProcessedData();
                }

                if (allData == null || allData.Count == 0)
                {
                    allData = this.MainProcessingManager.GetAllAltProcessedData();
                }

                if (allData != null && allData.Count > 0)
                {
                    var allFields = new string[mainDataGrid.Columns.Count];
                    for (var i = 0; i < mainDataGrid.Columns.Count; i++)
                    {
                        var columnName = mainDataGrid.Columns[i].HeaderText;
                        allFields[i] = columnName;
                    }

                    var databaseWizardForm = new DatabaseWizardForm(allFields, allData);
                    databaseWizardForm.ShowDialog();
                }
                else
                {
                    Messages.ShowError("No data available to use");
                }
            }
            catch (Exception ex)
            {
                Messages.ShowError("Unable to use this feature");
            }
        }

        private void propertiesConfigSelector_SelectedValueChanged(object sender, EventArgs e)
        {
            var config = OnTemplateConfigs.SingleOrDefault(x =>
                x.Configuration.Title == propertiesConfigSelector.SelectedItem?.ToString());

            if (config == null)
            {
                return;
            }

            this.SelectedTemplateConfigChanged(config);
        }
    }
}