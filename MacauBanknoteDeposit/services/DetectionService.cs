using MacauBanknoteDeposit.Extensions;
using OpenCvSharp;
using System;
using System.IO;
using System.Linq;
using Tensorflow;
using Tensorflow.Keras.Engine;
using static Tensorflow.Binding;
using static Tensorflow.KerasApi;

namespace MacauBanknoteDeposit.Services
{
    public class DetectionService : IDisposable
    {
        private const int InputSize = 224;
        private IModel _model; // 使用接口類型
        private string[] _labels;
        private bool _disposed;

        public DetectionResult LastResult { get; private set; }
        public float ConfidenceThreshold { get; private set; }

        public DetectionService()
        {
            LoadModel();
            LoadLabels();
            WarmUpModel();
        }



        private void LoadModel()
        {
            var modelPath = GetModelPath("keras_model.h5");
            _model = keras.models.load_model(modelPath);
            // 驗證輸入形狀的新方法
            ValidateInputShape();
        }

        private void LoadLabels()
        {
            var labelPath = GetModelPath("labels.txt");

            if (!File.Exists(labelPath))
                throw new FileNotFoundException($"標籤文件不存在：{labelPath}");

            _labels = File.ReadAllLines(labelPath);

            if (_labels.Length != 6 ||
                !_labels[0].StartsWith("MOP") ||
                !_labels[^1].StartsWith("MOP"))
            {
                throw new FormatException("標籤檔案格式錯誤，應包含6個以MOP開頭的面額");
            }
        }

        private string GetModelPath(string filename)
        {
            return Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "Model",
                filename
            );
        }

        private void WarmUpModel()
        {
            try
            {
                using var dummyImage = new Mat(InputSize, InputSize, MatType.CV_8UC3, Scalar.All(0));
                Detect(dummyImage);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("模型預熱失敗，請檢查模型相容性", ex);
            }
        }

        private void ValidateInputShape()
        {
            // 轉換為具體字典類型
            var config = _model.Layers[0].get_config() as System.Collections.Generic.IDictionary<string, object>;

            if (config != null && config.ContainsKey("batch_input_shape"))
            {
                // 取得並轉換輸入形狀
                var inputShape = config["batch_input_shape"] as System.Collections.IList;
                if (inputShape != null && inputShape.Count >= 3)
                {
                    // 取得並轉換輸入形狀
                    var dim1 = Convert.ToInt32(inputShape[1]);
                    var dim2 = Convert.ToInt32(inputShape[2]);

                    if (dim1 != InputSize || dim2 != InputSize)
                        throw new InvalidOperationException("模型輸入尺寸應為224x224");
                }
            }
        }

        public DetectionResult Detect(Mat inputImage)
        {
            if (inputImage.Empty())
                throw new ArgumentException("輸入影像為空");

            try
            {
                using var processedImage = PreprocessImage(inputImage);
                var predictions = _model.predict(processedImage)[0].ToArray<float>();
                LastResult = AnalyzePredictions(predictions);
                return LastResult;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("紙幣檢測過程中發生錯誤", ex);
            }
        }

        public Tensor PreprocessImage(Mat image)
        {
            using (var resized = new Mat())
            {
                Cv2.CvtColor(image, resized, ColorConversionCodes.BGR2RGB);
                Cv2.Resize(resized, resized, new OpenCvSharp.Size(InputSize, InputSize));
                return resized.ToTensor(); 
            }
        }

        private DetectionResult AnalyzePredictions(float[] predictions)
        {
            if (predictions.Length != _labels.Length)
                throw new InvalidDataException("預測結果與標籤數量不匹配");

            var maxIndex = 0;
            var maxConfidence = predictions[0];

            for (int i = 1; i < predictions.Length; i++)
            {
                if (predictions[i] > maxConfidence)
                {
                    maxIndex = i;
                    maxConfidence = predictions[i];
                }
            }

            return new DetectionResult
            {
                BanknoteType = _labels[maxIndex],
                Confidence = maxConfidence,
                IsValid = maxConfidence >= ConfidenceThreshold
            };
        }

        public void Dispose(bool v)
        {
            if (_disposed) return;

            if (_model is IDisposable disposable)
                disposable.Dispose();

            _disposed = true;
        }

        internal void Dispose()
        {
            throw new NotImplementedException();
        }

        void IDisposable.Dispose()
        {
            Dispose();
        }

        ~DetectionService()
        {
            Dispose(false);
        }
    }

    public class DetectionResult
    {
        public string BanknoteType { get; set; }
        public float Confidence { get; set; }
        public bool IsValid { get; set; }
    }
}