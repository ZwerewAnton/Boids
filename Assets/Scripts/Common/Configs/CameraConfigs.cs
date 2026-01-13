using UnityEngine;

namespace Common.Configs
{
    [CreateAssetMenu(fileName = "CameraConfigs", menuName = "My Assets/Configs/Camera Configs")]
    public class CameraConfigs : ScriptableObject
    {
        [Header("Orbit Settings")]
        public Quaternion startRotation;
        public float distance = 30f;
        public float minDistance = 20f;
        public float maxDistance = 40f;
        
        public float xSpeed = 300f;
        public float ySpeed = 300f;
        
        [Header("Zoom Settings")]
        public float zoomSpeed = 1f;
        
        [Header("Smooth Settings")]
        public float mouseSmooth = 10f;
        public float zoomSmooth = 5f;
    }
}