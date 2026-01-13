using Common.Camera.Input;
using Common.Configs;
using UnityEngine;
using Zenject;

namespace Common.Camera.Movement
{
    public class OrbitCameraMovement
    {
        private readonly CameraConfigs _configs;
        private readonly ICameraInputProvider _input;

        private Vector2 _eulerAngles;
        private Vector2 _smoothedDelta;
        private float _currentDistance;
        private float _currentZoomOffset;
        
        public float DesiredZoomOffset { get; private set; }

        [Inject]
        public OrbitCameraMovement(CameraConfigs configs, ICameraInputProvider inputProvider)
        {
            _configs = configs;
            _input = inputProvider;
            InitializeParameters();
        }

        public CameraMovementResult CalculateMovement()
        {
            _input.UpdateInput();

            var delta = _input.RotationDelta;
            _smoothedDelta = Vector2.Lerp(_smoothedDelta, delta, Time.deltaTime * _configs.mouseSmooth);
            _eulerAngles.y += _smoothedDelta.x * _configs.xSpeed * Time.deltaTime;
            _eulerAngles.x -= _smoothedDelta.y * _configs.ySpeed * Time.deltaTime;
            
            var zoomDelta = _input.ZoomDelta;
            if (Mathf.Abs(zoomDelta) > 0.0001f)
            {
                var maxDistance = _configs.maxDistance;
                var minDistance = _configs.minDistance;

                DesiredZoomOffset -= zoomDelta * (maxDistance - minDistance) * _configs.zoomSpeed;
                DesiredZoomOffset = Mathf.Clamp(
                    DesiredZoomOffset,
                    minDistance - _configs.distance,
                    maxDistance - _configs.distance
                );
            }

            _currentZoomOffset = Mathf.Lerp(
                _currentZoomOffset, DesiredZoomOffset, 
                Time.deltaTime * _configs.zoomSmooth
            );
            _currentDistance = _configs.distance + _currentZoomOffset;

            var rotation = Quaternion.Euler(_eulerAngles.x, _eulerAngles.y, 0);
            var negativeDistance = new Vector3(0, 0, -_currentDistance);
            var position = rotation * negativeDistance;
            
            return new CameraMovementResult(position, rotation);
        }

        public void ResetCamera()
        {
            DesiredZoomOffset = 0f;
        }

        private void InitializeParameters()
        {
            _eulerAngles = _configs.startRotation.eulerAngles;
            _currentDistance = _configs.distance;
            _currentZoomOffset = DesiredZoomOffset = 0f;
        }
    }
}