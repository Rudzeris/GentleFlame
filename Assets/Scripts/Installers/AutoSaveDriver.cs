using Assets.Scripts.Core.Services;
using System;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Installers
{
    /// <summary>Автосохранение по таймеру, при потере фокуса и при выходе (B5).</summary>
    public class AutoSaveDriver : ITickable, IInitializable, IDisposable
    {
        private const float SaveIntervalSeconds = 30f;

        private readonly SaveService _save;

        private float _elapsed;

        public AutoSaveDriver(SaveService save)
        {
            _save = save;
        }

        public void Initialize()
        {
            Application.focusChanged += OnFocusChanged;
            Application.quitting += OnQuitting;
        }

        public void Tick()
        {
            _elapsed += Time.deltaTime;

            if (_elapsed < SaveIntervalSeconds)
                return;

            _elapsed = 0f;
            _save.SaveGameState();
        }

        private void OnFocusChanged(bool hasFocus)
        {
            if (!hasFocus)
                _save.SaveGameState();
        }

        private void OnQuitting() => _save.SaveGameState();

        public void Dispose()
        {
            Application.focusChanged -= OnFocusChanged;
            Application.quitting -= OnQuitting;

            _save.SaveGameState();
        }
    }
}
