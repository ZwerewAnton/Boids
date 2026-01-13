using Common.Inputs;
using UnityEngine;
using Zenject;

namespace Common.Camera.Input
{
    public class MouseCameraInputProvider : ICameraInputProvider
    {
        private const float ScrollCoefficient = 0.001f;
        private readonly InputActions.CameraActions _actions;

        public Vector2 RotationDelta { get; private set; }
        public float ZoomDelta { get; private set; }

        [Inject]
        public MouseCameraInputProvider(InputHandler inputHandler)
        {
            _actions = inputHandler.CameraActions;
        }

        public void UpdateInput()
        {
            RotationDelta = _actions.Press.IsPressed()
                ? _actions.Look.ReadValue<Vector2>()
                : Vector2.zero;

            ZoomDelta = _actions.Zoom.ReadValue<float>();
        }
    }
}