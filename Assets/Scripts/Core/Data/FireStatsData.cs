using System;
using System.Collections.Generic;

namespace Assets.Scripts.Core.Data
{
    [Serializable]
    public class FireStatsData
    {
        public int maxFuelCapacity;
        public float temperature;
        public float burnTimeLeft;

        /// <summary>Состав очага. Целевая температура выводится из него, поэтому не сохраняется.</summary>
        public List<HearthSlotData> hearth = new List<HearthSlotData>();
    }
}
