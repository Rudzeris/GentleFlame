using Assets.Scripts.Core.Data;
using Assets.Scripts.Core.Enums;
using System;
using System.Collections.Generic;
using UniRx;

namespace Assets.Scripts.Core.Proxy
{
    /// <summary>
    /// Склад топлива. Раньше назывался Resources и конфликтовал с UnityEngine.Resources,
    /// из-за чего в коде приходилось писать Proxy.Resources (дефект T-25).
    /// </summary>
    public class Storage : IDisposable
    {
        public readonly ReactiveDictionary<FuelType, Fuel> Fuels;

        public readonly ResourcesData origin;

        private readonly CompositeDisposable _subscriptions = new CompositeDisposable();

        public Storage(ResourcesData origin)
        {
            this.origin = origin ?? new ResourcesData();
            this.origin.fuels ??= new List<FuelData>();

            Fuels = new ReactiveDictionary<FuelType, Fuel>();

            foreach (var data in this.origin.fuels)
            {
                if (data != null)
                    Fuels[data.type] = new Fuel(data);
            }

            Fuels.ObserveAdd().Subscribe(e => Write(e.Value)).AddTo(_subscriptions);
            Fuels.ObserveReplace().Subscribe(e => Write(e.NewValue)).AddTo(_subscriptions);
            Fuels.ObserveRemove().Subscribe(e => Erase(e.Key)).AddTo(_subscriptions);
        }

        /// <summary>Гарантирует наличие ячейки склада под указанный тип топлива.</summary>
        public Fuel EnsureSlot(FuelType type)
        {
            if (Fuels.TryGetValue(type, out var existing))
                return existing;

            var fuel = new Fuel(new FuelData(type, 0));
            Fuels[type] = fuel;
            return fuel;
        }

        private void Write(Fuel fuel)
        {
            if (fuel == null || origin.fuels.Contains(fuel.origin))
                return;

            // Ячейку этого типа могли завести раньше другим объектом — заменяем, а не дублируем.
            origin.fuels.RemoveAll(d => d == null || d.type == fuel.Type);
            origin.fuels.Add(fuel.origin);
        }

        private void Erase(FuelType type)
            => origin.fuels.RemoveAll(d => d == null || d.type == type);

        public void Dispose()
        {
            foreach (var pair in Fuels)
                pair.Value?.Dispose();

            _subscriptions.Dispose();
        }
    }
}
