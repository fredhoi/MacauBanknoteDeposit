namespace MacauBanknoteDeposit.Model
{
    public static class BanknoteValidator
    {
        public static bool IsValid(DetectionResult result)
            => result.IsValid && result.Confidence >= 0.8f;
    }

    public class DetectionResult
    {
        public string BanknoteType { get; set; }
        public float Confidence { get; set; }
        public bool IsValid { get; set; }
    }
}