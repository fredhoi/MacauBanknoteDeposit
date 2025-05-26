namespace MacauBanknoteDeposit.Model
{
    public static class BanknoteValidator
    {
        // 定義合法面額列表
        private static readonly HashSet<string> ValidDenominations = new HashSet<string>
        {
            "MOP10", "MOP20", "MOP50", "MOP100", "MOP500", "MOP1000"
        };

        public static bool Validate(DetectionResult result)
        {
            // 防禦性檢查：結果為空或未通過基礎驗證
            if (result == null || !result.IsValid)
                return false;

            // 檢查置信度閾值
            if (result.Confidence < 0.8f)
                return false;

            // 檢查是否為合法面額
            return ValidDenominations.Contains(result.BanknoteType?.Trim() ?? "");
        }
    }
}