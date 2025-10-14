using Assets.Scripts.Signals;
using UniRx;
using Zenject;

namespace Assets.Scripts.Services
{
    public interface ICurrencyService
    {
        ReadOnlyReactiveProperty<int> Coins { get; }
        ReadOnlyReactiveProperty<int> Crystals { get; }
        void AddCoins(int amount);
        bool SpendCoins(int amount);
        void AddCrystals(int amount);
        bool SpendCrystals(int amount);
    }
    public class CurrencyService : ICurrencyService
    {
        private ReactiveProperty<int> _coins = new ReactiveProperty<int>(0);
        private ReactiveProperty<int> _crystals = new ReactiveProperty<int>(0);
        public ReadOnlyReactiveProperty<int> Coins => _coins.ToReadOnlyReactiveProperty();
        public ReadOnlyReactiveProperty<int> Crystals => _crystals.ToReadOnlyReactiveProperty();
        private SignalBus _bus;
        public CurrencyService(SignalBus bus)
        {
            _bus = bus;
        }
        private void CurrencyChanged()
        {
            _bus.Fire<CurrencyChangedSignal>();
        }
        public void AddCoins(int amount)
        {
            if (amount <= 0) return;
            _coins.Value += amount;
            CurrencyChanged();
        }
        public bool SpendCoins(int amount)
        {
            if (amount <= 0) return false;
            if (_coins.Value < amount) return false;
            _coins.Value -= amount;
            CurrencyChanged();
            return true;
        }
        public void AddCrystals(int amount)
        {
            if (amount <= 0) return;
            _crystals.Value += amount;
            CurrencyChanged();
        }
        public bool SpendCrystals(int amount)
        {
            if (amount <= 0) return false;
            if (_crystals.Value < amount) return false;
            _crystals.Value -= amount;
            CurrencyChanged();
            return true;
        }
    }
}
