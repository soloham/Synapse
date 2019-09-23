using Cyotek.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;
using Synapse.Core.Configurations;
using Synapse.Core.Engines.Data;
using Synapse.Core.Templates;
using Synapse.Utilities;
using Synapse.Utilities.Enums;
using Synapse.Utilities.Objects;
using Syncfusion.WinForms.DataGrid;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Synapse.Core.Templates.Template;

namespace Synapse.Core.Managers
{
    internal class ProcessingManager
    {
        public int GetCurProcessingIndex { get; private set; }

        private Template CurrentTemplate;
        private SheetsList loadedSheetsData = new SheetsList();
        private List<string> dataColumns = new List<string>();
        private List<ProcessedDataRow> processedData = new List<ProcessedDataRow>();

        private ObservableCollection<dynamic> allProcessedDataSource = new ObservableCollection<dynamic>();
        private ObservableCollection<dynamic> manualProcessedDataSource = new ObservableCollection<dynamic>();
        private ObservableCollection<dynamic> faultyProcessedDataSource = new ObservableCollection<dynamic>();
        private ObservableCollection<dynamic> incompatibleProcessedDataSource = new ObservableCollection<dynamic>();

        public int GetTotalProcessedData { get => allProcessedDataSource.Count; }
        public int GetTotalManualProcessedData { get => manualProcessedDataSource.Count; }
        public int GetTotalFaultyProcessedData { get => faultyProcessedDataSource.Count; }
        public int GetTotalIncompatibleProcessedData { get => incompatibleProcessedDataSource.Count; }

        public event EventHandler<ProcessedDataType> OnDataSourceUpdated;

        public ProcessingManager(Template currentTemplate)
        {
            CurrentTemplate = currentTemplate;
        }

        internal void LoadSheets(SheetsList sheetsList)
        {
            this.loadedSheetsData = sheetsList;
        }
        internal void ClearData(ProcessedDataType processedDataType)
        {
            switch (processedDataType)
            {
                case ProcessedDataType.INCOMPATIBLE:
                    incompatibleProcessedDataSource.Clear();
                    break;
                case ProcessedDataType.FAULTY:
                    faultyProcessedDataSource.Clear();
                    break;
                case ProcessedDataType.MANUAL:
                    manualProcessedDataSource.Clear();
                    break;
                case ProcessedDataType.NORMAL:
                    allProcessedDataSource.Clear();
                    break;
            }

            processedData.RemoveAll(x => x.DataRowResultType == processedDataType);

            OnDataSourceUpdated?.Invoke(this, processedDataType);
        }
        internal void ClearAllData()
        {
            incompatibleProcessedDataSource.Clear();
            faultyProcessedDataSource.Clear();
            manualProcessedDataSource.Clear();
            allProcessedDataSource.Clear();

            processedData.Clear();

            var processedDataTypes = EnumHelper.ToList(typeof(ProcessedDataType));
            for (int i = 0; i < processedDataTypes.Count; i++)
            {
                OnDataSourceUpdated?.Invoke(this, (ProcessedDataType)i);
            }
        }

        internal void UpdateDataColumns(List<string> dataColumns)
        {
            this.dataColumns = dataColumns;
        }
        internal async Task StartProcessing(bool keepData, Action<ProcessedDataRow, double, double> OnSheetProcessed, List<string> dataColumns = null)
        {
            this.dataColumns = dataColumns;

            if (!keepData)
                ClearAllData();

            string[] sheetsPaths = loadedSheetsData.GetSheetsPath;
            var allConfigurations = ConfigurationsManager.GetAllConfigurations;

            double runningAverage = 0;
            double runningTotal = 0;
            for (int i = 0; i < sheetsPaths.Length; i++)
            {
                var t0 = DateTime.Now;
                GetCurProcessingIndex = i;

                dynamic dynamicDataRow = new ExpandoObject();
                Mat curSheet = new Mat(sheetsPaths[i]);
                Image<Gray, byte> curSheetImage = curSheet.ToImage<Gray, byte>();
                AlignmentPipelineResults alignmentPipelineResults = null;
                Image<Gray, byte> alignedSheet = null;
                await Task.Run(() => { alignedSheet = CurrentTemplate.AlignSheet(curSheetImage, out AlignmentPipelineResults _alignmentPipelineResults); alignmentPipelineResults = _alignmentPipelineResults; });
                curSheetImage.Dispose();
                curSheet.Dispose();
                if (alignmentPipelineResults.AlignmentMethodTestResultsList.TrueForAll(x => x.GetAlignmentMethodResultType == AlignmentPipelineResults.AlignmentMethodResultType.Failed))
                    continue;

                ProcessedDataType processedRowType = ProcessedDataType.NORMAL;
                List<ProcessedDataEntry> processedDataEntries = new List<ProcessedDataEntry>();
                int lastDataColumnsIndex = 0;
                ConfigurationBase curConfigurationBase = null;
                for (int i1 = 0; i1 < allConfigurations.Count; i1++)
                {
                    curConfigurationBase = allConfigurations[i1];
                    var processedDataEntry = curConfigurationBase.ProcessSheet(alignedSheet.Mat);
                    processedDataEntries.Add(processedDataEntry);

                    string[] formattedOutput = processedDataEntry.FormatData();
                    if (formattedOutput.Length == 1)
                    {
                        string dataTitle = dataColumns != null && dataColumns.Count > 0 ? dataColumns[lastDataColumnsIndex+1] : allConfigurations[i1].Title;
                        Functions.AddProperty(dynamicDataRow, dataTitle, formattedOutput[0]);

                        lastDataColumnsIndex++;
                    }
                    else
                    {
                        for (int i2 = 0; i2 < formattedOutput.Length; i2++)
                        {
                            string dataTitle = dataColumns != null && dataColumns.Count > 0 ? dataColumns[lastDataColumnsIndex + i2] : allConfigurations[i1].Title[0] + (i2 + 1).ToString();
                            Functions.AddProperty(dynamicDataRow, dataTitle, formattedOutput[i2]);

                        }
                        lastDataColumnsIndex += formattedOutput.Length-1;
                    }

                    switch (curConfigurationBase.GetMainConfigType)
                    {
                        case MainConfigType.OMR:
                            OMRConfiguration omrConfig = (OMRConfiguration)curConfigurationBase;
                            switch (omrConfig.OMRType)
                            {
                                case OMRType.Gradable:
                                    switch (omrConfig.KeyType)
                                    {
                                        case Keys.KeyType.General:
                                            break;
                                        case Keys.KeyType.ParameterBased:
                                            break;
                                        default:
                                            break;
                                    }
                                    break;
                                case OMRType.NonGradable:
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

                ProcessedDataRow processedDataRow = new ProcessedDataRow(processedDataEntries, i, sheetsPaths[i], processedRowType);
                processedData.Add(processedDataRow);

                Functions.AddProperty(dynamicDataRow, "DataRowObject", processedDataRow);
                switch(processedRowType)
                {
                    case ProcessedDataType.INCOMPATIBLE:
                        incompatibleProcessedDataSource.Add(dynamicDataRow);
                    break;
                    case ProcessedDataType.FAULTY:
                        faultyProcessedDataSource.Add(dynamicDataRow);
                    break;
                    case ProcessedDataType.MANUAL:
                        manualProcessedDataSource.Add(dynamicDataRow);
                    break;
                    case ProcessedDataType.NORMAL:
                        allProcessedDataSource.Add(dynamicDataRow);
                    break;
                }
                if (i == 0 && !keepData)
                    SynapseMain.GetSynapseMain.InitializeMainDataGrid(processedDataEntries, allProcessedDataSource);

                OnDataSourceUpdated?.Invoke(this, processedRowType);

                var t1 = DateTime.Now;
                runningTotal += (t1 - t0).TotalMilliseconds;
                runningAverage = runningTotal / (i+1);
                OnSheetProcessed(processedDataRow, runningAverage, runningTotal);
            }
        }
        internal async Task StartProcessingRaw(Func<bool> GetAlignSheet, Action<Bitmap> OnSheetAligned, Action<RectangleF, bool> OnOptionProcessed, Action<string> OnRegionProcessed, Func<double> GetWaitMS, Action OnProcessFinished)
        {
            string[] sheetsPaths = loadedSheetsData.GetSheetsPath;
            var allConfigurations = ConfigurationsManager.GetAllConfigurations;

            for (int i = 0; i < sheetsPaths.Length; i++)
            {
                GetCurProcessingIndex = i;

                Mat curSheet = new Mat(sheetsPaths[i]);
                AlignmentPipelineResults alignmentPipelineResults = null;
                Image<Gray, byte> alignedSheet = null;
                if (GetAlignSheet())
                {
                    await Task.Run(() => { alignedSheet = CurrentTemplate.AlignSheet(curSheet.ToImage<Gray, byte>(), out AlignmentPipelineResults _alignmentPipelineResults); alignmentPipelineResults = _alignmentPipelineResults; });
                    if (alignmentPipelineResults.AlignmentMethodTestResultsList.TrueForAll(x => x.GetAlignmentMethodResultType == AlignmentPipelineResults.AlignmentMethodResultType.Failed))
                        continue;
                }
                else
                    alignedSheet = curSheet.ToImage<Gray, byte>();

                await Task.Run(() => OnSheetAligned(alignedSheet.Bitmap));

                List<ProcessedDataEntry> processedDataEntries = new List<ProcessedDataEntry>();
                for (int i1 = 0; i1 < allConfigurations.Count; i1++)
                {
                    ProcessedDataEntry processedDataEntry = new ProcessedDataEntry();
                    if (allConfigurations[i1].GetMainConfigType == MainConfigType.OMR)
                    {
                        OMRConfiguration omrConfiguration = (OMRConfiguration)allConfigurations[i1];
                        processedDataEntry = await omrConfiguration.ProcessSheetRaw(alignedSheet.Mat, OnOptionProcessed, GetWaitMS);
                    }
                    else
                        processedDataEntry = allConfigurations[i1].ProcessSheet(alignedSheet.Mat);

                    processedDataEntries.Add(processedDataEntry);

                    string[] formattedOutput = processedDataEntry.FormatData();
                    if (formattedOutput.Length == 1)
                    {
                        await Task.Run(() => OnRegionProcessed(formattedOutput[0]));
                    }
                    else
                    {
                        string _formattedOutput = "";
                        for (int i2 = 0; i2 < formattedOutput.Length; i2++)
                        {
                            _formattedOutput += formattedOutput[i2];
                            if (i2 < formattedOutput.Length - 1)
                                _formattedOutput += ", ";
                            //Functions.AddProperty(dynamicDataRow, allConfigurations[i1].Title[0] + (i2 + 1).ToString(), formattedOutput[i2]);
                        }
                        await Task.Run(() => OnRegionProcessed(_formattedOutput));
                    }
                }

                ProcessedDataRow processedDataRow = new ProcessedDataRow(processedDataEntries, i, sheetsPaths[i], ProcessedDataType.NORMAL);
                processedData.Add(processedDataRow);
            }

            OnProcessFinished();
        }

        internal bool DataExists()
        {
            bool result = false;

            result = processedData.Count > 0;

            return result;
        }
        
    }
}
