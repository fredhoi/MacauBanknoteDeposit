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
            if (_detectionservice?.LastResult == null || !_detectionservice.LastResult.IsValid)
            {
                MessageBox.Show("請先完成有效的紙幣檢測", "操作中止",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // 防禦性檢查所有依賴對象
                if (_detectionservice == null || _depositservice == null)
                {
                    MessageBox.Show("系統服務未初始化", "嚴重錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (!_detectionservice.LastResult.IsValid)
                {
                    MessageBox.Show("請先進行紙幣驗證", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var result = _detectionservice.LastResult;
                if (BanknoteValidator.Validate(result))
                {
                    _depositservice.AddDeposit(result.BanknoteType);
                }
                else
                {
                    MessageBox.Show($"驗證失敗：{result.BanknoteType} 不符合要求", "錯誤",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"操作失敗：{ex.Message}", "錯誤",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CancelDeposit()
        {
            try
            {
                if (_depositservice == null)
                {
                    MessageBox.Show("存款服務未初始化", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                _depositservice.ResetCurrent();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"取消存款失敗：{ex.Message}", "錯誤",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void GenerateReport()
        {
            var report = _reportGenerator.GenerateReport(_depositservice.GetCurrentRecord());
            _reportGenerator.SaveReport(report);
            _depositservice.ResetAll();
        }
    }
}