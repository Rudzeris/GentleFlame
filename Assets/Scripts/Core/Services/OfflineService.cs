using Assets.Scripts.Core.Configs;
using Assets.Scripts.Core.Enums;
using Assets.Scripts.Core.Proxy;
using UnityEngine;

namespace Assets.Scripts.Core.Services
{
    public struct OfflineReport
    {
        /// <summary>Сколько реально просимулировано секунд (уже с учётом капа).</summary>
        public double SimulatedSeconds;

        /// <summary>Сколько игрок отсутствовал на самом деле.</summary>
        public double AwaySeconds;

        public int WarmthEarned;
        public int FuelBurned;

        /// <summary>Игрок упёрся в потолок накопления — повод показать это в интерфейсе.</summary>
        public bool WasCapped;
    }

    /// <summary>
    /// Офлайн-доход (GDD 5.6). Симуляция идёт теми же формулами и тем же шагом,
    /// что и онлайн-тик, поэтому поведение не расходится с игрой.
    /// </summary>
    public class OfflineService
    {
        private const float SimulationStepSeconds = 1f;

        private readonly FireStats _stats;
        private readonly Progression _progression;
        private readonly FireService _fireService;
        private readonly FireState _state;
        private readonly CurrencyService _currency;
        private readonly EconomyConfig _economy;
        private readonly ProgressionConfig _progressionConfig;

        public OfflineService(FireStats stats, FireState state, Progression progression,
            FireService fireService, CurrencyService currency, ConfigService configs)
        {
            _stats = stats;
            _state = state;
            _progression = progression;
            _fireService = fireService;
            _currency = currency;
            _economy = configs.Economy;
            _progressionConfig = configs.Progression;
        }

        public OfflineReport Simulate(double awaySeconds)
        {
            var report = new OfflineReport { AwaySeconds = awaySeconds > 0d ? awaySeconds : 0d };

            if (report.AwaySeconds <= 0d)
                return report;

            var cap = _progressionConfig.GetOfflineCapSeconds(_progression.Stage.Value);
            var simulated = report.AwaySeconds;

            if (cap > 0f && simulated > cap)
            {
                simulated = cap;
                report.WasCapped = true;
            }

            report.SimulatedSeconds = simulated;

            var fuelBefore = _stats.FuelAmount.Value;
            var accumulated = 0d;
            var steps = (int)(simulated / SimulationStepSeconds);

            for (var i = 0; i < steps; i++)
            {
                // Топливо кончилось — очаг ушёл в угли, дальше считать нечего.
                if (_stats.FuelAmount.Value <= 0 && _stats.Temperature.Value <= 0f)
                    break;

                _fireService.Tick(SimulationStepSeconds);

                accumulated += _economy.CalculateWarmthPerSecond(
                    _progression.Stage.Value, _stats.Temperature.Value, _state.Mood.Value)
                    * SimulationStepSeconds;
            }

            var earned = (int)(accumulated * _progressionConfig.offlineRate);

            if (earned > 0)
                _currency.AddCurrency(CurrencyType.Coin, earned);

            report.WarmthEarned = earned;
            report.FuelBurned = Mathf.Max(0, fuelBefore - _stats.FuelAmount.Value);

            return report;
        }
    }
}
