using Assets.Scripts.Core.Configs;
using Assets.Scripts.Core.Enums;
using Assets.Scripts.Core.Proxy;
using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Assets.Scripts.Core.Services
{
    /// <summary>
    /// Физика очага: приём топлива, догорание, температура с инерцией.
    /// Ничего не запускает сам — получает время от GameTickService.
    /// </summary>
    public class FireService : IGameTickListener, IDisposable
    {
        private readonly FireStats _stats;
        private readonly FuelsConfig _fuels;
        private readonly EconomyConfig _economy;

        private readonly Subject<FuelType> _fuelBurned = new Subject<FuelType>();

        /// <summary>Одна единица топлива догорела. На это подписаны прогрессия и аналитика.</summary>
        public IObservable<FuelType> FuelBurned => _fuelBurned;

        public FireService(FireStats stats, ConfigService configs)
        {
            _stats = stats;
            _fuels = configs.Fuels;
            _economy = configs.Economy;

            RecalculateTargetTemperature();
        }

        public int FreeSpace => Mathf.Max(0, _stats.MaxFuelCapacity.Value - _stats.FuelAmount.Value);

        public bool IsFreeFuel() => FreeSpace > 0;

        /// <summary>Положить топливо в очаг. Возвращает false, если места нет или тип неизвестен.</summary>
        public bool TryAddFuel(FuelType type, int count)
        {
            if (count <= 0)
                return false;

            if (_fuels.GetFuel(type) == null)
            {
                Debug.LogError($"FireService: неизвестный тип топлива {type} — нет FuelConfig");
                return false;
            }

            if (count > FreeSpace)
                return false;

            var current = _stats.Hearth.TryGetValue(type, out var existing) ? existing : 0;
            _stats.Hearth[type] = current + count;

            RecalculateTargetTemperature();

            if (_stats.BurnTimeLeft.Value <= 0f)
                _stats.BurnTimeLeft.Value = GetBurnTimeOfNextUnit();

            return true;
        }

        /// <summary>Забрать топливо из очага. Больше не уводит количество в минус (T-09).</summary>
        public bool TrySpendFuel(int count)
        {
            if (count <= 0 || count > _stats.FuelAmount.Value)
                return false;

            for (var i = 0; i < count; i++)
                ConsumeOneUnit();

            RecalculateTargetTemperature();
            return true;
        }

        public void Tick(float deltaTime)
        {
            if (deltaTime <= 0f)
                return;

            BurnDown(deltaTime);
            RecalculateTargetTemperature();
            ApplyThermalInertia(deltaTime);
        }

        private void BurnDown(float deltaTime)
        {
            if (_stats.FuelAmount.Value <= 0)
            {
                _stats.BurnTimeLeft.Value = 0f;
                return;
            }

            var remaining = _stats.BurnTimeLeft.Value - deltaTime;

            // Цикл, а не if: при большом deltaTime (лаг, офлайн-догон) за один тик
            // может догореть несколько единиц.
            while (remaining <= 0f && _stats.FuelAmount.Value > 0)
            {
                var burned = ConsumeOneUnit();

                if (burned.HasValue)
                    _fuelBurned.OnNext(burned.Value);

                if (_stats.FuelAmount.Value <= 0)
                {
                    remaining = 0f;
                    break;
                }

                remaining += GetBurnTimeOfNextUnit();
            }

            _stats.BurnTimeLeft.Value = remaining > 0f ? remaining : 0f;
        }

        /// <summary>
        /// Сгорает самое холодное топливо: растопка прогорает раньше поленьев.
        /// Детерминировано, поэтому воспроизводимо в тестах и в офлайн-симуляции.
        /// </summary>
        private FuelType? ConsumeOneUnit()
        {
            var type = GetNextUnitType();

            if (!type.HasValue)
                return null;

            var left = _stats.Hearth[type.Value] - 1;

            if (left > 0)
                _stats.Hearth[type.Value] = left;
            else
                _stats.Hearth.Remove(type.Value);

            return type;
        }

        private FuelType? GetNextUnitType()
        {
            FuelType? coldest = null;
            var coldestTemperature = float.MaxValue;

            foreach (var pair in _stats.Hearth)
            {
                if (pair.Value <= 0)
                    continue;

                var config = _fuels.GetFuel(pair.Key);
                var temperature = config != null ? config.temperature : 0f;

                if (temperature < coldestTemperature)
                {
                    coldestTemperature = temperature;
                    coldest = pair.Key;
                }
            }

            return coldest;
        }

        private float GetBurnTimeOfNextUnit()
        {
            var type = GetNextUnitType();

            if (!type.HasValue)
                return 0f;

            var config = _fuels.GetFuel(type.Value);

            if (config == null)
                return 0f;

            var multiplier = _fuels.baseBurnTimeMultiplier <= 0f ? 1f : _fuels.baseBurnTimeMultiplier;
            return config.burnTimeSeconds * multiplier;
        }

        /// <summary>
        /// Целевая температура — средневзвешенное по составу очага (GDD 5.3).
        /// Прежняя реализация смешивала фактическую температуру с новой и позволяла
        /// одной соломинке «перебить» полный очаг углей (T-08).
        /// </summary>
        private void RecalculateTargetTemperature()
        {
            var units = 0;
            var weighted = 0f;

            foreach (var pair in _stats.Hearth)
            {
                if (pair.Value <= 0)
                    continue;

                var config = _fuels.GetFuel(pair.Key);

                if (config == null)
                    continue;

                weighted += config.temperature * pair.Value;
                units += pair.Value;
            }

            _stats.TargetTemperature.Value = units > 0 ? weighted / units : 0f;
        }

        private void ApplyThermalInertia(float deltaTime)
        {
            var current = _stats.Temperature.Value;
            var target = _stats.TargetTemperature.Value;

            var rate = target > current ? _economy.heatingRate : _economy.coolingRate;
            var next = Mathf.Lerp(current, target, rate * deltaTime);

            _stats.Temperature.Value = next < 0f ? 0f : next;
        }

        public void Dispose() => _fuelBurned.Dispose();
    }
}
