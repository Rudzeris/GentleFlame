using Assets.Scripts.Core.Enums;
using System;

namespace Assets.Scripts.Core.Data
{
    /// <summary>
    /// Сколько единиц топлива данного типа сейчас лежит в очаге.
    /// Нужен, чтобы целевая температура считалась как средневзвешенное по составу очага (GDD 5.3),
    /// а не по одному «последнему» топливу.
    /// </summary>
    [Serializable]
    public class HearthSlotData
    {
        public FuelType type;
        public int count;

        public HearthSlotData() { }

        public HearthSlotData(FuelType type, int count)
        {
            this.type = type;
            this.count = count;
        }
    }
}
