using Assets.Scripts.Core.Enums;
using System;

namespace Assets.Scripts.Core.Data
{
    [Serializable]
    public class FuelData
    {
        public FuelType type;
        public int amount;

        public FuelData() { }

        public FuelData(FuelType type, int amount)
        {
            this.type = type;
            this.amount = amount;
        }
    }
}
