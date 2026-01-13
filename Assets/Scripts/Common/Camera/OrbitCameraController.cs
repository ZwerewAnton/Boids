using Common.Camera.Movement;
using UnityEngine;
using Zenject;

namespace Common.Camera
{
    public class OrbitCameraController : MonoBehaviour
    {
        private OrbitCameraMovement _cameraMovement;
        private Transform _cameraTransform;

        [Inject]
        private void Construct(OrbitCameraMovement cameraMovement)
        {
            _cameraMovement = cameraMovement;
        }
        
        private void LateUpdate()
        {
            UpdateCameraMovement();
        }

        private void UpdateCameraMovement()
        {
            var result = _cameraMovement.CalculateMovement();

            transform.rotation = result.Rotation;
            transform.position = result.Position;
        }
    }
}