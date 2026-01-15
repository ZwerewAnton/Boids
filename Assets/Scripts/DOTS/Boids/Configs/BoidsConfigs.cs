using UnityEngine;

namespace DOTS.Boids.Configs
{
    [CreateAssetMenu(fileName = "BoidsConfigs", menuName = "My Assets/Configs/Boids Configs (DOTS)")]
    public class BoidsConfigs : ScriptableObject
    {
        [Header("Prefabs & Spawn")]
        [SerializeField]
        public GameObject boidPrefab;
        [SerializeField]
        public int count = 100;
        [SerializeField]
        public float spawnRadius = 20f;

        [Header("Global Flocking Parameters")]
        [SerializeField]
        public float neighborRadius = 3f;
        [SerializeField, Range(0f, 5f)]
        public float cohesionWeight = 1f;
        [SerializeField, Range(0f, 5f)]
        public float alignmentWeight = 1f;
        [SerializeField, Range(0f, 5f)]
        public float separationWeight = 1.5f;

        [Header("Motion Limits")]
        [SerializeField]
        public float maxSpeed = 5f;
        [SerializeField]
        public float maxForce = 5f;

        [Header("Bounds")]
        [SerializeField]
        public Vector3 boundsCenter = Vector3.zero;
        [SerializeField]
        public float boundsRadius = 30f;
        [SerializeField]
        public float boundsAvoidanceWeight = 10f;
    }
}