using Assets.Scripts.Core.Enums;
using Assets.Scripts.Core.Proxy;
using Assets.Scripts.Core.Services;
using NUnit.Framework;

namespace Assets.Tests.EditMode
{
    [TestFixture]
    public class ProgressServiceTests
    {
        private GameState _state;
        private ProgressService _progress;

        [SetUp]
        public void SetUp()
        {
            _state = TestFactory.NewGameState();
            _progress = new ProgressService(_state.progression, _state.fireStats, TestFactory.Configs());
            _progress.Initialize();
        }

        [Test]
        public void AddExp_ActuallyAddsThePassedAmount()
        {
            // Прежняя реализация игнорировала аргумент целиком (T-03)
            _progress.AddExp(15);
            Assert.AreEqual(15, _state.progression.Exp.Value);
            Assert.AreEqual(1, _state.progression.Level.Value);
        }

        [Test]
        public void AddExp_LevelsUpOnExactThreshold()
        {
            // cost(1) = 20 * 1^2 = 20; сравнение должно быть >=, а не > (T-03)
            _progress.AddExp(20);

            Assert.AreEqual(2, _state.progression.Level.Value);
            Assert.AreEqual(0, _state.progression.Exp.Value);
        }

        [Test]
        public void AddExp_CascadesThroughMultipleLevels()
        {
            // 100 = 20 (L1→2) + 80 (L2→3)
            _progress.AddExp(100);

            Assert.AreEqual(3, _state.progression.Level.Value);
            Assert.AreEqual(0, _state.progression.Exp.Value);
        }

        [Test]
        public void AddExp_IgnoresZeroAndNegative()
        {
            _progress.AddExp(0);
            _progress.AddExp(-50);

            Assert.AreEqual(1, _state.progression.Level.Value);
            Assert.AreEqual(0, _state.progression.Exp.Value);
        }

        [Test]
        public void AddExp_DoesNotGrowCapacityByItself()
        {
            var before = _state.fireStats.MaxFuelCapacity.Value;

            _progress.AddExp(100);

            // Раньше ёмкость росла на +2 при каждом вызове AddExp (T-04)
            Assert.AreEqual(before, _state.fireStats.MaxFuelCapacity.Value);
        }

        [Test]
        public void Stage_AdvancesAtConfiguredLevel()
        {
            Assert.AreEqual(FireStage.Straw, _state.progression.Stage.Value);

            // 20 + 80 + 180 + 320 = 600 опыта -> ровно 5 уровень
            _progress.AddExp(600);

            Assert.AreEqual(5, _state.progression.Level.Value);
            Assert.AreEqual(FireStage.Wood, _state.progression.Stage.Value);
        }

        [Test]
        public void Initialize_SetsBaseCapacityForNewGame()
        {
            var fresh = TestFactory.NewGameState(capacity: 0);
            var service = new ProgressService(fresh.progression, fresh.fireStats, TestFactory.Configs());

            service.Initialize();

            Assert.AreEqual(5, fresh.fireStats.MaxFuelCapacity.Value);
        }
    }

    [TestFixture]
    public class ExperienceServiceTests
    {
        [Test]
        public void BurnedFuel_GrantsExperience()
        {
            var state = TestFactory.NewGameState(capacity: 5);
            var configs = TestFactory.Configs();

            var fire = new FireService(state.fireStats, configs);
            var progress = new ProgressService(state.progression, state.fireStats, configs);
            progress.Initialize();

            var stateService = new StateService(state.fireState, state.fireStats, configs);
            stateService.Initialize();

            var experience = new ExperienceService(fire, progress, state.fireState, configs);
            experience.Initialize();

            fire.TryAddFuel(FuelType.Straw, 2);
            fire.Tick(30f);
            fire.Tick(30f);

            Assert.Greater(state.progression.Exp.Value + (state.progression.Level.Value - 1) * 20, 0,
                "Догоревшее топливо обязано давать опыт");

            experience.Dispose();
        }

        [Test]
        public void NoBurning_GrantsNothing()
        {
            var state = TestFactory.NewGameState(capacity: 5);
            var configs = TestFactory.Configs();

            var fire = new FireService(state.fireStats, configs);
            var progress = new ProgressService(state.progression, state.fireStats, configs);
            progress.Initialize();

            var experience = new ExperienceService(fire, progress, state.fireState, configs);
            experience.Initialize();

            fire.Tick(60f);

            Assert.AreEqual(0, state.progression.Exp.Value);
            Assert.AreEqual(1, state.progression.Level.Value);

            experience.Dispose();
        }
    }
}
