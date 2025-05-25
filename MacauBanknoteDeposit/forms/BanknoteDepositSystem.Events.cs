using System;
using OpenCvSharp;

namespace MacauBanknoteDeposit.forms
{
    public partial class BanknoteDepositSystem
    {
        private void SetupEventHandlers()
        {
            btnStart.Click += (s, e) => DetectBanknote();
            btnConfirm.Click += (s, e) => ConfirmDeposit();
            btnCancel.Click += (s, e) => CancelDeposit();
            btnFinish.Click += (s, e) => GenerateReport();

            _camera.FrameUpdated += frame =>
            {
                if (pictureBox.InvokeRequired)
                {
                    pictureBox.BeginInvoke(new Action(() => UpdatePreview(frame)));
                }
                else
                {
                    UpdatePreview(frame);
                }
            };
        }

        private void UpdatePreview(Mat frame)
        {
            using (var bitmap = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(frame))
            {
                pictureBox.Image?.Dispose();
                pictureBox.Image = (Bitmap)bitmap.Clone();
            }
        }
    }
}