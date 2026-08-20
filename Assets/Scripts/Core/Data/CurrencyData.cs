using Assets.Scripts.Core.Enums;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Core.Data
{
    /// <summary>Пара «валюта → количество». Список вместо словаря: JsonUtility не умеет Dictionary.</summary>
    [Serializable]
    public class CurrencyEntry
    {
        public CurrencyType type;
        public int amount;

        public CurrencyEntry() { }

        public CurrencyEntry(CurrencyType type, int amount)
        {
            this.type = type;
            this.amount = amount;
        }
    }

    [Serializable]
    public class CurrencyData
    {
        public List<CurrencyEntry> currencies = new List<CurrencyEntry>();
    }
}
