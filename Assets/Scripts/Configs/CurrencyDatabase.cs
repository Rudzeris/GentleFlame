using UnityEngine;

namespace Assets.Scripts.Configs
{
    [CreateAssetMenu(fileName = "CurrencyDatabase", menuName = "Databases/CurrencyDatabase")]
    public class CurrencyDatabase : ScriptableObject
    {
        public CurrencyConfig[] Currencies;

        public CurrencyConfig Get(CurrencyType type)
        {
            foreach (var currency in Currencies)
            {
                if (currency.type == type) return currency;
            }
            return null;
        }
    }

}
