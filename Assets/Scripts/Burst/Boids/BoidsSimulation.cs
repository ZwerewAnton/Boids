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
        [Header("Prefabs & Spawn")]
        [SerializeField]
        private GameObject boidPrefab;
        [SerializeField]
        private int count = 100;
        [SerializeField]
        private Vector3 spawnBounds = new(20, 5, 20);

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
        private float boundsRadius = 30f;
        [SerializeField]
        private float boundsAvoidanceWeight = 10f;

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
                MaxSpeed = maxSpeed
            };
            var steeringJob = new SteeringJob
            {
                Positions = _positions,
                Velocities = _velocities,
                MaxSpeed = maxSpeed,
                MaxForce = maxForce,
                FlockingParameters = _flockingParameters,
                SteeringAccelerations = _steeringAccelerations
            };
            var moveJob = new MoveJob
            {
                BoundsAccelerations = _boundsAccelerations,
                SteeringAccelerations = _steeringAccelerations,
                Velocities = _velocities,
                Positions = _positions,
                MaxSpeed = maxSpeed,
                MaxForce = maxForce,
                DeltaTime = deltaTime
            };

            var boundsHandle = boundsJob.Schedule(count, 0);
            var steeringHandle = steeringJob.Schedule(count, 0);
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

            var transforms = new Transform[count];

            for (var i = 0; i < count; i++)
            {
                var position = transform.position + new Vector3(
                    Random.Range(-spawnBounds.x, spawnBounds.x),
                    Random.Range(-spawnBounds.y, spawnBounds.y),
                    Random.Range(-spawnBounds.z, spawnBounds.z)
                );

                transforms[i] = Instantiate(boidPrefab, position, Quaternion.identity, transform).transform;
            }

            return transforms;
        }

        private void InitializeDataStructs(Transform[] transforms)
        {
            _positions = new NativeArray<float3>(count, Allocator.Persistent);
            _velocities = new NativeArray<float3>(count, Allocator.Persistent);
            _boundsAccelerations = new NativeArray<float3>(count, Allocator.Persistent);
            _steeringAccelerations = new NativeArray<float3>(count, Allocator.Persistent);
            _transformAccessArray = new TransformAccessArray(transforms);

            _boundsParameters = new BoundsParameters
            {
                BoundsCenter = transform.position,
                BoundsRadius = boundsRadius,
                BoundsAvoidanceWeight = boundsAvoidanceWeight
            };
            _flockingParameters = new FlockingParameters
            {
                PerceptionRadius = neighborRadius,
                AlignmentWeight = alignmentWeight,
                CohesionWeight = cohesionWeight,
                SeparationWeight = separationWeight
            };
            
            for (var i = 0; i < count; i++)
            {
                _velocities[i] = Random.insideUnitSphere.normalized * (maxSpeed * 0.5f);
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
