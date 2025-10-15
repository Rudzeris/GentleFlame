using UnityEngine;

namespace Assets.Scripts.Configs
{
    public enum CurrencyType
    {
        Coin, Crystal
    }
    [CreateAssetMenu(menuName = "Configs/CurrencyConfig")]
    public class CurrencyConfig : ScriptableObject
    {
        [Header("Основные параметры")]
        public CurrencyType type;
        public string DisplayName;
        public Sprite icon;

        [Header("Ограничения")]
        public int startAmount = 0;
        public int maxAmount = 1000;
    }
}
