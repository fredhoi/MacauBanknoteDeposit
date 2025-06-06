using MacauBanknoteDeposit.Model;
using System.Text;


namespace MacauBanknoteDeposit.Services
{
    public class ReportGenerator
    {
        public string GenerateReport(DepositRecord record)
        {
            if (record == null)
                throw new ArgumentNullException(nameof(record), "存款記錄不能為空");

            if (record.Counts == null || !record.Counts.Any())
                return "無有效存款記錄錄";

            var sb = new StringBuilder();
            AppendReportHeader(sb);
            AppendDenominationDetails(sb, record);
            AppendReportFooter(sb, record.Total);
            return sb.ToString();
        }

        public void SaveReport(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                throw new ArgumentException("報告內容不能為空");

            using var dialog = new SaveFileDialog
            {
                Title = "保存存款報告",
                Filter = "文字檔案 (*.txt)|*.txt|所有檔案 (*.*)|*.*",
                FileName = $"存款報告_{DateTime.Now:yyyyMMddHHmmss}.txt",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                OverwritePrompt = true,
                ValidateNames = true
            };

            if (dialog.ShowDialog() != DialogResult.OK) return;

            try
            {
                File.WriteAllText(dialog.FileName, content, Encoding.UTF8);
                ShowSuccessDialog(dialog.FileName);
            }
            catch (Exception ex)
            {
                ShowErrorDialog($"保存失敗：{ex.Message}");
            }
        }

        #region 私有方法
        private void AppendReportHeader(StringBuilder sb)
        {
            sb.AppendLine("澳門幣存款明細報告");
            sb.AppendLine("════════════════════");
            sb.AppendLine($"報告時間：{DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            sb.AppendLine();
            sb.AppendLine("面額\t數量\t小計");
        }

        private void AppendDenominationDetails(StringBuilder sb, DepositRecord record)
        {
            foreach (var (type, count) in record.Counts.OrderByDescending(x => GetDenominationValue(x.Key)))
            {
                if (count <= 0) continue;

                if (int.TryParse(type.Replace("MOP", ""), out var value))
                {
                    sb.AppendLine($"{type}\t{count}\tMOP{value * count:#,##0}");
                }
                else
                {
                    sb.AppendLine($"{type}\t{count}\t(面額解析失敗)");
                }
            }
        }

        private void AppendReportFooter(StringBuilder sb, decimal total)
        {
            sb.AppendLine();
            sb.AppendLine($"存款總額：MOP{total:#,##0}");
            sb.AppendLine("════════════════════");
            sb.AppendLine("氹仔坊眾銀行 存款憑證");
            sb.AppendLine($"簽發時間：{DateTime.Now:yyyy-MM-dd HH:mm}");
        }

        private int GetDenominationValue(string type)
        {
            return type switch
            {
                "MOP1000" => 1000,
                "MOP500" => 500,
                "MOP100" => 100,
                "MOP50" => 50,
                "MOP20" => 20,
                "MOP10" => 10,
                _ => 0
            };
        }

        private void ShowSuccessDialog(string path)
        {
            MessageBox.Show(
                text: $"報告已保存至：\n{path}",
                caption: "保存成功",
                buttons: MessageBoxButtons.OK,
                icon: MessageBoxIcon.Information,
                defaultButton: MessageBoxDefaultButton.Button1,
                options: MessageBoxOptions.ServiceNotification);
        }

        private void ShowErrorDialog(string message)
        {
            MessageBox.Show(
                text: message,
                caption: "錯誤",
                buttons: MessageBoxButtons.OK,
                icon: MessageBoxIcon.Error,
                defaultButton: MessageBoxDefaultButton.Button1,
                options: MessageBoxOptions.RightAlign);
        }

        #endregion
    }
}