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
using Synapse.Utilities;
using Syncfusion.Windows.Forms.Tools;

namespace Synapse.Controls
{
    public partial class AnchorAlignmentMethodResultControl : UserControl
    {
        #region Properties
        internal AlignmentPipelineResults.AnchorAlignmentMethodResult GetAnchorAlignmentMethodResult { get => anchorAlignmentMethodResult; }
        private AlignmentPipelineResults.AnchorAlignmentMethodResult anchorAlignmentMethodResult ; 
        #endregion

        #region Variables
        private SynchronizationContext synchronizationContext;
        #endregion

        public AnchorAlignmentMethodResultControl()
        {
            InitializeComponent();
            synchronizationContext = SynchronizationContext.Current;
        }
        internal AnchorAlignmentMethodResultControl(AlignmentPipelineResults.AnchorAlignmentMethodResult anchorAlignmentMethodResult)
        {
            InitializeComponent();
            Awake();

            this.anchorAlignmentMethodResult = anchorAlignmentMethodResult;

            Initialize(anchorAlignmentMethodResult);
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

        private void Initialize(AlignmentPipelineResults.AnchorAlignmentMethodResult anchorAlignmentMethodResult)
        {
            var inputImage = anchorAlignmentMethodResult.InputImage;
            originalImageBox.Image = inputImage.Bitmap;
            var outputImage = anchorAlignmentMethodResult.OutputImage;
            resultImageBox.Image = outputImage.Bitmap;
            if (outputImage.Size != inputImage.Size)
                outputImage = outputImage.Resize(inputImage.Size.Width, inputImage.Size.Height, Emgu.CV.CvEnum.Inter.Cubic);
            var diffImage = inputImage.Sub(outputImage);
            differenceImageBox.Image = diffImage.Bitmap;
        }

        private void ResultImageBox_Paint(object sender, PaintEventArgs e)
        {
            if (resultImageBox.Image == null)
                return;

            var detectedAnchors = anchorAlignmentMethodResult.DetectedAnchors;
            var warpedAnchors = anchorAlignmentMethodResult.WarpedAnchors;
            var mainAnchors = anchorAlignmentMethodResult.MainAnchors;

            for (int i0 = 0; i0 < detectedAnchors.Length; i0++)
            {
                RectangleF detectedRect = detectedAnchors[i0];
                RectangleF warpedRect = warpedAnchors[i0];

                Graphics g = e.Graphics;

                Functions.DrawBox(g, resultImageBox.GetOffsetRectangle(mainAnchors[i0].GetAnchorRegion), resultImageBox.ZoomFactor, Color.FromArgb(100, Color.DodgerBlue), 2);
                //Functions.DrawBox(g, imageBox.GetOffsetRectangle(detectedRect), imageBox.ZoomFactor, Color.FromArgb(150, Color.Firebrick));
                Functions.DrawBox(g, resultImageBox.GetOffsetRectangle(warpedRect), resultImageBox.ZoomFactor, Color.FromArgb(120, Color.Crimson));
            }
        }
    }
}
