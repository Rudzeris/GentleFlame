using UniRx;

namespace Assets.Scripts.States
{
    public class FireState
    {
        public ReactiveProperty<float> TotalBurnTime { get; } = new ReactiveProperty<float>(0f);
        public ReactiveProperty<float> TimeToExtinguish { get; } = new ReactiveProperty<float>(30);
        public ReactiveProperty<int> HeatPower { get; } = new ReactiveProperty<int>(1);
        public ReactiveProperty<bool> IsAlive { get; } = new ReactiveProperty<bool>(true);

    }
}
