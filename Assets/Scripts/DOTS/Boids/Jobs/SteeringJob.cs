using Common.Utils;
using DOTS.Boids.Components.Parameters;
using DOTS.Boids.Components.Boid;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace DOTS.Boids.Jobs
{
    public partial struct SteeringJob : IJobEntity
    {
        [ReadOnly]
        public NativeArray<float3> Positions;
        [ReadOnly] 
        public NativeArray<float3> Velocities;
        [ReadOnly]
        public NativeParallelMultiHashMap<int, int> CellToBoid;
        [ReadOnly]
        public float CellSize;
        [ReadOnly]
        public MovementParameters MovementParameters;
        [ReadOnly]
        public FlockingParameters FlockingParameters;

        private const float Epsilon = 0.0001f;

        public void Execute(
            in Boid tag,
            Entity self,
            ref SteeringAcceleration steeringAcceleration,
            in LocalTransform transform,
            in Velocity velocity)
        {
            var position = transform.Position;
            var acceleration = float3.zero;

            var cell = GetCell(position);

            var neighborsCount = 0;
            var alignmentSum = float3.zero;
            var cohesionSum = float3.zero;
            var separationSum = float3.zero;
            var separationCount = 0;

            var perceptionRadius2 = FlockingParameters.PerceptionRadius * FlockingParameters.PerceptionRadius;
            var desiredSeparation2 = (FlockingParameters.PerceptionRadius * 0.5f);
            desiredSeparation2 *= desiredSeparation2;

            for (var x = -1; x <= 1; x++)
            for (var y = -1; y <= 1; y++)
            for (var z = -1; z <= 1; z++)
            {
                var hash = HashUtils.HashCellAbs(cell + new int3(x, y, z));

                if (CellToBoid.TryGetFirstValue(hash, out var boidIndex, out var it))
                {
                    do
                    {
                        if (boidIndex == self.Index) continue;

                        var otherPos = Positions[boidIndex];
                        var diff = otherPos - position;
                        var dist2 = math.lengthsq(diff);

                        if (dist2 > perceptionRadius2) 
                            continue;

                        neighborsCount++;
                        
                        //Alignment
                        alignmentSum += Velocities[boidIndex];
                        
                        //Cohesion
                        cohesionSum += otherPos;

                        //Separation
                        if (dist2 < desiredSeparation2)
                        {
                            separationSum += -diff / (dist2 + Epsilon);
                            separationCount++;
                        }

                    } while (CellToBoid.TryGetNextValue(out boidIndex, ref it));
                }
            }

            if (neighborsCount == 0)
            {
                steeringAcceleration.Value = float3.zero;
                return;
            }

            //Alignment
            alignmentSum /= neighborsCount;
            acceleration += ComputeAlignment(alignmentSum, velocity.Value, MovementParameters.MaxSpeed)
                            * FlockingParameters.AlignmentWeight;

            //Cohesion
            cohesionSum /= neighborsCount;
            acceleration += ComputeCohesion(cohesionSum, velocity.Value, position, MovementParameters.MaxSpeed)
                            * FlockingParameters.CohesionWeight;

            //Separation
            if (separationCount > 0)
            {
                separationSum /= separationCount;
                acceleration += ComputeSeparation(separationSum, velocity.Value, MovementParameters.MaxSpeed)
                                * FlockingParameters.SeparationWeight;
            }

            steeringAcceleration.Value =
                MathUtils.ClampMagnitude(acceleration, MovementParameters.MaxForce);
        }
        
        private int3 GetCell(float3 position)
        {
            return (int3)math.floor(position / CellSize);
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
