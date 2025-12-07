using Assets.Scripts.Core.Data;
using Assets.Scripts.Core.Enums;
using UniRx;

namespace Assets.Scripts.Core.Proxy
{
    public class Currency
    {
        public readonly ReactiveDictionary<CurrencyType, int> Currencies;

        public readonly CurrencyData origin;

        public Currency(CurrencyData origin)
        {
            this.origin = origin;

            Currencies = new ReactiveDictionary<CurrencyType, int>();

            foreach (var pair in origin.currencies)
                Currencies[pair.Key] = pair.Value;

            Currencies.ObserveAdd().Subscribe(pair => this.origin.currencies[pair.Key] = pair.Value);
            Currencies.ObserveRemove().Subscribe(pair => this.origin.currencies.Remove(pair.Key));
        }
    }
}
