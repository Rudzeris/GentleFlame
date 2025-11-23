using Assets.Scripts.Core.Data;
using UniRx;

namespace Assets.Scripts.Core.Proxy
{
    public class Progression
    {
        public readonly ReactiveProperty<int> Level;
        public readonly ReactiveProperty<int> Exp;
        public readonly ReactiveProperty<FireStage> Stage;

        public readonly ProgressionData Origin;

        public Progression(ProgressionData origin)
        {
            Origin = origin;

            Level = new ReactiveProperty<int>(origin.level);
            Exp = new ReactiveProperty<int>(origin.exp);
            Stage = new ReactiveProperty<FireStage>(origin.stage);

            Level.Subscribe(v => origin.level = v);
            Exp.Subscribe(v => origin.exp = v);
            Stage.Subscribe(v => origin.stage = v);
        }
    }
}
