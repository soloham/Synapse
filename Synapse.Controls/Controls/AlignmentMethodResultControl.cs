using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Synapse.Core.Templates;
using System.Threading;
using Emgu.CV;
using Emgu.CV.Structure;
using static Synapse.Core.Templates.Template;
using Syncfusion.Windows.Forms.Tools;

namespace Synapse.Controls
{
    public partial class AlignmentMethodResultControl : UserControl
    {
        #region Properties
        public AlignmentPipelineResults.AlignmentMethodResult GetAlignmentMethodResult { get => alignmentMethodResult; }
        private AlignmentPipelineResults.AlignmentMethodResult alignmentMethodResult; 
        #endregion

        #region Variables
        private SynchronizationContext synchronizationContext;
        #endregion

        public AlignmentMethodResultControl()
        {
            InitializeComponent();
            synchronizationContext = SynchronizationContext.Current;
        }
        public AlignmentMethodResultControl(AlignmentPipelineResults.AlignmentMethodResult alignmentMethodResult)
        {
            InitializeComponent();
            Awake();

            this.alignmentMethodResult = alignmentMethodResult;
            alignmentTimeValueLabel.Text = $"{alignmentMethodResult.AlignmentTime}ms";
        }

        private void Awake()
        {
            synchronizationContext = SynchronizationContext.Current;
            alignmentTimeValueLabel.Text = "0ms";// $"{alignmentMethodResult.AlignmentTime}ms";
        }

        public void GetResultImages(out Mat inputImage, out Mat outputImage, out Mat diffImage)
        {
            inputImage = alignmentMethodResult.InputImage;
            outputImage = alignmentMethodResult.OutputImage;
            diffImage = new Mat();
            if (outputImage.Size != inputImage.Size)
            {
                Mat resizedOutputImg = new Mat();
                CvInvoke.Resize(outputImage, resizedOutputImg, inputImage.Size);
                CvInvoke.Subtract(inputImage, resizedOutputImg, diffImage);
                //var resizedOutputImg = outputImage.Resize(inputImage.Size.Width, inputImage.Size.Height, Emgu.CV.CvEnum.Inter.Cubic);
                //diffImage = inputImage.ToImage<Gray, byte>().Sub(resizedOutputImg);
            }
            else
                CvInvoke.Subtract(inputImage, outputImage, diffImage);
        }
    }
}
