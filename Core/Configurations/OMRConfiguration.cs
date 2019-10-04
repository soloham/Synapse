using Emgu.CV;
using Synapse.Core.Engines;
using Synapse.Core.Engines.Data;
using Synapse.Core.Keys;
using Synapse.Utilities;
using Synapse.Utilities.Attributes;
using Synapse.Utilities.Memory;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Synapse.Core.Configurations
{
    #region Enums
    public enum OMRType
    {
        [EnumDescription("Gradable")]
        Gradable,
        [EnumDescription("Non Gradable")]
        Parameter
    }
    public enum MultiMarkAction
    {
        [EnumDescription("Mark As Manual")]
        MarkAsManual,
        [EnumDescription("Consider First")]
        ConsiderFirst,
        [EnumDescription("Consider Last")]
        ConsiderLast,
        [EnumDescription("Consider Correct")]
        ConsiderCorrect,
        [EnumDescription("Invalidate")]
        Invalidate
    }
    public enum NoneMarkedAction
    {
        [EnumDescription("Mark As Manual")]
        MarkAsManual,
        [EnumDescription("Invalidate")]
        Invalidate
    }
    #endregion

    #region Objects
    [Serializable]
    public class OMRRegionData
    {
        #region Enums
        public enum InterSpaceType
        {
            [EnumDescription("Constant")]
            CONSTANT,
            [EnumDescription("Array")]
            ARRAY
        }
        #endregion

        public OMRRegionData(Orientation orientation, int totalFields, RectangleF fieldsRegion, InterSpaceType interFieldsSpaceType, double interFieldsSpace, double[] interFieldsSpaces, int totalOptions, RectangleF optionsRegion, InterSpaceType interOptionsSpaceType, double interOptionsSpace, double[] interOptionsSpaces)
        {
            Orientation = orientation;

            TotalFields = totalFields;
            FieldsRegion = fieldsRegion;
            InterFieldsSpaceType = interFieldsSpaceType;
            InterFieldsSpace = interFieldsSpace;
            InterFieldsSpaces = interFieldsSpaces;

            TotalOptions = totalOptions;
            OptionsRegion = optionsRegion;
            InterOptionsSpaceType = interOptionsSpaceType;
            InterOptionsSpace = interOptionsSpace;
            InterOptionsSpaces = interOptionsSpaces;
        }

        #region Properties
        public Orientation Orientation { get; set; }

        #region Fields Properties
        public int TotalFields { get; set; }
        public RectangleF FieldsRegion { get; set; }
        public InterSpaceType InterFieldsSpaceType { get; set; }
        public double InterFieldsSpace { get; set; }
        public double[] InterFieldsSpaces { get; set; }
        public List<RectangleF> GetFieldsRects { get => fieldsRects == null || fieldsRects.Count == 0? CalculateFieldsRects() : fieldsRects; }
        private List<RectangleF> fieldsRects;
        public List<RectangleF> GetInterFieldsSpacesRects { get => interFieldsSpacesRects == null || interFieldsSpacesRects.Count == 0? CalculateInterFieldsSpacesRects() : interFieldsSpacesRects; }
        private List<RectangleF> interFieldsSpacesRects;
        #endregion

        #region Options Properties
        public int TotalOptions { get; set; }
        public RectangleF OptionsRegion { get; set; }
        public InterSpaceType InterOptionsSpaceType { get; set; }
        public double InterOptionsSpace { get; set; }
        public double[] InterOptionsSpaces { get; set; }
        public List<RectangleF> GetOptionsRects { get => optionsRects == null || optionsRects.Count == 0? CalculateOptionsRects() : optionsRects; }
        private List<RectangleF> optionsRects;
        public List<RectangleF> GetInterOptionsSpacesRects { get => interOptionsSpacesRects == null || interOptionsSpacesRects.Count == 0? CalculateInterOptionsSpacesRects() : interOptionsSpacesRects; }
        private List<RectangleF> interOptionsSpacesRects;
        #endregion
        #endregion

        #region Methods
        public List<RectangleF> CalculateFieldsRects()
        {
            fieldsRects = new List<RectangleF>();

            for (int i = 0; i < TotalFields; i++)
            {
                RectangleF curFieldRectF = new RectangleF();

                switch (Orientation)
                {
                    case Orientation.Horizontal:
                        switch (InterFieldsSpaceType)
                        {
                            case InterSpaceType.CONSTANT:
                                curFieldRectF = new RectangleF(new PointF(FieldsRegion.X, FieldsRegion.Y + (float)(i * (FieldsRegion.Height + InterFieldsSpace))), FieldsRegion.Size);
                                break;
                            case InterSpaceType.ARRAY:
                                curFieldRectF = new RectangleF(new PointF(FieldsRegion.X, i == 0 ? FieldsRegion.Y + (float)InterFieldsSpaces[0] : fieldsRects[i - 1].Bottom + (float)InterFieldsSpaces[i]), FieldsRegion.Size);
                                break;
                            default:
                                break;
                        }
                        break;
                    case Orientation.Vertical:
                        switch (InterFieldsSpaceType)
                        {
                            case InterSpaceType.CONSTANT:
                                curFieldRectF = new RectangleF(new PointF(FieldsRegion.X + (float)(i * (FieldsRegion.Width + InterFieldsSpace)), FieldsRegion.Y), FieldsRegion.Size);
                                break;
                            case InterSpaceType.ARRAY:
                                curFieldRectF = new RectangleF(new PointF(i == 0 ? FieldsRegion.X + (float)InterFieldsSpaces[0] : fieldsRects[i - 1].Right + (float)InterFieldsSpaces[i], FieldsRegion.Y), FieldsRegion.Size);
                                break;
                            default:
                                break;
                        }
                        break;
                    default:
                        break;
                }

                fieldsRects.Add(curFieldRectF);
            }

            return new List<RectangleF>(fieldsRects);
        }
        public List<RectangleF> CalculateInterFieldsSpacesRects()
        {
            interFieldsSpacesRects = new List<RectangleF>();

            RectangleF interFieldsSpaceRegionVertical = new RectangleF(new PointF(FieldsRegion.X + (FieldsRegion.Size.Width / 2f) - 2.7f, FieldsRegion.Y + FieldsRegion.Size.Height), new SizeF(20f, (float)InterFieldsSpace));
            RectangleF interFieldsSpaceRegionHorizontal = new RectangleF(new PointF(FieldsRegion.X + FieldsRegion.Size.Width, FieldsRegion.Y + (FieldsRegion.Size.Height / 2f) - 2.7f), new SizeF((float)InterFieldsSpace, 20f));

            float interSpaceVerticalWidth = 5f;
            float interSpaceHorizontalHeight = 5f;

            for (int i = 0; i < TotalFields; i++)
            {
                RectangleF curInterFieldSpaceRectF = new RectangleF();

                switch (Orientation)
                {
                    case Orientation.Horizontal:
                        switch (InterFieldsSpaceType)
                        {
                            case InterSpaceType.CONSTANT:
                                if (i == TotalFields - 1)
                                    break;

                                curInterFieldSpaceRectF = new RectangleF(new PointF(interFieldsSpaceRegionVertical.X, interFieldsSpaceRegionVertical.Y + (i * ((float)InterFieldsSpace + FieldsRegion.Height))), new SizeF(interSpaceVerticalWidth, (float)InterFieldsSpace));
                                break;
                            case InterSpaceType.ARRAY:
                                curInterFieldSpaceRectF = new RectangleF(new PointF(interFieldsSpaceRegionVertical.X, i == 0 ? FieldsRegion.Y : fieldsRects[i - 1].Bottom), new SizeF(interSpaceVerticalWidth, (float)InterFieldsSpaces[i]));
                                break;
                            default:
                                break;
                        }
                        break;
                    case Orientation.Vertical:
                        switch (InterFieldsSpaceType)
                        {
                            case InterSpaceType.CONSTANT:
                                if (i == TotalFields - 1)
                                    break;

                                curInterFieldSpaceRectF = new RectangleF(new PointF(interFieldsSpaceRegionHorizontal.X + (i * ((float)InterFieldsSpace + FieldsRegion.Width)), interFieldsSpaceRegionHorizontal.Y), new SizeF((float)InterFieldsSpace, interSpaceHorizontalHeight));
                                break;
                            case InterSpaceType.ARRAY:
                                curInterFieldSpaceRectF = new RectangleF(new PointF(i == 0 ? FieldsRegion.X : fieldsRects[i - 1].Right, interFieldsSpaceRegionHorizontal.Y), new SizeF((float)InterFieldsSpaces[i], interSpaceHorizontalHeight));
                                break;
                            default:
                                break;
                        }
                        break;
                }

                interFieldsSpacesRects.Add(curInterFieldSpaceRectF);
            }

            return new List<RectangleF>(interFieldsSpacesRects);
        }

        public List<RectangleF> CalculateOptionsRects()
        {
            optionsRects = new List<RectangleF>();

            RectangleF lastFieldOptionRect = new RectangleF();
            for (int i0 = 0; i0 < TotalFields; i0++)
            {
                for (int i = 0; i < TotalOptions; i++)
                {
                    RectangleF curOptionRectF = new RectangleF();

                    switch (Orientation)
                    {
                        case Orientation.Horizontal:
                            switch (InterOptionsSpaceType)
                            {
                                case InterSpaceType.CONSTANT:
                                    switch (InterFieldsSpaceType)
                                    {
                                        case InterSpaceType.CONSTANT:
                                            curOptionRectF = new RectangleF(new PointF(OptionsRegion.X + (float)(i * (OptionsRegion.Width + InterOptionsSpace)), OptionsRegion.Y + (float)(i0 * (FieldsRegion.Height + InterFieldsSpace))), OptionsRegion.Size);
                                            break;
                                        case InterSpaceType.ARRAY:
                                            curOptionRectF = new RectangleF(new PointF(OptionsRegion.X + (float)(i * (OptionsRegion.Width + InterOptionsSpace)), i0 == 0 ? (float)InterFieldsSpaces[0] + OptionsRegion.Y : (lastFieldOptionRect.Y + lastFieldOptionRect.Height) + (float)(FieldsRegion.Height + InterFieldsSpaces[i0])), OptionsRegion.Size);
                                            break;
                                        default:
                                            break;
                                    }
                                    break;
                                case InterSpaceType.ARRAY:
                                    switch (InterFieldsSpaceType)
                                    {
                                        case InterSpaceType.CONSTANT:
                                            curOptionRectF = new RectangleF(new PointF(i == 0 ? OptionsRegion.X + (float)InterOptionsSpaces[0] : optionsRects[i - 1].X + optionsRects[i - 1].Width + (float)InterOptionsSpaces[i], OptionsRegion.Y + (float)(i0 * (FieldsRegion.Height + InterFieldsSpace))), OptionsRegion.Size);
                                            break;
                                        case InterSpaceType.ARRAY:
                                            curOptionRectF = new RectangleF(new PointF(i == 0 ? OptionsRegion.X + (float)InterOptionsSpaces[0] : optionsRects[i - 1].X + optionsRects[i - 1].Width + (float)InterOptionsSpaces[i], i0 == 0 ? (float)InterFieldsSpaces[0] + OptionsRegion.Y : lastFieldOptionRect.Y + (float)(FieldsRegion.Height + InterFieldsSpaces[i0])), OptionsRegion.Size);
                                            break;
                                        default:
                                            break;
                                    }
                                    break;
                            }
                            break;
                        case Orientation.Vertical:
                            switch (InterOptionsSpaceType)
                            {
                                case InterSpaceType.CONSTANT:
                                    switch (InterFieldsSpaceType)
                                    {
                                        case InterSpaceType.CONSTANT:
                                            curOptionRectF = new RectangleF(new PointF(OptionsRegion.X + (float)(i0 * (FieldsRegion.Width + InterFieldsSpace)), OptionsRegion.Y + (float)(i * (OptionsRegion.Height + InterOptionsSpace))), OptionsRegion.Size);
                                            break;
                                        case InterSpaceType.ARRAY:
                                            curOptionRectF = new RectangleF(new PointF(i0 == 0 ? (float)InterFieldsSpaces[0] + OptionsRegion.X : lastFieldOptionRect.X + FieldsRegion.Width + (float)InterFieldsSpaces[i0], OptionsRegion.Y + (float)(i * (OptionsRegion.Height + InterOptionsSpace))), OptionsRegion.Size);
                                            break;
                                    }
                                    break;
                                case InterSpaceType.ARRAY:
                                    switch (InterFieldsSpaceType)
                                    {
                                        case InterSpaceType.CONSTANT:
                                            curOptionRectF = new RectangleF(new PointF(OptionsRegion.X + (float)(i0 * (FieldsRegion.Width + InterFieldsSpace)), i == 0 ? (float)InterOptionsSpaces[0] + OptionsRegion.Y : optionsRects[i - 1].Bottom + (float)InterOptionsSpaces[i]), OptionsRegion.Size);
                                            break;
                                        case InterSpaceType.ARRAY:
                                            curOptionRectF = new RectangleF(new PointF(i0 == 0 ? (float)InterFieldsSpaces[0] + OptionsRegion.X : lastFieldOptionRect.X + FieldsRegion.Width + (float)InterFieldsSpaces[i0], i == 0 ? (float)InterOptionsSpaces[0] + OptionsRegion.Y : optionsRects[i - 1].Bottom + (float)InterOptionsSpaces[i]), OptionsRegion.Size);
                                            break;
                                    }
                                    break;
                                default:
                                    break;
                            }
                            break;
                    }

                    optionsRects.Add(curOptionRectF);
                }

                lastFieldOptionRect = optionsRects.Count == 0 ? new RectangleF() : optionsRects[optionsRects.Count - 1];
            }

            return new List<RectangleF>(optionsRects);
        }
        public List<RectangleF> CalculateInterOptionsSpacesRects()
        {
            interOptionsSpacesRects = new List<RectangleF>();

            RectangleF InterOptionsSpaceRegionVertical = new RectangleF(new PointF(OptionsRegion.X + (OptionsRegion.Size.Width / 2f) - 1.35f, OptionsRegion.Y + OptionsRegion.Size.Height), new SizeF(10f, (float)InterOptionsSpace));
            RectangleF InterOptionsSpaceRegionHorizontal = new RectangleF(new PointF(OptionsRegion.X + OptionsRegion.Size.Width, OptionsRegion.Y + (OptionsRegion.Size.Height / 2f) - 1.35f), new SizeF((float)InterOptionsSpace, 10f));

            float interSpaceVerticalWidth = 2.5f;
            float interSpaceHorizontalHeight = 2.5f;

            RectangleF lastFieldInterOptionRect = new RectangleF();
            for (int i0 = 0; i0 < TotalFields; i0++)
            {
                RectangleF curInterOptionSpaceRectF = new RectangleF();

                for (int i = 0; i < TotalOptions; i++)
                {
                    switch (Orientation)
                    {
                        case Orientation.Horizontal:
                            switch (InterOptionsSpaceType)
                            {
                                case InterSpaceType.CONSTANT:
                                    if (i == TotalOptions - 1)
                                        break;

                                    switch (InterFieldsSpaceType)
                                    {
                                        case InterSpaceType.CONSTANT:
                                            curInterOptionSpaceRectF = new RectangleF(new PointF(InterOptionsSpaceRegionHorizontal.X + (float)(i * (InterOptionsSpace + OptionsRegion.Width)), InterOptionsSpaceRegionHorizontal.Y + (float)(i0 * (InterFieldsSpace + FieldsRegion.Height))), new SizeF((float)InterOptionsSpace, interSpaceHorizontalHeight));
                                            break;
                                        case InterSpaceType.ARRAY:
                                            curInterOptionSpaceRectF = new RectangleF(new PointF(InterOptionsSpaceRegionHorizontal.X + (float)(i * (InterOptionsSpace + OptionsRegion.Width)), InterOptionsSpaceRegionHorizontal.Y + (float)(i0 * (InterFieldsSpaces[i0] + FieldsRegion.Height))), new SizeF((float)InterOptionsSpace, interSpaceHorizontalHeight));
                                            break;
                                    }
                                    break;
                                case InterSpaceType.ARRAY:
                                    switch (InterFieldsSpaceType)
                                    {
                                        case InterSpaceType.CONSTANT:
                                            curInterOptionSpaceRectF = new RectangleF(new PointF(i == 0 ? OptionsRegion.X : optionsRects[i - 1].Right, InterOptionsSpaceRegionHorizontal.Y + (float)(i0 * (InterFieldsSpace + FieldsRegion.Height))), new SizeF((float)InterOptionsSpaces[i], interSpaceHorizontalHeight));
                                            break;
                                        case InterSpaceType.ARRAY:
                                            curInterOptionSpaceRectF = new RectangleF(new PointF(i == 0 ? OptionsRegion.X : optionsRects[i - 1].Right, i0 == 0 ? InterOptionsSpaceRegionHorizontal.Y + (float)InterFieldsSpaces[0] : lastFieldInterOptionRect.Y + (float)(InterFieldsSpaces[i0] + FieldsRegion.Height)), new SizeF((float)InterOptionsSpaces[i], interSpaceHorizontalHeight));
                                            break;
                                    }
                                    break;
                            }
                            break;
                        case Orientation.Vertical:
                            switch (InterOptionsSpaceType)
                            {
                                case InterSpaceType.CONSTANT:
                                    if (i == TotalOptions - 1)
                                        break;

                                    switch (InterFieldsSpaceType)
                                    {
                                        case InterSpaceType.CONSTANT:
                                            curInterOptionSpaceRectF = new RectangleF(new PointF(InterOptionsSpaceRegionVertical.X + (float)(i0 * (InterFieldsSpace + FieldsRegion.Width)), InterOptionsSpaceRegionVertical.Y + (float)(i * (InterOptionsSpace + OptionsRegion.Height))), new SizeF(interSpaceVerticalWidth, (float)InterOptionsSpace));
                                            break;
                                        case InterSpaceType.ARRAY:
                                            curInterOptionSpaceRectF = new RectangleF(new PointF(InterOptionsSpaceRegionVertical.X + (float)(i0 * (InterFieldsSpaces[i0] + FieldsRegion.Width)), InterOptionsSpaceRegionVertical.Y + (float)(i * (InterOptionsSpace + OptionsRegion.Height))), new SizeF(interSpaceVerticalWidth, (float)InterOptionsSpace));
                                            break;
                                    }
                                    break;
                                case InterSpaceType.ARRAY:
                                    switch (InterFieldsSpaceType)
                                    {
                                        case InterSpaceType.CONSTANT:
                                            curInterOptionSpaceRectF = new RectangleF(new PointF(InterOptionsSpaceRegionVertical.X + (float)(i0 * (InterFieldsSpace + FieldsRegion.Width)), i == 0 ? OptionsRegion.Y : optionsRects[i - 1].Bottom), new SizeF(interSpaceVerticalWidth, (float)InterOptionsSpaces[i]));
                                            break;
                                        case InterSpaceType.ARRAY:
                                            curInterOptionSpaceRectF = new RectangleF(new PointF(InterOptionsSpaceRegionVertical.X + (float)(i0 * (InterFieldsSpaces[i0] + FieldsRegion.Width)), i == 0 ? OptionsRegion.Y : optionsRects[i - 1].Bottom), new SizeF(interSpaceVerticalWidth, (float)InterOptionsSpaces[i]));
                                            break;
                                    }
                                    break;
                            }

                            break;
                    }

                    interOptionsSpacesRects.Add(curInterOptionSpaceRectF);
                }
                lastFieldInterOptionRect = curInterOptionSpaceRectF;
            }

            return new List<RectangleF>(interFieldsSpacesRects);
        }
        #endregion
    }
    #endregion

    [Serializable]
    internal class OMRConfiguration : ConfigurationBase
    {
        #region Public Properties
        [Browsable(false)]
        public OMRRegionData RegionData { get { return regionData; } set { regionData = value; } }
        [Browsable(false)]
        public int GetTotalFields { get { return RegionData.TotalFields; } set { } }
        [Browsable(false)]
        public int GetTotalOptions { get { return RegionData.TotalOptions; } set { } }
        [Category("Layout"), Description("Gets or sets the orientation of the OMR Region.")]
        public Orientation Orientation { get => RegionData.Orientation; set => RegionData.Orientation = value; }
        [Category("Behaviour"), Description("Gets or sets the type of the OMR Region.")]
        public OMRType OMRType { get; set; }
        [Category("Behaviour"), Description("Gets or sets the action upon multiple markings in a field for the OMR Region.")]
        public MultiMarkAction MultiMarkAction { get; set; }
        [Category("Behaviour"), Description("Gets or sets the action taken when none of the options in a field is marked for the OMR Region.")]
        public NoneMarkedAction NoneMarkedAction { get; set; }
        [Category("Behaviour"), Description("Gets or sets the type of key to use for the OMR Region.")]
        public KeyType KeyType { get; set; }
        [Category("Behaviour"), Description("Gets or sets the type of character shown for multi marked field for the OMR Region.")]
        public char MultiMarkSymbol { get; set; }
        [Category("Behaviour"), Description("Gets or sets the type of character shown for multi marked field for the OMR Region.")]
        public char NoneMarkedSymbol { get; set; }
        [Category("Behaviour"), Description("Gets or sets the type of character shown for multi marked field for the OMR Region.")]
        public double BlackCountThreshold { get; set; }
        #endregion

        #region Private Properties
        private OMRRegionData regionData;
        #endregion

        #region Variables
        public Dictionary<Parameter, AnswerKey> PB_AnswerKeys = new Dictionary<Parameter, AnswerKey>();
        public AnswerKey GeneralAnswerKey;
        #endregion

        #region Public Methods
        public OMRConfiguration(ConfigurationBase _base, OMRRegionData regionData, Orientation orientation, OMRType oMRType, MultiMarkAction multiMarkAction, KeyType keyType, char multiMarkSymbol, char noneMarkedSymbol, double blackCountThreshold) : base(_base)
        {
            this.regionData = regionData;
            Orientation = orientation;
            OMRType = oMRType;
            MultiMarkAction = multiMarkAction;
            KeyType = keyType;
            MultiMarkSymbol = multiMarkSymbol;
            NoneMarkedSymbol = noneMarkedSymbol;
            BlackCountThreshold = blackCountThreshold;
        }
        public OMRConfiguration(BaseData _baseData, OMRRegionData regionData, Orientation orientation, OMRType oMRType, MultiMarkAction multiMarkAction, KeyType keyType, char multiMarkSymbol, char noneMarkedSymbol, double blackCountThreshold) : base(_baseData)
        {
            this.regionData = regionData;
            Orientation = orientation;
            OMRType = oMRType;
            MultiMarkAction = multiMarkAction;
            KeyType = keyType;
            MultiMarkSymbol = multiMarkSymbol;
            NoneMarkedSymbol = noneMarkedSymbol;
            BlackCountThreshold = blackCountThreshold;
        }

        #region Answer Key
        public bool SetGeneralAnswerKey(AnswerKey key, out string err)
        {
            bool isSet = true;
            err = "";

            if (GeneralAnswerKey != null && Messages.ShowQuestion("A general key already exists, would you like to override it?") == DialogResult.No)
            {
                err = "User Denied";
                return false;
            }

            GeneralAnswerKey = new AnswerKey(key);

            return isSet;
        }
        public bool SetPBAnswerKeys(Dictionary<Parameter, AnswerKey> PB_AnswerKeys, out string err)
        {
            bool isSet = true;
            err = "";

            if (PB_AnswerKeys == null || PB_AnswerKeys.Count == 0)
            {
                err = "Invalid Parameter";
                return false;
            }
            this.PB_AnswerKeys = new Dictionary<Parameter, AnswerKey>(PB_AnswerKeys);

            return isSet;
        }
        public bool AddPBAnswerKey(Parameter parameter, AnswerKey answerKey, out string err)
        {
            bool isSet = true;
            err = "";

            if (PB_AnswerKeys == null)
                PB_AnswerKeys = new Dictionary<Parameter, AnswerKey>();

            string paramValue = parameter.parameterValue;
            if (paramValue == "" || parameter.parameterConfig == null)
            {
                err = "Invalid parameter value and/or configuration.";
                return false;
            }

            switch (parameter.parameterConfig.ValueDataType)
            {
                case ValueDataType.String:
                    break;
                case ValueDataType.Text:
                    if (!paramValue.All(char.IsLetter))
                    {
                        err = "Invalid parameter value, Text was expected.";
                        return false;
                    }
                    break;
                case ValueDataType.Alphabet:
                    if (!paramValue.All(char.IsLetter))
                    {
                        err = "Invalid parameter value, Text was expected.";
                        return false;
                    }
                    break;
                case ValueDataType.WholeNumber:
                    if (!paramValue.All(char.IsDigit))
                    {
                        err = "Invalid parameter value, Whole Number was expected.";
                        return false;
                    }
                    break;
                case ValueDataType.NaturalNumber:
                    if (!paramValue.All(char.IsDigit) || paramValue == "0")
                    {
                        err = "Invalid parameter value, Natural Number was expected.";
                        return false;
                    }
                    break;
                case ValueDataType.Integer:
                    if (!paramValue.All(char.IsDigit))
                    {
                        err = "Invalid parameter value, Integer was expected.";
                        return false;
                    }
                    break;
            }

            if (PB_AnswerKeys.Values.Any(x => x.Title == answerKey.Title))
            {
                if (Messages.ShowQuestion("A key with this title already exists, would you like to override it?") == DialogResult.No)
                {
                    err = "User Denied";
                    return false;
                }
                Parameter currentKeyParam = PB_AnswerKeys.First(x => x.Value.Title == answerKey.Title).Key;
                PB_AnswerKeys.Remove(currentKeyParam);
                PB_AnswerKeys[parameter] = answerKey;
            }
            else if (PB_AnswerKeys.Keys.Any(x => x.parameterConfig == parameter.parameterConfig && x.parameterValue == parameter.parameterValue))
            {
                if (Messages.ShowQuestion("A key with this parameter already exists, would you like to override it?") == DialogResult.No)
                {
                    err = "User Denied";
                    return false;
                }
                Parameter currentKeyParam = PB_AnswerKeys.Keys.First(x => x.parameterConfig == parameter.parameterConfig && x.parameterValue == parameter.parameterValue);
                PB_AnswerKeys[currentKeyParam] = answerKey;
            }
            else
                PB_AnswerKeys.Add(parameter, answerKey);

            return isSet;
        }
        public bool RemoveKey(out string err)
        {
            bool result = true;
            err = "";

            GeneralAnswerKey = null;

            return result;
        }
        public bool RemoveKey(Parameter parameter, out string err)
        {
            bool result = true;
            err = "";

            if (PB_AnswerKeys == null)
            {
                err = "No Parameter Based Answer Keys Found";
                return false;
            }

            if (!PB_AnswerKeys.Keys.Any(x => x.parameterConfig == parameter.parameterConfig && x.parameterValue == parameter.parameterValue))
            {
                err = "Invalid Parameter";
                return false;
            }

            Parameter currentKeyParam = PB_AnswerKeys.Keys.First(x => x.parameterConfig == parameter.parameterConfig && x.parameterValue == parameter.parameterValue);
            PB_AnswerKeys.Remove(currentKeyParam);

            return result;
        }
        public bool LoadKey(AnswerKey loadedKey, out string err)
        {
            bool result = true;
            err = "";

            try
            {
                if (true)
                {
                    GeneralAnswerKey = new AnswerKey(loadedKey);
                    result = true;
                }
                else
                {
                    err = "The provided key is not compatible";
                    result = false;
                }
            }
            catch (Exception ex)
            {
                err = ex.Message;
                result = false;
            }

            return result;
        }
        internal bool LoadKey(Parameter parameter, AnswerKey loadedKey, out string error)
        {
            string err = "";
            bool result = false;
            if (true)
            {
                result = AddPBAnswerKey(parameter, loadedKey, out err);
            }
            else
            {
                err = "The provided key is not compatible";
                result = false;
            }
            error = err;
            return result;
        }
        #endregion

        public char[] GetEscapeSymbols()
        {
            return new char[] { MultiMarkSymbol, NoneMarkedSymbol };
        }
        #endregion

        #region Static Methods
        public static OMRConfiguration CreateDefault(string regionName, Orientation orientation, ConfigArea configArea, OMRRegionData regionData, int processingIndex)
        {
            BaseData configurationBase = new BaseData(regionName, MainConfigType.OMR, configArea, ValueDataType.Integer, Typography.Continious, ValueRepresentation.Collective, ValueEditType.ReadOnly, new ConfigRange(), processingIndex);
            return new OMRConfiguration(configurationBase, regionData, orientation, OMRType.Parameter, MultiMarkAction.MarkAsManual, KeyType.General, '#', '*', 0.45);
        }

        public override ProcessedDataEntry ProcessSheet(Mat sheet)
        {
            OMREngine omrEngine = new OMREngine();
            return omrEngine.ProcessSheet(this, sheet);
        }
        public async Task<ProcessedDataEntry> ProcessSheetRaw(Mat sheet, Action<RectangleF, bool> OnOptionProcessed, Func<double> GetWaitMS)
        {
            OMREngine omrEngine = new OMREngine();
            return await omrEngine.ProcessSheetRaw(this, sheet, OnOptionProcessed, GetWaitMS);
        }
        #endregion
    }
}
