namespace Synapse.Core.Engines.Data
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Text;

    using Inlite.ClearImageNet;

    using Synapse.Core.Configurations;
    using Synapse.DeCore.Engines.Data;

    [Serializable]
    public struct ProcessedDataEntry
    {
        #region Objects

        [Serializable]
        public class SpecialCell
        {
            public SpecialCell((int entryIndex, int fieldIndex) cell, Color cellBackColor, Color cellForeColor)
            {
                this.cell = cell;
                this.cellBackColor = cellBackColor;
                this.cellForeColor = cellForeColor;
            }

            public (int entryIndex, int fieldIndex) cell { get; set; }
            public Color cellBackColor { get; set; }
            public Color cellForeColor { get; set; }
        }

        #endregion

        #region Properties

        public ConfigurationBase GetConfigurationBase =>
            Communicator.GetConfigurationBase?.Invoke(this.ActualConfigurationTitle);

        public string ConfigurationTitle { get; private set; }
        public string ActualConfigurationTitle { get; private set; }
        public MainConfigType GetMainConfigType => this.GetConfigurationBase.GetMainConfigType;
        public ProcessedDataType[] DataEntriesResultType { get; set; }
        public byte[,] OptionsOutputs { get; set; }

        public char[] GetFieldsOutputs
        {
            get => fieldsOutputs;
            set => fieldsOutputs = value;
        }

        private char[] fieldsOutputs;

        public string[] GetDataValues
        {
            get => dataValues;
            set => dataValues = value;
        }

        private string[] dataValues;

        public List<SpecialCell> SpecialCells;
        public Barcode[] BarcodesResult;

        public bool IsEdited { get; set; }

        #endregion


        #region Methods

        public ProcessedDataEntry(string configurationTitle, char[] fieldsOutputs,
            ProcessedDataType[] processedDataResultType, byte[,] optionsOutputs, Barcode[] barcodesResult = null,
            string actualConfigurationTitle = null)
        {
            this.ConfigurationTitle = configurationTitle;
            this.ActualConfigurationTitle = actualConfigurationTitle;
            this.fieldsOutputs = fieldsOutputs;
            this.OptionsOutputs = optionsOutputs;

            this.DataEntriesResultType = processedDataResultType;
            dataValues = null;
            this.IsEdited = false;
            SpecialCells = new List<SpecialCell>();

            BarcodesResult = barcodesResult;
            this.FormatData();
        }

        public ProcessedDataType? GetRegionDataType()
        {
            ProcessedDataType? result = null;

            if (this.GetMainConfigType == MainConfigType.OMR)
            {
                var omrConfiguration = (OMRConfiguration)this.GetConfigurationBase;
                switch (omrConfiguration.ValueRepresentation)
                {
                    case ValueRepresentation.Collective:
                        result = this.DataEntriesResultType.Contains(ProcessedDataType.INCOMPATIBLE)
                            ? ProcessedDataType.INCOMPATIBLE
                            : this.DataEntriesResultType.Contains(ProcessedDataType.FAULTY)
                                ? ProcessedDataType.FAULTY
                                : this.DataEntriesResultType.Contains(ProcessedDataType.MANUAL)
                                    ? ProcessedDataType.MANUAL
                                    : ProcessedDataType.NORMAL;
                        break;

                    case ValueRepresentation.Indiviual:
                        result = null;
                        break;

                    case ValueRepresentation.CombineTwo:
                        result = null;
                        break;
                }
            }
            else
            {
                result = this.DataEntriesResultType.Contains(ProcessedDataType.INCOMPATIBLE)
                    ? ProcessedDataType.INCOMPATIBLE
                    : this.DataEntriesResultType.Contains(ProcessedDataType.FAULTY)
                        ? ProcessedDataType.FAULTY
                        : this.DataEntriesResultType.Contains(ProcessedDataType.MANUAL)
                            ? ProcessedDataType.MANUAL
                            : ProcessedDataType.NORMAL;
            }

            return result;
        }

        public ProcessedDataType? GetRowDataType()
        {
            ProcessedDataType? result = null;

            if (this.GetMainConfigType == MainConfigType.OMR)
            {
                var omrConfiguration = (OMRConfiguration)this.GetConfigurationBase;
                switch (omrConfiguration.ValueRepresentation)
                {
                    case ValueRepresentation.Collective:
                        result = this.DataEntriesResultType.Contains(ProcessedDataType.INCOMPATIBLE)
                            ? ProcessedDataType.INCOMPATIBLE
                            : this.DataEntriesResultType.Contains(ProcessedDataType.FAULTY)
                                ? ProcessedDataType.FAULTY
                                : this.DataEntriesResultType.Contains(ProcessedDataType.MANUAL)
                                    ? ProcessedDataType.MANUAL
                                    : ProcessedDataType.NORMAL;
                        break;

                    case ValueRepresentation.Indiviual:
                        result = this.DataEntriesResultType.Contains(ProcessedDataType.INCOMPATIBLE)
                            ? ProcessedDataType.INCOMPATIBLE
                            : this.DataEntriesResultType.Contains(ProcessedDataType.FAULTY)
                                ? ProcessedDataType.FAULTY
                                : this.DataEntriesResultType.Contains(ProcessedDataType.MANUAL)
                                    ? ProcessedDataType.MANUAL
                                    : ProcessedDataType.NORMAL;
                        break;

                    case ValueRepresentation.CombineTwo:
                        result = this.DataEntriesResultType.Contains(ProcessedDataType.INCOMPATIBLE)
                            ? ProcessedDataType.INCOMPATIBLE
                            : this.DataEntriesResultType.Contains(ProcessedDataType.FAULTY)
                                ? ProcessedDataType.FAULTY
                                : this.DataEntriesResultType.Contains(ProcessedDataType.MANUAL)
                                    ? ProcessedDataType.MANUAL
                                    : ProcessedDataType.NORMAL;
                        break;
                }
            }
            else
            {
                result = this.DataEntriesResultType.Contains(ProcessedDataType.INCOMPATIBLE)
                    ? ProcessedDataType.INCOMPATIBLE
                    : this.DataEntriesResultType.Contains(ProcessedDataType.FAULTY)
                        ? ProcessedDataType.FAULTY
                        : this.DataEntriesResultType.Contains(ProcessedDataType.MANUAL)
                            ? ProcessedDataType.MANUAL
                            : ProcessedDataType.NORMAL;
            }

            return result;
        }

        public string[] FormatData()
        {
            var config = this.GetConfigurationBase;

            var fieldsOutput = new string(fieldsOutputs);

            if (config.GetMainConfigType == MainConfigType.OMR)
            {
                var configOMR = (OMRConfiguration)config;
                if (configOMR.ImplicitValue)
                {
                    var impliedValue = "";
                    var startImplied = false;
                    var endRemoveCount = 0;
                    for (var i = 0; i < fieldsOutput.Length; i++)
                    {
                        if (fieldsOutput[i] != configOMR.NoneMarkedSymbol)
                        {
                            startImplied = true;
                        }
                        else if (!startImplied)
                        {
                            this.DataEntriesResultType[i] = ProcessedDataType.NORMAL;
                        }

                        if (startImplied)
                        {
                            impliedValue += fieldsOutput[i];

                            if (fieldsOutput[i] == configOMR.NoneMarkedSymbol)
                            {
                                endRemoveCount++;
                            }
                            else
                            {
                                endRemoveCount = 0;
                            }
                        }
                    }

                    for (var i = this.DataEntriesResultType.Length - endRemoveCount;
                        i < this.DataEntriesResultType.Length;
                        i++) this.DataEntriesResultType[i] = ProcessedDataType.NORMAL;
                    impliedValue = impliedValue.Remove(impliedValue.Length - endRemoveCount, endRemoveCount);
                    fieldsOutput = impliedValue;
                }
            }

            var result = new List<string>();
            switch (config.ValueRepresentation)
            {
                case ValueRepresentation.Collective:
                    result.Add(fieldsOutput);
                    break;

                case ValueRepresentation.Indiviual:
                    for (var i = 0; i < fieldsOutput.Length; i++) result.Add(fieldsOutput[i] + "");
                    break;

                case ValueRepresentation.CombineTwo:
                    if (fieldsOutput.Length % 2 == 0)
                    {
                        for (var i = 0; i < fieldsOutput.Length; i += 2)
                            result.Add(string.Concat(fieldsOutput[i] + fieldsOutput[i + 1]));
                    }
                    else
                    {
                        for (var i = 0; i < fieldsOutput.Length; i++) result.Add(fieldsOutput[i] + "");
                    }

                    break;
            }

            dataValues = result.ToArray();

            return dataValues;
        }

        public static byte[,] GenerateRawOMRDataValues(OMRConfiguration omrConfiguration, char[] fieldsOutputs,
            char[] escapeChars)
        {
            var totalFields = (byte)omrConfiguration.GetTotalFields;
            var totalOptions = (byte)omrConfiguration.GetTotalOptions;

            var result = new byte[totalFields, totalOptions];

            var outputType = omrConfiguration.ValueDataType;
            var ascii = Encoding.ASCII.GetBytes(fieldsOutputs);
            var escapeAscii = Encoding.ASCII.GetBytes(escapeChars);
            switch (outputType)
            {
                case ValueDataType.String:
                    break;

                case ValueDataType.Text:
                    break;

                case ValueDataType.Alphabet:
                    for (var i = 0; i < totalFields; i++)
                    {
                        if (escapeAscii.Contains(ascii[i]))
                        {
                            continue;
                        }

                        if (ascii[i] == 64)
                        {
                            result[i, 0] = 2;
                            continue;
                        }

                        var optionIndex = ascii[i] - 65;
                        result[i, optionIndex] = 1;
                    }

                    break;

                case ValueDataType.WholeNumber:
                    for (var i = 0; i < totalFields; i++)
                    {
                        if (escapeAscii.Contains(ascii[i]))
                        {
                            continue;
                        }

                        var optionIndex = ascii[i] - 48;
                        result[i, optionIndex] = 1;
                    }

                    break;

                case ValueDataType.NaturalNumber:
                    for (var i = 0; i < totalFields; i++)
                    {
                        if (escapeAscii.Contains(ascii[i]))
                        {
                            continue;
                        }

                        var optionIndex = ascii[i] - 49;
                        result[i, optionIndex] = 1;
                    }

                    break;

                case ValueDataType.Integer:
                    break;
            }

            return result;
        }

        public ProcessedDataEntry CombineWith(ProcessedDataEntry toCombine)
        {
            var _fieldsOutputs = fieldsOutputs.Concat(toCombine.GetFieldsOutputs).ToArray();
            var dataEntriesResultType = this.DataEntriesResultType
                .Concat(toCombine.DataEntriesResultType).ToArray();
            var barcodesResult = BarcodesResult?.Concat(toCombine.BarcodesResult).ToArray();

            var curWidth = this.OptionsOutputs.GetLength(0);
            var widthToAdd = toCombine.OptionsOutputs.GetLength(0);
            var newWidth = curWidth + widthToAdd;

            var curHeight = this.OptionsOutputs.GetLength(1);
            var heightToAdd = toCombine.OptionsOutputs.GetLength(1);
            var newOptionOutputs = new byte[newWidth, curHeight];
            for (var i = 0; i < curWidth; i++)
            for (var j = 0; j < curHeight; j++)
                newOptionOutputs[i, j] = this.OptionsOutputs[i, j];
            for (var i = 0; i < widthToAdd; i++)
            for (var j = 0; j < heightToAdd; j++)
                newOptionOutputs[i + curWidth, j] = toCombine.OptionsOutputs[i, j];

            return new ProcessedDataEntry(this.ConfigurationTitle, _fieldsOutputs, dataEntriesResultType,
                newOptionOutputs, barcodesResult, this.ActualConfigurationTitle);
        }

        #endregion
    }
}