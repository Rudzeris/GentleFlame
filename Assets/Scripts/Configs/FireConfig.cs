using UnityEngine;

namespace Assets.Scripts.Configs
{
    [CreateAssetMenu(menuName = "Configs/FireConfig")]
    public class FireConfig : ScriptableObject
    {
        [Header("Топливо")]
        [Range(0.01f, 1f)] public float fuelConsumptionPerSecond = 0.01f;
        [Range(0.01f, 1f)] public float fuelGainPerWood = 0.1f;

        [Header("Ограничение")]
        public float maxFuel = 1f;
    }
}
