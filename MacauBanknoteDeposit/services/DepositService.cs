using System.Collections.Generic;
using System.Linq;

namespace MacauBanknoteDeposit.Services
{
    public class DepositService
    {
        public event System.Action<DepositRecord> DepositUpdated;

        private readonly Dictionary<string, int> _deposits = new Dictionary<string, int>
        {
            {"MOP10", 0}, {"MOP20", 0}, {"MOP50", 0},
            {"MOP100", 0}, {"MOP500", 0}, {"MOP1000", 0}
        };

        public void AddDeposit(string banknoteType)
        {
            if (_deposits.ContainsKey(banknoteType))
            {
                _deposits[banknoteType]++;
                DepositUpdated?.Invoke(GetCurrentRecord());
            }
        }

        public void ResetCurrent()
        {
            DepositUpdated?.Invoke(GetCurrentRecord());
        }

        public void ResetAll()
        {
            foreach (var key in _deposits.Keys.ToList())
                _deposits[key] = 0;
        }

        public DepositRecord GetCurrentRecord() => new DepositRecord(_deposits);

        public int TotalAmount => _deposits.Sum(kv =>
            int.Parse(kv.Key.Replace("MOP", "")) * kv.Value);
    }

    public class DepositRecord
    {
        public IReadOnlyDictionary<string, int> Counts { get; }
        public int Total { get; }

        public DepositRecord(IDictionary<string, int> counts)
        {
            Counts = new Dictionary<string, int>(counts);
            Total = counts.Sum(kv =>
                int.Parse(kv.Key.Replace("MOP", "")) * kv.Value);
        }
    }
}