namespace Synapse.DeCore.Engines.Data
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;

    using Synapse.Core.Engines.Data;
    using Synapse.Shared.Utilities.Extensions;

    [Serializable]
    public class DataPoint : ISerializable
    {
        #region Constants

        private const int Version = 1;

        #endregion

        #region Properties

        public (ObservableCollection<Dictionary<string, object>> processedDataSource,
            ObservableCollection<Dictionary<string, object>> manProcessedDataSource,
            ObservableCollection<Dictionary<string, object>> fauProcessedDataSource,
            ObservableCollection<Dictionary<string, object>> incProcessedDataSource) ProcessedData { get; set; }

        #endregion

        #region Constructors

        public DataPoint(
            (ObservableCollection<Dictionary<string, object>> processedDataSource,
                ObservableCollection<Dictionary<string, object>> manProcessedDataSource,
                ObservableCollection<Dictionary<string, object>> fauProcessedDataSource,
                ObservableCollection<Dictionary<string, object>> incProcessedDataSource) processedData)
        {
            this.ProcessedData = processedData;
        }

        public DataPoint(ObservableCollection<Dictionary<string, object>> processedDataSource,
            ObservableCollection<Dictionary<string, object>> manProcessedDataSource,
            ObservableCollection<Dictionary<string, object>> fauProcessedDataSource,
            ObservableCollection<Dictionary<string, object>> incProcessedDataSource)
        {
            this.ProcessedData = (processedDataSource, manProcessedDataSource, fauProcessedDataSource,
                incProcessedDataSource);
        }

        public DataPoint(SerializationInfo info, StreamingContext context)
        {
            var version = info.GetInt32("Version");
            if (version == 1)
            {
                // Restore properties for version 1
                this.ProcessedData =
                    ((ObservableCollection<Dictionary<string, object>> processedDataSource,
                        ObservableCollection<Dictionary<string, object>> manProcessedDataSource,
                        ObservableCollection<Dictionary<string, object>> fauProcessedDataSource,
                        ObservableCollection<Dictionary<string, object>> incProcessedDataSource))info.GetValue(
                        "ProcessedData",
                        typeof((ObservableCollection<Dictionary<string, object>> processedDataSource,
                            ObservableCollection<Dictionary<string, object>> manProcessedDataSource,
                            ObservableCollection<Dictionary<string, object>> fauProcessedDataSource,
                            ObservableCollection<Dictionary<string, object>> incProcessedDataSource)));
            }
        }

        #endregion

        #region Methods

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Version", Version);
            info.AddValue("ProcessedData", this.ProcessedData);
        }

        public (ObservableCollection<dynamic> processedDataSource, ObservableCollection<dynamic> manProcessedDataSource,
            ObservableCollection<dynamic> fauProcessedDataSource, ObservableCollection<dynamic> incProcessedDataSource)
            ReturnProcessedData()
        {
            (ObservableCollection<dynamic> processedDataSource, ObservableCollection<dynamic> manProcessedDataSource,
                ObservableCollection<dynamic> fauProcessedDataSource, ObservableCollection<dynamic>
                incProcessedDataSource) result = (new ObservableCollection<dynamic>(),
                    new ObservableCollection<dynamic>(), new ObservableCollection<dynamic>(),
                    new ObservableCollection<dynamic>());
            for (var i = 0; i < this.ProcessedData.processedDataSource.Count; i++)
                result.processedDataSource.Add(
                    (dynamic)DictionaryExtension.ToExpando(this.ProcessedData.processedDataSource[i]));
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

            for (var i = 0; i < result.processedDataSource.Count; i++)
            {
                var curData = result.processedDataSource[i];
                switch (curData.DataRowObject.DataRowResultType)
                {
                    case ProcessedDataType.NORMAL:
                        break;

                    case ProcessedDataType.MANUAL:
                        result.manProcessedDataSource.Add(curData);
                        break;

                    case ProcessedDataType.FAULTY:
                        result.fauProcessedDataSource.Add(curData);
                        break;

                    case ProcessedDataType.INCOMPATIBLE:
                        result.incProcessedDataSource.Add(curData);
                        break;
                }
            }

            return result;
        }

        public static bool Save(DataPoint dataPoint, string location, out Exception ex)
        {
            try
            {
                using (var fs = new FileStream(location, FileMode.Create))
                {
                    var bf = new BinaryFormatter();
                    bf.Serialize(fs, dataPoint);
                }

                ex = null;
                return true;
            }
            catch (Exception _ex)
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
                using (var fs = new FileStream(location, FileMode.Open))
                {
                    var bf = new BinaryFormatter();
                    result = (DataPoint)bf.Deserialize(fs);
                }
            }
            catch (Exception _ex)
            {
                ex = _ex;
            }

            return result;
        }

        #endregion
    }
}