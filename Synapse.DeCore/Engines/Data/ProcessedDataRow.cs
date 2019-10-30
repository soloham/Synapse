using Emgu.CV;
using Synapse.Utilities.Attributes;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Synapse.Core.Engines.Data
{
    [Serializable]
    public enum ProcessedDataType
    {
        [EnumDescription("Normal")]
        NORMAL = 0,
        [EnumDescription("Manual")]
        MANUAL = 1,
        [EnumDescription("Faulty")]
        FAULTY = 2,
        [EnumDescription("Incompatible")]
        INCOMPATIBLE = 3,
    }

    [Serializable]
    public struct ProcessedDataRow
    {
        #region Properties
        public int GetRowIndex { get => rowIndex; }
        private int rowIndex;
        public string RowSheetPath { get; set; }
        //public Templates.Template.AlignmentPipelineResults GetAlignmentPipelineResults { get => alignmentPipelineResults?? null; }
        //private Templates.Template.AlignmentPipelineResults alignmentPipelineResults;
        public ProcessedDataType DataRowResultType { get; set; }
        public List<ProcessedDataEntry> GetProcessedDataEntries { get => processedDataEntries; }
        private List<ProcessedDataEntry> processedDataEntries;

        public bool IsEdited { get; set; }
        public Shared.Enums.ProcessingEnums.RereadType RereadType { get; set; }
        #endregion

        #region

        #endregion
        #region Methods
        public ProcessedDataRow(List<ProcessedDataEntry> processedDataEntries, int rowIndex, string rowSheetPath, ProcessedDataType processedDataResultType)//, Templates.Template.AlignmentPipelineResults alignmentPipelineResults)
        {
            this.processedDataEntries = processedDataEntries;
            this.rowIndex = rowIndex;
            RowSheetPath = rowSheetPath;
            DataRowResultType = processedDataResultType;
            IsEdited = false;
            RereadType = Shared.Enums.ProcessingEnums.RereadType.NORMAL;
            //this.alignmentPipelineResults = alignmentPipelineResults;
        }
        public PointF[] GetInvAignedPointsF(RectangleF input)
        {
            PointF[] inputRectFCoordinates = new PointF[] { input.Location, new PointF(input.X + input.Width, input.Y), new PointF(input.X + input.Width, input.Y + input.Height), new PointF(input.X, input.Y + input.Height) };
            PointF[] outputRectFCoordinates = new PointF[0];

            //outputRectFCoordinates = CvInvoke.PerspectiveTransform(inputRectFCoordinates, GetAlignmentHomography);
            return outputRectFCoordinates;
        }
        #endregion
    }
}
