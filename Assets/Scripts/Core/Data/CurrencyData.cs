using Assets.Scripts.Core.Enums;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Core.Data
{
    [Serializable]
    public class CurrencyData
    {
        public Dictionary<CurrencyType, int> currencies;
    }
}
