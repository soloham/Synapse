using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Synapse.Core
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
                TemplateDataDirectory = Utilities.LSTM.GetTemplateDataPath(TemplateLocation);
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
        internal static bool SaveTemplate(Data tempData)
        {
            bool isSaved = true;

            OnSaveTemplateEvent?.Invoke(tempData);

            return isSaved;
        }

        internal static async Task<Template> LoadTemplate(string templateName)
        {
            return await Task.Run(() => Utilities.LSTM.LoadTemplate(templateName));
        }
        #endregion
    }
}
