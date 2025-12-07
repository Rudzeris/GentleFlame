using Assets.Scripts.Core.Enums;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Core.Configs
{
    [CreateAssetMenu(menuName = "Configs/Fuel/FuelsConfig")]
    public class FuelsConfig : ScriptableObject
    {
        [SerializeField] private List<FuelConfig> fuels;
        public float baseBurnTime;      // Сколько будет гореть одна единица топлива

        public FuelConfig GetFuel(FuelType type) => fuels.FirstOrDefault(f => f.type == type);

        private void OnValidate()
        {
            var dupricates = fuels
                .GroupBy(f => f?.type)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key);

            if (dupricates.Count() > 0)
            {
                string text = "FuelsConfig: найден дубликат. |";

                foreach (var fuel in dupricates)
                    text += fuel;

                text += "|";
            }
        }
    }
}
