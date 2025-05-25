using System;
using System.IO;
using Tensorflow;
using Tensorflow.Keras.Engine;
using OpenCvSharp;
using static Tensorflow.Binding;

namespace MacauBanknoteDeposit.Services
{
    public class DetectionService : IDisposable
    {
        private const int InputSize = 224;
        private const float ConfidenceThreshold = 0.8f;

        private IModel _model;
        private string[] _labels;
        private bool _disposed;

        public DetectionResult LastResult { get; private set; }

        public DetectionService()
        {
            LoadModel();
            LoadLabels();
            WarmUpModel();
        }

  
        private void LoadModel()
        {
            var modelPath = GetModelPath("keras_model.h5");

            if (!File.Exists(modelPath))
                throw new FileNotFoundException($"模型文件不存在：{modelPath}");

            try
            {
                _model = keras.models.load_model(modelPath);

                if (_model.inputs[0].shape[1] != InputSize ||
                    _model.inputs[0].shape[2] != InputSize)
                {
                    throw new InvalidOperationException("模型输入尺寸不匹配，预期 224x224");
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("模型加载失败，请检查模型格式和版本兼容性", ex);
            }
        }

        private void LoadLabels()
        {
            var labelPath = GetModelPath("labels.txt");

            if (!File.Exists(labelPath))
                throw new FileNotFoundException($"标签文件不存在：{labelPath}");

            _labels = File.ReadAllLines(labelPath);

            if (_labels.Length != 6 ||
                !_labels[0].StartsWith("MOP") ||
                !_labels[^1].StartsWith("MOP"))
            {
                throw new FormatException("标签文件格式错误，应包含6个以MOP开头的面额");
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
                throw new InvalidOperationException("模型预热失败，请检查模型兼容性", ex);
            }
        }

        public DetectionResult Detect(Mat inputImage)
        {
            if (inputImage.Empty())
                throw new ArgumentException("输入图像为空");

            try
            {
                using var processedImage = PreprocessImage(inputImage);
                var predictions = _model.predict(processedImage)[0].ToArray<float>();
                LastResult = AnalyzePredictions(predictions);
                return LastResult;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("纸币检测过程中发生错误", ex);
            }
        }

        private Tensor PreprocessImage(Mat image)
        {
            using var rgbImage = new Mat();
            using var resizedImage = new Mat();

            Cv2.CvtColor(image, rgbImage, ColorConversionCodes.BGR2RGB);

            Cv2.Resize(rgbImage, resizedImage, new OpenCvSharp.Size(InputSize, InputSize));
            var tensor = resizedImage.ToTensor()
                .astype(TF_DataType.TF_FLOAT) / 255.0f;
            return tf.expand_dims(tensor, 0);
        }

        private DetectionResult AnalyzePredictions(float[] predictions)
        {
            if (predictions.Length != _labels.Length)
                throw new InvalidDataException("预测结果与标签数量不匹配");

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

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                _model?.Dispose();
            }

            _disposed = true;
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