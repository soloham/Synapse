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
        #region Properties
        internal AlignmentPipelineResults GetPipelineResults { get => pipelineResults; }
        private AlignmentPipelineResults pipelineResults; 
        #endregion

        #region Variables
        private SynchronizationContext synchronizationContext;
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

        private void Initialize(AlignmentPipelineResults pipelineResults)
        {
            for (int i = 0; i < pipelineResults.AlignmentMethodTestResultsList.Count; i++)
            {
                AlignmentPipelineResults.AlignmentMethodResult alignmentMethodResult = pipelineResults.AlignmentMethodTestResultsList[i];

                switch (alignmentMethodResult.GetAlignmentMethodType)
                {
                    case AlignmentMethodType.Anchors:
                        AlignmentPipelineResults.AnchorAlignmentMethodResult anchorAlignmentMethodResult = (AlignmentPipelineResults.AnchorAlignmentMethodResult)alignmentMethodResult;
                        AnchorAlignmentMethodResultControl anchorAlignmentMethodResultControl = new AnchorAlignmentMethodResultControl(anchorAlignmentMethodResult);

                        TabPageAdv anchorTabPageAdv = new TabPageAdv(anchorAlignmentMethodResult.AlignmentMethod.MethodName);
                        anchorTabPageAdv.Controls.Add(anchorAlignmentMethodResultControl);
                        anchorAlignmentMethodResultControl.Dock = DockStyle.Fill;
                        pipelineResultsMainTabControl.TabPages.Add(anchorTabPageAdv);
                        break;
                    case AlignmentMethodType.Registration:
                        AlignmentPipelineResults.RegistrationAlignmentMethodResult registrationAlignmentMethodResult = (AlignmentPipelineResults.RegistrationAlignmentMethodResult)alignmentMethodResult;
                        RegistrationAlignmentMethodResultControl registrationAlignmentMethodResultControl = new RegistrationAlignmentMethodResultControl(registrationAlignmentMethodResult);

                        TabPageAdv registrationTabPageAdv = new TabPageAdv(registrationAlignmentMethodResult.AlignmentMethod.MethodName);
                        registrationTabPageAdv.Controls.Add(registrationAlignmentMethodResultControl);
                        registrationAlignmentMethodResultControl.Dock = DockStyle.Fill;
                        pipelineResultsMainTabControl.TabPages.Add(registrationTabPageAdv);
                        break;
                }
            }
        }
    }
}
