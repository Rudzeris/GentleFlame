using Assets.Scripts.Core.Proxy;
using Assets.Scripts.Core.Services;
using UniRx;

namespace Assets.Scripts.Presentation.ViewModels
{
    public class FireStatsViewModel
    {
        public readonly IReadOnlyReactiveProperty<int> FuelAmount;
        public readonly IReadOnlyReactiveProperty<float> Temperature;

        private readonly FireService _fireService;

        public FireStatsViewModel(FireStats stats, FireService fireService)
        {
            _fireService = fireService;

            FuelAmount = stats.FuelAmount;
            Temperature = stats.Temperature;
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
