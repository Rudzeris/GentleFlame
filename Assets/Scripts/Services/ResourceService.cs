using Assets.Scripts.Configs;
using Assets.Scripts.Signals;
using System;
using TMPro;
using UniRx;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Services
{
    public interface IResourceService
    {
        ReadOnlyReactiveProperty<int> Wood { get; }
        void AddWood(int amount);
        bool ConsumeWood(int amount);
    }
    public class ResourceService : IResourceService
    {
        private ReactiveProperty<int> _wood;
        public ReadOnlyReactiveProperty<int> Wood => _wood.ToReadOnlyReactiveProperty();
        private readonly SignalBus _bus;
        private readonly ResourceConfig _config;
        public ResourceService(SignalBus bus, ResourceConfig config)
        {
            _bus = bus;
            _config = config;
            Debug.Log($"config? {config != null}");
            _wood = new ReactiveProperty<int>(_config.startAmount);
            Debug.Log($"wood? {Wood != null}");
        }
        private void ResourceChanged()
        {
            _bus.Fire<ResourceChangedSignal>();
        }
        public void AddWood(int amount)
        {
            if (amount <= 0) return;
            _wood.Value = Math.Min(_wood.Value + amount, _config.maxAmount);
            ResourceChanged();
        }
        public bool ConsumeWood(int amount)
        {
            if (amount <= 0) return false;
            if (_wood.Value < amount) return false;
            _wood.Value -= amount;
            ResourceChanged();
            return true;
        }
    }
}
