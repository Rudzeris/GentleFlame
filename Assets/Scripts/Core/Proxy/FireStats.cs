using Assets.Scripts.Core.Data;
using UniRx;

namespace Assets.Scripts.Core.Proxy
{
    public class FireStats
    {
        public readonly ReactiveProperty<int> MaxFuelCapacity;
        public readonly ReactiveProperty<int> FuelAmount;
        public readonly ReactiveProperty<float> Temperature;

        public readonly FireStatsData Origin;
        public FireStats(FireStatsData origin)
        {
            Origin = origin;
            MaxFuelCapacity = new ReactiveProperty<int>(origin.maxFuelCapacity);
            FuelAmount = new ReactiveProperty<int>(origin.fuelAmount);
            Temperature = new ReactiveProperty<float>(origin.temperature);

            MaxFuelCapacity.Subscribe(number => origin.maxFuelCapacity = number);
            FuelAmount.Subscribe(number => origin.fuelAmount = number);
            Temperature.Subscribe(number => origin.temperature = number);
        }
    }
}
