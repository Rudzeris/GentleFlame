using Assets.Scripts.Core.Data;
using Assets.Scripts.Core.Enums;
using UniRx;

namespace Assets.Scripts.Core.Proxy
{
    public class FireState
    {
        public readonly ReactiveProperty<FireBright> Bright;
        public readonly ReactiveProperty<FireMood> Mood;

        public readonly FireStateData Origin;

        public FireState(FireStateData origin)
        {
            Origin = origin;

            Bright = new ReactiveProperty<FireBright>(origin.fireBright);
            Mood = new ReactiveProperty<FireMood>(origin.fireMood);

            Bright.Subscribe(v => origin.fireBright = v);
            Mood.Subscribe(v => origin.fireMood = v);
        }
    }
}
