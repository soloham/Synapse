using Synapse.Utilities.Attributes;
using System;

namespace Synapse.Core.Keys
{
    #region Enums
    public enum KeyType
    {
        [EnumDescription("General")]
        General,
        [EnumDescription("Parameter Based")]
        ParameterBased
    }
    #endregion

    [Serializable]
    public class AnswerKey
    {
        public string Title { get; private set; }
        public string GetConfigName { get; private set; }
        public int[][] GetKey { get; private set; }
        public Paper GetPaper { get; private set; }
        public bool IsActive { get; set; }

        public AnswerKey(AnswerKey key)
        {
            Title = key.Title;
            GetConfigName = key.GetConfigName;
            GetKey = key.GetKey;
            GetPaper = key.GetPaper;
            IsActive = key.IsActive;
        }
        public AnswerKey(string title, string getConfigName, int[][] getKey, Paper getPaperCode)
        {
            Title = title;
            GetConfigName = getConfigName;
            GetKey = getKey;
            GetPaper = getPaperCode;
            IsActive = true;
        }
    }
}
