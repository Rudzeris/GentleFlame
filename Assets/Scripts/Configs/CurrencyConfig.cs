using UnityEngine;

namespace Assets.Scripts.Configs
{
    [CreateAssetMenu(menuName = "Configs/CurrencyConfig")]
    public class CurrencyConfig : ScriptableObject
    {
        [Header("Основные параметры")]
        public string resourceName;
        public Sprite icon;

        [Header("Ограничения")]
        public int startCup = 0;
        public int softCup = 1000;
    }
}
