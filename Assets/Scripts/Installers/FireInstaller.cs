using Assets.Scripts.Core.Proxy;
using Assets.Scripts.Core.Services;
using Assets.Scripts.Presentation.ViewModels;
using Zenject;

namespace Assets.Scripts.Installers
{
    /// <summary>
    /// Композиционный корень сцены.
    /// Раньше здесь создавались собственные DTO мимо сохранения, а половина сервисов
    /// вообще не биндилась и была мёртвым кодом (T-07).
    /// </summary>
    public class FireInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            var time = new TimeService(new SystemTimeProvider());
            var configs = new ConfigService();
            var save = new SaveService(time);

            // Состояние читается из сейва до биндингов: прокси должны ссылаться
            // на те же объекты, которые потом уйдут на диск.
            var gameState = save.LoadGameState();

            Container.Bind<TimeService>().FromInstance(time).AsSingle();
            Container.Bind<ConfigService>().FromInstance(configs).AsSingle();
            Container.Bind<SaveService>().FromInstance(save).AsSingle();
            Container.Bind<ISaveProvider>().FromInstance(save).AsSingle();

            Container.Bind<GameState>().FromInstance(gameState).AsSingle();
            Container.Bind<FireStats>().FromInstance(gameState.fireStats).AsSingle();
            Container.Bind<FireState>().FromInstance(gameState.fireState).AsSingle();
            Container.Bind<Currency>().FromInstance(gameState.currency).AsSingle();
            Container.Bind<Storage>().FromInstance(gameState.storage).AsSingle();
            Container.Bind<Progression>().FromInstance(gameState.progression).AsSingle();

            Container.Bind<GameTickService>().AsSingle();
            Container.Bind<CurrencyService>().AsSingle();
            Container.Bind<StorageService>().AsSingle();
            Container.Bind<ProgressService>().AsSingle();
            Container.Bind<WarmthService>().AsSingle();
            Container.Bind<OfflineService>().AsSingle();

            // Эти три сервиса держат подписки UniRx: биндим через интерфейсы,
            // иначе Zenject не вызовет Dispose и подписки переживут сцену (T-12).
            Container.BindInterfacesAndSelfTo<FireService>().AsSingle();
            Container.BindInterfacesAndSelfTo<StateService>().AsSingle();
            Container.BindInterfacesAndSelfTo<ExperienceService>().AsSingle();

            Container.Bind<FireStatsViewModel>().AsSingle();

            Container.BindInterfacesAndSelfTo<GameBootstrap>().AsSingle().NonLazy();
            Container.BindInterfacesTo<GameTickDriver>().AsSingle().NonLazy();
            Container.BindInterfacesTo<AutoSaveDriver>().AsSingle().NonLazy();
        }
    }
}
