using static Synapse.Core.Templates.Template;

namespace Synapse.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Windows.Forms;

    using Emgu.CV;

    using Syncfusion.Windows.Forms.Tools;

    public partial class AlignmentPipelineResultsControl : UserControl
    {
        #region Events

        public delegate void OnSelectedMethodResultChanged(AlignmentMethodResultControl alignmentMethodResultControl,
            Mat inputImage, Mat outputImg, Mat diffImg);

        public event OnSelectedMethodResultChanged OnSelectedMethodResultChangedEvent;

        #endregion

        #region Properties

        public AlignmentPipelineResults GetPipelineResults { get; }

        #endregion

        #region Variables

        private SynchronizationContext synchronizationContext;

        private readonly List<AlignmentMethodResultControl> alignmentMethodResultControls =
            new List<AlignmentMethodResultControl>();

        #endregion

        public AlignmentPipelineResultsControl()
        {
            this.InitializeComponent();
            synchronizationContext = SynchronizationContext.Current;
        }

        public AlignmentPipelineResultsControl(AlignmentPipelineResults pipelineResults)
        {
            this.InitializeComponent();
            synchronizationContext = SynchronizationContext.Current;
            this.GetPipelineResults = pipelineResults;

            this.Initialize(pipelineResults);
        }

        public void Initialize(AlignmentPipelineResults pipelineResults)
        {
            if (pipelineResults == null || pipelineResults.AlignmentMethodTestResultsList.Count == 0)
            {
                return;
            }

            pipelineResultsMainTabControl.TabPages.Clear();
            for (var i = 0; i < pipelineResults.AlignmentMethodTestResultsList.Count; i++)
            {
                var alignmentMethodResult = pipelineResults.AlignmentMethodTestResultsList[i];
                var alignmentMethodResultControl = new AlignmentMethodResultControl(alignmentMethodResult);

                var methodTabPageAdv = new TabPageAdv(alignmentMethodResult.AlignmentMethod.MethodName);
                methodTabPageAdv.Controls.Add(alignmentMethodResultControl);
                alignmentMethodResultControl.Dock = DockStyle.Fill;
                pipelineResultsMainTabControl.TabPages.Add(methodTabPageAdv);

                alignmentMethodResultControls.Add(alignmentMethodResultControl);
            }

            var selectedAlignmentMethodResultControl =
                alignmentMethodResultControls[pipelineResultsMainTabControl.SelectedIndex];
            selectedAlignmentMethodResultControl.GetResultImages(out var inputImg, out var outputImg, out var diffImg);

            this.OnSelectedMethodResultChangedEvent?.Invoke(selectedAlignmentMethodResultControl, inputImg, outputImg,
                diffImg);
        }

        private void PipelineResultsMainTabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedAlignmentMethodResultControl =
                alignmentMethodResultControls[pipelineResultsMainTabControl.SelectedIndex];
            selectedAlignmentMethodResultControl.GetResultImages(out var inputImg, out var outputImg, out var diffImg);

            this.OnSelectedMethodResultChangedEvent?.Invoke(selectedAlignmentMethodResultControl, inputImg, outputImg,
                diffImg);
        }
    }
}