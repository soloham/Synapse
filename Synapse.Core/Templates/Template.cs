using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Features2D;
using Emgu.CV.Flann;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Synapse.Utilities;
using Synapse.Utilities.Attributes;
using Synapse.Utilities.Extensions;
using Synapse.Utilities.Memory;
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
    public class Template
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
        public class TemplateImage
        {
            public Bitmap GetBitmap { get { return bitmap == null ? CvInvoke.Imread(ImageLocation, ImreadModes.Grayscale).Bitmap : bitmap; } }
            private Bitmap bitmap;
            public Size Size { get => templateSize; set => templateSize = value; }
            private Size templateSize;
            public double TemplateScale { get => templateScale; set => templateScale = value; }
            private double templateScale;
            public double DeskewAngle { get => deskewPercent; set => deskewPercent = value; }
            private double deskewPercent;
            public string ImageLocation { get => templateImageLocation; set => templateImageLocation = value; }
            private string templateImageLocation;
            public Image<Gray, byte> GetGrayImage { get => grayImage; set => grayImage = value; }
            private Image<Gray, byte> grayImage;
            public TemplateImage(Size templateSize, double templateScale, double deskewPercent)
            {
                this.templateSize = templateSize;
                this.templateScale = templateScale;
                this.deskewPercent = deskewPercent;
            }

            public void Initialize()
            {
                bitmap = string.IsNullOrEmpty(ImageLocation)? null : new Bitmap(ImageLocation);

                if(bitmap != null)
                    grayImage = new Image<Gray, byte>(bitmap);
            }

            public void SetBitmap(Bitmap bitmap)
            {
                this.bitmap = new Bitmap(bitmap);if(bitmap != null)
                grayImage = new Image<Gray, byte>(bitmap);
            }
            public static TemplateImage Empty()
            {
                return new TemplateImage(Size.Empty, 1, 0);
            }
        }

        #region Alignment Pipeline
        [Serializable]
        public abstract class AlignmentMethod
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

            public abstract bool ApplyMethod(IInputArray input, out IOutputArray output, out Mat homography, out long matchTime, out Exception ex);
            public abstract bool ApplyMethod(IInputArray template, IInputArray input, out IOutputArray output, out Mat homography, out long matchTime, out Exception ex);

        }
        [Serializable]
        public class AnchorAlignmentMethod : AlignmentMethod
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

            public override bool ApplyMethod(IInputArray input, out IOutputArray output, out Mat homography, out long matchTime, out Exception ex)
            {
                bool isSuccess = false;
                var inputImg = (Image<Gray, byte>)input;
                CvInvoke.Resize(inputImg, inputImg, downscaleSize);

                Mat _output = new Mat();
                homography = null;

                PointF[] anchorCoordinates = new PointF[anchors.Count];

                ex = new Exception();

                Stopwatch watch = new Stopwatch();
                watch.Start();

                try
                {
                    for (int i = 0; i < anchors.Count; i++)
                    {
                        Anchor curAnchor = anchors[i];

                        Mat result = null;
                        CvInvoke.MatchTemplate(inputImg.Mat, curAnchor.GetAnchorImage, result, TemplateMatchingType.CcoeffNormed);

                        Point[] Max_Loc, Min_Loc;
                        double[] min, max;

                        result.MinMax(out min, out max, out Min_Loc, out Max_Loc);

                        if (max[0] > 0.85)
                            isSuccess = true;

                        anchorCoordinates[i] = Max_Loc[0];
                    }

                    homography = CvInvoke.FindHomography(anchorCoordinates, mainAnchorCoordinates, RobustEstimationAlgorithm.Ransac);
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
            public override bool ApplyMethod(IInputArray templateImage, IInputArray input, out IOutputArray output, out Mat homography, out long matchTime, out Exception ex)
            {
                bool isSuccess = false;
                var inputImg = (Mat)input;
                CvInvoke.Resize(inputImg, inputImg, downscaleSize);

                Mat _output = new Mat();
                homography = null;

                PointF[] anchorCoordinates = new PointF[anchors.Count];

                ex = new Exception();

                Stopwatch watch = new Stopwatch();
                watch.Start();

                try
                {
                    for (int i = 0; i < anchors.Count; i++)
                    {
                        Anchor curAnchor = anchors[i];

                        Mat result = null;
                        CvInvoke.MatchTemplate(inputImg, curAnchor.GetAnchorImage, result, TemplateMatchingType.CcoeffNormed);

                        Point[] Max_Loc, Min_Loc;
                        double[] min, max;

                        result.MinMax(out min, out max, out Min_Loc, out Max_Loc);

                        if (max[0] > 0.85)
                            isSuccess = true;

                        anchorCoordinates[i] = Max_Loc[0];
                    }

                    homography = CvInvoke.FindHomography(anchorCoordinates, mainAnchorCoordinates, RobustEstimationAlgorithm.Ransac);
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
            public bool ApplyMethod(IInputArray input, out IOutputArray output, out RectangleF[] detectedAnchors, out RectangleF[] warpedAnchors, out RectangleF[] scaledMainAnchors, out RectangleF scaledMainTestRegion, out Mat homography, out long matchTime, out Exception ex)
            {
                bool isSuccess = false;
                Mat inputImg = (Mat)input;
                Mat resizedInputImg = new Mat();
                //var resizedInputImg = downscaleSize != Size.Empty?  inputImg.Resize(downscaleSize.Width, downscaleSize.Height, Inter.Cubic) : inputImg;
                if (downscaleSize != Size.Empty)
                    CvInvoke.Resize(inputImg, resizedInputImg, downscaleSize);
                //inputImg = inputImg.Resize(outputSize.Width, outputSize.Height, Inter.Cubic);
                Mat _output = new Mat();
                homography = null;

                var anchorRegions = new RectangleF[anchors.Count];
                detectedAnchors = new RectangleF[anchors.Count];
                warpedAnchors = new RectangleF[anchors.Count];
                scaledMainAnchors = new RectangleF[anchors.Count];
                scaledMainTestRegion = new RectangleF();

                PointF[] anchorCoordinates = new PointF[anchors.Count];

                ex = new Exception();

                Stopwatch watch = new Stopwatch();
                watch.Start();

                try
                { 
                    Mat result = new Mat();
                    for (int i = 0; i < anchors.Count; i++)
                    {
                        Anchor curAnchor = anchors[i];

                        CvInvoke.MatchTemplate(resizedInputImg, curAnchor.GetAnchorImage, result, TemplateMatchingType.CcoeffNormed);

                        Point[] Max_Loc, Min_Loc;
                        double[] min, max;

                        result.MinMax(out min, out max, out Min_Loc, out Max_Loc);

                        if (max[0] > 0.65)
                            isSuccess = true;

                        //if(isSuccess)
                        //{
                        //    double threshold = max[0] * 0.7;
                        //    List<Point> allPossibleMatches = new List<Point>();
                        //    for (int i0 = 0; i0 < result.Width; i0++)
                        //    {
                        //        for (int j = 0; j < result.Height; j++)
                        //        {
                        //            double dataValue = (double)result.GetValue(i0, j);
                        //            if (dataValue >= threshold)
                        //                allPossibleMatches.Add(new Point(i0, j));
                        //        }
                        //    }
                        //}

                        anchorCoordinates[i] = Max_Loc[0];
                        anchorRegions[i] = curAnchor.GetAnchorRegion;
                        detectedAnchors[i] = new RectangleF(anchorCoordinates[i], anchorRegions[i].Size);
                    }
                    detectedAnchors = Functions.ResizeRegions(detectedAnchors, downscaleSize, outputSize);
                    anchorRegions = Functions.ResizeRegions(anchorRegions, downscaleSize, outputSize);
                    scaledMainTestRegion = Functions.ResizeRegion(testAnchor.GetAnchorRegion, downscaleSize, outputSize);
                    var scaledMainAnchorPoints = Functions.ResizePoints(mainAnchorCoordinates, downscaleSize, outputSize);
                    anchorCoordinates = Functions.ResizePoints(anchorCoordinates, downscaleSize, outputSize);

                    homography = CvInvoke.FindHomography(anchorCoordinates, scaledMainAnchorPoints, RobustEstimationAlgorithm.Ransac);
                    CvInvoke.WarpPerspective(inputImg, _output, homography, outputSize);
                    var warpedPoints = CvInvoke.PerspectiveTransform(anchorCoordinates, homography);

                    for (int i = 0; i < anchors.Count; i++)
                    {
                        warpedAnchors[i] = new RectangleF(warpedPoints[i], detectedAnchors[i].Size);
                        scaledMainAnchors[i] = new RectangleF(scaledMainAnchorPoints[i], anchorRegions[i].Size);
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
            public bool ApplyMethod(IInputArray templateImage, IInputArray input, out IOutputArray output, out PointF[] detectedAnchors, out PointF[] warpedAnchors, out PointF warpedTestPoint, out Mat homography, out long matchTime, out Exception ex)
            {
                bool isSuccess = false;
                var inputImg = (Image<Gray, byte>)input;
                CvInvoke.Resize(inputImg, inputImg, downscaleSize);
                Mat _output = new Mat();
                homography = null;

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

                    Mat result = null;
                    CvInvoke.MatchTemplate(inputImg.Mat, curAnchor.GetAnchorImage, result, TemplateMatchingType.CcoeffNormed);

                    Point[] Max_Loc, Min_Loc;
                    double[] min, max;

                    result.MinMax(out min, out max, out Min_Loc, out Max_Loc);

                    if (max[0] > 0.85)
                        isSuccess = true;

                    anchorCoordinates[i] = Max_Loc[0];

                    detectedAnchors[i] = anchorCoordinates[i];
                }
                warpedTestPoint = anchorCoordinates[anchorCoordinates.Length - 1];

                homography = CvInvoke.FindHomography(anchorCoordinates, mainAnchorCoordinates, RobustEstimationAlgorithm.Ransac);
                CvInvoke.WarpPerspective(input, _output, homography, outputSize);
                warpedAnchors = CvInvoke.PerspectiveTransform(anchorCoordinates, homography);
                watch.Stop();

                matchTime = watch.ElapsedMilliseconds;

                output = _output;
                return isSuccess;
            }

            public void SetAnchors(List<Anchor> anchors)
            {
                this.anchors = anchors;
            }

            public void SetTestAnchor(Anchor testAnchor)
            {
                this.testAnchor = testAnchor;
            }

            #endregion
        }
        [Serializable]
        public class RegistrationAlignmentMethod : AlignmentMethod
        {
            #region Enums
            public enum RegistrationMethodType
            {
                AKAZE,
                KAZE
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
                [Serializable]
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
                                            result = true;
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
                [Serializable]
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
                            using (Mat modelDescriptors = useStoredModelFeatures ? GetStoredModelDescriptors.Clone() : new Mat())
                            {
                                watch.Start();

                                //akaze = new AKAZE();
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
                                        matcher.Add(observedDescriptors);

                                        matcher.KnnMatch(modelDescriptors, matches, k, null);
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

                                            result = true;
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

            public override bool ApplyMethod(IInputArray input, out IOutputArray output, out Mat homography, out long matchTime, out Exception ex)
            {
                bool isSuccess = false;
                IOutputArray _homography;
                homography = null;

                output = null;

                isSuccess = registrationMethod.ApplyMethod(sourceImage, input, useStoredModelFeatures, out _homography, out VectorOfVectorOfDMatch matches, out matchTime, out long score, out Exception _ex);

                ex = _ex;

                if (isSuccess)
                {
                    try
                    {
                        homography = (Mat)_homography;

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
            public override bool ApplyMethod(IInputArray templateImage, IInputArray input, out IOutputArray output, out Mat homography, out long matchTime, out Exception ex)
            {
                bool isSuccess = false;
                IOutputArray _homography;
                homography = null;

                output = null;
                isSuccess = false;

                if (registrationMethod.ApplyMethod(templateImage, input, useStoredModelFeatures, out _homography, out VectorOfVectorOfDMatch matches, out matchTime, out long score, out ex))
                {
                    try
                    {
                        homography = (Mat)_homography;

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

        public void Activate()
        {
            TemplateData.IsActivatedd = true;
            SaveTemplate(TemplateData);
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
                public Template.AlignmentMethodType GetAlignmentMethodType { get => AlignmentMethod.GetAlignmentMethodType; }
                public Template.AlignmentMethod AlignmentMethod;
                public AlignmentMethodResultType GetAlignmentMethodResultType { get => alignmentMethodResultType; }
                private AlignmentMethodResultType alignmentMethodResultType;
                public Mat InputImage { get; set; }
                public Mat OutputImage { get; set; }
                public long AlignmentTime { get; set; }
                public Mat AlignmentHomography { get; set; }
                #endregion

                public AlignmentMethodResult(Template.AlignmentMethod alignmentMethod, AlignmentMethodResultType alignmentMethodResultType, Mat alignmentHomography, Mat inputImage, Mat outputImage, long alignmentTime)
                {
                    AlignmentMethod = alignmentMethod;
                    InputImage = inputImage;
                    OutputImage = outputImage;
                    AlignmentTime = alignmentTime;

                    this.alignmentMethodResultType = alignmentMethodResultType;
                    AlignmentHomography = alignmentHomography;
                }
            }
            public class AnchorAlignmentMethodResult : AlignmentMethodResult
            {
                #region Properties
                public AnchorAlignmentMethod.Anchor[] MainAnchors = new AnchorAlignmentMethod.Anchor[0];
                public RectangleF[] DetectedAnchors = new RectangleF[0];
                public RectangleF[] WarpedAnchors = new RectangleF[0];
                public RectangleF[] ScaledMainAnchors = new RectangleF[0];
                public RectangleF ScaledMainTestRegion = new RectangleF();
                #endregion

                public AnchorAlignmentMethodResult(AlignmentMethod alignmentMethod, AlignmentMethodResultType alignmentMethodResultType, Mat alignmentHomography, Mat inputImage, Mat outputImage, long alignmentTime, Template.AnchorAlignmentMethod.Anchor[] mainAnchors, RectangleF[] detectedAnchors, RectangleF[] warpedAnchors, RectangleF[] scaledMainAnchors, RectangleF scaledMainTestRegion) : base(alignmentMethod, alignmentMethodResultType, alignmentHomography, inputImage, outputImage, alignmentTime)
                {
                    MainAnchors = mainAnchors;
                    DetectedAnchors = detectedAnchors;
                    WarpedAnchors = warpedAnchors;
                    ScaledMainAnchors = scaledMainAnchors;
                    ScaledMainTestRegion = scaledMainTestRegion;
                }
            }
            public class RegistrationAlignmentMethodResult : AlignmentMethodResult
            {
                public RegistrationAlignmentMethodResult(Template.AlignmentMethod alignmentMethod, AlignmentMethodResultType alignmentMethodResultType, Mat alignmentHomography, Mat inputImage, Mat outputImage, long alignmentTime) : base(alignmentMethod, alignmentMethodResultType, alignmentHomography, inputImage, outputImage, alignmentTime)
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
        public class Data
        {
            public string TemplateName;
            public string TemplateLocation;
            public string TemplateDataDirectory;

            public TemplateImage GetTemplateImage { get => templateImage; }
            private TemplateImage templateImage;

            public List<AlignmentMethod> GetAlignmentPipeline { get => alignmentPipeline; }
            private List<AlignmentMethod> alignmentPipeline = new List<AlignmentMethod>();

            public bool IsActivatedd { get; set; }
            public Data(string templateName, string templateLocation, TemplateImage templateImage, List<AlignmentMethod> alignmentPipeline)
            {
                TemplateName = templateName;
                TemplateLocation = templateLocation;
                TemplateDataDirectory = Utilities.Memory.LSTM.GetTemplateDataPath(TemplateLocation);

                this.templateImage = templateImage;
                this.alignmentPipeline = alignmentPipeline;
            }

            public void Initialize()
            {
                GetTemplateImage.Initialize();
            }

            public void SetTemplateImage(TemplateImage templateImage)
            {
                this.templateImage = templateImage;
            }

            public void SetAlignmentPipeline(List<AlignmentMethod> alignmentPipeline)
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
            string ImageLocation = TemplateData.GetTemplateImage.ImageLocation;
            TemplateData.GetTemplateImage.ImageLocation = string.IsNullOrEmpty(ImageLocation) ? null : File.Exists(ImageLocation) ? ImageLocation : LSTM.GetTemplateImagePath(tmpData.TemplateName);
            TemplateData.Initialize();
        }
        public static Template CreateTemplate(string tmpName)
        {
            Template newTemplate = new Template(new Data(tmpName, "", TemplateImage.Empty(), new List<AlignmentMethod>()));
            SaveTemplate(newTemplate.TemplateData);
            return newTemplate;
        }
        public static async Task<bool> ChangeTemplateName(string oldName, string newName)
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
        public static bool SaveTemplate(Data tempData, bool saveImage = false)
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
        public static bool SaveTemplate(Data tempData, Image<Gray, byte> configuredTemplateImage)
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

        public static async Task<Template> LoadTemplate(string templateName)
        {
            return await Task.Run(() => Utilities.Memory.LSTM.LoadTemplate(templateName));
        }
        public static async Task<Template> ImportTemplate(string location)
        {
            Template template = null;

            template = await Task.Run(() => Utilities.Memory.LSTM.ImportTemplate(location));

            return template;
        }
        public static async Task<bool> DeleteTemplate(string templateName)
        {
            return await Task.Run(() => Utilities.Memory.LSTM.DeleteTemplate(templateName));
        }

        public void SetTemplateImage(TemplateImage image)
        {
            this.TemplateData.SetTemplateImage(image);
        }
        public void SetAlignmentPipeline(List<AlignmentMethod> alignmentPipeline)
        {
            this.TemplateData.SetAlignmentPipeline(alignmentPipeline);
        }
        public bool GetAlignedImage(string rowSheetPath, Shared.Enums.ProcessingEnums.RereadType rereadType, out Mat result)
        {
            result = new Mat();
            Mat unAlignedMat = new Mat(rowSheetPath, Emgu.CV.CvEnum.ImreadModes.Grayscale);
            switch (rereadType)
            {
                case Shared.Enums.ProcessingEnums.RereadType.NORMAL:
                    break;
                case Shared.Enums.ProcessingEnums.RereadType.ROTATE_C_90:
                    CvInvoke.Rotate(unAlignedMat, unAlignedMat, Emgu.CV.CvEnum.RotateFlags.Rotate90Clockwise);
                    break;
                case Shared.Enums.ProcessingEnums.RereadType.ROTATE_180:
                    CvInvoke.Rotate(unAlignedMat, unAlignedMat, Emgu.CV.CvEnum.RotateFlags.Rotate180);
                    break;
                case Shared.Enums.ProcessingEnums.RereadType.ROTATE_AC_90:
                    CvInvoke.Rotate(unAlignedMat, unAlignedMat, Emgu.CV.CvEnum.RotateFlags.Rotate90CounterClockwise);
                    break;
            }

            try
            {
                var resultImg = AlignSheet(unAlignedMat, out AlignmentPipelineResults alignmentPipelineResults);
                result = resultImg;
                unAlignedMat.Dispose();
                //CvInvoke.WarpPerspective(unAligned, result, GetAlignmentHomography, unAligned.Size, Emgu.CV.CvEnum.Inter.Cubic, Emgu.CV.CvEnum.Warp.Default, Emgu.CV.CvEnum.BorderType.Default);
                return alignmentPipelineResults.AlignmentMethodTestResultsList[0].GetAlignmentMethodResultType == Templates.Template.AlignmentPipelineResults.AlignmentMethodResultType.Successful;
            }
            catch (Exception ex)
            {
                result = unAlignedMat;
                return false;
            }
        }

        public Mat AlignSheet(Mat sheetImage, out AlignmentPipelineResults alignmentPipelineResults, bool log = true)
        {
            Mat outputImage = sheetImage;
            List<AlignmentMethod> alignmentPipeline = TemplateData.GetAlignmentPipeline;
            Mat grayImage = GetTemplateImage.GetGrayImage.Mat;

            alignmentPipelineResults = null;

            if (alignmentPipeline.Count <= 0)
                return outputImage;

            List<AlignmentPipelineResults.AlignmentMethodResult> alignmentMethodResults = new List<AlignmentPipelineResults.AlignmentMethodResult>();

            IOutputArray outputImageArr;
            CvInvoke.Resize(sheetImage, outputImage, grayImage.Size);
            //outputImage = sheetImage.Resize(grayImage.Width, grayImage.Height, Inter.Cubic);
            for (int i = 0; i < alignmentPipeline.Count; i++)
            {
                Exception exception = null;

                AlignmentPipelineResults.AlignmentMethodResult alignmentMethodResult = null;
                AlignmentMethod alignmentMethod = alignmentPipeline[i];

                if (alignmentMethod.PipelineIndex == -1) //|| ommittedAlignmetMethodIndeces.Contains(alignmentMethod.PipelineIndex))
                    continue;

                if (alignmentMethod.GetAlignmentMethodType == AlignmentMethodType.Anchors)
                {
                    var aIM = (AnchorAlignmentMethod)alignmentMethod;
                    bool isSuccess = aIM.ApplyMethod(outputImage, out outputImageArr, out RectangleF[] detectedAnchors, out RectangleF[] warpedAnchors, out RectangleF[] scaledMainAnchorRegions, out RectangleF scaledMainTestRegion, out Mat alignmentHomography, out long alignmentTime, out exception);
                    var mainAnchors = aIM.GetAnchors.ToArray();
                    if (isSuccess)
                    {
                        var outputMat = (Mat)outputImageArr;
                        outputImage = outputMat;
                    }
                    AlignmentPipelineResults.AnchorAlignmentMethodResult anchorAlignmentMethodResult = new AlignmentPipelineResults.AnchorAlignmentMethodResult(alignmentMethod, isSuccess ? AlignmentPipelineResults.AlignmentMethodResultType.Successful : AlignmentPipelineResults.AlignmentMethodResultType.Failed, null, null, null, alignmentTime, mainAnchors, detectedAnchors, warpedAnchors, scaledMainAnchorRegions, scaledMainTestRegion);
                    alignmentMethodResult = anchorAlignmentMethodResult;
                }
                else
                {
                    bool isSuccess = alignmentMethod.ApplyMethod(grayImage, outputImage, out outputImageArr, out Mat alignmentHomography, out long alignmentTime, out exception);
                    if (isSuccess)
                    {
                        var outputMat = (Mat)outputImageArr;
                        outputImage = outputMat;
                    }
                    alignmentMethodResult = new AlignmentPipelineResults.AlignmentMethodResult(alignmentMethod, isSuccess ? AlignmentPipelineResults.AlignmentMethodResultType.Successful : AlignmentPipelineResults.AlignmentMethodResultType.Failed, null, null, null, alignmentTime);
                }

                alignmentMethodResults.Add(alignmentMethodResult);

                if (alignmentMethodResult.GetAlignmentMethodResultType == AlignmentPipelineResults.AlignmentMethodResultType.Failed)
                {
                    string personnelData = exception.Message;

                    if (exception.StackTrace == null)
                    {
                        if(log)
                            Messages.ShowError("An error occured while applying the method: '" + alignmentMethod.MethodName + "' \n\n For concerned personnel: " + personnelData);
                        return outputImage;
                    }

                    for (int i0 = exception.StackTrace.Length - 1; i0 > 0; i0--)
                    {
                        if (exception.StackTrace[i0] == '/' || exception.StackTrace[i0] == '\\')
                        {
                            personnelData = exception.StackTrace.Substring(i0 + 1);
                            break;
                        }
                    }
                    Messages.ShowError("An error occured while applying the method: '" + alignmentMethod.MethodName + "' \n\n For concerned personnel: " + personnelData);
                }
            }

            alignmentPipelineResults = new AlignmentPipelineResults(alignmentMethodResults);
            //OnResultsGeneratedEvent?.Invoke(alignmentPipelineResults);

            return outputImage;
        }

        #endregion
    }
}
