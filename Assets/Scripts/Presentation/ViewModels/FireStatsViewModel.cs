using Assets.Scripts.Core.Enums;
using Assets.Scripts.Core.Proxy;
using Assets.Scripts.Core.Services;
using UniRx;

namespace Assets.Scripts.Presentation.ViewModels
{
    public class FireStatsViewModel
    {
        public readonly IReadOnlyReactiveProperty<int> FuelAmount;
        public readonly IReadOnlyReactiveProperty<int> MaxFuelCapacity;
        public readonly IReadOnlyReactiveProperty<float> Temperature;
        public readonly IReadOnlyReactiveProperty<FireBright> Bright;
        public readonly IReadOnlyReactiveProperty<FireMood> Mood;

        private readonly FireService _fireService;
        private readonly StorageService _storageService;

        public FireStatsViewModel(FireStats stats, FireState state,
            FireService fireService, StorageService storageService)
        {
            _fireService = fireService;
            _storageService = storageService;

            FuelAmount = stats.FuelAmount;
            MaxFuelCapacity = stats.MaxFuelCapacity;
            Temperature = stats.Temperature;
            Bright = state.Bright;
            Mood = state.Mood;
        }

        public bool HasSpace => _fireService.IsFreeFuel();

        public int InStorage(FuelType type) => _storageService.Get(type);

        /// <summary>
        /// Переносит топливо со склада в очаг. Списание и добавление идут одной операцией,
        /// чтобы топливо не исчезало при переполненном очаге.
        /// </summary>
        public bool AddFuelFromStorage(FuelType type, int count)
        {
            if (count <= 0 || count > _fireService.FreeSpace)
                return false;

            if (!_storageService.TrySpendFuel(type, count))
                return false;

            if (_fireService.TryAddFuel(type, count))
                return true;

            _storageService.AddFuel(type, count);
            return false;
        }
    }
}
