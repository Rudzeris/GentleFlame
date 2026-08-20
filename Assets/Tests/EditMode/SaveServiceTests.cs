using Assets.Scripts.Core.Data;
using Assets.Scripts.Core.Enums;
using Assets.Scripts.Core.Services;
using NUnit.Framework;
using System;
using System.IO;
using UnityEngine;

namespace Assets.Tests.EditMode
{
    [TestFixture]
    public class SaveServiceTests
    {
        private string _root;
        private FakeTimeProvider _clock;
        private TimeService _time;

        [SetUp]
        public void SetUp()
        {
            _root = Path.Combine(Path.GetTempPath(), "gentleflame_tests", Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_root);

            _clock = new FakeTimeProvider();
            _time = new TimeService(_clock);
        }

        private SaveService NewService() => new SaveService(_time, _root);

        [Test]
        public void FirstLaunch_DoesNotThrowAndCreatesState()
        {
            // Регрессия на T-05: раньше первый запуск падал с NullReferenceException,
            // потому что конструктор перебирал неинициализированный словарь.
            var service = NewService();
            var state = service.LoadGameState();

            Assert.IsNotNull(state);
            Assert.IsNotNull(state.currency);
            Assert.IsNotNull(state.storage);
            Assert.IsTrue(service.IsNewGame);
            Assert.IsTrue(File.Exists(service.SavePath));
        }

        [Test]
        public void RoundTrip_PreservesCurrenciesStorageAndHearth()
        {
            var first = NewService();
            var state = first.LoadGameState();

            new CurrencyService(state.currency).AddCurrency(CurrencyType.Coin, 1234);
            new StorageService(state.storage).AddFuel(FuelType.Log, 9);
            state.fireStats.Hearth[FuelType.Straw] = 3;
            state.fireStats.Temperature.Value = 41.5f;
            state.progression.Level.Value = 7;

            Assert.IsTrue(first.SaveGameState());

            var second = NewService();
            var loaded = second.LoadGameState();

            Assert.IsFalse(second.IsNewGame);
            Assert.AreEqual(1234, new CurrencyService(loaded.currency).Get(CurrencyType.Coin));
            Assert.AreEqual(9, new StorageService(loaded.storage).Get(FuelType.Log));
            Assert.AreEqual(3, loaded.fireStats.Hearth[FuelType.Straw]);
            Assert.AreEqual(3, loaded.fireStats.FuelAmount.Value);
            Assert.AreEqual(41.5d, loaded.fireStats.Temperature.Value, 0.01d);
            Assert.AreEqual(7, loaded.progression.Level.Value);
        }

        [Test]
        public void CorruptedSave_FallsBackToBackup()
        {
            var first = NewService();
            var state = first.LoadGameState();
            new CurrencyService(state.currency).AddCurrency(CurrencyType.Coin, 500);
            first.SaveGameState();

            // Второе сохранение переводит предыдущий валидный файл в резервную копию
            new CurrencyService(state.currency).AddCurrency(CurrencyType.Coin, 1);
            first.SaveGameState();

            File.WriteAllText(first.SavePath, "{ это не json");

            var second = NewService();
            var loaded = second.LoadGameState();

            Assert.IsNotNull(loaded);
            Assert.AreEqual(500, new CurrencyService(loaded.currency).Get(CurrencyType.Coin));
        }

        [Test]
        public void LegacyV1Save_IsMigratedToCurrentVersion()
        {
            var legacy = "{\"saveVersion\":1,\"lastSaveUtcTicks\":0," +
                         "\"progression\":{\"level\":0,\"exp\":0,\"stage\":0}}";

            File.WriteAllText(Path.Combine(_root, "gamestate.json"), legacy);

            var service = NewService();
            var loaded = service.LoadGameState();

            Assert.IsNotNull(loaded);
            Assert.AreEqual(SaveFormat.CurrentVersion, loaded.origin.saveVersion);
            Assert.AreEqual(1, loaded.progression.Level.Value, "Миграция обязана починить нулевой уровень");
        }

        [Test]
        public void AwayTime_IsMeasuredFromSaveTimestamp()
        {
            var first = NewService();
            first.LoadGameState();
            first.SaveGameState();

            _clock.Advance(TimeSpan.FromMinutes(30));

            var second = NewService();
            second.LoadGameState();

            Assert.AreEqual(1800d, second.LastAwaySeconds, 1d);
        }

        [Test]
        public void ClockMovedBackwards_YieldsZeroAwayTime()
        {
            var first = NewService();
            first.LoadGameState();
            first.SaveGameState();

            _clock.Advance(TimeSpan.FromHours(-5));

            var second = NewService();
            second.LoadGameState();

            Assert.AreEqual(0d, second.LastAwaySeconds, 0.001d);
        }

        [Test]
        public void Reset_ClearsProgress()
        {
            var service = NewService();
            var state = service.LoadGameState();
            new CurrencyService(state.currency).AddCurrency(CurrencyType.Coin, 999);
            service.SaveGameState();

            var fresh = service.ResetGameState();

            Assert.AreEqual(0, new CurrencyService(fresh.currency).Get(CurrencyType.Coin));
        }
    }
}
