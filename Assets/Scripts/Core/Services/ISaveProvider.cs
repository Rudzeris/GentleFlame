using Assets.Scripts.Core.Proxy;
using System;
using UniRx;

namespace Assets.Scripts.Core.Services
{
    public interface ISaveProvider
    {
        public GameState GameState { get; }

        public IObservable<GameState> LoadGameState();
        public IObservable<bool> SaveGameState();
        public IObservable<bool> ResetGameState();
    }
}
