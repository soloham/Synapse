using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;
using Synapse.Controls;
using Synapse.Core.Templates;
using Synapse.Utilities;
using Synapse.Utilities.Attributes;
using Synapse.Utilities.Enums;
using Synapse.Utilities.Memory;
using Syncfusion.WinForms.Controls;

namespace Synapse.Modules
{
    public partial class RegistrationAlignmentMethodForm : SfForm
    {
        #region Enums

        #endregion

        #region Properties

        #endregion

        #region Variables

        private Image<Gray, byte> templateImage;

        private int pipelineIndex;
        private string methodName;

        private SynchronizationContext synchronizationContext;

        #endregion

        #region Events

        internal delegate void OnConfigurationFinshed(Template.RegistrationAlignmentMethod registrationAlignmentMethod);
        internal event OnConfigurationFinshed OnConfigurationFinishedEvent;

        #endregion

        #region General Methods
        internal RegistrationAlignmentMethodForm(Template.RegistrationAlignmentMethod registrationAlignmentMethod, Image<Gray, byte> templateImage)
        {
            InitializeComponent();

            Awake();

            this.templateImage = templateImage;
            pipelineIndex = registrationAlignmentMethod.PipelineIndex;
            methodName = registrationAlignmentMethod.MethodName;

            Initialize(registrationAlignmentMethod);

        }
        internal RegistrationAlignmentMethodForm(Image<Gray, byte> templateImage, int pipelineIndex, string methodName = "Registration Method")
        {
            InitializeComponent();

            Awake();

            this.templateImage = templateImage;
            this.pipelineIndex = pipelineIndex;
            this.methodName = methodName;

            Initialize();

        }
        private void Awake()
        {
            synchronizationContext = SynchronizationContext.Current;
        }

        #endregion

        #region Private Methods

        private void Initialize(Template.RegistrationAlignmentMethod registrationAlignmentMethod)
        {
            SetupForConfigured(registrationAlignmentMethod);
        }
        private void Initialize()
        {
            SetupForConfiguration();
        }
        private void OnConfigurationFinishedCallback()
        {
        }

        private void SetupForConfigured(Template.RegistrationAlignmentMethod registrationAlignmentMethod)
        {
            
        }
        private void SetupForConfiguration()
        {

        }

        #region Configuration State
        private void ReconfigureBtn_Click(object sender, EventArgs e)
        {
            SetupForConfiguration();
        }

        private void SetBtn_Click(object sender, EventArgs e)
        {
        }
        private void DoneBtn_Click(object sender, EventArgs e)
        {

        }
        #endregion

        #endregion
    }
}