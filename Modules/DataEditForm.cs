using Emgu.CV;
using Synapse.Core.Configurations;
using Synapse.Core.Engines.Data;
using Synapse.Utilities;
using Synapse.Utilities.Objects;
using Syncfusion.WinForms.Controls;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using static Synapse.Core.Configurations.ConfigurationBase;
using static Synapse.SynapseMain;

namespace Synapse.Modules
{
    public partial class DataEditForm : SfForm
    {
        #region Properties
        #endregion

        #region Variables
        private OnTemplateConfig onTemplateConfig;
        private RectangleF configRegion;
        private Mat sheetImage;
        private dynamic dataRowObject;
        private ProcessedDataRow selectedProcessedDataRow;
        private ConfigurationBase configurationBase;
        string dataColumnName;
        int columnIndex;

        (int entryIndex, int fieldIndex) cellRepresentation;

        string curValue;
        #endregion

        #region Events
        #endregion

        #region General Methods
        internal DataEditForm(OnTemplateConfig onTemplateConfig, dynamic dataRowObject, string dataColumn, int columnIndex)
        {
            InitializeComponent();

            Mat alignedMat = null;
            if (!dataRowObject.DataRowObject.GetAlignedImage(out alignedMat))
                sheetImage = CvInvoke.Imread(dataRowObject.DataRowObject.RowSheetPath, Emgu.CV.CvEnum.ImreadModes.Grayscale);
            else
                sheetImage = alignedMat;

            this.dataRowObject = dataRowObject;
            selectedProcessedDataRow = dataRowObject.DataRowObject;
            this.dataColumnName = dataColumn;
            this.columnIndex = columnIndex;

            this.onTemplateConfig = onTemplateConfig;

            Awake();
        }
        private void Awake()
        {
            cellRepresentation = GetSynapseMain.GridCellsRepresentation[dataColumnName];
            ProcessedDataEntry entry = selectedProcessedDataRow.GetProcessedDataEntries[cellRepresentation.entryIndex];
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
                default:
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
                return;

            string err = String.Empty;
            string value = dataValueTextBox.Text;
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
                default:
                    break;
            }

            if(!String.IsNullOrEmpty(err))
            {
                Messages.ShowError(err);
                return;
            }

            Functions.AddProperty(dataRowObject, dataColumnName, dataValueTextBox.Text);
            ProcessedDataEntry entry = selectedProcessedDataRow.GetProcessedDataEntries[cellRepresentation.entryIndex];
            entry.GetDataValues[cellRepresentation.fieldIndex] = dataValueTextBox.Text;

            Dispose();
        }

        private void DataEditForm_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                if (dataValueTextBox.Text == "")
                    return;

                Functions.AddProperty(dataRowObject, dataColumnName, dataValueTextBox.Text);
                (int entryIndex, int fieldIndex) cellRepresentation = GetSynapseMain.GridCellsRepresentation[dataColumnName];
                ProcessedDataEntry entry = selectedProcessedDataRow.GetProcessedDataEntries[cellRepresentation.entryIndex];
                entry.GetDataValues[cellRepresentation.fieldIndex] = dataValueTextBox.Text;

                Dispose();
            }
        }

        private void imageBox_Paint(object sender, PaintEventArgs e)
        {
            DrawConfiguration(onTemplateConfig, e.Graphics);
        }

        private void DrawConfiguration(OnTemplateConfig onTemplateConfig, Graphics g)
        {
            ConfigArea configArea = onTemplateConfig.Configuration.GetConfigArea;
            MainConfigType mainConfigType = onTemplateConfig.Configuration.GetMainConfigType;

            ColorStates colorStates = onTemplateConfig.ColorStates;

            GraphicsState originalState;
            RectangleF curDrawFieldRectF = imageBox.GetOffsetRectangle(configArea.ConfigRect);
            onTemplateConfig.OffsetRectangle = curDrawFieldRectF;

            originalState = g.Save();

            switch (mainConfigType)
            {
                case MainConfigType.OMR:
                    Utilities.Functions.DrawBox(g, curDrawFieldRectF, imageBox.ZoomFactor, colorStates.CurrentColor, 0);
                    break;
                case MainConfigType.BARCODE:
                    Utilities.Functions.DrawBox(g, curDrawFieldRectF, imageBox.ZoomFactor, colorStates.CurrentColor, 0);
                    break;
                case MainConfigType.ICR:
                    //if (configArea.ConfigRect.Contains(curImageMouseLoc))
                    Utilities.Functions.DrawBox(g, curDrawFieldRectF, imageBox.ZoomFactor, colorStates.CurrentColor, 0);
                    break;
            }

            g.Restore(originalState);
        }

    }
}