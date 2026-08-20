using Assets.Scripts.Core.Configs;
using Assets.Scripts.Core.Enums;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Assets.Editor
{
    /// <summary>
    /// Создаёт конфиги по значениям из GDD 5.2 и 5.3.
    /// Ассеты нельзя завести из кода вне редактора, поэтому это разовое действие разработчика:
    /// GentleFlame → Create Default Configs.
    /// </summary>
    public static class ConfigBootstrap
    {
        private const string ConfigsFolder = "Assets/Resources/Configs";
        private const string FuelsFolder = ConfigsFolder + "/Fuels";

        private struct FuelPreset
        {
            public FuelType Type;
            public float Temperature;
            public float BurnTime;
            public int Price;
            public int Initial;

            public FuelPreset(FuelType type, float temperature, float burnTime, int price, int initial = 0)
            {
                Type = type;
                Temperature = temperature;
                BurnTime = burnTime;
                Price = price;
                Initial = initial;
            }
        }

        // Значения — стартовая гипотеза баланса из GDD 5.2, дальше тюнятся в таблице.
        private static readonly FuelPreset[] Presets =
        {
            new FuelPreset(FuelType.Straw, 20f, 30f, 2, initial: 10),
            new FuelPreset(FuelType.Twigs, 35f, 60f, 6, initial: 5),
            new FuelPreset(FuelType.Cones, 45f, 45f, 12),
            new FuelPreset(FuelType.Log, 60f, 180f, 40),
            new FuelPreset(FuelType.Resin, 80f, 60f, 90),
            new FuelPreset(FuelType.Coal, 75f, 480f, 150),
            new FuelPreset(FuelType.Heartwood, 95f, 900f, 600),
        };

        [MenuItem("GentleFlame/Create Default Configs")]
        public static void CreateDefaultConfigs()
        {
            EnsureFolder("Assets/Resources");
            EnsureFolder(ConfigsFolder);
            EnsureFolder(FuelsFolder);

            var fuels = new List<FuelConfig>();

            foreach (var preset in Presets)
            {
                var path = $"{FuelsFolder}/Fuel_{preset.Type}.asset";
                var config = AssetDatabase.LoadAssetAtPath<FuelConfig>(path);

                if (config == null)
                {
                    config = ScriptableObject.CreateInstance<FuelConfig>();
                    AssetDatabase.CreateAsset(config, path);
                }

                config.type = preset.Type;
                config.temperature = preset.Temperature;
                config.burnTimeSeconds = preset.BurnTime;
                config.priceInWarmth = preset.Price;
                config.initialAmount = preset.Initial;
                config.unitPerClick = 1;

                EditorUtility.SetDirty(config);
                fuels.Add(config);
            }

            var fuelsConfig = GetOrCreate<FuelsConfig>($"{ConfigsFolder}/FuelsConfig.asset");
            fuelsConfig.baseBurnTimeMultiplier = 1f;
            fuelsConfig.SetFuels(fuels);
            EditorUtility.SetDirty(fuelsConfig);

            GetOrCreate<EconomyConfig>($"{ConfigsFolder}/EconomyConfig.asset");
            GetOrCreate<ProgressionConfig>($"{ConfigsFolder}/ProgressionConfig.asset");

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"GentleFlame: конфиги созданы в {ConfigsFolder}");
        }

        private static T GetOrCreate<T>(string path) where T : ScriptableObject
        {
            var asset = AssetDatabase.LoadAssetAtPath<T>(path);

            if (asset != null)
                return asset;

            asset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, path);

            return asset;
        }

        private static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path))
                return;

            var parent = Path.GetDirectoryName(path)?.Replace('\\', '/');
            var leaf = Path.GetFileName(path);

            if (!string.IsNullOrEmpty(parent) && !AssetDatabase.IsValidFolder(parent))
                EnsureFolder(parent);

            AssetDatabase.CreateFolder(parent, leaf);
        }
    }
}
