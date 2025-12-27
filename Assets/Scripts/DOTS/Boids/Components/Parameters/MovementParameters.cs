using Unity.Entities;

namespace DOTS.Boids.Components
{
    public struct MovementParameters : IComponentData
    {
        public float MaxForce;
        public float MaxSpeed;
        public int Count;
    }
}