using Unity.Entities;

namespace DOTS.Boids.Components
{
    public struct FlockingParameters : IComponentData
    {
        public float PerceptionRadius;
        public float CohesionWeight;
        public float AlignmentWeight;
        public float SeparationWeight;
    }
}