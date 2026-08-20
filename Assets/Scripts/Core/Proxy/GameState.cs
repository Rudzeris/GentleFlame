using Assets.Scripts.Core.Data;
using System;

namespace Assets.Scripts.Core.Proxy
{
    public class GameState : IDisposable
    {
        public readonly Currency currency;
        public readonly FireState fireState;
        public readonly FireStats fireStats;
        public readonly Progression progression;
        public readonly Storage storage;

        public readonly GameStateData origin;

        public GameState(GameStateData origin)
        {
            this.origin = origin ?? new GameStateData();
            this.origin.EnsureNotNull();

            currency = new Currency(this.origin.currency);
            fireState = new FireState(this.origin.fireState);
            fireStats = new FireStats(this.origin.fireStats);
            progression = new Progression(this.origin.progression);
            storage = new Storage(this.origin.resources);
        }

        public void Dispose()
        {
            currency.Dispose();
            fireState.Dispose();
            fireStats.Dispose();
            progression.Dispose();
            storage.Dispose();
        }
    }
}
