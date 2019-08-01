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
        internal class Data
        {
            internal string TemplateName;
            internal string TemplateLocation;
            internal System.Drawing.Size TemplateSize;

            internal Data(string templateName, string templateLocation, Size templateSize)
            {
                TemplateName = templateName;
                TemplateLocation = templateLocation;
                TemplateSize = templateSize;
            }
        }
        public Data TemplateData;

        public string GetTemplateName { get { return TemplateData.TemplateName; } set { } }
        public string GetTemplateLocation { get { return TemplateData.TemplateLocation; } set { } }
        public Size GetTemplateSize { get { return TemplateData.TemplateSize; } set { } }

        public static Data CreateTemplate(string tmpName)
        {
            return new Data(tmpName, "", Size.Empty);
        }

        public Template(Data tmpData)
        {
            TemplateData = tmpData;
        }
    }
}
