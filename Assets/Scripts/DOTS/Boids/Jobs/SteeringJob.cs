using DOTS.Boids.Components;
using DOTS.Boids.Components.Boid;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Utils;

namespace DOTS.Boids.Jobs
{
    public partial struct SteeringJob : IJobEntity
    {
        [ReadOnly]
        public NativeArray<float3> Positions;
        [ReadOnly]
        public NativeParallelMultiHashMap<int, int> CellToBoid;
        [ReadOnly]
        public float CellSize;

        [ReadOnly]
        public MovementParameters MovementParameters;
        [ReadOnly]
        public FlockingParameters FlockingParameters;

        private const float Epsilon = 0.0001f;
        private const int P1 = 73856093;
        private const int P2 = 19349663;
        private const int P3 = 83492791;

        public void Execute(
            in Boid tag,
            Entity self,
            ref SteeringAcceleration steeringAcceleration,
            in LocalTransform transform,
            in Velocity velocity)
        {
            var acceleration = float3.zero;
            var position = transform.Position;

            var cell = GetCell(position);

            var neighborsCount = 0;
            var alignmentAverageVelocity = float3.zero;
            var cohesionCenter = float3.zero;
            var separationSteer = float3.zero;
            var avoidCount = 0;

            var perceptionRadius2 = FlockingParameters.PerceptionRadius * FlockingParameters.PerceptionRadius;
            var desiredSeparation2 = (FlockingParameters.PerceptionRadius * 0.5f);
            desiredSeparation2 *= desiredSeparation2;

            for (var x = -1; x <= 1; x++)
            for (var y = -1; y <= 1; y++)
            for (var z = -1; z <= 1; z++)
            {
                var hash = HashCell(cell + new int3(x, y, z));

                if (CellToBoid.TryGetFirstValue(hash, out int boidIndex, out var it))
                {
                    do
                    {
                        if (boidIndex == self.Index)
                            continue;
                        
                        var otherPos = Positions[boidIndex];

                        var diff = otherPos - position;
                        var dist2 = math.lengthsq(diff);
                        if (dist2 > perceptionRadius2) 
                            continue;

                        neighborsCount++;
                        alignmentAverageVelocity += diff;
                        cohesionCenter += otherPos;

                        if (dist2 < desiredSeparation2)
                        {
                            separationSteer += -diff / dist2;
                            avoidCount++;
                        }

                    } while (CellToBoid.TryGetNextValue(out boidIndex, ref it));
                }
            }

            if (neighborsCount == 0)
            {
                steeringAcceleration.Value = float3.zero;
                return;
            }

            // Alignment
            alignmentAverageVelocity /= neighborsCount;
            acceleration += ComputeAlignment(alignmentAverageVelocity, velocity.Value, MovementParameters.MaxSpeed)
                            * FlockingParameters.AlignmentWeight;

            // Cohesion
            cohesionCenter /= neighborsCount;
            acceleration += ComputeCohesion(cohesionCenter, velocity.Value, position, MovementParameters.MaxSpeed)
                            * FlockingParameters.CohesionWeight;

            // Separation
            if (avoidCount > 0)
            {
                separationSteer /= avoidCount;
                acceleration += ComputeSeparation(separationSteer, velocity.Value, MovementParameters.MaxSpeed)
                                * FlockingParameters.SeparationWeight;
            }

            acceleration = MathUtils.ClampMagnitude(acceleration, MovementParameters.MaxForce);
            steeringAcceleration.Value = acceleration;
        }

        private int3 GetCell(float3 position)
        {
            return (int3)math.floor(position / CellSize);
        }

        private static int HashCell(int3 cell)
        {
            return math.abs(cell.x * P1 ^ cell.y * P2 ^ cell.z * P3);
        }

        private static float3 ComputeAlignment(float3 averageVelocity, float3 boidVelocity, float maxSpeed)
        {
            if (math.lengthsq(averageVelocity) < Epsilon)
                return float3.zero;
            
            var desired = math.normalizesafe(averageVelocity) * maxSpeed;
            return desired - boidVelocity;
        }

        private static float3 ComputeCohesion(float3 center, float3 boidVelocity, float3 boidPosition, float maxSpeed)
        {
            var desired = center - boidPosition;
            if (math.lengthsq(desired) < Epsilon)
                return float3.zero;
            
            desired = math.normalizesafe(desired) * maxSpeed;
            return desired - boidVelocity;
        }

        private static float3 ComputeSeparation(float3 steer, float3 boidVelocity, float maxSpeed)
        {
            if (math.lengthsq(steer) < Epsilon)
                return float3.zero;
            
            var desired = math.normalizesafe(steer) * maxSpeed;
            return desired - boidVelocity;
        }
    }
}
