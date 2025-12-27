using DOTS.Boids.Components;
using DOTS.Boids.Components.Parameters;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace DOTS.Boids.Systems
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial struct SpatialHashInitializerSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<MovementParameters>();
            state.RequireForUpdate<FlockingParameters>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            if (SystemAPI.HasSingleton<SpatialHash>())
            {
                state.Enabled = false;
                return;
            }

            var movement = SystemAPI.GetSingleton<MovementParameters>();
            var flocking = SystemAPI.GetSingleton<FlockingParameters>();

            var entity = state.EntityManager.CreateEntity();
            state.EntityManager.AddComponentData(entity, new SpatialHash
            {
                Positions = new NativeArray<float3>(movement.Count, Allocator.Persistent),
                Velocities = new NativeArray<float3>(movement.Count, Allocator.Persistent),
                CellSize = flocking.PerceptionRadius,
                CellToBoid = new NativeParallelMultiHashMap<int, int>(movement.Count * 2, Allocator.Persistent),
                BoidCount = 0
            });

            state.Enabled = false;
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
            if (!SystemAPI.HasSingleton<SpatialHash>())
                return;

            var spatial = SystemAPI.GetSingleton<SpatialHash>();

            if (spatial.Positions.IsCreated)
                spatial.Positions.Dispose();
            if (spatial.Velocities.IsCreated)
                spatial.Velocities.Dispose();
            if (spatial.CellToBoid.IsCreated)
                spatial.CellToBoid.Dispose();
        }
    }
}