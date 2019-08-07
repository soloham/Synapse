using Synapse.Controls;
using Synapse.Core.Templates;
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
        public static string TemplateDataExt { get { return templateDataExt; } set { } }
        private static string templateDataExt = "tmd";
        public static string TemplateListItemsDataExt { get { return templateListItemsDataExt; } set { } }
        private static string templateListItemsDataExt = "tlid";
        #endregion
        #region Files & Directories Name
        public static string TemplateDataFileName { get { return templateDataFileName; } set { } }
        private static string templateDataFileName = $"Data.{TemplateDataExt}";
        public static string TemplateListItemsDataFileName { get { return templateListItemsDataFileName; } set { } }
        private static string templateListItemsDataFileName = $"Templates.{templateListItemsDataExt}";
        public static string TemplateDataDirName { get { return templateDataDirName; } set { } }
        private static string templateDataDirName = "Template Data";
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
        #endregion

        #region General Methods
        public static void Initialize()
        {
            Template.OnSaveTemplateEvent += SaveTemplate;
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
        #endregion

        #region RTM Methods
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
        #endregion
    }
}
