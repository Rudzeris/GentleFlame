using Assets.Scripts.Configs;
using Assets.Scripts.Services;
using Assets.Scripts.Signals;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Installers
{
    [CreateAssetMenu(menuName = "Installers/ProjectInstaller")]
    public class ProjectInstaller : ScriptableObjectInstaller<ProjectInstaller>
    {
        [SerializeField] private ResourceConfig _wood;
        [SerializeField] private CurrencyConfig _coins;
        [SerializeField] private CurrencyConfig _diamonds;
        public override void InstallBindings()
        {

            SignalBusInstaller.Install(Container);

            Container.DeclareSignal<CurrencyChangedSignal>();
            Container.DeclareSignal<ResourceChangedSignal>();
            Container.DeclareSignal<FireDiedSignal>();
            Container.DeclareSignal<FireFuelChangedSignal>();

            Container.BindInstance(_wood).AsSingle();

            Container.Bind<IResourceService>().To<ResourceService>().AsSingle();
            Container.Bind<ISaveService>().To<SaveService>().AsSingle();
            Container.Bind<IFocusService>().To<FocusService>().AsSingle();
            Container.Bind<ICurrencyService>().To<CurrencyService>().AsSingle();
        }
    }
}
