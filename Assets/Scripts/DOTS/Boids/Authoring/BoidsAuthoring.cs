using DOTS.Boids.Components;
using DOTS.Boids.Components.Parameters;
using Unity.Entities;
using UnityEngine;

namespace DOTS.Boids.Authoring
{
    public class BoidsAuthoring : MonoBehaviour
    {
        [Header("Prefabs & Spawn")]
        [SerializeField]
        private GameObject boidPrefab;
        [SerializeField]
        private int count = 100;
        [SerializeField]
        public float spawnRadius = 20f;

        [Header("Global flocking parameters")]
        [SerializeField]
        private float neighborRadius = 3f;
        [SerializeField, Range(0f, 5f)]
        private float cohesionWeight = 1f;
        [SerializeField, Range(0f, 5f)]
        private float alignmentWeight = 1f;
        [SerializeField, Range(0f, 5f)]
        private float separationWeight = 1.5f;

        [Header("Motion limits")]
        [SerializeField]
        private float maxSpeed = 5f;
        [SerializeField]
        private float maxForce = 5f;

        [Header("Bounds")]
        [SerializeField]
        private Vector3 boundsCenter = Vector3.zero;
        [SerializeField]
        private float boundsRadius = 30f;
        [SerializeField]
        private float boundsAvoidanceWeight = 10f;

        private class BoidsAuthoringBaker : Baker<BoidsAuthoring>
        {
            public override void Bake(BoidsAuthoring authoring)
            {
                var spawnerEntity = CreateAdditionalEntity(TransformUsageFlags.None);
                AddComponent(spawnerEntity, new BoidsSpawner
                {
                    Count = authoring.count,
                    SpawnRadius = authoring.spawnRadius,
                    BoidPrefab = GetEntity(authoring.boidPrefab, TransformUsageFlags.Dynamic)
                });

                var boundsParamsEntity = CreateAdditionalEntity(TransformUsageFlags.None);
                AddComponent(boundsParamsEntity, new BoundsParameters
                {
                    BoundsCenter = authoring.boundsCenter,
                    BoundsRadius = authoring.boundsRadius,
                    BoundsAvoidanceWeight = authoring.boundsAvoidanceWeight
                });

                var flockingParamsEntity = CreateAdditionalEntity(TransformUsageFlags.None);
                AddComponent(flockingParamsEntity, new FlockingParameters
                {
                    AlignmentWeight = authoring.alignmentWeight,
                    CohesionWeight = authoring.cohesionWeight,
                    SeparationWeight = authoring.separationWeight,
                    PerceptionRadius = authoring.neighborRadius
                });

                var movementParamsEntity = CreateAdditionalEntity(TransformUsageFlags.None);
                AddComponent(movementParamsEntity, new MovementParameters
                {
                    Count = authoring.count,
                    MaxSpeed = authoring.maxSpeed,
                    MaxForce = authoring.maxForce
                });
            }
        }
    }
}