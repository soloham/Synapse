using Synapse.Utilities.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Synapse.Core.Engines.Data
{
    public enum CustomDataEntryType
    {
        [EnumDescription("Function")]
        Function,
        [EnumDescription("Filled Percent")]
        FilledPercent,
        [EnumDescription("Sheet Index")]
        SheetIndex,
        [EnumDescription("File Name")]
        FileName,
        [EnumDescription("Folder Name")]
        FolderName
    }
    public enum FunctionType
    {
        [EnumDescription("Condition")]
        Condition,
        [EnumDescription("Addition")]
        Addition,
        [EnumDescription("Difference")]
        Difference,
        [EnumDescription("Multiplication")]
        Multiplication,
        [EnumDescription("Division")]
        Division,
        [EnumDescription("Percentage")]
        Percentage,
    }

    public class CustomDataEntry
    {
        #region Objects
        public abstract class CustomFunction
        {
            public FunctionType? CustomDataEntryFunctionType { get; set; }

        }
        #endregion

        #region Properties
        public string FieldTitle { get; private set; }
        public CustomDataEntryType CustomDataEntryType { get; set; }
        #endregion

        #region Variables

        #endregion

        #region Private Methods
        private CustomDataEntry(string fieldTitle, CustomDataEntryType customDataEntryType)
        {
            FieldTitle = fieldTitle;
            CustomDataEntryType = customDataEntryType;
        }
        #endregion

        #region Public Methods
        public static CustomDataEntry CreateDefault(string fieldTitle, CustomDataEntryType customDataEntryType)
        {
            return new CustomDataEntry(fieldTitle, customDataEntryType);
        }
        #endregion
    }
}
