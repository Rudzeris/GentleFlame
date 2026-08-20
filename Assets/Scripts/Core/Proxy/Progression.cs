using Assets.Scripts.Core.Data;
using Assets.Scripts.Core.Enums;
using System;
using UniRx;

namespace Assets.Scripts.Core.Proxy
{
    public class Progression : IDisposable
    {
        public readonly ReactiveProperty<int> Level;
        public readonly ReactiveProperty<int> Exp;
        public readonly ReactiveProperty<FireStage> Stage;

        public readonly ProgressionData origin;

        private readonly CompositeDisposable _subscriptions = new CompositeDisposable();

        public Progression(ProgressionData origin)
        {
            this.origin = origin ?? new ProgressionData();

            Level = new ReactiveProperty<int>(this.origin.level);
            Exp = new ReactiveProperty<int>(this.origin.exp);
            Stage = new ReactiveProperty<FireStage>(this.origin.stage);

            Level.Subscribe(v => this.origin.level = v).AddTo(_subscriptions);
            Exp.Subscribe(v => this.origin.exp = v).AddTo(_subscriptions);
            Stage.Subscribe(v => this.origin.stage = v).AddTo(_subscriptions);
        }

        public void Dispose() => _subscriptions.Dispose();
    }
}
