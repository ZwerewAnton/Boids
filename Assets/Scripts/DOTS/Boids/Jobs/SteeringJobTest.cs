using DOTS.Boids.Components;
using DOTS.Boids.Components.Boid;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Utils;

namespace DOTS.Boids.Jobs
{
    public partial struct SteeringJobTest : IJobEntity
    {
        [ReadOnly] public NativeArray<Entity> BoidEntities;
        [ReadOnly] public ComponentLookup<LocalTransform> TransformLookup;
        [ReadOnly] public ComponentLookup<Velocity> VelocityLookup;
        [ReadOnly] public MovementParameters MovementParameters;
        [ReadOnly] public FlockingParameters FlockingParameters;
        private const float Epsilon = 0.0001f;

        public void Execute(in Boid tag,
            Entity self,
            ref SteeringAcceleration steeringAcceleration,
            in LocalTransform transform,
            in Velocity velocity)
        {
            var perceptionRadius = FlockingParameters.PerceptionRadius;
            var acceleration = float3.zero;
            var position = transform.Position;
            var perceptionRadius2 = perceptionRadius * perceptionRadius;
            var neighborsCount = 0;
            var alignmentAverageVelocity = float3.zero;
            var cohesionCenter = float3.zero;
            var separationSteer = float3.zero;
            var avoidCount = 0;
            var desiredSeparation = perceptionRadius * 0.5f;
            var desiredSeparation2 = desiredSeparation * desiredSeparation;
            foreach (var other in BoidEntities)
            {
                if (other == self)
                    continue;
                var otherPos = TransformLookup[other].Position;
                var otherVel = VelocityLookup[other].Value;
                if (math.lengthsq(otherPos - position) > perceptionRadius2)
                    continue;
                neighborsCount++;
                alignmentAverageVelocity += otherVel;
                cohesionCenter += otherPos;
                var diff = position - otherPos;
                var d2 = math.lengthsq(diff);
                if (d2 > 0 && d2 < desiredSeparation2)
                {
                    separationSteer += diff / d2;
                    avoidCount++;
                }
            }

            if (neighborsCount == 0)
            {
                steeringAcceleration.Value = float3.zero;
                return;
            }

            alignmentAverageVelocity /= neighborsCount;
            acceleration += ComputeAlignment(alignmentAverageVelocity,
                                velocity.Value,
                                MovementParameters.MaxSpeed) *
                            FlockingParameters.AlignmentWeight;
            cohesionCenter /= neighborsCount;
            acceleration += ComputeCohesion(cohesionCenter,
                                velocity.Value,
                                position,
                                MovementParameters.MaxSpeed) *
                            FlockingParameters.CohesionWeight;
            if (avoidCount > 0)
            {
                separationSteer /= avoidCount;
                acceleration += ComputeSeparation(separationSteer,
                                    velocity.Value,
                                    MovementParameters.MaxSpeed) *
                                FlockingParameters.SeparationWeight;
            }

            acceleration = MathUtils.ClampMagnitude(acceleration,
                MovementParameters.MaxForce);
            steeringAcceleration.Value = acceleration;
        }

        private static float3 ComputeAlignment(float3 averageVelocity,
            float3 boidVelocity,
            float maxSpeed)
        {
            if (math.lengthsq(averageVelocity) < Epsilon)
                return float3.zero;
            var desired = math.normalizesafe(averageVelocity) * maxSpeed;
            var steering = desired - boidVelocity;
            return steering;
        }

        private static float3 ComputeCohesion(float3 center,
            float3 boidVelocity,
            float3 boidPosition,
            float maxSpeed)
        {
            var desired = (center - boidPosition);
            if (math.lengthsq(desired) < Epsilon)
                return float3.zero;
            desired = math.normalizesafe(desired) * maxSpeed;
            var steering = desired - boidVelocity;
            return steering;
        }

        private static float3 ComputeSeparation(float3 steer,
            float3 boidVelocity,
            float maxSpeed)
        {
            if (math.lengthsq(steer) < Epsilon)
                return float3.zero;
            var desired = math.normalizesafe(steer) * maxSpeed;
            var steering = desired - boidVelocity;
            return steering;
        }
    }
}