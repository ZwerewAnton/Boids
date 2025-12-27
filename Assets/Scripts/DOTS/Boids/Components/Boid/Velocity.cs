using Unity.Entities;
using Unity.Mathematics;

namespace DOTS.Boids.Components.Boid
{
    public struct Velocity : IComponentData
    {
        public float3 Value;
    }
}