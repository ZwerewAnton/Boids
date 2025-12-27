using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Jobs;
using Utils;

namespace Boids.Jobs
{
    [BurstCompile]
    public struct MoveJob : IJobParallelForTransform
    {
        [ReadOnly]
        public NativeArray<float3> SteeringAccelerations;
        [ReadOnly]
        public NativeArray<float3> BoundsAccelerations;
        [WriteOnly]
        public NativeArray<float3> Positions;
        
        public NativeArray<float3> Velocities;
        
        public float MaxSpeed;
        public float MaxForce;
        public float DeltaTime;
        
        private const float Epsilon = 0.0001f;
        
        public void Execute(int index, TransformAccess transform)
        {
            var acceleration = SteeringAccelerations[index] + BoundsAccelerations[index];
            acceleration = MathUtils.ClampMagnitude(acceleration, MaxForce);
            
            var velocity = Velocities[index];
            velocity += acceleration * DeltaTime;
            velocity = MathUtils.ClampMagnitude(velocity, MaxSpeed);
            Velocities[index] = velocity;

            var position = (float3)transform.position;
            position += velocity * DeltaTime;
            
            transform.position = position;
            Positions[index] = position;

            if (math.lengthsq(velocity) > Epsilon)
            {
                transform.rotation = Quaternion.LookRotation(math.normalizesafe(velocity));
            }
        }
    }
}