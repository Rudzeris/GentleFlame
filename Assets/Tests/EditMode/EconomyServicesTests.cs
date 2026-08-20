using Assets.Scripts.Core.Enums;
using Assets.Scripts.Core.Proxy;
using Assets.Scripts.Core.Services;
using NUnit.Framework;

namespace Assets.Tests.EditMode
{
    [TestFixture]
    public class CurrencyServiceTests
    {
        private GameState _state;
        private CurrencyService _currency;

        [SetUp]
        public void SetUp()
        {
            _state = TestFactory.NewGameState();
            _currency = new CurrencyService(_state.currency);
        }

        [Test]
        public void Get_ReturnsZeroForUnknownCurrency()
            => Assert.AreEqual(0, _currency.Get(CurrencyType.Crystal));

        [Test]
        public void AddCurrency_Works()
        {
            // Регрессия на инвертированное условие: раньше возвращало false, как только валюта существовала (T-01)
            Assert.IsTrue(_currency.AddCurrency(CurrencyType.Coin, 50));
            Assert.AreEqual(50, _currency.Get(CurrencyType.Coin));

            Assert.IsTrue(_currency.AddCurrency(CurrencyType.Coin, 25));
            Assert.AreEqual(75, _currency.Get(CurrencyType.Coin));
        }

        [Test]
        public void AddCurrency_RejectsNegative()
        {
            Assert.IsFalse(_currency.AddCurrency(CurrencyType.Coin, -10));
            Assert.AreEqual(0, _currency.Get(CurrencyType.Coin));
        }

        [Test]
        public void TrySpend_FailsWhenNotEnough()
        {
            _currency.AddCurrency(CurrencyType.Coin, 30);

            Assert.IsFalse(_currency.TrySpendCurrency(CurrencyType.Coin, 31));
            Assert.AreEqual(30, _currency.Get(CurrencyType.Coin));
        }

        [Test]
        public void TrySpend_DeductsWhenEnough()
        {
            _currency.AddCurrency(CurrencyType.Coin, 30);

            Assert.IsTrue(_currency.TrySpendCurrency(CurrencyType.Coin, 30));
            Assert.AreEqual(0, _currency.Get(CurrencyType.Coin));
        }

        [Test]
        public void Changes_ReachTheSerializableData()
        {
            _currency.AddCurrency(CurrencyType.Coin, 42);
            _currency.AddCurrency(CurrencyType.Coin, 8);

            // Без подписки на ObserveReplace значения не доходили до DTO и терялись при сохранении (T-10)
            var entry = _state.currency.origin.currencies.Find(e => e.type == CurrencyType.Coin);

            Assert.IsNotNull(entry);
            Assert.AreEqual(50, entry.amount);
        }
    }

    [TestFixture]
    public class StorageServiceTests
    {
        private GameState _state;
        private StorageService _storage;

        [SetUp]
        public void SetUp()
        {
            _state = TestFactory.NewGameState();
            _storage = new StorageService(_state.storage);
        }

        [Test]
        public void AddFuel_Works()
        {
            // Регрессия на T-02
            Assert.IsTrue(_storage.AddFuel(FuelType.Twigs, 10));
            Assert.AreEqual(10, _storage.Get(FuelType.Twigs));

            Assert.IsTrue(_storage.AddFuel(FuelType.Twigs, 5));
            Assert.AreEqual(15, _storage.Get(FuelType.Twigs));
        }

        [Test]
        public void TrySpend_FailsWhenNotEnoughOrMissing()
        {
            Assert.IsFalse(_storage.TrySpendFuel(FuelType.Coal, 1));

            _storage.AddFuel(FuelType.Coal, 2);
            Assert.IsFalse(_storage.TrySpendFuel(FuelType.Coal, 3));
            Assert.AreEqual(2, _storage.Get(FuelType.Coal));
        }

        [Test]
        public void Changes_ReachTheSerializableData()
        {
            _storage.AddFuel(FuelType.Log, 7);

            var entry = _state.storage.origin.fuels.Find(f => f.type == FuelType.Log);

            Assert.IsNotNull(entry);
            Assert.AreEqual(7, entry.amount);
        }
    }

    [TestFixture]
    public class StateServiceTests
    {
        private GameState _state;
        private StateService _service;

        [SetUp]
        public void SetUp()
        {
            _state = TestFactory.NewGameState();
            _service = new StateService(_state.fireState, _state.fireStats, TestFactory.Configs());
            _service.Initialize();
        }

        [Test]
        public void Brightness_FollowsTemperatureThresholds()
        {
            _state.fireStats.Temperature.Value = 0f;
            Assert.AreEqual(FireBright.Extinguished, _state.fireState.Bright.Value);

            _state.fireStats.Temperature.Value = 15f;
            Assert.AreEqual(FireBright.AlmostOut, _state.fireState.Bright.Value);

            _state.fireStats.Temperature.Value = 45f;
            Assert.AreEqual(FireBright.Dim, _state.fireState.Bright.Value);

            _state.fireStats.Temperature.Value = 80f;
            Assert.AreEqual(FireBright.Bright, _state.fireState.Bright.Value);
        }

        [Test]
        public void Mood_IsSleepWhenHearthEmpty()
        {
            _state.fireStats.Hearth.Remove(FuelType.Straw);
            Assert.AreEqual(FireMood.Sleep, _state.fireState.Mood.Value);
        }

        [Test]
        public void Mood_IsInspiresWhenFullAndHot()
        {
            _state.fireStats.Hearth[FuelType.Log] = 5; // 5/5 = 100% заполнения
            _state.fireStats.Temperature.Value = 90f;

            Assert.AreEqual(FireMood.Inspires, _state.fireState.Mood.Value);
        }

        [Test]
        public void Mood_IsSadWhenHearthNearlyEmpty()
        {
            _state.fireStats.Hearth[FuelType.Straw] = 1; // 1/5 = 20% < 30%
            _state.fireStats.Temperature.Value = 50f;

            Assert.AreEqual(FireMood.Sad, _state.fireState.Mood.Value);
        }
    }
}
