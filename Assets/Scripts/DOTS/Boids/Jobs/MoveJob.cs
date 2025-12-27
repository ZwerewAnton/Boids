using Common.Utils;
using DOTS.Boids.Components.Boid;
using DOTS.Boids.Components.Parameters;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace DOTS.Boids.Jobs
{
    public partial struct MoveJob : IJobEntity
    {
        public MovementParameters MovementParameters;
        public float DeltaTime;
        
        private const float Epsilon = 0.0001f;
        
        public void Execute(
            in Boid tag,
            in BoundsAcceleration boundsAcceleration,
            in SteeringAcceleration steeringAcceleration,
            ref LocalTransform transform,
            ref Velocity velocity)
        {
            var acceleration = steeringAcceleration.Value + boundsAcceleration.Value;
            acceleration = MathUtils.ClampMagnitude(acceleration, MovementParameters.MaxForce);
            
            var newVelocity = velocity.Value;
            newVelocity += acceleration * DeltaTime;
            newVelocity = MathUtils.ClampMagnitude(newVelocity, MovementParameters.MaxSpeed);
            velocity.Value = newVelocity;

            transform.Position += velocity.Value * DeltaTime;

            if (math.lengthsq(velocity.Value) > Epsilon)
            {
                transform.Rotation = quaternion.LookRotationSafe( 
                    math.normalizesafe(velocity.Value),
                    math.up()
                );
            }
        }
    }
}