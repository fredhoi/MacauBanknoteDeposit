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
            this.Text = "�D�����s�ڨt��";
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
                Text = "�}�l����",
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
                Text = "��e�s���`�B�GMOP 0"
            };

            var panel = new Panel
            {
                Size = new Size(200, 120),
                Location = new Point(420, 410),
                BackColor = Color.LightGray
            };

            btnConfirm = new Button { Text = "�T�{", Size = new Size(80, 30), Location = new Point(10, 10) };
            btnCancel = new Button { Text = "����", Size = new Size(80, 30), Location = new Point(100, 10) };
            btnFinish = new Button { Text = "����", Size = new Size(180, 30), Location = new Point(10, 50) };

            panel.Controls.AddRange(new Control[] { btnConfirm, btnCancel, btnFinish });
            this.Controls.AddRange(new Control[] { pictureBox, btnStart, lblResults, lblTotal, panel });
            this.ResumeLayout(false);
        }
    }
}