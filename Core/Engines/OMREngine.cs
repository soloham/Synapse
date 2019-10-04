using Emgu.CV;
using Synapse.Core.Configurations;
using Synapse.Core.Engines.Data;
using Synapse.Core.Engines.Interface;
using Synapse.Core.Keys;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace Synapse.Core.Engines
{
    internal class OMREngine : IEngine
    {
        public ProcessedDataEntry ProcessSheet(ConfigurationBase configuration, Mat sheetMat, Action<RectangleF, bool> OnOptionProcessed = null)
        {
            OMRConfiguration omrConfiguration = (OMRConfiguration)configuration;

            OMRRegionData regionData = omrConfiguration.RegionData;
            int totalFields = regionData.TotalFields;
            int totalOptions = regionData.TotalOptions;

            ProcessedDataType processedDataResultType = ProcessedDataType.NORMAL;

            PointF regionLocation = omrConfiguration.GetConfigArea.ConfigRect.Location;
            List<RectangleF> optionsRects = regionData.GetOptionsRects;

            char muliMarkSymbol = omrConfiguration.MultiMarkSymbol;
            MultiMarkAction multiMarkAction = omrConfiguration.MultiMarkAction;
            char noneMarkedSymbol = omrConfiguration.NoneMarkedSymbol;
            NoneMarkedAction noneMarkedAction = omrConfiguration.NoneMarkedAction;

            char[] regionFieldsOutputs = new char[totalFields];
            string regionOutput = "";

            int curOptionRectIndex = 0;
            List<byte> filledIndexes = new List<byte>();

            double curBlackCountThreshold = omrConfiguration.BlackCountThreshold;
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
                    int blackCount = data.Count(x => x < 180);
                    double blackCountPercent = blackCount / (double)data.Length;
                    bool isFilled = blackCountPercent > curBlackCountThreshold;
                    if (isFilled)
                        filledIndexes.Add((byte)j);

                    OnOptionProcessed?.Invoke(optionsRects[curOptionRectIndex], isFilled);

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
                                processedDataResultType = ProcessedDataType.MANUAL;
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
                            processedDataResultType = ProcessedDataType.MANUAL;
                            break;
                        case NoneMarkedAction.Invalidate:
                            curFieldOutput = noneMarkedSymbol;
                            break;
                    }
                }

                regionFieldsOutputs[i] = curFieldOutput;
                regionOutput += curFieldOutput.ToString();
            }

            ProcessedDataEntry processedDataEntry = new ProcessedDataEntry(configuration.Title, regionFieldsOutputs, processedDataResultType);
            return processedDataEntry;
        }

        public async Task<ProcessedDataEntry> ProcessSheetRaw(ConfigurationBase configuration, Mat sheetMat, Action<RectangleF, bool> OnOptionProcessed = null, Func<double> GetWaitMS = null)
        {
            OMRConfiguration omrConfiguration = (OMRConfiguration)configuration;

            OMRRegionData regionData = omrConfiguration.RegionData;
            int totalFields = regionData.TotalFields;
            int totalOptions = regionData.TotalOptions;

            ProcessedDataType processedDataResultType = ProcessedDataType.NORMAL;

            PointF regionLocation = omrConfiguration.GetConfigArea.ConfigRect.Location;
            List<RectangleF> optionsRects = regionData.GetOptionsRects;

            char muliMarkSymbol = omrConfiguration.MultiMarkSymbol;
            MultiMarkAction multiMarkAction = omrConfiguration.MultiMarkAction;
            char noneMarkedSymbol = omrConfiguration.NoneMarkedSymbol;
            NoneMarkedAction noneMarkedAction = omrConfiguration.NoneMarkedAction;

            char[] regionFieldsOutputs = new char[totalFields];
            string regionOutput = "";

            int curOptionRectIndex = 0;
            List<byte> filledIndexes = new List<byte>();

            double curBlackCountThreshold = omrConfiguration.BlackCountThreshold;
            for (int i = 0; i < totalFields; i++)
            {
                char curFieldOutput = '0';
                filledIndexes = new List<byte>();
                for (int j = 0; j < totalOptions; j++)
                {
                    Rectangle curOptionRect = Rectangle.Round(optionsRects[curOptionRectIndex]);
                    //curOptionRect = Rectangle.Round(optionsRects[(j == 0 && i == 0)? 0 : (j == totalOptions-1 && i == totalFields-1)? optionsRects.Count - 1 : curOptionRectIndex+1]);
                    curOptionRect.X += (int)regionLocation.X;
                    curOptionRect.Y += (int)regionLocation.Y;

                    Mat curOption = new Mat(sheetMat, curOptionRect);
                    var data = curOption.GetRawData();
                    int blackCount = data.Count(x => x < 150);
                    double blackCountPercent = blackCount / (double)data.Length;
                    bool isFilled = blackCountPercent > curBlackCountThreshold;
                    if (isFilled)
                        filledIndexes.Add((byte)j);

                    OnOptionProcessed?.Invoke(curOptionRect, isFilled);
                    if (OnOptionProcessed != null)
                        await Task.Delay(TimeSpan.FromMilliseconds(GetWaitMS()));

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
                                processedDataResultType = ProcessedDataType.MANUAL;
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
                            processedDataResultType = ProcessedDataType.MANUAL;
                            break;
                        case NoneMarkedAction.Invalidate:
                            curFieldOutput = noneMarkedSymbol;
                            break;
                    }
                }

                regionFieldsOutputs[i] = curFieldOutput;
                regionOutput += curFieldOutput.ToString();
            }

            ProcessedDataEntry processedDataEntry = new ProcessedDataEntry(configuration.Title, regionFieldsOutputs, processedDataResultType);
            return processedDataEntry;
        }

        public static (int totalMarks, int obtainedMarks) GradeSheet(AnswerKey answerKey, byte[][] observedOptions)
        {
            int totalMarks = 0, obtainedMarks = 0;

            int correctOptionValue = answerKey.GetPaper.GetCorrectOptionValue;
            int wrongOptionValue = answerKey.GetPaper.GetWrongOptionValue;
            int[][] keyOptions = answerKey.GetKey;
            int totalFields = keyOptions.Length;
            int totalOptions = keyOptions[0].Length;

            totalMarks = correctOptionValue * totalFields;
            for (int i = 0; i < totalFields; i++)
            {
                bool isCorrect = false;
                for (int j = 0; j < totalOptions; j++)
                {
                    if (observedOptions[i][j] == 1 && observedOptions[i][j] == keyOptions[i][j])
                        isCorrect = true;
                }
                if (isCorrect)
                    obtainedMarks += correctOptionValue;
                else
                    obtainedMarks -= wrongOptionValue;
            }

            return (totalMarks, obtainedMarks);
        }
        public static (int totalMarks, int obtainedMarks) GradeSheet(AnswerKey answerKey, byte[,] observedOptions)
        {
            int totalMarks = 0, obtainedMarks = 0;

            int correctOptionValue = answerKey.GetPaper.GetCorrectOptionValue;
            int wrongOptionValue = answerKey.GetPaper.GetWrongOptionValue;
            int[][] keyOptions = answerKey.GetKey;
            int totalFields = keyOptions.Length;
            int totalOptions = keyOptions[0].Length;

            totalMarks = correctOptionValue * totalFields;
            for (int i = 0; i < totalFields; i++)
            {
                bool isCorrect = false;
                for (int j = 0; j < totalOptions; j++)
                {
                    if (observedOptions[i,j] == 1 && observedOptions[i,j] == keyOptions[i][j])
                        isCorrect = true;
                }
                if (isCorrect)
                    obtainedMarks += correctOptionValue;
                else
                    obtainedMarks -= wrongOptionValue;
            }

            return (totalMarks, obtainedMarks);
        }
    }
}
