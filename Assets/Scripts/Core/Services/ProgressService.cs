using Assets.Scripts.Core.Proxy;
using System;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine.Assertions.Must;

namespace Assets.Scripts.Core.Services
{
    public class ProgressService
    {
        private readonly Progression _progression;
        private readonly FireStats _stats;

        public ProgressService(Progression progression, FireStats stats)
        {
            _progression = progression;
            _stats = stats;
        }

        public void AddExp(int exp)
        {
            var expForNextLevel = GetExpForLevel(_progression.Level.Value + 1);

            while (_progression.Exp.Value > expForNextLevel)
            {
                _progression.Level.Value++;
                _progression.Exp.Value -= expForNextLevel;
                expForNextLevel = GetExpForLevel(_progression.Level.Value + 1);
            }

            UpdateMaxFuelCapacity();
        }

        private void UpdateMaxFuelCapacity()
        {
            _stats.MaxFuelCapacity.Value += 2;
        }

        private int GetExpForLevel(int level)
            => level * level;
    }
}
