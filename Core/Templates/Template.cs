﻿using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Features2D;
using Emgu.CV.Flann;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Synapse.Utilities.Attributes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Synapse.Core.Templates.Template.Data;

namespace Synapse.Core.Templates
{
    internal class Template
    {
        #region Enums
        public enum AlignmentMethodType
        {
            [EnumDescription("Anchors")]
            Anchors,
            [EnumDescription("Registration")]
            Registration
        }
        public enum RegistrationMethod
        {
            [EnumDescription("AKaze")]
            AKaze,
            [EnumDescription("Kaze")]
            Kaze,
            [EnumDescription("ORB + SIFT")]
            ORB_SIFT
        }
        #endregion

        #region Objects
        [Serializable]
        internal class TemplateImage
        {
            public Bitmap GetBitmap { get { return bitmap == null ? new Bitmap(ImageLocation) : bitmap; } }
            private Bitmap bitmap;
            public Size Size { get => templateSize; set => templateSize = value; }
            private Size templateSize;
            public double TemplateScale { get => templateScale; set => templateScale = value; }
            private double templateScale;
            public double DeskewAngle { get => deskewPercent; set => deskewPercent = value; }
            private double deskewPercent;
            public string ImageLocation { get => templateImageLocation; set => templateImageLocation = value; }
            private string templateImageLocation;

            public TemplateImage(Size templateSize, double templateScale, double deskewPercent)
            {
                this.templateSize = templateSize;
                this.templateScale = templateScale;
                this.deskewPercent = deskewPercent;
            }

            public void Initialize()
            {
                bitmap = string.IsNullOrEmpty(ImageLocation)? null : new Bitmap(ImageLocation);
            }

            public void SetBitmap(Bitmap bitmap)
            {
                this.bitmap = new Bitmap(bitmap);
            }
            public static TemplateImage Empty()
            {
                return new TemplateImage(Size.Empty, 1, 0);
            }
        }

        #region Alignment Pipeline
        [Serializable]
        internal abstract class AlignmentMethod
        {
            public AlignmentMethodType GetAlignmentMethodType { get => alignmentMethodType; set { } }

            public string MethodName { get; internal set; }
            private AlignmentMethodType alignmentMethodType;
            public int PipelineIndex = 0;

            public AlignmentMethod(string methodName, AlignmentMethodType alignmentMethodType, int pipelineIndex)
            {
                MethodName = methodName;
                this.alignmentMethodType = alignmentMethodType;
                PipelineIndex = pipelineIndex;
            }

            public abstract bool ApplyMethod(IInputArray input, out IOutputArray output, out long matchTime, out Exception ex);
            public abstract bool ApplyMethod(IInputArray template, IInputArray input, out IOutputArray output, out long matchTime, out Exception ex);

        }
        [Serializable]
        internal class AnchorAlignmentMethod : AlignmentMethod
        {
            #region Objects
            [Serializable]
            public class Anchor
            {
                public RectangleF GetAnchorRegion { get => anchorRegion; }
                private RectangleF anchorRegion = new RectangleF();
                public Mat GetAnchorImage { get => anchorImage; }
                private Mat anchorImage;

                public PointF anchorCoordinates;

                public Anchor(RectangleF anchorRegion, Mat anchorImage)
                {
                    this.anchorRegion = anchorRegion;
                    this.anchorImage = anchorImage;

                    CalculateAnchorCoordinates();
                }

                private void CalculateAnchorCoordinates()
                {
                    anchorCoordinates = anchorRegion.Location;
                }
            }
            #endregion

            #region Properties
            public List<Anchor> GetAnchors { get => anchors; }
            private List<Anchor> anchors;
            public Anchor GetTestAnchor { get => testAnchor; }
            private Anchor testAnchor;
            public Size GetDownscaleSize { get => downscaleSize; }
            private Size downscaleSize;
            public double GetDownscaleScale { get => downscaleScale; }
            private double downscaleScale;
            #endregion

            #region Variables
            private Size outputSize;
            private PointF[] mainAnchorCoordinates;
            #endregion

            #region Methods
            public AnchorAlignmentMethod(List<Anchor> anchors, Anchor testAnchor, Size outputSize, int pipelineIndex, string methodName, Size downscaleSize, double downscaleScale) : base(methodName, AlignmentMethodType.Anchors, pipelineIndex)
            {
                this.anchors = anchors;
                this.testAnchor = testAnchor;
                this.outputSize = outputSize;
                this.downscaleSize = downscaleSize;
                this.downscaleScale = downscaleScale;

                ExtractCooridnates();
            }

            #region Public
            public void AddAnchor(RectangleF region, Mat image)
            {
                anchors.Add(new Anchor(region, image));

                ExtractCooridnates();
            }
            public void RemoveAnchor(int index)
            {
                anchors.RemoveAt(index);
                ExtractCooridnates();
            }
            #endregion
            #region Private
            private void ExtractCooridnates()
            {
                mainAnchorCoordinates = new PointF[anchors.Count];

                for (int i = 0; i < anchors.Count; i++)
                {
                    mainAnchorCoordinates[i] = anchors[i].anchorCoordinates;
                }
            }
            #endregion

            public override bool ApplyMethod(IInputArray input, out IOutputArray output, out long matchTime, out Exception ex)
            {
                bool isSuccess = false;
                var inputImg = (Image<Gray, byte>)input;
                inputImg = downscaleScale <= 0? inputImg.Resize(downscaleSize.Width, downscaleSize.Height, Inter.Cubic) : inputImg.Resize(downscaleScale, Inter.Cubic);
                Mat _output = new Mat();

                PointF[] anchorCoordinates = new PointF[anchors.Count];

                ex = new Exception();

                Stopwatch watch = new Stopwatch();
                watch.Start();

                try
                {
                    for (int i = 0; i < anchors.Count; i++)
                    {
                        Anchor curAnchor = anchors[i];

                        Mat result = new Mat();
                        CvInvoke.MatchTemplate(inputImg.Mat, curAnchor.GetAnchorImage, result, TemplateMatchingType.CcoeffNormed);

                        Point[] Max_Loc, Min_Loc;
                        double[] min, max;

                        result.MinMax(out min, out max, out Min_Loc, out Max_Loc);

                        if (max[0] > 0.85)
                            isSuccess = true;

                        anchorCoordinates[i] = Max_Loc[0];
                    }

                    var homography = CvInvoke.FindHomography(anchorCoordinates, mainAnchorCoordinates, RobustEstimationAlgorithm.Ransac);
                    CvInvoke.WarpPerspective(input, _output, homography, outputSize);
                }
                catch (Exception _ex)
                {
                    ex = _ex;
                    isSuccess = false;
                }

                watch.Stop();
                matchTime = watch.ElapsedMilliseconds;

                output = _output;
                return isSuccess;
            }
            public override bool ApplyMethod(IInputArray templateImage, IInputArray input, out IOutputArray output, out long matchTime, out Exception ex)
            {
                bool isSuccess = false;
                var inputImg = (Image<Gray, byte>)input;
                inputImg = downscaleScale <= 0 ? inputImg.Resize(downscaleSize.Width, downscaleSize.Height, Inter.Cubic) : inputImg.Resize(downscaleScale, Inter.Cubic);
                Mat _output = new Mat();

                PointF[] anchorCoordinates = new PointF[anchors.Count];

                ex = new Exception();

                Stopwatch watch = new Stopwatch();
                watch.Start();

                try
                {
                    for (int i = 0; i < anchors.Count; i++)
                    {
                        Anchor curAnchor = anchors[i];

                        Mat result = new Mat();
                        CvInvoke.MatchTemplate(inputImg.Mat, curAnchor.GetAnchorImage, result, Emgu.CV.CvEnum.TemplateMatchingType.CcoeffNormed);

                        Point[] Max_Loc, Min_Loc;
                        double[] min, max;

                        result.MinMax(out min, out max, out Min_Loc, out Max_Loc);

                        if (max[0] > 0.85)
                            isSuccess = true;

                        anchorCoordinates[i] = Max_Loc[0];
                    }

                    var homography = CvInvoke.FindHomography(anchorCoordinates, mainAnchorCoordinates, Emgu.CV.CvEnum.RobustEstimationAlgorithm.Ransac);
                    CvInvoke.WarpPerspective(input, _output, homography, outputSize);
                }
                catch(Exception _ex)
                {
                    ex = _ex;
                    isSuccess = false;
                }

                watch.Stop();
                matchTime = watch.ElapsedMilliseconds;

                output = _output;
                return isSuccess;
            }
            public bool ApplyMethod(IInputArray templateImage, IInputArray input, out IOutputArray output, out RectangleF[] detectedAnchors, out RectangleF[] warpedAnchors, out RectangleF warpedTestRegion, out long matchTime, out Exception ex)
            {
                bool isSuccess = false;
                var inputImg = (Image<Gray, byte>)input;
                inputImg = downscaleScale <= 0 ? (downscaleSize != Size.Empty? inputImg.Resize(downscaleSize.Width, downscaleSize.Height, Inter.Cubic) : inputImg) : inputImg.Resize(downscaleScale, Inter.Cubic);
                var templateImg = (Image<Gray, byte>)templateImage;
                Mat _output = new Mat();

                detectedAnchors = new RectangleF[anchors.Count];
                warpedAnchors = new RectangleF[anchors.Count];
                warpedTestRegion = new RectangleF();

                PointF[] anchorCoordinates = new PointF[anchors.Count];

                ex = new Exception();

                Stopwatch watch = new Stopwatch();
                watch.Start();

                try
                {
                    if (inputImg.Size != templateImg.Size)
                        inputImg = inputImg.Resize(templateImg.Width, templateImg.Height, Inter.Cubic);

                    for (int i = 0; i < anchors.Count; i++)
                    {
                        Anchor curAnchor = anchors[i];

                        Mat result = new Mat();
                        CvInvoke.MatchTemplate(inputImg.Mat, curAnchor.GetAnchorImage, result, Emgu.CV.CvEnum.TemplateMatchingType.CcoeffNormed);

                        Point[] Max_Loc, Min_Loc;
                        double[] min, max;

                        result.MinMax(out min, out max, out Min_Loc, out Max_Loc);

                        if (max[0] > 0.85)
                            isSuccess = true;

                        anchorCoordinates[i] = Max_Loc[0];

                        detectedAnchors[i] = new RectangleF(anchorCoordinates[i], curAnchor.GetAnchorRegion.Size);
                    }
                    anchorCoordinates[anchorCoordinates.Length - 1] = testAnchor.GetAnchorRegion.Location;
                    //mainAnchorCoordinates[mainAnchorCoordinates.Length - 1] = testRegion.GetAnchorRegion;

                    var homography = CvInvoke.FindHomography(anchorCoordinates, mainAnchorCoordinates, Emgu.CV.CvEnum.RobustEstimationAlgorithm.Ransac);
                    CvInvoke.WarpPerspective(inputImg, _output, homography, outputSize);
                    var warpedPoints = CvInvoke.PerspectiveTransform(anchorCoordinates, homography);
                    warpedTestRegion = new RectangleF(warpedPoints[warpedPoints.Length - 1], testAnchor.GetAnchorRegion.Size);

                    for (int i = 0; i < anchors.Count; i++)
                    {
                        warpedAnchors[i] = new RectangleF(warpedPoints[i], detectedAnchors[i].Size);
                    }
                }
                catch(Exception _ex)
                {
                    ex = _ex;
                    isSuccess = false;
                }

                watch.Stop();
                matchTime = watch.ElapsedMilliseconds;

                output = _output;
                return isSuccess;
            }
            public bool ApplyMethod(IInputArray templateImage, IInputArray input, out IOutputArray output, out PointF[] detectedAnchors, out PointF[] warpedAnchors, out PointF warpedTestPoint, out long matchTime, out Exception ex)
            {
                bool isSuccess = false;
                var inputImg = (Image<Gray, byte>)input;
                inputImg = downscaleScale <= 0 ? inputImg.Resize(downscaleSize.Width, downscaleSize.Height, Inter.Cubic) : inputImg.Resize(downscaleScale, Inter.Cubic);
                Mat _output = new Mat();

                detectedAnchors = new PointF[anchors.Count];
                warpedAnchors = new PointF[anchors.Count];
                warpedTestPoint = new PointF();

                PointF[] anchorCoordinates = new PointF[anchors.Count+1];

                ex = new Exception();

                Stopwatch watch = new Stopwatch();
                watch.Start();
                for (int i = 0; i < anchors.Count; i++)
                {
                    Anchor curAnchor = anchors[i];

                    Mat result = new Mat();
                    CvInvoke.MatchTemplate(inputImg.Mat, curAnchor.GetAnchorImage, result, Emgu.CV.CvEnum.TemplateMatchingType.CcoeffNormed);

                    Point[] Max_Loc, Min_Loc;
                    double[] min, max;

                    result.MinMax(out min, out max, out Min_Loc, out Max_Loc);

                    if (max[0] > 0.85)
                        isSuccess = true;

                    anchorCoordinates[i] = Max_Loc[0];

                    detectedAnchors[i] = anchorCoordinates[i];
                }
                warpedTestPoint = anchorCoordinates[anchorCoordinates.Length - 1];

                var homography = CvInvoke.FindHomography(anchorCoordinates, mainAnchorCoordinates, Emgu.CV.CvEnum.RobustEstimationAlgorithm.Ransac);
                CvInvoke.WarpPerspective(input, _output, homography, outputSize);
                warpedAnchors = CvInvoke.PerspectiveTransform(anchorCoordinates, homography);
                watch.Stop();

                matchTime = watch.ElapsedMilliseconds;

                output = _output;
                return isSuccess;
            }

            #endregion
        }
        [Serializable]
        internal class RegistrationAlignmentMethod : AlignmentMethod
        {
            #region Enums
            public enum RegistrationMethodType
            {
                KAZE,
                AKAZE
            }
            #endregion

            #region Objects
            [Serializable]
            public abstract class RegistrationMethod
            {
                #region Properties
                public RegistrationMethodType GetRegistrationMethodType { get; set; }
                public VectorOfKeyPoint GetStoredModelKeyPoints { get => storedModelKeyPoints; }
                private VectorOfKeyPoint storedModelKeyPoints;
                public Mat GetStoredModelDescriptors { get => storedModelDescriptors; }
                private Mat storedModelDescriptors;
                #endregion

                public RegistrationMethod(RegistrationMethodType getRegistrationMethodType)
                {
                    GetRegistrationMethodType = getRegistrationMethodType;
                }

                #region Methods
                public abstract bool ApplyMethod(IInputArray source, IInputArray observed, bool useStoredModelFeatures, out IOutputArray homography, out VectorOfVectorOfDMatch matches, out long matchTime, out long score, out Exception ex);
                public abstract bool GenerateFeatures(IInputArray source, out VectorOfKeyPoint generatedKeyPoints, out Mat generatedDescriptors);

                public void StoreModelFeatures(IInputArray modelImage)
                {
                    VectorOfKeyPoint _storedModelKeyPoints;
                    Mat _storedModelDescriptors;

                    GenerateFeatures(modelImage, out _storedModelKeyPoints, out _storedModelDescriptors);

                    this.storedModelKeyPoints = _storedModelKeyPoints;
                    this.storedModelDescriptors = _storedModelDescriptors;
                }
                #endregion
            }
            [Serializable]
            public class KazeRegistrationMethod : RegistrationMethod
            {
                #region Objects
                public class KazeData
                {
                    public KazeData(bool extended, bool upright, double threshold, int octaves, int sublevels, KAZE.Diffusivity diffusivity)
                    {
                        Extended = extended;
                        Upright = upright;
                        Threshold = threshold;
                        Octaves = octaves;
                        Sublevels = sublevels;
                        Diffusivity = diffusivity;
                    }

                    public bool Extended { get; set; }
                    public bool Upright { get; set; }
                    public double Threshold { get; set; }
                    public int Octaves { get; set; }
                    public int Sublevels { get; set; }
                    public KAZE.Diffusivity Diffusivity { get; set; }
                }
                #endregion

                #region Properties
                public KazeData GetKazeData { get => kazeData; }
                private KazeData kazeData;

                public KAZE GetKAZE { get => kaze; }
                private KAZE kaze;
                #endregion

                #region Methods
                #region Public
                public KazeRegistrationMethod(bool extended, bool upright, double threshold, int octaves, int sublevels, KAZE.Diffusivity diffusivity) : base(RegistrationMethodType.KAZE)
                {
                    kazeData = new KazeData(extended, upright, threshold, octaves, sublevels, diffusivity);
                    kaze = new KAZE(kazeData.Extended, kazeData.Upright, (float)kazeData.Threshold, kazeData.Octaves, kazeData.Sublevels, kazeData.Diffusivity);
                }

                public KazeRegistrationMethod(KazeData kazeData) : base(RegistrationMethodType.KAZE)
                {
                    this.kazeData = kazeData;
                    kaze = new KAZE(kazeData.Extended, kazeData.Upright, (float)kazeData.Threshold, kazeData.Octaves, kazeData.Sublevels, kazeData.Diffusivity);
                }
                #endregion

                #region Private
                private bool ExtractHomography (Mat modelImage, Mat observedImage, bool useStoredModelFeatures, out long matchTime, out VectorOfKeyPoint modelKeyPoints, out VectorOfKeyPoint observedKeyPoints, out VectorOfVectorOfDMatch matches, out Mat mask, out Mat homography, out long score)
                {
                    score = 0;
                    mask = new Mat();
                    matchTime = 0;

                    int k = 2;
                    double uniquenessThreshold = 0.75;

                    Stopwatch watch = new Stopwatch();
                    homography = null;

                    modelKeyPoints = new VectorOfKeyPoint();
                    observedKeyPoints = new VectorOfKeyPoint();
                    matches = new VectorOfVectorOfDMatch();

                    bool result = false;
                    try
                    {
                        using (UMat uModelImage = modelImage.GetUMat(AccessType.Read))
                        using (UMat uObservedImage = observedImage.GetUMat(AccessType.Read))
                        {
                            using (Mat observedDescriptors = new Mat())
                            using (Mat modelDescriptors = useStoredModelFeatures ? GetStoredModelDescriptors.Clone() : new Mat())
                            {
                                watch.Start();

                                if (!useStoredModelFeatures)
                                    kaze.DetectAndCompute(uModelImage, null, modelKeyPoints, modelDescriptors, false);
                                else
                                    modelKeyPoints = GetStoredModelKeyPoints;

                                kaze.DetectAndCompute(uObservedImage, null, observedKeyPoints, observedDescriptors, false);

                                if (modelKeyPoints.Size > 0 && observedKeyPoints.Size > 0)
                                {
                                    // KdTree for faster results / less accuracy
                                    using (var ip = new Emgu.CV.Flann.KdTreeIndexParams())
                                    using (var sp = new SearchParams())
                                    using (DescriptorMatcher matcher = new FlannBasedMatcher(ip, sp))
                                    {
                                        matcher.Add(modelDescriptors);

                                        matcher.KnnMatch(observedDescriptors, matches, k, null);
                                        if (matches.Size <= 0)
                                        {
                                            result = false;
                                        }
                                        else
                                        {
                                            mask = new Mat(matches.Size, 1, DepthType.Cv8U, 1);
                                            mask.SetTo(new MCvScalar(255));
                                            Features2DToolbox.VoteForUniqueness(matches, uniquenessThreshold, mask);

                                            // Calculate score based on matches size
                                            // ---------------------------------------------->
                                            score = 0;
                                            int total = 0;
                                            for (int i = 0; i < matches.Size; i++)
                                            {
                                                foreach (var e in matches[i].ToArray())
                                                    ++total;
                                                if ((int)mask.GetRawData(i)[0] == 0) continue;
                                                foreach (var e in matches[i].ToArray())
                                                    ++score;
                                            }
                                            score = (long)((score / (float)total) * 100);
                                            // <----------------------------------------------

                                            int nonZeroCount = CvInvoke.CountNonZero(mask);
                                            if (nonZeroCount >= 4)
                                            {
                                                nonZeroCount = Features2DToolbox.VoteForSizeAndOrientation(modelKeyPoints, observedKeyPoints, matches, mask, 1.6, 20);
                                                if (nonZeroCount >= 4)
                                                    homography = Features2DToolbox.GetHomographyMatrixFromMatchedFeatures(modelKeyPoints, observedKeyPoints, matches, mask, 2);
                                            }
                                        }
                                    }
                                    watch.Stop();

                                }
                                else
                                {
                                    result = false;
                                }
                            }
                        }
                    }
                    catch
                    {
                        result = false;
                    }

                    matchTime = watch.ElapsedMilliseconds;
                    return result;
                }
                public override bool ApplyMethod(IInputArray source, IInputArray observed, bool useStoredModelFeatures, out IOutputArray homography, out VectorOfVectorOfDMatch matches, out long matchTime, out long score, out Exception ex)
                {
                    bool isSuccess = false;

                    Mat _homography;
                    VectorOfKeyPoint modelKeyPoints;
                    VectorOfKeyPoint observedKeyPoints;

                    Mat mask;

                    matchTime = -1;
                    matches = null;
                    mask = null;
                    homography = null;
                    score = -1;
                    ex = new Exception();

                    try
                    {
                        var sourceImg = (Image<Gray, byte>)source;
                        var observedImg = (Image<Gray, byte>)observed;
                        isSuccess = ExtractHomography(sourceImg.Mat, observedImg.Mat, useStoredModelFeatures, out matchTime, out modelKeyPoints, out observedKeyPoints, out matches,
                            out mask, out _homography, out score);

                        if (isSuccess)
                        {
                            if (score > 0 && _homography != null)
                                isSuccess = true;
                            else
                                isSuccess = false;
                        }

                        homography = _homography;
                    }
                    catch(Exception _ex)
                    {
                        ex = _ex;
                        isSuccess = false;
                    }

                    return isSuccess;
                }
                public override bool GenerateFeatures(IInputArray source, out VectorOfKeyPoint generatedKeyPoints, out Mat generatedDescriptors)
                {
                    bool result = false;

                    generatedKeyPoints = new VectorOfKeyPoint();
                    generatedDescriptors = new Mat();
                    try
                    { 
                        Mat modelImage = (Mat)source;

                        using (UMat uModelImage = modelImage.GetUMat(AccessType.Read))
                        {
                            kaze.DetectAndCompute(uModelImage, null, generatedKeyPoints, generatedDescriptors, false);
                            result = true;
                        }
                    }
                    catch
                    {
                        result = false;
                    }

                    return result;
                }
                #endregion
                #endregion
            }
            [Serializable]
            public class AKazeRegistrationMethod : RegistrationMethod
            {
                #region Objects
                public class AKazeData
                {
                    public AKazeData(AKAZE.DescriptorType descriptorType, int descriptorSize, int channels, double threshold, int octaves, int layers, KAZE.Diffusivity diffusivity)
                    {
                        DescriptorType = descriptorType;
                        DescriptorSize = descriptorSize;
                        Channels = channels;
                        Threshold = threshold;
                        Octaves = octaves;
                        Layers = layers;
                        Diffusivity = diffusivity;
                    }

                    public AKAZE.DescriptorType DescriptorType { get; set; }
                    public int DescriptorSize { get; set; }
                    public int Channels { get; set; }
                    public double Threshold { get; set; }
                    public int Octaves { get; set; }
                    public int Layers { get; set; }
                    public KAZE.Diffusivity Diffusivity { get; set; }
                }
                #endregion

                #region Properties
                public AKazeData GetAKazeData { get => aKazeData; }
                private AKazeData aKazeData;

                public AKAZE GetAKAZE { get => akaze; }
                private AKAZE akaze;
                #endregion

                #region Methods
                #region Public
                public AKazeRegistrationMethod(AKAZE.DescriptorType descriptorType, int desSize, int channels, double threshold, int octaves, int layers, KAZE.Diffusivity diffusivity) : base(RegistrationMethodType.KAZE)
                {
                    aKazeData = new AKazeData(descriptorType, desSize, channels, threshold, octaves, layers, diffusivity);
                    akaze = new AKAZE(aKazeData.DescriptorType, aKazeData.DescriptorSize, aKazeData.Channels, (float)aKazeData.Threshold, aKazeData.Octaves, aKazeData.Layers, aKazeData.Diffusivity);
                }

                public AKazeRegistrationMethod(AKazeData aKazeData) : base(RegistrationMethodType.AKAZE)
                {
                    this.aKazeData = aKazeData;
                    akaze = new AKAZE(aKazeData.DescriptorType, aKazeData.DescriptorSize, aKazeData.Channels, (float)aKazeData.Threshold, aKazeData.Octaves, aKazeData.Layers, aKazeData.Diffusivity);
                }
                #endregion

                #region Private
                private bool ExtractHomography(Mat modelImage, Mat observedImage, bool useStoredModelFeatures, out long matchTime, out VectorOfKeyPoint modelKeyPoints, out VectorOfKeyPoint observedKeyPoints, out VectorOfVectorOfDMatch matches, out Mat mask, out Mat homography, out long score)
                {
                    score = 0;
                    mask = new Mat();
                    matchTime = 0;

                    int k = 2;
                    double uniquenessThreshold = 0.75;

                    Stopwatch watch = new Stopwatch();
                    homography = null;

                    modelKeyPoints = new VectorOfKeyPoint();
                    observedKeyPoints = new VectorOfKeyPoint();
                    matches = new VectorOfVectorOfDMatch();

                    bool result = false;

                    try
                    {
                        using (UMat uModelImage = modelImage.GetUMat(AccessType.Read))
                        using (UMat uObservedImage = observedImage.GetUMat(AccessType.Read))
                        {
                            using (Mat observedDescriptors = new Mat())
                            using (Mat modelDescriptors = useStoredModelFeatures ? GetStoredModelDescriptors : new Mat())
                            {
                                watch.Start();

                                if (!useStoredModelFeatures)
                                    akaze.DetectAndCompute(uModelImage, null, modelKeyPoints, modelDescriptors, false);
                                else
                                    modelKeyPoints = GetStoredModelKeyPoints;

                                akaze.DetectAndCompute(uObservedImage, null, observedKeyPoints, observedDescriptors, false);

                                if (modelKeyPoints.Size > 0 && observedKeyPoints.Size > 0)
                                {
                                    // KdTree for faster results / less accuracy
                                    using (var ip = new Emgu.CV.Flann.KdTreeIndexParams())
                                    using (var sp = new SearchParams())
                                    using (DescriptorMatcher matcher = new FlannBasedMatcher(ip, sp))
                                    {
                                        matcher.Add(modelDescriptors);

                                        matcher.KnnMatch(observedDescriptors, matches, k, null);
                                        if (matches.Size <= 0)
                                        {
                                            result = false;
                                        }
                                        else
                                        {
                                            mask = new Mat(matches.Size, 1, DepthType.Cv8U, 1);
                                            mask.SetTo(new MCvScalar(255));
                                            Features2DToolbox.VoteForUniqueness(matches, uniquenessThreshold, mask);

                                            // Calculate score based on matches size
                                            // ---------------------------------------------->
                                            score = 0;
                                            int total = 0;
                                            for (int i = 0; i < matches.Size; i++)
                                            {
                                                foreach (var e in matches[i].ToArray())
                                                    ++total;
                                                if ((int)mask.GetRawData(i)[0] == 0) continue;
                                                foreach (var e in matches[i].ToArray())
                                                    ++score;
                                            }
                                            score = (long)((score / (float)total) * 100);
                                            // <----------------------------------------------

                                            int nonZeroCount = CvInvoke.CountNonZero(mask);
                                            if (nonZeroCount >= 4)
                                            {
                                                nonZeroCount = Features2DToolbox.VoteForSizeAndOrientation(modelKeyPoints, observedKeyPoints, matches, mask, 1.6, 20);
                                                if (nonZeroCount >= 4)
                                                    homography = Features2DToolbox.GetHomographyMatrixFromMatchedFeatures(modelKeyPoints, observedKeyPoints, matches, mask, 2);
                                            }
                                        }
                                    }
                                    watch.Stop();
                                }
                                else
                                {
                                    result = false;
                                }
                            }
                        }

                    }
                    catch(Exception ex)
                    {
                        result = false;
                    }

                    matchTime = watch.ElapsedMilliseconds;
                    return result;
                }
                public override bool ApplyMethod(IInputArray source, IInputArray observed, bool useStoredModelFeatures, out IOutputArray homography, out VectorOfVectorOfDMatch matches, out long matchTime, out long score, out Exception ex)
                {
                    bool isSuccess = false;

                    Mat _homography;
                    VectorOfKeyPoint modelKeyPoints;
                    VectorOfKeyPoint observedKeyPoints;

                    Mat mask;

                    matchTime = -1;
                    matches = null;
                    mask = null;
                    homography = null;
                    score = -1;
                    ex = new Exception();

                    try
                    {
                        var sourceImg = (Image<Gray, byte>)source;
                        var observedImg = (Image<Gray, byte>)observed;
                        isSuccess = ExtractHomography(sourceImg.Mat, observedImg.Mat, useStoredModelFeatures, out matchTime, out modelKeyPoints, out observedKeyPoints, out matches,
                            out mask, out _homography, out score);

                        homography = _homography;
                        if (isSuccess)
                        {
                            if (score > 0 && _homography != null)
                                isSuccess = true;
                            else
                                isSuccess = false;
                        }
                    }
                    catch (Exception _ex)
                    {
                        ex = _ex;
                        isSuccess = false;
                    }

                    return isSuccess;
                }
                public override bool GenerateFeatures(IInputArray source, out VectorOfKeyPoint generatedKeyPoints, out Mat generatedDescriptors)
                {
                    bool result = false;

                    generatedKeyPoints = new VectorOfKeyPoint();
                    generatedDescriptors = new Mat();
                    try
                    {
                        Mat modelImage = (Mat)source;

                        using (UMat uModelImage = modelImage.GetUMat(AccessType.Read))
                        {
                            akaze.DetectAndCompute(uModelImage, null, generatedKeyPoints, generatedDescriptors, false);
                            result = true;
                        }
                    }
                    catch
                    {
                        result = false;
                    }

                    return result;
                }
                #endregion
                #endregion
            }
            #endregion

            #region Properties
            public RegistrationMethod GetRegistrationMethod { get => registrationMethod; }
            RegistrationMethod registrationMethod;

            public IInputArray GetSourceImage { get => sourceImage; }
            IInputArray sourceImage;
            public Size GetOutputWidth { get => outputWidth; }
            private Size outputWidth;

            public bool GetUseStoredModelFeatures { get => useStoredModelFeatures;  }
            private bool useStoredModelFeatures;
            #endregion

            public RegistrationAlignmentMethod(int pipelineIndex, string methodName, RegistrationMethod registrationMethod, IInputArray sourceImage, Size outputWidth) : base(methodName, AlignmentMethodType.Registration, pipelineIndex)
            {
                this.sourceImage = sourceImage;
                this.registrationMethod = registrationMethod;
                this.outputWidth = outputWidth;
            }

            public override bool ApplyMethod(IInputArray input, out IOutputArray output, out long matchTime, out Exception ex)
            {
                bool isSuccess = false;
                IOutputArray _homography;

                output = null;

                isSuccess = registrationMethod.ApplyMethod(sourceImage, input, useStoredModelFeatures, out _homography, out VectorOfVectorOfDMatch matches, out matchTime, out long score, out Exception _ex);

                ex = _ex;

                if (isSuccess)
                {
                    try
                    {
                        Mat homography = (Mat)_homography;

                        if (score > 0 && homography != null)
                        {
                            Mat warped = new Mat();
                            CvInvoke.WarpPerspective(input, warped, homography, outputWidth, Inter.Cubic, Warp.Default, BorderType.Replicate);

                            output = warped;
                            isSuccess = true;
                        }
                        else
                        {
                            output = null;
                            isSuccess = false;
                        }
                    }
                    catch(Exception __ex)
                    {
                        ex = __ex;
                        isSuccess = false;
                    }
                }

                return isSuccess;
            }
            public override bool ApplyMethod(IInputArray templateImage, IInputArray input, out IOutputArray output, out long matchTime, out Exception ex)
            {
                bool isSuccess = false;
                IOutputArray _homography;

                output = null;
                isSuccess = false;

                if (registrationMethod.ApplyMethod(templateImage, input, useStoredModelFeatures, out _homography, out VectorOfVectorOfDMatch matches, out matchTime, out long score, out ex))
                {
                    try
                    {
                        Mat homography = (Mat)_homography;

                        if (score > 0 && homography != null)
                        {
                            Mat warped = new Mat();
                            CvInvoke.WarpPerspective(input, warped, homography, outputWidth, Inter.Cubic, Warp.Default, BorderType.Replicate);

                            output = warped;
                            isSuccess = true;
                        }
                        else
                        {
                            output = null;
                            isSuccess = false;
                        }
                    }
                    catch(Exception _ex)
                    {
                        ex = _ex;

                        output = null;
                        isSuccess = false;
                    }
                }

                return isSuccess;
            }
            public void StoreModelFeatures(IInputArray model, bool use)
            {
                registrationMethod.StoreModelFeatures(model);
                if(use) useStoredModelFeatures = true;
            }
        }
        #endregion

        #region Alignment Pipeline Results
        public class AlignmentPipelineResults
        {
            #region Enums
            public enum AlignmentMethodResultType
            {
                Successful,
                Failed
            }
            #endregion

            #region Objects
            public class AlignmentMethodResult
            { 
                #region Properties
                internal Template.AlignmentMethodType GetAlignmentMethodType { get => AlignmentMethod.GetAlignmentMethodType; }
                internal Template.AlignmentMethod AlignmentMethod;
                public AlignmentMethodResultType GetAlignmentMethodResultType { get => alignmentMethodResultType; }
                private AlignmentMethodResultType alignmentMethodResultType;
                public Image<Gray, byte> InputImage { get; set; }
                public Image<Gray, byte> OutputImage { get; set; }
                public long AlignmentTime { get; set; }
                #endregion

                internal AlignmentMethodResult(Template.AlignmentMethod alignmentMethod, AlignmentMethodResultType alignmentMethodResultType, Image<Gray, byte> inputImage, Image<Gray, byte> outputImage, long alignmentTime)
                {
                    AlignmentMethod = alignmentMethod;
                    InputImage = inputImage;
                    OutputImage = outputImage;
                    AlignmentTime = alignmentTime;

                    this.alignmentMethodResultType = alignmentMethodResultType;
                }
            }
            public class AnchorAlignmentMethodResult : AlignmentMethodResult
            {
                #region Properties
                internal AnchorAlignmentMethod.Anchor[] MainAnchors = new AnchorAlignmentMethod.Anchor[0];
                public RectangleF[] DetectedAnchors = new RectangleF[0];
                public RectangleF[] WarpedAnchors = new RectangleF[0];
                public RectangleF MainTestRegion = new RectangleF();
                public RectangleF WarpedTestRegion = new RectangleF();
                #endregion

                internal AnchorAlignmentMethodResult(AlignmentMethod alignmentMethod, AlignmentMethodResultType alignmentMethodResultType, Image<Gray, byte> inputImage, Image<Gray, byte> outputImage, long alignmentTime, Template.AnchorAlignmentMethod.Anchor[] mainAnchors, RectangleF[] detectedAnchors, RectangleF[] warpedAnchors, RectangleF mainTestRegion, RectangleF warpedTestRegion) : base(alignmentMethod, alignmentMethodResultType, inputImage, outputImage, alignmentTime)
                {
                    MainAnchors = mainAnchors;
                    DetectedAnchors = detectedAnchors;
                    WarpedAnchors = warpedAnchors;
                    MainTestRegion = mainTestRegion;
                    WarpedTestRegion = warpedTestRegion;
                }
            }
            public class RegistrationAlignmentMethodResult : AlignmentMethodResult
            {
                internal RegistrationAlignmentMethodResult(Template.AlignmentMethod alignmentMethod, AlignmentMethodResultType alignmentMethodResultType, Image<Gray, byte> inputImage, Image<Gray, byte> outputImage, long alignmentTime) : base(alignmentMethod, alignmentMethodResultType, inputImage, outputImage, alignmentTime)
                {
                }
            }
            #endregion

            #region Properties
            public List<AlignmentMethodResult> AlignmentMethodTestResultsList { get; set; }
            #endregion

            public AlignmentPipelineResults(List<AlignmentMethodResult> alignmentMethodTestResultsList)
            {
                AlignmentMethodTestResultsList = alignmentMethodTestResultsList;
            }
        }
        #endregion

        [Serializable]
        internal class Data
        {
            internal string TemplateName;
            internal string TemplateLocation;
            internal string TemplateDataDirectory;

            internal TemplateImage GetTemplateImage { get => templateImage; }
            private TemplateImage templateImage;

            internal List<AlignmentMethod> GetAlignmentPipeline { get => alignmentPipeline; }
            private List<AlignmentMethod> alignmentPipeline = new List<AlignmentMethod>();

            internal Data(string templateName, string templateLocation, TemplateImage templateImage, List<AlignmentMethod> alignmentPipeline)
            {
                TemplateName = templateName;
                TemplateLocation = templateLocation;
                TemplateDataDirectory = Utilities.Memory.LSTM.GetTemplateDataPath(TemplateLocation);

                this.templateImage = templateImage;
                this.alignmentPipeline = alignmentPipeline;
            }

            internal void Initialize()
            {
                GetTemplateImage.Initialize();
            }

            internal void SetTemplateImage(TemplateImage templateImage)
            {
                this.templateImage = templateImage;
            }

            internal void SetAlignmentPipeline(List<AlignmentMethod> alignmentPipeline)
            {
                this.alignmentPipeline = alignmentPipeline;
            }
        }
        #endregion

        #region Properties
        public string GetTemplateName { get { return TemplateData.TemplateName; } set { } }
        public TemplateImage GetTemplateImage { get { return TemplateData.GetTemplateImage; } set { } }
        public string GetTemplateLocation { get { return TemplateData.TemplateLocation; } set { } }
        #endregion

        #region Events
        public delegate bool OnTemplateDataCallback(Data templateData);
        public delegate Template OnTemplateNameCallback(string templateName);
        public static event OnTemplateDataCallback OnSaveTemplateEvent;

        public delegate bool OnTemplateConfiguredCallback(Data templateData, Bitmap templateImage);
        public static event OnTemplateConfiguredCallback OnSaveConfiguredTemplateEvent;
        #endregion

        #region Variables
        public Data TemplateData;
        #endregion

        #region Methods
        public Template(Data tmpData)
        {
            TemplateData = tmpData;
            TemplateData.Initialize();
        }
        public static Template CreateTemplate(string tmpName)
        {
            Template newTemplate = new Template(new Data(tmpName, "", TemplateImage.Empty(), new List<AlignmentMethod>()));
            SaveTemplate(newTemplate.TemplateData);
            return newTemplate;
        }
        internal static async Task<bool> ChangeTemplateName(string oldName, string newName)
        {
            bool result = false;
            Template template = await LoadTemplate(oldName);
            template.TemplateData.TemplateName = newName;

            string newNameLocation = template.TemplateData.TemplateLocation.Replace(oldName, newName);
            Directory.Move(template.TemplateData.TemplateLocation, newNameLocation);
            template.TemplateData.TemplateLocation = newNameLocation;
            template.TemplateData.TemplateDataDirectory = Utilities.Memory.LSTM.GetTemplateDataPath(newNameLocation);

            result = SaveTemplate(template.TemplateData);
            return result;
        }
        internal static bool SaveTemplate(Data tempData, bool saveImage = false)
        {
            bool isSaved = true;

            try
            {
                if(!saveImage)
                    OnSaveTemplateEvent?.Invoke(tempData);
                else
                    OnSaveConfiguredTemplateEvent?.Invoke(tempData, tempData.GetTemplateImage.GetBitmap);
            }
            catch (Exception ex)
            {
                isSaved = false;
            }
            return isSaved;
        }
        internal static bool SaveTemplate(Data tempData, Image<Gray, byte> configuredTemplateImage)
        {
            bool isSaved = true;

            try
            {
                OnSaveConfiguredTemplateEvent?.Invoke(tempData, configuredTemplateImage.Bitmap);
            }
            catch (Exception ex)
            {
                isSaved = false;
            }
            return isSaved;
        }

        internal static async Task<Template> LoadTemplate(string templateName)
        {
            return await Task.Run(() => Utilities.Memory.LSTM.LoadTemplate(templateName));
        }
        internal static async Task<Template> ImportTemplate(string location)
        {
            Template template = null;

            template = await Task.Run(() => Utilities.Memory.LSTM.ImportTemplate(location));

            return template;
        }
        internal static async Task<bool> DeleteTemplate(string templateName)
        {
            return await Task.Run(() => Utilities.Memory.LSTM.DeleteTemplate(templateName));
        }

        internal void SetTemplateImage(TemplateImage image)
        {
            this.TemplateData.SetTemplateImage(image);
        }
        internal void SetAlignmentPipeline(List<AlignmentMethod> alignmentPipeline)
        {
            this.TemplateData.SetAlignmentPipeline(alignmentPipeline);
        }
        #endregion
    }
}
