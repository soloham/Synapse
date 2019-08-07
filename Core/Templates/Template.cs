using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Synapse.Core.Templates
{
    internal class Template
    {
        #region Objects
        [Serializable]
        internal class Data
        {
            internal string TemplateName;
            internal string TemplateLocation;
            internal string TemplateDataDirectory;
            internal Size TemplateSize;

            internal Data(string templateName, string templateLocation, Size templateSize)
            {
                TemplateName = templateName;
                TemplateLocation = templateLocation;
                TemplateDataDirectory = Utilities.Memory.LSTM.GetTemplateDataPath(TemplateLocation);
                TemplateSize = templateSize;
            }
        }
        #endregion

        #region Properties
        public string GetTemplateName { get { return TemplateData.TemplateName; } set { } }
        public string GetTemplateLocation { get { return TemplateData.TemplateLocation; } set { } }
        public Size GetTemplateSize { get { return TemplateData.TemplateSize; } set { } }
        #endregion

        #region Events
        public delegate bool OnTemplateDataCallback(Data templateData);
        public delegate Template OnTemplateNameCallback(string templateName);
        public static event OnTemplateDataCallback OnSaveTemplateEvent;
        #endregion

        #region Variables
        public Data TemplateData;
        #endregion

        #region Methods
        public Template(Data tmpData)
        {
            TemplateData = tmpData;
        }
        public static Template CreateTemplate(string tmpName)
        {
            Template newTemplate = new Template(new Data(tmpName, "", Size.Empty));
            SaveTemplate(newTemplate.TemplateData);
            return newTemplate;
        }
        internal static async Task<bool> ChangeTemplateName(string oldName, string newName)
        {
            bool result = false;

            Template template = await LoadTemplate(oldName);
            template.TemplateData.TemplateName = newName;

            string newNameLocation = template.TemplateData.TemplateLocation.Replace(oldName, newName);
            Directory.Move(template.TemplateData.TemplateLocation, newNameLocation);
            template.TemplateData.TemplateLocation = newNameLocation;
            template.TemplateData.TemplateDataDirectory = Utilities.Memory.LSTM.GetTemplateDataPath(newNameLocation);

            result = SaveTemplate(template.TemplateData);
            return result;
        }
        internal static bool SaveTemplate(Data tempData)
        {
            bool isSaved = true;

            try
            {
                OnSaveTemplateEvent?.Invoke(tempData);
            }
            catch (Exception ex)
            {
                isSaved = false;
            }
            return isSaved;
        }
        internal static async Task<Template> LoadTemplate(string templateName)
        {
            return await Task.Run(() => Utilities.Memory.LSTM.LoadTemplate(templateName));
        }
        internal static async Task<Template> ImportTemplate(string location)
        {
            Template template = null;

            template = await Task.Run(() => Utilities.Memory.LSTM.ImportTemplate(location));

            return template;
        }
        internal static async Task<bool> DeleteTemplate(string templateName)
        {
            return await Task.Run(() => Utilities.Memory.LSTM.DeleteTemplate(templateName));
        }
        #endregion
    }
}
