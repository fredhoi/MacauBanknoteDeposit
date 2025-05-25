namespace MacauBanknoteDeposit.Model
{
    public class DetectionResult
    {
        public string BanknoteType { get; set; }
        public float Confidence { get; set; }
        public bool IsValid { get; set; }
    }
}