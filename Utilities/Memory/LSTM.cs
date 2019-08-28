using Synapse.Controls;
using Synapse.Core.Configurations;
using Synapse.Core.Templates;
using Synapse.Utilities.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Synapse.Utilities.Memory
{
    internal class LSTM
    {
        #region Properties
        #region Paths
        public static string RootDataPath { get { return rootDataPath; } set { } }
        private static string rootDataPath = Path.Combine(Application.UserAppDataPath, Application.ProductName);
        public static string TemplatesRootDataPath { get { return templatesRootDataPath; } set { } }
        private static string templatesRootDataPath = Path.Combine(RootDataPath, "Templates");
        public static string AppRootDataPath { get { return appRootDataPath; } set { } }
        private static string appRootDataPath = Path.Combine(RootDataPath, "App Data");
        #endregion
        #region Extensions
        public static string TemplateImageExt { get { return templateImageExt; } set { } }
        private static string templateImageExt = "jpg";
        public static string TemplateDataExt { get { return templateDataExt; } set { } }
        private static string templateDataExt = "tmd";
        public static string TemplateListItemsDataExt { get { return templateListItemsDataExt; } set { } }
        private static string templateListItemsDataExt = "tlid";

        public static string ConfigDataFileExt { get { return configDataFileExt; } set { } }
        private static string configDataFileExt = "dat";
        #endregion
        #region Files & Directories Name
        public static string TemplateDataFileName { get { return templateDataFileName; } set { } }
        private static string templateDataFileName = $"Data.{TemplateDataExt}";
        public static string TemplateImageFileName { get { return templateImageFileName; } set { } }
        private static string templateImageFileName = $"Template Image.{TemplateImageExt}";
        public static string TemplateListItemsDataFileName { get { return templateListItemsDataFileName; } set { } }
        private static string templateListItemsDataFileName = $"Templates.{templateListItemsDataExt}";
        public static string ConfigDataFileName { get { return configDataFileName; } set { } }
        private static string configDataFileName = $"Configuration.{ConfigDataFileExt}";
        public static string TemplateDataDirName { get { return templateDataDirName; } set { } }
        private static string templateDataDirName = "Template Data";


        public static string OMRConfigRootDirName { get { return oMRConfigRootDirName; } set { } }
        private static string oMRConfigRootDirName = "OMR";
        public static string OBRConfigRootDirName { get { return oBRConfigRootDirName; } set { } }
        private static string oBRConfigRootDirName = "OBR";
        public static string ICRConfigRootDirName { get { return iCRConfigRootDirName; } set { } }
        private static string iCRConfigRootDirName = "ICR";

        public static string ConfigDataDirName { get { return configDataDirName; } set { } }
        private static string configDataDirName = "Configuration Data";

        #endregion
        #region General
        public static LogLevel LogLevelState { get { return logLevelState; } set { logLevelState = value; } }
        private static LogLevel logLevelState;
        #endregion
        #endregion

        #region Enums
        public enum LogLevel
        {
            Low,
            Moderate,
            High
        }
        #endregion

        #region Objects
        static BinaryFormatter bf = new BinaryFormatter();
        #endregion

        #region Helper Methods
        public static string GetTemplateDataPath(string templateLocation)
        {
            return $"{templateLocation}/{TemplateDataDirName}";
        }
        public static string GetCurrentTemplateRootPath()
        {
            string templateName = SynapseMain.GetCurrentTemplate.GetTemplateName;
            return Path.Combine(TemplatesRootDataPath, templateName);
        }
        public static string GetConfigRootPath(MainConfigType mainConfigType)
        {
            string result = "";

            string templateRootPath = GetCurrentTemplateRootPath();
            switch (mainConfigType)
            {
                case MainConfigType.OMR:
                    result = $"{templateRootPath}\\{OMRConfigRootDirName}";
                    break;
                case MainConfigType.BARCODE:
                    result = $"{templateRootPath}\\{OBRConfigRootDirName}";
                    break;
                case MainConfigType.ICR:
                    result = $"{templateRootPath}\\{ICRConfigRootDirName}";
                    break;
             }

            return result;
        }
        public static string GetConfigDataPath(string configTitle, MainConfigType mainConfigType)
        {
            string result = "";

            string configRootPath = GetConfigRootPath(mainConfigType);
            result = $"{configRootPath}\\{configTitle}\\{ConfigDataDirName}";

            return result;
        }
        public static string GetConfigDataFilePath(string configTitle, MainConfigType mainConfigType)
        {
            string result = "";

            string configDataPath = GetConfigDataPath(configTitle, mainConfigType);
            result = $"{configDataPath}\\{ConfigDataFileName}";

            return result;
        }

        #endregion

        #region General Methods
        public static void Initialize()
        {
            Template.OnSaveTemplateEvent += SaveTemplate;
            Template.OnSaveConfiguredTemplateEvent += SaveConfiguredTemplate;
        }

        public static bool DeleteTemplate(string templateName)
        {
            bool result = true;

            try
            {
                string tmdPath = $"{TemplatesRootDataPath}/{templateName}/{templateDataDirName}/{templateDataFileName}";
                if (!File.Exists(tmdPath))
                    return false;

                Directory.Delete($"{TemplatesRootDataPath}/{templateName}", true);

            }
            catch (Exception ex)
            {
                if (logLevelState >= LogLevel.Low)
                    Messages.DeleteDirectoryException(ex);

                return false;
            }

            return result;
        }
        #endregion

        #region WTM Methods
        public static bool SaveTemplate(Template.Data templateData)
        {
            bool isSaved = true;

            try
            {
                if (templateData.TemplateLocation == "" || !templateData.TemplateLocation.Contains(TemplatesRootDataPath))
                {
                    templateData.TemplateLocation = Path.Combine(TemplatesRootDataPath, templateData.TemplateName);
                    templateData.TemplateDataDirectory = $"{templateData.TemplateLocation}/{TemplateDataDirName}";

                    Directory.CreateDirectory(templateData.TemplateDataDirectory);
                }

                using (FileStream fs = new FileStream($"{templateData.TemplateDataDirectory}/{templateDataFileName}", FileMode.Create))
                    bf.Serialize(fs, templateData);
            }
            catch (Exception ex)
            {
                if (LogLevelState >= LogLevel.Low)
                    Messages.SaveFileException(ex);

                isSaved = false;
            }

            return isSaved;
        }
        private static bool SaveConfiguredTemplate(Template.Data templateData, System.Drawing.Bitmap templateImage)
        {
            bool isSaved = true;

            string imageSaveLoc = "";
            try
            {
                if (templateData.TemplateLocation == "" || !templateData.TemplateLocation.Contains(TemplatesRootDataPath))
                {
                    templateData.TemplateLocation = Path.Combine(TemplatesRootDataPath, templateData.TemplateName);
                    templateData.TemplateDataDirectory = $"{templateData.TemplateLocation}/{TemplateDataDirName}";

                    Directory.CreateDirectory(templateData.TemplateDataDirectory);
                }

                imageSaveLoc = $"{templateData.TemplateDataDirectory}/{templateImageFileName}";

                //// Grab the binary data.
                //byte[] data = File.ReadAllBytes(imageSaveLoc);

                //// Read in the data but do not close, before using the stream.
                //Stream originalBinaryDataStream = new MemoryStream(data);
                //System.Drawing.Bitmap image = new System.Drawing.Bitmap(originalBinaryDataStream);
                //image.Save(imageSaveLoc);
                //originalBinaryDataStream.Dispose();
                templateImage.Save(imageSaveLoc);

                templateData.GetTemplateImage.ImageLocation = imageSaveLoc;

                using (FileStream fs = new FileStream($"{templateData.TemplateDataDirectory}/{templateDataFileName}", FileMode.Create))
                    bf.Serialize(fs, templateData);
            }
            catch (Exception ex)
            {
                if (LogLevelState >= LogLevel.Low)
                    Messages.SaveFileException(ex);

                isSaved = false;
            }

            return isSaved;
        }
        public static bool SaveTemplateListItems(List<TemplateListItem.ObjectData> templateListItems)
        {
            bool isSaved = true;

            try
            {
                Directory.CreateDirectory(AppRootDataPath);
                using (FileStream fs = new FileStream($"{AppRootDataPath}/{TemplateListItemsDataFileName}", FileMode.Create))
                    bf.Serialize(fs, templateListItems);
            }
            catch (Exception ex)
            {
                if (LogLevelState >= LogLevel.Low)
                    Messages.SaveFileException(ex);

                isSaved = false;
            }

            return isSaved;
        }
        public static Template ImportTemplate(string templatePath)
        {
            Template result = null;

            try
            {
                string tmdPath = $"{templatePath}/{templateDataDirName}/{templateDataFileName}";
                if (!File.Exists(tmdPath))
                    return null;

                using (FileStream fs = new FileStream(tmdPath, FileMode.Open))
                {
                    Template.Data templateData = (Template.Data)bf.Deserialize(fs);
                    templateData.TemplateLocation = "";
                    templateData.TemplateDataDirectory = "";
                    result = new Template(templateData);
                }

            }
            catch (Exception ex)
            {
                if (logLevelState >= LogLevel.Moderate)
                    Messages.LoadFileException(ex);

                return null;
            }

            return result;
        }

        public static bool SaveConfigData(ConfigurationBase config, MainConfigType mainConfigType, out Exception _ex)
        {
            bool result = true;
            _ex = new Exception();

            string configDataPath = GetConfigDataPath(config.Title, mainConfigType);
            if (!Directory.Exists(configDataPath))
                Directory.CreateDirectory(configDataPath);

            string configDataFilePath = GetConfigDataFilePath(config.Title, mainConfigType);
            switch (mainConfigType)
            {
                case MainConfigType.OMR:
                    OMRConfiguration oMRConfiguration = (OMRConfiguration)config;

                    try
                    {
                        BinaryFormatter bf = new BinaryFormatter();
                        using (FileStream fs = new FileStream(configDataFilePath, FileMode.Create))
                        {
                            bf.Serialize(fs, oMRConfiguration);
                        }
                    }
                    catch(Exception ex)
                    {
                        _ex = ex;
                        result = false;
                    }
                    break;
                case MainConfigType.BARCODE:


                    try
                    {
                        
                    }
                    catch (Exception ex)
                    {
                        _ex = ex;
                        result = false;
                    }
                    break;
                case MainConfigType.ICR:


                    try
                    {
                        
                    }
                    catch (Exception ex)
                    {
                        _ex = ex;
                        result = false;
                    }
                    break;
            }

            return result;
        }
        public static bool DeleteConfigData(ConfigurationBase config, MainConfigType mainConfigType, out Exception _ex)
        {
            bool result = true;
            _ex = new Exception();

            string configRootPath = GetConfigRootPath(mainConfigType);
            string configPath = $"{configRootPath}\\{config.Title}";

            try
            { 
                Directory.Delete(configPath, true);
            }
            catch (Exception ex)
            {
                _ex = ex;
                result = false;
            }

            return result;
        }

        #endregion

        #region RFM Methods
        public static async Task<List<TemplateListItem.ObjectData>> LoadTemplateListItemsAsync()
        {
            List<TemplateListItem.ObjectData> templateListItems = new List<TemplateListItem.ObjectData>();

            await Task.Run(() =>
            {
                try
                {
                    using (FileStream fs = new FileStream($"{AppRootDataPath}/{TemplateListItemsDataFileName}", FileMode.Open))
                        templateListItems = (List<TemplateListItem.ObjectData>)bf.Deserialize(fs);
                }
                catch (Exception ex)
                {
                    if (logLevelState >= LogLevel.Low)
                        Messages.LoadFileException(ex);
                }
            });

            return templateListItems;
        }
        public static Template LoadTemplate(string templateName)
        {
            Template result = null;

            try
            {
                string tmdPath = $"{TemplatesRootDataPath}/{templateName}/{templateDataDirName}/{templateDataFileName}";
                if (!File.Exists(tmdPath))
                    return null;

                using (FileStream fs = new FileStream(tmdPath, FileMode.Open))
                {
                    Template.Data templateData = (Template.Data)bf.Deserialize(fs);
                    result = new Template(templateData);
                }
            }
            catch (Exception ex)
            {
                if (logLevelState >= LogLevel.Moderate)
                    Messages.LoadFileException(ex);

                return null;
            }

            return result;
        }

        public static async Task<List<ConfigurationBase>> LoadAllConfigurations()
        {
            List<ConfigurationBase> configurationBases = new List<ConfigurationBase>();

            var mainConfigTypes = EnumHelper.ToList(typeof(MainConfigType));
            for (int i = 0; i < mainConfigTypes.Count; i++)
            {
                configurationBases.AddRange(await LoadConfiguration((MainConfigType)i));
            }

            return configurationBases;
        }
        public static async Task<List<ConfigurationBase>> LoadConfiguration(MainConfigType mainConfigType)
        {
            List<ConfigurationBase> configurationBases = new List<ConfigurationBase>();

            string omrConfigPath = GetConfigRootPath(mainConfigType);
            if (!Directory.Exists(omrConfigPath))
                return configurationBases;

            string[] configsPaths = Directory.GetDirectories(omrConfigPath);

            await Task.Run(() =>
            {
                for (int i = 0; i < configsPaths.Length; i++)
                {
                    string configDataFilePath = Path.Combine(configsPaths[i], ConfigDataDirName, configDataFileName);

                    BinaryFormatter bf = new BinaryFormatter();
                    using (FileStream fs = new FileStream(configDataFilePath, FileMode.Open))
                    {
                        ConfigurationBase configurationBase = (OMRConfiguration)bf.Deserialize(fs);
                        configurationBases.Add(configurationBase);
                    }
                }
            });

            return configurationBases;
        }
        #endregion
    }
}
