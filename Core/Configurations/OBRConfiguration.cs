using Emgu.CV;
using Inlite.ClearImage;
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
        [Category("Format"), Description("Get or set the format(s) of barcode expected from the Barcode Region.")]
        public bool Code128 { get; set; }
        [Category("Format"), Description("Get or set the format(s) of barcode expected from the Barcode Region.")]
        public bool Code93 { get; set; }
        [Category("Format"), Description("Get or set the value to automatically detect the barcode format in the Barcode Region.")]
        public bool Code39 { get; set; }
        [Category("Format"), Description("Get or set the format(s) of barcode expected from the Barcode Region.")]
        public bool AutoDetect1DBarcode { get; set; }
        [Category("Data"), Description("Get or set the value that determines wether to search the entire sheet for barcode if none is found in the Barcode Region.")]
        public bool SearchFullIfNull { get; set; }
        [Category("Data"), Description("Get or set the algorithm preference used during barcode recognition in the Barcode Region.")]
        public EBarcodeAlgorithm AlgorithmPreference { get; set; }
        [Category("Directions"), Description("Get or set the value that determines wether to search the entire sheet for barcode if none is found in the Barcode Region.")]
        public bool CheckHorizontal { get; set; }

        [Category("Directions"), Description("Get or set the value that determines wether to search the entire sheet for barcode if none is found in the Barcode Region.")]
        public bool CheckVertical { get; set; }

        [Category("Directions"), Description("Get or set the value that determines wether to search the entire sheet for barcode if none is found in the Barcode Region.")]
        public bool CheckDiagonal { get; set; }

        [Category("Directions"), Description("Get or set the value that determines wether to search the entire sheet for barcode if none is found in the Barcode Region.")]
        public bool CheckAll { get; set; }
        #endregion

        #region Public Methods
        public OBRConfiguration(ConfigurationBase _base, FBarcodeType barcodeType, bool autoDetect1DBarcode, EBarcodeAlgorithm algorithmPreference) : base(_base)
        {
            //BarcodeType = barcodeType;
            AutoDetect1DBarcode = autoDetect1DBarcode;
            AlgorithmPreference = algorithmPreference;
        }
        public OBRConfiguration(BaseData _baseData, FBarcodeType barcodeType, bool autoDetect1DBarcode, EBarcodeAlgorithm algorithmPreference) : base(_baseData)
        {
            //BarcodeType = barcodeType;
            AutoDetect1DBarcode = autoDetect1DBarcode;
            AlgorithmPreference = algorithmPreference;
        }
        #endregion

        #region Static Methods

        public static OBRConfiguration CreateDefault(string regionName, ConfigArea configArea, int processingIndex)
        {
            BaseData baseData = new BaseData(regionName, MainConfigType.BARCODE, configArea, ValueDataType.Text, Typography.Continious, ValueRepresentation.Collective, ValueEditType.ReadOnly, new ConfigRange(), processingIndex);
            return new OBRConfiguration(baseData, FBarcodeType.cibfCode128, true, EBarcodeAlgorithm.cibBestRecognition);
        }

        public override ProcessedDataEntry ProcessSheet(Mat sheet)
        {
            BarcodeEngine barcodeEngine = new BarcodeEngine();
            barcodeEngine.Initialize();

            return barcodeEngine.ProcessSheet(this, sheet);
        }
        #endregion
    }
}
