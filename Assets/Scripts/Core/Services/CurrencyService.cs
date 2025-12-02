using Assets.Scripts.Core.Enums;
using Assets.Scripts.Core.Proxy;
using UnityEngine;

namespace Assets.Scripts.Core.Services
{
    public class CurrencyService
    {
        private readonly Currency _currency;

        public CurrencyService(Currency currency)
        {
            _currency = currency;
        }

        public bool AddCurrency(CurrencyType type, int amount)
        {
            if (amount < 0 || _currency.Currencies.ContainsKey(type))
            {
                Debug.LogError($"Добавляете отрицательный {type} или его не существует");
                return false;
            }

            _currency.Currencies[type] += amount;

            return true;
        }

        public bool TrySpendCurrency(CurrencyType type, int amount)
        {
            if (amount < 0 || _currency.Currencies.ContainsKey(type))
            {
                Debug.LogError($"Добавляете отрицательный {type} или его не существует");
                return false;
            }

            if (_currency.Currencies[type] - amount < 0)
            {
                Debug.LogWarning($"Недостаточно {type}");
                return false;
            }

            _currency.Currencies[type] -= amount;

            return true;
        }
    }
}
