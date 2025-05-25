using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace MacauBanknoteDeposit.Services
{
    public class ReportGenerator
    {
        public string GenerateReport(DepositRecord record)
        {
            var sb = new StringBuilder();
            sb.AppendLine("澳門幣存款明細報告");
            sb.AppendLine("====================");
            sb.AppendLine($"報告時間：{DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            sb.AppendLine();

            sb.AppendLine("面額\t數量\t小計");
            foreach (var (type, count) in record.Counts)
            {
                if (count > 0)
                {
                    var value = int.Parse(type.Replace("MOP", ""));
                    sb.AppendLine($"{type}\t{count}\tMOP{value * count:#,##0}");
                }
            }

            sb.AppendLine();
            sb.AppendLine($"存款總額：MOP{record.Total:#,##0}");
            sb.AppendLine("====================");
            sb.AppendLine("澳門商業銀行 存款憑證");

            return sb.ToString();
        }

        public void SaveReport(string content)
        {
            using (var dialog = new SaveFileDialog())
            {
                dialog.Title = "保存存款報告";
                dialog.Filter = "文字檔案 (*.txt)|*.txt|所有檔案 (*.*)|*.*";
                dialog.FileName = $"存款報告_{DateTime.Now:yyyyMMddHHmmss}.txt";
                dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                dialog.OverwritePrompt = true;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        File.WriteAllText(dialog.FileName, content, Encoding.UTF8);
                        MessageBox.Show($"報告已保存至：{dialog.FileName}",
                                      "保存成功",
                                      MessageBoxButtons.OK,
                                      MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"保存失敗：{ex.Message}",
                                      "錯誤",
                                      MessageBoxButtons.OK,
                                      MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}