using DOTS.Boids.Components;
using DOTS.Boids.Jobs;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;

namespace DOTS.Boids.Systems
{
    public partial struct BoidsBoundsSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BoundsParameters>();
            state.RequireForUpdate<MovementParameters>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var bounds = SystemAPI.GetSingleton<BoundsParameters>();
            var movement = SystemAPI.GetSingleton<MovementParameters>();

            var job = new BoundsJob
            {
                BoundsParameters = bounds,
                MaxSpeed = movement.MaxSpeed
            };

            job.ScheduleParallel();
        }
    }
}