using Assets.Scripts.Core.Data;
using Assets.Scripts.Core.Enums;
using System;
using UniRx;

namespace Assets.Scripts.Core.Proxy
{
    /// <summary>
    /// Реактивная обёртка над кошельком игрока. Любое изменение (добавление, замена, удаление)
    /// зеркалится обратно в DTO — иначе значения не попадают в сохранение.
    /// </summary>
    public class Currency : IDisposable
    {
        public readonly ReactiveDictionary<CurrencyType, int> Currencies;

        public readonly CurrencyData origin;

        private readonly CompositeDisposable _subscriptions = new CompositeDisposable();

        public Currency(CurrencyData origin)
        {
            this.origin = origin ?? new CurrencyData();
            this.origin.currencies ??= new System.Collections.Generic.List<CurrencyEntry>();

            Currencies = new ReactiveDictionary<CurrencyType, int>();

            foreach (var entry in this.origin.currencies)
            {
                if (entry != null)
                    Currencies[entry.type] = entry.amount;
            }

            Currencies.ObserveAdd().Subscribe(e => Write(e.Key, e.Value)).AddTo(_subscriptions);
            Currencies.ObserveReplace().Subscribe(e => Write(e.Key, e.NewValue)).AddTo(_subscriptions);
            Currencies.ObserveRemove().Subscribe(e => Erase(e.Key)).AddTo(_subscriptions);
        }

        private void Write(CurrencyType type, int amount)
        {
            var entry = origin.currencies.Find(e => e != null && e.type == type);

            if (entry == null)
                origin.currencies.Add(new CurrencyEntry(type, amount));
            else
                entry.amount = amount;
        }

        private void Erase(CurrencyType type)
            => origin.currencies.RemoveAll(e => e == null || e.type == type);

        public void Dispose() => _subscriptions.Dispose();
    }
}
