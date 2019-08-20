using Synapse.Core.Keys;
using Synapse.Utilities.Attributes;
using Synapse.Utilities.Memory;
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
        String,
        [EnumDescription("Text")]
        Text,
        [EnumDescription("Alphabet")]
        Alphabet,
        [EnumDescription("Whole Number")]
        WholeNumber,
        [EnumDescription("Natural Number")]
        NaturalNumber,
        [EnumDescription("Integer")]
        Integer
    }
    internal enum Typography
    {
        [EnumDescription("Continious")]
        Continious,
        [EnumDescription("Hyphenated")]
        Hyphenated,
    }
    internal enum ValueRepresentation
    {
        [EnumDescription("Collective")]
        Collective,
        [EnumDescription("Indiviual")]
        Indiviual,
        [EnumDescription("Combine Two")]
        CombineTwo,
    }
    internal enum ValueEditType
    {
        [EnumDescription("Editable")]
        Editable,
        [EnumDescription("Read Only")]
        ReadOnly,
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
            public ConfigArea(RectangleF configRect, Bitmap configBitmap)
            {
                ConfigRect = configRect;
                ConfigBitmap = configBitmap;
            }

            public RectangleF ConfigRect;
            public Bitmap ConfigBitmap;
        }
        #endregion

        #region Properties
        #region Public
        [Category("Appearance"), Description("Get or set the title for the OMR Region.")]
        public string Title { get; set; }
        [Browsable(false)]
        public ConfigArea GetConfigArea { get { return configArea; } set { } }
        [Browsable(false)]
        public ConfigRange GetConfigRange { get { return configRange; } set { } }
        [Browsable(false)]
        public MainConfigType GetMainConfigType { get { return mainConfigType; } }
        [Browsable(false)]
        public int ProcessingIndex { get; internal set; }
        [Category("Data"), Description("Get or set the type of data the OMR Region represents.")]
        public ValueDataType ValueDataType { get; set; }
        [Category("Appearance"), Description("Get or set the appearance of the value for the OMR Region.")]
        public Typography Typography { get; set; }
        [Category("Layout"), Description("Get or set the representaion strucure of data for the OMR Region.")]
        public ValueRepresentation ValueRepresentation { get; set; }
        [Category("Data"), Description("Get or set the type of data editing to be used for the OMR Region.")]
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
        public ConfigurationBase(string title, MainConfigType mainConfigType, ConfigArea configArea, ValueDataType valueDataType, Typography typography, ValueRepresentation valueRepresentation, ValueEditType valueEditType, ConfigRange configRange, int processingIndex)
        {
            Title = title;
            this.mainConfigType = mainConfigType;
            this.configArea = configArea;
            ValueDataType = valueDataType;
            Typography = typography;
            ValueRepresentation = valueRepresentation;
            ValueEditType = valueEditType;
            this.configRange = configRange;
            ProcessingIndex = processingIndex;
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
        #region Static
        public static bool Save(ConfigurationBase config, out Exception ex)
        {
            bool result = false;

            result = LSTM.SaveConfigData(config, config.GetMainConfigType, out ex);

            return result;
        }
        public static bool Delete(ConfigurationBase config, out Exception ex)
        {
            bool result = false;

            result = LSTM.DeleteConfigData(config, config.GetMainConfigType, out ex);

            return result;
        }
        #endregion
        #endregion

    }
}
