using Emgu.CV;
using Synapse.Core.Configurations;
using Synapse.Core.Engines.Data;
using Synapse.Core.Engines.Interface;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Synapse.Core.Engines
{
    internal class OMREngine : IEngine
    {
        public async Task<ProcessedDataEntry> ProcessSheet(ConfigurationBase configuration, Mat sheetMat)
        {
            OMRConfiguration omrConfiguration = (OMRConfiguration)configuration;

            OMRRegionData regionData = omrConfiguration.RegionData;
            int totalFields = regionData.TotalFields;
            int totalOptions = regionData.TotalOptions;

            byte[,] rawDataValues = new byte[totalFields, totalOptions];
            ProcessedDataResultType processedDataResultType = ProcessedDataResultType.NORMAL;

            List<RectangleF> optionsRects = regionData.GetOptionsRects;

            for (int i = 0; i < totalFields; i++)
            {
                for (int j = 0; j < totalOptions; j++)
                {
                    Mat curOption = new Mat(sheetMat, Rectangle.Round(optionsRects[i]));
                    var data = curOption.GetRawData();
                    int blackCount = data.Count(x => x < 80);
                    double blackCountPercent = blackCount / (double)data.Length;
                    if (blackCountPercent > 0.8)
                        rawDataValues[i, j] = 1;
                    else
                        rawDataValues[i, j] = 0;

                }
            }
            await Task.Run(() => SynapseMain.GetSynapseMain.SetCurrentImage(sheetMat.Bitmap));

            ProcessedDataEntry processedDataEntry = new ProcessedDataEntry(configuration, new string[0], processedDataResultType);
            return processedDataEntry;
        }
    }
}
