using MacauBanknoteDeposit.Services;
using MacauBanknoteDeposit.Model;
using OpenCvSharp;
using System.Windows.Forms;

namespace MacauBanknoteDeposit.forms
{
    public partial class BanknoteDepositSystem : Form
    {
        private readonly CameraService _camera;
        private readonly DetectionService _classifier;
        private readonly DepositService _depositservice;
        private readonly ReportGenerator _reportGenerator;

        public BanknoteDepositSystem()
        {
            InitializeComponent();

            _camera = new CameraService();
            _classifier = new DetectionService();
            _depositservice = new DepositService();
            _reportGenerator = new ReportGenerator();

            SetupEventHandlers();
            _camera.StartCapture();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            _camera?.Dispose();
            _classifier?.Dispose();
        }
    }
}