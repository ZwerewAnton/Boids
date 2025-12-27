using Unity.Entities;
using Unity.Mathematics;

namespace DOTS.Boids.Components.Parameters
{
    public struct BoundsParameters : IComponentData
    {
        public float BoundsRadius;
        public float BoundsAvoidanceWeight;
        public float3 BoundsCenter;
    }
}