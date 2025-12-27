using Burst.Boids.Struct;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace Burst.Boids.Jobs
{
    [BurstCompile]
    public struct BoundsJob : IJobParallelFor
    {
        [ReadOnly]
        public NativeArray<float3> Positions;
        [ReadOnly]
        public NativeArray<float3> Velocities;
        [WriteOnly]
        public NativeArray<float3> BoundsAccelerations;
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
            
            var distanceSq = math.lengthsq(offset);
            var limitSq = radius * radius * 0.9f * 0.9f;
            
            var steer = float3.zero;
            
            if (distanceSq > limitSq)
            {
                var desired = 
                    math.normalizesafe(center - position) * MaxSpeed;
                
                steer = 
                    (desired - Velocities[index])
                    * BoundsParameters.BoundsAvoidanceWeight;
            }
            
            BoundsAccelerations[index] = steer;
        }
    }
}