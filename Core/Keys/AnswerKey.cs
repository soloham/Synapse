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
    internal class AnswerKey
    {
        public string Title { get; private set; }
        public string GetConfigName { get; private set; }
        public int[][] GetKey { get; private set; }
        public Paper GetPaperCode { get; private set; }

        public AnswerKey(AnswerKey key)
        {
            Title = key.Title;
            GetConfigName = key.GetConfigName;
            GetKey = key.GetKey;
            GetPaperCode = key.GetPaperCode;
        }
        public AnswerKey(string title, string getConfigName, int[][] getKey, Paper getPaperCode)
        {
            Title = title;
            GetConfigName = getConfigName;
            GetKey = getKey;
            GetPaperCode = getPaperCode;
        }
    }
}
