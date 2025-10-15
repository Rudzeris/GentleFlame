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
        [SerializeField] private FuelDatabase _fuelDatabase;
        [SerializeField] private CurrencyDatabase _currencyDatabase;
        public override void InstallBindings()
        {
            SignalBusInstaller.Install(Container);

            Container.DeclareSignal<CurrencyChangedSignal>();
            Container.DeclareSignal<FuelChangedSignal>();
            Container.DeclareSignal<FireDiedSignal>();
            Container.DeclareSignal<FireFuelChangedSignal>();

            Container.BindInstance(_fuelDatabase).AsSingle();
            Container.BindInstance(_currencyDatabase).AsSingle();

            Container.Bind<IFuelService>().To<FuelService>().AsSingle();
            Container.Bind<ISaveService>().To<SaveService>().AsSingle();
            Container.Bind<IFocusService>().To<FocusService>().AsSingle();
            Container.Bind<ICurrencyService>().To<CurrencyService>().AsSingle();
        }
    }
}
