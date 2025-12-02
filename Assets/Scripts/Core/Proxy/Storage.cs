using Assets.Scripts.Core.Data;
using Assets.Scripts.Core.Enums;
using UniRx;

namespace Assets.Scripts.Core.Proxy
{
    public class Storage
    {
        public readonly ReactiveDictionary<FuelType, Fuel> Fuels;

        private readonly StorageData _origin;

        public Storage(StorageData origin)
        {
            _origin = origin;

            Fuels = new ReactiveDictionary<FuelType, Fuel>();

            foreach (var pair in origin.fuels)
                Fuels[pair.Key] = new Fuel(pair.Value);

            Fuels.ObserveAdd().Subscribe(pair => _origin.fuels[pair.Key] = pair.Value.origin);
            Fuels.ObserveRemove().Subscribe(pair => _origin.fuels.Remove(pair.Key));
        }
    }
}
