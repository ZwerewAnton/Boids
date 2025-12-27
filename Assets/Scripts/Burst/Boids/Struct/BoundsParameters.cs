using Unity.Mathematics;

namespace Boids.Struct
{
    public struct BoundsParameters
    {
        public float BoundsRadius;
        public float BoundsAvoidanceWeight;
        public float3 BoundsCenter;
    }
}