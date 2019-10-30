using Emgu.CV;
using Emgu.CV.Structure;
using Synapse.Core.Engines.Data;
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
    public enum MainConfigType
    {
        [EnumDescription("OMR")]
        OMR,
        [EnumDescription("Barcode")]
        BARCODE,
        [EnumDescription("ICR")]
        ICR        
    }
    public enum ValueDataType
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
    public enum Typography
    {
        [EnumDescription("Continious")]
        Continious,
        [EnumDescription("Hyphenated")]
        Hyphenated,
    }
    public enum ValueRepresentation
    {
        [EnumDescription("Collective")]
        Collective,
        [EnumDescription("Indiviual")]
        Indiviual,
        [EnumDescription("Combine Two")]
        CombineTwo,
    }
    public enum ValueEditType
    {
        [EnumDescription("Editable")]
        Editable,
        [EnumDescription("Read Only")]
        ReadOnly,
    }
    #endregion

    #region Objects
    [Serializable]
    public class ConfigRange
    {
        [Serializable]
        public class ARange
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
        public class Range
        {
            public string ParameterTitle { get; set; }
            public Parameter rangeParameter;
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
    public abstract class ConfigurationBase
    {
        #region Objects
        [Serializable]
        public class ConfigArea
        {
            public ConfigArea(RectangleF configRect, Bitmap configBitmap)
            {
                ConfigRect = configRect;
                ConfigBitmap = configBitmap;
            }

            public RectangleF ConfigRect;
            public Bitmap ConfigBitmap;
        }
        [Serializable]
        public struct BaseData
        {
            public string title;
            public MainConfigType mainConfigType;
            public ConfigArea configArea;
            public ValueDataType valueDataType;
            public Typography typography;
            public ValueRepresentation valueRepresentation;
            public ValueEditType valueEditType;
            public ConfigRange configRange;
            public int processingIndex;

            public BaseData(string title, MainConfigType mainConfigType, ConfigArea configArea, ValueDataType valueDataType, Typography typography, ValueRepresentation valueRepresentation, ValueEditType valueEditType, ConfigRange configRange, int processingIndex)
            {
                this.title = title;
                this.mainConfigType = mainConfigType;
                this.configArea = configArea;
                this.valueDataType = valueDataType;
                this.typography = typography;
                this.valueRepresentation = valueRepresentation;
                this.valueEditType = valueEditType;
                this.configRange = configRange;
                this.processingIndex = processingIndex;
            }
            public BaseData(ConfigurationBase initializationData)
            {
                title = initializationData.Title;
                mainConfigType = initializationData.GetMainConfigType;
                configArea = initializationData.GetConfigArea;
                valueDataType = initializationData.ValueDataType;
                typography = initializationData.Typography;
                valueRepresentation = initializationData.ValueRepresentation;
                valueEditType = initializationData.ValueEditType;
                configRange = initializationData.configRange;
                this.processingIndex = initializationData.ProcessingIndex;
            }
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
        public int ProcessingIndex { get; set; }
        [Category("Data"), Description("Get or set the type of data the OMR Region represents.")]
        public ValueDataType ValueDataType { get; set; }
        [Category("Appearance"), Description("Get or set the appearance of the value for the OMR Region.")]
        public Typography Typography { get; set; }
        [Category("Layout"), Description("Get or set the representaion strucure of data for the OMR Region.")]
        public ValueRepresentation ValueRepresentation { get; set; }
        [Category("Data"), Description("Get or set the type of data editing to be used for the OMR Region.")]
        public ValueEditType ValueEditType { get; set; }
        [Category("Data"), Description("Get or set the value indicating if the output of this region will be added to the name of the sheet's file.")]
        public bool AddToFileName { get; set; }
        #endregion
        #region Private
#pragma warning disable IDE0044 // Add readonly modifier
        private ConfigArea configArea;
#pragma warning restore IDE0044 // Add readonly modifier
        private MainConfigType mainConfigType;
        private ConfigRange configRange = null;
        #endregion
        #endregion

        #region Variables
        public delegate bool LSTMConfigDataDelegate(ConfigurationBase configurationBase, MainConfigType mainConfigType, out Exception ex);
        public static LSTMConfigDataDelegate DeleteConfigData;
        public static LSTMConfigDataDelegate SaveConfigData;
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
        public ConfigurationBase(BaseData baseData)
        {
            Title = baseData.title;
            this.mainConfigType = baseData.mainConfigType;
            this.configArea = baseData.configArea;
            ValueDataType = baseData.valueDataType;
            Typography = baseData.typography;
            ValueRepresentation = baseData.valueRepresentation;
            ValueEditType = baseData.valueEditType;
            this.configRange = baseData.configRange;
            ProcessingIndex = baseData.processingIndex;
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

        #region NotInUse
        //public ConfigurationBase Create(string title, MainConfigType mainConfigType, ConfigArea configArea, ValueDataType valueDataType, Typography typography, ValueRepresentation valueRepresentation, ValueEditType valueEditType, ConfigRange configRange, int processingIndex)
        //{
        //    return new ConfigurationBase(title, mainConfigType, configArea, valueDataType, typography, valueRepresentation, valueEditType, configRange, processingIndex);
        //}
        //public ConfigurationBase Create(ConfigurationBase initializationData)
        //{
        //    return new ConfigurationBase(initializationData);
        //}
        #endregion

        //public abstract ProcessedDataEntry ProcessSheet(Mat sheet, string originalSheetPath = "");
        #endregion
        #region Static
        public static bool Save(ConfigurationBase config, out Exception ex)
        {
            bool? result = false;

            ex = null;
            result = SaveConfigData?.Invoke(config, config.GetMainConfigType, out ex);

            return result.GetValueOrDefault();
        }
        public static bool Delete(ConfigurationBase config, out Exception ex)
        {
            bool? result = false;

            ex = null;
            result = DeleteConfigData?.Invoke(config, config.GetMainConfigType, out ex);

            return result.GetValueOrDefault();
        }
        #endregion
        #endregion
    }
}
