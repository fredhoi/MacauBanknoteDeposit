using OpenCvSharp.ImgHash;
using System.Drawing;
using System.Windows.Forms;

namespace MacauBanknoteDeposit.forms
{
    public partial class BanknoteDepositSystem
    {
        private PictureBox pictureBox;
        private PictureBox backgroundPictureBox;
        private Button btnStart;
        private Button btnConfirm;
        private Button btnCancel;
        private Button btnFinish;
        private Label lblResults;
        private Label lblTotal;
        private Label lblSuccess;
        private ListView lstDeposits;

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.Size = new Size(1280, 720);
            this.Text = "澳門幣存款系統";
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.DoubleBuffered = true;

            lstDeposits = new ListView
            {
                View = View.Details,
                Columns = { "面額", "數量" },
                Size = new Size(250, 190),
                Location = new Point(620, 490),
                Width = 250
            };
            Controls.Add(lstDeposits);

            pictureBox = new PictureBox
            {
                Size = new Size(870, 490),
                Location = new Point(0, 0),
                SizeMode = PictureBoxSizeMode.StretchImage
            };

            backgroundPictureBox = new PictureBox
            {
                Image = Image.FromFile("image/Yuuka.jpg"),
                Location = new Point(870, 0),
                Size = new Size(410, 430),
                SizeMode = PictureBoxSizeMode.StretchImage
            };

            lblResults = new Label
            {
                Location = new Point(10, 500),
                Font = new Font("Microsoft JhengHei", 20, FontStyle.Bold),
                AutoSize = true,
                ForeColor = Color.Blue
            };

            lblTotal = new Label
            {
                Location = new Point(10, 570),
                Text = "當前存款總額：MOP 0",
                Font = new Font("Microsoft JhengHei", 30, FontStyle.Bold),
                ForeColor = Color.DarkGreen,
                AutoSize = true,
            };

            lblSuccess = new Label
            {
                Location = new Point(270, 550),
                AutoSize = true,
                Font = new Font("Microsoft JhengHei", 30, FontStyle.Bold),
                ForeColor = Color.Green,
                Visible = false
            };


            var panel = new Panel
            {
                Size = new Size(400, 270),
                Location = new Point(870, 430),
                BackColor = Color.Azure
            };

            btnConfirm = new Button { Text = "確認", Font = new Font("Microsoft JhengHei", 20, FontStyle.Bold), Size = new Size(170, 100), Location = new Point(20, 20) };
            btnCancel = new Button { Text = "取消", Font = new Font("Microsoft JhengHei", 20, FontStyle.Bold), Size = new Size(170, 100), Location = new Point(210, 20) };
            btnStart = new Button { Text = "驗證紙幣", Font = new Font("Microsoft JhengHei", 20, FontStyle.Bold), Size = new Size(170, 100), Location=new Point(20, 130) };
            btnFinish = new Button { Text = "完成", Font = new Font("Microsoft JhengHei", 20, FontStyle.Bold), Size = new Size(170, 100), Location = new Point(210, 130) };

            panel.Controls.AddRange(new Control[] { btnConfirm, btnCancel, btnFinish, btnStart});
            this.Controls.AddRange(new Control[] { pictureBox, lblResults, lblTotal, lblSuccess, backgroundPictureBox, panel });
            this.ResumeLayout(false);
            this.lstDeposits.Columns[0].Width = 125;
            this.lstDeposits.Columns[1].Width = 125;
            backgroundPictureBox.SendToBack();
        }
    }
}