using Assets.Scripts.Configs;
using Assets.Scripts.Signals;
using Assets.Scripts.States;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Services
{
    public interface IFireService
    {
        FireState FireState { get; }
        void StartBurning();
        void StopBurning();
        void AddFuel(float amount);
    }
    public class FireService : IFireService
    {
        private readonly FireState _fireState;
        private readonly FireConfig _config;
        private readonly SignalBus _bus;
        private CancellationTokenSource _cts;

        public FireState FireState => _fireState;

        public FireService(FireState fireState, SignalBus bus, FireConfig config)
        {
            _fireState = fireState;
            _bus = bus;
            _config = config;
        }

        public void StartBurning()
        {
            StopBurning();
            _cts = new CancellationTokenSource();
            BurnLoop(_cts.Token).Forget();
            Debug.Log("StartBurning");
        }

        public void StopBurning()
        {
            _cts?.Cancel();
            _cts = null;
            _bus.Fire<FireDiedSignal>();
            Debug.Log("EndBurning");
        }

        private async UniTaskVoid BurnLoop(CancellationToken token)
        {
            while(!token.IsCancellationRequested && _fireState.IsAlive.Value)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: token);

                _fireState.BurnTime.Value += 1f;
                _fireState.FuelLevel.Value -= _config.fuelConsumptionPerSecond;
                
                Debug.Log($"time: {_fireState.BurnTime}, fuel: {_fireState.FuelLevel}, alive: {_fireState.IsAlive}");
                _bus.Fire<FireFuelChangedSignal>();

                if (_fireState.FuelLevel.Value <= 0f)
                {
                    _fireState.IsAlive.Value = false;
                    StopBurning();
                }
            }
        }

        public void AddFuel(float amount)
        {
            if (_fireState.IsAlive.Value)
                _fireState.FuelLevel.Value = Math.Min(_config.maxFuel, _fireState.FuelLevel.Value + _config.fuelGainPerWood * amount);
        }
    }
}