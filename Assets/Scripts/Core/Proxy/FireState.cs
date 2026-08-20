using Assets.Scripts.Core.Data;
using Assets.Scripts.Core.Enums;
using System;
using UniRx;

namespace Assets.Scripts.Core.Proxy
{
    public class FireState : IDisposable
    {
        public readonly ReactiveProperty<FireBright> Bright;
        public readonly ReactiveProperty<FireMood> Mood;

        public readonly FireStateData origin;

        private readonly CompositeDisposable _subscriptions = new CompositeDisposable();

        public FireState(FireStateData origin)
        {
            this.origin = origin ?? new FireStateData();

            Bright = new ReactiveProperty<FireBright>(this.origin.fireBright);
            Mood = new ReactiveProperty<FireMood>(this.origin.fireMood);

            Bright.Subscribe(v => this.origin.fireBright = v).AddTo(_subscriptions);
            Mood.Subscribe(v => this.origin.fireMood = v).AddTo(_subscriptions);
        }

        public void Dispose() => _subscriptions.Dispose();
    }
}
