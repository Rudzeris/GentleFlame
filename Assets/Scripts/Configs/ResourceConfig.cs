using UnityEngine;

namespace Assets.Scripts.Configs
{
    [CreateAssetMenu(menuName = "Configs/ResourceConfig")]
    public class ResourceConfig : ScriptableObject
    {
        [Header("Основные параметры")]
        public string resourceName;
        public Sprite icon;

        [Header("Ограничения")]
        public int startAmount = 10;
        public int maxAmount = 30;
    }
}
