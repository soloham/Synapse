using Emgu.CV;
using Synapse.Core.Engines.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Synapse.Core.Engines.Interface
{
    internal interface IEngine
    {
        #region Methods
        ProcessedDataEntry ProcessSheet(Configurations.ConfigurationBase configuration, Mat sheet);
        #endregion
    }
}
