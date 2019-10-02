using Synapse.Utilities.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Synapse.Core.Managers
{
    public class GeneralManager
    {
        internal static ExamPapers GetExamPapers { get; private set; }

        public async static Task Initialize()
        {
            GetExamPapers = await LSTM.LoadPapers();

            if (GetExamPapers == null)
                GetExamPapers = new ExamPapers(new List<Paper>());
        }

        internal static bool RemovePaper(Paper paper)
        {
            return GetExamPapers.GetPapers.Remove(paper);
        }
        internal async static Task<bool> RemovePaper(int paperCode)
        {
            Paper paper = GetExamPapers.GetPapers.Find(x => x.Code == paperCode);
            if (paper == null)
                return false;

            bool isSuccess = GetExamPapers.GetPapers.Remove(paper);

            await LSTM.SavePapers(GetExamPapers);

            return isSuccess;
        }
    }
}
