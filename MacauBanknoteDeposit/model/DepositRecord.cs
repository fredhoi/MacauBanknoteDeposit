using System.Collections.Generic;

namespace MacauBanknoteDeposit.Model
{
    public class DepositRecord
    {
        public IReadOnlyDictionary<string, int> Counts { get; }
        public decimal Total { get; }

        // ���T���c�y���
        public DepositRecord(IDictionary<string, int> counts, decimal total)
        {
            Counts = new Dictionary<string, int>(counts);
            Total = total;
        }
    }
}