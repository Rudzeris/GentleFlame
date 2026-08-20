using Assets.Scripts.Core.Enums;
using UnityEngine;

namespace Assets.Scripts.Core.Configs
{
    /// <summary>Кривая опыта, стадии и офлайн-капы (GDD 5.3, 5.4, 5.6).</summary>
    [CreateAssetMenu(menuName = "GentleFlame/Configs/ProgressionConfig")]
    public class ProgressionConfig : ScriptableObject
    {
        [Header("Кривая опыта: cost(L) = multiplier * L^exponent")]
        public int expCurveMultiplier = 20;
        public float expCurveExponent = 2f;

        [Header("Ёмкость очага")]
        public int baseFuelCapacity = 5;

        [Tooltip("Потолок ёмкости по стадиям: Straw, Wood, Stone, Bonfire, Legendary")]
        public int[] capacityCapPerStage = { 8, 14, 22, 34, 50 };

        [Header("Стадии: минимальный уровень для Wood, Stone, Bonfire, Legendary")]
        public int[] stageMinLevel = { 5, 12, 25, 40 };

        [Header("Офлайн (GDD 5.6)")]
        [Tooltip("Доля от онлайн-дохода, начисляемая офлайн")]
        public float offlineRate = 0.5f;

        [Tooltip("Максимум накопления в часах по стадиям")]
        public float[] offlineCapHoursPerStage = { 1f, 2f, 4f, 8f, 12f };

        /// <summary>Сколько опыта нужно, чтобы перейти с уровня level на level + 1.</summary>
        public int GetExpForNextLevel(int level)
        {
            if (level < 1)
                level = 1;

            return Mathf.Max(1, (int)(expCurveMultiplier * Mathf.Pow(level, expCurveExponent)));
        }

        public FireStage GetStageForLevel(int level)
        {
            if (stageMinLevel == null)
                return FireStage.Straw;

            var stage = FireStage.Straw;

            for (var i = 0; i < stageMinLevel.Length; i++)
            {
                if (level >= stageMinLevel[i])
                    stage = (FireStage)(i + 1);
            }

            return stage;
        }

        public int GetCapacityCap(FireStage stage)
        {
            if (capacityCapPerStage == null || capacityCapPerStage.Length == 0)
                return baseFuelCapacity;

            var index = Mathf.Clamp((int)stage, 0, capacityCapPerStage.Length - 1);
            return capacityCapPerStage[index];
        }

        public float GetOfflineCapSeconds(FireStage stage)
        {
            if (offlineCapHoursPerStage == null || offlineCapHoursPerStage.Length == 0)
                return 0f;

            var index = Mathf.Clamp((int)stage, 0, offlineCapHoursPerStage.Length - 1);
            return offlineCapHoursPerStage[index] * 3600f;
        }
    }
}
