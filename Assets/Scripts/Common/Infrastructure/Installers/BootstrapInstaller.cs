using Common.Configs;
using Common.Inputs;
using UnityEngine;
using Zenject;

namespace Common.Infrastructure.Installers
{
    public class BootstrapInstaller : MonoInstaller
    {
        [SerializeField] private CameraConfigs cameraConfigs;
        
        public override void InstallBindings()
        {
            BindCameraConfigs();
            BindInputHandler();
        }
        
        private void BindCameraConfigs()
        {
            Container.Bind<CameraConfigs>().FromInstance(cameraConfigs).AsSingle().NonLazy();
        }

        private void BindInputHandler()
        {
            Container.BindInterfacesAndSelfTo<InputHandler>().AsSingle().NonLazy();
        }
    }
}