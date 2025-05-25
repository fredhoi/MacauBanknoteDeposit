using System.Drawing;
using System.Windows.Forms;

namespace MacauBanknoteDeposit.forms
{
    public partial class BanknoteDepositSystem
    {
        private PictureBox pictureBox;
        private Button btnStart;
        private Button btnConfirm;
        private Button btnCancel;
        private Button btnFinish;
        private Label lblResults;
        private Label lblTotal;

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.Size = new Size(640, 550);
            this.Text = "澳門幣存款系統";
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.DoubleBuffered = true;

            pictureBox = new PictureBox
            {
                Size = new Size(640, 400),
                Location = new Point(0, 0),
                SizeMode = PictureBoxSizeMode.StretchImage
            };

            btnStart = new Button
            {
                Text = "開始驗證",
                Size = new Size(100, 30),
                Location = new Point(270, 410)
            };

            lblResults = new Label
            {
                Location = new Point(10, 410),
                AutoSize = true,
                ForeColor = Color.Blue
            };

            lblTotal = new Label
            {
                Location = new Point(10, 450),
                Text = "當前存款總額：MOP 0"
            };

            var panel = new Panel
            {
                Size = new Size(200, 120),
                Location = new Point(420, 410),
                BackColor = Color.LightGray
            };

            btnConfirm = new Button { Text = "確認", Size = new Size(80, 30), Location = new Point(10, 10) };
            btnCancel = new Button { Text = "取消", Size = new Size(80, 30), Location = new Point(100, 10) };
            btnFinish = new Button { Text = "完成", Size = new Size(180, 30), Location = new Point(10, 50) };

            panel.Controls.AddRange(new Control[] { btnConfirm, btnCancel, btnFinish });
            this.Controls.AddRange(new Control[] { pictureBox, btnStart, lblResults, lblTotal, panel });
            this.ResumeLayout(false);
        }
    }
}