namespace Assets.Scripts.Core.Services
{
    /// <summary>
    /// Подписчик игрового тика. Введён вместо цикла while(true) в конструкторе FireService (T-11):
    /// сервисы больше не запускают собственные задачи и не живут вне жизненного цикла сцены.
    /// </summary>
    public interface IGameTickListener
    {
        void Tick(float deltaTime);
    }
}
