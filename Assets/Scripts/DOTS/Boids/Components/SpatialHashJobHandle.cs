using Unity.Entities;
using Unity.Jobs;

namespace DOTS.Boids.Components
{
    public struct SpatialHashJobHandle : IComponentData
    {
        public JobHandle Handle;
    }
}