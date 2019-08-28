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
        internal AlignmentPipelineResults.AlignmentMethodResult SelectedAlignmentMethodResult { get { return selectedAlignmentMethodResult; } set { selectedAlignmentMethodResult = value; SelectedMethodResultChanged(value); } }
        AlignmentPipelineResults.AlignmentMethodResult selectedAlignmentMethodResult;
        #endregion

        #region Variables

        private List<Template.AlignmentMethod> mainAlignmentPipeline;
        private List<Template.AlignmentMethod> testAlignmentPipeline;

        private Image<Gray, byte> templateImage;
        private Image<Gray, byte> testImage;

        private SynchronizationContext synchronizationContext;

        Image<Gray, byte> outputImage;
        private AlignmentPipelineResults alignmentPipelineResults;

        private AlignmentPipelineResults.AnchorAlignmentMethodResult anchorAlignmentMethodResult;
        private bool drawResultAnchors = false;
        #endregion

        #region Events

        internal delegate void OnResultsGenerated(AlignmentPipelineResults alignmentPipelineResults);
        internal event OnResultsGenerated OnResultsGeneratedEvent;

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

            resultsDockingManager.SetEnableDocking(resultImageBoxPanel, true);
            resultsDockingManager.DockControlInAutoHideMode(resultImageBoxPanel, DockingStyle.Right, 400);
            resultsDockingManager.SetMenuButtonVisibility(resultImageBoxPanel, false);
            resultsDockingManager.SetDockLabel(resultImageBoxPanel, "Result Image");
            resultsDockingManager.SetDockVisibility(resultImageBoxPanel, false);

            resultsDockingManager.SetEnableDocking(originalImageBoxPanel, true);
            resultsDockingManager.DockControlInAutoHideMode(originalImageBoxPanel, DockingStyle.Left, 400);
            resultsDockingManager.SetMenuButtonVisibility(originalImageBoxPanel, false);
            resultsDockingManager.SetDockLabel(originalImageBoxPanel, "Original Image");
            resultsDockingManager.SetDockVisibility(originalImageBoxPanel, false);

            resultsDockingManager.SetEnableDocking(differenceImageBoxPanel, true);
            resultsDockingManager.DockControlInAutoHideMode(differenceImageBoxPanel, DockingStyle.Left, 400);
            resultsDockingManager.SetMenuButtonVisibility(differenceImageBoxPanel, false);
            resultsDockingManager.SetDockLabel(differenceImageBoxPanel, "Difference Image");
            resultsDockingManager.SetDockVisibility(differenceImageBoxPanel, false);

            alignmentPipelineResultsControl.OnSelectedMethodResultChangedEvent += (AlignmentMethodResultControl alignmentMethodResultControl, Image<Gray, byte> inputImg, Image<Gray, byte> outputImg, Image<Gray, byte> diffImg) =>
            {
                originalImageBox.Image = inputImg.Bitmap;
                resultImageBox.Image = outputImg.Bitmap;
                differenceImageBox.Image = diffImg.Bitmap;

                SelectedAlignmentMethodResult = alignmentMethodResultControl.GetAlignmentMethodResult;
            };
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
                    anchorSP.Dock = DockStyle.Fill;
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
                Exception exception = null;

                AlignmentPipelineResults.AlignmentMethodResult alignmentMethodResult = null;
                AlignmentMethod alignmentMethod = testAlignmentPipeline[i];

                if (alignmentMethod.PipelineIndex == -1)
                    continue;

                if (alignmentMethod.GetAlignmentMethodType == AlignmentMethodType.Anchors)
                {
                    var aIM = (AnchorAlignmentMethod)alignmentMethod;
                    bool isSuccess = aIM.ApplyMethod(outputImage, testImage, out outputImageArr, out RectangleF[] detectedAnchors, out RectangleF[] warpedAnchors, out RectangleF warpedTestRegion, out long alignmentTime, out exception);
                    var mainAnchors = aIM.GetAnchors.ToArray();
                    var mainTestRegion = aIM.GetTestRegion.GetAnchorRegion;
                    if (isSuccess)
                    {
                        var outputMat = (Mat)outputImageArr;
                        outputImage = outputMat.ToImage<Gray, byte>();
                    }
                    AlignmentPipelineResults.AnchorAlignmentMethodResult anchorAlignmentMethodResult = new AlignmentPipelineResults.AnchorAlignmentMethodResult(alignmentMethod, isSuccess? AlignmentPipelineResults.AlignmentMethodResultType.Successful : AlignmentPipelineResults.AlignmentMethodResultType.Failed, testImage, outputImage, alignmentTime, mainAnchors, detectedAnchors, warpedAnchors, mainTestRegion, warpedTestRegion);
                    alignmentMethodResult = anchorAlignmentMethodResult;
                }
                else
                {
                    bool isSuccess = alignmentMethod.ApplyMethod(outputImage, testImage, out outputImageArr, out long alignmentTime, out exception);
                    if (isSuccess)
                    {
                        var outputMat = (Mat)outputImageArr;
                        outputImage = outputMat.ToImage<Gray, byte>();
                    }
                    alignmentMethodResult = new AlignmentPipelineResults.AlignmentMethodResult(alignmentMethod, isSuccess? AlignmentPipelineResults.AlignmentMethodResultType.Successful : AlignmentPipelineResults.AlignmentMethodResultType.Failed, testImage, outputImage, alignmentTime);
                }

                alignmentMethodResults.Add(alignmentMethodResult);
                
                if(alignmentMethodResult.GetAlignmentMethodResultType == AlignmentPipelineResults.AlignmentMethodResultType.Failed)
                {
                    string personnelData = "";

                    for (int i0 = exception.StackTrace.Length-1; i0 > 0; i0--)
                    {
                        if(exception.StackTrace[i0] == '/' || exception.StackTrace[i0] == '\\')
                        {
                            personnelData = exception.StackTrace.Substring(i0 + 1);
                            break;
                        }
                    }
                    Messages.ShowError("An error occured while applying the method: '" + alignmentMethod.MethodName + "' \n\n For concerned personnel: " + personnelData);
                }
            }

            OnResultsGeneratedEvent?.Invoke(alignmentPipelineResults);

            if (alignmentMethodResults.Count == 0)
                return;

            alignmentPipelineResults = new AlignmentPipelineResults(alignmentMethodResults);
            alignmentPipelineResultsControl.Initialize(alignmentPipelineResults);
        }

        private void SelectedMethodResultChanged(AlignmentPipelineResults.AlignmentMethodResult alignmentMethodResult)
        {
            switch (alignmentMethodResult.GetAlignmentMethodType)
            {
                case AlignmentMethodType.Anchors:
                    anchorAlignmentMethodResult = (AlignmentPipelineResults.AnchorAlignmentMethodResult)alignmentMethodResult;
                    drawResultAnchors = true;
                    break;
                case AlignmentMethodType.Registration:
                    drawResultAnchors = false;
                    break;
            }
        }

        private void ResultImageBox_Paint(object sender, PaintEventArgs e)
        {
            if (resultImageBox.Image == null)
                return;

            if (drawResultAnchors)
            {
                var detectedAnchors = anchorAlignmentMethodResult.DetectedAnchors;
                var warpedAnchors = anchorAlignmentMethodResult.WarpedAnchors;
                var mainAnchors = anchorAlignmentMethodResult.MainAnchors;
                var mainTestRegion = anchorAlignmentMethodResult.MainTestRegion;
                var warpedTestRegion = anchorAlignmentMethodResult.WarpedTestRegion;

                Graphics g = e.Graphics;
                for (int i0 = 0; i0 < detectedAnchors.Length; i0++)
                {
                    //RectangleF detectedRect = detectedAnchors[i0];
                    RectangleF warpedRect = warpedAnchors[i0];

                    //Functions.DrawBox(g, imageBox.GetOffsetRectangle(detectedRect), imageBox.ZoomFactor, Color.FromArgb(150, Color.Firebrick));
                    Functions.DrawBox(g, resultImageBox.GetOffsetRectangle(mainAnchors[i0].GetAnchorRegion), resultImageBox.ZoomFactor, Color.FromArgb(100, Color.DodgerBlue), 2);
                    Functions.DrawBox(g, resultImageBox.GetOffsetRectangle(warpedRect), resultImageBox.ZoomFactor, Color.FromArgb(120, Color.Crimson));
                }

                Functions.DrawBox(g, resultImageBox.GetOffsetRectangle(mainTestRegion), resultImageBox.ZoomFactor, Color.FromArgb(100, Color.MediumAquamarine));
                Functions.DrawBox(g, resultImageBox.GetOffsetRectangle(warpedTestRegion), resultImageBox.ZoomFactor, Color.FromArgb(120, Color.MediumAquamarine));
            }
        }

        private void AlignmentPipelineTestMainTabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (alignmentPipelineTestMainTabControl.SelectedTab.Text == "Results")
            {
                resultsDockingManager.SetDockVisibility(resultImageBoxPanel, true);
                resultsDockingManager.SetDockVisibility(originalImageBoxPanel, true);
                resultsDockingManager.SetDockVisibility(differenceImageBoxPanel, true);
            }
            else
            {
                resultsDockingManager.SetDockVisibility(resultImageBoxPanel, false);
                resultsDockingManager.SetDockVisibility(originalImageBoxPanel, false);
                resultsDockingManager.SetDockVisibility(differenceImageBoxPanel, false);
            }
        }

        private void AlignmentPipelineTabControl_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void OriginalImageBox_Paint(object sender, PaintEventArgs e)
        {
            if (originalImageBox.Image == null)
                return;

            if (drawResultAnchors)
            {
                Size curSize = resultImageBox.Image.Size;
                Size newSize = originalImageBox.Image.Size;

                var detectedAnchors = ResizeAnchors(anchorAlignmentMethodResult.DetectedAnchors, curSize, newSize);
                var warpedAnchors = ResizeAnchors(anchorAlignmentMethodResult.WarpedAnchors, curSize, newSize);
                RectangleF[] mainAnchors = new RectangleF[anchorAlignmentMethodResult.MainAnchors.Length];
                var mainTestRegion = anchorAlignmentMethodResult.MainTestRegion;
                var warpedTestRegion = anchorAlignmentMethodResult.WarpedTestRegion;

                for (int i0 = 0; i0 < detectedAnchors.Length; i0++)
                {
                    mainAnchors[i0] = anchorAlignmentMethodResult.MainAnchors[i0].GetAnchorRegion;
                }

                mainAnchors = ResizeAnchors(mainAnchors, curSize, newSize);

                Graphics g = e.Graphics;
                for (int i0 = 0; i0 < detectedAnchors.Length; i0++)
                {
                    //RectangleF detectedRect = detectedAnchors[i0];
                    RectangleF warpedRect = warpedAnchors[i0];


                    //Functions.DrawBox(g, imageBox.GetOffsetRectangle(detectedRect), imageBox.ZoomFactor, Color.FromArgb(150, Color.Firebrick));
                    Functions.DrawBox(g, originalImageBox.GetOffsetRectangle(mainAnchors[i0]), originalImageBox.ZoomFactor, Color.FromArgb(100, Color.DodgerBlue), 2);
                    Functions.DrawBox(g, originalImageBox.GetOffsetRectangle(warpedRect), originalImageBox.ZoomFactor, Color.FromArgb(120, Color.Crimson));
                }

                Functions.DrawBox(g, originalImageBox.GetOffsetRectangle(mainTestRegion), originalImageBox.ZoomFactor, Color.FromArgb(100, Color.MediumAquamarine));
                Functions.DrawBox(g, originalImageBox.GetOffsetRectangle(warpedTestRegion), originalImageBox.ZoomFactor, Color.FromArgb(120, Color.MediumAquamarine));
            }
        }

        private RectangleF[] ResizeAnchors(RectangleF[] rectangleFs, SizeF curSize, SizeF newSize)
        {
            RectangleF[] results = new RectangleF[rectangleFs.Length];

            float xScaleRatio = newSize.Width / curSize.Width;
            float yScaleRatio = newSize.Height / curSize.Height;

            for (int i = 0; i < results.Length; i++)
            {
                results[i] = new RectangleF(new PointF(rectangleFs[i].X * xScaleRatio, rectangleFs[i].Y *yScaleRatio), new SizeF(rectangleFs[i].Size.Width * xScaleRatio, rectangleFs[i].Size.Height * yScaleRatio));
            }
            return results;
        }

        private void DoneBtn_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
    #endregion

    #endregion


}