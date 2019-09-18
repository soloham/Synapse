using Cyotek.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;
using Synapse.Core.Configurations;
using Synapse.Core.Engines.Data;
using Synapse.Core.Templates;
using Synapse.Utilities;
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
        private List<ProcessedDataRow> processedData = new List<ProcessedDataRow>();
        private ObservableCollection<dynamic> processedDataSource = new ObservableCollection<dynamic>();

        public ProcessingManager(Template currentTemplate)
        {
            CurrentTemplate = currentTemplate;
        }

        internal void LoadSheets(SheetsList sheetsList)
        {
            this.loadedSheetsData = sheetsList;
        }
        internal async Task StartProcessing(bool keepData)
        {
            if (!keepData)
                processedDataSource.Clear();

            string[] sheetsPaths = loadedSheetsData.GetSheetsPath;
            var allConfigurations = ConfigurationsManager.GetAllConfigurations;

            for (int i = 0; i < sheetsPaths.Length; i++)
            {
                GetCurProcessingIndex = i;

                dynamic dynamicDataRow = new ExpandoObject();
                Mat curSheet = new Mat(sheetsPaths[i]);
                AlignmentPipelineResults alignmentPipelineResults = null;
                Image<Gray, byte> alignedSheet = null;
                await Task.Run(() => { alignedSheet = CurrentTemplate.AlignSheet(curSheet.ToImage<Gray, byte>(), out AlignmentPipelineResults _alignmentPipelineResults); alignmentPipelineResults = _alignmentPipelineResults; });
                if (alignmentPipelineResults.AlignmentMethodTestResultsList.TrueForAll(x => x.GetAlignmentMethodResultType == AlignmentPipelineResults.AlignmentMethodResultType.Failed))
                    continue;

                //templateImageBox.Image = alignedSheet.Bitmap;
                //await Task.Delay(TimeSpan.FromMilliseconds(0.5));

                List<ProcessedDataEntry> processedDataEntries = new List<ProcessedDataEntry>();
                for (int i1 = 0; i1 < allConfigurations.Count; i1++)
                {
                    var processedDataEntry = allConfigurations[i1].ProcessSheet(alignedSheet.Mat);
                    processedDataEntries.Add(processedDataEntry);

                    string[] formattedOutput = processedDataEntry.FormatData();
                    if (formattedOutput.Length == 1)
                    {
                        Functions.AddProperty(dynamicDataRow, allConfigurations[i1].Title, formattedOutput[0]);
                    }
                    else
                    {
                        for (int i2 = 0; i2 < formattedOutput.Length; i2++)
                        {
                            Functions.AddProperty(dynamicDataRow, allConfigurations[i1].Title[0] + (i2 + 1).ToString(), formattedOutput[i2]);
                        }
                    }
                }

                ProcessedDataRow processedDataRow = new ProcessedDataRow(processedDataEntries, i, sheetsPaths[i], ProcessedDataResultType.NORMAL, alignmentPipelineResults);
                processedData.Add(processedDataRow);

                Functions.AddProperty(dynamicDataRow, "DataRowObject", processedDataRow);
                processedDataSource.Add(dynamicDataRow);

                if (i == 0 && !keepData)
                    SynapseMain.GetSynapseMain.InitializeMainDataGrid(processedDataEntries, processedDataSource);
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
                    ProcessedDataEntry processedDataEntry = null;
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

                ProcessedDataRow processedDataRow = new ProcessedDataRow(processedDataEntries, i, sheetsPaths[i], ProcessedDataResultType.NORMAL, alignmentPipelineResults);
                processedData.Add(processedDataRow);
            }

            OnProcessFinished();
        }

        internal bool DataExists()
        {
            bool result = false;

            result = processedDataSource.Count > 0;

            return result;
        }
    }
}
