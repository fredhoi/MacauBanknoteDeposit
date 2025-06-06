using MacauBanknoteDeposit.Model;
using System.Collections.Concurrent;

namespace MacauBanknoteDeposit.Services
{
    public class DepositService
    {
        public event Action<DepositRecord> DepositUpdated;
        public event Action<decimal> TotalAmountUpdated;


        private readonly Dictionary<string, int> _deposits = new Dictionary<string, int>
        {
            {"MOP10", 0}, {"MOP20", 0}, {"MOP50", 0},
            {"MOP100", 0}, {"MOP500", 0}, {"MOP1000", 0}
        };

        private decimal _totalAmount;
        private readonly ConcurrentDictionary<string, int> _depositCounts = new ConcurrentDictionary<string, int>();

        public decimal TotalAmount
        {
            get => _totalAmount;
            private set
            {
                _totalAmount = value;
                TotalAmountUpdated?.Invoke(value); // 觸發事件
            }
        }

        public DepositService()
        {
            // 初始化默認面額計數
            var denominations = new[] { "MOP10", "MOP20", "MOP50", "MOP100", "MOP500", "MOP1000" };
            foreach (var denom in denominations)
            {
                _depositCounts.TryAdd(denom, 0);
            }
        }

        public DepositRecord GetCurrentRecord()
        {
            return new DepositRecord(
                 counts: new Dictionary<string, int>(_depositCounts), // 傳遞副本
                 total: TotalAmount
             );
        }

        public void AddDeposit(string banknoteType)
        {
            // 使用線程安全方法更新
            _depositCounts.AddOrUpdate(banknoteType, 1, (key, oldValue) => oldValue + 1);
            TotalAmount += GetDenominationValue(banknoteType);
            DepositUpdated?.Invoke(GetCurrentRecord());
        }

        public void ResetCurrent()
        {
            // 計算本次存款總額
            decimal currentAmount = 0m;
            foreach (var kvp in _depositCounts)
            {
                currentAmount += GetDenominationValue(kvp.Key) * kvp.Value;
            }

            // 從總額扣除本次存款金額
            TotalAmount -= currentAmount;

            // 重置當前計數
            foreach (var key in _depositCounts.Keys.ToList())
                _depositCounts[key] = 0;

            // 觸發事件，更新UI
            DepositUpdated?.Invoke(GetCurrentRecord());
            TotalAmountUpdated?.Invoke(TotalAmount);
        }

        public void ResetAll()
        {
            // 完全重置所有數據
            foreach (var key in _depositCounts.Keys.ToList())
                _depositCounts[key] = 0;
            TotalAmount = 0;
            DepositUpdated?.Invoke(GetCurrentRecord());
        }
        private decimal GetDenominationValue(string type)
        {
            return type switch
            {
                "MOP10" => 10,
                "MOP20" => 20,
                "MOP50" => 50,
                "MOP100" => 100,
                "MOP500" => 500,
                "MOP1000" => 1000,
                _ => throw new ArgumentException("無效面額")
            };
        }
    }
}