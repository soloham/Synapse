using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Synapse.Core.Configurations;

namespace Synapse.Core.Engines.Data
{
    [Serializable]
    internal class ProcessedDataEntry
    {
        #region Properties
        public ConfigurationBase GetConfigurationBase { get => configurationBase; }
        private ConfigurationBase configurationBase;
        public MainConfigType GetMainConfigType { get => GetConfigurationBase.GetMainConfigType; }
        public ProcessedDataType DataEntryResultType { get; set; }

        public byte[,] GetRawDataValues { get => rawDataValues; }
        private byte[,] rawDataValues;
        public char[] GetFieldsOutputs { get => fieldsOutputs; }
        private char[] fieldsOutputs;
        public string[] GetDataValues { get => dataValues; }
        private string[] dataValues;

        public bool IsEdited { get; set; }
        #endregion

        #region Methods
        public ProcessedDataEntry(ConfigurationBase configurationBase, byte[,] rawDataValues, char[] fieldsOutputs, ProcessedDataType processedDataResultType)
        {
            this.configurationBase = configurationBase;
            this.fieldsOutputs = fieldsOutputs;
            this.rawDataValues = rawDataValues;

            DataEntryResultType = processedDataResultType;
            FormatData();
        }

        public string[] FormatData()
        {
            List<string> result = new List<string>();

            switch (configurationBase.ValueRepresentation)
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
        #endregion
    }
}
