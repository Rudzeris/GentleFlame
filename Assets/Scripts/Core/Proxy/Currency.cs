using Assets.Scripts.Core.Data;
using Assets.Scripts.Core.Enums;
using UniRx;

namespace Assets.Scripts.Core.Proxy
{
    public class Currency
    {
        public readonly ReactiveDictionary<CurrencyType, int> Currencies;

        private readonly CurrencyData _origin;

        public Currency(CurrencyData origin)
        {
            _origin = origin;

            Currencies = new ReactiveDictionary<CurrencyType, int>();

            foreach (var pair in origin.currencies)
                Currencies[pair.Key] = pair.Value;

            Currencies.ObserveAdd().Subscribe(pair => _origin.currencies[pair.Key] = pair.Value);
            Currencies.ObserveRemove().Subscribe(pair => _origin.currencies.Remove(pair.Key));
        }
    }
}
