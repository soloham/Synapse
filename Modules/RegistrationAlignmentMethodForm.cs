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
        private Template.RegistrationAlignmentMethod selectedRegistrationAlignmentMethod = null;
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

        private void SetupForConfigured(Template.RegistrationAlignmentMethod registrationAlignmentMethod)
        {
            selectedRegistrationAlignmentMethod = registrationAlignmentMethod;
            var registrationMethodType = selectedRegistrationAlignmentMethod.GetRegistrationMethod.GetRegistrationMethodType;
            registrationTypeTabControl.SelectedIndex = (int)registrationMethodType;

            switch (registrationMethodType)
            {
                case Template.RegistrationAlignmentMethod.RegistrationMethodType.KAZE:
                    Template.RegistrationAlignmentMethod.KazeRegistrationMethod kazeRegistrationMethod = (Template.RegistrationAlignmentMethod.KazeRegistrationMethod)registrationAlignmentMethod.GetRegistrationMethod;
                    Template.RegistrationAlignmentMethod.KazeRegistrationMethod.KazeData kazeData = kazeRegistrationMethod.GetKazeData;
                    SetKazeData(kazeData);
                    break;
                case Template.RegistrationAlignmentMethod.RegistrationMethodType.AKAZE:
                    Template.RegistrationAlignmentMethod.AKazeRegistrationMethod aKazeRegistrationMethod = (Template.RegistrationAlignmentMethod.AKazeRegistrationMethod)registrationAlignmentMethod.GetRegistrationMethod;
                    Template.RegistrationAlignmentMethod.AKazeRegistrationMethod.AKazeData aKazeData = aKazeRegistrationMethod.GetAKazeData;
                    SetAKazeData(aKazeData);
                    break;
            }

            genModelFeaturesBtn.Enabled = true;
            useStoredModelFeaturesToggle.Enabled = true;
            useStoredModelFeaturesToggle.ToggleState = selectedRegistrationAlignmentMethod.GetUseStoredModelFeatures? ToggleButtonState.Active : ToggleButtonState.Inactive;
        }
        private void SetupForConfiguration()
        {

        }

        private Template.RegistrationAlignmentMethod.KazeRegistrationMethod.KazeData GetKazeData()
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

            Template.RegistrationAlignmentMethod.KazeRegistrationMethod.KazeData kazeData = new Template.RegistrationAlignmentMethod.KazeRegistrationMethod.KazeData(extended, upright, descThresh, descOcts, descSbls, diffType);
            return kazeData;
        }
        private void SetKazeData(Template.RegistrationAlignmentMethod.KazeRegistrationMethod.KazeData kazeData)
        {
            synchronizationContext.Send(new SendOrPostCallback(
            delegate (object state)
            {
                kazeExtendedToggle.ToggleState = kazeData.Extended? ToggleButtonState.Active : ToggleButtonState.Inactive;
                kazeUprightToggle.ToggleState = kazeData.Upright? ToggleButtonState.Active : ToggleButtonState.Inactive;
                kazeThresholdValueBox.DoubleValue = kazeData.Threshold;
                kazeOctavesValueBox.IntegerValue = kazeData.Octaves;
                kazeSublvlsValueBox.IntegerValue = kazeData.Sublevels;
                kazeDiffTypeValueBox.SelectedValue = kazeData.Diffusivity;
            }
            ), null);
        }
        private Template.RegistrationAlignmentMethod.AKazeRegistrationMethod.AKazeData GetAKazeData()
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

            Template.RegistrationAlignmentMethod.AKazeRegistrationMethod.AKazeData aKazeData = new Template.RegistrationAlignmentMethod.AKazeRegistrationMethod.AKazeData(descType, descSize, descChannels, descThresh, descOcts, descLayers, diffType);
            return aKazeData;
        }
        private void SetAKazeData(Template.RegistrationAlignmentMethod.AKazeRegistrationMethod.AKazeData aKazeData)
        {
            synchronizationContext.Send(new SendOrPostCallback(
            delegate (object state)
            {
                akazeDescTypeValueBox.SelectedIndex = (int)aKazeData.DescriptorType;
                akazeDescSizeValueBox.IntegerValue = aKazeData.DescriptorSize;
                akazeDescChannelsValueBox.IntegerValue = aKazeData.Channels;
                akazeDescThresholdValueBox.DoubleValue = aKazeData.Threshold;
                akazeOctavesValueBox.IntegerValue = aKazeData.Octaves;
                akazeLayersOptionValueBox.IntegerValue = aKazeData.Layers;
                akazeDiffTypeValueBox.SelectedValue = aKazeData.Diffusivity;
            }
            ), null);
        }

        #region Configuration Controls
        private void RegistrationTypeTabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }
        private void GenModelFeaturesBtn_Click(object sender, EventArgs e)
        {
            if (selectedRegistrationAlignmentMethod == null)
                return;

            selectedRegistrationAlignmentMethod.StoreModelFeatures(templateImage.Mat, useStoredModelFeaturesToggle.ToggleState == ToggleButtonState.Active);
        }
        private void UseStoredModelFeaturesToggle_ToggleStateChanged(object sender, ToggleStateChangedEventArgs e)
        {
            if (selectedRegistrationAlignmentMethod == null)
                return;

            selectedRegistrationAlignmentMethod.StoreModelFeatures(templateImage.Mat, e.ToggleState == ToggleButtonState.Active);
        }

        private void DoneBtn_Click(object sender, EventArgs e)
        {
            OnConfigurationFinishedEvent?.Invoke(selectedRegistrationAlignmentMethod);
        }
        private void SetBtn_Click(object sender, EventArgs e)
        {
            Template.RegistrationAlignmentMethod.RegistrationMethodType registrationMethodType = (Template.RegistrationAlignmentMethod.RegistrationMethodType)registrationTypeTabControl.SelectedIndex;
            switch (registrationMethodType)
            {
                case Template.RegistrationAlignmentMethod.RegistrationMethodType.KAZE:
                    Template.RegistrationAlignmentMethod.KazeRegistrationMethod.KazeData kazeData = GetKazeData();
                    Template.RegistrationAlignmentMethod.KazeRegistrationMethod kazeRegistrationMethod = new Template.RegistrationAlignmentMethod.KazeRegistrationMethod(kazeData);
                    selectedRegistrationAlignmentMethod = new Template.RegistrationAlignmentMethod(pipelineIndex, methodName, kazeRegistrationMethod, templateImage.Mat, templateImage.Size);
                    break;
                case Template.RegistrationAlignmentMethod.RegistrationMethodType.AKAZE:
                    Template.RegistrationAlignmentMethod.AKazeRegistrationMethod.AKazeData aKazeData = GetAKazeData();
                    Template.RegistrationAlignmentMethod.AKazeRegistrationMethod aKazeRegistrationMethod = new Template.RegistrationAlignmentMethod.AKazeRegistrationMethod(aKazeData);
                    selectedRegistrationAlignmentMethod = new Template.RegistrationAlignmentMethod(pipelineIndex, methodName, aKazeRegistrationMethod, templateImage.Mat, templateImage.Size);
                    break;
            }

            if(useStoredModelFeaturesToggle.ToggleState == ToggleButtonState.Active)
            {
                selectedRegistrationAlignmentMethod.StoreModelFeatures(templateImage, true);
            }

            if (!genModelFeaturesBtn.Enabled)
            {
                genModelFeaturesBtn.Enabled = true;
                useStoredModelFeaturesToggle.Enabled = true;
            }

            useStoredModelFeaturesToggle.ToggleState = selectedRegistrationAlignmentMethod.GetUseStoredModelFeatures? ToggleButtonState.Active : ToggleButtonState.Inactive;
        }
        #endregion
        #endregion
    }
}