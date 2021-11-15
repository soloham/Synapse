namespace Synapse.Utilities.Memory
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Threading.Tasks;
    using System.Windows.Forms;

    using Synapse.Controls;
    using Synapse.Core;
    using Synapse.Core.Configurations;
    using Synapse.Core.Templates;
    using Synapse.Utilities.Enums;

    public class LSTM
    {
        #region Properties

        #region Paths

        public static string RootDataPath
        {
            get => rootDataPath;
            set { }
        }

        private static readonly string
            rootDataPath = Path.Combine(Application.UserAppDataPath, Application.ProductName);

        public static string TemplatesRootDataPath
        {
            get => templatesRootDataPath;
            set { }
        }

        private static readonly string templatesRootDataPath = Path.Combine(RootDataPath, "Templates");

        public static string AppRootDataPath
        {
            get => appRootDataPath;
            set { }
        }

        private static readonly string appRootDataPath = Path.Combine(RootDataPath, "App Data");

        #endregion

        #region Extensions

        public static string TemplateImageExt
        {
            get => templateImageExt;
            set { }
        }

        private static readonly string templateImageExt = "jpg";

        public static string TemplateDataExt
        {
            get => templateDataExt;
            set { }
        }

        private static readonly string templateDataExt = "tmd";

        public static string TemplateListItemsDataExt
        {
            get => templateListItemsDataExt;
            set { }
        }

        private static readonly string templateListItemsDataExt = "tlid";

        public static string ConfigDataFileExt
        {
            get => configDataFileExt;
            set { }
        }

        private static readonly string configDataFileExt = "dat";

        public static string PapersDataFileExt
        {
            get => papersDataFileExt;
            set { }
        }

        private static readonly string papersDataFileExt = "exp";

        #endregion

        #region Files & Directories Name

        private static readonly string licenseFileName = "License.lc";

        public static string LicenseListItemsDataFileName
        {
            get => licenseFileName;
            set { }
        }

        public static string TemplateDataFileName
        {
            get => templateDataFileName;
            set { }
        }

        private static readonly string templateDataFileName = $"Data.{TemplateDataExt}";

        public static string TemplateImageFileName
        {
            get => templateImageFileName;
            set { }
        }

        private static readonly string templateImageFileName = $"Template Image.{TemplateImageExt}";

        public static string TemplateListItemsDataFileName
        {
            get => templateListItemsDataFileName;
            set { }
        }

        private static readonly string templateListItemsDataFileName = $"Templates.{templateListItemsDataExt}";

        public static string ConfigDataFileName
        {
            get => configDataFileName;
            set { }
        }

        private static readonly string configDataFileName = $"Configuration.{ConfigDataFileExt}";

        public static string PapersDataFileName
        {
            get => papersDataFileName;
            set { }
        }

        private static readonly string papersDataFileName = $"Exam Papers.{PapersDataFileExt}";

        public static string TemplateDataDirName
        {
            get => templateDataDirName;
            set { }
        }

        private static readonly string templateDataDirName = "Template Data";


        public static string OMRConfigRootDirName
        {
            get => oMRConfigRootDirName;
            set { }
        }

        private static readonly string oMRConfigRootDirName = "OMR";

        public static string OBRConfigRootDirName
        {
            get => oBRConfigRootDirName;
            set { }
        }

        private static readonly string oBRConfigRootDirName = "OBR";

        public static string ICRConfigRootDirName
        {
            get => iCRConfigRootDirName;
            set { }
        }

        private static readonly string iCRConfigRootDirName = "ICR";

        public static string ConfigDataDirName
        {
            get => configDataDirName;
            set { }
        }

        private static readonly string configDataDirName = "Configuration Data";

        #endregion

        #region General

        public static LogLevel LogLevelState { get; set; }

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

        #region

        public static Func<Template> GetCurrentTemplate;

        #endregion

        #region Objects

        private static readonly BinaryFormatter bf = new BinaryFormatter();

        #endregion

        #region Helper Methods

        public static string GetTemplateDataPath(string templateLocation)
        {
            return $"{templateLocation}/{TemplateDataDirName}";
        }

        public static string GetCurrentTemplateRootPath()
        {
            var templateName = GetCurrentTemplate?.Invoke().GetTemplateName;
            return Path.Combine(TemplatesRootDataPath, templateName);
        }

        public static string GetTemplateImagePath(string templateName)
        {
            var templateDataDir = Path.Combine(TemplatesRootDataPath, templateName);
            return Path.Combine(templateDataDir, TemplateDataDirName, TemplateImageFileName);
        }

        public static string GetConfigRootPath(MainConfigType mainConfigType)
        {
            var result = "";

            var templateRootPath = GetCurrentTemplateRootPath();
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
            var result = "";

            var configRootPath = GetConfigRootPath(mainConfigType);
            result = $"{configRootPath}\\{configTitle}\\{ConfigDataDirName}";

            return result;
        }

        public static string GetConfigDataFilePath(string configTitle, MainConfigType mainConfigType)
        {
            var result = "";

            var configDataPath = GetConfigDataPath(configTitle, mainConfigType);
            result = $"{configDataPath}\\{ConfigDataFileName}";

            return result;
        }

        #endregion

        #region General Methods

        public static void Initialize()
        {
            Template.OnSaveTemplateEvent += SaveTemplate;
            Template.OnSaveConfiguredTemplateEvent += SaveConfiguredTemplate;

            ConfigurationBase.DeleteConfigData = DeleteConfigData;
            ConfigurationBase.SaveConfigData = SaveConfigData;
        }

        public static bool DeleteTemplate(string templateName)
        {
            var result = true;

            try
            {
                var tmdPath = $"{TemplatesRootDataPath}/{templateName}/{templateDataDirName}/{templateDataFileName}";
                if (!File.Exists(tmdPath))
                {
                    return false;
                }

                Directory.Delete($"{TemplatesRootDataPath}/{templateName}", true);
            }
            catch (Exception ex)
            {
                if (LogLevelState >= LogLevel.Low)
                {
                    Messages.DeleteDirectoryException(ex);
                }

                return false;
            }

            return result;
        }

        #endregion

        #region WTM Methods

        public static bool SaveTemplate(Template.Data templateData)
        {
            var isSaved = true;

            try
            {
                if (templateData.TemplateLocation == "" ||
                    !templateData.TemplateLocation.Contains(TemplatesRootDataPath))
                {
                    templateData.TemplateLocation = Path.Combine(TemplatesRootDataPath, templateData.TemplateName);
                    templateData.TemplateDataDirectory = $"{templateData.TemplateLocation}/{TemplateDataDirName}";

                    Directory.CreateDirectory(templateData.TemplateDataDirectory);
                }

                using (var fs = new FileStream($"{templateData.TemplateDataDirectory}/{templateDataFileName}",
                    FileMode.Create))
                {
                    bf.Serialize(fs, templateData);
                }
            }
            catch (Exception ex)
            {
                if (LogLevelState >= LogLevel.Low)
                {
                    Messages.SaveFileException(ex);
                }

                isSaved = false;
            }

            return isSaved;
        }

        private static bool SaveConfiguredTemplate(Template.Data templateData, Bitmap templateImage)
        {
            var isSaved = true;

            var imageSaveLoc = "";
            try
            {
                if (templateData.TemplateLocation == "" ||
                    !templateData.TemplateLocation.Contains(TemplatesRootDataPath))
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

                using (var fs = new FileStream($"{templateData.TemplateDataDirectory}/{templateDataFileName}",
                    FileMode.Create))
                {
                    bf.Serialize(fs, templateData);
                }
            }
            catch (Exception ex)
            {
                if (LogLevelState >= LogLevel.Low)
                {
                    Messages.SaveFileException(ex);
                }

                isSaved = false;
            }

            return isSaved;
        }

        public static bool SaveTemplateListItems(List<TemplateListItem.ObjectData> templateListItems)
        {
            var isSaved = true;

            try
            {
                Directory.CreateDirectory(AppRootDataPath);
                using (var fs = new FileStream($"{AppRootDataPath}/{TemplateListItemsDataFileName}", FileMode.Create))
                {
                    bf.Serialize(fs, templateListItems);
                }
            }
            catch (Exception ex)
            {
                if (LogLevelState >= LogLevel.Low)
                {
                    Messages.SaveFileException(ex);
                }

                isSaved = false;
            }

            return isSaved;
        }

        public static Template ImportTemplate(string templatePath)
        {
            Template result = null;

            try
            {
                var tmdPath = $"{templatePath}/{templateDataDirName}/{templateDataFileName}";
                if (!File.Exists(tmdPath))
                {
                    return null;
                }

                using (var fs = new FileStream(tmdPath, FileMode.Open))
                {
                    var templateData = (Template.Data)bf.Deserialize(fs);
                    templateData.TemplateLocation = "";
                    templateData.TemplateDataDirectory = "";
                    result = new Template(templateData);
                }
            }
            catch (Exception ex)
            {
                if (LogLevelState >= LogLevel.Moderate)
                {
                    Messages.LoadFileException(ex);
                }

                return null;
            }

            return result;
        }

        public static bool SaveConfigData(ConfigurationBase config, MainConfigType mainConfigType, out Exception _ex)
        {
            var result = true;
            _ex = new Exception();

            var configDataPath = GetConfigDataPath(config.Title, mainConfigType);
            if (!Directory.Exists(configDataPath))
            {
                Directory.CreateDirectory(configDataPath);
            }

            var configDataFilePath = GetConfigDataFilePath(config.Title, mainConfigType);
            switch (mainConfigType)
            {
                case MainConfigType.OMR:
                    var omrConfiguration = (OMRConfiguration)config;

                    try
                    {
                        var bf = new BinaryFormatter();
                        using (var fs = new FileStream(configDataFilePath, FileMode.Create))
                        {
                            bf.Serialize(fs, omrConfiguration);
                        }
                    }
                    catch (Exception ex)
                    {
                        _ex = ex;
                        result = false;
                    }

                    break;

                case MainConfigType.BARCODE:
                    var obrConfiguration = (OBRConfiguration)config;

                    try
                    {
                        var bf = new BinaryFormatter();
                        using (var fs = new FileStream(configDataFilePath, FileMode.Create))
                        {
                            bf.Serialize(fs, obrConfiguration);
                        }
                    }
                    catch (Exception ex)
                    {
                        _ex = ex;
                        result = false;
                    }

                    break;

                case MainConfigType.ICR:
                    var icrConfiguration = (ICRConfiguration)config;

                    try
                    {
                        var bf = new BinaryFormatter();
                        using (var fs = new FileStream(configDataFilePath, FileMode.Create))
                        {
                            bf.Serialize(fs, icrConfiguration);
                        }
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
            var result = true;
            _ex = new Exception();

            var configRootPath = GetConfigRootPath(mainConfigType);
            var configPath = $"{configRootPath}\\{config.Title}";

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
            var templateListItems = new List<TemplateListItem.ObjectData>();

            await Task.Run(() =>
            {
                try
                {
                    using (var fs = new FileStream($"{AppRootDataPath}/{TemplateListItemsDataFileName}", FileMode.Open))
                    {
                        templateListItems = (List<TemplateListItem.ObjectData>)bf.Deserialize(fs);
                    }
                }
                catch (Exception ex)
                {
                    if (LogLevelState >= LogLevel.Low)
                    {
                        Messages.LoadFileException(ex);
                    }
                }
            });

            return templateListItems;
        }

        public static string LoadLicenseKey()
        {
            string result = null;

            try
            {
                var tmdPath = $"{AppRootDataPath}/{licenseFileName}";
                if (!File.Exists(tmdPath))
                {
                    return null;
                }

                using (var fs = new FileStream(tmdPath, FileMode.Open))
                {
                    var license = (string)bf.Deserialize(fs);
                    result = license;
                }
            }
            catch (Exception ex)
            {
                if (LogLevelState >= LogLevel.Moderate)
                {
                    Messages.LoadFileException(ex);
                }

                return null;
            }

            return result;
        }

        public static Template LoadTemplate(string templateName)
        {
            Template result = null;

            try
            {
                var tmdPath = $"{TemplatesRootDataPath}/{templateName}/{templateDataDirName}/{templateDataFileName}";
                if (!File.Exists(tmdPath))
                {
                    return null;
                }

                using (var fs = new FileStream(tmdPath, FileMode.Open))
                {
                    var templateData = (Template.Data)bf.Deserialize(fs);
                    result = new Template(templateData);
                }
            }
            catch (Exception ex)
            {
                if (LogLevelState >= LogLevel.Moderate)
                {
                    Messages.LoadFileException(ex);
                }

                return null;
            }

            return result;
        }

        public static async Task<List<ConfigurationBase>> LoadAllConfigurations()
        {
            var configurationBases = new List<ConfigurationBase>();

            var mainConfigTypes = EnumHelper.ToList(typeof(MainConfigType));
            for (var i = 0; i < mainConfigTypes.Count; i++)
                configurationBases.AddRange(await LoadConfiguration((MainConfigType)i));

            return configurationBases;
        }

        public static async Task<List<ConfigurationBase>> LoadConfiguration(MainConfigType mainConfigType)
        {
            var configurationBases = new List<ConfigurationBase>();

            var omrConfigPath = GetConfigRootPath(mainConfigType);
            if (!Directory.Exists(omrConfigPath))
            {
                return configurationBases;
            }

            var configsPaths = Directory.GetDirectories(omrConfigPath);

            await Task.Run(() =>
            {
                for (var i = 0; i < configsPaths.Length; i++)
                {
                    var configDataFilePath = Path.Combine(configsPaths[i], ConfigDataDirName, configDataFileName);

                    var bf = new BinaryFormatter();
                    using (var fs = new FileStream(configDataFilePath, FileMode.Open))
                    {
                        var configurationBase = (ConfigurationBase)bf.Deserialize(fs);
                        configurationBases.Add(configurationBase);
                    }
                }
            });

            return configurationBases;
        }

        public static async Task<ExamPapers> LoadPapers()
        {
            ExamPapers examPapers = null;

            var paperDataFilePath = Path.Combine(AppRootDataPath, PapersDataFileName);
            if (!File.Exists(paperDataFilePath))
            {
                return examPapers;
            }

            await Task.Run(() =>
            {
                var bf = new BinaryFormatter();
                using (var fs = new FileStream(paperDataFilePath, FileMode.Open))
                {
                    examPapers = (ExamPapers)bf.Deserialize(fs);
                }
            });

            return examPapers;
        }

        public static async Task<bool> SavePapers(ExamPapers examPapers)
        {
            var paperDataFilePath = Path.Combine(AppRootDataPath, PapersDataFileName);

            try
            {
                await Task.Run(() =>
                {
                    var bf = new BinaryFormatter();
                    using (var fs = new FileStream(paperDataFilePath, FileMode.Create))
                    {
                        bf.Serialize(fs, examPapers);
                    }
                });
                return true;
            }
            catch
            {
                return false;
            }
        }


        public static bool SaveLicenseKey(string licenseKey)
        {
            var licenseKeyFilePath = Path.Combine(AppRootDataPath, licenseFileName);

            try
            {
                var bf = new BinaryFormatter();
                using (var fs = new FileStream(licenseKeyFilePath, FileMode.Create))
                {
                    bf.Serialize(fs, licenseKey);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion
    }
}