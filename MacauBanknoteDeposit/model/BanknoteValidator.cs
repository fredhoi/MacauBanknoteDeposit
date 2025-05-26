namespace MacauBanknoteDeposit.Model
{
    public static class BanknoteValidator
    {
        // �w�q�X�k���B�C��
        private static readonly HashSet<string> ValidDenominations = new HashSet<string>
        {
            "MOP10", "MOP20", "MOP50", "MOP100", "MOP500", "MOP1000"
        };

        public static bool Validate(DetectionResult result)
        {
            // ���m���ˬd�G���G���ũΥ��q�L��¦����
            if (result == null || !result.IsValid)
                return false;

            // �ˬd�m�H���H��
            if (result.Confidence < 0.8f)
                return false;

            // �ˬd�O�_���X�k���B
            return ValidDenominations.Contains(result.BanknoteType?.Trim() ?? "");
        }
    }
}