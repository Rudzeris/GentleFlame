using Assets.Scripts.Core.Enums;
using UnityEngine;

namespace Assets.Scripts.Core.Configs
{
    /// <summary>Параметры одного вида топлива. Значения по умолчанию соответствуют GDD 5.2.</summary>
    [CreateAssetMenu(menuName = "GentleFlame/Configs/Fuel/FuelConfig")]
    public class FuelConfig : ScriptableObject
    {
        public FuelType type;

        [Tooltip("Температура единицы топлива, 0..100")]
        public float temperature = 20f;

        [Tooltip("Базовое время горения одной единицы, секунды")]
        public float burnTimeSeconds = 30f;

        [Tooltip("Цена одной единицы в Тепле")]
        public int priceInWarmth = 2;

        [Tooltip("Сколько единиц добавляется за одно действие игрока")]
        public int unitPerClick = 1;

        [Tooltip("Стартовое количество на складе у нового игрока")]
        public int initialAmount;
    }
}
