namespace Synapse.Core.Engines
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Threading.Tasks;

    using Emgu.CV;

    using Synapse.Core.Configurations;
    using Synapse.Core.Engines.Data;
    using Synapse.Core.Engines.Interface;
    using Synapse.Core.Keys;

    public class OMREngine : IEngine
    {
        public ProcessedDataEntry ProcessSheet(ConfigurationBase configuration, Mat sheetMat,
            Action<RectangleF, bool> OnOptionProcessed = null, string originalSheetPath = "")
        {
            var omrConfiguration = (OMRConfiguration)configuration;

            var regionData = omrConfiguration.RegionData;
            var totalFields = regionData.TotalFields;
            var totalOptions = regionData.TotalOptions;
            var instances = regionData.TotalInstances == 0 ? 1 : regionData.TotalInstances;

            var processedDataResultsType = new ProcessedDataType[totalFields * instances];

            var regionLocation = omrConfiguration.GetConfigArea.ConfigRect.Location;
            var optionsRects = regionData.GetOptionsRects;

            var multiMarkSymbol = omrConfiguration.MultiMarkSymbol;
            var multiMarkAction = omrConfiguration.MultiMarkAction;
            var noneMarkedSymbol = omrConfiguration.NoneMarkedSymbol;
            var noneMarkedAction = omrConfiguration.NoneMarkedAction;

            var regionFieldsOutputs = new char[totalFields * instances];
            var regionOutput = "";

            var curOptionRectIndex = 0;
            var optionsOutputs = new byte[totalFields * instances, totalOptions];

            var curBlackCountThreshold = omrConfiguration.BlackCountThreshold;
            var curFieldOutputIndex = 0;
            for (var i0 = 0; i0 < instances; i0++)
            for (var i = 0; i < totalFields; i++)
            {
                var curFieldOutput = omrConfiguration.ValueDataType == ValueDataType.Alphabet ? 'O' : '0';
                byte totalFilled = 0;
                var filledIndexes = new List<byte>();
                var averageBlackCountPercent = 0.15;

                //for (int k = 0; k < totalOptions; k++)
                //{
                //    Rectangle curOptionRect = Rectangle.Round(optionsRects[curOptionRectIndex]);
                //    curOptionRect.X += (int)regionLocation.X;
                //    curOptionRect.Y += (int)regionLocation.Y;

                //    Mat curOption = new Mat(sheetMat, curOptionRect);
                //    var data = curOption.GetRawData();


                //    int blackCount = data.Count(x => x < 180);
                //}

                for (var j = 0; j < totalOptions; j++)
                {
                    var curOptionRect = Rectangle.Round(optionsRects[curOptionRectIndex]);
                    curOptionRect.X += (int)regionLocation.X;
                    curOptionRect.Y += (int)regionLocation.Y;

                    var curOption = new Mat(sheetMat, curOptionRect);
                    var data = curOption.GetRawData();
                    var minValue = data.Min();
                    var blackCount = data.Count(x => x < 180);
                    var blackCountPercent = blackCount / (double)data.Length;
                    var isFilled = blackCountPercent > curBlackCountThreshold;
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
                                curFieldOutput = multiMarkSymbol;
                                processedDataResultsType[curFieldOutputIndex] = ProcessedDataType.MANUAL;
                                break;

                            case MultiMarkAction.ConsiderFirst:
                                curFieldOutput = (char)(filledIndexes[0] + 65);
                                break;

                            case MultiMarkAction.ConsiderLast:
                                curFieldOutput = (char)(filledIndexes[filledIndexes.Count - 1] + 65);
                                break;

                            case MultiMarkAction.ConsiderCorrect:
                                curFieldOutput = OMRConfiguration.CONSIDER_CORRECT_SYMBOL;
                                break;

                            case MultiMarkAction.Invalidate:
                                curFieldOutput = multiMarkSymbol;
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

            var title = !string.IsNullOrEmpty(configuration.ParentTitle)
                ? configuration.ParentTitle
                : configuration.Title;

            var processedDataEntry = new ProcessedDataEntry(title, regionFieldsOutputs,
                processedDataResultsType, optionsOutputs, actualConfigurationTitle: configuration.Title);
            return processedDataEntry;
        }

        public async Task<ProcessedDataEntry> ProcessSheetRaw(ConfigurationBase configuration, Mat sheetMat,
            Action<RectangleF, bool> OnOptionProcessed = null, Func<double> GetWaitMS = null)
        {
            var omrConfiguration = (OMRConfiguration)configuration;

            var regionData = omrConfiguration.RegionData;
            var totalFields = regionData.TotalFields;
            var totalOptions = regionData.TotalOptions;

            var processedDataResultsType = new ProcessedDataType[totalFields];

            var regionLocation = omrConfiguration.GetConfigArea.ConfigRect.Location;
            var optionsRects = regionData.GetOptionsRects;

            var muliMarkSymbol = omrConfiguration.MultiMarkSymbol;
            var multiMarkAction = omrConfiguration.MultiMarkAction;
            var noneMarkedSymbol = omrConfiguration.NoneMarkedSymbol;
            var noneMarkedAction = omrConfiguration.NoneMarkedAction;

            var regionFieldsOutputs = new char[totalFields];
            var regionOutput = "";

            var curOptionRectIndex = 0;
            var optionsOutputs = new byte[totalFields, totalOptions];

            var curBlackCountThreshold = omrConfiguration.BlackCountThreshold;
            for (var i = 0; i < totalFields; i++)
            {
                var curFieldOutput = '0';
                byte totalFilled = 0;
                var filledIndexes = new List<byte>();

                for (var j = 0; j < totalOptions; j++)
                {
                    var curOptionRect = Rectangle.Round(optionsRects[curOptionRectIndex]);
                    //curOptionRect = Rectangle.Round(optionsRects[(j == 0 && i == 0)? 0 : (j == totalOptions-1 && i == totalFields-1)? optionsRects.Count - 1 : curOptionRectIndex+1]);
                    curOptionRect.X += (int)regionLocation.X;
                    curOptionRect.Y += (int)regionLocation.Y;

                    var curOption = new Mat(sheetMat, curOptionRect);
                    var data = curOption.GetRawData();
                    var blackCount = data.Count(x => x < 150);
                    var blackCountPercent = blackCount / (double)data.Length;
                    var isFilled = blackCountPercent > curBlackCountThreshold;
                    if (isFilled)
                    {
                        optionsOutputs[i, j] = 1;
                        totalFilled++;

                        filledIndexes.Add((byte)j);
                    }

                    OnOptionProcessed?.Invoke(curOptionRect, isFilled);
                    if (OnOptionProcessed != null)
                    {
                        await Task.Delay(TimeSpan.FromMilliseconds(GetWaitMS()));
                    }

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
                                curFieldOutput = (char)(optionsOutputs[i, filledIndexes[filledIndexes.Count - 1]] + 65);
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

            var title = !string.IsNullOrEmpty(configuration.ParentTitle)
                ? configuration.ParentTitle
                : configuration.Title;

            var processedDataEntry = new ProcessedDataEntry(title, regionFieldsOutputs,
                processedDataResultsType, optionsOutputs, actualConfigurationTitle: configuration.Title);
            return processedDataEntry;
        }

        public static (int totalMarks, int obtainedMarks) GradeSheet(AnswerKey answerKey, byte[][] observedOptions)
        {
            int totalMarks = 0, obtainedMarks = 0;

            var correctOptionValue = answerKey.GetPaper.GetCorrectOptionValue;
            var wrongOptionValue = answerKey.GetPaper.GetWrongOptionValue;
            var keyOptions = answerKey.GetKey;
            var totalFields = keyOptions.Length;
            var totalOptions = keyOptions[0].Length;

            totalMarks = correctOptionValue * totalFields;
            for (var i = 0; i < totalFields; i++)
            {
                var isCorrect = false;
                for (var j = 0; j < totalOptions; j++)
                    if (observedOptions[i][j] == 1 && observedOptions[i][j] == keyOptions[i][j])
                    {
                        isCorrect = true;
                    }

                if (isCorrect)
                {
                    obtainedMarks += correctOptionValue;
                }
                else
                {
                    obtainedMarks -= wrongOptionValue;
                }
            }

            return (totalMarks, obtainedMarks);
        }

        public static (int totalMarks, int obtainedMarks) GradeSheet(AnswerKey answerKey, byte[,] observedOptions,
            MultiMarkAction multiMarkAction)
        {
            int totalMarks = 0, obtainedMarks = 0;

            var correctOptionValue = answerKey.GetPaper.GetCorrectOptionValue;
            var wrongOptionValue = answerKey.GetPaper.GetWrongOptionValue;
            var keyOptions = answerKey.GetKey;
            var totalFields = keyOptions.Length;
            var totalOptions = keyOptions[0].Length;

            totalMarks = correctOptionValue * totalFields;
            for (var i = 0; i < totalFields; i++)
            {
                var isCorrect = false;

                for (var j = 0; j < totalOptions; j++)
                    if (observedOptions[i, j] == 1)
                    {
                        if (isCorrect)
                        {
                            if (multiMarkAction == MultiMarkAction.MarkAsManual ||
                                multiMarkAction == MultiMarkAction.Invalidate)
                            {
                                isCorrect = false;
                            }
                            else if (multiMarkAction == MultiMarkAction.ConsiderCorrect)
                            {
                                isCorrect = true;
                            }
                            else if (multiMarkAction == MultiMarkAction.ConsiderLast)
                            {
                                isCorrect = observedOptions[i, j] == keyOptions[i][j] ? true : false;
                            }
                        }
                        else
                        {
                            if (observedOptions[i, j] == keyOptions[i][j])
                            {
                                isCorrect = true;
                            }
                        }
                    }

                if (isCorrect)
                {
                    obtainedMarks += correctOptionValue;
                }
                else
                {
                    obtainedMarks -= wrongOptionValue;
                }
            }

            return (totalMarks, obtainedMarks);
        }
    }
}