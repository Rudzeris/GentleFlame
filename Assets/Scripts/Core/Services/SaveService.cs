using Assets.Scripts.Core.Data;
using Assets.Scripts.Core.Proxy;
using System;
using UniRx;
using UnityEngine;

namespace Assets.Scripts.Core.Services
{
    public class SaveService : ISaveProvider
    {
        private const string GAME_STATE_KEY = "GentleFlame_Save";
        private readonly ConfigService _configService;
        public GameState GameState { get; private set; }

        private GameStateData _gameStateOrigin;

        public SaveService(ConfigService configService)
        {
            _configService = configService;
        }

        public IObservable<GameState> LoadGameState()
        {
            if (!PlayerPrefs.HasKey(GAME_STATE_KEY))
            {
                GameState = CreateGameStateFromSettings();
                Debug.Log("Game State created from settings: " + JsonUtility.ToJson(_gameStateOrigin, true));

                SaveGameState();
            }
            else
            {
                var json = PlayerPrefs.GetString(GAME_STATE_KEY);
                _gameStateOrigin = JsonUtility.FromJson<GameStateData>(json);
                GameState = new GameState(_gameStateOrigin);

                Debug.Log("Game State loaded: " + json);
            }

            return Observable.Return(GameState);
        }

        private GameState CreateGameStateFromSettings()
        {
            var currencyData = new CurrencyData();
            var fireStateData = new FireStateData();
            var fireStatsData = new FireStatsData();
            var progressionData = new ProgressionData();
            var resourcesData = new ResourcesData();

            _gameStateOrigin = new GameStateData(
                currencyData, fireStateData, fireStatsData, progressionData, resourcesData);

            GameState = new GameState(_gameStateOrigin);

            return GameState;
        }

        public IObservable<bool> ResetGameState()
        {
            GameState = CreateGameStateFromSettings();
            SaveGameState();

            return Observable.Return(true);
        }

        public IObservable<bool> SaveGameState()
        {
            var json = JsonUtility.ToJson(_gameStateOrigin, true);
            PlayerPrefs.SetString(GAME_STATE_KEY, json);

            return Observable.Return(true);
        }
    }
}
