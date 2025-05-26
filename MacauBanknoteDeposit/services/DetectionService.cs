using MacauBanknoteDeposit.Extensions;
using MacauBanknoteDeposit.Model;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms.Onnx;
using Microsoft.ML.OnnxRuntime;
using OpenCvSharp;
using System;
using System.IO;
using System.Linq;

namespace MacauBanknoteDeposit.Services
{
    public record DetectionService(DetectionResult LastResult, float ConfidenceThreshold) : IDisposable
    {
        private const int InputSize = 224;
        private ITransformer? _onnxModel;
        private MLContext _mlContext;
        private PredictionEngine<ModelInput, ModelOutput> _predictionEngine;
        private string[] _labels;
        private bool _disposed;
        public DetectionResult LastResult { get; private set; } = new DetectionResult();
        public float ConfidenceThreshold { get; } = 0.8f;
        public DetectionService() : this(default, default)
        {
            LoadModel();
            LoadLabels();
            WarmUpModel();
        }

        // 定義輸入輸出數據結構
        public class ModelInput
        {
            [ColumnName("sequential_17_input")]
            [VectorType(224 * 224 * 3)]
            public float[] Input { get; set; }
        }

        public class ModelOutput
        {
            [ColumnName("sequential_19")]
            public float[] Output { get; set; }
        }
        private void LoadModel()
        {
            var modelPath = GetModelPath("model.onnx");

            // 使用 ONNX Runtime 驗證輸入形狀
            using var session = new InferenceSession(modelPath);
            var inputMetadata = session.InputMetadata.First();
            var inputShape = inputMetadata.Value.Dimensions;
            ValidateInputShape(inputShape);

            // 初始化 ML.NET Pipeline
            _mlContext = new MLContext();
            var pipeline = _mlContext.Transforms.ApplyOnnxModel(
                modelFile: modelPath,
                outputColumnNames: new[] { "sequential_19" },
                inputColumnNames: new[] { "sequential_17_input" }
            );

            // 擬合模型
            var data = new[] { new ModelInput { Input = new float[InputSize * InputSize * 3] } };
            var dataView = _mlContext.Data.LoadFromEnumerable(data);
            _onnxModel = pipeline.Fit(dataView);

            InitializePredictor();
        }

        // 預測引擎初始化
        private void InitializePredictor()
        {
            _predictionEngine = _mlContext.Model.CreatePredictionEngine<ModelInput, ModelOutput>(_onnxModel);
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
        private void WarmUpModel()
        {
            try
            {
                // **使用空白数据预热**
                var dummyInput = new ModelInput
                {
                    Input = new float[InputSize * InputSize * 3]
                };
                _predictionEngine.Predict(dummyInput);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("模型预热失败", ex);
            }
        }

        private string GetModelPath(string filename)
            => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Model", filename);

        private void ValidateInputShape(int[] inputShape)
        {
            if (inputShape.Length != 4 ||
                inputShape[1] != InputSize ||  // 高度應為 224
                inputShape[2] != InputSize ||  // 寬度應為 224
                inputShape[3] != 3)            // 通道數應為 3
            {
                throw new InvalidOperationException(
                    $"模型輸入形狀應為 [批次, 224, 224, 3] (NHWC 格式)，實際為 [{string.Join(",", inputShape)}]");
            }
        }

        public DetectionResult Detect(Mat inputImage)
        {
            if (inputImage.Empty())
                throw new ArgumentException("輸入圖像為空");

            try
            {
                var processedData = PreprocessImage(inputImage);
                var input = new ModelInput { Input = processedData };
                var prediction = _predictionEngine.Predict(input);
                var result = AnalyzePredictions(prediction.Output);

                // 更新 LastResult
                LastResult = result;
                return result;
            }
            catch (Exception ex)
            {
                LastResult = new DetectionResult { IsValid = false };
                throw new InvalidOperationException("紙幣檢測失敗", ex);
            }

        }

        private float[] PreprocessImage(Mat image)
        {
            // Step 1: 調整尺寸和顏色空間（BGR → RGB）
            using var resized = new Mat();
            Cv2.CvtColor(image, resized, ColorConversionCodes.BGR2RGB);
            Cv2.Resize(resized, resized, new OpenCvSharp.Size(InputSize, InputSize));

            // Step 2: 歸一化到 [0, 1] 並轉換為 float[]
            using var floatMat = new Mat();
            resized.ConvertTo(floatMat, MatType.CV_32FC3, 1.0 / 255.0);

            // Step 3: 直接提取 HWC 格式數據
            var hwcData = new float[InputSize * InputSize * 3];
            unsafe
            {
                var ptr = (float*)floatMat.Data;
                for (int i = 0; i < hwcData.Length; i++)
                {
                    hwcData[i] = ptr[i];
                }
            }

            return hwcData;
        }

        private DetectionResult AnalyzePredictions(float[] predictions)
        {
            if (predictions.Length != _labels.Length)
                throw new InvalidDataException("預測結果與標籤數量不匹配");

            var maxIndex = Array.IndexOf(predictions, predictions.Max());
            var maxConfidence = predictions[maxIndex];

            return new DetectionResult
            {
                BanknoteType = _labels[maxIndex],
                Confidence = maxConfidence,
                IsValid = maxConfidence >= ConfidenceThreshold
            };
        }


        public void Dispose()
        {
            if (_disposed) return;

            // **释放ML.NET相关资源**
            _predictionEngine?.Dispose();
            _onnxModel = null;

            _disposed = true;
            GC.SuppressFinalize(this);
        }
    }
}