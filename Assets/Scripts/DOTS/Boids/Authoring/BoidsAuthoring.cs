using DOTS.Boids.Components;
using DOTS.Boids.Components.Parameters;
using DOTS.Boids.Configs;
using Unity.Entities;
using UnityEngine;

namespace DOTS.Boids.Authoring
{
    public class BoidsAuthoring : MonoBehaviour
    {
        [SerializeField]
        private BoidsConfigs boidsConfigs;

        private class BoidsAuthoringBaker : Baker<BoidsAuthoring>
        {
            public override void Bake(BoidsAuthoring authoring)
            {
                var spawnerEntity = CreateAdditionalEntity(TransformUsageFlags.None);
                AddComponent(spawnerEntity, new BoidsSpawner
                {
                    Count = authoring.boidsConfigs.count,
                    SpawnRadius = authoring.boidsConfigs.spawnRadius,
                    BoidPrefab = GetEntity(authoring.boidsConfigs.boidPrefab, TransformUsageFlags.Dynamic)
                });

                var boundsParamsEntity = CreateAdditionalEntity(TransformUsageFlags.None);
                AddComponent(boundsParamsEntity, new BoundsParameters
                {
                    BoundsCenter = authoring.boidsConfigs.boundsCenter,
                    BoundsRadius = authoring.boidsConfigs.boundsRadius,
                    BoundsAvoidanceWeight = authoring.boidsConfigs.boundsAvoidanceWeight
                });

                var flockingParamsEntity = CreateAdditionalEntity(TransformUsageFlags.None);
                AddComponent(flockingParamsEntity, new FlockingParameters
                {
                    AlignmentWeight = authoring.boidsConfigs.alignmentWeight,
                    CohesionWeight = authoring.boidsConfigs.cohesionWeight,
                    SeparationWeight = authoring.boidsConfigs.separationWeight,
                    PerceptionRadius = authoring.boidsConfigs.neighborRadius
                });

                var movementParamsEntity = CreateAdditionalEntity(TransformUsageFlags.None);
                AddComponent(movementParamsEntity, new MovementParameters
                {
                    Count = authoring.boidsConfigs.count,
                    MaxSpeed = authoring.boidsConfigs.maxSpeed,
                    MaxForce = authoring.boidsConfigs.maxForce
                });
            }
        }
    }
}