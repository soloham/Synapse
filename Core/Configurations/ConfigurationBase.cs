using Synapse.Utilities.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Synapse.Core.Configurations
{
    internal enum ValueDataType
    {
        [EnumDescription("String")]
        STRING,
        [EnumDescription("Text")]
        TEXT,
        [EnumDescription("Whole Number")]
        WHOLE_NUMBER,
        [EnumDescription("Natural Number")]
        NATURAL_NUMBER,
        [EnumDescription("Integer")]
        INTEGER
    }
    internal enum Typography
    {
        [EnumDescription("Continious")]
        CONTINIOUS,
        [EnumDescription("Hyphenated")]
        HYPHENATED,
    }
    internal enum ValueRepresentation
    {
        [EnumDescription("Collective")]
        COLLECTIVE,
        [EnumDescription("Indiviual")]
        INDIVIUAL,
        [EnumDescription("Combine Two")]
        COM2,
    }
    internal enum ValueEditType
    {
        [EnumDescription("Editable")]
        EDITABLE,
        [EnumDescription("Read Only")]
        READ_ONLY,
    }

    [Serializable]
    internal class ConfigurationBase
    {
        #region Objects
        [Serializable]
        internal class ConfigArea
        {
            public ConfigArea(Padding padding, Size size)
            {
                Padding = padding;
                Size = size;
            }

            public Padding Padding { get; set; }
            public Size Size { get; set; }
            
        }
        #endregion

        #region Properties
        #region Public
        public string Title { get; set; }
        [Browsable(false)]
        public ConfigArea GetConfigArea { get { return configArea; } set { } }
        public ValueDataType ValueDataType { get; set; }
        public Typography Typography { get; set; }
        public ValueRepresentation ValueRepresentation { get; set; }
        public ValueEditType ValueEditType { get; set; }
        #endregion
        #region Private
        #pragma warning disable IDE0044 // Add readonly modifier
        private ConfigArea configArea;
        #pragma warning restore IDE0044 // Add readonly modifier
        #endregion
        #endregion

        #region Methods
        #region Public
        public ConfigurationBase(string title, ConfigArea configArea, ValueDataType valueDataType, Typography typography, ValueRepresentation valueRepresentation, ValueEditType valueEditType)
        {
            Title = title;
            this.configArea = configArea;
            ValueDataType = valueDataType;
            Typography = typography;
            ValueRepresentation = valueRepresentation;
            ValueEditType = valueEditType;
        }
        #endregion
        #endregion
    }
}
