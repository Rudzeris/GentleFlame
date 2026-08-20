using Assets.Scripts.Core.Data;

namespace Assets.Scripts.Core.Services
{
    /// <summary>Шаг миграции сейва с версии FromVersion на FromVersion + 1.</summary>
    public interface ISaveMigration
    {
        int FromVersion { get; }
        void Apply(GameStateData data);
    }

    /// <summary>
    /// v1 → v2. В v1 валюты и склад лежали в Dictionary, который JsonUtility не сериализовал вовсе,
    /// поэтому переносить оттуда нечего — задача шага в том, чтобы привести структуру к валидному виду.
    /// </summary>
    public class MigrationV1ToV2 : ISaveMigration
    {
        public int FromVersion => 1;

        public void Apply(GameStateData data)
        {
            data.EnsureNotNull();

            if (data.progression.level < 1)
                data.progression.level = 1;

            data.saveVersion = 2;
        }
    }
}
