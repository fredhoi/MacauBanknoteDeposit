using System;
using OpenCvSharp;
using OpenCvSharp.Extensions;

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
            if (pictureBox.InvokeRequired)
            {
                pictureBox.BeginInvoke(new Action(() => UpdatePreview(frame)));
                return;
            }

            try
            {
                using (var bitmap = BitmapConverter.ToBitmap(frame))
                {
                    // 保留上一幀引用避免立即回收
                    var oldImage = pictureBox.Image;
                    pictureBox.Image = (Bitmap)bitmap.Clone();
                    oldImage?.Dispose();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"畫面更新錯誤: {ex.Message}");
            }
        }
    }
}