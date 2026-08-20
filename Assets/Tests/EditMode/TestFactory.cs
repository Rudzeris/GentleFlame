using Assets.Scripts.Core.Configs;
using Assets.Scripts.Core.Data;
using Assets.Scripts.Core.Enums;
using Assets.Scripts.Core.Proxy;
using Assets.Scripts.Core.Services;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Tests.EditMode
{
    /// <summary>Сборка изолированного игрового состояния для тестов: без сцены и без Resources.</summary>
    public static class TestFactory
    {
        public static FuelConfig Fuel(FuelType type, float temperature, float burnTime, int initialAmount = 0)
        {
            var config = ScriptableObject.CreateInstance<FuelConfig>();
            config.type = type;
            config.temperature = temperature;
            config.burnTimeSeconds = burnTime;
            config.initialAmount = initialAmount;
            return config;
        }

        public static FuelsConfig Fuels(params FuelConfig[] fuels)
        {
            var config = ScriptableObject.CreateInstance<FuelsConfig>();
            config.baseBurnTimeMultiplier = 1f;
            config.SetFuels(new List<FuelConfig>(fuels));
            return config;
        }

        /// <summary>Солома 20°/30 с и полено 60°/180 с — минимальный набор для проверки смешивания.</summary>
        public static FuelsConfig DefaultFuels()
            => Fuels(Fuel(FuelType.Straw, 20f, 30f), Fuel(FuelType.Log, 60f, 180f));

        public static EconomyConfig Economy() => ScriptableObject.CreateInstance<EconomyConfig>();

        public static ProgressionConfig Progression() => ScriptableObject.CreateInstance<ProgressionConfig>();

        public static ConfigService Configs(FuelsConfig fuels = null)
            => new ConfigService(fuels ?? DefaultFuels(), Economy(), Progression());

        public static GameState NewGameState(int capacity = 5)
        {
            var data = new GameStateData();
            data.EnsureNotNull();
            data.fireStats.maxFuelCapacity = capacity;
            return new GameState(data);
        }
    }

    /// <summary>Управляемое время: тесты офлайна не должны зависеть от системных часов.</summary>
    public class FakeTimeProvider : ITimeProvider
    {
        public DateTime Now = new DateTime(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc);
        public DateTime UtcNow => Now;
        public void Advance(TimeSpan by) => Now += by;
    }
}
