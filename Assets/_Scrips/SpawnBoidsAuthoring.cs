using DefaultNamespace;
using Unity.Entities;
using Unity.Physics;
using Unity.Mathematics;
using Random = Unity.Mathematics.Random;

class SpawnBoidsAuthoring : SpawnRandomObjectsAuthoringBase<BoidsSpawnSettings>
{
    public int GroupID;
    public float AngularSpeed;
    public float MaxSpeed;
    public float Distancing;

    internal override void Configure(ref BoidsSpawnSettings spawnSettings)
    {
        spawnSettings.AngularSpeed = AngularSpeed;
        spawnSettings.MaxSpeed = MaxSpeed;
        spawnSettings.Distancing = Distancing;
        spawnSettings.GroupID = GroupID;
    }
}

struct BoidsSpawnSettings : IComponentData, ISpawnSettings
{
    public Entity Prefab { get; set; }
    public float3 Position { get; set; }
    public quaternion Rotation { get; set; }
    public float3 Range { get; set; }
    public int Count { get; set; }
    public float AngularSpeed { get; set; }
    public float MaxSpeed { get; set; }
    public float Distancing { get; set; }
    public int GroupID { get; set; }
}

class SpawnBoidsSystem : SpawnRandomObjectsSystemBase<BoidsSpawnSettings>
{
    internal override int GetRandomSeed(BoidsSpawnSettings spawnSettings)
    {
        var seed = base.GetRandomSeed(spawnSettings);
        seed = (seed * 397) ^ spawnSettings.Prefab.GetHashCode();
        return seed;
    }

    internal override void ConfigureInstance(Entity instance, BoidsSpawnSettings spawnSettings)
    {
        EntityManager.AddSharedComponentData(instance, new BoidGroup
        {
            Group = spawnSettings.GroupID,
            Distancing = spawnSettings.Distancing,
            AngularSpeed = spawnSettings.AngularSpeed,
            MaxSpeed = spawnSettings.MaxSpeed,
        });
        EntityManager.AddComponentData(instance, new Velocity{ Vel = 0.0f});
        EntityManager.AddComponentData(instance, new Direction{ Dir = float3.zero});
    }
}