using Synapse.Core.Configurations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Synapse.DeCore.Engines.Data
{
    public class Communicator
    {
        public static Func<string, ConfigurationBase> GetConfigurationBase;

        public static void Initialize(Func<string, ConfigurationBase> getCongigBaseFunc)
        {
            GetConfigurationBase = getCongigBaseFunc;
        }
    }
}
