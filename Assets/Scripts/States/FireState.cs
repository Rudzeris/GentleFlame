using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UniRx;

namespace Assets.Scripts.States
{
    public class FireState
    {
        public ReactiveProperty<float> BurnTime { get; } = new ReactiveProperty<float>(0f);
        public ReactiveProperty<float> FuelLevel { get; } = new ReactiveProperty<float>(1f);
        public ReactiveProperty<bool> IsAlive { get; } = new ReactiveProperty<bool>(true);

    }
}
