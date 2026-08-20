using Assets.Scripts.Core.Proxy;

namespace Assets.Scripts.Core.Services
{
    public interface ISaveProvider
    {
        GameState GameState { get; }

        /// <summary>Загружает сейв, при его отсутствии создаёт новую игру. Никогда не возвращает null.</summary>
        GameState LoadGameState();

        bool SaveGameState();
        GameState ResetGameState();
    }
}
