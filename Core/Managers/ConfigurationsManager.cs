using Synapse.Core.Configurations;
using Synapse.Core.Templates;
using Synapse.Utilities.Memory;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Synapse.Core.Managers
{
    internal class ConfigurationsManager
    {
        #region Properties
        public static List<ConfigurationBase> GetAllConfigurations { get => allConfigurations; set { } }
        private static List<ConfigurationBase> allConfigurations = new List<ConfigurationBase>();

        public static List<OMRConfiguration> GetOMRConfigurations { get => omrConfigurations; set { } }
        private static List<OMRConfiguration> omrConfigurations = new List<OMRConfiguration>();
        #endregion

        #region Events

        public static event EventHandler<ConfigurationBase> OnConfigurationDeletedEvent;

        #endregion

        #region Static Methods
        public static async Task Initialize()
        {
            allConfigurations = await LSTM.LoadAllConfigurations();

            var clonedConfigs = new List<ConfigurationBase>(allConfigurations);
            for (int i = 0; i < clonedConfigs.Count; i++)
            {
                ConfigurationBase configuration = clonedConfigs[i];
                allConfigurations.Remove(configuration);
                allConfigurations.Insert(configuration.ProcessingIndex, configuration);
            }
        }
        public static void AddConfiguration(ConfigurationBase configuration)
        {
            switch (configuration.GetMainConfigType)
            {
                case MainConfigType.OMR:
                    OMRConfiguration omrConfiguration = (OMRConfiguration)configuration;
                    omrConfigurations.Add(omrConfiguration);
                    break;
                case MainConfigType.BARCODE:
                    break;
                case MainConfigType.ICR:
                    break;
            }

            allConfigurations.Add(configuration);

            SynapseMain.GetSynapseMain.StatusCheck();
        }
        public static bool RemoveConfiguration(object sender, ConfigurationBase configuration)
        {
            bool isRemoved = false;
            switch (configuration.GetMainConfigType)
            {
                case MainConfigType.OMR:
                    OMRConfiguration omrConfiguration = (OMRConfiguration)configuration;
                    isRemoved = allConfigurations.Remove(omrConfiguration);

                    if (isRemoved)
                        OnConfigurationDeletedEvent?.Invoke(sender, omrConfiguration);
                    break;
                case MainConfigType.BARCODE:
                    break;
                case MainConfigType.ICR:
                    ICRConfiguration icrConfiguration = (ICRConfiguration)configuration;
                    isRemoved = allConfigurations.Remove(icrConfiguration);

                    if (isRemoved)
                        OnConfigurationDeletedEvent?.Invoke(sender, icrConfiguration);
                    break;
            }
            return isRemoved;
        }
        public static ConfigurationBase GetConfiguration(string configTitle)
        {
            ConfigurationBase result = null;
            result = allConfigurations.Find(x => x.Title == configTitle);

            return result;
        }
        public static bool ValidateName(string configTitle)
        {
            return allConfigurations.TrueForAll(x => x.Title != configTitle);
        }
        #endregion
    }
}