using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Synapse.Core.Configurations;
using Synapse.Core.Managers;

namespace Synapse.Core.Engines.Data
{
    [Serializable]
    internal struct ProcessedDataEntry
    {
        #region Properties
        public ConfigurationBase GetConfigurationBase { get => ConfigurationsManager.GetConfiguration(configurationTitle); }
        private string configurationTitle;
        public MainConfigType GetMainConfigType { get => GetConfigurationBase.GetMainConfigType; }
        public ProcessedDataType DataEntryResultType { get; set; }

        public char[] GetFieldsOutputs { get => fieldsOutputs; }
        private char[] fieldsOutputs;
        public string[] GetDataValues { get => dataValues; }
        private string[] dataValues;

        public bool IsEdited { get; set; }
        #endregion

        #region Methods
        public ProcessedDataEntry(string configurationTitle, char[] fieldsOutputs, ProcessedDataType processedDataResultType)
        {
            this.configurationTitle = configurationTitle;
            this.fieldsOutputs = fieldsOutputs;

            DataEntryResultType = processedDataResultType;
            dataValues = null;
            IsEdited = false;

            FormatData();
        }

        public string[] FormatData()
        {
            List<string> result = new List<string>();
            var config = GetConfigurationBase;
            switch (config.ValueRepresentation)
            {
                case ValueRepresentation.Collective:
                    result.Add(string.Concat(fieldsOutputs));
                    break;
                case ValueRepresentation.Indiviual:
                    for (int i = 0; i < fieldsOutputs.Length; i++)
                    {
                        result.Add(fieldsOutputs[i] + "");
                    }
                    break;
                case ValueRepresentation.CombineTwo:
                    if (fieldsOutputs.Length % 2 == 0)
                    {
                        for (int i = 0; i < fieldsOutputs.Length; i += 2)
                        {
                            result.Add(string.Concat(fieldsOutputs[i] + fieldsOutputs[i + 1]));
                        }
                    }
                    else
                    {
                        for (int i = 0; i < fieldsOutputs.Length; i++)
                        {
                            result.Add(fieldsOutputs[i] + "");
                        }
                    }
                    break;
            }

            dataValues = result.ToArray();

            return dataValues;
        }
        public static byte[,] GenerateRawOMRDataValues(OMRConfiguration omrConfiguration, char[] fieldsOutputs, char[] escapeChars)
        {
            byte totalFields = (byte)omrConfiguration.GetTotalFields;
            byte totalOptions = (byte)omrConfiguration.GetTotalOptions;

            byte[,] result = new byte[totalFields, totalOptions];

            var outputType = omrConfiguration.ValueDataType;
            byte[] ascii = Encoding.ASCII.GetBytes(fieldsOutputs);
            byte[] escapeAscii = Encoding.ASCII.GetBytes(escapeChars);
            switch (outputType)
            {
                case ValueDataType.String:
                    break;
                case ValueDataType.Text:
                    break;
                case ValueDataType.Alphabet:
                    for (int i = 0; i < ascii.Length; i++)
                    {
                        if (escapeAscii.Contains(ascii[i]))
                            continue;

                        int optionIndex = ascii[i] - 65;
                        result[i, optionIndex] = 1;
                    }
                    break;
                case ValueDataType.WholeNumber:
                    for (int i = 0; i < ascii.Length; i++)
                    {
                        if (escapeAscii.Contains(ascii[i]))
                            continue;

                        int optionIndex = ascii[i] - 48;
                        result[i, optionIndex] = 1;
                    }
                    break;
                case ValueDataType.NaturalNumber:
                    for (int i = 0; i < ascii.Length; i++)
                    {
                        if (escapeAscii.Contains(ascii[i]))
                            continue;

                        int optionIndex = ascii[i] - 49;
                        result[i, optionIndex] = 1;
                    }
                    break;
                case ValueDataType.Integer:
                    break;
                default:
                    break;
            }

            return result;
        }
        #endregion
    }
}
