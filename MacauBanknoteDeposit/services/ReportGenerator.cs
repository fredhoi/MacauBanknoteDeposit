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
            sb.AppendLine("�D�����s�ک��ӳ��i");
            sb.AppendLine("====================");
            sb.AppendLine($"���i�ɶ��G{DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            sb.AppendLine();

            sb.AppendLine("���B\t�ƶq\t�p�p");
            foreach (var (type, count) in record.Counts)
            {
                if (count > 0)
                {
                    var value = int.Parse(type.Replace("MOP", ""));
                    sb.AppendLine($"{type}\t{count}\tMOP{value * count:#,##0}");
                }
            }

            sb.AppendLine();
            sb.AppendLine($"�s���`�B�GMOP{record.Total:#,##0}");
            sb.AppendLine("====================");
            sb.AppendLine("�D���ӷ~�Ȧ� �s�ھ���");

            return sb.ToString();
        }

        public void SaveReport(string content)
        {
            using (var dialog = new SaveFileDialog())
            {
                dialog.Title = "�O�s�s�ڳ��i";
                dialog.Filter = "��r�ɮ� (*.txt)|*.txt|�Ҧ��ɮ� (*.*)|*.*";
                dialog.FileName = $"�s�ڳ��i_{DateTime.Now:yyyyMMddHHmmss}.txt";
                dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                dialog.OverwritePrompt = true;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        File.WriteAllText(dialog.FileName, content, Encoding.UTF8);
                        MessageBox.Show($"���i�w�O�s�ܡG{dialog.FileName}",
                                      "�O�s���\",
                                      MessageBoxButtons.OK,
                                      MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"�O�s���ѡG{ex.Message}",
                                      "���~",
                                      MessageBoxButtons.OK,
                                      MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}