using Emgu.CV;
using Synapse.Core.Engines;
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
    internal class OBRConfiguration : ConfigurationBase
    {
        #region Public Properties
        [Category("Behaviour"), Description("Get or set the name of the Barcode Region.")]
        public string RegionName { get; set; }
        [Category("Behaviour"), Description("Get or set the type of output result expected from the Barcode Region.")]
        #endregion

        #region Public Methods
        public OBRConfiguration(ConfigurationBase _base) : base(_base)
        {
        }
        public OBRConfiguration(BaseData _baseData) : base(_baseData)
        {
        }
        #endregion

        #region Static Methods

        public static OBRConfiguration CreateDefault(string regionName, ConfigArea configArea, int processingIndex)
        {
            BaseData baseData = new BaseData(regionName, MainConfigType.BARCODE, configArea, ValueDataType.Text, Typography.Continious, ValueRepresentation.Collective, ValueEditType.ReadOnly, new ConfigRange(), processingIndex);
            return new OBRConfiguration(baseData);
        }

        public override ProcessedDataEntry ProcessSheet(Mat sheet)
        {
            BarcodeEngine barcodeEngine = new BarcodeEngine();
            return barcodeEngine.ProcessSheet(this, sheet);
        }
        #endregion
    }
}
