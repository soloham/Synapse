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
using Synapse.Utilities.Extensions;
using Synapse.Utilities.Memory;
using Syncfusion.Windows.Forms.Tools;
using Syncfusion.WinForms.Controls;

namespace Synapse.Modules
{
    public partial class AlignmentPipelineTestForm : SfForm
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
        internal AlignmentPipelineTestForm(List<Template.AlignmentMethod> alignmentPipeline, Image<Gray, byte> modelImage)
        {
            InitializeComponent();

            Awake();

            Initialize(alignmentPipeline);
        }

        private void Awake()
        {
            synchronizationContext = SynchronizationContext.Current;
        }

        #endregion

        #region Private Methods

        private void Initialize(List<Template.AlignmentMethod> alignmentMethods)
        {
            for (int i = 0; i < alignmentMethods.Count; i++)
            {
                Template.AlignmentMethod alignmentMethod = alignmentMethods[i];

                TabPageAdv methodTabPage = CreateAlignmentMethodTabPage(alignmentMethod);
                alignmentPipelineTabControl.TabPages.Add(methodTabPage);
                methodTabPage.Dock = DockStyle.Fill;
            }
        }

        private TabPageAdv CreateAlignmentMethodTabPage(Template.AlignmentMethod alignmentMethod)
        {
            TabPageAdv result = null;
            switch (alignmentMethod.GetAlignmentMethodType)
            {
                case Template.AlignmentMethodType.Anchors:
                    Template.AnchorAlignmentMethod anchorAlignmentMethod = (Template.AnchorAlignmentMethod)alignmentMethod;

                    AnchorsSettingsPanel anchorSP = new AnchorsSettingsPanel(anchorAlignmentMethod.GetAnchors);

                    TabPageAdv anchorTabPage = new TabPageAdv(alignmentMethod.MethodName);
                    anchorTabPage.Controls.Add(anchorSP);
                    anchorSP.Dock = DockStyle.Top;

                    result = anchorTabPage;
                    break;
                case Template.AlignmentMethodType.Registration:
                    Template.RegistrationAlignmentMethod registrationAlignmentMethod = (Template.RegistrationAlignmentMethod)alignmentMethod;

                    switch (registrationAlignmentMethod.GetRegistrationMethod.GetRegistrationMethodType)
                    {
                        case Template.RegistrationAlignmentMethod.RegistrationMethodType.KAZE:
                            Template.RegistrationAlignmentMethod.KazeRegistrationMethod kazeRegistrationMethod = (Template.RegistrationAlignmentMethod.KazeRegistrationMethod)registrationAlignmentMethod.GetRegistrationMethod;

                            KazeSettingsControl kazeSettingsControl = new KazeSettingsControl(kazeRegistrationMethod.GetKazeData);

                            TabPageAdv kazeTabPage = new TabPageAdv(alignmentMethod.MethodName);
                            kazeTabPage.Controls.Add(kazeSettingsControl);
                            kazeSettingsControl.Dock = DockStyle.Top;

                            result = kazeTabPage;
                            break;
                        case Template.RegistrationAlignmentMethod.RegistrationMethodType.AKAZE:
                            Template.RegistrationAlignmentMethod.AKazeRegistrationMethod akazeRegistrationMethod = (Template.RegistrationAlignmentMethod.AKazeRegistrationMethod)registrationAlignmentMethod.GetRegistrationMethod;

                            AKazeSettingsControl aKazeSettingsControl = new AKazeSettingsControl(akazeRegistrationMethod.GetAKazeData);
                            
                            TabPageAdv akazeTabPage = new TabPageAdv(alignmentMethod.MethodName);
                            akazeTabPage.Controls.Add(aKazeSettingsControl);
                            aKazeSettingsControl.Dock = DockStyle.Top;

                            result = akazeTabPage;
                            break;
                    }
                    break;
            }

            return result;
        }

        #region Configuration Controls

        #endregion

        #endregion
    }
}