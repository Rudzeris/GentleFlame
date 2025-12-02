using Assets.Scripts.Core.Data;
using Assets.Scripts.Core.Enums;
using UniRx;

namespace Assets.Scripts.Core.Proxy
{
    public class Fuel
    {
        public FuelType Type => origin.type;
        public readonly ReactiveProperty<int> Amount;

        public readonly FuelData origin;

        public Fuel(FuelData origin)
        {
            this.origin = origin;
            Amount = new ReactiveProperty<int>(origin.amount);

            Amount.Subscribe(amount => this.origin.amount = amount);
        }
    }
}
