using static Synapse.Core.Templates.Template;
using static Synapse.SynapseMain;

namespace Synapse.Modules
{
    using System;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Linq;
    using System.Windows.Forms;

    using Emgu.CV;
    using Emgu.CV.CvEnum;

    using Synapse.Core.Configurations;
    using Synapse.Core.Engines;
    using Synapse.Core.Engines.Data;
    using Synapse.Core.Keys;
    using Synapse.Core.Managers;
    using Synapse.Utilities;

    using Syncfusion.WinForms.Controls;

    public partial class DataEditForm : SfForm
    {
        #region Properties

        #endregion

        #region Variables

        private readonly OnTemplateConfig onTemplateConfig;
        private RectangleF configRegion;
        private readonly Mat sheetImage;
        private readonly dynamic dataRowObjectObject;
        private ProcessedDataRow selectedProcessedDataRow;
        private ConfigurationBase configurationBase;
        private readonly string dataColumnName;
        private int columnIndex;

        private (int entryIndex, int fieldIndex) cellRepresentation;

        private string curValue;

        #endregion

        #region Events

        #endregion

        #region General Methods

        public DataEditForm(OnTemplateConfig onTemplateConfig, dynamic dataRowObjectObject, string dataColumn,
            int columnIndex)
        {
            this.InitializeComponent();

            Mat alignedMat = null;
            if (!GetCurrentTemplate.GetAlignedImage(dataRowObjectObject.DataRowObject.RowSheetPath,
                dataRowObjectObject.DataRowObject.RereadType, out alignedMat, out bool _))
            {
                sheetImage = CvInvoke.Imread(dataRowObjectObject.DataRowObject.RowSheetPath, ImreadModes.Grayscale);
            }
            else
            {
                sheetImage = alignedMat;
            }

            this.dataRowObjectObject = dataRowObjectObject;
            selectedProcessedDataRow = dataRowObjectObject.DataRowObject;
            dataColumnName = dataColumn;
            this.columnIndex = columnIndex;

            this.onTemplateConfig = onTemplateConfig;

            this.Awake();
        }

        private void Awake()
        {
            cellRepresentation = GetSynapseMain.GridCellsRepresentation[dataColumnName];
            var entry = selectedProcessedDataRow.GetProcessedDataEntries[cellRepresentation.entryIndex];
            configRegion = entry.GetConfigurationBase.GetConfigArea.ConfigRect;
            imageBox.Image = sheetImage.Bitmap;
            imageBox.ZoomToRegion(configRegion);
            curValue = Functions.GetProperty(dataRowObjectObject, dataColumnName);
            configurationBase = entry.GetConfigurationBase;

            dataValueTextBox.Text = curValue;
            dataValueTextBox.SelectionStart = 0;
            dataValueTextBox.SelectionLength = dataValueTextBox.Text.Length;
            dataValueTextBox.Focus();

            switch (configurationBase.GetMainConfigType)
            {
                case MainConfigType.OMR:
                    dataValueTextBox.MaxLength = curValue.Length;
                    break;

                case MainConfigType.BARCODE:
                    break;

                case MainConfigType.ICR:
                    break;
            }
        }

        #endregion

        #region UI Methods

        #endregion

        #region Main Methods

        #endregion

        private void setDataValueBtn_Click(object sender, EventArgs e)
        {
            if (dataValueTextBox.Text == "")
            {
                return;
            }

            var err = string.Empty;
            var value = dataValueTextBox.Text;
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

            var newValue = dataValueTextBox.Text;
            Functions.AddProperty(dataRowObjectObject, dataColumnName, newValue);
            var processedDataEntry = selectedProcessedDataRow.GetProcessedDataEntries[cellRepresentation.entryIndex];
            processedDataEntry.DataEntriesResultType[cellRepresentation.fieldIndex] = ProcessedDataType.NORMAL;
            processedDataEntry.GetDataValues[cellRepresentation.fieldIndex] = dataValueTextBox.Text;
            selectedProcessedDataRow.GetProcessedDataEntries[cellRepresentation.entryIndex] = processedDataEntry;

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
                                            Functions.AddProperty(dataRowObjectObject, "AnswerKey", generalKey);

                                            for (var i2 = 0; i2 < 2; i2++)
                                            {
                                                var dataTitle = i2 == 0
                                                    ? omrConfig.Title + " Score " + omrConfig.GeneralAnswerKeys[k].Title
                                                    : i2 == 1
                                                        ? omrConfig.Title + " Paper " +
                                                          omrConfig.GeneralAnswerKeys[k].Title
                                                        : omrConfig.Title + $" x{i2}";
                                                Functions.AddProperty(dataRowObjectObject, dataTitle,
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
                                                Functions.AddProperty(dataRowObjectObject, dataTitle, "—");

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
                                .GetConfigurations(MainConfigType.OMR,
                                    x => (x as OMRConfiguration).OMRType == OMRType.Gradable &&
                                         (x as OMRConfiguration).KeyType == KeyType.ParameterBased)
                                .Cast<OMRConfiguration>()
                                .SelectMany(config => config.PB_AnswerKeys.ToList())
                                .Where(keyValue => keyValue.Key.parameterConfig.Title == omrConfig.Title
                                                   && keyValue.Key.parameterValue == value)
                                .ToList();

                            var allOrderedConfigurations = ConfigurationsManager.GetOrderedConfigurations();
                            var configsBySheetSide = allOrderedConfigurations.GroupBy(x => x.SheetSide)
                                .ToDictionary(x => x.Key, x => x.ToList());

                            var omrEngine = new OMREngine();

                            gradingKeys.ForEach(gradingKey =>
                            {
                                var answerKey = gradingKey.Value;
                                var answerKeyConfig =
                                    (OMRConfiguration)ConfigurationsManager.GetConfiguration(answerKey.GetConfigName);

                                var answersProcessedDataEntry = omrEngine.ProcessSheet(answerKeyConfig, sheetImage,
                                    null, selectedProcessedDataRow.RowSheetPath);

                                var backSideConfigs = configsBySheetSide.ContainsKey(SheetSideType.Back)
                                    ? configsBySheetSide[SheetSideType.Back]
                                        ?.FindAll(x => x.ParentTitle == answerKeyConfig.Title
                                                       && x.ParameterConfigTitle ==
                                                       answerKeyConfig.ParameterConfigTitle &&
                                                       x.ParameterConfigValue == answerKeyConfig.ParameterConfigValue
                                                       && x is OMRConfiguration)
                                        .Cast<OMRConfiguration>()
                                        .ToList()
                                    : null;

                                if (backSideConfigs?.Any() ?? false)
                                {
                                    var backSheetPath = selectedProcessedDataRow.RowBackSheetPath;
                                    selectedProcessedDataRow.RowBackSheetPath = backSheetPath;
                                    var curBackSheet = CvInvoke.Imread(backSheetPath, ImreadModes.Grayscale);
                                    var backAlignedSheet = GetCurrentTemplate.AlignSheet(curBackSheet,
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
                                            var backProcessedDataEntry = omrEngine.ProcessSheet(backSideConfig,
                                                backAlignedSheet, null,
                                                backSheetPath);

                                            answersProcessedDataEntry =
                                                answersProcessedDataEntry.CombineWith(backProcessedDataEntry);
                                        }
                                    }
                                }

                                var answersConfigTitle = answerKeyConfig.ParentTitle ?? answerKeyConfig.Title;
                                var answersProcessedEntryIndex =
                                    selectedProcessedDataRow.GetProcessedDataEntries.FindIndex(x =>
                                        x.ConfigurationTitle == answersConfigTitle);

                                var formattedOutput = answersProcessedDataEntry.FormatData();
                                if (formattedOutput.Length == 1)
                                {
                                    var dataTitle = answersConfigTitle;
                                    Functions.AddProperty(dataRowObjectObject, dataTitle, formattedOutput[0]);
                                }
                                else
                                {
                                    for (var i2 = 0; i2 < formattedOutput.Length; i2++)
                                    {
                                        var dataTitle = answersConfigTitle[0] + (i2 + 1).ToString();
                                        Functions.AddProperty(dataRowObjectObject, dataTitle, formattedOutput[i2]);
                                    }
                                }

                                if (answersProcessedEntryIndex > 0)
                                {
                                    selectedProcessedDataRow.GetProcessedDataEntries[answersProcessedEntryIndex] =
                                        answersProcessedDataEntry;
                                }
                                else
                                {
                                    selectedProcessedDataRow.GetProcessedDataEntries.Add(answersProcessedDataEntry);
                                }

                                var gradingResult =
                                    OMREngine.GradeSheet(answerKey, answersProcessedDataEntry.OptionsOutputs,
                                        answerKeyConfig.MultiMarkAction);

                                Functions.AddProperty(dataRowObjectObject, "AnswerKey", answerKey);

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
                                    Functions.AddProperty(dataRowObjectObject, dataTitle,
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

            if (!string.IsNullOrEmpty(err))
            {
                Messages.ShowError(err);
                return;
            }

            selectedProcessedDataRow.IsEdited = true;
            Functions.AddProperty(dataRowObjectObject, "DataRowObject", selectedProcessedDataRow);

            this.Dispose();
        }

        private void DataEditForm_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                if (dataValueTextBox.Text == "")
                {
                    return;
                }

                Functions.AddProperty(dataRowObjectObject, dataColumnName, dataValueTextBox.Text);
                var cellRepresentation = GetSynapseMain.GridCellsRepresentation[dataColumnName];
                var entry = selectedProcessedDataRow.GetProcessedDataEntries[cellRepresentation.entryIndex];
                entry.GetDataValues[cellRepresentation.fieldIndex] = dataValueTextBox.Text;

                this.Dispose();
            }
        }

        private void imageBox_Paint(object sender, PaintEventArgs e)
        {
            this.DrawConfiguration(onTemplateConfig, e.Graphics);
        }

        private void DrawConfiguration(OnTemplateConfig onTemplateConfig, Graphics g)
        {
            var configArea = onTemplateConfig.Configuration.GetConfigArea;
            var mainConfigType = onTemplateConfig.Configuration.GetMainConfigType;

            var colorStates = onTemplateConfig.ColorStates;

            GraphicsState originalState;
            var curDrawFieldRectF = imageBox.GetOffsetRectangle(configArea.ConfigRect);
            onTemplateConfig.OffsetRectangle = curDrawFieldRectF;

            originalState = g.Save();

            switch (mainConfigType)
            {
                case MainConfigType.OMR:
                    Functions.DrawBox(g, curDrawFieldRectF, imageBox.ZoomFactor, colorStates.CurrentColor, 0);
                    break;

                case MainConfigType.BARCODE:
                    Functions.DrawBox(g, curDrawFieldRectF, imageBox.ZoomFactor, colorStates.CurrentColor, 0);
                    break;

                case MainConfigType.ICR:
                    //if (configArea.ConfigRect.Contains(curImageMouseLoc))
                    Functions.DrawBox(g, curDrawFieldRectF, imageBox.ZoomFactor, colorStates.CurrentColor, 0);
                    break;
            }

            g.Restore(originalState);
        }
    }
}