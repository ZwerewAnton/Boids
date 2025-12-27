using Burst.Boids.Struct;
using Common.Utils;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace Burst.Boids.Jobs
{
    [BurstCompile(
        FloatMode = FloatMode.Fast,
        FloatPrecision = FloatPrecision.Low
    )]
    public struct SteeringJob : IJobParallelFor
    {
        [ReadOnly]
        public NativeArray<float3> Positions;
        [ReadOnly]
        public NativeArray<float3> Velocities;
        [WriteOnly]
        public NativeArray<float3> SteeringAccelerations;
        public FlockingParameters FlockingParameters;
        public float MaxSpeed;
        public float MaxForce;
        
        private const float Epsilon = 0.0001f;
        
        public void Execute(int index)
        {
            var perceptionRadius = FlockingParameters.PerceptionRadius;
            var acceleration = float3.zero;
            var position = Positions[index];
            var velocity = Velocities[index];
            var perceptionRadius2 = perceptionRadius * perceptionRadius;
            var neighborsCount = 0;
            
            var alignmentAverageVelocity = float3.zero;
            
            var cohesionCenter = float3.zero;
            
            var separationSteer = float3.zero;
            var avoidCount = 0;
            var desiredSeparation = perceptionRadius * 0.5f;
            var desiredSeparation2 = desiredSeparation * desiredSeparation;

            for (var i = 0; i < Positions.Length; i++)
            {
                if (i == index)
                    continue;
                if (math.lengthsq(Positions[i] - position) > perceptionRadius2)
                    continue;

                neighborsCount++;
                
                //Alignment
                alignmentAverageVelocity += Velocities[i];
                
                //Cohesion
                cohesionCenter += Positions[i];
                
                //Separation
                var diff = position - Positions[i];
                var d2 = math.lengthsq(diff);
                if (d2 > 0 && d2 < desiredSeparation2)
                {
                    separationSteer += diff / d2;
                    avoidCount++;
                }
            }

            if (neighborsCount == 0)
            {
                SteeringAccelerations[index] = float3.zero;
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

            acceleration = MathUtils.ClampMagnitude(acceleration, MaxForce);
            SteeringAccelerations[index] = acceleration;
        }

        private static float3 ComputeAlignment(float3 averageVelocity, float3 boidVelocity, float maxSpeed)
        {
            if (math.lengthsq(averageVelocity) < Epsilon) 
                return float3.zero;
            
            var desired = math.normalizesafe(averageVelocity) * maxSpeed;
            var steering = desired - boidVelocity;
            return steering;
        }
        
        private static float3 ComputeCohesion(float3 center, float3 boidVelocity,  float3 boidPosition, float maxSpeed)
        {
            var desired = (center - boidPosition);
            if (math.lengthsq(desired) < Epsilon) 
                return float3.zero;

            desired = math.normalizesafe(desired) * maxSpeed;
            var steering = desired - boidVelocity;
            return steering;
        }

        private static float3 ComputeSeparation(float3 steer, float3 boidVelocity, float maxSpeed)
        {
            if (math.lengthsq(steer) < Epsilon) 
                return float3.zero;

            var desired = math.normalizesafe(steer) * maxSpeed;
            var steering = desired - boidVelocity;
            return steering;
        }
    }
}