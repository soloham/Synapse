using Emgu.CV;
using Synapse.Core.Engines.Data;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Synapse.Core.Engines.Interface
{
    public interface IEngine
    {
        #region Methods
        ProcessedDataEntry ProcessSheet(Configurations.ConfigurationBase configuration, Mat sheet, Action<RectangleF, bool> OnOptionProcessed = null, string originalSheetPath = "");
        #endregion
    }
}
