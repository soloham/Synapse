using Synapse.Utilities.Attributes;
using System;
using System.Collections.Generic;

namespace Synapse.Core
{
    public enum PaperDirection
    {
        [EnumDescription("Right to Left")]
        RightToLeft,
        [EnumDescription("Left to Right")]
        LeftToRight
    }
    [Serializable]
    internal class ExamPapers
    {
        public ExamPapers(List<Paper> papers)
        {
            GetPapers = papers;
        }

        public List<Paper> GetPapers { get; private set; }
    }
    [Serializable]
    internal class Paper
    {
        public Paper(int code, string title, int getFieldsCount, int getOptionsCount, PaperDirection getPaperDirection)
        {
            Code = code;
            Title = title;
            GetFieldsCount = getFieldsCount;
            GetOptionsCount = getOptionsCount;
            GetPaperDirection = getPaperDirection;
        }

        public int Code { get; private set; }
        public string Title { get; private set; }
        public int GetFieldsCount { get; private set; }
        public int GetOptionsCount { get; private set; }
        public PaperDirection GetPaperDirection { get; private set; }

        internal void Reset(Paper newPaper)
        {
            Code = newPaper.Code;
            Title = newPaper.Title;
            GetFieldsCount = newPaper.GetFieldsCount;
            GetOptionsCount = newPaper.GetOptionsCount;
            GetPaperDirection = newPaper.GetPaperDirection;
        }
    }
}
