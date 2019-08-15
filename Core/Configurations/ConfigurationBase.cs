using Synapse.Core.Keys;
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
    #region Enums
    internal enum MainConfigType
    {
        [EnumDescription("OMR")]
        OMR,
        [EnumDescription("Barcode")]
        BARCODE,
        [EnumDescription("ICR")]
        ICR        
    }
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
    #endregion

    #region Objects
    [Serializable]
    internal class ConfigRange
    {
        [Serializable]
        internal class ARange
        {
            public ARange()
            {
                Min = 0;
                Max = 1;

                HasRange = false;
            }
            public ARange(int min, int max) : this()
            {
                Min = min;
                Max = max;

                HasRange = true;
            }

            public bool HasRange { get; set; }
            public int Min { get; set; }
            public int Max { get; set; }

            public bool Validate(int number)
            {
                return number < Min ? false : number > Max ? false : true;
            }
        }
        [Serializable]
        internal class Range
        {
            public string ParameterTitle { get; set; }
            internal Parameter rangeParameter;
            public List<ARange> aRanges = new List<ARange>();

            public Range(string parameterTitle, Parameter rangeParameter, List<ARange> aRanges)
            {
                ParameterTitle = parameterTitle;
                this.rangeParameter = rangeParameter;
                this.aRanges = aRanges;
            }
            public Range(List<ARange> aRanges, string parameter = "")
            {
                this.aRanges = aRanges;

                this.ParameterTitle = parameter;
            }

            public Range(ARange defARange)
            {
                aRanges = new List<ARange> { defARange };
            }
        }

    }
    #endregion

    [Serializable]
    internal class ConfigurationBase
    {
        #region Objects
        [Serializable]
        internal class ConfigArea
        {
            public ConfigArea(RectangleF configRect)
            {
                ConfigRect = configRect;
            }

            public RectangleF ConfigRect;
            
        }
        #endregion

        #region Properties
        #region Public
        public string Title { get; set; }
        [Browsable(false)]
        public ConfigArea GetConfigArea { get { return configArea; } set { } }
        public ConfigRange GetConfigRange { get { return configRange; } set { } }
        public MainConfigType GetMainConfigType { get { return mainConfigType; } }
        public ValueDataType ValueDataType { get; set; }
        public Typography Typography { get; set; }
        public ValueRepresentation ValueRepresentation { get; set; }
        public ValueEditType ValueEditType { get; set; }
        #endregion
        #region Private
#pragma warning disable IDE0044 // Add readonly modifier
        private ConfigArea configArea;
#pragma warning restore IDE0044 // Add readonly modifier
        private MainConfigType mainConfigType;
        private ConfigRange configRange = null;
        #endregion
        #endregion

        #region Methods
        #region Public
        public ConfigurationBase(string title, MainConfigType mainConfigType, ConfigArea configArea, ValueDataType valueDataType, Typography typography, ValueRepresentation valueRepresentation, ValueEditType valueEditType, ConfigRange configRange)
        {
            Title = title;
            this.mainConfigType = mainConfigType;
            this.configArea = configArea;
            ValueDataType = valueDataType;
            Typography = typography;
            ValueRepresentation = valueRepresentation;
            ValueEditType = valueEditType;
            this.configRange = configRange;
        }
        public ConfigurationBase(ConfigurationBase initializationData)
        {
            Title = initializationData.Title;
            this.mainConfigType = initializationData.GetMainConfigType;
            this.configArea = initializationData.GetConfigArea;
            ValueDataType = initializationData.ValueDataType;
            Typography = initializationData.Typography;
            ValueRepresentation = initializationData.ValueRepresentation;
            ValueEditType = initializationData.ValueEditType;
            this.configRange = initializationData.configRange;
        }
        #endregion
        #endregion
    }
}
