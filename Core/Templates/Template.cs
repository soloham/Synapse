using Emgu.CV;
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
            public Bitmap GetBitmap { get => new Bitmap(ImageLocation); set { } }
            public Size Size { get => templateSize; set => templateSize = value; }
            private Size templateSize;
            public double TemplateScale { get => templateScale; set => templateScale = value; }
            private double templateScale;
            public double DeskewPercent { get => deskewPercent; set => deskewPercent = value; }
            private double deskewPercent;
            public string ImageLocation { get => templateImageLocation; set => templateImageLocation = value; }
            private string templateImageLocation;

            public TemplateImage(Size templateSize, double templateScale, double deskewPercent)
            {
                this.templateSize = templateSize;
                this.templateScale = templateScale;
                this.deskewPercent = deskewPercent;
            }

            public static TemplateImage Empty()
            {
                return new TemplateImage(Size.Empty, 1, 0);
            }
        }

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

            public abstract bool ApplyMethod(IInputArray input, out IOutputArray output);
        }
        [Serializable]
        internal class AnchorAlignmentMethod : AlignmentMethod
        {
            #region Objects
            public class Anchor
            {
                public RectangleF GetAnchorRegion { get => anchorRegion; }
                private RectangleF anchorRegion = new RectangleF();
                public IInputArray GetAnchorImage { get => anchorImage; }
                private IInputArray anchorImage;

                public PointF anchorCoordinates;

                public Anchor(RectangleF anchorRegion, IInputArray anchorImage)
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
            #endregion

            #region Variables
            private Size outputSize;
            private PointF[] mainAnchorCoordinates;
            #endregion

            #region Methods
            public AnchorAlignmentMethod(List<Anchor> anchors, Size outputSize, int pipelineIndex, string methodName) : base(methodName, AlignmentMethodType.Anchors, pipelineIndex)
            {
                this.anchors = anchors;
                this.outputSize = outputSize;

                ExtractCooridnates();
            }

            #region Public
            public void AddAnchor(RectangleF region, IInputArray image)
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

            public override bool ApplyMethod(IInputArray input, out IOutputArray output)
            {
                bool isSuccess = false;
                Image<Gray, byte> _output = null;

                PointF[] anchorCoordinates = new PointF[anchors.Count];
                for (int i = 0; i < anchors.Count; i++)
                {
                    Anchor curAnchor = anchors[i];

                    Mat result = null;
                    CvInvoke.MatchTemplate(input, curAnchor.GetAnchorImage, result, Emgu.CV.CvEnum.TemplateMatchingType.CcoeffNormed);

                    Point[] Max_Loc, Min_Loc;
                    double[] min, max;

                    result.MinMax(out min, out max, out Min_Loc, out Max_Loc);

                    if(max[0] > 0.85)
                        isSuccess = true;

                    anchorCoordinates[i] = Max_Loc[0];
                }

                var homography = CvInvoke.FindHomography(anchorCoordinates, mainAnchorCoordinates, Emgu.CV.CvEnum.RobustEstimationAlgorithm.Ransac);
                CvInvoke.WarpPerspective(input, _output, homography, outputSize);

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
                public abstract bool ApplyMethod(IInputArray source, IInputArray observed, bool useStoredModelFeatures, out IOutputArray homography, out VectorOfVectorOfDMatch matches, out long matchTime, out long score);
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
                            using (Mat modelDescriptors = useStoredModelFeatures ? GetStoredModelDescriptors : new Mat())
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
                                                if ((int)mask.GetData().GetValue(i) == 0) continue;
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
                public override bool ApplyMethod(IInputArray source, IInputArray observed, bool useStoredModelFeatures, out IOutputArray homography, out VectorOfVectorOfDMatch matches, out long matchTime, out long score)
                {
                    bool isSuccess = false;

                    Mat _homography;
                    VectorOfKeyPoint modelKeyPoints;
                    VectorOfKeyPoint observedKeyPoints;

                    Mat mask;
                    isSuccess = ExtractHomography((Mat)source, (Mat)observed, useStoredModelFeatures, out matchTime, out modelKeyPoints, out observedKeyPoints, out matches,
                        out mask, out _homography, out score);

                    homography = _homography;
                    if (isSuccess)
                    {
                        if (score > 0 && _homography != null)
                            isSuccess = true;
                        else
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
                                                if ((int)mask.GetData().GetValue(i) == 0) continue;
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
                public override bool ApplyMethod(IInputArray source, IInputArray observed, bool useStoredModelFeatures, out IOutputArray homography, out VectorOfVectorOfDMatch matches, out long matchTime, out long score)
                {
                    bool isSuccess = false;

                    Mat _homography;
                    VectorOfKeyPoint modelKeyPoints;
                    VectorOfKeyPoint observedKeyPoints;

                    Mat mask;
                    isSuccess = ExtractHomography((Mat)source, (Mat)observed, useStoredModelFeatures, out matchTime, out modelKeyPoints, out observedKeyPoints, out matches,
                        out mask, out _homography, out score);

                    homography = _homography;
                    if (isSuccess)
                    {
                        if (score > 0 && _homography != null)
                            isSuccess = true;
                        else
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

            public override bool ApplyMethod(IInputArray input, out IOutputArray output)
            {
                bool result = false;
                IOutputArray _homography;
                registrationMethod.ApplyMethod(sourceImage, input, useStoredModelFeatures, out _homography, out VectorOfVectorOfDMatch matches, out long matchTime, out long score);
                Mat homography = (Mat)_homography;

                if (score > 0 && homography != null)
                {
                    Mat warped = new Mat();
                    CvInvoke.WarpPerspective(input, warped, homography, outputWidth, Inter.Cubic, Warp.Default, BorderType.Replicate);

                    output = warped;
                    result = true;
                }
                else
                {
                    output = null;
                    result = false;
                }

                return result;
            }
            public void StoreModelFeatures(IInputArray model, bool use)
            {
                registrationMethod.StoreModelFeatures(model);
                if(use) useStoredModelFeatures = true;
            }
        }

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
        #endregion

        #region Variables
        public Data TemplateData;
        #endregion

        #region Methods
        public Template(Data tmpData)
        {
            TemplateData = tmpData;
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
        internal static bool SaveTemplate(Data tempData)
        {
            bool isSaved = true;

            try
            {
                OnSaveTemplateEvent?.Invoke(tempData);
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
        #endregion
    }
}
