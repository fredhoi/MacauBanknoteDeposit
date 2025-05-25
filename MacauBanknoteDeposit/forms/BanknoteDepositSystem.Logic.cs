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
                ? $"�˴���G{result.BanknoteType}\n�i�H�סG{result.Confidence:P0}"
                : "�L�k�ѧO";
        }

        private void ConfirmDeposit()
        {
            var result = _classifier.LastResult;
            if (BanknoteValidator.IsValid(result))
            {
                _depositservice.AddDeposit(result.BanknoteType);
                lblTotal.Text = $"�`�B�GMOP {_depositservice.TotalAmount:#,##0}";
            }
        }

        private void CancelDeposit() => _depositservice.ResetCurrent();

        private void GenerateReport()
        {
            var report = _reportGenerator.Generate(_depositservice.GetRecords());
            _reportGenerator.Save(report);
            _depositservice.ResetAll();
            lblTotal.Text = "��e�s���`�B�GMOP 0";
        }
    }
}