using Boids.Struct;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace Boids.Jobs
{
    public struct BoundsJob : IJobParallelFor
    {
        [ReadOnly]
        public NativeArray<Vector3> Positions;
        [ReadOnly]
        public NativeArray<Vector3> Velocities;
        [WriteOnly]
        public NativeArray<Vector3> BoundsAccelerations;
        [ReadOnly]
        public BoundsParameters BoundsParameters;
        [ReadOnly]
        public float MaxSpeed;
        
        public void Execute(int index)
        {
            var center = BoundsParameters.BoundsCenter;
            var radius = BoundsParameters.BoundsRadius;
            var position = Positions[index];

            var offset = position - center;
            var distance2 = offset.sqrMagnitude;
            var limit2 = radius * radius * 0.9f * 0.9f;
            var steer = Vector3.zero;
            if (distance2 > limit2)
            {
                var desired = (center - position).normalized * MaxSpeed;
                steer = (desired - Velocities[index]) * BoundsParameters.BoundsAvoidanceWeight;
            }
            BoundsAccelerations[index] = steer;
        }
    }
}