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

        public int Get(CurrencyType type)
            => _currency.Currencies.TryGetValue(type, out var value) ? value : 0;

        /// <summary>
        /// Прежняя версия отваливалась, когда валюта СУЩЕСТВУЕТ (условие было инвертировано, T-01),
        /// из-за чего начисление не работало никогда.
        /// </summary>
        public bool AddCurrency(CurrencyType type, int amount)
        {
            if (amount < 0)
            {
                Debug.LogError($"CurrencyService: попытка начислить отрицательное количество {type}: {amount}");
                return false;
            }

            if (amount == 0)
                return true;

            _currency.Currencies[type] = Get(type) + amount;
            return true;
        }

        public bool TrySpendCurrency(CurrencyType type, int amount)
        {
            if (amount < 0)
            {
                Debug.LogError($"CurrencyService: попытка списать отрицательное количество {type}: {amount}");
                return false;
            }

            if (amount == 0)
                return true;

            var current = Get(type);

            // Недостаток средств — штатная игровая ситуация, а не ошибка (T-24).
            if (current < amount)
                return false;

            _currency.Currencies[type] = current - amount;
            return true;
        }
    }
}
