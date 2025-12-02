using Assets.Scripts.Core.Enums;
using Assets.Scripts.Core.Proxy;
using UniRx;

namespace Assets.Scripts.Core.Services
{
    public class StateService
    {
        private readonly FireState _state;
        private readonly FireStats _stats;

        public StateService(FireState state, FireStats stats)
        {
            _state = state;
            _stats = stats;

            SetupStateUpdates();
        }

        private void SetupStateUpdates()
        {
            _stats.Temperature.Subscribe(UpdateFireBrightness);
            _stats.FuelAmount.Subscribe(UpdateFireMood);
        }

        private void UpdateFireBrightness(float temperature)
        {
            _state.Bright.Value = (temperature) switch
            {
                <= 0 => FireBright.Extinguished,
                < 30 => FireBright.AlmostOut,
                < 60 => FireBright.Dim,
                _ => FireBright.Bright
            };
        }

        private void UpdateFireMood(int fuel)
        {
            _state.Mood.Value = fuel switch
            {
                <= 0 => FireMood.Sleep,
                < 5 => FireMood.Sad,
                _ => FireMood.Happy
            };
        }
    }
}
