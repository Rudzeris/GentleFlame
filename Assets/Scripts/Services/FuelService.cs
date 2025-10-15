using Assets.Scripts.Configs;
using Assets.Scripts.Signals;
using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Services
{
    public interface IFuelService
    {
        ReadOnlyReactiveProperty<int> GetFuel(FuelType type);
        void AddFuel(FuelType type, int amount);
        bool ConsumeFuel(FuelType type, int amount);
    }
    public class FuelService : IFuelService
    {
        private readonly Dictionary<FuelType, ReactiveProperty<int>> _fuels = new Dictionary<FuelType, ReactiveProperty<int>>();

        private readonly SignalBus _bus;
        private readonly FuelDatabase _database;
        public FuelService(SignalBus bus, FuelDatabase database)
        {
            _bus = bus;
            _database = database;

            foreach (var config in _database.Fuels)
                _fuels[config.type] = new ReactiveProperty<int>(config.startAmount);
        }

        public void AddFuel(FuelType type, int amount)
        {
            if (amount <= 0) return;
            if (!_fuels.ContainsKey(type)) return;

            var config = _database.Get(type);
            var current = _fuels[type].Value;
            _fuels[type].Value = Math.Min(current + amount, config.maxAmount);

            FireAction();
        }

        public bool ConsumeFuel(FuelType type, int amount)
        {
            if (amount <= 0) return false;
            if (!_fuels.ContainsKey(type)) return false;

            var current = _fuels[type].Value;
            if (current < amount) return false;

            _fuels[type].Value -= amount;
            FireAction();
            return true;
        }

        public ReadOnlyReactiveProperty<int> GetFuel(FuelType type)
        {
            if (_fuels.TryGetValue(type, out var fuel))
                return fuel.ToReadOnlyReactiveProperty();

            Debug.LogError($"Fuel type {type} not found in FuelService");
            return new ReactiveProperty<int>(0).ToReadOnlyReactiveProperty();
        }

        private void FireAction()
        {
            _bus.Fire<FuelChangedSignal>();
        }
    }
}
