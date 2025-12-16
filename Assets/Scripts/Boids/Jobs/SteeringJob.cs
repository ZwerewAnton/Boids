using Boids.Struct;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace Boids.Jobs
{
    public struct SteeringJob : IJobParallelFor
    {
        [ReadOnly]
        public NativeArray<Vector3> Positions;
        [ReadOnly]
        public NativeArray<Vector3> Velocities;
        [WriteOnly]
        public NativeArray<Vector3> SteeringAccelerations;
        public FlockingParameters FlockingParameters;
        public float MaxSpeed;
        public float MaxForce;
        
        public void Execute(int index)
        {
            var perceptionRadius = FlockingParameters.PerceptionRadius;
            var acceleration = Vector3.zero;
            var position = Positions[index];
            var velocity = Velocities[index];
            var perceptionRadius2 = perceptionRadius * perceptionRadius;
            var neighborsCount = 0;
            
            var alignmentAverageVelocity = Vector3.zero;
            
            var cohesionCenter = Vector3.zero;
            
            var separationSteer = Vector3.zero;
            var avoidCount = 0;
            var desiredSeparation = perceptionRadius * 0.5f;
            var desiredSeparation2 = desiredSeparation * desiredSeparation;

            for (var i = 0; i < Positions.Length; i++)
            {
                if (i == index)
                    continue;
                if ((Positions[i] - position).sqrMagnitude > perceptionRadius2)
                    continue;

                neighborsCount++;
                
                //Alignment
                alignmentAverageVelocity += Velocities[i];
                
                //Cohesion
                cohesionCenter += Positions[i];
                
                //Separation
                var diff = position - Positions[i];
                var d2 = diff.sqrMagnitude;
                if (d2 > 0 && d2 < desiredSeparation2)
                {
                    separationSteer += diff / d2;
                    avoidCount++;
                }
            }

            if (neighborsCount == 0)
            {
                SteeringAccelerations[index] = Vector3.zero;
                return;
            }

            //Alignment
            alignmentAverageVelocity /= neighborsCount;
            acceleration += ComputeAlignment(alignmentAverageVelocity, velocity, MaxSpeed) * FlockingParameters.AlignmentWeight;

            //Cohesion
            cohesionCenter /= neighborsCount;
            acceleration += ComputeCohesion(cohesionCenter, velocity, position, MaxSpeed) * FlockingParameters.CohesionWeight;
            
            //Separation
            if (avoidCount > 0)
            {
                separationSteer /= avoidCount;
                acceleration += ComputeSeparation(separationSteer, velocity, MaxSpeed) * FlockingParameters.SeparationWeight;
            }

            acceleration = Vector3.ClampMagnitude(acceleration, MaxForce);
            SteeringAccelerations[index] = acceleration;
        }

        private static Vector3 ComputeAlignment(Vector3 averageVelocity, Vector3 boidVelocity, float maxSpeed)
        {
            if (averageVelocity.sqrMagnitude < Mathf.Epsilon) 
                return Vector3.zero;
            
            var desired = averageVelocity.normalized * maxSpeed;
            var steering = desired - boidVelocity;
            return steering;
        }
        
        private static Vector3 ComputeCohesion(Vector3 center, Vector3 boidVelocity,  Vector3 boidPosition, float maxSpeed)
        {
            var desired = (center - boidPosition);
            if (desired.sqrMagnitude < Mathf.Epsilon) 
                return Vector3.zero;

            desired = desired.normalized * maxSpeed;
            var steering = desired - boidVelocity;
            return steering;
        }

        private static Vector3 ComputeSeparation(Vector3 steer, Vector3 boidVelocity, float maxSpeed)
        {
            if (steer.sqrMagnitude < Mathf.Epsilon) 
                return Vector3.zero;

            var desired = steer.normalized * maxSpeed;
            var steering = desired - boidVelocity;
            return steering;
        }
    }
}