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
    using Synapse.Core.Engines.Data;
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
        private readonly dynamic dataRowObject;
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

        public DataEditForm(OnTemplateConfig onTemplateConfig, dynamic dataRowObject, string dataColumn,
            int columnIndex)
        {
            this.InitializeComponent();

            Mat alignedMat = null;
            if (!GetCurrentTemplate.GetAlignedImage(dataRowObject.DataRowObject.RowSheetPath,
                dataRowObject.DataRowObject.RereadType, out alignedMat, out bool _))
            {
                sheetImage = CvInvoke.Imread(dataRowObject.DataRowObject.RowSheetPath, ImreadModes.Grayscale);
            }
            else
            {
                sheetImage = alignedMat;
            }

            this.dataRowObject = dataRowObject;
            selectedProcessedDataRow = dataRowObject.DataRowObject;
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
            curValue = Functions.GetProperty(dataRowObject, dataColumnName);
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

            switch (configurationBase.GetMainConfigType)
            {
                case MainConfigType.OMR:

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

            Functions.AddProperty(dataRowObject, dataColumnName, dataValueTextBox.Text);
            var entry = selectedProcessedDataRow.GetProcessedDataEntries[cellRepresentation.entryIndex];
            entry.DataEntriesResultType[cellRepresentation.fieldIndex] = ProcessedDataType.NORMAL;
            entry.GetDataValues[cellRepresentation.fieldIndex] = dataValueTextBox.Text;

            selectedProcessedDataRow.IsEdited = true;
            selectedProcessedDataRow.GetProcessedDataEntries[cellRepresentation.entryIndex] = entry;
            Functions.AddProperty(dataRowObject, "DataRowObject", selectedProcessedDataRow);

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

                Functions.AddProperty(dataRowObject, dataColumnName, dataValueTextBox.Text);
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