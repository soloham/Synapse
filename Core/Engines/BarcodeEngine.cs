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
    internal class BarcodeEngine : IEngine
    {
        public ProcessedDataEntry ProcessSheet(ConfigurationBase configuration, Mat sheet, Action<RectangleF, bool> OnOptionProcessed = null)
        {
            OBRConfiguration obrConfiguration = (OBRConfiguration)configuration;

            RectangleF barcodeRegion = obrConfiguration.GetConfigArea.ConfigRect;
            Bitmap barcodeBmp = sheet.Bitmap.Clone(barcodeRegion, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);

            CiBarcodes barcodes = ReadBarcodes1D(barcodeBmp);
            string output = barcodes.Count > 0? barcodes.GetItem(0).Text : "-";

            return new ProcessedDataEntry(configuration.Title, output.ToCharArray(), new ProcessedDataType[] { ProcessedDataType.NORMAL });
        }

        CiBarcodes ReadBarcodes1D(Bitmap imageFile)
        {
            // Create objects and open input images
            CiServer ci = Inlite.ClearImageNet.Server.GetThreadServer();
            CiBarcodePro reader = ci.CreateBarcodePro();
            reader.Image.OpenFromBitmap(imageFile.GetHbitmap());
            // Configure reader
            //reader.Type = FBarcodeType.cibfCode39 | FBarcodeType.cibfCode128;  // Select barcode types to read
            reader.AutoDetect1D = true;  // Enable automatic detection of barcode type (Slower processing)
                                         // reader.Directions = FBarcodeDirections.cibHorz;   // Limit barcode search direction (Faster processing)
            reader.Find(0);
            foreach (CiBarcode Barcode in reader.Barcodes)
                Console.WriteLine(Barcode.Text);

            return reader.Barcodes;
        }
    }
}
