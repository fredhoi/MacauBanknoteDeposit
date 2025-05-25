using System;
using MacauBanknoteDeposit.Model;
using MacauBanknoteDeposit.Services;

namespace MacauBanknoteDeposit.forms
{
    public partial class BanknoteDepositSystem
    {
        private void DetectBanknote()
        {
            var frame = _camera.GetCurrentFrame();
            if (frame == null) return;

            var result = _detectionservice.Detect(frame);
            lblResults.Text = result.IsValid
                ? $"檢測到：{result.BanknoteType}\n可信度：{result.Confidence:P0}"
                : "無法識別";
        }

        private void ConfirmDeposit()
        {
            var result = _detectionservice.LastResult;
            if (BanknoteValidator.IsValid(result))
            {
                _depositservice.AddDeposit(result.BanknoteType);
                lblTotal.Text = $"總額：MOP {_depositservice.TotalAmount:#,##0}";
            }
        }

        private void CancelDeposit() => _depositservice.ResetCurrent();

        private void GenerateReport()
        {
            var report = _reportGenerator.GenerateReport(_depositservice.GetCurrentRecord());
            _reportGenerator.SaveReport(report);
            _depositservice.ResetAll();
        }
    }
}