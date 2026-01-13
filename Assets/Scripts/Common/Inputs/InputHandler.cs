using System;

namespace Common.Inputs
{
    public class InputHandler : IDisposable
    {
        public InputActions.CameraActions CameraActions { get; private set; }
        private InputActions InputActions { get; set; }

        public InputHandler()
        {
            InputActions = new InputActions();
            CameraActions = InputActions.Camera;
            EnableActions();
        }
        
        private void EnableActions()
        {
            InputActions.Enable();
        }

        private void DisableActions()
        {
            InputActions.Disable();
        }

        public void Dispose()
        {
            DisableActions();
            InputActions?.Dispose();
        }
    }
}