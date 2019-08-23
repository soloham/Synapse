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
using Emgu.CV.Features2D;
using Synapse.Utilities.Enums;

namespace Synapse.Controls
{
    public partial class AKazeSettingsControl : UserControl
    {
        #region Properties
        private Template.RegistrationAlignmentMethod.AKazeRegistrationMethod.AKazeData akazeData;
        #endregion

        #region Variables
        private SynchronizationContext synchronizationContext;
        #endregion

        public AKazeSettingsControl()
        {
            InitializeComponent();
        }
        internal AKazeSettingsControl(Template.RegistrationAlignmentMethod.AKazeRegistrationMethod.AKazeData akazeData)
        {
            InitializeComponent();

            synchronizationContext = SynchronizationContext.Current;
            this.akazeData = akazeData;

            akazeDiffTypeValueBox.DataSource = EnumHelper.ToList(typeof(KAZE.Diffusivity));
            akazeDiffTypeValueBox.DisplayMember = "Value";
            akazeDiffTypeValueBox.ValueMember = "Key";

            akazeDescTypeValueBox.DataSource = EnumHelper.ToList(typeof(AKAZE.DescriptorType));
            akazeDescTypeValueBox.DisplayMember = "Value";
            akazeDescTypeValueBox.ValueMember = "Key";

            InitializeAKazePanel(akazeData);
        }

        private void InitializeAKazePanel(Template.RegistrationAlignmentMethod.AKazeRegistrationMethod.AKazeData aKazeData)
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
    }
}
