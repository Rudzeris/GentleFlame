using System;

namespace Assets.Scripts.Core.Data
{
    [Serializable]
    public class GameStateData
    {
        /// <summary>Версия формата сохранения. Используется цепочкой миграций в SaveService.</summary>
        public int saveVersion = SaveFormat.CurrentVersion;

        /// <summary>UTC-время последнего сохранения в тиках. Основа офлайн-расчёта.</summary>
        public long lastSaveUtcTicks;

        public CurrencyData currency = new CurrencyData();
        public FireStateData fireState = new FireStateData();
        public FireStatsData fireStats = new FireStatsData();
        public ProgressionData progression = new ProgressionData();
        public ResourcesData resources = new ResourcesData();

        public GameStateData() { }

        public GameStateData(CurrencyData currency, FireStateData fireState, FireStatsData fireStats,
            ProgressionData progression, ResourcesData resources)
        {
            this.currency = currency ?? new CurrencyData();
            this.fireState = fireState ?? new FireStateData();
            this.fireStats = fireStats ?? new FireStatsData();
            this.progression = progression ?? new ProgressionData();
            this.resources = resources ?? new ResourcesData();
        }

        /// <summary>Защита от сейва, в котором вложенный объект оказался null (старый формат, обрыв записи).</summary>
        public void EnsureNotNull()
        {
            currency ??= new CurrencyData();
            fireState ??= new FireStateData();
            fireStats ??= new FireStatsData();
            progression ??= new ProgressionData();
            resources ??= new ResourcesData();

            currency.currencies ??= new System.Collections.Generic.List<CurrencyEntry>();
            resources.fuels ??= new System.Collections.Generic.List<FuelData>();
            fireStats.hearth ??= new System.Collections.Generic.List<HearthSlotData>();
        }
    }

    public static class SaveFormat
    {
        /// <summary>v1 — исходный формат со словарями. v2 — списки пар, состав очага, версия и метка времени.</summary>
        public const int CurrentVersion = 2;
    }
}
