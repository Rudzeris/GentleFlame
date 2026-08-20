using Assets.Scripts.Core.Configs;
using Assets.Scripts.Core.Enums;
using Assets.Scripts.Core.Proxy;
using System;
using UniRx;

namespace Assets.Scripts.Core.Services
{
    /// <summary>Производные состояния огня: яркость от температуры, настроение от заполнения и тепла.</summary>
    public class StateService : IDisposable
    {
        private readonly FireState _state;
        private readonly FireStats _stats;
        private readonly EconomyConfig _economy;

        private readonly CompositeDisposable _subscriptions = new CompositeDisposable();

        public StateService(FireState state, FireStats stats, ConfigService configs)
        {
            _state = state;
            _stats = stats;
            _economy = configs.Economy;

            _stats.Temperature.Subscribe(_ => Refresh()).AddTo(_subscriptions);
            _stats.FuelAmount.Subscribe(_ => Refresh()).AddTo(_subscriptions);
            _stats.MaxFuelCapacity.Subscribe(_ => Refresh()).AddTo(_subscriptions);
        }

        /// <summary>Принудительно пересчитывает производные состояния (вызывается при старте игры).</summary>
        public void Initialize() => Refresh();

        private void Refresh()
        {
            UpdateBrightness(_stats.Temperature.Value);
            UpdateMood(_stats.FuelAmount.Value, _stats.Temperature.Value);
        }

        private void UpdateBrightness(float temperature)
        {
            if (temperature <= 0f)
                _state.Bright.Value = FireBright.Extinguished;
            else if (temperature < _economy.brightAlmostOutThreshold)
                _state.Bright.Value = FireBright.AlmostOut;
            else if (temperature < _economy.brightDimThreshold)
                _state.Bright.Value = FireBright.Dim;
            else
                _state.Bright.Value = FireBright.Bright;
        }

        private void UpdateMood(int fuel, float temperature)
        {
            if (fuel <= 0)
            {
                _state.Mood.Value = FireMood.Sleep;
                return;
            }

            var capacity = _stats.MaxFuelCapacity.Value;
            var ratio = capacity > 0 ? (float)fuel / capacity : 0f;

            if (ratio >= _economy.moodInspiresFuelRatio && temperature >= _economy.moodInspiresTemperature)
                _state.Mood.Value = FireMood.Inspires;
            else if (ratio < _economy.moodSadFuelRatio || temperature < _economy.moodHappyTemperature)
                _state.Mood.Value = FireMood.Sad;
            else
                _state.Mood.Value = FireMood.Happy;
        }

        public void Dispose() => _subscriptions.Dispose();
    }
}
