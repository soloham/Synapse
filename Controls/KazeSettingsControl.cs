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
using Syncfusion.Windows.Forms.Tools;
using Emgu.CV.Features2D;
using Synapse.Utilities.Enums;

namespace Synapse.Controls
{
    public partial class KazeSettingsControl : UserControl
    {
        #region Events

        internal delegate void SetDataDelegate(Template.RegistrationAlignmentMethod.KazeRegistrationMethod.KazeData kazeData, bool useStoredModelFeatures, int pipelineIndex);
        internal SetDataDelegate OnSetDataEvent;

        public event EventHandler<int> OnResetDataEvent;

        #endregion

        #region Properties
        private Template.RegistrationAlignmentMethod.KazeRegistrationMethod.KazeData kazeData;
        #endregion

        #region Variables
        private SynchronizationContext synchronizationContext;
        private bool initialIsUseStoredModelFeaturesBool;
        private int pipelineIndex;
        #endregion

        public KazeSettingsControl()
        {
            InitializeComponent();
        }

        internal KazeSettingsControl(Template.RegistrationAlignmentMethod.KazeRegistrationMethod.KazeData kazeData, bool isUseStoredEnabled, int pipelineIndex)
        {
            InitializeComponent();

            synchronizationContext = SynchronizationContext.Current;
            this.kazeData = kazeData;
            initialIsUseStoredModelFeaturesBool = isUseStoredEnabled;
            this.pipelineIndex = pipelineIndex;

            kazeDiffTypeValueBox.DataSource = EnumHelper.ToList(typeof(KAZE.Diffusivity));
            kazeDiffTypeValueBox.DisplayMember = "Value";
            kazeDiffTypeValueBox.ValueMember = "Key";

            InitializeKazePanel(kazeData, isUseStoredEnabled);
        }

        internal void InitializeKazePanel(Template.RegistrationAlignmentMethod.KazeRegistrationMethod.KazeData kazeData, bool isUseStoredEnabled)
        {
            synchronizationContext.Send(new SendOrPostCallback(
            delegate (object state)
            {
                kazeExtendedToggle.ToggleState = kazeData.Extended ? ToggleButtonState.Active : ToggleButtonState.Inactive;
                kazeUprightToggle.ToggleState = kazeData.Upright ? ToggleButtonState.Active : ToggleButtonState.Inactive;
                kazeThresholdValueBox.DoubleValue = kazeData.Threshold;
                kazeOctavesValueBox.IntegerValue = kazeData.Octaves;
                kazeSublvlsValueBox.IntegerValue = kazeData.Sublevels;
                kazeDiffTypeValueBox.SelectedValue = kazeData.Diffusivity;
                kazeUseModelFeaturesOptionToggle.ToggleState = isUseStoredEnabled ? ToggleButtonState.Active : ToggleButtonState.Inactive; 
            }
            ), null);
        }
        internal Template.RegistrationAlignmentMethod.KazeRegistrationMethod.KazeData GetKazeData()
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

        private void Reset()
        {
            InitializeKazePanel(kazeData, initialIsUseStoredModelFeaturesBool);
        }

        private void SetBtn_Click(object sender, EventArgs e)
        {
            OnSetDataEvent?.Invoke(GetKazeData(), kazeUseModelFeaturesOptionToggle.ToggleState == Syncfusion.Windows.Forms.Tools.ToggleButtonState.Active, pipelineIndex);
        }
        private void ResetBtn_Click(object sender, EventArgs e)
        {
            Reset();
            OnResetDataEvent?.Invoke(this, pipelineIndex);
        }

    }
}
