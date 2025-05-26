namespace MacauBanknoteDeposit.Model
{
    public static class BanknoteValidator
    {
        public static bool Validate(DetectionResult result)
            => result.IsValid && result.Confidence >= 0.8f;
    }
}