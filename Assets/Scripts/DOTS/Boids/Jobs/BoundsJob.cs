using DOTS.Boids.Components;
using DOTS.Boids.Components.Boid;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace DOTS.Boids.Jobs
{
    [BurstCompile]
    public partial struct BoundsJob : IJobEntity
    {
        public BoundsParameters BoundsParameters;
        public float MaxSpeed;

        public void Execute(
            in Boid tag,
            ref BoundsAcceleration boundsAcceleration,
            in LocalTransform transform,
            in Velocity velocity)
        {
            var center = BoundsParameters.BoundsCenter;
            var radius = BoundsParameters.BoundsRadius;

            var position = transform.Position;
            var offset = position - center;

            var distanceSq = math.lengthsq(offset);
            var limitSq = radius * radius * 0.9f * 0.9f;

            var steer = float3.zero;

            if (distanceSq > limitSq)
            {
                var desired =
                    math.normalizesafe(center - position) * MaxSpeed;

                steer =
                    (desired - velocity.Value)
                    * BoundsParameters.BoundsAvoidanceWeight;
            }

            boundsAcceleration.Value = steer;
        }
    }
}