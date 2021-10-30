namespace Synapse.Controls
{
    using System;
    using System.Threading;
    using System.Windows.Forms;

    using Emgu.CV.Features2D;

    using Synapse.Core.Templates;
    using Synapse.Utilities.Enums;

    using Syncfusion.Windows.Forms.Tools;

    public partial class KazeSettingsControl : UserControl
    {
        #region Events

        public delegate void SetDataDelegate(
            Template.RegistrationAlignmentMethod.KazeRegistrationMethod.KazeData kazeData, bool useStoredModelFeatures,
            int pipelineIndex);

        public SetDataDelegate OnSetDataEvent;

        public event EventHandler<int> OnResetDataEvent;

        #endregion

        #region Properties

        private readonly Template.RegistrationAlignmentMethod.KazeRegistrationMethod.KazeData kazeData;

        #endregion

        #region Variables

        private readonly SynchronizationContext synchronizationContext;
        private readonly bool initialIsUseStoredModelFeaturesBool;
        private readonly int pipelineIndex;

        #endregion

        public KazeSettingsControl()
        {
            this.InitializeComponent();
        }

        public KazeSettingsControl(Template.RegistrationAlignmentMethod.KazeRegistrationMethod.KazeData kazeData,
            bool isUseStoredEnabled, int pipelineIndex)
        {
            this.InitializeComponent();

            synchronizationContext = SynchronizationContext.Current;
            this.kazeData = kazeData;
            initialIsUseStoredModelFeaturesBool = isUseStoredEnabled;
            this.pipelineIndex = pipelineIndex;

            kazeDiffTypeValueBox.DataSource = EnumHelper.ToList(typeof(KAZE.Diffusivity));
            kazeDiffTypeValueBox.DisplayMember = "Value";
            kazeDiffTypeValueBox.ValueMember = "Key";

            this.InitializeKazePanel(kazeData, isUseStoredEnabled);
        }

        public void InitializeKazePanel(Template.RegistrationAlignmentMethod.KazeRegistrationMethod.KazeData kazeData,
            bool isUseStoredEnabled)
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
                kazeUseModelFeaturesOptionToggle.ToggleState =
                    isUseStoredEnabled ? ToggleButtonState.Active : ToggleButtonState.Inactive;
            }, null);
        }

        public Template.RegistrationAlignmentMethod.KazeRegistrationMethod.KazeData GetKazeData()
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

        private void Reset()
        {
            this.InitializeKazePanel(kazeData, initialIsUseStoredModelFeaturesBool);
        }

        private void SetBtn_Click(object sender, EventArgs e)
        {
            OnSetDataEvent?.Invoke(this.GetKazeData(),
                kazeUseModelFeaturesOptionToggle.ToggleState == ToggleButtonState.Active, pipelineIndex);
        }

        private void ResetBtn_Click(object sender, EventArgs e)
        {
            this.Reset();
            this.OnResetDataEvent?.Invoke(this, pipelineIndex);
        }
    }
}