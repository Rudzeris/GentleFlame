using Assets.Scripts.Core.Configs;
using UnityEngine;

namespace Assets.Scripts.Core.Services
{
    public class ConfigService
    {
        [SerializeField] private string fuelsConfig = "Configs/FuelsConfig";
        public FuelsConfig Fuels { get; private set; }

        public ConfigService()
        {
            Fuels = Resources.Load<FuelsConfig>(fuelsConfig);

            if (Fuels == null)
                Debug.LogError($"Не найден FuelsConfig по пути {fuelsConfig}");
        }
    }
}
