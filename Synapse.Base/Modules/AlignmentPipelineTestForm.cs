using static Synapse.Core.Templates.Template;

namespace Synapse.Modules
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Threading;
    using System.Windows.Forms;

    using Emgu.CV;

    using Synapse.Controls;
    using Synapse.Utilities;

    using Syncfusion.Windows.Forms.Tools;
    using Syncfusion.WinForms.Controls;

    public partial class AlignmentPipelineTestForm : SfForm
    {
        #region Enums

        #endregion

        #region Properties

        public AlignmentPipelineResults.AlignmentMethodResult SelectedAlignmentMethodResult
        {
            get => selectedAlignmentMethodResult;
            set
            {
                selectedAlignmentMethodResult = value;
                this.SelectedMethodResultChanged(value);
            }
        }

        private AlignmentPipelineResults.AlignmentMethodResult selectedAlignmentMethodResult;

        #endregion

        #region Variables

        private readonly List<AlignmentMethod> mainAlignmentPipeline;
        private readonly List<AlignmentMethod> testAlignmentPipeline;

        private readonly Mat templateImage;
        private readonly Mat testImage;

        private SynchronizationContext synchronizationContext;

        private Mat outputImage = new Mat();
        private AlignmentPipelineResults alignmentPipelineResults;

        private AlignmentPipelineResults.AnchorAlignmentMethodResult anchorAlignmentMethodResult;
        private bool drawResultAnchors;

        private readonly List<int> ommittedAlignmetMethodIndeces = new List<int>();

        #endregion

        #region Events

        public delegate void OnResultsGenerated(AlignmentPipelineResults alignmentPipelineResults);

        public event OnResultsGenerated OnResultsGeneratedEvent;

        #endregion

        #region General Methods

        public AlignmentPipelineTestForm(List<AlignmentMethod> alignmentPipeline, Mat templateImage, Mat testImage)
        {
            this.InitializeComponent();
            this.Awake();

            mainAlignmentPipeline = alignmentPipeline;
            testAlignmentPipeline = new List<AlignmentMethod>(alignmentPipeline);

            this.templateImage = templateImage;
            this.testImage = testImage;

            this.Initialize(alignmentPipeline);
        }

        private void Awake()
        {
            synchronizationContext = SynchronizationContext.Current;

            resultsDockingManager.SetEnableDocking(resultImageBoxPanel, true);
            resultsDockingManager.DockControl(resultImageBoxPanel, this, DockingStyle.Right, 400);
            resultsDockingManager.SetMenuButtonVisibility(resultImageBoxPanel, false);
            resultsDockingManager.SetDockLabel(resultImageBoxPanel, "Result Image");
            resultsDockingManager.SetDockVisibility(resultImageBoxPanel, true);

            resultsDockingManager.SetEnableDocking(originalImageBoxPanel, true);
            resultsDockingManager.DockControl(originalImageBoxPanel, this, DockingStyle.Left, 400);
            resultsDockingManager.SetMenuButtonVisibility(originalImageBoxPanel, false);
            resultsDockingManager.SetDockLabel(originalImageBoxPanel, "Original Image");
            resultsDockingManager.SetDockVisibility(originalImageBoxPanel, true);

            resultsDockingManager.SetEnableDocking(differenceImageBoxPanel, true);
            resultsDockingManager.DockControlInAutoHideMode(differenceImageBoxPanel, DockingStyle.Left, 400);
            resultsDockingManager.SetMenuButtonVisibility(differenceImageBoxPanel, false);
            resultsDockingManager.SetDockLabel(differenceImageBoxPanel, "Difference Image");
            resultsDockingManager.SetDockVisibility(differenceImageBoxPanel, true);

            alignmentPipelineResultsControl.OnSelectedMethodResultChangedEvent +=
                (alignmentMethodResultControl, inputImg, outputImg, diffImg) =>
                {
                    originalImageBox.Image = inputImg.Bitmap;
                    //originalImageBox.ZoomToFit();
                    resultImageBox.Image = outputImg.Bitmap;
                    //resultImageBox.ZoomToFit();
                    differenceImageBox.Image = diffImg.Bitmap;
                    //differenceImageBox.ZoomToFit();

                    this.SelectedAlignmentMethodResult = alignmentMethodResultControl.GetAlignmentMethodResult;
                };
        }

        #endregion

        #region Private Methods

        private void Initialize(List<AlignmentMethod> alignmentMethods)
        {
            pipelineTestMainTablePanel.Dock = DockStyle.None;
            pipelineTestControlsPanel.Dock = DockStyle.None;
            MainLayoutPanel.SetRow(pipelineTestMainTablePanel, 0);
            MainLayoutPanel.SetRow(pipelineTestControlsPanel, 1);
            pipelineTestMainTablePanel.Dock = DockStyle.Fill;
            pipelineTestControlsPanel.Dock = DockStyle.Fill;

            pipelineTestSettingsTablePanel.RowCount =
                alignmentMethods.Count == 1 ? alignmentMethods.Count + 1 : alignmentMethods.Count;
            pipelineTestSettingsTablePanel.RowStyles.Clear();

            for (var i = 0; i < pipelineTestSettingsTablePanel.RowCount; i++)
                if (i == pipelineTestSettingsTablePanel.RowCount - 1 && alignmentMethods.Count == 1)
                {
                    pipelineTestSettingsTablePanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
                }
                else
                {
                    pipelineTestSettingsTablePanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));
                    var alignmentMethod = alignmentMethods[i];

                    var methodTabPage = this.CreateAlignmentMethodTabPage(alignmentMethod);
                    alignmentPipelineTabControl.TabPages.Add(methodTabPage);
                    methodTabPage.Dock = DockStyle.Fill;

                    var pipelineTestMethodSettingControl =
                        new PipelineTestMethodSettingControl(alignmentMethod.MethodName, alignmentMethod.PipelineIndex);
                    pipelineTestMethodSettingControl.OnEnabledChangedEvent += (pipelineIndex, isEnabled) =>
                    {
                        if (!isEnabled)
                        {
                            ommittedAlignmetMethodIndeces.Add(pipelineIndex);
                        }
                        else
                        {
                            ommittedAlignmetMethodIndeces.Remove(pipelineIndex);
                        }
                    };
                    pipelineTestSettingsTablePanel.Controls.Add(pipelineTestMethodSettingControl, 0, i);
                    pipelineTestMethodSettingControl.Dock = DockStyle.Top;
                }
        }

        private TabPageAdv CreateAlignmentMethodTabPage(AlignmentMethod alignmentMethod)
        {
            TabPageAdv result = null;
            switch (alignmentMethod.GetAlignmentMethodType)
            {
                case AlignmentMethodType.Anchors:
                    var anchorAlignmentMethod = (AnchorAlignmentMethod)alignmentMethod;

                    var anchorSP = new AnchorsSettingsPanel(anchorAlignmentMethod.GetAnchors);

                    var anchorTabPage = new TabPageAdv(alignmentMethod.MethodName);
                    anchorTabPage.Controls.Add(anchorSP);
                    anchorSP.Dock = DockStyle.Fill;
                    anchorTabPage.AutoScroll = true;

                    result = anchorTabPage;
                    break;

                case AlignmentMethodType.Registration:
                    var registrationAlignmentMethod = (RegistrationAlignmentMethod)alignmentMethod;

                    switch (registrationAlignmentMethod.GetRegistrationMethod.GetRegistrationMethodType)
                    {
                        case RegistrationAlignmentMethod.RegistrationMethodType.KAZE:
                            var kazeRegistrationMethod =
                                (RegistrationAlignmentMethod.KazeRegistrationMethod)registrationAlignmentMethod
                                    .GetRegistrationMethod;

                            var kazeSettingsControl = new KazeSettingsControl(kazeRegistrationMethod.GetKazeData,
                                registrationAlignmentMethod.GetUseStoredModelFeatures,
                                registrationAlignmentMethod.PipelineIndex);
                            kazeSettingsControl.OnSetDataEvent +=
                                (kazeData, useStoredModelFeatures, pipelineIndex) =>
                                {
                                    var _registrationAlignmentMethod =
                                        (RegistrationAlignmentMethod)mainAlignmentPipeline[pipelineIndex];
                                    var methodName = registrationAlignmentMethod.MethodName;
                                    var inputImage = registrationAlignmentMethod.GetSourceImage;
                                    var outputWidth = registrationAlignmentMethod.GetOutputWidth;

                                    var kazeAlignmentMethod = new RegistrationAlignmentMethod(pipelineIndex, methodName,
                                        new RegistrationAlignmentMethod.KazeRegistrationMethod(kazeData), inputImage,
                                        outputWidth);
                                    testAlignmentPipeline[pipelineIndex] = kazeAlignmentMethod;
                                };
                            kazeSettingsControl.OnResetDataEvent += (sender, pipelineIndex) =>
                            {
                                var _registrationAlignmentMethod =
                                    (RegistrationAlignmentMethod)mainAlignmentPipeline[pipelineIndex];
                                var _kazeRegistrationMethod =
                                    (RegistrationAlignmentMethod.KazeRegistrationMethod)_registrationAlignmentMethod
                                        .GetRegistrationMethod;

                                var kSC = (KazeSettingsControl)sender;
                                kSC.InitializeKazePanel(_kazeRegistrationMethod.GetKazeData,
                                    _registrationAlignmentMethod.GetUseStoredModelFeatures);

                                testAlignmentPipeline[pipelineIndex] = mainAlignmentPipeline[pipelineIndex];
                            };

                            var kazeTabPage = new TabPageAdv(alignmentMethod.MethodName);
                            kazeTabPage.Controls.Add(kazeSettingsControl);
                            kazeSettingsControl.Dock = DockStyle.Top;
                            kazeTabPage.AutoScroll = true;

                            result = kazeTabPage;
                            break;

                        case RegistrationAlignmentMethod.RegistrationMethodType.AKAZE:
                            var akazeRegistrationMethod =
                                (RegistrationAlignmentMethod.AKazeRegistrationMethod)registrationAlignmentMethod
                                    .GetRegistrationMethod;

                            var aKazeSettingsControl = new AKazeSettingsControl(akazeRegistrationMethod.GetAKazeData,
                                registrationAlignmentMethod.GetUseStoredModelFeatures,
                                registrationAlignmentMethod.PipelineIndex);
                            aKazeSettingsControl.OnSetDataEvent +=
                                (akazeData, useStoredModelFeatures, pipelineIndex) =>
                                {
                                    var _registrationAlignmentMethod =
                                        (RegistrationAlignmentMethod)mainAlignmentPipeline[pipelineIndex];
                                    var methodName = registrationAlignmentMethod.MethodName;
                                    var inputImage = registrationAlignmentMethod.GetSourceImage;
                                    var outputWidth = registrationAlignmentMethod.GetOutputWidth;

                                    var akazeAlignmentMethod = new RegistrationAlignmentMethod(pipelineIndex,
                                        methodName, new RegistrationAlignmentMethod.AKazeRegistrationMethod(akazeData),
                                        inputImage, outputWidth);
                                    testAlignmentPipeline[pipelineIndex] = akazeAlignmentMethod;
                                };
                            aKazeSettingsControl.OnResetDataEvent += (sender, pipelineIndex) =>
                            {
                                var _registrationAlignmentMethod =
                                    (RegistrationAlignmentMethod)mainAlignmentPipeline[pipelineIndex];
                                var _akazeRegistrationMethod =
                                    (RegistrationAlignmentMethod.AKazeRegistrationMethod)_registrationAlignmentMethod
                                        .GetRegistrationMethod;

                                var akSC = (AKazeSettingsControl)sender;
                                akSC.InitializeAKazePanel(_akazeRegistrationMethod.GetAKazeData,
                                    _registrationAlignmentMethod.GetUseStoredModelFeatures);

                                testAlignmentPipeline[pipelineIndex] = mainAlignmentPipeline[pipelineIndex];
                            };

                            var akazeTabPage = new TabPageAdv(alignmentMethod.MethodName);
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
            {
                return;
            }

            var alignmentMethodResults = new List<AlignmentPipelineResults.AlignmentMethodResult>();

            IOutputArray outputImageArr;
            outputImage = testImage;
            CvInvoke.Resize(testImage, outputImage, templateImage.Size);
            //outputImage = testImage.Resize(templateImage.Width, templateImage.Height, Emgu.CV.CvEnum.Inter.Cubic);
            for (var i = 0; i < testAlignmentPipeline.Count; i++)
            {
                Exception exception = null;

                AlignmentPipelineResults.AlignmentMethodResult alignmentMethodResult = null;
                var alignmentMethod = testAlignmentPipeline[i];

                if (alignmentMethod.PipelineIndex == -1 ||
                    ommittedAlignmetMethodIndeces.Contains(alignmentMethod.PipelineIndex))
                {
                    continue;
                }

                if (alignmentMethod.GetAlignmentMethodType == AlignmentMethodType.Anchors)
                {
                    var aIM = (AnchorAlignmentMethod)alignmentMethod;
                    var isSuccess = aIM.ApplyMethod(outputImage, out outputImageArr, out var detectedAnchors,
                        out var warpedAnchors, out var scaledMainAnchorRegions, out var scaledMainTestRegion,
                        out var homography, out var alignmentTime, out exception);
                    var mainAnchors = aIM.GetAnchors.ToArray();
                    if (isSuccess)
                    {
                        var outputMat = (Mat)outputImageArr;
                        outputImage = outputMat;
                    }

                    var anchorAlignmentMethodResult = new AlignmentPipelineResults.AnchorAlignmentMethodResult(
                        alignmentMethod,
                        isSuccess
                            ? AlignmentPipelineResults.AlignmentMethodResultType.Successful
                            : AlignmentPipelineResults.AlignmentMethodResultType.Failed, homography, testImage,
                        outputImage, alignmentTime, mainAnchors, detectedAnchors, warpedAnchors,
                        scaledMainAnchorRegions, scaledMainTestRegion);
                    alignmentMethodResult = anchorAlignmentMethodResult;
                }
                else
                {
                    var isSuccess = alignmentMethod.ApplyMethod(templateImage, outputImage, out outputImageArr,
                        out var homography, out var alignmentTime, out exception);
                    if (isSuccess)
                    {
                        var outputMat = (Mat)outputImageArr;
                        outputImage = outputMat;
                    }

                    alignmentMethodResult = new AlignmentPipelineResults.AlignmentMethodResult(alignmentMethod,
                        isSuccess
                            ? AlignmentPipelineResults.AlignmentMethodResultType.Successful
                            : AlignmentPipelineResults.AlignmentMethodResultType.Failed, homography, testImage,
                        outputImage, alignmentTime);
                }

                alignmentMethodResults.Add(alignmentMethodResult);

                if (alignmentMethodResult.GetAlignmentMethodResultType ==
                    AlignmentPipelineResults.AlignmentMethodResultType.Failed)
                {
                    var personnelData = exception.Message;

                    if (exception.StackTrace == null)
                    {
                        Messages.ShowError("An error occured while applying the method: '" +
                                           alignmentMethod.MethodName + "' \n\n For concerned personnel: " +
                                           personnelData);
                        return;
                    }

                    for (var i0 = exception.StackTrace.Length - 1; i0 > 0; i0--)
                        if (exception.StackTrace[i0] == '/' || exception.StackTrace[i0] == '\\')
                        {
                            personnelData = exception.StackTrace.Substring(i0 + 1);
                            break;
                        }

                    Messages.ShowError("An error occured while applying the method: '" + alignmentMethod.MethodName +
                                       "' \n\n For concerned personnel: " + personnelData);
                }
            }

            alignmentPipelineResults = new AlignmentPipelineResults(alignmentMethodResults);
            alignmentPipelineResultsControl.Initialize(alignmentPipelineResults);

            this.OnResultsGeneratedEvent?.Invoke(alignmentPipelineResults);
        }

        private void SelectedMethodResultChanged(AlignmentPipelineResults.AlignmentMethodResult alignmentMethodResult)
        {
            switch (alignmentMethodResult.GetAlignmentMethodType)
            {
                case AlignmentMethodType.Anchors:
                    anchorAlignmentMethodResult =
                        (AlignmentPipelineResults.AnchorAlignmentMethodResult)alignmentMethodResult;
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
            {
                return;
            }

            if (drawResultAnchors)
            {
                var detectedAnchors = anchorAlignmentMethodResult.DetectedAnchors;
                var warpedAnchors = anchorAlignmentMethodResult.WarpedAnchors;
                var scaledMainAnchors = anchorAlignmentMethodResult.ScaledMainAnchors;
                var scaledMainTestRegion = anchorAlignmentMethodResult.ScaledMainTestRegion;

                var g = e.Graphics;
                for (var i0 = 0; i0 < detectedAnchors.Length; i0++)
                {
                    var detectedRect = detectedAnchors[i0];
                    var warpedRect = warpedAnchors[i0];

                    Functions.DrawBox(g, resultImageBox.GetOffsetRectangle(detectedRect), resultImageBox.ZoomFactor,
                        Color.FromArgb(120, Color.Firebrick));
                    Functions.DrawBox(g, resultImageBox.GetOffsetRectangle(scaledMainAnchors[i0]),
                        resultImageBox.ZoomFactor, Color.FromArgb(100, Color.DodgerBlue));
                    Functions.DrawBox(g, resultImageBox.GetOffsetRectangle(warpedRect), resultImageBox.ZoomFactor,
                        Color.FromArgb(120, Color.Crimson));
                }

                Functions.DrawBox(g, resultImageBox.GetOffsetRectangle(scaledMainTestRegion), resultImageBox.ZoomFactor,
                    Color.FromArgb(150, Color.MediumAquamarine));
            }
        }

        private void OriginalImageBox_Paint(object sender, PaintEventArgs e)
        {
            if (originalImageBox.Image == null)
            {
                return;
            }

            if (drawResultAnchors)
            {
                var curSize = resultImageBox.Image.Size;
                var newSize = originalImageBox.Image.Size;

                var detectedAnchors =
                    Functions.ResizeRegions(anchorAlignmentMethodResult.DetectedAnchors, curSize, newSize);
                var warpedAnchors =
                    Functions.ResizeRegions(anchorAlignmentMethodResult.WarpedAnchors, curSize, newSize);
                var scaledMainAnchors =
                    Functions.ResizeRegions(anchorAlignmentMethodResult.ScaledMainAnchors, curSize, newSize);
                var scaledMainTestRegion =
                    Functions.ResizeRegion(anchorAlignmentMethodResult.ScaledMainTestRegion, curSize, newSize);

                var g = e.Graphics;
                for (var i0 = 0; i0 < detectedAnchors.Length; i0++)
                {
                    var detectedRect = detectedAnchors[i0];
                    var warpedRect = warpedAnchors[i0];

                    Functions.DrawBox(g, originalImageBox.GetOffsetRectangle(detectedRect), originalImageBox.ZoomFactor,
                        Color.FromArgb(150, Color.Firebrick));
                    Functions.DrawBox(g, originalImageBox.GetOffsetRectangle(scaledMainAnchors[i0]),
                        originalImageBox.ZoomFactor, Color.FromArgb(100, Color.DodgerBlue));
                    Functions.DrawBox(g, originalImageBox.GetOffsetRectangle(warpedRect), originalImageBox.ZoomFactor,
                        Color.FromArgb(120, Color.Crimson));
                }

                Functions.DrawBox(g, originalImageBox.GetOffsetRectangle(scaledMainTestRegion),
                    originalImageBox.ZoomFactor, Color.FromArgb(150, Color.MediumAquamarine));
            }
        }

        private void AlignmentPipelineTestMainTabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            //if (alignmentPipelineTestMainTabControl.SelectedTab.Text == "Results")
            //{
            //    resultsDockingManager.SetDockVisibility(resultImageBoxPanel, true);
            //    resultsDockingManager.SetDockVisibility(originalImageBoxPanel, true);
            //    resultsDockingManager.SetDockVisibility(differenceImageBoxPanel, true);
            //}
            //else
            //{
            //    resultsDockingManager.SetDockVisibility(resultImageBoxPanel, false);
            //    resultsDockingManager.SetDockVisibility(originalImageBoxPanel, false);
            //    resultsDockingManager.SetDockVisibility(differenceImageBoxPanel, false);
            //}
        }

        private void AlignmentPipelineTabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void DoneBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }

    #endregion

    #endregion
}