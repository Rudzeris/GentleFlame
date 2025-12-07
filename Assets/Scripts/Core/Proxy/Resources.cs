using Assets.Scripts.Core.Data;
using Assets.Scripts.Core.Enums;
using UniRx;

namespace Assets.Scripts.Core.Proxy
{
    public class Resources
    {
        public readonly ReactiveDictionary<FuelType, Fuel> Fuels;

        private readonly ResourcesData _origin;

        public Resources(ResourcesData origin)
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
