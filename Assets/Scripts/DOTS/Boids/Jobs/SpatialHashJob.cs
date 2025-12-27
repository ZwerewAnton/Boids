using Common.Utils;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace DOTS.Boids.Jobs
{
    [BurstCompile]
    public struct SpatialHashBuildJob : IJobParallelFor
    {
        [ReadOnly]
        public NativeArray<float3> Positions;
        [ReadOnly]
        public int BoidCount;
        [ReadOnly]
        public float CellSize;
        [WriteOnly]
        public NativeParallelMultiHashMap<int, int>.ParallelWriter CellToBoid;

        public void Execute(int index)
        {
            if (index >= BoidCount)
                return;

            var pos = Positions[index];
            var cell = (int3)math.floor(pos / CellSize);

            var hash = HashUtils.HashCell(cell);

            CellToBoid.Add(hash, index);
        }
    }
}