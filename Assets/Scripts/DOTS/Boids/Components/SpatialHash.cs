using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace DOTS.Boids.Components
{
    public struct SpatialHash : IComponentData
    {
        public NativeParallelMultiHashMap<int, int> CellToBoid;
        public NativeArray<float3> Positions;
        public NativeArray<float3> Velocities;
        public int BoidCount;
        public float CellSize;
    }
}