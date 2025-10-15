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
        void AddFuel(FuelType type, int amount);
    }
    public class FireService : IFireService
    {
        private readonly FireState _fireState;
        private readonly FireConfig _config;
        private readonly SignalBus _bus;
        private readonly FuelDatabase _database;
        private CancellationTokenSource _cts;

        public FireState FireState => _fireState;

        public FireService(FireState fireState, SignalBus bus, FireConfig config, FuelDatabase database)
        {
            _fireState = fireState;
            _bus = bus;
            _config = config;
            _database = database;

            _fireState.TimeToExtinguish.Value = config.baseBurnTime;
        }

        public void StartBurning()
        {
            StopBurning();
            _cts = new CancellationTokenSource();
            BurnLoop(_cts.Token).Forget();
            Debug.Log("Fire started");
        }

        public void StopBurning()
        {
            _cts?.Cancel();
            _cts = null;
            _bus.Fire<FireDiedSignal>();
            Debug.Log("Fire stopped");
        }

        private async UniTaskVoid BurnLoop(CancellationToken token)
        {
            while (!token.IsCancellationRequested && _fireState.IsAlive.Value)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: token);

                Tick(1f);

                _bus.Fire<FireFuelChangedSignal>();

                if (_fireState.TimeToExtinguish.Value <= 0f)
                {
                    StopBurning();
                }
            }
        }

        private void Tick(float deltaTime)
        {
            _fireState.TotalBurnTime.Value += deltaTime * _fireState.HeatPower.Value;

            _fireState.TimeToExtinguish.Value -= deltaTime;

            _fireState.HeatPower.Value = Math.Max(1, _fireState.HeatPower.Value);

            _fireState.IsAlive.Value = _fireState.TimeToExtinguish.Value > 0f;
        }

        public void AddFuel(FuelType type, int amount)
        {
            if (!_fireState.IsAlive.Value) return;
            if(_database.Get(type) == null) return;

            _fireState.TimeToExtinguish.Value = Math.Min(_config.maxBurnTime, _fireState.TimeToExtinguish.Value + amount*_database.Get(type).burnTimeSeconds);

            _fireState.HeatPower.Value = Math.Max(_fireState.HeatPower.Value, _database.Get(type).rarity);
        }
    }
}