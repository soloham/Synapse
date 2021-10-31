namespace Synapse.Core.Configurations
{
    using System;
    using System.ComponentModel;

    [Serializable]
    public class ICRConfiguration : ConfigurationBase
    {
        #region Public Properties

        [Category("Behaviour")]
        [Description("Get or set the name of the ICR Region.")]
        public string RegionName { get; set; }

        [Category("Behaviour")]
        [Description("Get or set the type of output result expected from the ICR Region.")]

        #endregion

        #region Public Methods

        public ICRConfiguration(ConfigurationBase _base) : base(_base)
        {
        }

        public ICRConfiguration(BaseData _baseData) : base(_baseData)
        {
        }

        #endregion

        #region Static Methods

        public static ICRConfiguration CreateDefault(string regionName, ConfigArea configArea, int processingIndex)
        {
            var baseData = new BaseData(regionName, MainConfigType.ICR, "", configArea,
                ValueDataType.Integer,
                Typography.Continious, ValueRepresentation.Collective, ValueEditType.ReadOnly, new ConfigRange(),
                processingIndex);
            return new ICRConfiguration(baseData);
        }

        //public override ProcessedDataEntry ProcessSheet(Mat sheet, string originalSheetPath = "")
        //{
        //    return new ProcessedDataEntry(Title, new char[1], new ProcessedDataType[1]);
        //}

        #endregion
    }
}