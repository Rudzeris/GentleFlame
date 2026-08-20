using Assets.Scripts.Core.Data;
using Assets.Scripts.Core.Proxy;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Assets.Scripts.Core.Services
{
    /// <summary>
    /// Сохранение в файл с версией формата, миграциями и резервной копией.
    /// Прежняя версия писала в PlayerPrefs, не имела версии и падала на первом запуске,
    /// потому что перебирала неинициализированные словари (T-05, T-13).
    /// </summary>
    public class SaveService : ISaveProvider
    {
        private const string FileName = "gamestate.json";
        private const string BackupFileName = "gamestate.backup.json";

        private readonly TimeService _time;
        private readonly List<ISaveMigration> _migrations;
        private readonly string _root;

        private GameStateData _origin;

        public GameState GameState { get; private set; }

        /// <summary>Сколько секунд игрок отсутствовал по данным загруженного сейва.</summary>
        public double LastAwaySeconds { get; private set; }

        /// <summary>Игра начата с нуля — значит, нужно выдать стартовое топливо и показать FTUE.</summary>
        public bool IsNewGame { get; private set; }

        public SaveService(TimeService time) : this(time, Application.persistentDataPath) { }

        public SaveService(TimeService time, string root)
        {
            _time = time;
            _root = root;

            _migrations = new List<ISaveMigration> { new MigrationV1ToV2() };
        }

        public string SavePath => Path.Combine(_root, FileName);
        public string BackupPath => Path.Combine(_root, BackupFileName);

        public GameState LoadGameState()
        {
            var data = ReadFromDisk(SavePath) ?? ReadFromDisk(BackupPath);

            if (data == null)
            {
                GameState = CreateNewGame();
                SaveGameState();
                return GameState;
            }

            IsNewGame = false;
            Migrate(data);

            LastAwaySeconds = _time.SecondsSince(data.lastSaveUtcTicks);

            _origin = data;
            GameState = new GameState(_origin);

            return GameState;
        }

        public bool SaveGameState()
        {
            if (_origin == null)
                return false;

            try
            {
                Directory.CreateDirectory(_root);

                _origin.saveVersion = SaveFormat.CurrentVersion;
                _origin.lastSaveUtcTicks = _time.NowTicks;

                var json = JsonUtility.ToJson(_origin, true);
                var temp = SavePath + ".tmp";

                File.WriteAllText(temp, json);

                // Предыдущий валидный сейв становится резервной копией, и только потом
                // временный файл занимает основное место — обрыв записи не оставляет игрока без данных.
                if (File.Exists(SavePath))
                    File.Copy(SavePath, BackupPath, true);

                if (File.Exists(SavePath))
                    File.Delete(SavePath);

                File.Move(temp, SavePath);

                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"SaveService: не удалось сохранить игру: {e.Message}");
                return false;
            }
        }

        public GameState ResetGameState()
        {
            GameState?.Dispose();
            GameState = CreateNewGame();
            SaveGameState();

            return GameState;
        }

        private GameState CreateNewGame()
        {
            _origin = new GameStateData();
            _origin.EnsureNotNull();
            _origin.lastSaveUtcTicks = _time.NowTicks;

            LastAwaySeconds = 0d;
            IsNewGame = true;

            return new GameState(_origin);
        }

        private GameStateData ReadFromDisk(string path)
        {
            try
            {
                if (!File.Exists(path))
                    return null;

                var json = File.ReadAllText(path);

                if (string.IsNullOrWhiteSpace(json))
                    return null;

                var data = JsonUtility.FromJson<GameStateData>(json);

                if (data == null)
                    return null;

                data.EnsureNotNull();
                return data;
            }
            catch (Exception e)
            {
                Debug.LogError($"SaveService: сейв {path} повреждён и будет пропущен: {e.Message}");
                return null;
            }
        }

        private void Migrate(GameStateData data)
        {
            if (data.saveVersion >= SaveFormat.CurrentVersion)
                return;

            var guard = 0;

            while (data.saveVersion < SaveFormat.CurrentVersion && guard++ < 32)
            {
                var step = _migrations.Find(m => m.FromVersion == data.saveVersion);

                if (step == null)
                {
                    Debug.LogError(
                        $"SaveService: нет миграции с версии {data.saveVersion}. " +
                        "Сейв принят как есть — часть данных может быть потеряна.");

                    data.saveVersion = SaveFormat.CurrentVersion;
                    break;
                }

                step.Apply(data);
            }
        }
    }
}
