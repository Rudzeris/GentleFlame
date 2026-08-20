using Assets.Scripts.Core.Services;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Installers
{
    /// <summary>
    /// Единственное место, где движковое время попадает в игровую логику.
    /// Сервисы получают deltaTime и остаются тестируемыми без сцены (T-11).
    /// </summary>
    public class GameTickDriver : ITickable
    {
        private readonly GameTickService _tick;

        public GameTickDriver(GameTickService tick)
        {
            _tick = tick;
        }

        public void Tick() => _tick.Tick(Time.deltaTime);
    }
}
