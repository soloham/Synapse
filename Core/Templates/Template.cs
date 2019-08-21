using Emgu.CV;
using Emgu.CV.Structure;
using Synapse.Utilities.Attributes;
using System;
using System.Collections.Generic;
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

            protected AlignmentMethod(string methodName, AlignmentMethodType alignmentMethodType, int pipelineIndex)
            {
                MethodName = methodName;
                this.alignmentMethodType = alignmentMethodType;
                PipelineIndex = pipelineIndex;
            }

            protected abstract bool ApplyMethod(IInputArray input, out IOutputArray output);
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

            protected override bool ApplyMethod(IInputArray input, out IOutputArray output)
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
            public RegistrationAlignmentMethod(int pipelineIndex, string methodName) : base(methodName, AlignmentMethodType.Registration, pipelineIndex)
            {

            }

            protected override bool ApplyMethod(IInputArray input, out IOutputArray output)
            {
                throw new NotImplementedException();
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
