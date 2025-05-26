using OpenCvSharp;
using System.Diagnostics;
using System.Windows.Forms;

namespace MacauBanknoteDeposit.Services
{
    public class CameraService : IDisposable
    {
        private VideoCapture _capture;
        private System.Windows.Forms.Timer _timer;
        public event Action<Mat> FrameUpdated;
        private bool _disposed;

        public void StartCapture()
        {
            if (!IsCameraAvailable(0))
                throw new InvalidOperationException("未檢測到可用相機");

            _capture = new VideoCapture(0);
            _capture.Set(VideoCaptureProperties.FrameWidth, 640);
            _capture.Set(VideoCaptureProperties.FrameHeight, 480);

            _timer = new System.Windows.Forms.Timer { Interval = 30 };
            _timer.Tick += (s, e) =>
            {
                try
                {
                    using (var frame = new Mat())
                    {
                        if (_capture != null && _capture.IsOpened())
                        {
                            if (_capture.Read(frame) && !frame.Empty())
                            {
                                FrameUpdated?.Invoke(frame.Clone());
                            }
                            else
                            {
                                // 可記錄錯誤或嘗試重啟相機
                                Debug.WriteLine("幀讀取失敗");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"相機錯誤: {ex.Message}");
                }
            };
            _timer.Start();
        }

        private bool IsCameraAvailable(int index)
        {
            using (var testCapture = new VideoCapture(index))
            {
                return testCapture.IsOpened();
            }
        }

        public Mat GetCurrentFrame() => _capture?.RetrieveMat();

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
                _timer?.Stop();
                _timer?.Dispose();
                _capture?.Release();
                _capture?.Dispose();
            }
            _disposed = true;
        }
    }
}