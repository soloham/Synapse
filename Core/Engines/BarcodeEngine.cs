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
        private CiServer ciServer;
        private CiBarcodePro barcodeRreader;

        internal void Initialize()
        {
            // Create objects
            ciServer = Server.GetThreadServer();
            barcodeRreader = ciServer.CreateBarcodePro();
        }

        public ProcessedDataEntry ProcessSheet(ConfigurationBase configuration, Mat sheet, Action<RectangleF, bool> OnOptionProcessed = null)
        {
            OBRConfiguration obrConfiguration = (OBRConfiguration)configuration;

            RectangleF barcodeRegion = obrConfiguration.GetConfigArea.ConfigRect;

            // Configure reader
            barcodeRreader.AutoDetect1D = obrConfiguration.AutoDetect1DBarcode; // Enable automatic detection of barcode type (Slower processing)
            if(!obrConfiguration.AutoDetect1DBarcode)
                barcodeRreader.Type = obrConfiguration.BarcodeType; // Select barcode types to read
            if (obrConfiguration.CheckAll) // Limit barcode search direction (Faster processing)
                barcodeRreader.Directions = FBarcodeDirections.cibHorz | FBarcodeDirections.cibVert | FBarcodeDirections.cibDiag;
            else
            {
                if (obrConfiguration.CheckHorizontal)// Limit barcode search direction (Faster processing)
                    barcodeRreader.Directions = !obrConfiguration.CheckVertical && !obrConfiguration.CheckDiagonal ? FBarcodeDirections.cibHorz : barcodeRreader.Directions | FBarcodeDirections.cibHorz;
                if (obrConfiguration.CheckVertical)// Limit barcode search direction (Faster processing)
                    barcodeRreader.Directions = !obrConfiguration.CheckHorizontal && !obrConfiguration.CheckDiagonal ? FBarcodeDirections.cibHorz : barcodeRreader.Directions | FBarcodeDirections.cibVert;
                if (obrConfiguration.CheckDiagonal)// Limit barcode search direction (Faster processing)
                    barcodeRreader.Directions = !obrConfiguration.CheckVertical && !obrConfiguration.CheckHorizontal ? FBarcodeDirections.cibHorz : barcodeRreader.Directions | FBarcodeDirections.cibDiag;
            }
            barcodeRreader.Algorithm = obrConfiguration.AlgorithmPreference;

            string output = "-";
            using (Bitmap region = sheet.Bitmap.Clone(barcodeRegion, System.Drawing.Imaging.PixelFormat.Format8bppIndexed)) {
                // open input images
                barcodeRreader.Image.OpenFromBitmap(region.GetHbitmap());

                CiBarcodes barcodes = ReadBarcodes1D();
                if (barcodes.Count == 0 && obrConfiguration.SearchFullIfNull)
                {
                    barcodeRreader.Image.OpenFromBitmap(sheet.Bitmap.GetHbitmap());
                    barcodes = ReadBarcodes1D();
                }
                output = barcodes.Count > 0 ? barcodes.GetItem(0).Text : "-";
            }

            return new ProcessedDataEntry(configuration.Title, output.ToCharArray(), new ProcessedDataType[] { ProcessedDataType.NORMAL });
            //using (Bitmap barcodeBmp = sheet.Bitmap.Clone(barcodeRegion, System.Drawing.Imaging.PixelFormat.Format8bppIndexed))
            //{
                
            //}
        }

        CiBarcodes ReadBarcodes1D()
        {
            barcodeRreader.Find(0);

            return barcodeRreader.Barcodes;
        }
    }
}
