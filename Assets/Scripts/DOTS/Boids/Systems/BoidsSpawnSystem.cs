using DOTS.Boids.Components;
using DOTS.Boids.Components.Boid;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace DOTS.Boids.Systems
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial struct BoidsSpawnSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<MovementParameters>();
            state.RequireForUpdate<BoidsSpawner>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var movementParameters = SystemAPI.GetSingleton<MovementParameters>();
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

            foreach (var (spawner, entity) in
                     SystemAPI.Query<RefRO<BoidsSpawner>>().WithEntityAccess())
            {
                for (var i = 0; i < spawner.ValueRO.Count; i++)
                {
                    var boid = ecb.Instantiate(spawner.ValueRO.BoidPrefab);
                    var pos = RandomPosition(spawner.ValueRO.SpawnRadius);
                    ecb.AddComponent<Boid>(boid);
                    ecb.SetComponent(boid, LocalTransform.FromPosition(pos));
                    
                    ecb.AddComponent(boid, new Velocity { Value = RandomVelocity(movementParameters.MaxSpeed) });
                    ecb.AddComponent(boid, new SteeringAcceleration { Value = float3.zero });
                    ecb.AddComponent(boid, new BoundsAcceleration { Value = float3.zero });
                }

                ecb.DestroyEntity(entity);
            }

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }

        private static float3 RandomPosition(float radius)
        {
            return UnityEngine.Random.insideUnitSphere * radius;
        }        
        
        private static float3 RandomVelocity(float maxSpeed)
        {
            return UnityEngine.Random.insideUnitSphere.normalized * (maxSpeed * 0.5f);
        }
    }
}