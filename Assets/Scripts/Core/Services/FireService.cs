using Assets.Scripts.Core.Proxy;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

namespace Assets.Scripts.Core.Services
{
    public class FireService
    {
        public FireStats Stats { get; }
        public FireState State { get; }

        private CancellationTokenSource _cts;

        public FireService(FireStats stats, FireState state)
        {
            Stats = stats;
            State = state;

            _cts = new CancellationTokenSource();
            RunTicker(_cts.Token).Forget();
        }

        private async UniTaskVoid RunTicker(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                Tick(1f);
                await UniTask.Delay(1000, cancellationToken: token);
            }
        }

        public bool AddFuel(int count, float temperature)
        {
            if (Stats.FuelAmount.Value + count > Stats.MaxFuelCapacity.Value)
            {
                Debug.LogError("FuelAmount is fill");
                return false;
            }

            if (Stats.TargetTemperature.Value < temperature)
                Stats.TargetTemperature.Value = temperature;
            else
                Stats.TargetTemperature.Value = (Stats.Temperature.Value * Stats.FuelAmount.Value + count * temperature) / (Stats.FuelAmount.Value + count);

            Stats.FuelAmount.Value += count;

            return true;
        }

        public bool IsFreeFuel() => Stats.FuelAmount.Value < Stats.MaxFuelCapacity.Value;

        public bool TrySpendFuel(int count)
        {
            if (Stats.FuelAmount.Value <= 0)
                return false;

            Stats.FuelAmount.Value -= count;
            return true;
        }

        public void Tick(float deltaTime)
        {
            if (Stats.FuelAmount.Value > 0)
            {
                Stats.BurnTimeLeft.Value -= deltaTime;

                if (Stats.BurnTimeLeft.Value <= 0)
                {
                    Stats.FuelAmount.Value -= 1;
                    Stats.BurnTimeLeft.Value = Stats.FuelAmount.Value > 0 ? GetFuelBurnTime() : 0;
                }
            }
            else
                Stats.TargetTemperature.Value = 0;


            Stats.Temperature.Value = UnityEngine.Mathf.Lerp(
                    Stats.Temperature.Value,
                    Stats.TargetTemperature.Value,
                    deltaTime * 0.3f);
        }

        private float GetFuelBurnTime() => 5f;
    }
}
