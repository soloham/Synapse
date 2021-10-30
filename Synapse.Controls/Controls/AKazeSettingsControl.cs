namespace Synapse.Controls
{
    using System;
    using System.Threading;
    using System.Windows.Forms;

    using Emgu.CV.Features2D;

    using Synapse.Core.Templates;
    using Synapse.Utilities.Enums;

    using Syncfusion.Windows.Forms.Tools;

    public partial class AKazeSettingsControl : UserControl
    {
        #region Events

        public delegate void SetDataDelegate(
            Template.RegistrationAlignmentMethod.AKazeRegistrationMethod.AKazeData aKazeData,
            bool useStoredModelFeatures, int pipelineIndex);

        public SetDataDelegate OnSetDataEvent;

        public event EventHandler<int> OnResetDataEvent;

        #endregion

        #region Properties

        private readonly Template.RegistrationAlignmentMethod.AKazeRegistrationMethod.AKazeData akazeData;

        #endregion

        #region Variables

        private readonly SynchronizationContext synchronizationContext;
        private readonly bool initialIsUseStoredModelFeaturesBool;
        private readonly int pipelineIndex;

        #endregion

        public AKazeSettingsControl()
        {
            this.InitializeComponent();
        }

        public AKazeSettingsControl(Template.RegistrationAlignmentMethod.AKazeRegistrationMethod.AKazeData akazeData,
            bool isUseStoredEnabled, int pipelineIndex)
        {
            this.InitializeComponent();

            synchronizationContext = SynchronizationContext.Current;
            this.akazeData = akazeData;
            initialIsUseStoredModelFeaturesBool = isUseStoredEnabled;
            this.pipelineIndex = pipelineIndex;

            akazeDiffTypeValueBox.DataSource = EnumHelper.ToList(typeof(KAZE.Diffusivity));
            akazeDiffTypeValueBox.DisplayMember = "Value";
            akazeDiffTypeValueBox.ValueMember = "Key";

            akazeDescTypeValueBox.DataSource = EnumHelper.ToList(typeof(AKAZE.DescriptorType));
            akazeDescTypeValueBox.DisplayMember = "Value";
            akazeDescTypeValueBox.ValueMember = "Key";

            this.InitializeAKazePanel(akazeData, isUseStoredEnabled);
        }

        public void InitializeAKazePanel(
            Template.RegistrationAlignmentMethod.AKazeRegistrationMethod.AKazeData aKazeData, bool isUseStoredEnabled)
        {
            synchronizationContext.Send(delegate
            {
                akazeDescTypeValueBox.SelectedIndex = (int)aKazeData.DescriptorType;
                akazeDescSizeValueBox.IntegerValue = aKazeData.DescriptorSize;
                akazeDescChannelsValueBox.IntegerValue = aKazeData.Channels;
                akazeDescThresholdValueBox.DoubleValue = aKazeData.Threshold;
                akazeOctavesValueBox.IntegerValue = aKazeData.Octaves;
                akazeLayersOptionValueBox.IntegerValue = aKazeData.Layers;
                akazeDiffTypeValueBox.SelectedValue = aKazeData.Diffusivity;
                akazeUseStoredModelFeaturesToggle.ToggleState =
                    isUseStoredEnabled ? ToggleButtonState.Active : ToggleButtonState.Inactive;
            }, null);
        }

        private Template.RegistrationAlignmentMethod.AKazeRegistrationMethod.AKazeData GetAKazeData()
        {
            var descType = AKAZE.DescriptorType.Kaze;
            var diffType = KAZE.Diffusivity.PmG2;
            var descSize = 0;
            var descChannels = 3;
            var descThresh = 0.001f;
            var descOcts = 4;
            var descLayers = 4;

            synchronizationContext.Send(delegate
            {
                descType = (AKAZE.DescriptorType)akazeDescTypeValueBox.SelectedIndex;
                descSize = (int)akazeDescSizeValueBox.IntegerValue;
                descChannels = (int)akazeDescChannelsValueBox.IntegerValue;
                descThresh = (float)akazeDescThresholdValueBox.DoubleValue;
                descOcts = (int)akazeOctavesValueBox.IntegerValue;
                descLayers = (int)akazeLayersOptionValueBox.IntegerValue;
                diffType = (KAZE.Diffusivity)akazeDiffTypeValueBox.SelectedIndex;
            }, null);

            var aKazeData = new Template.RegistrationAlignmentMethod.AKazeRegistrationMethod.AKazeData(descType,
                descSize, descChannels, descThresh, descOcts, descLayers, diffType);
            return aKazeData;
        }

        private void Reset()
        {
            this.InitializeAKazePanel(akazeData, initialIsUseStoredModelFeaturesBool);
        }

        private void SetBtn_Click(object sender, EventArgs e)
        {
            OnSetDataEvent?.Invoke(this.GetAKazeData(),
                akazeUseStoredModelFeaturesToggle.ToggleState == ToggleButtonState.Active, pipelineIndex);
        }

        private void ResetBtn_Click(object sender, EventArgs e)
        {
            this.Reset();
            this.OnResetDataEvent?.Invoke(this, pipelineIndex);
        }
    }
}