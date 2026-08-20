using Assets.Scripts.Core.Enums;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Core.Configs
{
    [CreateAssetMenu(menuName = "GentleFlame/Configs/Fuel/FuelsConfig")]
    public class FuelsConfig : ScriptableObject
    {
        [SerializeField] private List<FuelConfig> fuels = new List<FuelConfig>();

        [Tooltip("Глобальный множитель времени горения. Ручка для тюнинга темпа игры.")]
        public float baseBurnTimeMultiplier = 1f;

        private Dictionary<FuelType, FuelConfig> _byType;

        public IReadOnlyList<FuelConfig> All => fuels;

        /// <summary>Заполнение списка из редакторного генератора конфигов и из тестов.</summary>
        public void SetFuels(IEnumerable<FuelConfig> source)
        {
            fuels = new List<FuelConfig>(source);
            _byType = null;
        }

        /// <summary>Поиск по типу. Кэшируется: раньше был LINQ-перебор на каждый вызов из тика (T-17).</summary>
        public FuelConfig GetFuel(FuelType type)
        {
            if (_byType == null)
                BuildCache();

            return _byType.TryGetValue(type, out var config) ? config : null;
        }

        private void BuildCache()
        {
            _byType = new Dictionary<FuelType, FuelConfig>();

            foreach (var fuel in fuels)
            {
                if (fuel == null || _byType.ContainsKey(fuel.type))
                    continue;

                _byType[fuel.type] = fuel;
            }
        }

        private void OnValidate()
        {
            _byType = null;

            var seen = new HashSet<FuelType>();
            var duplicates = new List<FuelType>();

            foreach (var fuel in fuels)
            {
                if (fuel == null)
                    continue;

                if (!seen.Add(fuel.type))
                    duplicates.Add(fuel.type);
            }

            if (duplicates.Count > 0)
                Debug.LogError($"FuelsConfig: найдены дубликаты типов топлива: {string.Join(", ", duplicates)}");
        }
    }
}
