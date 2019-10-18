using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace MessagesTensorFlow
{
    public class TFEngine
    {
        static readonly string _assetsPath = Path.Combine(Environment.CurrentDirectory, "assets");
        static readonly string _imagesFolder = Path.Combine(_assetsPath, "train");
        static readonly string _savePath = Path.Combine(_assetsPath, "predict");
        static readonly string _trainTagsTsv = Path.Combine(_imagesFolder, "tags.tsv");
        static readonly string _inceptionTensorFlowModel = Path.Combine(_assetsPath, "inception5h", "tensorflow_inception_graph.pb");        

        static readonly object _lock = new object();
        
        private static TFEngine _instance;
        public static TFEngine Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new TFEngine();
                    }
                    return _instance;
                }

            }
        }
    }
}
