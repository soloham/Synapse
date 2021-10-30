namespace Synapse.Core.Engines.Data
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;

    using Synapse.Shared.Enums;
    using Synapse.Utilities.Attributes;

    [Serializable]
    public enum ProcessedDataType
    {
        [EnumDescription("Normal")] NORMAL = 0,
        [EnumDescription("Manual")] MANUAL = 1,
        [EnumDescription("Faulty")] FAULTY = 2,
        [EnumDescription("Incompatible")] INCOMPATIBLE = 3
    }

    [Serializable]
    public struct ProcessedDataRow
    {
        #region Properties

        public int GetRowIndex { get; }

        public string RowSheetPath { get; set; }

        //public Templates.Template.AlignmentPipelineResults GetAlignmentPipelineResults { get => alignmentPipelineResults?? null; }
        //private Templates.Template.AlignmentPipelineResults alignmentPipelineResults;
        public ProcessedDataType DataRowResultType { get; set; }
        public List<ProcessedDataEntry> GetProcessedDataEntries { get; }

        public bool IsEdited { get; set; }
        public ProcessingEnums.RereadType RereadType { get; set; }

        #endregion

        #region

        #endregion

        #region Methods

        public ProcessedDataRow(List<ProcessedDataEntry> processedDataEntries, int rowIndex, string rowSheetPath,
            ProcessedDataType processedDataResultType) //, Templates.Template.AlignmentPipelineResults alignmentPipelineResults)
        {
            this.GetProcessedDataEntries = processedDataEntries;
            this.GetRowIndex = rowIndex;
            this.RowSheetPath = rowSheetPath;
            this.DataRowResultType = processedDataResultType;
            this.IsEdited = false;
            this.RereadType = ProcessingEnums.RereadType.NORMAL;
            //this.alignmentPipelineResults = alignmentPipelineResults;
        }

        public PointF[] GetInvAignedPointsF(RectangleF input)
        {
            var inputRectFCoordinates = new[]
            {
                input.Location, new PointF(input.X + input.Width, input.Y),
                new PointF(input.X + input.Width, input.Y + input.Height), new PointF(input.X, input.Y + input.Height)
            };
            var outputRectFCoordinates = new PointF[0];

            //outputRectFCoordinates = CvInvoke.PerspectiveTransform(inputRectFCoordinates, GetAlignmentHomography);
            return outputRectFCoordinates;
        }

        #endregion
    }
}