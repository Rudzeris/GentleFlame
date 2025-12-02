using System;

namespace Assets.Scripts.Core.Data
{
    [Serializable]
    public class FireStatsData
    {
        public int maxFuelCapacity;
        public int fuelAmount;
        public float targetTemperature;
        public float temperature;
        public float burnTimeLeft;
    }
}
