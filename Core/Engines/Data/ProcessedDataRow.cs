using Emgu.CV;
using Emgu.CV.Structure;
using Synapse.Utilities.Attributes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Synapse.Core.Engines.Data
{
    [Serializable]
    internal enum ProcessedDataType
    {
        [EnumDescription("Incompatible")]
        INCOMPATIBLE,
        [EnumDescription("Faulty")]
        FAULTY,
        [EnumDescription("Manual")]
        MANUAL,
        [EnumDescription("Normal")]
        NORMAL,
    }

    [Serializable]
    internal struct ProcessedDataRow
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
        #endregion

        #region Methods
        internal ProcessedDataRow(List<ProcessedDataEntry> processedDataEntries, int rowIndex, string rowSheetPath, ProcessedDataType processedDataResultType)//, Templates.Template.AlignmentPipelineResults alignmentPipelineResults)
        {
            this.processedDataEntries = processedDataEntries;
            this.rowIndex = rowIndex;
            RowSheetPath = rowSheetPath;
            DataRowResultType = processedDataResultType;
            IsEdited = false;
            //this.alignmentPipelineResults = alignmentPipelineResults;
        }
        internal Mat GetAlignedImage()
        {
            Mat result = new Mat();
            Mat unAlignedMat = new Mat(RowSheetPath, Emgu.CV.CvEnum.ImreadModes.AnyColor);

            var resultImg = SynapseMain.GetCurrentTemplate.AlignSheet(unAlignedMat, out Templates.Template.AlignmentPipelineResults alignmentPipelineResults);
            result = resultImg;
            //CvInvoke.WarpPerspective(unAligned, result, GetAlignmentHomography, unAligned.Size, Emgu.CV.CvEnum.Inter.Cubic, Emgu.CV.CvEnum.Warp.Default, Emgu.CV.CvEnum.BorderType.Default);
            return result;
        }
        internal PointF[] GetInvAignedPointsF(RectangleF input)
        {
            PointF[] inputRectFCoordinates = new PointF[] { input.Location, new PointF(input.X + input.Width, input.Y), new PointF(input.X + input.Width, input.Y + input.Height), new PointF(input.X, input.Y + input.Height) };
            PointF[] outputRectFCoordinates = new PointF[0];

            //outputRectFCoordinates = CvInvoke.PerspectiveTransform(inputRectFCoordinates, GetAlignmentHomography);
            return outputRectFCoordinates;
        }
        #endregion
    }
}
