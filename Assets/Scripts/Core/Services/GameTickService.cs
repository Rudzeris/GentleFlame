using System.Collections.Generic;

namespace Assets.Scripts.Core.Services
{
    /// <summary>
    /// Единая точка раздачи игрового времени. Драйвится из Unity (Zenject ITickable),
    /// сам по себе о движке ничего не знает — поэтому тестируется без сцены.
    /// </summary>
    public class GameTickService
    {
        private readonly List<IGameTickListener> _listeners = new List<IGameTickListener>();
        private readonly List<IGameTickListener> _pendingAdd = new List<IGameTickListener>();
        private readonly List<IGameTickListener> _pendingRemove = new List<IGameTickListener>();

        private bool _ticking;

        public bool IsPaused { get; set; }

        public void Register(IGameTickListener listener)
        {
            if (listener == null)
                return;

            if (_ticking)
                _pendingAdd.Add(listener);
            else if (!_listeners.Contains(listener))
                _listeners.Add(listener);
        }

        public void Unregister(IGameTickListener listener)
        {
            if (listener == null)
                return;

            if (_ticking)
                _pendingRemove.Add(listener);
            else
                _listeners.Remove(listener);
        }

        public void Tick(float deltaTime)
        {
            if (IsPaused || deltaTime <= 0f)
                return;

            _ticking = true;

            for (var i = 0; i < _listeners.Count; i++)
                _listeners[i].Tick(deltaTime);

            _ticking = false;

            if (_pendingAdd.Count > 0)
            {
                foreach (var listener in _pendingAdd)
                {
                    if (!_listeners.Contains(listener))
                        _listeners.Add(listener);
                }

                _pendingAdd.Clear();
            }

            if (_pendingRemove.Count > 0)
            {
                foreach (var listener in _pendingRemove)
                    _listeners.Remove(listener);

                _pendingRemove.Clear();
            }
        }
    }
}
