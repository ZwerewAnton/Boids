using Unity.Entities;
using Unity.Mathematics;

namespace DOTS.Boids.Components.Boid
{
    public struct SteeringAcceleration : IComponentData
    {
        public float3 Value;
    }
}