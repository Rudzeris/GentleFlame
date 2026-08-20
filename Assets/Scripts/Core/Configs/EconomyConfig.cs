using Assets.Scripts.Core.Enums;
using UnityEngine;

namespace Assets.Scripts.Core.Configs
{
    /// <summary>
    /// Все коэффициенты экономики и физики огня. Ни одно из этих чисел
    /// не должно появляться литералом в сервисах (критерий E4 вехи M1).
    /// </summary>
    [CreateAssetMenu(menuName = "GentleFlame/Configs/EconomyConfig")]
    public class EconomyConfig : ScriptableObject
    {
        [Header("Тепловая инерция (GDD 5.3)")]
        [Tooltip("Скорость нагрева: доля разницы, покрываемая за секунду")]
        public float heatingRate = 0.30f;

        [Tooltip("Скорость остывания. Меньше нагрева — очаг прощает игрока")]
        public float coolingRate = 0.12f;

        [Header("Доход Тепла: R * (T / divisor)^exponent * moodMultiplier")]
        public float warmthTemperatureDivisor = 50f;
        public float warmthExponent = 1.3f;

        [Tooltip("Базовая ставка Тепла в секунду по стадиям: Straw, Wood, Stone, Bonfire, Legendary")]
        public float[] warmthRatePerStage = { 0.5f, 1.5f, 5f, 18f, 60f };

        [Header("Множители настроения")]
        public float moodSleepMultiplier = 0.2f;
        public float moodSadMultiplier = 0.8f;
        public float moodHappyMultiplier = 1.0f;
        public float moodInspiresMultiplier = 1.5f;
        public float moodAngryMultiplier = 0.9f;

        [Header("Пороги яркости по температуре (GDD 4.6.3)")]
        public float brightAlmostOutThreshold = 30f;
        public float brightDimThreshold = 60f;

        [Header("Пороги настроения (GDD 4.6.4)")]
        [Tooltip("Доля заполнения очага, ниже которой огонь грустит")]
        public float moodSadFuelRatio = 0.3f;

        [Tooltip("Доля заполнения очага для состояния «вдохновлён»")]
        public float moodInspiresFuelRatio = 0.8f;

        [Tooltip("Температура для состояния «вдохновлён»")]
        public float moodInspiresTemperature = 80f;

        [Tooltip("Минимальная температура для «счастлив»")]
        public float moodHappyTemperature = 30f;

        public float GetWarmthRate(FireStage stage)
        {
            var index = (int)stage;

            if (warmthRatePerStage == null || warmthRatePerStage.Length == 0)
                return 0f;

            index = Mathf.Clamp(index, 0, warmthRatePerStage.Length - 1);
            return warmthRatePerStage[index];
        }

        /// <summary>
        /// Доход Тепла в секунду (GDD 5.3): R_stage * (T / divisor)^exponent * M_mood.
        /// Единая точка расчёта — используется и онлайн-тиком, и офлайн-симуляцией,
        /// чтобы они не разъехались.
        /// </summary>
        public float CalculateWarmthPerSecond(FireStage stage, float temperature, FireMood mood)
        {
            if (temperature <= 0f)
                return 0f;

            var divisor = warmthTemperatureDivisor <= 0f ? 1f : warmthTemperatureDivisor;
            var normalized = temperature / divisor;

            return GetWarmthRate(stage) * Mathf.Pow(normalized, warmthExponent) * GetMoodMultiplier(mood);
        }

        public float GetMoodMultiplier(FireMood mood)
        {
            switch (mood)
            {
                case FireMood.Sleep: return moodSleepMultiplier;
                case FireMood.Sad: return moodSadMultiplier;
                case FireMood.Inspires: return moodInspiresMultiplier;
                case FireMood.Angry: return moodAngryMultiplier;
                default: return moodHappyMultiplier;
            }
        }
    }
}
