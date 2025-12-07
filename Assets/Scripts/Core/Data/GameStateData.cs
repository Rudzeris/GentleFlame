using System;

namespace Assets.Scripts.Core.Data
{
    [Serializable]
    public class GameStateData
    {
        public CurrencyData currency;
        public FireStateData fireState;
        public FireStatsData fireStats;
        public ProgressionData progression;
        public ResourcesData resources;

        public GameStateData(CurrencyData currency, FireStateData fireState, FireStatsData fireStats, ProgressionData progression, ResourcesData resources)
        {
            this.currency = currency;
            this.fireState = fireState;
            this.fireStats = fireStats;
            this.progression = progression;
            this.resources = resources;
        }
    }
}
