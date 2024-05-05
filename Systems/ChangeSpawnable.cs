using Colossal.Serialization.Entities;
using CS2_RealisticPopulation.Patches;
using Game;
using Game.Audio;
using Game.Prefabs;
using Game.Simulation;
using Game.UI.Menu;
using RealisticPopulation;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Scripting;

namespace WG_CS2_RealisticPopulation.Systems
{
    public partial class ChangeSpawnable : GameSystemBase
    {
        private PrefabSystem m_PrefabSystem;
        private EntityQuery m_SpawnableQuery;
        private static bool initialiseQueued = false;

        protected override void OnCreate()
        {
            // Guessing the list of structs stored in a spawnable building taken from
            // Game.Prefabs.BuildingInitializeSystem.OnUpdate()
            m_PrefabSystem = base.World.GetOrCreateSystemManaged<PrefabSystem>();
            // Might have to combine?
            // Can only be structs to get it in an array easily
            m_SpawnableQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<SpawnableBuildingData, BuildingData, BuildingPropertyData>()
                .Build(this);
            RequireForUpdate(m_SpawnableQuery);

            Mod.log.Info(nameof(ChangeSpawnable) + " job created");
            base.OnCreate();
        }

        protected override void OnGamePreload(Purpose purpose, GameMode mode)
        {
            Mod.log.Info(nameof(ChangeSpawnable) + " job OnGamePreload");
            // Do query and make changes as the game is loading
            if (!initialiseQueued && (mode == GameMode.Game))
            {
                // Instantiate the job struct
                var changeSpawnablesJob = new ChangeSpawnablesJob();
                //changeSpawnablesJob.prefabRefHandle = this.GetComponentTypeHandle<PrefabRef>(false);
                //changeSpawnablesJob.prefabDataHandle = this.GetComponentTypeHandle<PrefabData>(false);
                changeSpawnablesJob.spawnableDataHandle = this.GetComponentTypeHandle<SpawnableBuildingData>(false);
                changeSpawnablesJob.buildingPropertyDataHandle = this.GetComponentTypeHandle<BuildingPropertyData>(true);
                changeSpawnablesJob.bdHandle = this.GetComponentTypeHandle<BuildingData>(false);
                //changeSpawnablesJob.subMeshBuffer = this.GetBufferTypeHandle<SubMesh>(false);
                //changeSpawnablesJob.meshHandle = this.GetComponentTypeHandle<MeshData>(false);
                //changeSpawnablesJob.zdHandle = this.GetComponentTypeHandle<ZoneData>(false);

                // Schedule the job
                this.Dependency
                    = changeSpawnablesJob.ScheduleParallel(m_SpawnableQuery, this.Dependency);
                initialiseQueued = true; // Need this to stop parsing over the buildings on each load
            }
        }

        protected override void OnUpdate()
        {

        }
    }
}
