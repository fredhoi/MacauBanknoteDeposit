namespace MacauBanknoteDeposit.Model
{
    public static class BanknoteValidator
    {
        public static bool IsValid(DetectionResult result)
            => !(!result.IsValid || result.Confidence < 0.8f);

        internal static bool IsValid(Services.DetectionResult result)
        {
            throw new NotImplementedException();
        }
    }
}