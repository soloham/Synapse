using Synapse.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Synapse.Utilities
{
    internal class LSTM
    {
        #region Properties
        #region Paths
        public static string RootDataPath { get { return rootDataPath; } set { } }
        private static string rootDataPath = Path.Combine(Application.UserAppDataPath, Application.ProductName);
        #endregion
        #region Extensions
        public static string TemplateDataExt { get { return templateDataExt; } set { } }
        private static string templateDataExt = "tmd";

        #endregion
        #region Files & Directories Name
        public static string TemplateDataFileName { get { return templateDataFileName; } set { } }
        private static string templateDataFileName = $"Data.{TemplateDataExt}";
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
            High,
            Moderate,
            Low
        }
        #endregion

        #region Objects
        static BinaryFormatter bf = new BinaryFormatter();
        #endregion

        #region General Methods
        public static void Initialize()
        {
            Template.OnSaveTemplateEvent += SaveTemplate;
        }
        #endregion

        #region Helper Methods
        public static string GetTemplateDataPath(string templateLocation)
        {
            return $"{templateLocation}/{TemplateDataDirName}";
        }
        #endregion
        
        #region WTM Methods
        public static bool SaveTemplate(Template.Data templateData)
        {
            bool isSaved = true;

            try
            {
                if (templateData.TemplateLocation == "")
                {
                    templateData.TemplateLocation = Path.Combine(RootDataPath, templateData.TemplateName);
                    templateData.TemplateDataDirectory = $"{templateData.TemplateLocation}/{TemplateDataDirName}";
                    Directory.CreateDirectory(templateData.TemplateDataDirectory);
                }

                using (FileStream fs = new FileStream($"{templateData.TemplateDataDirectory}/{templateDataFileName}", FileMode.Create))
                    bf.Serialize(fs, templateData);
            }
            catch(Exception ex)
            {
                if (LogLevelState >= LogLevel.Low)
                    Messages.SaveFileException(ex);

                isSaved = false;
            }

            return isSaved;
        }
        #endregion

        #region RTM Methods
        public static Template LoadTemplate(string templateName)
        {
            Template result = null;

            try
            {
                string tmdPath = $"{RootDataPath}/{templateName}/{templateDataDirName}/{templateDataFileName}";
                if (!File.Exists(tmdPath))
                    return null;

                using (FileStream fs = new FileStream(tmdPath, FileMode.Open))
                {
                    Template.Data templateData = (Template.Data)bf.Deserialize(fs);
                    result = new Template(templateData);
                }
            }
            catch(Exception ex)
            {
                if (logLevelState >= LogLevel.Low)
                    Messages.LoadFileException(ex);

                return null;
            }

            return result;
        }
        #endregion
    }
}
