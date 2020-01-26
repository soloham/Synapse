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
    public class OMREngine : IEngine
    {
        public ProcessedDataEntry ProcessSheet(ConfigurationBase configuration, Mat sheetMat, Action<RectangleF, bool> OnOptionProcessed = null, string originalSheetPath = "")
        {
            OMRConfiguration omrConfiguration = (OMRConfiguration)configuration;

            OMRRegionData regionData = omrConfiguration.RegionData;
            int totalFields = regionData.TotalFields;
            int totalOptions = regionData.TotalOptions;
            int instances = regionData.TotalInstances == 0 ? 1 : regionData.TotalInstances; 

            ProcessedDataType[] processedDataResultsType = new ProcessedDataType[(totalFields*instances)];

            PointF regionLocation = omrConfiguration.GetConfigArea.ConfigRect.Location;
            List<RectangleF> optionsRects = regionData.GetOptionsRects;

            char muliMarkSymbol = omrConfiguration.MultiMarkSymbol;
            MultiMarkAction multiMarkAction = omrConfiguration.MultiMarkAction;
            char noneMarkedSymbol = omrConfiguration.NoneMarkedSymbol;
            NoneMarkedAction noneMarkedAction = omrConfiguration.NoneMarkedAction;

            char[] regionFieldsOutputs = new char[totalFields * instances];
            string regionOutput = "";

            int curOptionRectIndex = 0;
            byte[,] optionsOutputs = new byte[totalFields * instances, totalOptions];

            double curBlackCountThreshold = omrConfiguration.BlackCountThreshold;
            int curFieldOutputIndex = 0;
            for (int i0 = 0; i0 < instances; i0++)
            {
                for (int i = 0; i < totalFields; i++)
                {
                    char curFieldOutput = omrConfiguration.ValueDataType == ValueDataType.Alphabet? 'O' : '0';
                    byte totalFilled = 0;
                    List<byte> filledIndexes = new List<byte>();
                    double averageBlackCountPercent = 0.15;

                    //for (int k = 0; k < totalOptions; k++)
                    //{
                    //    Rectangle curOptionRect = Rectangle.Round(optionsRects[curOptionRectIndex]);
                    //    curOptionRect.X += (int)regionLocation.X;
                    //    curOptionRect.Y += (int)regionLocation.Y;

                    //    Mat curOption = new Mat(sheetMat, curOptionRect);
                    //    var data = curOption.GetRawData();

                        
                    //    int blackCount = data.Count(x => x < 180);
                    //}

                    for (int j = 0; j < totalOptions; j++)
                    {
                        Rectangle curOptionRect = Rectangle.Round(optionsRects[curOptionRectIndex]);
                        curOptionRect.X += (int)regionLocation.X;
                        curOptionRect.Y += (int)regionLocation.Y;

                        Mat curOption = new Mat(sheetMat, curOptionRect);
                        var data = curOption.GetRawData();
                        byte minValue = data.Min();
                        int blackCount = data.Count(x => x < 180);
                        double blackCountPercent = blackCount / (double)data.Length;
                        bool isFilled = blackCountPercent > curBlackCountThreshold;
                        if (isFilled)
                        {
                            optionsOutputs[curFieldOutputIndex, j] = 1;
                            totalFilled++;

                            filledIndexes.Add((byte)j);
                        }
                        OnOptionProcessed?.Invoke(optionsRects[curOptionRectIndex], isFilled);

                        curOptionRectIndex++;
                    }

                    if (totalFilled > 0)
                    {
                        if (totalFilled > 1)
                        {
                            switch (multiMarkAction)
                            {
                                case MultiMarkAction.MarkAsManual:
                                    curFieldOutput = muliMarkSymbol;
                                    processedDataResultsType[curFieldOutputIndex] = ProcessedDataType.MANUAL;
                                    break;
                                case MultiMarkAction.ConsiderFirst:
                                    curFieldOutput = (char)(filledIndexes[0] + 65);
                                    break;
                                case MultiMarkAction.ConsiderLast:
                                    curFieldOutput = (char)(filledIndexes[filledIndexes.Count-1] + 65);
                                    break;
                                case MultiMarkAction.ConsiderCorrect:
                                    curFieldOutput = OMRConfiguration.CONSIDER_CORRECT_SYMBOL;
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
                                processedDataResultsType[curFieldOutputIndex] = ProcessedDataType.MANUAL;
                                break;
                            case NoneMarkedAction.Invalidate:
                                curFieldOutput = noneMarkedSymbol;
                                break;
                        }
                    }

                    regionFieldsOutputs[curFieldOutputIndex] = curFieldOutput;
                    regionOutput += curFieldOutput.ToString();

                    curFieldOutputIndex++;
                }
            }

            ProcessedDataEntry processedDataEntry = new ProcessedDataEntry(configuration.Title, regionFieldsOutputs, processedDataResultsType, optionsOutputs);
            return processedDataEntry;
        }

        public async Task<ProcessedDataEntry> ProcessSheetRaw(ConfigurationBase configuration, Mat sheetMat, Action<RectangleF, bool> OnOptionProcessed = null, Func<double> GetWaitMS = null)
        {
            OMRConfiguration omrConfiguration = (OMRConfiguration)configuration;

            OMRRegionData regionData = omrConfiguration.RegionData;
            int totalFields = regionData.TotalFields;
            int totalOptions = regionData.TotalOptions;

            ProcessedDataType[] processedDataResultsType = new ProcessedDataType[totalFields];

            PointF regionLocation = omrConfiguration.GetConfigArea.ConfigRect.Location;
            List<RectangleF> optionsRects = regionData.GetOptionsRects;

            char muliMarkSymbol = omrConfiguration.MultiMarkSymbol;
            MultiMarkAction multiMarkAction = omrConfiguration.MultiMarkAction;
            char noneMarkedSymbol = omrConfiguration.NoneMarkedSymbol;
            NoneMarkedAction noneMarkedAction = omrConfiguration.NoneMarkedAction;

            char[] regionFieldsOutputs = new char[totalFields];
            string regionOutput = "";

            int curOptionRectIndex = 0;
            byte[,] optionsOutputs = new byte[totalFields, totalOptions];

            double curBlackCountThreshold = omrConfiguration.BlackCountThreshold;
            for (int i = 0; i < totalFields; i++)
            {
                char curFieldOutput = '0';
                byte totalFilled = 0;
                List<byte> filledIndexes = new List<byte>();

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
                    {
                        optionsOutputs[i,j] = 1;
                        totalFilled++;

                        filledIndexes.Add((byte)j);
                    }
                    OnOptionProcessed?.Invoke(curOptionRect, isFilled);
                    if (OnOptionProcessed != null)
                        await Task.Delay(TimeSpan.FromMilliseconds(GetWaitMS()));

                    curOptionRectIndex++;
                }

                if (totalFilled > 0)
                {
                    if (totalFilled > 1)
                    {
                        switch (multiMarkAction)
                        {
                            case MultiMarkAction.MarkAsManual:
                                curFieldOutput = muliMarkSymbol;
                                processedDataResultsType[i] = ProcessedDataType.MANUAL;
                                break;
                            case MultiMarkAction.ConsiderFirst:
                                curFieldOutput = (char)(optionsOutputs[i, filledIndexes[0]] + 65);
                                break;
                            case MultiMarkAction.ConsiderLast:
                                curFieldOutput = (char)(optionsOutputs[i, filledIndexes[filledIndexes.Count-1]] + 65);
                                break;
                            case MultiMarkAction.ConsiderCorrect:
                                curFieldOutput = OMRConfiguration.CONSIDER_CORRECT_SYMBOL;
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
                                curFieldOutput = (char)(optionsOutputs[i, filledIndexes[0]] + 65);
                                break;
                            case ValueDataType.WholeNumber:
                                curFieldOutput = (char)(optionsOutputs[i, filledIndexes[0]] + 48);
                                break;
                            case ValueDataType.NaturalNumber:
                                curFieldOutput = (char)(optionsOutputs[i, filledIndexes[0]] + 49);
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
                            processedDataResultsType[i] = ProcessedDataType.MANUAL;
                            break;
                        case NoneMarkedAction.Invalidate:
                            curFieldOutput = noneMarkedSymbol;
                            break;
                    }
                }

                regionFieldsOutputs[i] = curFieldOutput;
                regionOutput += curFieldOutput.ToString();
            }

            ProcessedDataEntry processedDataEntry = new ProcessedDataEntry(configuration.Title, regionFieldsOutputs, processedDataResultsType, optionsOutputs);
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
        public static (int totalMarks, int obtainedMarks) GradeSheet(AnswerKey answerKey, byte[,] observedOptions, MultiMarkAction multiMarkAction)
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
                    if (observedOptions[i, j] == 1)
                    {
                        if (isCorrect)
                        {
                            if (multiMarkAction == MultiMarkAction.MarkAsManual || multiMarkAction == MultiMarkAction.Invalidate)
                                isCorrect = false;
                            else if (multiMarkAction == MultiMarkAction.ConsiderCorrect)
                                isCorrect = true;
                            else if(multiMarkAction == MultiMarkAction.ConsiderLast)
                                isCorrect = observedOptions[i, j] == keyOptions[i][j]? true : false;
                        }
                        else
                        {
                            if (observedOptions[i, j] == keyOptions[i][j])
                                isCorrect = true;
                        }
                    }
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
