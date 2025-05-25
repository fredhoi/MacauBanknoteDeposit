using System;

namespace MacauBanknoteDeposit.forms
{
    public partial class BanknoteDepositSystem
    {
        private void DetectBanknote()
        {
            var frame = _camera.GetCurrentFrame();
            if (frame == null) return;

            var result = _classifier.Detect(frame);
            lblResults.Text = result.IsValid
                ? $"檢測到：{result.BanknoteType}\n可信度：{result.Confidence:P0}"
                : "無法識別";
        }

        private void ConfirmDeposit()
        {
            var result = _classifier.LastResult;
            if (BanknoteValidator.IsValid(result))
            {
                _depositservice.AddDeposit(result.BanknoteType);
                lblTotal.Text = $"總額：MOP {_depositservice.TotalAmount:#,##0}";
            }
        }

        private void CancelDeposit() => _depositservice.ResetCurrent();

        private void GenerateReport()
        {
            var report = _reportGenerator.Generate(_depositservice.GetRecords());
            _reportGenerator.Save(report);
            _depositservice.ResetAll();
            lblTotal.Text = "當前存款總額：MOP 0";
        }
    }
}