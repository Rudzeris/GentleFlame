using Assets.Scripts.Core.Proxy;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

namespace Assets.Scripts.Core.Services
{
    public class FireService
    {
        private readonly FireStats _stats;
        private readonly FireState _state;

        private CancellationTokenSource _cts;

        public FireService(FireStats stats, FireState state)
        {
            _stats = stats;
            _state = state;

            _stats.MaxFuelCapacity.Value = 5;

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
            if (_stats.FuelAmount.Value + count > _stats.MaxFuelCapacity.Value)
            {
                Debug.LogError("FuelAmount is fill");
                return false;
            }

            if (_stats.TargetTemperature.Value < temperature)
                _stats.TargetTemperature.Value = temperature;
            else
                _stats.TargetTemperature.Value = (_stats.Temperature.Value * _stats.FuelAmount.Value + count * temperature) / (_stats.FuelAmount.Value + count);

            _stats.FuelAmount.Value += count;

            return true;
        }

        public bool IsFreeFuel() => _stats.FuelAmount.Value < _stats.MaxFuelCapacity.Value;

        public bool TrySpendFuel(int count)
        {
            if (_stats.FuelAmount.Value <= 0)
                return false;

            _stats.FuelAmount.Value -= count;
            return true;
        }

        public void Tick(float deltaTime)
        {
            if (_stats.FuelAmount.Value > 0)
            {
                _stats.BurnTimeLeft.Value -= deltaTime;

                if (_stats.BurnTimeLeft.Value <= 0)
                {
                    _stats.FuelAmount.Value -= 1;
                    _stats.BurnTimeLeft.Value = _stats.FuelAmount.Value > 0 ? GetFuelBurnTime() : 0;
                }
            }
            else
                _stats.TargetTemperature.Value = 0;


            _stats.Temperature.Value = UnityEngine.Mathf.Lerp(
                    _stats.Temperature.Value,
                    _stats.TargetTemperature.Value,
                    deltaTime * 0.3f);
        }

        private float GetFuelBurnTime() => 5f;
    }
}
