using Emgu.CV;
using Emgu.CV.Structure;
using Synapse.Utilities.Attributes;
using Syncfusion.WinForms.DataGrid;
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
        public Managers.ProcessingManager.RereadType RereadType { get; set; }
        #endregion

        #region Methods
        internal ProcessedDataRow(List<ProcessedDataEntry> processedDataEntries, int rowIndex, string rowSheetPath, ProcessedDataType processedDataResultType)//, Templates.Template.AlignmentPipelineResults alignmentPipelineResults)
        {
            this.processedDataEntries = processedDataEntries;
            this.rowIndex = rowIndex;
            RowSheetPath = rowSheetPath;
            DataRowResultType = processedDataResultType;
            IsEdited = false;
            RereadType = Managers.ProcessingManager.RereadType.NORMAL;
            //this.alignmentPipelineResults = alignmentPipelineResults;
        }
        internal bool GetAlignedImage(out Mat result)
        {
            result = new Mat();
            Mat unAlignedMat = new Mat(RowSheetPath, Emgu.CV.CvEnum.ImreadModes.Grayscale);
            switch (RereadType)
            {
                case Managers.ProcessingManager.RereadType.NORMAL:
                    break;
                case Managers.ProcessingManager.RereadType.ROTATE_C_90:
                    CvInvoke.Rotate(unAlignedMat, unAlignedMat, Emgu.CV.CvEnum.RotateFlags.Rotate90Clockwise);
                    break;
                case Managers.ProcessingManager.RereadType.ROTATE_180:
                    CvInvoke.Rotate(unAlignedMat, unAlignedMat, Emgu.CV.CvEnum.RotateFlags.Rotate180);
                    break;
                case Managers.ProcessingManager.RereadType.ROTATE_AC_90:
                    CvInvoke.Rotate(unAlignedMat, unAlignedMat, Emgu.CV.CvEnum.RotateFlags.Rotate90CounterClockwise);
                    break;
            }

            try
            {
                var resultImg = SynapseMain.GetCurrentTemplate.AlignSheet(unAlignedMat, out Templates.Template.AlignmentPipelineResults alignmentPipelineResults);
                result = resultImg;
                unAlignedMat.Dispose();
                //CvInvoke.WarpPerspective(unAligned, result, GetAlignmentHomography, unAligned.Size, Emgu.CV.CvEnum.Inter.Cubic, Emgu.CV.CvEnum.Warp.Default, Emgu.CV.CvEnum.BorderType.Default);
                return alignmentPipelineResults.AlignmentMethodTestResultsList[0].GetAlignmentMethodResultType == Templates.Template.AlignmentPipelineResults.AlignmentMethodResultType.Successful;
            }
            catch(Exception ex)
            {
                result = unAlignedMat;
                return false;
            }
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
