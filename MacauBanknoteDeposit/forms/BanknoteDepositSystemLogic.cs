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
                ? $"�˴���G{result.BanknoteType}\n�i�H�סG{result.Confidence:P0}"
                : "�L�k�ѧO";
        }

        private void ConfirmDeposit()
        {
            var result = _detectionservice.LastResult;
            if (BanknoteValidator.IsValid(result))
            {
                _depositservice.AddDeposit(result.BanknoteType);
                lblTotal.Text = $"�`�B�GMOP {_depositservice.TotalAmount:#,##0}";
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