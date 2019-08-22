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
                #endregion

                public RegistrationMethod(RegistrationMethodType getRegistrationMethodType)
                {
                    GetRegistrationMethodType = getRegistrationMethodType;
                }

                #region Methods
                public abstract bool ApplyMethod(IInputArray source, IInputArray observed, out IOutputArray homography, out VectorOfVectorOfDMatch matches, out long matchTime, out long score); 
                #endregion
            }
            [Serializable]
            public class KazeRegistrationMethod : RegistrationMethod
            {
                #region Properties
                public bool Extended { get; set; }
                public bool Upright { get; set; }
                public double Threshold { get; set; }
                public int Octaves { get; set; }
                public int Sublevels { get; set; }
                public KAZE.Diffusivity Diffusivity { get; set; }

                public KAZE GetKAZE { get => kaze; }
                private KAZE kaze;
                #endregion


                #region Methods
                #region Public
                public KazeRegistrationMethod(bool extended, bool upright, double threshold, int octaves, int sublevels, KAZE.Diffusivity diffusivity) : base(RegistrationMethodType.KAZE)
                {
                    Extended = extended;
                    Upright = upright;
                    Threshold = threshold;
                    Octaves = octaves;
                    Sublevels = sublevels;
                    Diffusivity = diffusivity;

                    kaze = new KAZE(Extended, Upright, (float)Threshold, Octaves, Sublevels, Diffusivity);
                }
                #endregion

                #region Private
                private bool ExtractHomography (Mat modelImage, Mat observedImage, out long matchTime, out VectorOfKeyPoint modelKeyPoints, out VectorOfKeyPoint observedKeyPoints, out VectorOfVectorOfDMatch matches, out Mat mask, out Mat homography, out long score)
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

                    using (UMat uModelImage = modelImage.GetUMat(AccessType.Read))
                    using (UMat uObservedImage = observedImage.GetUMat(AccessType.Read))
                    {
                        using (Mat observedDescriptors = new Mat())
                        using (Mat modelDescriptors = new Mat())
                        {
                            watch.Start();
                            kaze.DetectAndCompute(uModelImage, null, modelKeyPoints, modelDescriptors, false);
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
                                        score = 0;
                                        mask = new Mat();
                                        matchTime = 0;
                                        return false;
                                    }
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
                                watch.Stop();

                            }
                            else
                            {
                                score = 0;
                                mask = new Mat();
                                matchTime = 0;
                                return false;
                            }
                        }
                    }
                    matchTime = watch.ElapsedMilliseconds;
                    return true;
                }
                public override bool ApplyMethod(IInputArray source, IInputArray observed, out IOutputArray homography, out VectorOfVectorOfDMatch matches, out long matchTime, out long score)
                {
                    bool isSuccess = false;

                    Mat _homography;
                    VectorOfKeyPoint modelKeyPoints;
                    VectorOfKeyPoint observedKeyPoints;

                    Mat mask;
                    isSuccess = ExtractHomography((Mat)source, (Mat)observed, out matchTime, out modelKeyPoints, out observedKeyPoints, out matches,
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
                #endregion
                #endregion
            }
            #endregion

            #region Properties
            public RegistrationMethod GetRegistrationMethod { get => registrationMethod; }
            RegistrationMethod registrationMethod;
            #endregion

            public RegistrationAlignmentMethod(int pipelineIndex, string methodName, RegistrationMethod registrationMethod) : base(methodName, AlignmentMethodType.Registration, pipelineIndex)
            {
                this.registrationMethod = registrationMethod;
            }

            public override bool ApplyMethod(IInputArray input, out IOutputArray output)
            {
                bool result = registrationMethod.ApplyMethod(input, out output);
                return result;
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
