namespace Synapse.Modules
{
    using System;
    using System.Threading;

    using Emgu.CV;
    using Emgu.CV.Features2D;

    using Synapse.Core.Templates;
    using Synapse.Utilities.Enums;

    using Syncfusion.Windows.Forms.Tools;
    using Syncfusion.WinForms.Controls;

    public partial class RegistrationAlignmentMethodForm : SfForm
    {
        #region Enums

        #endregion

        #region Properties

        #endregion

        #region Variables

        private readonly Mat templateImage;

        private readonly int pipelineIndex;
        private readonly string methodName;

        private SynchronizationContext synchronizationContext;
        private Template.RegistrationAlignmentMethod selectedRegistrationAlignmentMethod;

        #endregion

        #region Events

        public delegate void OnConfigurationFinshed(Template.RegistrationAlignmentMethod registrationAlignmentMethod);

        public event OnConfigurationFinshed OnConfigurationFinishedEvent;

        #endregion

        #region General Methods

        public RegistrationAlignmentMethodForm(Template.RegistrationAlignmentMethod registrationAlignmentMethod,
            Mat templateImage)
        {
            this.InitializeComponent();

            this.Awake();

            this.templateImage = templateImage;
            pipelineIndex = registrationAlignmentMethod.PipelineIndex;
            methodName = registrationAlignmentMethod.MethodName;

            this.Initialize(registrationAlignmentMethod);
        }

        public RegistrationAlignmentMethodForm(Mat templateImage, int pipelineIndex,
            string methodName = "Registration Method")
        {
            this.InitializeComponent();

            this.Awake();

            this.templateImage = templateImage;
            this.pipelineIndex = pipelineIndex;
            this.methodName = methodName;

            this.Initialize();
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
            this.SetupForConfigured(registrationAlignmentMethod);
        }

        private void Initialize()
        {
            this.SetupForConfiguration();
        }

        private void SetupForConfigured(Template.RegistrationAlignmentMethod registrationAlignmentMethod)
        {
            selectedRegistrationAlignmentMethod = registrationAlignmentMethod;
            var registrationMethodType =
                selectedRegistrationAlignmentMethod.GetRegistrationMethod.GetRegistrationMethodType;
            registrationTypeTabControl.SelectedIndex = (int)registrationMethodType;

            switch (registrationMethodType)
            {
                case Template.RegistrationAlignmentMethod.RegistrationMethodType.KAZE:
                    var kazeRegistrationMethod =
                        (Template.RegistrationAlignmentMethod.KazeRegistrationMethod)registrationAlignmentMethod
                            .GetRegistrationMethod;
                    var kazeData = kazeRegistrationMethod.GetKazeData;
                    this.SetKazeData(kazeData);
                    break;

                case Template.RegistrationAlignmentMethod.RegistrationMethodType.AKAZE:
                    var aKazeRegistrationMethod =
                        (Template.RegistrationAlignmentMethod.AKazeRegistrationMethod)registrationAlignmentMethod
                            .GetRegistrationMethod;
                    var aKazeData = aKazeRegistrationMethod.GetAKazeData;
                    this.SetAKazeData(aKazeData);
                    break;
            }

            genModelFeaturesBtn.Enabled = true;
            useStoredModelFeaturesToggle.Enabled = true;
            useStoredModelFeaturesToggle.ToggleState = selectedRegistrationAlignmentMethod.GetUseStoredModelFeatures
                ? ToggleButtonState.Active
                : ToggleButtonState.Inactive;
        }

        private void SetupForConfiguration()
        {
        }

        private Template.RegistrationAlignmentMethod.KazeRegistrationMethod.KazeData GetKazeData()
        {
            var extended = false;
            var upright = false;
            var descThresh = 0.001f;
            var descOcts = 1;
            var descSbls = 4;
            var diffType = KAZE.Diffusivity.PmG2;

            synchronizationContext.Send(delegate
            {
                extended = kazeExtendedToggle.ToggleState == ToggleButtonState.Active;
                upright = kazeUprightToggle.ToggleState == ToggleButtonState.Active;
                descThresh = (float)kazeThresholdValueBox.DoubleValue;
                descOcts = (int)kazeOctavesValueBox.IntegerValue;
                descSbls = (int)kazeSublvlsValueBox.IntegerValue;
                diffType = (KAZE.Diffusivity)kazeDiffTypeValueBox.SelectedIndex;
            }, null);

            var kazeData =
                new Template.RegistrationAlignmentMethod.KazeRegistrationMethod.KazeData(extended, upright, descThresh,
                    descOcts, descSbls, diffType);
            return kazeData;
        }

        private void SetKazeData(Template.RegistrationAlignmentMethod.KazeRegistrationMethod.KazeData kazeData)
        {
            synchronizationContext.Send(delegate
            {
                kazeExtendedToggle.ToggleState =
                    kazeData.Extended ? ToggleButtonState.Active : ToggleButtonState.Inactive;
                kazeUprightToggle.ToggleState =
                    kazeData.Upright ? ToggleButtonState.Active : ToggleButtonState.Inactive;
                kazeThresholdValueBox.DoubleValue = kazeData.Threshold;
                kazeOctavesValueBox.IntegerValue = kazeData.Octaves;
                kazeSublvlsValueBox.IntegerValue = kazeData.Sublevels;
                kazeDiffTypeValueBox.SelectedValue = kazeData.Diffusivity;
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

        private void SetAKazeData(Template.RegistrationAlignmentMethod.AKazeRegistrationMethod.AKazeData aKazeData)
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
            }, null);
        }

        #region Configuration Controls

        private void RegistrationTypeTabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void GenModelFeaturesBtn_Click(object sender, EventArgs e)
        {
            if (selectedRegistrationAlignmentMethod == null)
            {
                return;
            }

            selectedRegistrationAlignmentMethod.StoreModelFeatures(templateImage,
                useStoredModelFeaturesToggle.ToggleState == ToggleButtonState.Active);
        }

        private void UseStoredModelFeaturesToggle_ToggleStateChanged(object sender, ToggleStateChangedEventArgs e)
        {
            if (selectedRegistrationAlignmentMethod == null)
            {
                return;
            }

            selectedRegistrationAlignmentMethod.StoreModelFeatures(templateImage,
                e.ToggleState == ToggleButtonState.Active);
        }

        private void DoneBtn_Click(object sender, EventArgs e)
        {
            this.OnConfigurationFinishedEvent?.Invoke(selectedRegistrationAlignmentMethod);
        }

        private void SetBtn_Click(object sender, EventArgs e)
        {
            var registrationMethodType =
                (Template.RegistrationAlignmentMethod.RegistrationMethodType)registrationTypeTabControl.SelectedIndex;
            switch (registrationMethodType)
            {
                case Template.RegistrationAlignmentMethod.RegistrationMethodType.KAZE:
                    var kazeData = this.GetKazeData();
                    var kazeRegistrationMethod =
                        new Template.RegistrationAlignmentMethod.KazeRegistrationMethod(kazeData);
                    selectedRegistrationAlignmentMethod = new Template.RegistrationAlignmentMethod(pipelineIndex,
                        methodName, kazeRegistrationMethod, templateImage, templateImage.Size);
                    break;

                case Template.RegistrationAlignmentMethod.RegistrationMethodType.AKAZE:
                    var aKazeData = this.GetAKazeData();
                    var aKazeRegistrationMethod =
                        new Template.RegistrationAlignmentMethod.AKazeRegistrationMethod(aKazeData);
                    selectedRegistrationAlignmentMethod = new Template.RegistrationAlignmentMethod(pipelineIndex,
                        methodName, aKazeRegistrationMethod, templateImage, templateImage.Size);
                    break;
            }

            if (useStoredModelFeaturesToggle.ToggleState == ToggleButtonState.Active)
            {
                selectedRegistrationAlignmentMethod.StoreModelFeatures(templateImage, true);
            }

            if (!genModelFeaturesBtn.Enabled)
            {
                genModelFeaturesBtn.Enabled = true;
                useStoredModelFeaturesToggle.Enabled = true;
            }

            useStoredModelFeaturesToggle.ToggleState = selectedRegistrationAlignmentMethod.GetUseStoredModelFeatures
                ? ToggleButtonState.Active
                : ToggleButtonState.Inactive;
        }

        #endregion

        #endregion
    }
}