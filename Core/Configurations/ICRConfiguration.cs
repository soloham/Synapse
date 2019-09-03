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
        #endregion

        #region Static Methods
        public static ICRConfiguration CreateDefault(string regionName, Orientation orientation, ConfigArea configArea, OMRRegionData regionData, int processingIndex)
        {
            ConfigurationBase configurationBase = new ConfigurationBase(regionName, MainConfigType.ICR, configArea, ValueDataType.Integer, Typography.Continious, ValueRepresentation.Collective, ValueEditType.ReadOnly, new ConfigRange(), processingIndex);
            return new ICRConfiguration(configurationBase);
        }
        #endregion
    }
}
