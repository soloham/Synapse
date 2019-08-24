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
using static Synapse.Core.Templates.Template;

namespace Synapse.Modules
{
    public partial class AlignmentPipelineTestForm : SfForm
    {
        #region Enums

        #endregion

        #region Properties

        #endregion

        #region Variables

        private List<Template.AlignmentMethod> mainAlignmentPipeline;
        private List<Template.AlignmentMethod> testAlignmentPipeline;

        private Image<Gray, byte> templateImage;
        private Image<Gray, byte> testImage;

        private SynchronizationContext synchronizationContext;

        Image<Gray, byte> outputImage;
        private AlignmentPipelineResults alignmentPipelineResults;

        #endregion

        #region Events

        internal delegate void OnConfigurationFinshed(Template.RegistrationAlignmentMethod registrationAlignmentMethod);
        internal event OnConfigurationFinshed OnConfigurationFinishedEvent;

        #endregion

        #region General Methods
        internal AlignmentPipelineTestForm(List<Template.AlignmentMethod> alignmentPipeline, Image<Gray, byte> templateImage, Image<Gray, byte> testImage)
        {
            InitializeComponent();
            Awake();

            this.mainAlignmentPipeline = alignmentPipeline;
            testAlignmentPipeline = new List<Template.AlignmentMethod>(alignmentPipeline);

            this.templateImage = templateImage;
            this.testImage = testImage;

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
            pipelineTestMainTablePanel.Dock = DockStyle.None;
            pipelineTestControlsPanel.Dock = DockStyle.None;
            MainLayoutPanel.SetRow(pipelineTestMainTablePanel, 0);
            MainLayoutPanel.SetRow(pipelineTestControlsPanel, 1);
            pipelineTestMainTablePanel.Dock = DockStyle.Fill;
            pipelineTestControlsPanel.Dock = DockStyle.Fill;

            pipelineTestSettingsTablePanel.RowCount = alignmentMethods.Count == 1 ? alignmentMethods.Count + 1 : alignmentMethods.Count;
            pipelineTestSettingsTablePanel.RowStyles.Clear();

            for (int i = 0; i < pipelineTestSettingsTablePanel.RowCount; i++)
            {
                if (i == pipelineTestSettingsTablePanel.RowCount - 1 && alignmentMethods.Count == 1)
                    pipelineTestSettingsTablePanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
                else
                {
                    pipelineTestSettingsTablePanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));
                    Template.AlignmentMethod alignmentMethod = alignmentMethods[i];

                    TabPageAdv methodTabPage = CreateAlignmentMethodTabPage(alignmentMethod);
                    alignmentPipelineTabControl.TabPages.Add(methodTabPage);
                    methodTabPage.Dock = DockStyle.Fill;

                    PipelineTestMethodSettingControl pipelineTestMethodSettingControl = new PipelineTestMethodSettingControl(alignmentMethod.MethodName, alignmentMethod.PipelineIndex);
                    pipelineTestMethodSettingControl.OnEnabledChangedEvent += (int pipelineIndex, bool isEnabled) => 
                    {
                        if (!isEnabled)
                            testAlignmentPipeline[pipelineIndex].PipelineIndex = -1;
                        else
                            testAlignmentPipeline[pipelineIndex].PipelineIndex = pipelineIndex;
                    };
                    pipelineTestSettingsTablePanel.Controls.Add(pipelineTestMethodSettingControl, 0, i);
                    pipelineTestMethodSettingControl.Dock = DockStyle.Top;
                }
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
                    anchorTabPage.AutoScroll = true;

                    result = anchorTabPage;
                    break;
                case Template.AlignmentMethodType.Registration:
                    Template.RegistrationAlignmentMethod registrationAlignmentMethod = (Template.RegistrationAlignmentMethod)alignmentMethod;

                    switch (registrationAlignmentMethod.GetRegistrationMethod.GetRegistrationMethodType)
                    {
                        case Template.RegistrationAlignmentMethod.RegistrationMethodType.KAZE:
                            Template.RegistrationAlignmentMethod.KazeRegistrationMethod kazeRegistrationMethod = (Template.RegistrationAlignmentMethod.KazeRegistrationMethod)registrationAlignmentMethod.GetRegistrationMethod;

                            KazeSettingsControl kazeSettingsControl = new KazeSettingsControl(kazeRegistrationMethod.GetKazeData, registrationAlignmentMethod.GetUseStoredModelFeatures, registrationAlignmentMethod.PipelineIndex);
                            kazeSettingsControl.OnSetDataEvent += (Template.RegistrationAlignmentMethod.KazeRegistrationMethod.KazeData kazeData, bool useStoredModelFeatures, int pipelineIndex) =>
                            {
                                Template.RegistrationAlignmentMethod _registrationAlignmentMethod = (Template.RegistrationAlignmentMethod)mainAlignmentPipeline[pipelineIndex];
                                string methodName = registrationAlignmentMethod.MethodName;
                                IInputArray inputImage = registrationAlignmentMethod.GetSourceImage;
                                Size outputWidth = registrationAlignmentMethod.GetOutputWidth;

                                Template.RegistrationAlignmentMethod kazeAlignmentMethod = new Template.RegistrationAlignmentMethod(pipelineIndex, methodName, new Template.RegistrationAlignmentMethod.KazeRegistrationMethod(kazeData), inputImage, outputWidth);
                                testAlignmentPipeline[pipelineIndex] = kazeAlignmentMethod;
                            };
                            kazeSettingsControl.OnResetDataEvent += (object sender, int pipelineIndex) =>
                            {
                                Template.RegistrationAlignmentMethod _registrationAlignmentMethod = (Template.RegistrationAlignmentMethod)mainAlignmentPipeline[pipelineIndex];
                                Template.RegistrationAlignmentMethod.KazeRegistrationMethod _kazeRegistrationMethod = (Template.RegistrationAlignmentMethod.KazeRegistrationMethod)_registrationAlignmentMethod.GetRegistrationMethod; 

                                var kSC = (KazeSettingsControl)sender;
                                kSC.InitializeKazePanel(_kazeRegistrationMethod.GetKazeData, _registrationAlignmentMethod.GetUseStoredModelFeatures);

                                testAlignmentPipeline[pipelineIndex] = mainAlignmentPipeline[pipelineIndex];
                            };

                            TabPageAdv kazeTabPage = new TabPageAdv(alignmentMethod.MethodName);
                            kazeTabPage.Controls.Add(kazeSettingsControl);
                            kazeSettingsControl.Dock = DockStyle.Top;
                            kazeTabPage.AutoScroll = true;

                            result = kazeTabPage;
                            break;
                        case Template.RegistrationAlignmentMethod.RegistrationMethodType.AKAZE:
                            Template.RegistrationAlignmentMethod.AKazeRegistrationMethod akazeRegistrationMethod = (Template.RegistrationAlignmentMethod.AKazeRegistrationMethod)registrationAlignmentMethod.GetRegistrationMethod;

                            AKazeSettingsControl aKazeSettingsControl = new AKazeSettingsControl(akazeRegistrationMethod.GetAKazeData, registrationAlignmentMethod.GetUseStoredModelFeatures, registrationAlignmentMethod.PipelineIndex);
                            aKazeSettingsControl.OnSetDataEvent += (Template.RegistrationAlignmentMethod.AKazeRegistrationMethod.AKazeData akazeData, bool useStoredModelFeatures, int pipelineIndex) =>
                            {
                                Template.RegistrationAlignmentMethod _registrationAlignmentMethod = (Template.RegistrationAlignmentMethod)mainAlignmentPipeline[pipelineIndex];
                                string methodName = registrationAlignmentMethod.MethodName;
                                IInputArray inputImage = registrationAlignmentMethod.GetSourceImage;
                                Size outputWidth = registrationAlignmentMethod.GetOutputWidth;

                                Template.RegistrationAlignmentMethod akazeAlignmentMethod = new Template.RegistrationAlignmentMethod(pipelineIndex, methodName, new Template.RegistrationAlignmentMethod.AKazeRegistrationMethod(akazeData), inputImage, outputWidth);
                                testAlignmentPipeline[pipelineIndex] = akazeAlignmentMethod;
                            };
                            aKazeSettingsControl.OnResetDataEvent += (object sender, int pipelineIndex) =>
                            {
                                Template.RegistrationAlignmentMethod _registrationAlignmentMethod = (Template.RegistrationAlignmentMethod)mainAlignmentPipeline[pipelineIndex];
                                Template.RegistrationAlignmentMethod.AKazeRegistrationMethod _akazeRegistrationMethod = (Template.RegistrationAlignmentMethod.AKazeRegistrationMethod)_registrationAlignmentMethod.GetRegistrationMethod;

                                var akSC = (AKazeSettingsControl)sender;
                                akSC.InitializeAKazePanel(_akazeRegistrationMethod.GetAKazeData, _registrationAlignmentMethod.GetUseStoredModelFeatures);

                                testAlignmentPipeline[pipelineIndex] = mainAlignmentPipeline[pipelineIndex];
                            };

                            TabPageAdv akazeTabPage = new TabPageAdv(alignmentMethod.MethodName);
                            akazeTabPage.Controls.Add(aKazeSettingsControl);
                            aKazeSettingsControl.Dock = DockStyle.Top;
                            akazeTabPage.AutoScroll = true;

                            result = akazeTabPage;
                            break;
                    }
                    break;
            }

            return result;
        }
        #region Test Controls

        private void TestBtn_Click(object sender, EventArgs e)
        {
            if (testAlignmentPipeline.Count <= 0)
                return;

            List<AlignmentPipelineResults.AlignmentMethodResult> alignmentMethodResults = new List<AlignmentPipelineResults.AlignmentMethodResult>();

            IOutputArray outputImageArr;
            outputImage = templateImage;
            for (int i = 0; i < testAlignmentPipeline.Count; i++)
            {
                AlignmentPipelineResults.AlignmentMethodResult alignmentMethodResult = null;
                AlignmentMethod alignmentMethod = testAlignmentPipeline[i];

                if (alignmentMethod.PipelineIndex == -1)
                    continue;

                if (alignmentMethod.GetAlignmentMethodType == AlignmentMethodType.Anchors)
                {
                    var aIM = (AnchorAlignmentMethod)alignmentMethod;
                    aIM.ApplyMethod(outputImage, testImage, out outputImageArr, out RectangleF[] detectedAnchors, out RectangleF[] warpedAnchors, out long alignmentTime);

                    var mainAnchors = aIM.GetAnchors.ToArray();

                    var outputMat = (Mat)outputImageArr;
                    outputImage = outputMat.ToImage<Gray, byte>();

                    AlignmentPipelineResults.AnchorAlignmentMethodResult anchorAlignmentMethodResult = new AlignmentPipelineResults.AnchorAlignmentMethodResult(alignmentMethod, testImage, outputImage, alignmentTime, mainAnchors, detectedAnchors, warpedAnchors);
                    alignmentMethodResult = anchorAlignmentMethodResult;
                }
                else
                {
                    alignmentMethod.ApplyMethod(outputImage, testImage, out outputImageArr, out long alignmentTime);

                    var outputMat = (Mat)outputImageArr;
                    outputImage = outputMat.ToImage<Gray, byte>();

                    alignmentMethodResult = new AlignmentPipelineResults.AlignmentMethodResult(alignmentMethod, testImage, outputImage, alignmentTime);
                }

                alignmentMethodResults.Add(alignmentMethodResult);
            }

            testResultsTabPage.Controls.Clear();

            alignmentPipelineResults = new AlignmentPipelineResults(alignmentMethodResults);
            AlignmentPipelineResultsControl alignmentPipelineResultsControl = new AlignmentPipelineResultsControl(alignmentPipelineResults);
            testResultsTabPage.Controls.Add(alignmentPipelineResultsControl);
            alignmentPipelineResultsControl.Dock = DockStyle.Fill;
        }
        #endregion

        #endregion
    }
}