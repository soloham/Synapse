namespace Synapse.Core
{
    using System;
    using System.Collections.Generic;

    using Synapse.Utilities.Attributes;

    public enum PaperDirection
    {
        [EnumDescription("Right to Left")] RightToLeft,
        [EnumDescription("Left to Right")] LeftToRight
    }

    [Serializable]
    public class ExamPapers
    {
        public ExamPapers(List<Paper> papers)
        {
            this.GetPapers = papers;
        }

        public List<Paper> GetPapers { get; private set; }
    }

    [Serializable]
    public class Paper
    {
        public Paper(string code, string title, int getFieldsCount, int getOptionsCount,
            PaperDirection getPaperDirection)
        {
            this.Code = code;
            this.Title = title;
            this.GetFieldsCount = getFieldsCount;
            this.GetOptionsCount = getOptionsCount;
            this.GetPaperDirection = getPaperDirection;
        }

        public string Code { get; private set; }
        public string Title { get; private set; }
        public int GetFieldsCount { get; private set; }
        public int GetOptionsCount { get; private set; }
        public PaperDirection GetPaperDirection { get; private set; }

        public int GetCorrectOptionValue
        {
            get => 1;
            private set { }
        }

        public int GetWrongOptionValue
        {
            get => 0;
            private set { }
        }

        public void Reset(Paper newPaper)
        {
            this.Code = newPaper.Code;
            this.Title = newPaper.Title;
            this.GetFieldsCount = newPaper.GetFieldsCount;
            this.GetOptionsCount = newPaper.GetOptionsCount;
            this.GetPaperDirection = newPaper.GetPaperDirection;
        }
    }
}