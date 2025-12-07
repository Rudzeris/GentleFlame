using Assets.Scripts.Core.Data;
using System;

namespace Assets.Scripts.Core.Proxy
{
    [Serializable]
    public class GameState
    {
        public readonly Currency currency;
        public readonly FireState fireState;
        public readonly FireStats fireStats;
        public readonly Progression progression;
        public readonly Resources resources;

        public readonly GameStateData origin;

        public GameState(GameStateData origin)
        {
            this.origin = origin;

            currency = new Currency(origin.currency);
            fireState = new FireState(origin.fireState);
            fireStats = new FireStats(origin.fireStats);
            progression = new Progression(origin.progression);
            resources = new Resources(origin.resources);
        }
    }
}
