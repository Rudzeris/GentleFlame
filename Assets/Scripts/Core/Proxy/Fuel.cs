using Assets.Scripts.Core.Data;
using Assets.Scripts.Core.Enums;
using System;
using UniRx;

namespace Assets.Scripts.Core.Proxy
{
    public class Fuel : IDisposable
    {
        public FuelType Type => origin.type;
        public readonly ReactiveProperty<int> Amount;

        public readonly FuelData origin;

        private readonly CompositeDisposable _subscriptions = new CompositeDisposable();

        public Fuel(FuelData origin)
        {
            this.origin = origin;
            Amount = new ReactiveProperty<int>(origin.amount);

            Amount.Subscribe(amount => this.origin.amount = amount).AddTo(_subscriptions);
        }

        public void Dispose() => _subscriptions.Dispose();
    }
}
