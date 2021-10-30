namespace Synapse.Core.Managers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Synapse.Core.Configurations;
    using Synapse.DeCore.Engines.Data;
    using Synapse.Utilities;
    using Synapse.Utilities.Memory;

    public class ConfigurationsManager
    {
        #region Properties

        public static List<ConfigurationBase> GetAllConfigurations
        {
            get => allConfigurations;
            set { }
        }

        private static List<ConfigurationBase> allConfigurations = new List<ConfigurationBase>();

        #endregion

        #region Events

        public static event EventHandler<ConfigurationBase> OnConfigurationDeletedEvent;

        #endregion

        #region Variables

        public static Action CheckAppStatus;

        #endregion

        #region Static Methods

        public static async Task Initialize(Action checkAppStatus)
        {
            allConfigurations = await LSTM.LoadAllConfigurations();

            var clonedConfigs = new List<ConfigurationBase>(allConfigurations);
            for (var i = 0; i < clonedConfigs.Count; i++)
            {
                var configuration = clonedConfigs[i];
                allConfigurations.Remove(configuration);
                if (configuration.ProcessingIndex >= allConfigurations.Count)
                {
                    configuration.ProcessingIndex = allConfigurations.Count == 0 ? 0 : allConfigurations.Count - 1;
                }

                allConfigurations.Insert(configuration.ProcessingIndex, configuration);
            }

            CheckAppStatus = checkAppStatus;

            Communicator.Initialize(GetConfiguration);
        }

        public static void AddConfiguration(ConfigurationBase configuration)
        {
            //switch (configuration.GetMainConfigType)
            //{
            //    case MainConfigType.OMR:
            //        OMRConfiguration omrConfiguration = (OMRConfiguration)configuration;
            //        omrConfigurations.Add(omrConfiguration);
            //        break;
            //    case MainConfigType.BARCODE:
            //        break;
            //    case MainConfigType.ICR:
            //        break;
            //}

            allConfigurations.Add(configuration);

            CheckAppStatus?.Invoke();
        }

        public static bool RemoveConfiguration(object sender, ConfigurationBase configuration)
        {
            var isRemoved = false;
            switch (configuration.GetMainConfigType)
            {
                case MainConfigType.OMR:
                    var omrConfiguration = (OMRConfiguration)configuration;
                    isRemoved = allConfigurations.Remove(omrConfiguration);

                    if (isRemoved)
                    {
                        OnConfigurationDeletedEvent?.Invoke(sender, omrConfiguration);
                    }

                    break;

                case MainConfigType.BARCODE:
                    var obrConfiguration = (OBRConfiguration)configuration;
                    isRemoved = allConfigurations.Remove(obrConfiguration);

                    if (isRemoved)
                    {
                        OnConfigurationDeletedEvent?.Invoke(sender, obrConfiguration);
                    }

                    break;

                case MainConfigType.ICR:
                    var icrConfiguration = (ICRConfiguration)configuration;
                    isRemoved = allConfigurations.Remove(icrConfiguration);

                    if (isRemoved)
                    {
                        OnConfigurationDeletedEvent?.Invoke(sender, icrConfiguration);
                    }

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

        public static List<ConfigurationBase> GetConfigurations(MainConfigType mainConfigType,
            Func<ConfigurationBase, bool> selector)
        {
            if (allConfigurations == null || allConfigurations.Count == 0)
            {
                return null;
            }

            return allConfigurations.FindAll(x => x.GetMainConfigType == mainConfigType && selector(x));
        }

        public static bool ValidateName(string configTitle)
        {
            return allConfigurations.TrueForAll(x => x.Title != configTitle);
        }

        public static async Task<bool[]> SaveAllConfigurations()
        {
            var isSaved = new bool[allConfigurations.Count];
            var allConfigs = GetAllConfigurations;
            for (var i = 0; i < allConfigs.Count; i++)
            {
                Exception ex = null;
                isSaved[i] = await Task.Run(() => ConfigurationBase.Save(allConfigs[i], out ex));

                if (!isSaved[i])
                {
                    Messages.SaveFileException(ex);
                }
            }

            return isSaved;
        }

        #endregion
    }
}