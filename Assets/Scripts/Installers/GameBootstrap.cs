using Assets.Scripts.Core.Configs;
using Assets.Scripts.Core.Services;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Installers
{
    /// <summary>
    /// Порядок старта игры: привести прогрессию в согласованное состояние,
    /// выдать стартовое топливо новичку, начислить офлайн-доход, запустить тик.
    /// </summary>
    public class GameBootstrap : IInitializable
    {
        private readonly SaveService _save;
        private readonly ConfigService _configs;
        private readonly ProgressService _progress;
        private readonly StorageService _storage;
        private readonly OfflineService _offline;
        private readonly GameTickService _tick;
        private readonly FireService _fire;
        private readonly WarmthService _warmth;
        private readonly StateService _state;
        private readonly ExperienceService _experience;

        public OfflineReport LastOfflineReport { get; private set; }

        public GameBootstrap(SaveService save, ConfigService configs, ProgressService progress,
            StorageService storage, OfflineService offline, GameTickService tick,
            FireService fire, WarmthService warmth, StateService state, ExperienceService experience)
        {
            _save = save;
            _configs = configs;
            _progress = progress;
            _storage = storage;
            _offline = offline;
            _tick = tick;
            _fire = fire;
            _warmth = warmth;
            _state = state;
            _experience = experience;
        }

        public void Initialize()
        {
            if (!_configs.IsValid)
            {
                Debug.LogError("GameBootstrap: конфиги не загружены, игра запущена не будет");
                return;
            }

            _progress.Initialize();
            _state.Initialize();
            _experience.Initialize();

            if (_save.IsNewGame)
                GrantStartingFuel();
            else
                LastOfflineReport = _offline.Simulate(_save.LastAwaySeconds);

            _tick.Register(_fire);
            _tick.Register(_warmth);


            _save.SaveGameState();
        }

        private void GrantStartingFuel()
        {
            foreach (var fuel in _configs.Fuels.All)
            {
                if (fuel != null && fuel.initialAmount > 0)
                    _storage.AddFuel(fuel.type, fuel.initialAmount);
            }
        }
    }
}
