using System.Windows.Forms;

namespace MacauBanknoteDeposit
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new forms.BanknoteDepositSystem());
        }
    }
}