using Assets.Scripts.Core.Configs;
using Assets.Scripts.Core.Enums;
using Assets.Scripts.Core.Proxy;

namespace Assets.Scripts.Core.Services
{
    /// <summary>
    /// Начисление Тепла за горение (GDD 5.3). Дробный доход копится в аккумуляторе
    /// и переводится в валюту целыми единицами, иначе при малых ставках игрок не получал бы ничего.
    /// </summary>
    public class WarmthService : IGameTickListener
    {
        private readonly FireStats _stats;
        private readonly FireState _state;
        private readonly Progression _progression;
        private readonly CurrencyService _currency;
        private readonly EconomyConfig _economy;

        private float _accumulator;

        public WarmthService(FireStats stats, FireState state, Progression progression,
            CurrencyService currency, ConfigService configs)
        {
            _stats = stats;
            _state = state;
            _progression = progression;
            _currency = currency;
            _economy = configs.Economy;
        }

        public float WarmthPerSecond => _economy.CalculateWarmthPerSecond(
            _progression.Stage.Value, _stats.Temperature.Value, _state.Mood.Value);

        public void Tick(float deltaTime)
        {
            if (deltaTime <= 0f)
                return;

            _accumulator += WarmthPerSecond * deltaTime;

            if (_accumulator < 1f)
                return;

            var whole = (int)_accumulator;
            _accumulator -= whole;

            _currency.AddCurrency(CurrencyType.Coin, whole);
        }
    }
}
