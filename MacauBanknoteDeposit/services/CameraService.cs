using OpenCvSharp;
using System.Windows.Forms;

namespace MacauBanknoteDeposit.Services
{
    public class CameraService : IDisposable
    {
        private VideoCapture _capture;
        private System.Windows.Forms.Timer _timer;
        public event Action<Mat> FrameUpdated;

        public void StartCapture()
        {
            _capture = new VideoCapture(0);
            _capture.Set(VideoCaptureProperties.FrameWidth, 640);
            _capture.Set(VideoCaptureProperties.FrameHeight, 480);

            _timer = new System.Windows.Forms.Timer { Interval = 30 };
            _timer.Tick += (s, e) =>
            {
                using (var frame = new Mat())
                {
                    if (_capture.Read(frame) && !frame.Empty())
                    {
                        FrameUpdated?.Invoke(frame.Clone());
                    }
                }
            };
            _timer.Start();
        }

        public Mat GetCurrentFrame() => _capture?.RetrieveMat();

        public void Dispose(bool v)
        {
            _timer?.Stop();
            _capture?.Release();
        }

        internal void Dispose()
        {
            throw new NotImplementedException();
        }

        void IDisposable.Dispose()
        {
            Dispose();
        }
    }
}