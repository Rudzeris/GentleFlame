using UnityEngine;

namespace Assets.Scripts.Configs
{

    [CreateAssetMenu(fileName = "FuelDatabase", menuName = "Databases/FuelDatabase")]
    public class FuelDatabase : ScriptableObject
    {
        public FuelConfig[] Fuels;

        public FuelConfig Get(FuelType type)
        {
            foreach (var fuel in Fuels)
            {
                if (fuel.type == type) return fuel;
            }
            return null;
        }
    }
}