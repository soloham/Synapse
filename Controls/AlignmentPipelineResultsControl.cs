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
    public partial class AlignmentPipelineResultsControl : UserControl
    {
        #region Events
        internal delegate void OnSelectedMethodResultChanged(AlignmentMethodResultControl alignmentMethodResultControl, Image<Gray, byte> inputImage, Image<Gray, byte> outputImg, Image<Gray, byte> diffImg);
        internal event OnSelectedMethodResultChanged OnSelectedMethodResultChangedEvent;
        #endregion

        #region Properties
        internal AlignmentPipelineResults GetPipelineResults { get => pipelineResults; }
        private AlignmentPipelineResults pipelineResults; 
        #endregion

        #region Variables
        private SynchronizationContext synchronizationContext;
        private List<AlignmentMethodResultControl> alignmentMethodResultControls = new List<AlignmentMethodResultControl>();
        #endregion

        public AlignmentPipelineResultsControl()
        {
            InitializeComponent();
            synchronizationContext = SynchronizationContext.Current;
        }
        internal AlignmentPipelineResultsControl(AlignmentPipelineResults pipelineResults)
        {
            InitializeComponent();
            synchronizationContext = SynchronizationContext.Current;
            this.pipelineResults = pipelineResults;

            Initialize(pipelineResults);
        }

        internal void Initialize(AlignmentPipelineResults pipelineResults)
        {
            pipelineResultsMainTabControl.TabPages.Clear();
            for (int i = 0; i < pipelineResults.AlignmentMethodTestResultsList.Count; i++)
            {
                AlignmentPipelineResults.AlignmentMethodResult alignmentMethodResult = pipelineResults.AlignmentMethodTestResultsList[i];
                AlignmentMethodResultControl alignmentMethodResultControl = new AlignmentMethodResultControl(alignmentMethodResult);

                TabPageAdv methodTabPageAdv = new TabPageAdv(alignmentMethodResult.AlignmentMethod.MethodName);
                methodTabPageAdv.Controls.Add(alignmentMethodResultControl);
                alignmentMethodResultControl.Dock = DockStyle.Fill;
                pipelineResultsMainTabControl.TabPages.Add(methodTabPageAdv);

                alignmentMethodResultControls.Add(alignmentMethodResultControl);

            }

            var selectedAlignmentMethodResultControl = alignmentMethodResultControls[pipelineResultsMainTabControl.SelectedIndex];
            selectedAlignmentMethodResultControl.GetResultImages(out Image<Gray, byte> inputImg, out Image<Gray, byte> outputImg, out Image<Gray, byte> diffImg);

            OnSelectedMethodResultChangedEvent?.Invoke(selectedAlignmentMethodResultControl, inputImg, outputImg, diffImg);
        }

        private void PipelineResultsMainTabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedAlignmentMethodResultControl = alignmentMethodResultControls[pipelineResultsMainTabControl.SelectedIndex];
            selectedAlignmentMethodResultControl.GetResultImages(out Image<Gray, byte> inputImg, out Image<Gray, byte> outputImg, out Image<Gray, byte> diffImg);

            OnSelectedMethodResultChangedEvent?.Invoke(selectedAlignmentMethodResultControl, inputImg, outputImg, diffImg);
        }
    }
}
