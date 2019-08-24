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
        internal AlignmentPipelineResults.AlignmentMethodResult GetAlignmentMethodResult { get => alignmentMethodResult; }
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
        internal AlignmentMethodResultControl(AlignmentPipelineResults.AlignmentMethodResult alignmentMethodResult)
        {
            InitializeComponent();
            Awake();
            this.alignmentMethodResult = alignmentMethodResult;

            Initialize(alignmentMethodResult);
        }

        private void Awake()
        {
            synchronizationContext = SynchronizationContext.Current;

            resultsDockingManager.SetEnableDocking(resultImageBoxPanel, true);
            resultsDockingManager.DockControlInAutoHideMode(resultImageBoxPanel, DockingStyle.Right, 300);
            resultsDockingManager.SetMenuButtonVisibility(resultImageBoxPanel, false);
            resultsDockingManager.SetDockLabel(resultImageBoxPanel, "Result Image");

            resultsDockingManager.SetEnableDocking(originalImageBoxPanel, true);
            resultsDockingManager.DockControlInAutoHideMode(originalImageBoxPanel, DockingStyle.Right, 300);
            resultsDockingManager.SetMenuButtonVisibility(originalImageBoxPanel, false);
            resultsDockingManager.SetDockLabel(originalImageBoxPanel, "Original Image");

            resultsDockingManager.SetEnableDocking(differenceImageBoxPanel, true);
            resultsDockingManager.DockControlInAutoHideMode(differenceImageBoxPanel, DockingStyle.Right, 300);
            resultsDockingManager.SetMenuButtonVisibility(differenceImageBoxPanel, false);
            resultsDockingManager.SetDockLabel(differenceImageBoxPanel, "Difference Image");
        }

        private void Initialize(AlignmentPipelineResults.AlignmentMethodResult alignmentMethodResult)
        {
            var inputImage = alignmentMethodResult.InputImage;
            originalImageBox.Image = inputImage.Bitmap;
            var outputImage = alignmentMethodResult.OutputImage;
            resultImageBox.Image = outputImage.Bitmap;
            var diffImage = inputImage.Sub(outputImage);
            differenceImageBox.Image = diffImage.Bitmap;
        }
    }
}
