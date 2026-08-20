using Assets.Scripts.Core.Configs;
using Assets.Scripts.Core.Proxy;
using UnityEngine;

namespace Assets.Scripts.Core.Services
{
    /// <summary>
    /// Опыт, уровни и стадии костра.
    /// Прежняя версия игнорировала переданный опыт и безусловно наращивала ёмкость очага (T-03, T-04).
    /// </summary>
    public class ProgressService
    {
        private readonly Progression _progression;
        private readonly FireStats _stats;
        private readonly ProgressionConfig _config;

        public ProgressService(Progression progression, FireStats stats, ConfigService configs)
        {
            _progression = progression;
            _stats = stats;
            _config = configs.Progression;
        }

        /// <summary>Приводит стадию и ёмкость в соответствие текущему уровню. Вызывается при старте игры.</summary>
        public void Initialize()
        {
            if (_progression.Level.Value < 1)
                _progression.Level.Value = 1;

            UpdateStage();

            if (_stats.MaxFuelCapacity.Value < _config.baseFuelCapacity)
                _stats.MaxFuelCapacity.Value = _config.baseFuelCapacity;

            ClampCapacityToStage();
        }

        public int GetExpForNextLevel() => _config.GetExpForNextLevel(_progression.Level.Value);

        public void AddExp(int amount)
        {
            if (amount <= 0)
                return;

            _progression.Exp.Value += amount;

            var needed = _config.GetExpForNextLevel(_progression.Level.Value);

            // >= , а не > : ровно пороговое значение обязано давать уровень (T-03).
            while (_progression.Exp.Value >= needed)
            {
                _progression.Exp.Value -= needed;
                _progression.Level.Value++;
                needed = _config.GetExpForNextLevel(_progression.Level.Value);
            }

            UpdateStage();
        }

        private void UpdateStage()
        {
            var stage = _config.GetStageForLevel(_progression.Level.Value);

            if (_progression.Stage.Value == stage)
                return;

            _progression.Stage.Value = stage;
            ClampCapacityToStage();
        }

        /// <summary>
        /// Ёмкость растёт от покупаемых апгрейдов, а не сама по себе.
        /// Здесь только потолок стадии, чтобы апгрейды не обгоняли контент.
        /// </summary>
        private void ClampCapacityToStage()
        {
            var cap = _config.GetCapacityCap(_progression.Stage.Value);

            if (_stats.MaxFuelCapacity.Value > cap)
                _stats.MaxFuelCapacity.Value = cap;
        }
    }
}
