namespace Synapse.Core.Managers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Synapse.Core.Configurations;
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

            AllConfigurationTypeStringConverter.GetAllConfigurations = () =>
            {
                var parameterConfigs = ConfigurationsManager
                    .GetAllConfigurations
                    .Select(x => x.Title)
                    .ToList();

                return parameterConfigs;
            };

            ParameterConfigurationTypeStringConverter.GetParameterConfigurations = () =>
            {
                var parameterConfigs = ConfigurationsManager
                    .GetConfigurations(MainConfigType.OMR,
                        x =>
                        {
                            var omrX = (OMRConfiguration)x;
                            return omrX.OMRType == OMRType.Parameter;
                        })
                    .Select(x => x.Title)
                    .ToList();

                return parameterConfigs;
            };
        }

        public static bool RemovePaper(Paper paper)
        {
            return GetExamPapers.GetPapers.Remove(paper);
        }

        public static async Task<bool> RemovePaper(string paperCode)
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