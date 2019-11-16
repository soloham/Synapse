using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Synapse.DeCore.Engines.Data
{
    public class DataPoint : ISerializable
    {
        #region Constants
        private const int Version = 1;
        #endregion

        #region Properties
        public List<ObservableCollection<dynamic>> ProcessedData { get; set; }
        #endregion

        public DataPoint()
        {

        }

        public DataPoint(SerializationInfo info, StreamingContext context)
        {
            int version = info.GetInt32("Example_Version");
            if (version == 1)
            {
                // Restore properties for version 1
                ProcessedData = (List<ObservableCollection<dynamic>>)info.GetValue("ProcessedData", typeof(List<ObservableCollection<dynamic>>));
            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {

        }
    }
}
