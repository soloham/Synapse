using static Synapse.Core.Templates.Template;
using static Synapse.Shared.Enums.ProcessingEnums;

namespace Synapse.Core.Managers
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Drawing;
    using System.Dynamic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using Emgu.CV;
    using Emgu.CV.CvEnum;

    using Synapse.Core.Configurations;
    using Synapse.Core.Engines;
    using Synapse.Core.Engines.Data;
    using Synapse.Core.Keys;
    using Synapse.Core.Templates;
    using Synapse.Utilities;
    using Synapse.Utilities.Enums;
    using Synapse.Utilities.Objects;

    public class ProcessingManager
    {
        #region Enums

        #endregion

        public BackgroundWorker ProcessingWorker;

        public int GetCurProcessingIndex { get; private set; }

        private readonly bool IsVerified;
        private readonly Template CurrentTemplate;
        private SheetsList loadedSheetsData = new SheetsList();
        private List<string> dataColumns = new List<string>();
        private readonly List<ProcessedDataRow> processedData = new List<ProcessedDataRow>();

        private readonly Action<List<ProcessedDataEntry>, (ObservableCollection<dynamic> processedDataSource,
            ObservableCollection<dynamic> manProcessedDataSource, ObservableCollection<dynamic> fauProcessedDataSource,
            ObservableCollection<dynamic> incProcessedDataSource), int, bool> InitializeDataGrids;

        private readonly Func<bool, List<string>> GetGridDataColumns;

        private ObservableCollection<dynamic> allProcessedDataSource = new ObservableCollection<dynamic>();
        private readonly ObservableCollection<dynamic> altAllProcessedDataSource = new ObservableCollection<dynamic>();
        private ObservableCollection<dynamic> manualProcessedDataSource = new ObservableCollection<dynamic>();
        private ObservableCollection<dynamic> faultyProcessedDataSource = new ObservableCollection<dynamic>();
        private ObservableCollection<dynamic> incompatibleProcessedDataSource = new ObservableCollection<dynamic>();

        public int GetTotalProcessedData => allProcessedDataSource.Count;
        public int GetTotalManualProcessedData => manualProcessedDataSource.Count;
        public int GetTotalFaultyProcessedData => faultyProcessedDataSource.Count;
        public int GetTotalIncompatibleProcessedData => incompatibleProcessedDataSource.Count;
        public bool IsProcessing { get; private set; }
        public bool IsPaused { get; private set; }
        public bool VisualizeData { get; set; } = true;

        public const int MAX_UNACTIVATED_SHEETS = 500;

        public event EventHandler<ProcessedDataType> OnDataSourceUpdated;
        public event EventHandler<(ProcessedDataRow, double, double)> OnSheetProcessed;
        public event EventHandler<(bool cancelled, object result, Exception error)> OnProcessingComplete;

        public OMREngine CurOMREngine;
        public BarcodeEngine CurBarcodeEngine;

        public ProcessingManager(bool isVerified, Template currentTemplate,
            Action<List<ProcessedDataEntry>, (ObservableCollection<dynamic> processedDataSource,
                    ObservableCollection<dynamic> manProcessedDataSource, ObservableCollection<dynamic>
                    fauProcessedDataSource, ObservableCollection<dynamic> incProcessedDataSource), int, bool>
                initializeDataGridsAction, Func<bool, List<string>> getGridDataColumnsFunc)
        {
            IsVerified = isVerified;
            CurrentTemplate = currentTemplate;
            InitializeDataGrids = initializeDataGridsAction;
            GetGridDataColumns = getGridDataColumnsFunc;
            this.IsProcessing = false;

            CurOMREngine = new OMREngine();
            CurBarcodeEngine = new BarcodeEngine();
        }

        public void LoadSheets(SheetsList sheetsList)
        {
            loadedSheetsData = sheetsList;
        }

        public void ClearData(ProcessedDataType processedDataType)
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
                    altAllProcessedDataSource.Clear();
                    break;
            }

            processedData.RemoveAll(x => x.DataRowResultType == processedDataType);

            this.OnDataSourceUpdated?.Invoke(this, processedDataType);
        }

        public void ClearAllData()
        {
            try
            {
                incompatibleProcessedDataSource.Clear();
                faultyProcessedDataSource.Clear();
                manualProcessedDataSource.Clear();
                allProcessedDataSource.Clear();
                altAllProcessedDataSource.Clear();

                processedData.Clear();

                var processedDataTypes = EnumHelper.ToList(typeof(ProcessedDataType));
                for (var i = 0; i < processedDataTypes.Count; i++)
                    this.OnDataSourceUpdated?.Invoke(this, (ProcessedDataType)i);
            }
            catch (Exception ex)
            {
            }
        }

        public void UpdateDataColumns(List<string> dataColumns)
        {
            this.dataColumns = dataColumns;
        }

        public void StartProcessing(bool keepData, List<string> dataColumns = null)
        {
            this.dataColumns = dataColumns;

            if (!keepData)
            {
                this.ClearAllData();
            }

            ProcessingWorker = new BackgroundWorker();
            ProcessingWorker.WorkerReportsProgress = true;
            ProcessingWorker.WorkerSupportsCancellation = true;

            ProcessingWorker.DoWork += this.ProcessingWorker_DoWork;
            ProcessingWorker.ProgressChanged += this.ProcessingWorker_ProgressChanged;
            ProcessingWorker.RunWorkerCompleted += this.ProcessingWorker_RunWorkerCompleted;

            ProcessingWorker.RunWorkerAsync(keepData);
        }

        public void StopProcessing()
        {
            ProcessingWorker.CancelAsync();

            this.IsPaused = false;
        }

        public void PauseProcessing()
        {
            this.IsPaused = true;
        }

        public void ResumeProcessing()
        {
            this.IsPaused = false;
        }


        public DateTime uiClock0 = DateTime.Now;

        private void ProcessingWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            var (keepData, processedDataRow, dynamicDataRow, runningAverage, runningTotal, extraColumns) =
                ((bool, ProcessedDataRow, dynamic, double, double, int))e.UserState;

            if (processedDataRow.GetRowIndex == 0 && !keepData)
            {
                InitializeDataGrids?.Invoke(processedDataRow.GetProcessedDataEntries,
                    (allProcessedDataSource, manualProcessedDataSource, faultyProcessedDataSource,
                        incompatibleProcessedDataSource), extraColumns, false);
            }

            //if(!VisualizeData)
            //    InitializeDataGrids?.Invoke(processedDataRow.GetProcessedDataEntries, (null, null, null, null), extraColumns, false);

            try
            {
                //if (allLatencyDataObjects.Count > 0)
                //{
                //    allLatencyDataObjects.ForEach(x => allProcessedDataSource.Add(x));
                //    allLatencyDataObjects.Clear();
                //}
                if (this.VisualizeData)
                {
                    allProcessedDataSource.Add(dynamicDataRow);
                }
                else
                {
                    altAllProcessedDataSource.Add(dynamicDataRow);
                }

                switch (processedDataRow.DataRowResultType)
                {
                    case ProcessedDataType.INCOMPATIBLE:
                        //if (incompatibleLatencyDataObjects.Count > 0)
                        //{
                        //    incompatibleLatencyDataObjects.ForEach(x => incompatibleProcessedDataSource.Add(x));
                        //    incompatibleLatencyDataObjects.Clear();
                        //}
                        if (this.VisualizeData)
                        {
                            incompatibleProcessedDataSource.Add(dynamicDataRow);
                        }

                        break;

                    case ProcessedDataType.FAULTY:
                        //if (faultyLatencyDataObjects.Count > 0)
                        //{
                        //    faultyLatencyDataObjects.ForEach(x => faultyProcessedDataSource.Add(x));
                        //    faultyLatencyDataObjects.Clear();
                        //}
                        if (this.VisualizeData)
                        {
                            faultyProcessedDataSource.Add(dynamicDataRow);
                        }

                        break;

                    case ProcessedDataType.MANUAL:
                        //if (manualLatencyDataObjects.Count > 0)
                        //{
                        //    manualLatencyDataObjects.ForEach(x => manualProcessedDataSource.Add(x));
                        //    manualLatencyDataObjects.Clear();
                        //}
                        var manualCount = manualProcessedDataSource.Count;
                        if (this.VisualizeData)
                        {
                            manualProcessedDataSource.Insert(manualCount == 0 ? 0 : manualCount - 1, dynamicDataRow);
                        }

                        break;

                    case ProcessedDataType.NORMAL:
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ProgressChanged Ex: {ex.Message}");
            }

            this.OnDataSourceUpdated?.Invoke(this, processedDataRow.DataRowResultType);
            this.OnSheetProcessed?.Invoke(this, (processedDataRow, runningAverage, runningTotal));

            //DateTime uiClock1 = DateTime.Now;
            //var diff = (uiClock1 - uiClock0).TotalMilliseconds;
            //if (diff >= 50)
            //{


            //    uiClock0 = DateTime.Now;
            //}
        }

        private void ProcessingWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.IsProcessing = false;
            this.OnProcessingComplete?.Invoke(this, (e.Cancelled, e.Cancelled ? null : e.Result, e.Error));
        }

        private void ProcessingWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            var worker = (BackgroundWorker)sender;
            var sheetsPaths = loadedSheetsData.GetSheetsPath.OrderBy(x => x).ToArray();
            var allOrderedConfigurations = ConfigurationsManager.GetOrderedConfigurations();
            var configsBySheetSide = allOrderedConfigurations.GroupBy(x => x.SheetSide)
                .ToDictionary(x => x.Key, x => x.ToList());

            var frontSheetConfigs = configsBySheetSide[SheetSideType.Front].Where(x => string.IsNullOrEmpty(x.ParentTitle)).ToList();

            double runningAverage = 0;
            double runningTotal = 0;

            var extraColumns = 2;

            var keepData = (bool)e.Argument;

            this.IsProcessing = true;
            var pauseGrading = false;
            var isActivated = IsVerified;

            var renameFields = frontSheetConfigs.FindAll(x => x.AddToFileName);
            var backSheetsCount = 0;
            var iIncrementCount = 1;

            for (var i = 0; i < sheetsPaths.Length; i += iIncrementCount)
            {
                var rowIndex = i - backSheetsCount;
                iIncrementCount = 1;
                if (worker.CancellationPending || !isActivated && i >= MAX_UNACTIVATED_SHEETS)
                {
                    e.Cancel = true;
                    return;
                }

                var t0 = DateTime.Now;
                this.GetCurProcessingIndex = i;

                dynamic dynamicDataRow = new ExpandoObject();
                var processedRowType = ProcessedDataType.NORMAL;

                Mat curSheet = null;
                AlignmentPipelineResults alignmentPipelineResults = null;
                Mat alignedSheet = null;
                bool isTestSuccessful;

                try
                {
                    curSheet = CvInvoke.Imread(sheetsPaths[i], ImreadModes.Grayscale);
                    //await Task.Run(() => { alignedSheet = CurrentTemplate.AlignSheet(curSheet, out AlignmentPipelineResults _alignmentPipelineResults); alignmentPipelineResults = _alignmentPipelineResults; });
                    alignedSheet = CurrentTemplate.AlignSheet(curSheet, out var _alignmentPipelineResults,
                        out isTestSuccessful, false);
                    alignmentPipelineResults = _alignmentPipelineResults;
                    curSheet.Dispose();
                    if (alignmentPipelineResults.AlignmentMethodTestResultsList.TrueForAll(x =>
                        x.GetAlignmentMethodResultType == AlignmentPipelineResults.AlignmentMethodResultType.Failed))
                    {
                        continue;
                    }
                }
                catch (Exception ex)
                {
                    processedRowType = ProcessedDataType.INCOMPATIBLE;

                    var gridDataColumns = GetGridDataColumns?.Invoke(false);
                    for (var i1 = 0; i1 < gridDataColumns.Count; i1++)
                    {
                        var dataTitle = gridDataColumns[i1];
                        Functions.AddProperty(dynamicDataRow, dataTitle, "—");
                    }

                    var processedDataEntriesEx = new List<ProcessedDataEntry>();
                    for (var i1 = 0; i1 < frontSheetConfigs.Count; i1++)
                        processedDataEntriesEx.Add(new ProcessedDataEntry(frontSheetConfigs[i1].Title,
                            new[] { '—' },
                            new[] { ProcessedDataType.INCOMPATIBLE }, new byte[0, 0]));
                    var processedDataRowEx =
                        new ProcessedDataRow(processedDataEntriesEx, rowIndex, sheetsPaths[i], processedRowType);
                    processedData.Add(processedDataRowEx);

                    Functions.AddProperty(dynamicDataRow, "DataRowObject", processedDataRowEx);
                    var t1Ex = DateTime.Now;
                    runningTotal += (t1Ex - t0).TotalMilliseconds;
                    runningAverage = runningTotal / (i + 1);

                    curSheet.Dispose();

                    worker.ReportProgress(0,
                        (keepData, processedDataRowEx, dynamicDataRow, runningAverage, runningTotal, extraColumns));

                    continue;
                }

                var processedDataEntries = new List<ProcessedDataEntry>();
                var lastDataColumnsIndex = -1;
                ConfigurationBase curConfigurationBase = null;

                var backSheetPath = string.Empty;
                var parameterBasedGradings =
                    new List<(ProcessedDataEntry toGradeEntry, Dictionary<int, byte[]> markCorrectFields, Parameter[]
                        gradingParameters)>();
                for (var i1 = 0; i1 < frontSheetConfigs.Count; i1++)
                {
                    curConfigurationBase = frontSheetConfigs[i1];
                    ProcessedDataEntry? _processedDataEntry = null;
                    Dictionary<int, byte[]> markCorrectFields = null;
                    switch (curConfigurationBase.GetMainConfigType)
                    {
                        case MainConfigType.OMR:
                        {
                            var skipProcessing = false;
                            if (!string.IsNullOrEmpty(curConfigurationBase.ParameterConfigTitle) &&
                                !string.IsNullOrEmpty(curConfigurationBase.ParameterConfigValue))
                            {
                                var processedParameterEntry = processedDataEntries
                                    .Single(x => x.ConfigurationTitle == curConfigurationBase.ParameterConfigTitle);
                                var processedParameterValue = processedParameterEntry.FormatData();

                                if (!string.Equals(processedParameterValue[0],
                                    curConfigurationBase.ParameterConfigValue, StringComparison.OrdinalIgnoreCase))
                                {
                                    var childIndex = allOrderedConfigurations.FindIndex(x => x.ParentTitle == curConfigurationBase.Title && x.ParameterConfigTitle == processedParameterEntry.ConfigurationTitle && string.Equals(x.ParameterConfigValue, processedParameterValue[0], StringComparison.OrdinalIgnoreCase));
                                    if (childIndex > 0)
                                    {
                                        curConfigurationBase = allOrderedConfigurations[childIndex];
                                    }
                                    else
                                    {
                                        _processedDataEntry = CurOMREngine.GetDefaultDataEntry(curConfigurationBase,
                                            alignedSheet, null,
                                            sheetsPaths[i]);

                                        skipProcessing = true;
                                    }
                                }
                            }

                            if (!skipProcessing)
                            {
                                _processedDataEntry = CurOMREngine.ProcessSheet(curConfigurationBase, alignedSheet,
                                    null,
                                    sheetsPaths[i]);
                            }

                            var backSideConfigs = configsBySheetSide.ContainsKey(SheetSideType.Back)
                                ? configsBySheetSide[SheetSideType.Back]
                                    ?.FindAll(x => x.ParentTitle == curConfigurationBase.Title
                                                   && x.ParameterConfigTitle ==
                                                   curConfigurationBase.ParameterConfigTitle &&
                                                   x.ParameterConfigValue == curConfigurationBase.ParameterConfigValue
                                                   && x is OMRConfiguration)
                                    .Cast<OMRConfiguration>()
                                    .ToList()
                                : null;

                            if (backSideConfigs?.Any() ?? false)
                            {
                                backSheetPath = sheetsPaths[i + 1];
                                var curBackSheet = CvInvoke.Imread(backSheetPath, ImreadModes.Grayscale);
                                var backAlignedSheet = CurrentTemplate.AlignSheet(curBackSheet,
                                    out var backAlignmentPipelineResults,
                                    out var isBackTestSuccessful, false);
                                curBackSheet.Dispose();
                                if (backAlignmentPipelineResults.AlignmentMethodTestResultsList.TrueForAll(x =>
                                    x.GetAlignmentMethodResultType ==
                                    AlignmentPipelineResults.AlignmentMethodResultType.Successful))
                                {
                                    for (var iBack = 0; iBack < backSideConfigs.Count; iBack++)
                                    {
                                        var backSideConfig = backSideConfigs[iBack];
                                        ProcessedDataEntry backProcessedDataEntry;
                                        if (!skipProcessing)
                                        {
                                            backProcessedDataEntry = CurOMREngine.ProcessSheet(backSideConfig,
                                                backAlignedSheet, null,
                                                backSheetPath);
                                        }
                                        else
                                        {
                                            backProcessedDataEntry = CurOMREngine.GetDefaultDataEntry(backSideConfig,
                                                backAlignedSheet, null,
                                                backSheetPath);
                                        }

                                        if (_processedDataEntry.HasValue)
                                        {
                                            _processedDataEntry =
                                                _processedDataEntry?.CombineWith(backProcessedDataEntry);
                                        }
                                    }

                                    iIncrementCount++;
                                }
                            }
                        }
                            break;

                        case MainConfigType.BARCODE:
                            _processedDataEntry = CurBarcodeEngine.ProcessSheet(curConfigurationBase, alignedSheet,
                                null, sheetsPaths[i]);
                            break;

                        case MainConfigType.ICR:
                            _processedDataEntry = new ProcessedDataEntry(curConfigurationBase.Title,
                                new[] { 'N', 'I', 'L' },
                                new[] { ProcessedDataType.NORMAL, ProcessedDataType.NORMAL, ProcessedDataType.NORMAL },
                                new byte[0, 0]);
                            break;
                    }

                    if (!_processedDataEntry.HasValue)
                    {
                        continue;
                    }

                    var processedDataEntry = _processedDataEntry.Value;
                    processedDataEntries.Add(processedDataEntry);

                    var formattedOutput = processedDataEntry.FormatData();
                    if (formattedOutput.Length == 1)
                    {
                        var dataTitle = dataColumns != null && dataColumns.Count > 0
                            ? dataColumns[lastDataColumnsIndex + 1]
                            : frontSheetConfigs[i1].Title;
                        Functions.AddProperty(dynamicDataRow, dataTitle, formattedOutput[0]);

                        lastDataColumnsIndex++;
                    }
                    else
                    {
                        for (var i2 = 0; i2 < formattedOutput.Length; i2++)
                        {
                            var dataTitle = dataColumns != null && dataColumns.Count > 0
                                ? dataColumns[lastDataColumnsIndex + 1 + i2]
                                : frontSheetConfigs[i1].Title[0] + (i2 + 1).ToString();
                            Functions.AddProperty(dynamicDataRow, dataTitle, formattedOutput[i2]);
                        }

                        lastDataColumnsIndex += formattedOutput.Length;
                    }

                    switch (curConfigurationBase.GetMainConfigType)
                    {
                        case MainConfigType.OMR:
                            var omrConfig = (OMRConfiguration)curConfigurationBase;
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
                                                        for (var j = startIndex; j < curTotal; j++)
                                                        {
                                                            //processedDataEntry.GetFieldsOutputs[j] = '—';
                                                            processedDataEntry.DataEntriesResultType[j] =
                                                                ProcessedDataType.NORMAL;
                                                            processedDataEntry.SpecialCells.Add(
                                                                new ProcessedDataEntry.SpecialCell((i1, j + 1),
                                                                    Color.FromArgb(220, 238, 245), Color.Black));
                                                            //** IMPOSSIBLE??
                                                            //string dataTitle = dataColumns != null && dataColumns.Count > 0 ? dataColumns[(lastDataColumnsIndex + 1) + j] : allConfigurations[i1].Title[0] + (j + 1).ToString();
                                                            //Functions.AddProperty(dynamicDataRow, dataTitle, "—");
                                                        }
                                                    }

                                                    //var rawValues = ProcessedDataEntry.GenerateRawOMRDataValues(omrConfig, processedDataEntry.GetFieldsOutputs, omrConfig.GetEscapeSymbols());
                                                    var gradeResult = OMREngine.GradeSheet(generalKey,
                                                        processedDataEntry.OptionsOutputs,
                                                        omrConfig.MultiMarkAction);
                                                    Functions.AddProperty(dynamicDataRow, "AnswerKey", generalKey);

                                                    for (var i2 = 0; i2 < 2; i2++)
                                                    {
                                                        var dataTitle = i2 == 0
                                                            ? omrConfig.Title + " Score " +
                                                              omrConfig.GeneralAnswerKeys[k].Title
                                                            : i2 == 1
                                                                ? omrConfig.Title + " Paper " +
                                                                  omrConfig.GeneralAnswerKeys[k].Title
                                                                : omrConfig.Title + $" x{i2}";
                                                        Functions.AddProperty(dynamicDataRow, dataTitle,
                                                            i2 == 0 ? gradeResult.obtainedMarks + "" :
                                                            i2 == 1 ? generalKey.GetPaper.Title : generalKey.Title);

                                                        //lastDataColumnsIndexEx++;
                                                        extraColumns++;
                                                    }
                                                }
                                                catch (Exception ex)
                                                {
                                                    for (var i2 = 0; i2 < 2; i2++)
                                                    {
                                                        var dataTitle = i2 == 0
                                                            ? omrConfig.Title + " Score " +
                                                              omrConfig.GeneralAnswerKeys[k].Title
                                                            : i2 == 1
                                                                ? omrConfig.Title + " Paper " +
                                                                  omrConfig.GeneralAnswerKeys[k].Title
                                                                : omrConfig.Title + $" x{i2}";
                                                        Functions.AddProperty(dynamicDataRow, dataTitle, "—");

                                                        //lastDataColumnsIndexEx++;
                                                        extraColumns++;
                                                    }
                                                }

                                            break;

                                        case KeyType.ParameterBased:
                                            parameterBasedGradings.Add((processedDataEntry, markCorrectFields,
                                                omrConfig.PB_AnswerKeys.Keys.ToArray()));
                                            //lastDataColumnsIndexEx += 3;
                                            break;
                                    }

                                    break;

                                case OMRType.Parameter:
                                    break;
                            }

                            break;

                        case MainConfigType.BARCODE:
                            //OBRConfiguration obrConfig = (OBRConfiguration)curConfigurationBaseEx;
                            break;

                        case MainConfigType.ICR:
                            break;
                    }

                    switch (processedDataEntry.GetRowDataType().GetValueOrDefault())
                    {
                        case ProcessedDataType.NORMAL:
                            break;

                        case ProcessedDataType.MANUAL:
                            if (processedRowType == ProcessedDataType.NORMAL)
                            {
                                processedRowType = ProcessedDataType.MANUAL;
                            }

                            break;

                        case ProcessedDataType.FAULTY:
                            if (processedRowType != ProcessedDataType.INCOMPATIBLE)
                            {
                                processedRowType = ProcessedDataType.FAULTY;
                            }

                            break;

                        case ProcessedDataType.INCOMPATIBLE:
                            processedRowType = ProcessedDataType.INCOMPATIBLE;
                            break;
                    }
                }

                for (var pbEntryIndex = 0; pbEntryIndex < parameterBasedGradings.Count; pbEntryIndex++)
                {
                    var pbGradingData = parameterBasedGradings[pbEntryIndex];
                    var omrConfig = (OMRConfiguration)pbGradingData.toGradeEntry.GetConfigurationBase;
                    var gradingParameters = pbGradingData.gradingParameters;
                    try
                    {
                        var curParameter = gradingParameters.First(x => processedDataEntries.Any(y =>
                            y.GetConfigurationBase.Title == x.parameterConfig.Title &&
                            y.FormatData()[0] == x.parameterValue));
                        var paramKey = omrConfig.PB_AnswerKeys[curParameter];

                        if (pbGradingData.toGradeEntry.GetFieldsOutputs.Length > paramKey.GetPaper.GetFieldsCount)
                        {
                            var startIndex = paramKey.GetPaper.GetFieldsCount;
                            var curTotal = pbGradingData.toGradeEntry.GetFieldsOutputs.Length;
                            for (var j = startIndex; j < curTotal; j++)
                                //pbGradingData.toGradeEntry.GetFieldsOutputs[j] = '—';
                                pbGradingData.toGradeEntry.DataEntriesResultType[j] = ProcessedDataType.NORMAL;
                        }

                        //var rawValues = ProcessedDataEntry.GenerateRawOMRDataValues(omrConfig, pbGradingData.toGradeEntry.GetFieldsOutputs, omrConfig.GetEscapeSymbols());
                        var gradeResult = OMREngine.GradeSheet(paramKey, pbGradingData.toGradeEntry.OptionsOutputs,
                            omrConfig.MultiMarkAction);
                        Functions.AddProperty(dynamicDataRow, "AnswerKey", paramKey);

                        for (var k = 0; k < omrConfig.PB_AnswerKeys.Count; k++)
                        for (var i2 = 0; i2 < 2; i2++)
                        {
                            var dataTitle = i2 == 0
                                ? (omrConfig.ParentTitle ?? omrConfig.Title) + " Score " +
                                  omrConfig.PB_AnswerKeys.Values.ToArray()[k].Title
                                : i2 == 1
                                    ? (omrConfig.ParentTitle ?? omrConfig.Title) + " Paper " +
                                      omrConfig.PB_AnswerKeys.Values.ToArray()[k].Title
                                    : (omrConfig.ParentTitle ?? omrConfig.Title) + $" x{i2}";
                            Functions.AddProperty(dynamicDataRow, dataTitle,
                                i2 == 0 ? gradeResult.obtainedMarks + "" :
                                i2 == 1 ? paramKey.GetPaper.Code : paramKey.Title);

                            extraColumns++;
                        }
                    }
                    catch (Exception ex)
                    {
                        for (var k = 0; k < omrConfig.PB_AnswerKeys.Count; k++)
                        for (var i2 = 0; i2 < 2; i2++)
                        {
                            var dataTitle = i2 == 0
                                ? (omrConfig.ParentTitle ?? omrConfig.Title) + " Score " +
                                  omrConfig.PB_AnswerKeys.Values.ToArray()[k].Title
                                : i2 == 1
                                    ? (omrConfig.ParentTitle ?? omrConfig.Title) + " Paper " +
                                      omrConfig.PB_AnswerKeys.Values.ToArray()[k].Title
                                    : (omrConfig.ParentTitle ?? omrConfig.Title) + $" x{i2}";
                            Functions.AddProperty(dynamicDataRow, dataTitle, "—");

                            extraColumns++;
                        }
                    }
                }

                curSheet.Dispose();

                //File Renaming
                var currentName = sheetsPaths[i];
                var newSheetName = currentName;
                try
                {
                    var curFileName = Path.GetFileNameWithoutExtension(currentName);
                    var newFinalName = curFileName;
                    for (var n = 0; n < renameFields.Count; n++)
                    {
                        var entry = processedDataEntries.Find(x => x.ConfigurationTitle == renameFields[n].Title);
                        if (entry.DataEntriesResultType.Any(x => x != ProcessedDataType.NORMAL))
                        {
                            continue;
                        }

                        var values = entry.GetDataValues;
                        var finalValue = newFinalName == "" ? "" : "-";
                        for (var o = 0; o < values.Length; o++) finalValue += values[o];

                        newFinalName += finalValue;
                    }

                    if (!string.IsNullOrEmpty(newFinalName) && newFinalName != curFileName)
                    {
                        newSheetName = currentName.Replace(curFileName, newFinalName);
                        var existCount = 0;
                        while (File.Exists(newSheetName))
                        {
                            existCount++;
                            var existsFileName = Path.GetFileNameWithoutExtension(newSheetName);
                            newSheetName = newSheetName.Replace(existsFileName, $"{newFinalName} ({existCount})");
                        }

                        File.Move(currentName, newSheetName);

                        sheetsPaths[i] = newSheetName;
                    }
                }
                catch (Exception ex)
                {
                    newSheetName = sheetsPaths[i];
                }

                var processedDataRow = new ProcessedDataRow(processedDataEntries, rowIndex, newSheetName,
                    processedRowType, backSheetPath);
                processedData.Add(processedDataRow);

                Functions.AddProperty(dynamicDataRow, "File Name", Path.GetFileName(newSheetName));
                Functions.AddProperty(dynamicDataRow, "Front Sheet Path", processedDataRow.RowSheetPath);
                if(!string.IsNullOrEmpty(backSheetPath))
                {
                    Functions.AddProperty(dynamicDataRow, "Back Sheet Path", processedDataRow.RowBackSheetPath);
                }
                Functions.AddProperty(dynamicDataRow, "DataRowObject", processedDataRow);
                var t1 = DateTime.Now;
                runningTotal += (t1 - t0).TotalMilliseconds;
                runningAverage = runningTotal / (rowIndex + 1);

                if (i + iIncrementCount >= sheetsPaths.Length)
                {
                    this.GetCurProcessingIndex = sheetsPaths.Length - 1;
                }

                worker.ReportProgress(0,
                    (keepData, processedDataRow, dynamicDataRow, runningAverage, runningTotal, extraColumns));

                //Pause Mechanism
                if (this.IsPaused)
                {
                    do
                    {
                        pauseGrading = !this.IsPaused && this.IsProcessing;
                    } while (!pauseGrading);
                }
                //Dispatcher.CurrentDispatcher.InvokeAsync(new Action(() =>
                //{
                //}), DispatcherPriority.ContextIdle);
            }
        }

        public async Task StartProcessingRaw(Func<bool> GetAlignSheet, Action<Bitmap> OnSheetAligned,
            Action<RectangleF, bool> OnOptionProcessed, Action<string> OnRegionProcessed, Func<double> GetWaitMS,
            Action OnProcessFinished)
        {
            var sheetsPaths = loadedSheetsData.GetSheetsPath;
            var allConfigurations = ConfigurationsManager.GetAllConfigurations;

            for (var i = 0; i < sheetsPaths.Length; i++)
            {
                this.GetCurProcessingIndex = i;

                var curSheet = CvInvoke.Imread(sheetsPaths[i], ImreadModes.Grayscale);
                AlignmentPipelineResults alignmentPipelineResults = null;
                Mat alignedSheet = null;
                if (GetAlignSheet())
                {
                    await Task.Run(() =>
                    {
                        alignedSheet =
                            CurrentTemplate.AlignSheet(curSheet, out var _alignmentPipelineResults, out var _);
                        alignmentPipelineResults = _alignmentPipelineResults;
                    });
                    if (alignmentPipelineResults.AlignmentMethodTestResultsList.TrueForAll(x =>
                        x.GetAlignmentMethodResultType == AlignmentPipelineResults.AlignmentMethodResultType.Failed))
                    {
                        continue;
                    }
                }
                else
                {
                    alignedSheet = curSheet;
                }

                await Task.Run(() => OnSheetAligned(alignedSheet.Bitmap));

                var processedRowType = ProcessedDataType.NORMAL;
                var processedDataEntriesEx = new List<ProcessedDataEntry>();
                for (var i1 = 0; i1 < allConfigurations.Count; i1++)
                {
                    var processedDataEntry = new ProcessedDataEntry();

                    switch (allConfigurations[i1].GetMainConfigType)
                    {
                        case MainConfigType.OMR:
                            var omrConfiguration = (OMRConfiguration)allConfigurations[i1];
                            processedDataEntry = await CurOMREngine.ProcessSheetRaw(omrConfiguration, alignedSheet,
                                OnOptionProcessed, GetWaitMS);
                            break;

                        case MainConfigType.BARCODE:
                            var obrConfiguration = (OBRConfiguration)allConfigurations[i1];
                            processedDataEntry = CurBarcodeEngine.ProcessSheet(obrConfiguration, alignedSheet);
                            break;

                        case MainConfigType.ICR:
                            break;
                    }

                    processedDataEntriesEx.Add(processedDataEntry);

                    var formattedOutput = processedDataEntry.FormatData();
                    if (formattedOutput.Length == 1)
                    {
                        await Task.Run(() => OnRegionProcessed(formattedOutput[0]));
                    }
                    else
                    {
                        var _formattedOutput = "";
                        for (var i2 = 0; i2 < formattedOutput.Length; i2++)
                        {
                            _formattedOutput += formattedOutput[i2];
                            if (i2 < formattedOutput.Length - 1)
                            {
                                _formattedOutput += ", ";
                            }
                            //Functions.AddProperty(dynamicDataRow, allConfigurations[i1].Title[0] + (i2 + 1).ToString(), formattedOutput[i2]);
                        }

                        await Task.Run(() => OnRegionProcessed(_formattedOutput));
                    }

                    switch (processedDataEntry.GetRowDataType().GetValueOrDefault())
                    {
                        case ProcessedDataType.NORMAL:
                            break;

                        case ProcessedDataType.MANUAL:
                            if (processedRowType == ProcessedDataType.NORMAL)
                            {
                                processedRowType = ProcessedDataType.MANUAL;
                            }

                            break;

                        case ProcessedDataType.FAULTY:
                            if (processedRowType != ProcessedDataType.INCOMPATIBLE)
                            {
                                processedRowType = ProcessedDataType.FAULTY;
                            }

                            break;

                        case ProcessedDataType.INCOMPATIBLE:
                            processedRowType = ProcessedDataType.INCOMPATIBLE;
                            break;
                    }
                }

                var processedDataRow =
                    new ProcessedDataRow(processedDataEntriesEx, i, sheetsPaths[i], processedRowType);
                processedData.Add(processedDataRow);
            }

            OnProcessFinished();
        }

        public bool DataExists()
        {
            var result = false;

            result = processedData.Count > 0;

            return result;
        }

        public async void ReprocessData(List<dynamic> selectedData, RereadType rereadType)
        {
            var sheetsPaths = new List<string>();
            selectedData.ForEach(x =>
            {
                sheetsPaths.Add(x.DataRowObject.RowSheetPath);

                if (!string.IsNullOrEmpty(x.DataRowObject.RowBackSheetPath))
                {
                    sheetsPaths.Add(x.DataRowObject.RowBackSheetPath);
                }
            });

            var allOrderedConfigurations = ConfigurationsManager.GetOrderedConfigurations();
            var configsBySheetSide = allOrderedConfigurations.GroupBy(x => x.SheetSide)
                .ToDictionary(x => x.Key, x => x.ToList());

            var frontSheetConfigs = configsBySheetSide[SheetSideType.Front].Where(x => string.IsNullOrEmpty(x.ParentTitle)).ToList();
            var backSheetsCount = 0;
            var iIncrementCount = 1;

            //List<ConfigurationBase> renameFields = allConfigurations.FindAll(x => x.AddToFileName);
            for (var i = 0; i < sheetsPaths.Count; i += iIncrementCount)
            {
                var rowIndex = i - backSheetsCount;
                iIncrementCount = 1;
                this.GetCurProcessingIndex = i;

                dynamic dynamicDataRow = new ExpandoObject();
                var processedRowType = ProcessedDataType.NORMAL;

                Mat curSheet = null;
                AlignmentPipelineResults alignmentPipelineResults = null;
                Mat alignedSheet = null;

                try
                {
                    curSheet = CvInvoke.Imread(sheetsPaths[i], ImreadModes.Grayscale);
                    switch (rereadType)
                    {
                        case RereadType.NORMAL:
                            break;

                        case RereadType.ROTATE_C_90:
                            CvInvoke.Rotate(curSheet, curSheet, RotateFlags.Rotate90Clockwise);
                            break;

                        case RereadType.ROTATE_180:
                            CvInvoke.Rotate(curSheet, curSheet, RotateFlags.Rotate180);
                            break;

                        case RereadType.ROTATE_AC_90:
                            CvInvoke.Rotate(curSheet, curSheet, RotateFlags.Rotate90CounterClockwise);
                            break;
                    }

                    await Task.Run(() =>
                    {
                        alignedSheet = CurrentTemplate.AlignSheet(curSheet, out var _alignmentPipelineResults,
                            out var isTestSuccessful);
                        alignmentPipelineResults = _alignmentPipelineResults;
                    });
                    curSheet.Dispose();
                    if (alignmentPipelineResults.AlignmentMethodTestResultsList.TrueForAll(x =>
                        x.GetAlignmentMethodResultType == AlignmentPipelineResults.AlignmentMethodResultType.Failed))
                    {
                        continue;
                    }
                }
                catch (Exception ex)
                {
                    continue;
                }

                var processedDataEntries = new List<ProcessedDataEntry>();
                var lastDataColumnsIndex = -1;
                ConfigurationBase curConfigurationBase = null;

                var backSheetPath = string.Empty;
                var parameterBasedGradings =
                    new List<(ProcessedDataEntry toGradeEntry, Parameter[] gradingParameters)>();
                for (var i1 = 0; i1 < frontSheetConfigs.Count; i1++)
                {
                    curConfigurationBase = frontSheetConfigs[i1];
                    ProcessedDataEntry? _processedDataEntry = null;
                    Dictionary<int, byte[]> markCorrectFields = null;

                    switch (curConfigurationBase.GetMainConfigType)
                    {
                        case MainConfigType.OMR:
                        {
                            var skipProcessing = false;
                            if (!string.IsNullOrEmpty(curConfigurationBase.ParameterConfigTitle) &&
                                !string.IsNullOrEmpty(curConfigurationBase.ParameterConfigValue))
                            {
                                var processedParameterEntry = processedDataEntries
                                    .Single(x => x.ConfigurationTitle == curConfigurationBase.ParameterConfigTitle);
                                var processedParameterValue = processedParameterEntry.FormatData();

                                if (!string.Equals(processedParameterValue[0],
                                    curConfigurationBase.ParameterConfigValue, StringComparison.OrdinalIgnoreCase))
                                {
                                    var childIndex = allOrderedConfigurations.FindIndex(x => x.ParentTitle == curConfigurationBase.Title && x.ParameterConfigTitle == processedParameterEntry.ConfigurationTitle && string.Equals(x.ParameterConfigValue, processedParameterValue[0], StringComparison.OrdinalIgnoreCase));
                                    if (childIndex > 0)
                                    {
                                        curConfigurationBase = allOrderedConfigurations[childIndex];
                                    }
                                    else
                                    {
                                        _processedDataEntry = CurOMREngine.GetDefaultDataEntry(curConfigurationBase,
                                            alignedSheet, null,
                                            sheetsPaths[i]);

                                        skipProcessing = true;
                                    }
                                }
                            }
                            
                            if (!skipProcessing)
                            {
                                _processedDataEntry = CurOMREngine.ProcessSheet(curConfigurationBase, alignedSheet,
                                    null,
                                    sheetsPaths[i]);
                            }

                            var backSideConfigs = configsBySheetSide.ContainsKey(SheetSideType.Back)
                                ? configsBySheetSide[SheetSideType.Back]
                                    ?.FindAll(x => x.ParentTitle == curConfigurationBase.Title
                                                   && x.ParameterConfigTitle ==
                                                   curConfigurationBase.ParameterConfigTitle &&
                                                   x.ParameterConfigValue == curConfigurationBase.ParameterConfigValue
                                                   && x is OMRConfiguration)
                                    .Cast<OMRConfiguration>()
                                    .ToList()
                                : null;

                            if (backSideConfigs?.Any() ?? false)
                            {
                                backSheetPath = sheetsPaths[i + 1];
                                var curBackSheet = CvInvoke.Imread(backSheetPath, ImreadModes.Grayscale);
                                var backAlignedSheet = CurrentTemplate.AlignSheet(curBackSheet,
                                    out var backAlignmentPipelineResults,
                                    out var isBackTestSuccessful, false);
                                curBackSheet.Dispose();
                                if (backAlignmentPipelineResults.AlignmentMethodTestResultsList.TrueForAll(x =>
                                    x.GetAlignmentMethodResultType ==
                                    AlignmentPipelineResults.AlignmentMethodResultType.Successful))
                                {
                                    for (var iBack = 0; iBack < backSideConfigs.Count; iBack++)
                                    {
                                        var backSideConfig = backSideConfigs[iBack];
                                        ProcessedDataEntry backProcessedDataEntry;
                                        if (!skipProcessing)
                                        {
                                            backProcessedDataEntry = CurOMREngine.ProcessSheet(backSideConfig,
                                                backAlignedSheet, null,
                                                backSheetPath);
                                        }
                                        else
                                        {
                                            backProcessedDataEntry = CurOMREngine.GetDefaultDataEntry(backSideConfig,
                                                backAlignedSheet, null,
                                                backSheetPath);
                                        }

                                        if (_processedDataEntry.HasValue)
                                        {
                                            _processedDataEntry =
                                                _processedDataEntry?.CombineWith(backProcessedDataEntry);
                                        }
                                    }

                                    iIncrementCount++;
                                }
                            }
                        }
                            break;

                        case MainConfigType.BARCODE:
                            _processedDataEntry = CurBarcodeEngine.ProcessSheet(curConfigurationBase, alignedSheet,
                                null, sheetsPaths[i]);
                            break;

                        case MainConfigType.ICR:
                            _processedDataEntry = new ProcessedDataEntry(curConfigurationBase.Title,
                                new[] { 'N', 'I', 'L' },
                                new[] { ProcessedDataType.NORMAL, ProcessedDataType.NORMAL, ProcessedDataType.NORMAL },
                                new byte[0, 0]);
                            break;
                    }

                    if (!_processedDataEntry.HasValue)
                    {
                        continue;
                    }

                    var processedDataEntry = _processedDataEntry.Value;
                    processedDataEntries.Add(processedDataEntry);

                    var formattedOutput = processedDataEntry.FormatData();
                    if (formattedOutput.Length == 1)
                    {
                        var dataTitle = dataColumns != null && dataColumns.Count > 0
                            ? dataColumns[lastDataColumnsIndex + 1]
                            : frontSheetConfigs[i1].Title;
                        Functions.AddProperty(dynamicDataRow, dataTitle, formattedOutput[0]);

                        lastDataColumnsIndex++;
                    }
                    else
                    {
                        for (var i2 = 0; i2 < formattedOutput.Length; i2++)
                        {
                            var dataTitle = dataColumns != null && dataColumns.Count > 0
                                ? dataColumns[lastDataColumnsIndex + 1 + i2]
                                : frontSheetConfigs[i1].Title[0] + (i2 + 1).ToString();
                            Functions.AddProperty(dynamicDataRow, dataTitle, formattedOutput[i2]);
                        }

                        lastDataColumnsIndex += formattedOutput.Length;
                    }

                    switch (processedDataEntry.GetRowDataType().GetValueOrDefault())
                    {
                        case ProcessedDataType.NORMAL:
                            break;

                        case ProcessedDataType.MANUAL:
                            if (processedRowType == ProcessedDataType.NORMAL)
                            {
                                processedRowType = ProcessedDataType.MANUAL;
                            }

                            break;

                        case ProcessedDataType.FAULTY:
                            if (processedRowType != ProcessedDataType.INCOMPATIBLE)
                            {
                                processedRowType = ProcessedDataType.FAULTY;
                            }

                            break;

                        case ProcessedDataType.INCOMPATIBLE:
                            processedRowType = ProcessedDataType.INCOMPATIBLE;
                            break;
                    }

                    switch (curConfigurationBase.GetMainConfigType)
                    {
                        case MainConfigType.OMR:
                            var omrConfig = (OMRConfiguration)curConfigurationBase;
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

                                                    //var rawValues = ProcessedDataEntry.GenerateRawOMRDataValues(omrConfig, processedDataEntry.GetFieldsOutputs, omrConfig.GetEscapeSymbols());
                                                    var gradeResult = OMREngine.GradeSheet(generalKey,
                                                        processedDataEntry.OptionsOutputs,
                                                        omrConfig.MultiMarkAction);
                                                    Functions.AddProperty(dynamicDataRow, "AnswerKey", generalKey);

                                                    for (var i2 = 0; i2 < 2; i2++)
                                                    {
                                                        var dataTitle = i2 == 0
                                                            ? omrConfig.Title + " Score " +
                                                              omrConfig.GeneralAnswerKeys[k].Title
                                                            : i2 == 1
                                                                ? omrConfig.Title + " Paper " +
                                                                  omrConfig.GeneralAnswerKeys[k].Title
                                                                : omrConfig.Title + $" x{i2}";
                                                        Functions.AddProperty(dynamicDataRow, dataTitle,
                                                            i2 == 0 ? gradeResult.obtainedMarks + "" :
                                                            i2 == 1 ? generalKey.GetPaper.Title : generalKey.Title);

                                                        //lastDataColumnsIndexEx++;
                                                    }
                                                }
                                                catch
                                                {
                                                    for (var i2 = 0; i2 < 2; i2++)
                                                    {
                                                        var dataTitle = i2 == 0
                                                            ? omrConfig.Title + " Score " +
                                                              omrConfig.GeneralAnswerKeys[k].Title
                                                            : i2 == 1
                                                                ? omrConfig.Title + " Paper " +
                                                                  omrConfig.GeneralAnswerKeys[k].Title
                                                                : omrConfig.Title + $" x{i2}";
                                                        Functions.AddProperty(dynamicDataRow, dataTitle, "—");

                                                        //lastDataColumnsIndexEx++;
                                                    }
                                                }

                                            break;

                                        case KeyType.ParameterBased:
                                            parameterBasedGradings.Add((processedDataEntry,
                                                omrConfig.PB_AnswerKeys.Keys.ToArray()));
                                            //lastDataColumnsIndexEx += 3;
                                            break;
                                    }

                                    break;

                                case OMRType.Parameter:
                                    break;
                            }

                            break;

                        case MainConfigType.BARCODE:
                            //OBRConfiguration obrConfig = (OBRConfiguration)curConfigurationBaseEx;
                            break;

                        case MainConfigType.ICR:
                            break;
                    }
                }

                for (var pbEntryIndex = 0; pbEntryIndex < parameterBasedGradings.Count; pbEntryIndex++)
                {
                    var pbGradingData = parameterBasedGradings[pbEntryIndex];
                    var omrConfig = (OMRConfiguration)pbGradingData.toGradeEntry.GetConfigurationBase;
                    var gradingParameters = pbGradingData.gradingParameters;
                    try
                    {
                        var curParameter = gradingParameters.First(x => processedDataEntries.Any(y =>
                            y.GetConfigurationBase == x.parameterConfig && y.FormatData()[0] == x.parameterValue));
                        var paramKey = omrConfig.PB_AnswerKeys[curParameter];

                        var rawValues = ProcessedDataEntry.GenerateRawOMRDataValues(omrConfig,
                            pbGradingData.toGradeEntry.GetFieldsOutputs, omrConfig.GetEscapeSymbols());
                        var gradeResult = OMREngine.GradeSheet(paramKey, pbGradingData.toGradeEntry.OptionsOutputs,
                            omrConfig.MultiMarkAction);
                        Functions.AddProperty(dynamicDataRow, "AnswerKey", paramKey);

                        for (var k = 0; k < omrConfig.GeneralAnswerKeys.Count; k++)
                        for (var i2 = 0; i2 < 2; i2++)
                        {
                            var dataTitle = i2 == 0
                                ? omrConfig.Title + " Score " + omrConfig.GeneralAnswerKeys[k].Title
                                : i2 == 1
                                    ? omrConfig.Title + " Paper " + omrConfig.GeneralAnswerKeys[k].Title
                                    : omrConfig.Title + $" x{i2}";
                            Functions.AddProperty(dynamicDataRow, dataTitle,
                                i2 == 0 ? gradeResult.obtainedMarks + "" :
                                i2 == 1 ? paramKey.GetPaper.Title : paramKey.Title);
                        }
                    }
                    catch (Exception ex)
                    {
                        for (var k = 0; k < omrConfig.GeneralAnswerKeys.Count; k++)
                        for (var i2 = 0; i2 < 2; i2++)
                        {
                            var dataTitle = i2 == 0
                                ? omrConfig.Title + " Score " + omrConfig.GeneralAnswerKeys[k].Title
                                : i2 == 1
                                    ? omrConfig.Title + " Paper " + omrConfig.GeneralAnswerKeys[k].Title
                                    : omrConfig.Title + $" x{i2}";
                            Functions.AddProperty(dynamicDataRow, dataTitle, "—");
                        }
                    }
                }

                curSheet.Dispose();

                //File Renaming
                var currentName = sheetsPaths[i];
                var newName = currentName;

                #region Renaming Mechanism

                //try
                //{
                //    string curFileName = Path.GetFileNameWithoutExtension(currentName);
                //    string newFinalName = curFileName;
                //    for (int n = 0; n < renameFields.Count; n++)
                //    {
                //        var entry = processedDataEntries.Find(x => x.ConfigurationTitle == renameFields[n].Title);
                //        if (entry.DataEntriesResultType.Any(x => x != ProcessedDataType.NORMAL))
                //            continue;

                //        string[] values = entry.GetDataValues;
                //        string finalValue = newFinalName == "" ? "" : "-";
                //        for (int o = 0; o < values.Length; o++)
                //        {
                //            finalValue += values[o];
                //        }

                //        newFinalName += finalValue;
                //    }

                //    if (!string.IsNullOrEmpty(newFinalName) && newFinalName != curFileName)
                //    {
                //        newName = currentName.Replace(curFileName, newFinalName);
                //        int existCount = 0;
                //        while (File.Exists(newName))
                //        {
                //            existCount++;
                //            string existsFileName = Path.GetFileNameWithoutExtension(newName);
                //            newName = newName.Replace(existsFileName, $"{newFinalName} ({existCount})");
                //        }
                //        File.Move(currentName, newName);

                //        sheetsPaths[i] = newName;
                //    }
                //}
                //catch (Exception ex)
                //{
                //    newName = sheetsPaths[i];
                //}

                #endregion

                var processedDataRow = new ProcessedDataRow(processedDataEntries, rowIndex, newName, processedRowType, backSheetPath);
                processedDataRow.RereadType = rereadType;

                Functions.AddProperty(dynamicDataRow, "DataRowObject", processedDataRow);

                int dataIndex = manualProcessedDataSource.IndexOf(selectedData[i]);
                manualProcessedDataSource.RemoveAt(dataIndex);
                //manualProcessedDataSource.Insert(dataIndex, dynamicDataRow);
                manualProcessedDataSource.Add(dynamicDataRow);

                int allDataIndex = allProcessedDataSource.IndexOf(selectedData[i]);
                allProcessedDataSource.RemoveAt(allDataIndex);
                allProcessedDataSource.Insert(allDataIndex, dynamicDataRow);

                var proDataIndex =
                    processedData.FindIndex(x => x.RowSheetPath == selectedData[i].DataRowObject.RowSheetPath);
                processedData.RemoveAt(proDataIndex);
                processedData.Insert(proDataIndex, processedDataRow);
            }
        }

        public void SetData(ObservableCollection<dynamic> processedDataSource,
            ObservableCollection<dynamic> manProcessedDataSource, ObservableCollection<dynamic> fauProcessedDataSource,
            ObservableCollection<dynamic> incProcessedDataSource)
        {
            allProcessedDataSource = processedDataSource;
            manualProcessedDataSource = manProcessedDataSource;
            faultyProcessedDataSource = fauProcessedDataSource;
            incompatibleProcessedDataSource = incProcessedDataSource;

            for (var i = 0; i < processedDataSource.Count; i++)
                processedData.Add(processedDataSource[i].DataRowObject);
            for (var i = 0; i < manProcessedDataSource.Count; i++)
                processedData.Add(manProcessedDataSource[i].DataRowObject);
            for (var i = 0; i < fauProcessedDataSource.Count; i++)
                processedData.Add(fauProcessedDataSource[i].DataRowObject);
            for (var i = 0; i < incProcessedDataSource.Count; i++)
                processedData.Add(incProcessedDataSource[i].DataRowObject);

            InitializeDataGrids?.Invoke(allProcessedDataSource[0].DataRowObject.GetProcessedDataEntries,
                (allProcessedDataSource, manualProcessedDataSource, faultyProcessedDataSource,
                    incompatibleProcessedDataSource), 0, true);
        }

        public ObservableCollection<dynamic> GetAllProcessedData()
        {
            if (allProcessedDataSource != null && allProcessedDataSource.Count > 0)
            {
                return allProcessedDataSource;
            }

            return null;
        }

        public ObservableCollection<dynamic> GetAllAltProcessedData()
        {
            if (altAllProcessedDataSource != null && altAllProcessedDataSource.Count > 0)
            {
                return altAllProcessedDataSource;
            }

            return null;
        }
    }
}