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
using Emgu.CV.Features2D;
using Emgu.CV.Structure;
using Synapse.Controls;
using Synapse.Core.Templates;
using Synapse.Utilities;
using Synapse.Utilities.Attributes;
using Synapse.Utilities.Enums;
using Synapse.Utilities.Memory;
using Syncfusion.Windows.Forms.Tools;
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

            kazeDiffTypeValueBox.DataSource = EnumHelper.ToList(typeof(KAZE.Diffusivity));
            kazeDiffTypeValueBox.DisplayMember = "Value";
            kazeDiffTypeValueBox.ValueMember = "Key";

            akazeDiffTypeValueBox.DataSource = EnumHelper.ToList(typeof(KAZE.Diffusivity));
            akazeDiffTypeValueBox.DisplayMember = "Value";
            akazeDiffTypeValueBox.ValueMember = "Key";

            akazeDescTypeValueBox.DataSource = EnumHelper.ToList(typeof(AKAZE.DescriptorType));
            akazeDescTypeValueBox.DisplayMember = "Value";
            akazeDescTypeValueBox.ValueMember = "Key";
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

        private KAZE GetKAZE()
        {
            bool extended = false;
            bool upright = false;
            float descThresh = 0.001f;
            int descOcts = 1;
            int descSbls = 4;
            var diffType = KAZE.Diffusivity.PmG2;

            synchronizationContext.Send(new SendOrPostCallback(
            delegate (object state)
            {
                extended = kazeExtendedToggle.ToggleState == ToggleButtonState.Active;
                upright = kazeUprightToggle.ToggleState == ToggleButtonState.Active;
                descThresh = (float)kazeThresholdValueBox.DoubleValue;
                descOcts = (int)kazeOctavesValueBox.IntegerValue;
                descSbls = (int)kazeSublvlsValueBox.IntegerValue;
                diffType = (KAZE.Diffusivity)kazeDiffTypeValueBox.SelectedIndex;
            }
            ), null);

            KAZE kaze = new KAZE(extended, upright, descThresh, descOcts, descSbls, diffType);
            return kaze;
        }
        private AKAZE GetAKAZE()
        {
            var descType = AKAZE.DescriptorType.Kaze;
            var diffType = KAZE.Diffusivity.PmG2;
            int descSize = 0;
            int descChannels = 3;
            float descThresh = 0.001f;
            int descOcts = 4;
            int descLayers = 4;

            synchronizationContext.Send(new SendOrPostCallback(
            delegate (object state)
            {
                descType = (AKAZE.DescriptorType)akazeDescTypeValueBox.SelectedIndex;
                descSize = (int)akazeDescSizeValueBox.IntegerValue;
                descChannels = (int)akazeDescChannelsValueBox.IntegerValue;
                descThresh = (float)akazeDescThresholdValueBox.DoubleValue;
                descOcts = (int)akazeOctavesValueBox.IntegerValue;
                descLayers = (int)akazeLayersOptionValueBox.IntegerValue;
                diffType = (KAZE.Diffusivity)akazeDiffTypeValueBox.SelectedIndex;
            }
            ), null);

            AKAZE akaze = new AKAZE(descType, descSize, descChannels, descThresh, descOcts, descLayers, diffType);
            return akaze;
        }

        #region Configuration Controls
        private void DoneBtn_Click(object sender, EventArgs e)
        {

        }
        #endregion

        #endregion
    }
}