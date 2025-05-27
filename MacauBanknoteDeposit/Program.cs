using MacauBanknoteDeposit.forms;

namespace MacauBanknoteDeposit
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            // 初始化
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                Application.Run(new BanknoteDepositSystem());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"應用程序啟動失敗: {ex.Message}",
                              "嚴重錯誤",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Error);
            }
        }
    }
}
