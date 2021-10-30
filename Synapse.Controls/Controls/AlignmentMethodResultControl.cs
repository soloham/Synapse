using static Synapse.Core.Templates.Template;

namespace Synapse.Controls
{
    using System.Threading;
    using System.Windows.Forms;

    using Emgu.CV;

    public partial class AlignmentMethodResultControl : UserControl
    {
        #region Properties

        public AlignmentPipelineResults.AlignmentMethodResult GetAlignmentMethodResult { get; }

        #endregion

        #region Variables

        private SynchronizationContext synchronizationContext;

        #endregion

        public AlignmentMethodResultControl()
        {
            this.InitializeComponent();
            synchronizationContext = SynchronizationContext.Current;
        }

        public AlignmentMethodResultControl(AlignmentPipelineResults.AlignmentMethodResult alignmentMethodResult)
        {
            this.InitializeComponent();
            this.Awake();

            this.GetAlignmentMethodResult = alignmentMethodResult;
            alignmentTimeValueLabel.Text = $"{alignmentMethodResult.AlignmentTime}ms";
        }

        private void Awake()
        {
            synchronizationContext = SynchronizationContext.Current;
            alignmentTimeValueLabel.Text = "0ms"; // $"{alignmentMethodResult.AlignmentTime}ms";
        }

        public void GetResultImages(out Mat inputImage, out Mat outputImage, out Mat diffImage)
        {
            inputImage = this.GetAlignmentMethodResult.InputImage;
            outputImage = this.GetAlignmentMethodResult.OutputImage;
            diffImage = new Mat();
            if (outputImage.Size != inputImage.Size)
            {
                var resizedOutputImg = new Mat();
                CvInvoke.Resize(outputImage, resizedOutputImg, inputImage.Size);
                CvInvoke.Subtract(inputImage, resizedOutputImg, diffImage);
                //var resizedOutputImg = outputImage.Resize(inputImage.Size.Width, inputImage.Size.Height, Emgu.CV.CvEnum.Inter.Cubic);
                //diffImage = inputImage.ToImage<Gray, byte>().Sub(resizedOutputImg);
            }
            else
            {
                CvInvoke.Subtract(inputImage, outputImage, diffImage);
            }
        }
    }
}