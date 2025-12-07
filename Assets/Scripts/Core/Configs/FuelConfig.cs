using Assets.Scripts.Core.Enums;
using System;
using UnityEngine;

namespace Assets.Scripts.Core.Configs
{
    [CreateAssetMenu(menuName = "Configs/Fuel/FuelConfig")]
    public class FuelConfig : ScriptableObject
    {
        public FuelType type;
        public int unitPerClick;        // Сколько добавляет при клике
        public int initialAmount;       // Стартовое значение
        public float baseTemperature;   // Температура топлива
    }
}
