using Assets.Scripts.Core.Data;
using Assets.Scripts.Core.Proxy;
using Assets.Scripts.Core.Services;
using Assets.Scripts.Presentation.ViewModels;
using Zenject;

namespace Assets.Scripts.Installers
{
    public class FireInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            // origin Data (DTO)
            var statsData = new FireStatsData();
            var stateData = new FireStateData();

            // Proxy
            var stats = new FireStats(statsData);
            var state = new FireState(stateData);

            // Bind Proxy
            Container.Bind<FireStats>().FromInstance(stats).AsSingle();
            Container.Bind<FireState>().FromInstance(state).AsSingle();

            // Bind Service
            Container.Bind<FireService>().AsSingle();

            // Bind ViewModel
            Container.Bind<FireStatsViewModel>().AsSingle();
        }
    }
}
