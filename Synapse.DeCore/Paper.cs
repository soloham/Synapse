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
    public class ExamPapers
    {
        public ExamPapers(List<Paper> papers)
        {
            GetPapers = papers;
        }

        public List<Paper> GetPapers { get; private set; }
    }
    [Serializable]
    public class Paper
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
        public int GetCorrectOptionValue { get => 1; private set { } }
        public int GetWrongOptionValue { get => 0; private set { } }

        public void Reset(Paper newPaper)
        {
            Code = newPaper.Code;
            Title = newPaper.Title;
            GetFieldsCount = newPaper.GetFieldsCount;
            GetOptionsCount = newPaper.GetOptionsCount;
            GetPaperDirection = newPaper.GetPaperDirection;
        }
    }
}
