using Unity.Collections;
using UnityEngine;
using UnityEngine.Jobs;

namespace Boids.Jobs
{
    public struct MoveJob : IJobParallelForTransform
    {
        [ReadOnly]
        public NativeArray<Vector3> SteeringAccelerations;
        [ReadOnly]
        public NativeArray<Vector3> BoundsAccelerations;
        [WriteOnly]
        public NativeArray<Vector3> Positions;
        
        public NativeArray<Vector3> Velocities;
        
        public float MaxSpeed;
        public float MaxForce;
        public float DeltaTime;
        
        public void Execute(int index, TransformAccess transform)
        {
            var acceleration = SteeringAccelerations[index] + BoundsAccelerations[index];
            acceleration = Vector3.ClampMagnitude(acceleration, MaxForce);
            
            var velocity = Velocities[index];
            velocity += acceleration * DeltaTime;
            velocity = Vector3.ClampMagnitude(velocity, MaxSpeed);
            Velocities[index] = velocity;

            var position = transform.position + velocity * DeltaTime;
            transform.position = position;
            Positions[index] = position;

            if (velocity.sqrMagnitude > Mathf.Epsilon)
            {
                transform.rotation = Quaternion.LookRotation(velocity.normalized);
            }
        }
    }
}