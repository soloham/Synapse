namespace Synapse.Core.Managers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Synapse.Utilities.Memory;

    public class GeneralManager
    {
        public static ExamPapers GetExamPapers { get; private set; }

        public static async Task Initialize()
        {
            GetExamPapers = await LSTM.LoadPapers();

            if (GetExamPapers == null)
            {
                GetExamPapers = new ExamPapers(new List<Paper>());
            }
        }

        public static bool RemovePaper(Paper paper)
        {
            return GetExamPapers.GetPapers.Remove(paper);
        }

        public static async Task<bool> RemovePaper(int paperCode)
        {
            var paper = GetExamPapers.GetPapers.Find(x => x.Code == paperCode);
            if (paper == null)
            {
                return false;
            }

            var isSuccess = GetExamPapers.GetPapers.Remove(paper);

            await LSTM.SavePapers(GetExamPapers);

            return isSuccess;
        }
    }
}