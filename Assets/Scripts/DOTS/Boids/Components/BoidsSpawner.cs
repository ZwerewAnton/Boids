using Unity.Entities;

namespace DOTS.Boids.Components
{
    public struct BoidsSpawner : IComponentData
    {
        public Entity BoidPrefab;
        public int Count;
        public float SpawnRadius;
    }
}