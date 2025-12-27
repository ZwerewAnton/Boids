// using DOTS.Boids.Components;
// using DOTS.Boids.Components.Boid;
// using Unity.Burst;
// using Unity.Collections;
// using Unity.Entities;
// using Unity.Mathematics;
//
// namespace DOTS.Boids.Systems
// {
//     [UpdateInGroup(typeof(InitializationSystemGroup))]
//     public partial struct BoidInitializationSystem : ISystem
//     {
//         public NativeArray<float3> SteeringAccelerations;
//         public NativeArray<float3> BoundsAccelerations;
//
//         [BurstCompile]
//         public void OnUpdate(ref SystemState state)
//         {
//             var boidQuery = SystemAPI.QueryBuilder().WithAll<BoidVelocity>().Build();
//             var count = boidQuery.CalculateEntityCount();
//
//             if (count == 0)
//                 return;
//
//             SteeringAccelerations = new NativeArray<float3>(count, Allocator.Persistent);
//             BoundsAccelerations = new NativeArray<float3>(count, Allocator.Persistent);
//
//             state.Enabled = false;
//         }
//
//         [BurstCompile]
//         public void OnDestroy(ref SystemState state)
//         {
//             SteeringAccelerations.Dispose(state.Dependency);
//             BoundsAccelerations.Dispose(state.Dependency);
//         }
//     }
// }