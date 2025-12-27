using Unity.Entities;
using Unity.Mathematics;

namespace DOTS.Boids.Components.Boid
{
    public struct BoundsAcceleration : IComponentData
    {
        public float3 Value;
    }
}