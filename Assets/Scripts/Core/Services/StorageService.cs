using Assets.Scripts.Core.Enums;
using Assets.Scripts.Core.Proxy;
using UnityEngine;

namespace Assets.Scripts.Core.Services
{
    public class StorageService
    {
        private readonly Proxy.Resources _storage;

        public StorageService(Proxy.Resources storage)
        {
            _storage = storage;
        }

        public bool AddFuel(FuelType type, int amount)
        {
            if (amount < 0 || _storage.Fuels.ContainsKey(type))
            {
                Debug.LogError($"Добавляете отрицательный {type} или его не существует");
                return false;
            }

            _storage.Fuels[type].Amount.Value += amount;

            return true;
        }

        public bool TrySpendFuel(FuelType type, int amount)
        {
            if (amount < 0 || _storage.Fuels.ContainsKey(type))
            {
                Debug.LogError($"Добавляете отрицательный {type} или его не существует");
                return false;
            }

            if (_storage.Fuels[type].Amount.Value - amount < 0)
            {
                Debug.LogWarning($"Недостаточно {type}");
                return false;
            }

            _storage.Fuels[type].Amount.Value -= amount;

            return true;
        }
    }
}
