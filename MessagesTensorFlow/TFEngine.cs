using Microsoft.ML;
using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace MessagesTensorFlow
{
    public class TFEngine
    {
        static readonly string _assetsPath = Path.Combine(Environment.CurrentDirectory, "assets");
        static readonly string _imagesFolder = Path.Combine(_assetsPath, "train");
        static readonly string _savePath = Path.Combine(_assetsPath, "predict");
        static readonly string _trainTagsTsv = Path.Combine(_imagesFolder, "tags.tsv");
        static readonly string _inceptionTensorFlowModel = Path.Combine(_assetsPath, "inception5h", "tensorflow_inception_graph.pb");

        const int ImageHeight = 224;
        const int ImageWidth = 224;
        const float Mean = 117;
        const bool ChannelsLast = true;

        static readonly object _lock = new object();

        private static WebClient _client = new WebClient();
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
        public class ImageData
        {
            [LoadColumn(0)]
            public string ImagePath;

            [LoadColumn(1)]
            public string Label;
        }

        public class ImagePrediction : ImageData
        {
            public float[] Score;

            public string PredictedLabelValue;
        }


        private TFEngine()
        {
            _mlContext = new MLContext();
            GenerateModel();
        }
        private IEstimator<ITransformer> _pipeline;
        private ITransformer _model;
        private MLContext _mlContext;

        public void GenerateModel()
        {
            _pipeline = _mlContext.Transforms.LoadImages(outputColumnName: "input", imageFolder: _imagesFolder, inputColumnName: nameof(ImageData.ImagePath))                
                .Append(_mlContext.Transforms.ResizeImages(outputColumnName: "input", imageWidth: ImageWidth, imageHeight: ImageHeight, inputColumnName: "input"))
                .Append(_mlContext.Transforms.ExtractPixels(outputColumnName: "input", interleavePixelColors: ChannelsLast, offsetImage: Mean))
                .Append(_mlContext.Model.LoadTensorFlowModel(_inceptionTensorFlowModel)
                .ScoreTensorFlowModel(outputColumnNames: new[] { "softmax2_pre_activation" }, inputColumnNames: new[] { "input" }, addBatchDimensionInput: true))
                .Append(_mlContext.Transforms.Conversion.MapValueToKey(outputColumnName: "LabelKey", inputColumnName: "Label"))
                .Append(_mlContext.MulticlassClassification.Trainers.LbfgsMaximumEntropy(labelColumnName: "LabelKey", featureColumnName: "softmax2_pre_activation"))
                .Append(_mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabelValue", "PredictedLabel"))
                .AppendCacheCheckpoint(_mlContext);
            IDataView trainingData = _mlContext.Data.LoadFromTextFile<ImageData>(path: _trainTagsTsv, hasHeader: false);
            _model = _pipeline.Fit(trainingData);            
        }
        public string ClassifySingleImage(string imageUrl)
        {
            try
            {
                var filename = Path.Combine(_savePath, $"{Guid.NewGuid()}.jpg");
                _client.DownloadFile(imageUrl, filename);
                var imageData = new ImageData()
                {
                    ImagePath = filename
                };

                var predictor = _mlContext.Model.CreatePredictionEngine<ImageData, ImagePrediction>(_model);
                var prediction = predictor.Predict(imageData);
                var response = $"I'm about {prediction.Score.Max() * 100}% sure that the image you sent me is a {prediction.PredictedLabelValue}";
                Console.WriteLine($"Image: {Path.GetFileName(imageData.ImagePath)} predicted as: {prediction.PredictedLabelValue} with score: {prediction.Score.Max() * 100} ");
                return response;
            }
            catch (Exception)
            {
                return "Something went wrong when trying to classify image";
            }
        }

        public string AddTrainingImage(string imageUrl, string label)
        {
            try
            {
                var id = Guid.NewGuid();
                var fileName = Path.Combine(_imagesFolder, $"{id}.jpg");
                _client.DownloadFile(imageUrl, fileName);
                File.AppendAllText(_trainTagsTsv, $"{id}.jpg\t{label}" + Environment.NewLine);
                IDataView trainingData = _mlContext.Data.LoadFromTextFile<ImageData>(path: _trainTagsTsv, hasHeader: false);
                _model = _pipeline.Fit(trainingData);                
                return $"I have trained myself to recognize the image you sent me as a {label}. Your teaching is appreciated";
            }
            catch (Exception)
            {
                return "something went wrong when trying to train on image";
            }
        }
    }
}
