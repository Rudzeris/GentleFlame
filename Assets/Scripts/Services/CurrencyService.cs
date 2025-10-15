using Assets.Scripts.Configs;
using Assets.Scripts.Signals;
using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Services
{
    public interface ICurrencyService
    {
        ReadOnlyReactiveProperty<int> GetCurrency(CurrencyType type);
        void AddCurrency(CurrencyType type, int amount);
        bool SpendCurrency(CurrencyType type, int amount);
    }
    public class CurrencyService : ICurrencyService
    {
        private readonly Dictionary<CurrencyType, ReactiveProperty<int>> _currencies = new Dictionary<CurrencyType, ReactiveProperty<int>>();

        private readonly SignalBus _bus;
        private readonly CurrencyDatabase _database;
        public CurrencyService(SignalBus bus, CurrencyDatabase database)
        {
            _bus = bus;
            _database = database;

            foreach (var config in _database.Currencies)
                _currencies[config.type] = new ReactiveProperty<int>(config.startAmount);
        }
        private void FireAction()
        {
            _bus.Fire<CurrencyChangedSignal>();
        }

        public ReadOnlyReactiveProperty<int> GetCurrency(CurrencyType type)
        {
            if (_currencies.TryGetValue(type, out var currency))
                return currency.ToReadOnlyReactiveProperty();

            Debug.LogError($"Currency type {type} not found in CurrencyService");
            return new ReactiveProperty<int>(0).ToReadOnlyReactiveProperty();
        }

        public void AddCurrency(CurrencyType type, int amount)
        {
            if (amount <= 0) return;
            if (!_currencies.ContainsKey(type)) return;

            var config = _database.Get(type);
            var current = _currencies[type].Value;
            _currencies[type].Value = Math.Min(current + amount, config.maxAmount);

            FireAction();
        }

        public bool SpendCurrency(CurrencyType type, int amount)
        {
            if (amount <= 0) return false;
            if (!_currencies.ContainsKey(type)) return false;

            var current = _currencies[type].Value;
            if (current < amount) return false;

            _currencies[type].Value -= amount;
            FireAction();
            return true;
        }
    }
}
