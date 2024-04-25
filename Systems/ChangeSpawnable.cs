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
        private EntityQuery m_SpawnableQuery;

        protected override void OnCreate()
        {
            // Might have to combine?
            // Can only be structs to get it in an array easily
            m_SpawnableQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<PrefabData, SpawnableBuildingData, BuildingData, BuildingPropertyData>()
                .Build(this);
            RequireForUpdate(m_SpawnableQuery);

            Mod.log.Info(nameof(ChangeSpawnable) + " job created");
            base.OnCreate();
        }

        protected override void OnGamePreload(Purpose purpose, GameMode mode)
        {
            Mod.log.Info(nameof(ChangeSpawnable) + " job OnGamePreload");
            // Do query and make changes as the game is loading
            if (mode == GameMode.Game)
            {
                Mod.log.Info("Game mode");
                // Instantiate the job struct
                var changeSpawnablesJob = new ChangeSpawnablesJob();
                changeSpawnablesJob.spawnableDataHandle = this.GetComponentTypeHandle<SpawnableBuildingData>(false);
                changeSpawnablesJob.buildingPropertyDataHandle = this.GetComponentTypeHandle<BuildingPropertyData>(true);
                changeSpawnablesJob.bdHandle = this.GetComponentTypeHandle<BuildingData>(false);

                // Schedule the job
                this.Dependency
                    = changeSpawnablesJob.ScheduleParallel(m_SpawnableQuery, this.Dependency);
            }
        }

        protected override void OnUpdate()
        {

        }
    }
}
