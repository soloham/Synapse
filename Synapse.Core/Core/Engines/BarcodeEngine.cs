using Emgu.CV;
using Inlite.ClearImage;
using Inlite.ClearImageNet;
using Synapse.Core.Configurations;
using Synapse.Core.Engines.Data;
using Synapse.Core.Engines.Interface;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Synapse.Core.Engines
{
    public class BarcodeEngine : IEngine
    {
        BarcodeReader barcodeReader;
        public BarcodeEngine()
        {
            Initialize();
        }
        public void Initialize()
        {
            // Create objects
            //ciServer = Server.GetThreadServer();
            //barcodeReader = ciServer.CreateBarcodePro();
            barcodeReader = new BarcodeReader();
        }

        public ProcessedDataEntry ProcessSheet(ConfigurationBase configuration, Mat sheet, Action<RectangleF, bool> OnOptionProcessed = null, string originalSheetPath = "")
        {
            OBRConfiguration obrConfiguration = (OBRConfiguration)configuration;

            RectangleF barcodeRegion = obrConfiguration.GetConfigArea.ConfigRect;

            // Configure reader
            //CiServer ciServer = Server.GetThreadServer();
            //CiBarcodePro barcodeReader = ciServer.CreateBarcodePro();
            barcodeReader.Auto1D = obrConfiguration.AutoDetect1DBarcode; // Enable automatic detection of barcode type (Slower processing)
            if (!obrConfiguration.AutoDetect1DBarcode)
            {// Select barcode types to read
                barcodeReader.Code128 = obrConfiguration.Code128; 
                barcodeReader.Code93 = obrConfiguration.Code93; 
                barcodeReader.Code39 = obrConfiguration.Code39; 
            }
            if (obrConfiguration.CheckAll) // Limit barcode search direction (Faster processing)
            {
                barcodeReader.Horizontal = true;
                barcodeReader.Vertical = true;
                barcodeReader.Diagonal = true;
            }
            else
            {
                barcodeReader.Horizontal = obrConfiguration.CheckHorizontal;    
                barcodeReader.Vertical = obrConfiguration.CheckVertical;    
                barcodeReader.Diagonal = obrConfiguration.CheckDiagonal;    
            }
            //barcodeReader.Algorithm = obrConfiguration.AlgorithmPreference;

            string output = "-";
            using (Bitmap region = sheet.Bitmap.Clone(barcodeRegion, System.Drawing.Imaging.PixelFormat.Format8bppIndexed))
            {
                Barcode[] barcodes = null;
                try
                {
                    barcodes = barcodeReader.Read(region);   // Read barcodes
                }
                catch (Exception ex)
                { 
                    Console.WriteLine("Exception: " + ex.ToString()); 
                }
                finally
                {
                    if (barcodeReader != null) barcodeReader.Dispose();
                }  //Free image memory.
                if (barcodes.Length == 0 && obrConfiguration.SearchFullIfNull)
                {
                    using (Mat orignalSheet = CvInvoke.Imread(originalSheetPath))
                    {
                        barcodes = barcodeReader.Read(orignalSheet.Bitmap);   // Read barcodes
                    }
                }
                output = barcodes.Length > 0 ? barcodes[0].Text : "-";
            }

            return new ProcessedDataEntry(configuration.Title, output.ToCharArray(), new ProcessedDataType[] { ProcessedDataType.NORMAL }, new byte[0,0]);
        }

        void ReadBarcodes1D(string fileName, int page)
        {
            BarcodeReader reader = null;
            try
            {
                reader = new BarcodeReader();   // Create and configure reader
                reader.Code39 = true; reader.Code128 = true;
                Barcode[] barcodes = reader.Read(fileName, page);   // Read barcodes
                foreach (Barcode barcode in barcodes)   // Process results
                { Console.WriteLine("Barcode type: " + barcode.Type.ToString() + "  Text: " + Environment.NewLine + barcode.Text); }
            }
            catch (Exception ex)
            { Console.WriteLine("Exception: " + ex.ToString()); }
            finally
            { if (reader != null) reader.Dispose(); }  // ClearImage 9 and latter.  Free image memory.
        }
    }
}
