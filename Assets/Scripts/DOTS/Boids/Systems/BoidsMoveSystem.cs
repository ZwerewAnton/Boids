using DOTS.Boids.Components;
using DOTS.Boids.Jobs;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;

namespace DOTS.Boids.Systems
{
    [UpdateAfter(typeof(BoidsBoundsSystem))]
    [UpdateAfter(typeof(BoidsSteeringSystem))]
    public partial struct BoidsMoveSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<MovementParameters>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var movement = SystemAPI.GetSingleton<MovementParameters>();
            var deltaTime = SystemAPI.Time.DeltaTime;
            
            var job = new MoveJob
            {
                MovementParameters = movement,
                DeltaTime = deltaTime
            };
            
            job.ScheduleParallel();
        }
    }
}