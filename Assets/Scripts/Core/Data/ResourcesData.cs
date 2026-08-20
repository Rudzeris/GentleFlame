using System;
using System.Collections.Generic;

namespace Assets.Scripts.Core.Data
{
    /// <summary>Склад топлива игрока.</summary>
    [Serializable]
    public class ResourcesData
    {
        public List<FuelData> fuels = new List<FuelData>();
    }
}
