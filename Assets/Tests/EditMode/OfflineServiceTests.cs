using Assets.Scripts.Core.Enums;
using Assets.Scripts.Core.Proxy;
using Assets.Scripts.Core.Services;
using NUnit.Framework;

namespace Assets.Tests.EditMode
{
    [TestFixture]
    public class OfflineServiceTests
    {
        private GameState _state;
        private ConfigService _configs;
        private FireService _fire;
        private CurrencyService _currency;
        private OfflineService _offline;

        [SetUp]
        public void SetUp()
        {
            _state = TestFactory.NewGameState(capacity: 10);
            _configs = TestFactory.Configs();

            _fire = new FireService(_state.fireStats, _configs);
            _currency = new CurrencyService(_state.currency);

            var stateService = new StateService(_state.fireState, _state.fireStats, _configs);
            stateService.Initialize();

            _offline = new OfflineService(_state.fireStats, _state.fireState, _state.progression,
                _fire, _currency, _configs);
        }

        [Test]
        public void ZeroAway_EarnsNothing()
        {
            _fire.TryAddFuel(FuelType.Log, 5);

            var report = _offline.Simulate(0d);

            Assert.AreEqual(0, report.WarmthEarned);
            Assert.AreEqual(0d, report.SimulatedSeconds, 0.001d);
            Assert.IsFalse(report.WasCapped);
        }

        [Test]
        public void NegativeAway_IsTreatedAsZero()
        {
            _fire.TryAddFuel(FuelType.Log, 5);

            var report = _offline.Simulate(-500d);

            Assert.AreEqual(0, report.WarmthEarned);
            Assert.AreEqual(0d, report.AwaySeconds, 0.001d);
        }

        [Test]
        public void BurningFire_EarnsWarmth()
        {
            _fire.TryAddFuel(FuelType.Log, 5); // 5 * 180 с = 900 с горения

            var report = _offline.Simulate(600d);

            Assert.Greater(report.WarmthEarned, 0);
            Assert.Greater(report.FuelBurned, 0);
            Assert.AreEqual(600d, report.SimulatedSeconds, 0.001d);
        }

        [Test]
        public void AwayLongerThanCap_IsCapped()
        {
            _fire.TryAddFuel(FuelType.Log, 10);

            // Стадия Straw: кап 1 час
            var report = _offline.Simulate(10d * 3600d);

            Assert.IsTrue(report.WasCapped);
            Assert.AreEqual(3600d, report.SimulatedSeconds, 0.001d);
            Assert.AreEqual(36000d, report.AwaySeconds, 0.001d);
        }

        [Test]
        public void EmptyHearth_EarnsNothing()
        {
            var report = _offline.Simulate(3600d);

            Assert.AreEqual(0, report.WarmthEarned);
            Assert.AreEqual(0, report.FuelBurned);
        }

        [Test]
        public void OfflineIncome_IsHalvedRelativeToOnline()
        {
            _fire.TryAddFuel(FuelType.Log, 5);

            var report = _offline.Simulate(600d);
            var offlineEarned = report.WarmthEarned;

            // Тот же отрезок онлайн, с чистого состояния
            var online = TestFactory.NewGameState(capacity: 10);
            var onlineFire = new FireService(online.fireStats, _configs);
            var onlineCurrency = new CurrencyService(online.currency);
            var onlineState = new StateService(online.fireState, online.fireStats, _configs);
            onlineState.Initialize();

            var warmth = new WarmthService(online.fireStats, online.fireState, online.progression,
                onlineCurrency, _configs);

            onlineFire.TryAddFuel(FuelType.Log, 5);

            for (var i = 0; i < 600; i++)
            {
                onlineFire.Tick(1f);
                warmth.Tick(1f);
            }

            var onlineEarned = onlineCurrency.Get(CurrencyType.Coin);

            Assert.Greater(onlineEarned, 0);
            Assert.AreEqual(onlineEarned * 0.5d, offlineEarned, onlineEarned * 0.05d + 1d,
                "Офлайн должен давать примерно половину онлайн-дохода");
        }
    }

    [TestFixture]
    public class GameTickServiceTests
    {
        private class CountingListener : IGameTickListener
        {
            public int Ticks;
            public void Tick(float deltaTime) => Ticks++;
        }

        [Test]
        public void Tick_DeliversToRegisteredListeners()
        {
            var service = new GameTickService();
            var listener = new CountingListener();

            service.Register(listener);
            service.Tick(1f);
            service.Tick(1f);

            Assert.AreEqual(2, listener.Ticks);
        }

        [Test]
        public void Tick_SkipsWhenPausedOrZeroDelta()
        {
            var service = new GameTickService();
            var listener = new CountingListener();
            service.Register(listener);

            service.IsPaused = true;
            service.Tick(1f);
            service.IsPaused = false;
            service.Tick(0f);

            Assert.AreEqual(0, listener.Ticks);
        }

        [Test]
        public void Unregister_StopsDelivery()
        {
            var service = new GameTickService();
            var listener = new CountingListener();

            service.Register(listener);
            service.Tick(1f);
            service.Unregister(listener);
            service.Tick(1f);

            Assert.AreEqual(1, listener.Ticks);
        }

        [Test]
        public void RegisteringDuringTick_DoesNotCorruptIteration()
        {
            var service = new GameTickService();
            var added = new CountingListener();

            service.Register(new SelfRegisteringListener(service, added));
            service.Tick(1f); // регистрация происходит внутри тика
            service.Tick(1f);

            Assert.AreEqual(1, added.Ticks);
        }

        private class SelfRegisteringListener : IGameTickListener
        {
            private readonly GameTickService _service;
            private readonly IGameTickListener _toAdd;
            private bool _done;

            public SelfRegisteringListener(GameTickService service, IGameTickListener toAdd)
            {
                _service = service;
                _toAdd = toAdd;
            }

            public void Tick(float deltaTime)
            {
                if (_done) return;
                _done = true;
                _service.Register(_toAdd);
            }
        }
    }
}
