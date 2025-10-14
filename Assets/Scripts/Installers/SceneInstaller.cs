using Assets.Scripts.Configs;
using Assets.Scripts.Services;
using Assets.Scripts.States;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Installers
{
    public class SceneInstaller : MonoInstaller
    {
        [SerializeField] private FireConfig _fireConfig;
        public override void InstallBindings()
        {
            Container.Bind<FireState>().AsSingle();
            Container.BindInstance(_fireConfig).AsSingle();
            Container.Bind<IFireService>().To<FireService>().AsSingle()
                .OnInstantiated<IFireService>((ctx, fireService) => fireService.StartBurning()).NonLazy();
        }
    }
}
