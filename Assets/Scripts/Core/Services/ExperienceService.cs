using Assets.Scripts.Core.Configs;
using Assets.Scripts.Core.Proxy;
using System;
using UniRx;

namespace Assets.Scripts.Core.Services
{
    /// <summary>
    /// Связывает горение с прогрессией: каждая догоревшая единица топлива даёт опыт,
    /// умноженный на настроение огня (GDD 5.3). Без этого сервиса событие FuelBurned
    /// никто не слушал и уровень не рос.
    /// </summary>
    public class ExperienceService : IDisposable
    {
        private readonly FireService _fire;
        private readonly ProgressService _progress;
        private readonly FireState _state;
        private readonly EconomyConfig _economy;

        private readonly CompositeDisposable _subscriptions = new CompositeDisposable();

        private float _accumulator;

        public ExperienceService(FireService fire, ProgressService progress, FireState state, ConfigService configs)
        {
            _fire = fire;
            _progress = progress;
            _state = state;
            _economy = configs.Economy;
        }

        /// <summary>Подписка живёт отдельно от конструктора — сервис не начинает работать сам по себе.</summary>
        public void Initialize()
            => _fire.FuelBurned.Subscribe(_ => OnFuelBurned()).AddTo(_subscriptions);

        private void OnFuelBurned()
        {
            _accumulator += _economy.GetMoodMultiplier(_state.Mood.Value);

            if (_accumulator < 1f)
                return;

            var whole = (int)_accumulator;
            _accumulator -= whole;

            _progress.AddExp(whole);
        }

        public void Dispose() => _subscriptions.Dispose();
    }
}
