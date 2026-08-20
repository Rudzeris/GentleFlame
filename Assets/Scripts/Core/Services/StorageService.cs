using Assets.Scripts.Core.Enums;
using Assets.Scripts.Core.Proxy;
using UnityEngine;

namespace Assets.Scripts.Core.Services
{
    public class StorageService
    {
        private readonly Storage _storage;

        public StorageService(Storage storage)
        {
            _storage = storage;
        }

        public int Get(FuelType type)
            => _storage.Fuels.TryGetValue(type, out var fuel) ? fuel.Amount.Value : 0;

        /// <summary>Инвертированное условие приводило к тому, что склад не работал вовсе (T-02).</summary>
        public bool AddFuel(FuelType type, int amount)
        {
            if (amount < 0)
            {
                Debug.LogError($"StorageService: попытка добавить отрицательное количество {type}: {amount}");
                return false;
            }

            if (amount == 0)
                return true;

            var slot = _storage.EnsureSlot(type);
            slot.Amount.Value += amount;
            return true;
        }

        public bool TrySpendFuel(FuelType type, int amount)
        {
            if (amount < 0)
            {
                Debug.LogError($"StorageService: попытка списать отрицательное количество {type}: {amount}");
                return false;
            }

            if (amount == 0)
                return true;

            if (!_storage.Fuels.TryGetValue(type, out var fuel) || fuel.Amount.Value < amount)
                return false;

            fuel.Amount.Value -= amount;
            return true;
        }
    }
}
