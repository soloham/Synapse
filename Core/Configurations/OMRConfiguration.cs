using Synapse.Core.Keys;
using Synapse.Utilities;
using Synapse.Utilities.Attributes;
using System;
using System.Collections.Generic;
using System.Drawing;
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
        GRADABLE,
        [EnumDescription("Non Gradable")]
        NON_GRADABLE
    }
    public enum MultiMarkAction
    {
        [EnumDescription("Mark As Manual")]
        MARK_AS_MANUAL,
        [EnumDescription("Invalidate")]
        INVALIDATE
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

        public OMRRegionData(RectangleF fieldsRegion, InterSpaceType interFieldsSpaceType, double interFieldsSpace, double[] interFieldsSpaces, RectangleF optionsRegion, InterSpaceType interOptionsSpaceType, double interOptionsSpace, double[] interOptionsSpaces)
        {
            FieldsRegion = fieldsRegion;
            InterFieldsSpaceType = interFieldsSpaceType;
            InterFieldsSpace = interFieldsSpace;
            InterFieldsSpaces = interFieldsSpaces;

            OptionsRegion = optionsRegion;
            InterOptionsSpaceType = interOptionsSpaceType;
            InterOptionsSpace = interOptionsSpace;
            InterOptionsSpaces = interOptionsSpaces;
        }

        #region Fields Properties
        public RectangleF FieldsRegion { get; set; }
        public InterSpaceType InterFieldsSpaceType { get; set; }
        public double InterFieldsSpace { get; set; }
        public double[] InterFieldsSpaces { get; set; }
        #endregion

        #region Options Properties
        public RectangleF OptionsRegion { get; set; }
        public InterSpaceType InterOptionsSpaceType { get; set; }
        public double InterOptionsSpace { get; set; }
        public double[] InterOptionsSpaces { get; set; }
        #endregion
    }
    #endregion

    [Serializable]
    internal class OMRConfiguration : ConfigurationBase
    {
        #region Public Properties
        public OMRRegionData RegionData { get { return regionData; } set { regionData = value; } }
        public int TotalFields { get; set; }
        public int TotalOptions { get; set; }
        public Orientation Orientation { get; set; }
        public OMRType OMRType { get; set; }
        public MultiMarkAction MultiMarkAction { get; set; }
        public KeyType KeyType { get; set; }
        #endregion

        #region Private Properties
        private OMRRegionData regionData;
        #endregion

        #region Variables
        public Dictionary<Parameter, AnswerKey> PB_AnswerKeys = new Dictionary<Parameter, AnswerKey>();
        public AnswerKey GeneralAnswerKey;
        #endregion

        #region Public Methods
        public OMRConfiguration(ConfigurationBase _base, OMRRegionData regionData, int totalFields, int totalOptions, Orientation orientation, OMRType oMRType, MultiMarkAction multiMarkAction, KeyType keyType) : base(_base)
        {
            this.regionData = regionData;
            TotalFields = totalFields;
            TotalOptions = totalOptions;
            Orientation = orientation;
            OMRType = oMRType;
            MultiMarkAction = multiMarkAction;
            KeyType = keyType;
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

            if (parameter.parameterValue == "" || parameter.parameterConfig == null)
            {
                err = "Invalid Parameter";
                return false;
            }

            if (PB_AnswerKeys.Keys.Any(x => x.parameterConfig == parameter.parameterConfig && x.parameterValue == parameter.parameterValue))
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
        #endregion
    }
}
