using MacauBanknoteDeposit.Model;
using MacauBanknoteDeposit.Services;
using OpenCvSharp;
using System.Windows.Forms;

namespace MacauBanknoteDeposit.forms
{
    public partial class BanknoteDepositSystem : Form
    {
        private readonly CameraService _camera;
        private readonly DetectionService _detectionservice;
        private readonly DepositService _depositservice;
        private readonly ReportGenerator _reportGenerator;

        public BanknoteDepositSystem()
        {
            InitializeComponent();
            _camera = new CameraService();
            _detectionservice = new DetectionService();
            _depositservice = new DepositService();
            _reportGenerator = new ReportGenerator();

            // 確保事件訂閱在初始化之後
            _depositservice.TotalAmountUpdated += OnTotalAmountUpdated;
            _depositservice.DepositUpdated += OnDepositUpdated;

            SetupEventHandlers();
            _camera.StartCapture();
        }

        private void OnTotalAmountUpdated(decimal amount)
        {
            if (lblTotal.InvokeRequired)
            {
                lblTotal.BeginInvoke(new Action(() => lblTotal.Text = $"總額：MOP {amount:#,##0}"));
            }
            else
            {
                lblTotal.Text = $"總額：MOP {amount:#,##0}";
            }
        }

        private void OnDepositUpdated(DepositRecord record)
        {
            // 更新存款明細列表
            if (lstDeposits.InvokeRequired)
            {
                lstDeposits.BeginInvoke(new Action(() =>
                {
                    lstDeposits.Items.Clear();
                    foreach (var entry in record.Counts)
                    {
                        if (entry.Value > 0)
                        {
                            var item = new ListViewItem(entry.Key);
                            item.SubItems.Add(entry.Value.ToString());
                            lstDeposits.Items.Add(item);
                        }
                    }
                }));
            }
            else
            {
                lstDeposits.Items.Clear();
                foreach (var entry in record.Counts)
                {
                    if (entry.Value > 0)
                    {
                        var item = new ListViewItem(entry.Key);
                        item.SubItems.Add(entry.Value.ToString());
                        lstDeposits.Items.Add(item);
                    }
                }
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            _camera?.Dispose();
            _detectionservice?.Dispose();
        }
    }
}