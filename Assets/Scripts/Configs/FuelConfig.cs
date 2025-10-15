using UnityEngine;

namespace Assets.Scripts.Configs
{
    public enum FuelType
    {
        Wood
    }
    [CreateAssetMenu(menuName = "Configs/FuelConfig")]
    public class FuelConfig : ScriptableObject
    {
        [Header("Основные параметры")]
        public FuelType type;
        public string displayName;
        public Sprite icon;
        public int burnTimeSeconds;
        public int rarity;

        [Header("Ограничения")]
        public int startAmount = 0;
        public int maxAmount = 64;
    }
}
