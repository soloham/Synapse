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
        
        #endregion

        #region Private Properties
        #endregion

        #region Variables
        #endregion

        #region Public Methods
        public ICRConfiguration(ConfigurationBase _base) : base(_base)
        {
        }
        #endregion

        #region Static Methods

        public static ICRConfiguration CreateDefault(string regionName, ConfigArea configArea, int processingIndex)
        {
            ConfigurationBase configurationBase = new ConfigurationBase(regionName, MainConfigType.ICR, configArea, ValueDataType.Integer, Typography.Continious, ValueRepresentation.Collective, ValueEditType.ReadOnly, new ConfigRange(), processingIndex);
            return new ICRConfiguration(configurationBase);
        }
        #endregion
    }
}
