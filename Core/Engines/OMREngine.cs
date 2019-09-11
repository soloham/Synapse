using Emgu.CV;
using Synapse.Core.Configurations;
using Synapse.Core.Engines.Data;
using Synapse.Core.Engines.Interface;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Synapse.Core.Engines
{
    internal class OMREngine : IEngine
    {
        public ProcessedDataEntry ProcessSheet(ConfigurationBase configuration, Mat sheetMat)
        {
            OMRConfiguration omrConfiguration = (OMRConfiguration)configuration;

            OMRRegionData regionData = omrConfiguration.RegionData;
            int totalFields = regionData.TotalFields;
            int totalOptions = regionData.TotalOptions;

            byte[,] rawDataValues = new byte[totalFields, totalOptions];
            ProcessedDataResultType processedDataResultType = ProcessedDataResultType.NORMAL;

            PointF regionLocation = omrConfiguration.GetConfigArea.ConfigRect.Location;
            List<RectangleF> optionsRects = regionData.GetOptionsRects;
            List<RectangleF> fieldsRects = regionData.GetFieldsRects;

            char muliMarkSymbol = omrConfiguration.MultiMarkSymbol;
            MultiMarkAction multiMarkAction = omrConfiguration.MultiMarkAction;
            char noneMarkedSymbol = omrConfiguration.NoneMarkedSymbol;
            NoneMarkedAction noneMarkedAction = omrConfiguration.NoneMarkedAction;

            char[] regionFieldsOutputs = new char[totalFields];
            string regionOutput = "";

            int curOptionRectIndex = 0;
            List<byte> filledIndexes = new List<byte>();
            for (int i = 0; i < totalFields; i++)
            {
                char curFieldOutput = '0';
                filledIndexes = new List<byte>();
                for (int j = 0; j < totalOptions; j++)
                {
                    Rectangle curOptionRect = Rectangle.Round(optionsRects[curOptionRectIndex]);
                    curOptionRect.X += (int)regionLocation.X;
                    curOptionRect.Y += (int)regionLocation.Y;

                    Mat curOption = new Mat(sheetMat, curOptionRect);
                    var data = curOption.GetRawData();
                    int blackCount = data.Count(x => x < 150);
                    double blackCountPercent = blackCount / (double)data.Length;
                    bool isFilled = blackCountPercent > 0.30;
                    if (isFilled)
                    {
                        rawDataValues[i, j] = 1;

                        filledIndexes.Add((byte)j);
                    }
                    else
                        rawDataValues[i, j] = 0;

                    curOptionRectIndex++;
                }

                byte totalFilled = (byte)filledIndexes.Count;
                if (totalFilled > 0)
                {
                    if (totalFilled > 1)
                    {
                        switch (multiMarkAction)
                        {
                            case MultiMarkAction.MarkAsManual:
                                curFieldOutput = muliMarkSymbol;
                                processedDataResultType = ProcessedDataResultType.MANUAL;
                                break;
                            case MultiMarkAction.ConsiderFirst:
                                break;
                            case MultiMarkAction.ConsiderLast:
                                break;
                            case MultiMarkAction.ConsiderCorrect:
                                break;
                            case MultiMarkAction.Invalidate:
                                curFieldOutput = muliMarkSymbol;
                                break;
                        }
                    }
                    else
                    {
                        switch (omrConfiguration.ValueDataType)
                        {
                            case ValueDataType.String:
                                break;
                            case ValueDataType.Text:
                                break;
                            case ValueDataType.Alphabet:
                                curFieldOutput = (char)(filledIndexes[0] + 65);
                                break;
                            case ValueDataType.WholeNumber:
                                curFieldOutput = (char)(filledIndexes[0] + 48);
                                break;
                            case ValueDataType.NaturalNumber:
                                curFieldOutput = (char)(filledIndexes[0] + 49);
                                break;
                            case ValueDataType.Integer:
                                break;
                            default:
                                break;
                        }
                    }
                }
                else
                {
                    switch (noneMarkedAction)
                    {
                        case NoneMarkedAction.MarkAsManual:
                            curFieldOutput = noneMarkedSymbol;
                            processedDataResultType = ProcessedDataResultType.MANUAL;
                            break;
                        case NoneMarkedAction.Invalidate:
                            curFieldOutput = noneMarkedSymbol;
                            break;
                    }
                }

                regionFieldsOutputs[i] = curFieldOutput;
                regionOutput += curFieldOutput.ToString();
            }

            ProcessedDataEntry processedDataEntry = new ProcessedDataEntry(configuration, rawDataValues, regionFieldsOutputs, processedDataResultType);
            return processedDataEntry;
        }
    }
}
