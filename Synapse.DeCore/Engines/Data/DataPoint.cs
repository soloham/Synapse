using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Synapse.DeCore.Engines.Data
{
    [Serializable]
    public class DataPoint : ISerializable
    {
        #region Constants
        private const int Version = 1;
        #endregion

        #region Properties
        public (ObservableCollection<Dictionary<string, object>> processedDataSource, ObservableCollection<Dictionary<string, object>> manProcessedDataSource, ObservableCollection<Dictionary<string, object>> fauProcessedDataSource, ObservableCollection<Dictionary<string, object>> incProcessedDataSource) ProcessedData { get; set; }
        #endregion

        #region Constructors
        public DataPoint((ObservableCollection<Dictionary<string, object>> processedDataSource, ObservableCollection<Dictionary<string, object>> manProcessedDataSource, ObservableCollection<Dictionary<string, object>> fauProcessedDataSource, ObservableCollection<Dictionary<string, object>> incProcessedDataSource) processedData)
        {
            ProcessedData = processedData;
        }
        public DataPoint(ObservableCollection<Dictionary<string, object>> processedDataSource, ObservableCollection<Dictionary<string, object>> manProcessedDataSource, ObservableCollection<Dictionary<string, object>> fauProcessedDataSource, ObservableCollection<Dictionary<string, object>> incProcessedDataSource)
        {
            ProcessedData = (processedDataSource, manProcessedDataSource, fauProcessedDataSource, incProcessedDataSource);
        }

        public DataPoint(SerializationInfo info, StreamingContext context)
        {
            int version = info.GetInt32("Version");
            if (version == 1)
            {
                // Restore properties for version 1
                ProcessedData = ((ObservableCollection<Dictionary<string, object>> processedDataSource, ObservableCollection<Dictionary<string, object>> manProcessedDataSource, ObservableCollection<Dictionary<string, object>> fauProcessedDataSource, ObservableCollection<Dictionary<string, object>> incProcessedDataSource))info.GetValue("ProcessedData", typeof((ObservableCollection<Dictionary<string, object>> processedDataSource, ObservableCollection<Dictionary<string, object>> manProcessedDataSource, ObservableCollection<Dictionary<string, object>> fauProcessedDataSource, ObservableCollection<Dictionary<string, object>> incProcessedDataSource)));
            }
        }
        #endregion

        #region Methods
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Version", Version);
            info.AddValue("ProcessedData", ProcessedData);
        }

        public (ObservableCollection<dynamic> processedDataSource, ObservableCollection<dynamic> manProcessedDataSource, ObservableCollection<dynamic> fauProcessedDataSource, ObservableCollection<dynamic> incProcessedDataSource) ReturnProcessedData()
        {
            (ObservableCollection<dynamic> processedDataSource, ObservableCollection<dynamic> manProcessedDataSource, ObservableCollection<dynamic> fauProcessedDataSource, ObservableCollection<dynamic> incProcessedDataSource) result = (new ObservableCollection<dynamic>(), new ObservableCollection<dynamic>(), new ObservableCollection<dynamic>(), new ObservableCollection<dynamic>());
            for (int i = 0; i < ProcessedData.processedDataSource.Count; i++)
            {
                result.processedDataSource.Add((dynamic)Shared.Utilities.Extensions.DictionaryExtension.ToExpando(ProcessedData.processedDataSource[i]));
            }
            //for (int i = 0; i < ProcessedData.manProcessedDataSource.Count; i++)
            //{
            //    result.manProcessedDataSource.Add((dynamic)Shared.Utilities.Extensions.DictionaryExtension.ToExpando(ProcessedData.manProcessedDataSource[i]));
            //}
            //for (int i = 0; i < ProcessedData.fauProcessedDataSource.Count; i++)
            //{
            //    result.fauProcessedDataSource.Add((dynamic)Shared.Utilities.Extensions.DictionaryExtension.ToExpando(ProcessedData.fauProcessedDataSource[i]));
            //}
            //for (int i = 0; i < ProcessedData.incProcessedDataSource.Count; i++)
            //{
            //    result.incProcessedDataSource.Add((dynamic)Shared.Utilities.Extensions.DictionaryExtension.ToExpando(ProcessedData.incProcessedDataSource[i]));
            //}

            for (int i = 0; i < result.processedDataSource.Count; i++)
            {
                dynamic curData = result.processedDataSource[i];
                switch (curData.DataRowObject.DataRowResultType)
                {
                    case Core.Engines.Data.ProcessedDataType.NORMAL:
                        break;
                    case Core.Engines.Data.ProcessedDataType.MANUAL:
                        result.manProcessedDataSource.Add(curData);
                        break;
                    case Core.Engines.Data.ProcessedDataType.FAULTY:
                        result.fauProcessedDataSource.Add(curData);
                        break;
                    case Core.Engines.Data.ProcessedDataType.INCOMPATIBLE:
                        result.incProcessedDataSource.Add(curData);
                        break;
                    default:
                        break;
                }
            }

            return result;
        }

        public static bool Save(DataPoint dataPoint, string location, out Exception ex)
        {
            try
            {
                using (FileStream fs = new FileStream(location, FileMode.Create))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(fs, dataPoint);
                }

                ex = null;
                return true;
            }
            catch(Exception _ex)
            {
                ex = _ex;

                return false;
            }
        }
        public static DataPoint Load(string location, out Exception ex)
        {
            DataPoint result = null;
            ex = null;
            try
            {
                using(FileStream fs = new FileStream(location, FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    result = (DataPoint)bf.Deserialize(fs);
                }
            }
            catch(Exception _ex)
            {
                ex = _ex;
            }

            return result;
        }
        #endregion

    }
}