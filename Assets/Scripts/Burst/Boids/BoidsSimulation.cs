using Burst.Boids.Configs;
using Burst.Boids.Jobs;
using Burst.Boids.Struct;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Jobs;
using Random = UnityEngine.Random;

namespace Burst.Boids
{
    public class BoidsSimulation : MonoBehaviour
    {
        [SerializeField]
        private BoidsConfigs boidsConfigs;
        
        private BoundsParameters _boundsParameters;
        private FlockingParameters _flockingParameters;
        
        private NativeArray<float3> _positions;
        private NativeArray<float3> _velocities;
        private NativeArray<float3> _boundsAccelerations;
        private NativeArray<float3> _steeringAccelerations;
        private TransformAccessArray _transformAccessArray;

        private void Start()
        {
            var transforms = SpawnInitialBoids();
            InitializeDataStructs(transforms);
        }

        private void Update()
        {
            var deltaTime = Time.deltaTime;
            
            var boundsJob = new BoundsJob
            {
                BoundsAccelerations = _boundsAccelerations,
                BoundsParameters = _boundsParameters,
                Positions = _positions,
                Velocities = _velocities,
                MaxSpeed = boidsConfigs.maxSpeed
            };
            var steeringJob = new SteeringJob
            {
                Positions = _positions,
                Velocities = _velocities,
                MaxSpeed = boidsConfigs.maxSpeed,
                MaxForce = boidsConfigs.maxForce,
                FlockingParameters = _flockingParameters,
                SteeringAccelerations = _steeringAccelerations
            };
            var moveJob = new MoveJob
            {
                BoundsAccelerations = _boundsAccelerations,
                SteeringAccelerations = _steeringAccelerations,
                Velocities = _velocities,
                Positions = _positions,
                MaxSpeed = boidsConfigs.maxSpeed,
                MaxForce = boidsConfigs.maxForce,
                DeltaTime = deltaTime
            };

            var boundsHandle = boundsJob.Schedule(boidsConfigs.count, 0);
            var steeringHandle = steeringJob.Schedule(boidsConfigs.count, 0);
            var combined = JobHandle.CombineDependencies(boundsHandle, steeringHandle);
            var moveHandle = moveJob.Schedule(_transformAccessArray, combined);
            moveHandle.Complete();
        }

        private Transform[] SpawnInitialBoids()
        {
            for (var i = transform.childCount - 1; i >= 0; i--)
            {
                Destroy(transform.GetChild(i).gameObject);
            }

            var transforms = new Transform[boidsConfigs.count];

            for (var i = 0; i < boidsConfigs.count; i++)
            {
                var position = transform.position + Random.insideUnitSphere * boidsConfigs.spawnRadius;

                transforms[i] = Instantiate(boidsConfigs.boidPrefab, position, Quaternion.identity, transform).transform;
            }

            return transforms;
        }

        private void InitializeDataStructs(Transform[] transforms)
        {
            var count = boidsConfigs.count;
            _positions = new NativeArray<float3>(count, Allocator.Persistent);
            _velocities = new NativeArray<float3>(count, Allocator.Persistent);
            _boundsAccelerations = new NativeArray<float3>(count, Allocator.Persistent);
            _steeringAccelerations = new NativeArray<float3>(count, Allocator.Persistent);
            _transformAccessArray = new TransformAccessArray(transforms);

            _boundsParameters = new BoundsParameters
            {
                BoundsCenter = transform.position,
                BoundsRadius = boidsConfigs.boundsRadius,
                BoundsAvoidanceWeight = boidsConfigs.boundsAvoidanceWeight
            };
            _flockingParameters = new FlockingParameters
            {
                PerceptionRadius = boidsConfigs.neighborRadius,
                AlignmentWeight = boidsConfigs.alignmentWeight,
                CohesionWeight = boidsConfigs.cohesionWeight,
                SeparationWeight = boidsConfigs.separationWeight
            };
            
            for (var i = 0; i < count; i++)
            {
                _velocities[i] = Random.insideUnitSphere.normalized * (boidsConfigs.maxSpeed * 0.5f);
                _positions[i] = transforms[i].position;
            }
        }
        
        private void OnDestroy()
        {
            _positions.Dispose();
            _velocities.Dispose();
            _boundsAccelerations.Dispose();
            _steeringAccelerations.Dispose();
            _transformAccessArray.Dispose();
        }
    }
}
