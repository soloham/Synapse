using Emgu.CV;
using Synapse.Core.Engines.Data;
using Synapse.Core.Keys;
using Synapse.Utilities;
using Synapse.Utilities.Attributes;
using Synapse.Utilities.Memory;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Synapse.Core.Configurations
{
    [Serializable]
    internal class ICRConfiguration : ConfigurationBase
    {
        #region Public Properties
        [Category("Behaviour"), Description("Get or set the name of the ICR Region.")]
        public string RegionName { get; set; }
        [Category("Behaviour"), Description("Get or set the type of output result expected from the ICR Region.")]
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
            ConfigurationBase.BaseData baseData = new ConfigurationBase.BaseData(regionName, MainConfigType.ICR, configArea, ValueDataType.Integer, Typography.Continious, ValueRepresentation.Collective, ValueEditType.ReadOnly, new ConfigRange(), processingIndex);
            return new ICRConfiguration(baseData);
        }

        public override ProcessedDataEntry ProcessSheet(Mat sheet)
        {
            return new ProcessedDataEntry(this, new byte[1, 1], new char[1], ProcessedDataType.FAULTY);
        }
        #endregion
    }
}
