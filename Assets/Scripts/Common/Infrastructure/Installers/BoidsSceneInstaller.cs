using Common.Camera.Input;
using Common.Camera.Movement;
using Zenject;

namespace Common.Infrastructure.Installers
{
    public class BoidsSceneInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            BindCameraInputProvider();
            BindOrbitCameraMovement();
        }
        
        private void BindCameraInputProvider()
        {
            Container.Bind<ICameraInputProvider>().To<MouseCameraInputProvider>().AsSingle();
        }
        
        private void BindOrbitCameraMovement()
        {
            Container.Bind<OrbitCameraMovement>().AsSingle().NonLazy();
        }
    }
}