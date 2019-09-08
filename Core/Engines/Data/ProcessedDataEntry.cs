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
        public ProcessedDataResultType DataEntryResultType { get; set; }

        public char[] GetRawDataValues { get => rawDataValues; }
        private char[] rawDataValues;
        public string[] GetDataValues { get => dataValues; }
        private string[] dataValues;

        public bool IsEdited { get; set; }
        #endregion

        #region Methods
        public ProcessedDataEntry(ConfigurationBase configurationBase, char[] rawDataValues, ProcessedDataResultType processedDataResultType)
        {
            this.configurationBase = configurationBase;
            this.rawDataValues = rawDataValues;

            FormatData();
            DataEntryResultType = processedDataResultType;
        }

        public string[] FormatData()
        {
            List<string> result = new List<string>();

            switch (configurationBase.ValueRepresentation)
            {
                case ValueRepresentation.Collective:
                    result.Add(string.Concat(rawDataValues));
                    break;
                case ValueRepresentation.Indiviual:
                    for (int i = 0; i < rawDataValues.Length; i++)
                    {
                        result.Add(rawDataValues[i] + "");
                    }
                    break;
                case ValueRepresentation.CombineTwo:
                    if (rawDataValues.Length % 2 == 0)
                    {
                        for (int i = 0; i < rawDataValues.Length; i += 2)
                        {
                            result.Add(string.Concat(rawDataValues[i] + rawDataValues[i + 1]));
                        }
                    }
                    else
                    {
                        for (int i = 0; i < rawDataValues.Length; i++)
                        {
                            result.Add(rawDataValues[i] + "");
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
