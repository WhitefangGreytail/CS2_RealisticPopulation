using Game.Common;
using Game.Objects;
using Game.Prefabs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Entities;
using static Game.Prefabs.VehicleSelectRequirementData;
using Unity.Mathematics;
using Game.Zones;
using Unity.Burst.Intrinsics;
using Unity.Burst;
using Game;
using Game.UI;

namespace CS2_RealisticPopulation.Patches
{
    public class DoPatching
    {
        public DoPatching(UpdateSystem updateSystem)
        {
            ModifySpawnables fsJob = new ModifySpawnables();

        }
    }

    // https://docs.unity3d.com/Packages/com.unity.entities@0.8/manual/chunk_iteration_job.html
    public class ModifySpawnables: IJobChunk
    {
        public ComponentLookup<SpawnableBuildingData> m_SpawnableDatas;
        public ComponentLookup<BuildingPropertyData> m_BuildingPropertyDatas;
        public ComponentTypeHandle<SpawnableBuildingData> spawnableBuildingDataHandle;
        public ComponentTypeHandle<PrefabRef> m_PrefabType;

        public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
        {
            // Game.Zones.BlockSystem.UpdateBlocksJob - I do not get it. These arrays are 1 for 1? Maybe spread out for performance

            // Referencing Game.Simulation.ResidentialDemandSystem.UpdateResidentialDemandJob for the best part
            NativeArray<PrefabRef> prefabNativeArray = chunk.GetNativeArray(ref m_PrefabType);

            // Too bad these are structs. Can't extend </3
            for (int i = 0; i < prefabNativeArray.Length; i++)
            {
                // Need ZoneProperties, BuildingPrefab buildingPrefab, level
                Entity prefab = prefabNativeArray[i].m_Prefab;
                SpawnableBuildingData spawnableBuildingData = m_SpawnableDatas[prefab];
                BuildingPropertyData buildingPropertyData = m_BuildingPropertyDatas[prefab];

                //DynamicBuffer<SubMesh> subMeshes = subMeshBufferAccessor[i];

                // TODO - Check for building type
                //buildingPropertyDataArray[i] = // Change households
                BuildingPropertyData newBuildingData = changeHouseholds(buildingPropertyData);
                m_BuildingPropertyDatas[prefab] = newBuildingData; // See if this works. Maybe get nativeArray for faster access
            }
        }

        /**
            * Changes the household count using the mesh size, lot size and the building type
            */
        private BuildingPropertyData changeHouseholds(BuildingPropertyData current)
        {
            current.m_ResidentialProperties *= 2;
            return current;
        }
    }
}
