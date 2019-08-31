using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Synapse.Controls
{
    public partial class PipelineTestMethodSettingControl : UserControl
    {
        #region Events
        public delegate void EnabledChangedDelegate(int pipelineIndex, bool isEnabled);
        public event EnabledChangedDelegate OnEnabledChangedEvent;
        #endregion

        #region Properties
        public string MethodName { get; set; }
        public int PipelineIndex { get; set; }
        #endregion

        public PipelineTestMethodSettingControl(string methodName, int pipelineIndex)
        {
            InitializeComponent();

            MethodName = methodName;
            methodNameLabel.Text = MethodName;

            PipelineIndex = pipelineIndex;

            methodEnabledSettingsToggle.ToggleState = Syncfusion.Windows.Forms.Tools.ToggleButtonState.Active;
        }

        private void MethodEnabledSettingsToggle_ToggleStateChanged(object sender, Syncfusion.Windows.Forms.Tools.ToggleStateChangedEventArgs e)
        {
            OnEnabledChangedEvent?.Invoke(PipelineIndex, e.ToggleState == Syncfusion.Windows.Forms.Tools.ToggleButtonState.Active);
        }
    }
}
