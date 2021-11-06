namespace Synapse.Core.Engines
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;

    using Emgu.CV;

    using Inlite.ClearImageNet;

    using Synapse.Core.Configurations;
    using Synapse.Core.Engines.Data;
    using Synapse.Core.Engines.Interface;

    public class BarcodeEngine : IEngine
    {
        private BarcodeReader barcodeReader;

        public BarcodeEngine()
        {
            this.Initialize();
        }

        public void Initialize()
        {
            // Create objects
            //ciServer = Server.GetThreadServer();
            //barcodeReader = ciServer.CreateBarcodePro();
            barcodeReader = new BarcodeReader();
        }

        public ProcessedDataEntry ProcessSheet(ConfigurationBase configuration, Mat sheet,
            Action<RectangleF, bool> OnOptionProcessed = null, string originalSheetPath = "")
        {
            var obrConfiguration = (OBRConfiguration)configuration;

            var barcodeRegion = obrConfiguration.GetConfigArea.ConfigRect;

            // Configure reader
            //CiServer ciServer = Server.GetThreadServer();
            //CiBarcodePro barcodeReader = ciServer.CreateBarcodePro();
            barcodeReader.Auto1D =
                obrConfiguration.AutoDetect1DBarcode; // Enable automatic detection of barcode type (Slower processing)
            if (!obrConfiguration.AutoDetect1DBarcode)
            {
                // Select barcode types to read
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

            var output = "-";
            Barcode[] barcodes = null;
            using (var region = sheet.Bitmap.Clone(barcodeRegion, PixelFormat.Format8bppIndexed))
            {
                try
                {
                    barcodes = barcodeReader.Read(region); // Read barcodes
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception: " + ex);
                }
                finally
                {
                    if (barcodeReader != null)
                    {
                        barcodeReader.Dispose();
                    }
                } //Free image memory.

                if (barcodes.Length == 0 && obrConfiguration.SearchFullIfNull)
                {
                    using (var orignalSheet = CvInvoke.Imread(originalSheetPath))
                    {
                        barcodes = barcodeReader.Read(orignalSheet.Bitmap); // Read barcodes
                    }
                }

                output = barcodes.Length > 0 ? barcodes[0].Text : "-";
            }

            var title = !string.IsNullOrEmpty(configuration.ParentTitle)
                ? configuration.ParentTitle
                : configuration.Title;

            return new ProcessedDataEntry(title, output.ToCharArray(), new[] { ProcessedDataType.NORMAL },
                new byte[0, 0], barcodes, configuration.Title);
        }

        private void ReadBarcodes1D(string fileName, int page)
        {
            BarcodeReader reader = null;
            try
            {
                reader = new BarcodeReader(); // Create and configure reader
                reader.Code39 = true;
                reader.Code128 = true;
                var barcodes = reader.Read(fileName, page); // Read barcodes
                foreach (var barcode in barcodes)           // Process results
                    Console.WriteLine("Barcode type: " + barcode.Type + "  Text: " + Environment.NewLine +
                                      barcode.Text);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Dispose();
                }
            } // ClearImage 9 and latter.  Free image memory.
        }
    }
}