using Assets.Scripts.Core.Configs;
using UnityEngine;

namespace Assets.Scripts.Core.Services
{
    /// <summary>
    /// Доступ к конфигам. Раньше путь был помечен [SerializeField] на обычном C#-классе,
    /// где атрибут не работает, а самого ассета в проекте не существовало (T-15).
    /// </summary>
    public class ConfigService
    {
        public const string FuelsConfigPath = "Configs/FuelsConfig";
        public const string EconomyConfigPath = "Configs/EconomyConfig";
        public const string ProgressionConfigPath = "Configs/ProgressionConfig";

        public FuelsConfig Fuels { get; private set; }
        public EconomyConfig Economy { get; private set; }
        public ProgressionConfig Progression { get; private set; }

        public bool IsValid => Fuels != null && Economy != null && Progression != null;

        /// <summary>Боевой путь: конфиги грузятся из Resources.</summary>
        public ConfigService()
        {
            Fuels = Load<FuelsConfig>(FuelsConfigPath);
            Economy = Load<EconomyConfig>(EconomyConfigPath);
            Progression = Load<ProgressionConfig>(ProgressionConfigPath);
        }

        /// <summary>Путь для тестов и редакторных инструментов: конфиги передаются напрямую.</summary>
        public ConfigService(FuelsConfig fuels, EconomyConfig economy, ProgressionConfig progression)
        {
            Fuels = fuels;
            Economy = economy;
            Progression = progression;
        }

        private static T Load<T>(string path) where T : ScriptableObject
        {
            var asset = Resources.Load<T>(path);

            if (asset == null)
            {
                Debug.LogError(
                    $"ConfigService: не найден {typeof(T).Name} по пути Resources/{path}. " +
                    "Создайте ассеты через меню GentleFlame → Create Default Configs.");
            }

            return asset;
        }
    }
}
