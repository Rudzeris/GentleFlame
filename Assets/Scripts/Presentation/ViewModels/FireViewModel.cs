using Assets.Scripts.Core.Data;
using Assets.Scripts.Core.Services;
using UniRx;

namespace Assets.Scripts.Presentation.ViewModels
{
    public class FireViewModel
    {
        public IReadOnlyReactiveProperty<int> FuelAmount => _fireService.Stats.FuelAmount;
        public IReadOnlyReactiveProperty<float> Temperature => _fireService.Stats.Temperature;
        public IReadOnlyReactiveProperty<FireBright> Bright => _fireService.State.Bright;
        public IReadOnlyReactiveProperty<FireMood> Mood => _fireService.State.Mood;

        private readonly FireService _fireService;

        public FireViewModel(FireService fireService)
        {
            _fireService = fireService;
        }

        public bool AddFuel(int count, float temperature)
        {
            return _fireService.AddFuel(count, temperature);
        }

        public bool TrySpendFuel(int count)
        {
            return _fireService.TrySpendFuel(count);
        }
    }
}
