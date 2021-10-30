namespace Synapse.Core.Templates
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Threading.Tasks;

    using Emgu.CV;
    using Emgu.CV.CvEnum;
    using Emgu.CV.Features2D;
    using Emgu.CV.Flann;
    using Emgu.CV.Structure;
    using Emgu.CV.Util;

    using Synapse.Shared.Enums;
    using Synapse.Utilities;
    using Synapse.Utilities.Attributes;
    using Synapse.Utilities.Memory;

    public class Template
    {
        #region Enums

        public enum AlignmentMethodType
        {
            [EnumDescription("Anchors")] Anchors,
            [EnumDescription("Registration")] Registration
        }

        public enum RegistrationMethod
        {
            [EnumDescription("AKaze")] AKaze,
            [EnumDescription("Kaze")] Kaze,
            [EnumDescription("ORB + SIFT")] ORB_SIFT
        }

        #endregion

        #region Objects

        [Serializable]
        public class TemplateImage
        {
            public Bitmap GetBitmap =>
                bitmap == null ? CvInvoke.Imread(this.ImageLocation, ImreadModes.Grayscale).Bitmap : bitmap;

            private Bitmap bitmap;

            public Size Size
            {
                get => templateSize;
                set => templateSize = value;
            }

            private Size templateSize;

            public double TemplateScale
            {
                get => templateScale;
                set => templateScale = value;
            }

            private double templateScale;

            public double DeskewAngle
            {
                get => deskewPercent;
                set => deskewPercent = value;
            }

            private double deskewPercent;

            public string ImageLocation
            {
                get => templateImageLocation;
                set => templateImageLocation = value;
            }

            private string templateImageLocation;

            public Image<Gray, byte> GetGrayImage
            {
                get => grayImage;
                set => grayImage = value;
            }

            private Image<Gray, byte> grayImage;

            public TemplateImage(Size templateSize, double templateScale, double deskewPercent)
            {
                this.templateSize = templateSize;
                this.templateScale = templateScale;
                this.deskewPercent = deskewPercent;
            }

            public void Initialize()
            {
                bitmap = string.IsNullOrEmpty(this.ImageLocation) ? null : new Bitmap(this.ImageLocation);

                if (bitmap != null)
                {
                    grayImage = new Image<Gray, byte>(bitmap);
                }
            }

            public void SetBitmap(Bitmap bitmap)
            {
                this.bitmap = new Bitmap(bitmap);
                if (bitmap != null)
                {
                    grayImage = new Image<Gray, byte>(bitmap);
                }
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
            public AlignmentMethodType GetAlignmentMethodType
            {
                get => alignmentMethodType;
                set { }
            }

            public string MethodName { get; internal set; }
            private AlignmentMethodType alignmentMethodType;
            public int PipelineIndex;

            public AlignmentMethod(string methodName, AlignmentMethodType alignmentMethodType, int pipelineIndex)
            {
                this.MethodName = methodName;
                this.alignmentMethodType = alignmentMethodType;
                PipelineIndex = pipelineIndex;
            }

            public abstract bool ApplyMethod(IInputArray input, out IOutputArray output, out Mat homography,
                out long matchTime, out Exception ex);

            public abstract bool ApplyMethod(IInputArray template, IInputArray input, out IOutputArray output,
                out Mat homography, out long matchTime, out Exception ex);
        }

        [Serializable]
        public class AnchorAlignmentMethod : AlignmentMethod
        {
            #region Objects

            [Serializable]
            public class Anchor
            {
                public RectangleF GetAnchorRegion => anchorRegion;
                private RectangleF anchorRegion;
                public Mat GetAnchorImage { get; }

                public PointF anchorCoordinates;

                public Anchor(RectangleF anchorRegion, Mat anchorImage)
                {
                    this.anchorRegion = anchorRegion;
                    this.GetAnchorImage = anchorImage;

                    this.CalculateAnchorCoordinates();
                }

                private void CalculateAnchorCoordinates()
                {
                    anchorCoordinates = anchorRegion.Location;
                }
            }

            #endregion

            #region Properties

            public List<Anchor> GetAnchors { get; private set; }

            public Anchor GetTestAnchor { get; private set; }

            public Size GetDownscaleSize { get; }

            public double GetDownscaleScale { get; }

            #endregion

            #region Variables

            private Size outputSize;
            private PointF[] mainAnchorCoordinates;

            #endregion

            #region Methods

            public AnchorAlignmentMethod(List<Anchor> anchors, Anchor testAnchor, Size outputSize, int pipelineIndex,
                string methodName, Size downscaleSize, double downscaleScale) : base(methodName,
                AlignmentMethodType.Anchors, pipelineIndex)
            {
                this.GetAnchors = anchors;
                this.GetTestAnchor = testAnchor;
                this.outputSize = outputSize;
                this.GetDownscaleSize = downscaleSize;
                this.GetDownscaleScale = downscaleScale;

                this.ExtractCooridnates();
            }

            #region Public

            public void AddAnchor(RectangleF region, Mat image)
            {
                this.GetAnchors.Add(new Anchor(region, image));

                this.ExtractCooridnates();
            }

            public void RemoveAnchor(int index)
            {
                this.GetAnchors.RemoveAt(index);
                this.ExtractCooridnates();
            }

            #endregion

            #region Private

            private void ExtractCooridnates()
            {
                mainAnchorCoordinates = new PointF[this.GetAnchors.Count];

                for (var i = 0; i < this.GetAnchors.Count; i++)
                    mainAnchorCoordinates[i] = this.GetAnchors[i].anchorCoordinates;
            }

            #endregion

            public override bool ApplyMethod(IInputArray input, out IOutputArray output, out Mat homography,
                out long matchTime, out Exception ex)
            {
                var isSuccess = false;
                var inputImg = (Image<Gray, byte>)input;
                CvInvoke.Resize(inputImg, inputImg, this.GetDownscaleSize);

                var _output = new Mat();
                homography = null;

                var anchorCoordinates = new PointF[this.GetAnchors.Count];

                ex = new Exception();

                var watch = new Stopwatch();
                watch.Start();

                try
                {
                    for (var i = 0; i < this.GetAnchors.Count; i++)
                    {
                        var curAnchor = this.GetAnchors[i];

                        Mat result = null;
                        CvInvoke.MatchTemplate(inputImg.Mat, curAnchor.GetAnchorImage, result,
                            TemplateMatchingType.CcoeffNormed);

                        Point[] Max_Loc, Min_Loc;
                        double[] min, max;

                        result.MinMax(out min, out max, out Min_Loc, out Max_Loc);

                        if (max[0] > 0.85)
                        {
                            isSuccess = true;
                        }

                        anchorCoordinates[i] = Max_Loc[0];
                    }

                    homography = CvInvoke.FindHomography(anchorCoordinates, mainAnchorCoordinates,
                        RobustEstimationAlgorithm.Ransac);
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

            public override bool ApplyMethod(IInputArray templateImage, IInputArray input, out IOutputArray output,
                out Mat homography, out long matchTime, out Exception ex)
            {
                var isSuccess = false;
                var inputImg = (Mat)input;
                CvInvoke.Resize(inputImg, inputImg, this.GetDownscaleSize);

                var _output = new Mat();
                homography = null;

                var anchorCoordinates = new PointF[this.GetAnchors.Count];

                ex = new Exception();

                var watch = new Stopwatch();
                watch.Start();

                try
                {
                    for (var i = 0; i < this.GetAnchors.Count; i++)
                    {
                        var curAnchor = this.GetAnchors[i];

                        Mat result = null;
                        CvInvoke.MatchTemplate(inputImg, curAnchor.GetAnchorImage, result,
                            TemplateMatchingType.CcoeffNormed);

                        Point[] Max_Loc, Min_Loc;
                        double[] min, max;

                        result.MinMax(out min, out max, out Min_Loc, out Max_Loc);

                        if (max[0] > 0.85)
                        {
                            isSuccess = true;
                        }

                        anchorCoordinates[i] = Max_Loc[0];
                    }

                    homography = CvInvoke.FindHomography(anchorCoordinates, mainAnchorCoordinates,
                        RobustEstimationAlgorithm.Ransac);
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

            public bool ApplyMethod(IInputArray input, out IOutputArray output, out RectangleF[] detectedAnchors,
                out RectangleF[] warpedAnchors, out RectangleF[] scaledMainAnchors, out RectangleF scaledMainTestRegion,
                out Mat homography, out long matchTime, out Exception ex)
            {
                var isSuccess = false;
                var inputImg = (Mat)input;
                var resizedInputImg = new Mat();
                //var resizedInputImg = downscaleSize != Size.Empty?  inputImg.Resize(downscaleSize.Width, downscaleSize.Height, Inter.Cubic) : inputImg;
                if (this.GetDownscaleSize != Size.Empty)
                {
                    CvInvoke.Resize(inputImg, resizedInputImg, this.GetDownscaleSize);
                }

                //inputImg = inputImg.Resize(outputSize.Width, outputSize.Height, Inter.Cubic);
                var _output = new Mat();
                homography = null;

                var anchorRegions = new RectangleF[this.GetAnchors.Count];
                detectedAnchors = new RectangleF[this.GetAnchors.Count];
                warpedAnchors = new RectangleF[this.GetAnchors.Count];
                scaledMainAnchors = new RectangleF[this.GetAnchors.Count];
                scaledMainTestRegion = new RectangleF();

                var anchorCoordinates = new PointF[this.GetAnchors.Count];

                ex = new Exception();

                var watch = new Stopwatch();
                watch.Start();

                try
                {
                    var result = new Mat();
                    for (var i = 0; i < this.GetAnchors.Count; i++)
                    {
                        var curAnchor = this.GetAnchors[i];

                        CvInvoke.MatchTemplate(resizedInputImg, curAnchor.GetAnchorImage, result,
                            TemplateMatchingType.CcoeffNormed);

                        Point[] Max_Loc, Min_Loc;
                        double[] min, max;

                        result.MinMax(out min, out max, out Min_Loc, out Max_Loc);

                        if (max[0] > 0.65)
                        {
                            isSuccess = true;
                        }

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

                    detectedAnchors = Functions.ResizeRegions(detectedAnchors, this.GetDownscaleSize, outputSize);
                    anchorRegions = Functions.ResizeRegions(anchorRegions, this.GetDownscaleSize, outputSize);
                    scaledMainTestRegion = Functions.ResizeRegion(this.GetTestAnchor.GetAnchorRegion,
                        this.GetDownscaleSize, outputSize);
                    var scaledMainAnchorPoints =
                        Functions.ResizePoints(mainAnchorCoordinates, this.GetDownscaleSize, outputSize);
                    anchorCoordinates = Functions.ResizePoints(anchorCoordinates, this.GetDownscaleSize, outputSize);

                    homography = CvInvoke.FindHomography(anchorCoordinates, scaledMainAnchorPoints,
                        RobustEstimationAlgorithm.Ransac);
                    CvInvoke.WarpPerspective(inputImg, _output, homography, outputSize);
                    var warpedPoints = CvInvoke.PerspectiveTransform(anchorCoordinates, homography);

                    for (var i = 0; i < this.GetAnchors.Count; i++)
                    {
                        warpedAnchors[i] = new RectangleF(warpedPoints[i], detectedAnchors[i].Size);
                        scaledMainAnchors[i] = new RectangleF(scaledMainAnchorPoints[i], anchorRegions[i].Size);
                    }
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

            public bool ApplyMethod(IInputArray templateImage, IInputArray input, out IOutputArray output,
                out PointF[] detectedAnchors, out PointF[] warpedAnchors, out PointF warpedTestPoint,
                out Mat homography, out long matchTime, out Exception ex)
            {
                var isSuccess = false;
                var inputImg = (Image<Gray, byte>)input;
                CvInvoke.Resize(inputImg, inputImg, this.GetDownscaleSize);
                var _output = new Mat();
                homography = null;

                detectedAnchors = new PointF[this.GetAnchors.Count];
                warpedAnchors = new PointF[this.GetAnchors.Count];
                warpedTestPoint = new PointF();

                var anchorCoordinates = new PointF[this.GetAnchors.Count + 1];

                ex = new Exception();

                var watch = new Stopwatch();
                watch.Start();
                for (var i = 0; i < this.GetAnchors.Count; i++)
                {
                    var curAnchor = this.GetAnchors[i];

                    Mat result = null;
                    CvInvoke.MatchTemplate(inputImg.Mat, curAnchor.GetAnchorImage, result,
                        TemplateMatchingType.CcoeffNormed);

                    Point[] Max_Loc, Min_Loc;
                    double[] min, max;

                    result.MinMax(out min, out max, out Min_Loc, out Max_Loc);

                    if (max[0] > 0.85)
                    {
                        isSuccess = true;
                    }

                    anchorCoordinates[i] = Max_Loc[0];

                    detectedAnchors[i] = anchorCoordinates[i];
                }

                warpedTestPoint = anchorCoordinates[anchorCoordinates.Length - 1];

                homography = CvInvoke.FindHomography(anchorCoordinates, mainAnchorCoordinates,
                    RobustEstimationAlgorithm.Ransac);
                CvInvoke.WarpPerspective(input, _output, homography, outputSize);
                warpedAnchors = CvInvoke.PerspectiveTransform(anchorCoordinates, homography);
                watch.Stop();

                matchTime = watch.ElapsedMilliseconds;

                output = _output;
                return isSuccess;
            }

            public void SetAnchors(List<Anchor> anchors)
            {
                this.GetAnchors = anchors;
            }

            public void SetTestAnchor(Anchor testAnchor)
            {
                this.GetTestAnchor = testAnchor;
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
                public VectorOfKeyPoint GetStoredModelKeyPoints { get; private set; }

                public Mat GetStoredModelDescriptors { get; private set; }

                #endregion

                public RegistrationMethod(RegistrationMethodType getRegistrationMethodType)
                {
                    this.GetRegistrationMethodType = getRegistrationMethodType;
                }

                #region Methods

                public abstract bool ApplyMethod(IInputArray source, IInputArray observed, bool useStoredModelFeatures,
                    out IOutputArray homography, out VectorOfVectorOfDMatch matches, out long matchTime, out long score,
                    out Exception ex);

                public abstract bool GenerateFeatures(IInputArray source, out VectorOfKeyPoint generatedKeyPoints,
                    out Mat generatedDescriptors);

                public void StoreModelFeatures(IInputArray modelImage)
                {
                    VectorOfKeyPoint _storedModelKeyPoints;
                    Mat _storedModelDescriptors;

                    this.GenerateFeatures(modelImage, out _storedModelKeyPoints, out _storedModelDescriptors);

                    this.GetStoredModelKeyPoints = _storedModelKeyPoints;
                    this.GetStoredModelDescriptors = _storedModelDescriptors;
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
                    public KazeData(bool extended, bool upright, double threshold, int octaves, int sublevels,
                        KAZE.Diffusivity diffusivity)
                    {
                        this.Extended = extended;
                        this.Upright = upright;
                        this.Threshold = threshold;
                        this.Octaves = octaves;
                        this.Sublevels = sublevels;
                        this.Diffusivity = diffusivity;
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

                public KazeData GetKazeData { get; }

                public KAZE GetKAZE { get; }

                #endregion

                #region Methods

                #region Public

                public KazeRegistrationMethod(bool extended, bool upright, double threshold, int octaves, int sublevels,
                    KAZE.Diffusivity diffusivity) : base(RegistrationMethodType.KAZE)
                {
                    this.GetKazeData = new KazeData(extended, upright, threshold, octaves, sublevels, diffusivity);
                    this.GetKAZE = new KAZE(this.GetKazeData.Extended, this.GetKazeData.Upright,
                        (float)this.GetKazeData.Threshold, this.GetKazeData.Octaves, this.GetKazeData.Sublevels,
                        this.GetKazeData.Diffusivity);
                }

                public KazeRegistrationMethod(KazeData kazeData) : base(RegistrationMethodType.KAZE)
                {
                    this.GetKazeData = kazeData;
                    this.GetKAZE = new KAZE(kazeData.Extended, kazeData.Upright, (float)kazeData.Threshold,
                        kazeData.Octaves, kazeData.Sublevels, kazeData.Diffusivity);
                }

                #endregion

                #region Private

                private bool ExtractHomography(Mat modelImage, Mat observedImage, bool useStoredModelFeatures,
                    out long matchTime, out VectorOfKeyPoint modelKeyPoints, out VectorOfKeyPoint observedKeyPoints,
                    out VectorOfVectorOfDMatch matches, out Mat mask, out Mat homography, out long score)
                {
                    score = 0;
                    mask = new Mat();
                    matchTime = 0;

                    var k = 2;
                    var uniquenessThreshold = 0.75;

                    var watch = new Stopwatch();
                    homography = null;

                    modelKeyPoints = new VectorOfKeyPoint();
                    observedKeyPoints = new VectorOfKeyPoint();
                    matches = new VectorOfVectorOfDMatch();

                    var result = false;
                    try
                    {
                        using (var uModelImage = modelImage.GetUMat(AccessType.Read))
                        using (var uObservedImage = observedImage.GetUMat(AccessType.Read))
                        {
                            using (var observedDescriptors = new Mat())
                            using (var modelDescriptors = useStoredModelFeatures
                                ? this.GetStoredModelDescriptors.Clone()
                                : new Mat())
                            {
                                watch.Start();

                                if (!useStoredModelFeatures)
                                {
                                    this.GetKAZE.DetectAndCompute(uModelImage, null, modelKeyPoints, modelDescriptors,
                                        false);
                                }
                                else
                                {
                                    modelKeyPoints = this.GetStoredModelKeyPoints;
                                }

                                this.GetKAZE.DetectAndCompute(uObservedImage, null, observedKeyPoints,
                                    observedDescriptors, false);

                                if (modelKeyPoints.Size > 0 && observedKeyPoints.Size > 0)
                                {
                                    // KdTree for faster results / less accuracy
                                    using (var ip = new KdTreeIndexParams())
                                    using (var sp = new SearchParams())
                                    using (DescriptorMatcher matcher = new FlannBasedMatcher(ip, sp))
                                    {
                                        matcher.Add(modelDescriptors);

                                        matcher.KnnMatch(observedDescriptors, matches, k);
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
                                            var total = 0;
                                            for (var i = 0; i < matches.Size; i++)
                                            {
                                                foreach (var e in matches[i].ToArray())
                                                    ++total;
                                                if (mask.GetRawData(i)[0] == 0)
                                                {
                                                    continue;
                                                }

                                                foreach (var e in matches[i].ToArray())
                                                    ++score;
                                            }

                                            score = (long)(score / (float)total * 100);
                                            // <----------------------------------------------

                                            var nonZeroCount = CvInvoke.CountNonZero(mask);
                                            if (nonZeroCount >= 4)
                                            {
                                                nonZeroCount =
                                                    Features2DToolbox.VoteForSizeAndOrientation(modelKeyPoints,
                                                        observedKeyPoints, matches, mask, 1.6, 20);
                                                if (nonZeroCount >= 4)
                                                {
                                                    homography =
                                                        Features2DToolbox.GetHomographyMatrixFromMatchedFeatures(
                                                            modelKeyPoints, observedKeyPoints, matches, mask, 2);
                                                }
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

                public override bool ApplyMethod(IInputArray source, IInputArray observed, bool useStoredModelFeatures,
                    out IOutputArray homography, out VectorOfVectorOfDMatch matches, out long matchTime, out long score,
                    out Exception ex)
                {
                    var isSuccess = false;

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
                        isSuccess = this.ExtractHomography(sourceImg.Mat, observedImg.Mat, useStoredModelFeatures,
                            out matchTime, out modelKeyPoints, out observedKeyPoints, out matches,
                            out mask, out _homography, out score);

                        if (isSuccess)
                        {
                            if (score > 0 && _homography != null)
                            {
                                isSuccess = true;
                            }
                            else
                            {
                                isSuccess = false;
                            }
                        }

                        homography = _homography;
                    }
                    catch (Exception _ex)
                    {
                        ex = _ex;
                        isSuccess = false;
                    }

                    return isSuccess;
                }

                public override bool GenerateFeatures(IInputArray source, out VectorOfKeyPoint generatedKeyPoints,
                    out Mat generatedDescriptors)
                {
                    var result = false;

                    generatedKeyPoints = new VectorOfKeyPoint();
                    generatedDescriptors = new Mat();
                    try
                    {
                        var modelImage = (Mat)source;

                        using (var uModelImage = modelImage.GetUMat(AccessType.Read))
                        {
                            this.GetKAZE.DetectAndCompute(uModelImage, null, generatedKeyPoints, generatedDescriptors,
                                false);
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
                    public AKazeData(AKAZE.DescriptorType descriptorType, int descriptorSize, int channels,
                        double threshold, int octaves, int layers, KAZE.Diffusivity diffusivity)
                    {
                        this.DescriptorType = descriptorType;
                        this.DescriptorSize = descriptorSize;
                        this.Channels = channels;
                        this.Threshold = threshold;
                        this.Octaves = octaves;
                        this.Layers = layers;
                        this.Diffusivity = diffusivity;
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

                public AKazeData GetAKazeData { get; }

                public AKAZE GetAKAZE { get; }

                #endregion

                #region Methods

                #region Public

                public AKazeRegistrationMethod(AKAZE.DescriptorType descriptorType, int desSize, int channels,
                    double threshold, int octaves, int layers, KAZE.Diffusivity diffusivity) : base(
                    RegistrationMethodType.KAZE)
                {
                    this.GetAKazeData = new AKazeData(descriptorType, desSize, channels, threshold, octaves, layers,
                        diffusivity);
                    this.GetAKAZE = new AKAZE(this.GetAKazeData.DescriptorType, this.GetAKazeData.DescriptorSize,
                        this.GetAKazeData.Channels, (float)this.GetAKazeData.Threshold, this.GetAKazeData.Octaves,
                        this.GetAKazeData.Layers, this.GetAKazeData.Diffusivity);
                }

                public AKazeRegistrationMethod(AKazeData aKazeData) : base(RegistrationMethodType.AKAZE)
                {
                    this.GetAKazeData = aKazeData;
                    this.GetAKAZE = new AKAZE(aKazeData.DescriptorType, aKazeData.DescriptorSize, aKazeData.Channels,
                        (float)aKazeData.Threshold, aKazeData.Octaves, aKazeData.Layers, aKazeData.Diffusivity);
                }

                #endregion

                #region Private

                private bool ExtractHomography(Mat modelImage, Mat observedImage, bool useStoredModelFeatures,
                    out long matchTime, out VectorOfKeyPoint modelKeyPoints, out VectorOfKeyPoint observedKeyPoints,
                    out VectorOfVectorOfDMatch matches, out Mat mask, out Mat homography, out long score)
                {
                    score = 0;
                    mask = new Mat();
                    matchTime = 0;

                    var k = 2;
                    var uniquenessThreshold = 0.75;

                    var watch = new Stopwatch();
                    homography = null;

                    modelKeyPoints = new VectorOfKeyPoint();
                    observedKeyPoints = new VectorOfKeyPoint();
                    matches = new VectorOfVectorOfDMatch();

                    var result = false;

                    try
                    {
                        using (var uModelImage = modelImage.GetUMat(AccessType.Read))
                        using (var uObservedImage = observedImage.GetUMat(AccessType.Read))
                        {
                            using (var observedDescriptors = new Mat())
                            using (var modelDescriptors = useStoredModelFeatures
                                ? this.GetStoredModelDescriptors.Clone()
                                : new Mat())
                            {
                                watch.Start();

                                //akaze = new AKAZE();
                                if (!useStoredModelFeatures)
                                {
                                    this.GetAKAZE.DetectAndCompute(uModelImage, null, modelKeyPoints, modelDescriptors,
                                        false);
                                }
                                else
                                {
                                    modelKeyPoints = this.GetStoredModelKeyPoints;
                                }

                                this.GetAKAZE.DetectAndCompute(uObservedImage, null, observedKeyPoints,
                                    observedDescriptors, false);

                                if (modelKeyPoints.Size > 0 && observedKeyPoints.Size > 0)
                                {
                                    // KdTree for faster results / less accuracy
                                    using (var ip = new KdTreeIndexParams())
                                    using (var sp = new SearchParams())
                                    using (DescriptorMatcher matcher = new FlannBasedMatcher(ip, sp))
                                    {
                                        matcher.Add(observedDescriptors);

                                        matcher.KnnMatch(modelDescriptors, matches, k);
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
                                            var total = 0;
                                            for (var i = 0; i < matches.Size; i++)
                                            {
                                                foreach (var e in matches[i].ToArray())
                                                    ++total;
                                                if (mask.GetRawData(i)[0] == 0)
                                                {
                                                    continue;
                                                }

                                                foreach (var e in matches[i].ToArray())
                                                    ++score;
                                            }

                                            score = (long)(score / (float)total * 100);
                                            // <----------------------------------------------

                                            var nonZeroCount = CvInvoke.CountNonZero(mask);
                                            if (nonZeroCount >= 4)
                                            {
                                                nonZeroCount =
                                                    Features2DToolbox.VoteForSizeAndOrientation(modelKeyPoints,
                                                        observedKeyPoints, matches, mask, 1.6, 20);
                                                if (nonZeroCount >= 4)
                                                {
                                                    homography =
                                                        Features2DToolbox.GetHomographyMatrixFromMatchedFeatures(
                                                            modelKeyPoints, observedKeyPoints, matches, mask, 2);
                                                }
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
                    catch (Exception ex)
                    {
                        result = false;
                    }

                    matchTime = watch.ElapsedMilliseconds;
                    return result;
                }

                public override bool ApplyMethod(IInputArray source, IInputArray observed, bool useStoredModelFeatures,
                    out IOutputArray homography, out VectorOfVectorOfDMatch matches, out long matchTime, out long score,
                    out Exception ex)
                {
                    var isSuccess = false;

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
                        isSuccess = this.ExtractHomography(sourceImg.Mat, observedImg.Mat, useStoredModelFeatures,
                            out matchTime, out modelKeyPoints, out observedKeyPoints, out matches,
                            out mask, out _homography, out score);

                        homography = _homography;
                        if (isSuccess)
                        {
                            if (score > 0 && _homography != null)
                            {
                                isSuccess = true;
                            }
                            else
                            {
                                isSuccess = false;
                            }
                        }
                    }
                    catch (Exception _ex)
                    {
                        ex = _ex;
                        isSuccess = false;
                    }

                    return isSuccess;
                }

                public override bool GenerateFeatures(IInputArray source, out VectorOfKeyPoint generatedKeyPoints,
                    out Mat generatedDescriptors)
                {
                    var result = false;

                    generatedKeyPoints = new VectorOfKeyPoint();
                    generatedDescriptors = new Mat();
                    try
                    {
                        var modelImage = (Mat)source;

                        using (var uModelImage = modelImage.GetUMat(AccessType.Read))
                        {
                            this.GetAKAZE.DetectAndCompute(uModelImage, null, generatedKeyPoints, generatedDescriptors,
                                false);
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

            public RegistrationMethod GetRegistrationMethod { get; }

            public IInputArray GetSourceImage { get; }

            public Size GetOutputWidth { get; }

            public bool GetUseStoredModelFeatures { get; private set; }

            #endregion

            public RegistrationAlignmentMethod(int pipelineIndex, string methodName,
                RegistrationMethod registrationMethod, IInputArray sourceImage, Size outputWidth) : base(methodName,
                AlignmentMethodType.Registration, pipelineIndex)
            {
                this.GetSourceImage = sourceImage;
                this.GetRegistrationMethod = registrationMethod;
                this.GetOutputWidth = outputWidth;
            }

            public override bool ApplyMethod(IInputArray input, out IOutputArray output, out Mat homography,
                out long matchTime, out Exception ex)
            {
                var isSuccess = false;
                IOutputArray _homography;
                homography = null;

                output = null;

                isSuccess = this.GetRegistrationMethod.ApplyMethod(this.GetSourceImage, input,
                    this.GetUseStoredModelFeatures, out _homography, out var matches, out matchTime, out var score,
                    out var _ex);

                ex = _ex;

                if (isSuccess)
                {
                    try
                    {
                        homography = (Mat)_homography;

                        if (score > 0 && homography != null)
                        {
                            var warped = new Mat();
                            CvInvoke.WarpPerspective(input, warped, homography, this.GetOutputWidth, Inter.Cubic,
                                Warp.Default, BorderType.Replicate);

                            output = warped;
                            isSuccess = true;
                        }
                        else
                        {
                            output = null;
                            isSuccess = false;
                        }
                    }
                    catch (Exception __ex)
                    {
                        ex = __ex;
                        isSuccess = false;
                    }
                }

                return isSuccess;
            }

            public override bool ApplyMethod(IInputArray templateImage, IInputArray input, out IOutputArray output,
                out Mat homography, out long matchTime, out Exception ex)
            {
                var isSuccess = false;
                IOutputArray _homography;
                homography = null;

                output = null;
                isSuccess = false;

                if (this.GetRegistrationMethod.ApplyMethod(templateImage, input, this.GetUseStoredModelFeatures,
                    out _homography, out var matches, out matchTime, out var score, out ex))
                {
                    try
                    {
                        homography = (Mat)_homography;

                        if (score > 0 && homography != null)
                        {
                            var warped = new Mat();
                            CvInvoke.WarpPerspective(input, warped, homography, this.GetOutputWidth, Inter.Cubic,
                                Warp.Default, BorderType.Replicate);

                            output = warped;
                            isSuccess = true;
                        }
                        else
                        {
                            output = null;
                            isSuccess = false;
                        }
                    }
                    catch (Exception _ex)
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
                this.GetRegistrationMethod.StoreModelFeatures(model);
                if (use)
                {
                    this.GetUseStoredModelFeatures = true;
                }
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

                public AlignmentMethodType GetAlignmentMethodType => AlignmentMethod.GetAlignmentMethodType;
                public AlignmentMethod AlignmentMethod;
                public AlignmentMethodResultType GetAlignmentMethodResultType { get; }

                public Mat InputImage { get; set; }
                public Mat OutputImage { get; set; }
                public long AlignmentTime { get; set; }
                public Mat AlignmentHomography { get; set; }

                #endregion

                public AlignmentMethodResult(AlignmentMethod alignmentMethod,
                    AlignmentMethodResultType alignmentMethodResultType, Mat alignmentHomography, Mat inputImage,
                    Mat outputImage, long alignmentTime)
                {
                    AlignmentMethod = alignmentMethod;
                    this.InputImage = inputImage;
                    this.OutputImage = outputImage;
                    this.AlignmentTime = alignmentTime;

                    this.GetAlignmentMethodResultType = alignmentMethodResultType;
                    this.AlignmentHomography = alignmentHomography;
                }
            }

            public class AnchorAlignmentMethodResult : AlignmentMethodResult
            {
                #region Properties

                public AnchorAlignmentMethod.Anchor[] MainAnchors = new AnchorAlignmentMethod.Anchor[0];
                public RectangleF[] DetectedAnchors = new RectangleF[0];
                public RectangleF[] WarpedAnchors = new RectangleF[0];
                public RectangleF[] ScaledMainAnchors = new RectangleF[0];
                public RectangleF ScaledMainTestRegion;

                #endregion

                public AnchorAlignmentMethodResult(AlignmentMethod alignmentMethod,
                    AlignmentMethodResultType alignmentMethodResultType, Mat alignmentHomography, Mat inputImage,
                    Mat outputImage, long alignmentTime, AnchorAlignmentMethod.Anchor[] mainAnchors,
                    RectangleF[] detectedAnchors, RectangleF[] warpedAnchors, RectangleF[] scaledMainAnchors,
                    RectangleF scaledMainTestRegion) : base(alignmentMethod, alignmentMethodResultType,
                    alignmentHomography, inputImage, outputImage, alignmentTime)
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
                public RegistrationAlignmentMethodResult(AlignmentMethod alignmentMethod,
                    AlignmentMethodResultType alignmentMethodResultType, Mat alignmentHomography, Mat inputImage,
                    Mat outputImage, long alignmentTime) : base(alignmentMethod, alignmentMethodResultType,
                    alignmentHomography, inputImage, outputImage, alignmentTime)
                {
                }
            }

            #endregion

            #region Properties

            public List<AlignmentMethodResult> AlignmentMethodTestResultsList { get; set; }

            #endregion

            public AlignmentPipelineResults(List<AlignmentMethodResult> alignmentMethodTestResultsList)
            {
                this.AlignmentMethodTestResultsList = alignmentMethodTestResultsList;
            }
        }

        #endregion

        [Serializable]
        public class Data
        {
            public string TemplateName;
            public string TemplateLocation;
            public string TemplateDataDirectory;

            public TemplateImage GetTemplateImage { get; private set; }

            public List<AlignmentMethod> GetAlignmentPipeline { get; private set; } = new List<AlignmentMethod>();

            private Dictionary<string, object> Properties { get; set; }

            public bool IsActivatedd { get; set; }

            public Data(string templateName, string templateLocation, TemplateImage templateImage,
                List<AlignmentMethod> alignmentPipeline, Dictionary<string, object> properties)
            {
                TemplateName = templateName;
                TemplateLocation = templateLocation;
                TemplateDataDirectory = LSTM.GetTemplateDataPath(TemplateLocation);

                this.GetTemplateImage = templateImage;
                this.GetAlignmentPipeline = alignmentPipeline;
                this.Properties = properties;
            }

            public void Initialize()
            {
                this.GetTemplateImage.Initialize();
            }

            public void SetTemplateImage(TemplateImage templateImage)
            {
                this.GetTemplateImage = templateImage;
            }

            public void SetAlignmentPipeline(List<AlignmentMethod> alignmentPipeline)
            {
                this.GetAlignmentPipeline = alignmentPipeline;
            }

            public bool AddProperty(string key, object value)
            {
                if (this.Properties == null)
                {
                    this.Properties = new Dictionary<string, object>();
                }

                if (this.Properties != null)
                {
                    if (this.Properties.ContainsKey(key))
                    {
                        try
                        {
                            this.Properties[key] = value;
                            return true;
                        }
                        catch (Exception ex)
                        {
                            return false;
                        }
                    }

                    try
                    {
                        this.Properties.Add(key, value);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        return false;
                    }
                }

                return true;
            }

            public bool RemoveProperty(string key)
            {
                if (this.Properties != null)
                {
                    try
                    {
                        this.Properties.Remove(key);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        return false;
                    }
                }

                return true;
            }

            public object GetProperty(string key)
            {
                object value = null;
                if (this.Properties != null)
                {
                    try
                    {
                        value = this.Properties[key];
                    }
                    catch (Exception ex)
                    {
                    }
                }

                return value;
            }
        }

        #endregion

        #region Properties

        public string GetTemplateName
        {
            get => TemplateData.TemplateName;
            set { }
        }

        public TemplateImage GetTemplateImage
        {
            get => TemplateData.GetTemplateImage;
            set { }
        }

        public string GetTemplateLocation
        {
            get => TemplateData.TemplateLocation;
            set { }
        }

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
            var ImageLocation = TemplateData.GetTemplateImage.ImageLocation;
            TemplateData.GetTemplateImage.ImageLocation = string.IsNullOrEmpty(ImageLocation) ? null :
                File.Exists(ImageLocation) ? ImageLocation : LSTM.GetTemplateImagePath(tmpData.TemplateName);
            TemplateData.Initialize();
        }

        public static Template CreateTemplate(string tmpName)
        {
            var newTemplate = new Template(new Data(tmpName, "", TemplateImage.Empty(), new List<AlignmentMethod>(),
                new Dictionary<string, object>()));
            SaveTemplate(newTemplate.TemplateData);
            return newTemplate;
        }

        public static async Task<bool> ChangeTemplateName(string oldName, string newName)
        {
            var result = false;
            var template = await LoadTemplate(oldName);
            template.TemplateData.TemplateName = newName;

            var newNameLocation = template.TemplateData.TemplateLocation.Replace(oldName, newName);
            Directory.Move(template.TemplateData.TemplateLocation, newNameLocation);
            template.TemplateData.TemplateLocation = newNameLocation;
            template.TemplateData.TemplateDataDirectory = LSTM.GetTemplateDataPath(newNameLocation);

            result = SaveTemplate(template.TemplateData);
            return result;
        }

        public static bool SaveTemplate(Data tempData, bool saveImage = false)
        {
            var isSaved = true;

            try
            {
                if (!saveImage)
                {
                    OnSaveTemplateEvent?.Invoke(tempData);
                }
                else
                {
                    OnSaveConfiguredTemplateEvent?.Invoke(tempData, tempData.GetTemplateImage.GetBitmap);
                }
            }
            catch (Exception ex)
            {
                isSaved = false;
            }

            return isSaved;
        }

        public static bool SaveTemplate(Data tempData, Image<Gray, byte> configuredTemplateImage)
        {
            var isSaved = true;

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
            return await Task.Run(() => LSTM.LoadTemplate(templateName));
        }

        public static async Task<Template> ImportTemplate(string location)
        {
            Template template = null;

            template = await Task.Run(() => LSTM.ImportTemplate(location));

            return template;
        }

        public static async Task<bool> DeleteTemplate(string templateName)
        {
            return await Task.Run(() => LSTM.DeleteTemplate(templateName));
        }

        public void SetTemplateImage(TemplateImage image)
        {
            TemplateData.SetTemplateImage(image);
        }

        public void SetAlignmentPipeline(List<AlignmentMethod> alignmentPipeline)
        {
            TemplateData.SetAlignmentPipeline(alignmentPipeline);
        }

        public bool GetAlignedImage(string rowSheetPath, ProcessingEnums.RereadType rereadType, out Mat result)
        {
            result = new Mat();
            var unAlignedMat = new Mat(rowSheetPath, ImreadModes.Grayscale);
            switch (rereadType)
            {
                case ProcessingEnums.RereadType.NORMAL:
                    break;

                case ProcessingEnums.RereadType.ROTATE_C_90:
                    CvInvoke.Rotate(unAlignedMat, unAlignedMat, RotateFlags.Rotate90Clockwise);
                    break;

                case ProcessingEnums.RereadType.ROTATE_180:
                    CvInvoke.Rotate(unAlignedMat, unAlignedMat, RotateFlags.Rotate180);
                    break;

                case ProcessingEnums.RereadType.ROTATE_AC_90:
                    CvInvoke.Rotate(unAlignedMat, unAlignedMat, RotateFlags.Rotate90CounterClockwise);
                    break;
            }

            try
            {
                var resultImg = this.AlignSheet(unAlignedMat, out var alignmentPipelineResults);
                result = resultImg;
                unAlignedMat.Dispose();
                //CvInvoke.WarpPerspective(unAligned, result, GetAlignmentHomography, unAligned.Size, Emgu.CV.CvEnum.Inter.Cubic, Emgu.CV.CvEnum.Warp.Default, Emgu.CV.CvEnum.BorderType.Default);
                return alignmentPipelineResults.AlignmentMethodTestResultsList[0].GetAlignmentMethodResultType ==
                       AlignmentPipelineResults.AlignmentMethodResultType.Successful;
            }
            catch (Exception ex)
            {
                result = unAlignedMat;
                return false;
            }
        }

        public Mat AlignSheet(Mat sheetImage, out AlignmentPipelineResults alignmentPipelineResults, bool log = true)
        {
            var outputImage = sheetImage;
            var alignmentPipeline = TemplateData.GetAlignmentPipeline;
            var grayImage = this.GetTemplateImage.GetGrayImage.Mat;

            alignmentPipelineResults = null;

            if (alignmentPipeline.Count <= 0)
            {
                return outputImage;
            }

            var alignmentMethodResults = new List<AlignmentPipelineResults.AlignmentMethodResult>();

            IOutputArray outputImageArr;
            CvInvoke.Resize(sheetImage, outputImage, grayImage.Size);
            //outputImage = sheetImage.Resize(grayImage.Width, grayImage.Height, Inter.Cubic);
            for (var i = 0; i < alignmentPipeline.Count; i++)
            {
                Exception exception = null;

                AlignmentPipelineResults.AlignmentMethodResult alignmentMethodResult = null;
                var alignmentMethod = alignmentPipeline[i];

                if (alignmentMethod.PipelineIndex ==
                    -1) //|| ommittedAlignmetMethodIndeces.Contains(alignmentMethod.PipelineIndex))
                {
                    continue;
                }

                if (alignmentMethod.GetAlignmentMethodType == AlignmentMethodType.Anchors)
                {
                    var aIM = (AnchorAlignmentMethod)alignmentMethod;
                    var isSuccess = aIM.ApplyMethod(outputImage, out outputImageArr, out var detectedAnchors,
                        out var warpedAnchors, out var scaledMainAnchorRegions, out var scaledMainTestRegion,
                        out var alignmentHomography, out var alignmentTime, out exception);
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
                            : AlignmentPipelineResults.AlignmentMethodResultType.Failed, null, null, null,
                        alignmentTime, mainAnchors, detectedAnchors, warpedAnchors, scaledMainAnchorRegions,
                        scaledMainTestRegion);
                    alignmentMethodResult = anchorAlignmentMethodResult;
                }
                else
                {
                    var isSuccess = alignmentMethod.ApplyMethod(grayImage, outputImage, out outputImageArr,
                        out var alignmentHomography, out var alignmentTime, out exception);
                    if (isSuccess)
                    {
                        var outputMat = (Mat)outputImageArr;
                        outputImage = outputMat;
                    }

                    alignmentMethodResult = new AlignmentPipelineResults.AlignmentMethodResult(alignmentMethod,
                        isSuccess
                            ? AlignmentPipelineResults.AlignmentMethodResultType.Successful
                            : AlignmentPipelineResults.AlignmentMethodResultType.Failed, null, null, null,
                        alignmentTime);
                }

                alignmentMethodResults.Add(alignmentMethodResult);

                if (alignmentMethodResult.GetAlignmentMethodResultType ==
                    AlignmentPipelineResults.AlignmentMethodResultType.Failed)
                {
                    var personnelData = exception.Message;

                    if (exception.StackTrace == null)
                    {
                        if (log)
                        {
                            Messages.ShowError("An error occured while applying the method: '" +
                                               alignmentMethod.MethodName + "' \n\n For concerned personnel: " +
                                               personnelData);
                        }

                        return outputImage;
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
            //OnResultsGeneratedEvent?.Invoke(alignmentPipelineResults);

            return outputImage;
        }

        #endregion
    }
}