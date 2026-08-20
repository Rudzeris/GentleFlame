using Assets.Scripts.Core.Data;
using Assets.Scripts.Core.Enums;
using System;
using System.Collections.Generic;
using UniRx;

namespace Assets.Scripts.Core.Proxy
{
    /// <summary>
    /// Состояние очага. Hearth — состав горящего топлива; FuelAmount выводится из него
    /// и больше не является независимым полем, которое можно рассогласовать.
    /// </summary>
    public class FireStats : IDisposable
    {
        public readonly ReactiveProperty<int> MaxFuelCapacity;
        public readonly ReactiveProperty<float> Temperature;
        public readonly ReactiveProperty<float> TargetTemperature;
        public readonly ReactiveProperty<float> BurnTimeLeft;

        /// <summary>Состав очага: тип топлива → количество единиц.</summary>
        public readonly ReactiveDictionary<FuelType, int> Hearth;

        /// <summary>Суммарное количество единиц в очаге. Производное от Hearth.</summary>
        public readonly ReactiveProperty<int> FuelAmount;

        public readonly FireStatsData origin;

        private readonly CompositeDisposable _subscriptions = new CompositeDisposable();

        public FireStats(FireStatsData origin)
        {
            this.origin = origin ?? new FireStatsData();
            this.origin.hearth ??= new List<HearthSlotData>();

            MaxFuelCapacity = new ReactiveProperty<int>(this.origin.maxFuelCapacity);
            Temperature = new ReactiveProperty<float>(this.origin.temperature);
            BurnTimeLeft = new ReactiveProperty<float>(this.origin.burnTimeLeft);
            TargetTemperature = new ReactiveProperty<float>(0f);

            Hearth = new ReactiveDictionary<FuelType, int>();

            foreach (var slot in this.origin.hearth)
            {
                if (slot != null && slot.count > 0)
                    Hearth[slot.type] = slot.count;
            }

            FuelAmount = new ReactiveProperty<int>(SumHearth());

            MaxFuelCapacity.Subscribe(v => this.origin.maxFuelCapacity = v).AddTo(_subscriptions);
            Temperature.Subscribe(v => this.origin.temperature = v).AddTo(_subscriptions);
            BurnTimeLeft.Subscribe(v => this.origin.burnTimeLeft = v).AddTo(_subscriptions);

            Hearth.ObserveAdd().Subscribe(e => OnHearthChanged(e.Key, e.Value)).AddTo(_subscriptions);
            Hearth.ObserveReplace().Subscribe(e => OnHearthChanged(e.Key, e.NewValue)).AddTo(_subscriptions);
            Hearth.ObserveRemove().Subscribe(e => OnHearthRemoved(e.Key)).AddTo(_subscriptions);
        }

        private int SumHearth()
        {
            var total = 0;

            foreach (var pair in Hearth)
                total += pair.Value;

            return total;
        }

        private void OnHearthChanged(FuelType type, int count)
        {
            var slot = origin.hearth.Find(s => s != null && s.type == type);

            if (slot == null)
                origin.hearth.Add(new HearthSlotData(type, count));
            else
                slot.count = count;

            FuelAmount.Value = SumHearth();
        }

        private void OnHearthRemoved(FuelType type)
        {
            origin.hearth.RemoveAll(s => s == null || s.type == type);
            FuelAmount.Value = SumHearth();
        }

        public void Dispose() => _subscriptions.Dispose();
    }
}
