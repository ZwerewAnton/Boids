using DOTS.Boids.Components;
using DOTS.Boids.Components.Boid;
using DOTS.Boids.Jobs;
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using UnityEngine;

namespace DOTS.Boids.Systems
{
    public partial struct BoidsSteeringSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<MovementParameters>();
            state.RequireForUpdate<FlockingParameters>();
            state.RequireForUpdate<SpatialHash>();
            state.RequireForUpdate<SpatialHashJobHandle>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var spatial = SystemAPI.GetSingletonRW<SpatialHash>();

            spatial.ValueRW.BoidCount = 0;
            spatial.ValueRW.CellToBoid.Clear();

            foreach (var (transform, velocity) in
                     SystemAPI.Query<RefRO<LocalTransform>, RefRO<Velocity>>()
                         .WithAll<Boid>())
            {
                var index = spatial.ValueRW.BoidCount++;

                spatial.ValueRW.Positions[index] = transform.ValueRO.Position;
                spatial.ValueRW.Velocities[index] = velocity.ValueRO.Value;
            }

            var job = new SpatialHashBuildJob
            {
                Positions = spatial.ValueRO.Positions,
                BoidCount = spatial.ValueRO.BoidCount,
                CellSize = spatial.ValueRO.CellSize,
                CellToBoid = spatial.ValueRW.CellToBoid.AsParallelWriter()
            };

            var hashJobHandle = job.Schedule(
                spatial.ValueRO.BoidCount,
                64,
                state.Dependency);
            
            var movementParameters = SystemAPI.GetSingleton<MovementParameters>();
            var flockingParameters = SystemAPI.GetSingleton<FlockingParameters>();
            var spatialHash = SystemAPI.GetSingleton<SpatialHash>();
            
            var steeringJob = new SteeringJob
            {
                Positions = spatialHash.Positions,
                CellToBoid = spatialHash.CellToBoid,
                CellSize = flockingParameters.PerceptionRadius,
                FlockingParameters = flockingParameters,
                MovementParameters = movementParameters,
            };
            
            var steeringHandle = steeringJob.ScheduleParallel(hashJobHandle);

            state.Dependency = steeringHandle;
        }
    }
}