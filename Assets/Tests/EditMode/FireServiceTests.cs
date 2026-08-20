using Assets.Scripts.Core.Enums;
using Assets.Scripts.Core.Proxy;
using Assets.Scripts.Core.Services;
using NUnit.Framework;
using UniRx;

namespace Assets.Tests.EditMode
{
    [TestFixture]
    public class FireServiceTests
    {
        private GameState _state;
        private FireStats _stats;
        private FireService _fire;

        [SetUp]
        public void SetUp()
        {
            _state = TestFactory.NewGameState(capacity: 5);
            _stats = _state.fireStats;
            _fire = new FireService(_stats, TestFactory.Configs());
        }

        [Test]
        public void AddFuel_RespectsCapacity()
        {
            Assert.IsTrue(_fire.TryAddFuel(FuelType.Straw, 5));
            Assert.AreEqual(5, _stats.FuelAmount.Value);

            Assert.IsFalse(_fire.TryAddFuel(FuelType.Straw, 1), "Очаг переполнен — добавление должно отклоняться");
            Assert.AreEqual(5, _stats.FuelAmount.Value);
        }

        [Test]
        public void AddFuel_RejectsUnknownTypeAndNonPositiveCount()
        {
            Assert.IsFalse(_fire.TryAddFuel(FuelType.Coal, 1), "Для Coal нет FuelConfig в тестовом наборе");
            Assert.IsFalse(_fire.TryAddFuel(FuelType.Straw, 0));
            Assert.IsFalse(_fire.TryAddFuel(FuelType.Straw, -3));
            Assert.AreEqual(0, _stats.FuelAmount.Value);
        }

        [Test]
        public void TargetTemperature_IsMassWeightedAverage()
        {
            _fire.TryAddFuel(FuelType.Straw, 3);   // 20°
            _fire.TryAddFuel(FuelType.Log, 1);     // 60°

            // (20*3 + 60*1) / 4 = 30 — одна соломинка больше не «перебивает» очаг (T-08)
            Assert.AreEqual(30d, _stats.TargetTemperature.Value, 0.001d);
        }

        [Test]
        public void TargetTemperature_IsZeroWhenHearthEmpty()
        {
            _fire.TryAddFuel(FuelType.Straw, 1);
            Assert.IsTrue(_fire.TrySpendFuel(1));

            Assert.AreEqual(0d, _stats.TargetTemperature.Value, 0.001d);
        }

        [Test]
        public void Burning_ConsumesColdestFuelFirst()
        {
            _fire.TryAddFuel(FuelType.Straw, 1);
            _fire.TryAddFuel(FuelType.Log, 1);

            _fire.Tick(30f); // ровно время горения соломы

            Assert.AreEqual(1, _stats.FuelAmount.Value);
            Assert.IsFalse(_stats.Hearth.ContainsKey(FuelType.Straw), "Солома должна прогореть раньше полена");
            Assert.AreEqual(60d, _stats.TargetTemperature.Value, 0.001d);
        }

        [Test]
        public void Burning_HandlesMultipleUnitsInOneLargeTick()
        {
            _fire.TryAddFuel(FuelType.Straw, 3); // 3 * 30 с = 90 с

            _fire.Tick(95f);

            Assert.AreEqual(0, _stats.FuelAmount.Value);
            Assert.AreEqual(0d, _stats.BurnTimeLeft.Value, 0.001d);
        }

        [Test]
        public void BurnTime_ComesFromConfigNotHardcoded()
        {
            _fire.TryAddFuel(FuelType.Log, 1);
            Assert.AreEqual(180d, _stats.BurnTimeLeft.Value, 0.001d);
        }

        [Test]
        public void TrySpendFuel_NeverGoesNegative()
        {
            _fire.TryAddFuel(FuelType.Straw, 2);

            Assert.IsFalse(_fire.TrySpendFuel(3), "Нельзя забрать больше, чем лежит в очаге");
            Assert.AreEqual(2, _stats.FuelAmount.Value);

            Assert.IsFalse(_fire.TrySpendFuel(0));
            Assert.IsTrue(_fire.TrySpendFuel(2));
            Assert.AreEqual(0, _stats.FuelAmount.Value);
        }

        [Test]
        public void Temperature_HeatsGraduallyTowardsTarget()
        {
            _fire.TryAddFuel(FuelType.Log, 1); // цель 60°

            _fire.Tick(1f);

            // Lerp(0, 60, heatingRate 0.30) = 18
            Assert.AreEqual(18d, _stats.Temperature.Value, 0.01d);
            Assert.Less(_stats.Temperature.Value, 60d, "Температура не должна прыгать сразу к цели");
        }

        [Test]
        public void Temperature_CoolsSlowerThanItHeats()
        {
            _fire.TryAddFuel(FuelType.Log, 1);
            _fire.Tick(1f);
            var afterHeating = _stats.Temperature.Value;

            _fire.TrySpendFuel(1); // очаг пуст, цель 0
            _fire.Tick(1f);

            var cooled = afterHeating - _stats.Temperature.Value;
            var heated = afterHeating;

            Assert.Less(cooled, heated, "Остывание должно быть медленнее нагрева — это прощает игрока");
        }

        [Test]
        public void FuelBurned_EventFiresWithBurnedType()
        {
            FuelType? burned = null;
            _fire.FuelBurned.Subscribe(type => burned = type);

            _fire.TryAddFuel(FuelType.Straw, 1);
            _fire.Tick(30f);

            Assert.IsTrue(burned.HasValue);
            Assert.AreEqual(FuelType.Straw, burned.Value);
        }
    }
}
