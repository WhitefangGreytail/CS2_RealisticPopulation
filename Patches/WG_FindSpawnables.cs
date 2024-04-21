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
using RealisticPopulation;
using UnityEngine.Windows;
using Game.Tools;
using System.Runtime.CompilerServices;

namespace CS2_RealisticPopulation.Patches
{
    /*
     * [Error  : Unity Log] [SceneFlow] [CRITICAL]  System update error during PrefabUpdate->ObjectInitializeSystem: System.Collections.Generic.KeyNotFoundException: The given key 'FountainPlaza01 Mesh (Game.Prefabs.RenderPrefab)' was not present in the dictionary.
  at System.Collections.Generic.Dictionary`2[TKey,TValue].get_Item (TKey key) [0x0001e] in <b89873cb176e44a995a4781c7487d410>:0
  at Game.Prefabs.PrefabSystem.GetEntity (Game.Prefabs.PrefabBase prefab) [0x00000] in <feff44428bf3435d97abac1bad09fa9d>:0
  at Game.Prefabs.ObjectInitializeSystem.InitializePrefab (Game.Prefabs.ObjectGeometryPrefab objectPrefab, Game.Prefabs.PlaceableObjectData placeableObjectData, Game.Prefabs.ObjectGeometryData& objectGeometryData, Game.Prefabs.GrowthScaleData& growthScaleData, Game.Prefabs.StackData& stackData, Game.Prefabs.QuantityObjectData& quantityObjectData, Game.Prefabs.CreatureData& creatureData, Unity.Entities.DynamicBuffer`1[T] meshes, Unity.Entities.DynamicBuffer`1[T] meshGroups, Unity.Entities.DynamicBuffer`1[T] characterElements, System.Boolean isPlantObject, System.Boolean isHumanObject, System.Boolean isBuildingObject, System.Boolean isVehicleObject, System.Boolean isCreatureObject) [0x00448] in <feff44428bf3435d97abac1bad09fa9d>:0
  at Game.Prefabs.ObjectInitializeSystem.OnUpdate () [0x00681] in <feff44428bf3435d97abac1bad09fa9d>:0
  at Unity.Entities.SystemBase.Update () [0x0004e] in <42dd0aeaaef34ed8acb4b4fe5f093234>:0
  at Game.UpdateSystem.Update (Game.SystemUpdatePhase phase) [0x0004e] in <feff44428bf3435d97abac1bad09fa9d>:0
     */

    // Referencing from https://docs.unity3d.com/Manual/JobSystemCreatingJobs.html
    // Not sure what version CS2 is but this seems ok
    // https://docs.unity3d.com/Packages/com.unity.entities@1.0/manual/iterating-data-ijobchunk-implement.html
    //[BurstCompile]
    public struct ChangeSpawnablesJob : IJobChunk
    {
        public ComponentTypeHandle<SpawnableBuildingData> spawnableDataHandle; // TODO - Try to replace
        public ComponentTypeHandle<BuildingPropertyData> buildingPropertyDataHandle;
        public ComponentTypeHandle<PrefabRef> prefabRefHandle;

        public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
        {
            // Game.Zones.BlockSystem.UpdateBlocksJob - I do not get it. These arrays are 1 for 1? Maybe spread out for performance

            // Referencing Game.Simulation.ResidentialDemandSystem.UpdateResidentialDemandJob for the best part
            NativeArray<BuildingPropertyData> bpdArray = chunk.GetNativeArray(ref buildingPropertyDataHandle);
            NativeArray<SpawnableBuildingData> spdArray = chunk.GetNativeArray(ref spawnableDataHandle);
            // Too bad these are structs. Can't extend </3
            Mod.log.Info($"Starting + {bpdArray.Length}");
            for (int i = 0; i < bpdArray.Length; i++)
            {
                // Need ZoneProperties, BuildingPrefab buildingPrefab, level
                BuildingPropertyData buildingPropertyData = bpdArray[i];
                SpawnableBuildingData spawnableBuildingData = spdArray[i];

                //DynamicBuffer<SubMesh> subMeshes = subMeshBufferAccessor[i];

                // TODO - Check for building type
                //buildingPropertyDataArray[i] = // Change households
                BuildingPropertyData newBuildingData = changeHouseholds(buildingPropertyData);

                Mod.log.Info("Game");
                bpdArray[i] = newBuildingData; // See if this works. Maybe get nativeArray for faster access
            }
            Mod.log.Info("End");
        }

        /**
        * Changes the household count using the mesh size, lot size and the building type
        */
        private BuildingPropertyData changeHouseholds(BuildingPropertyData current)
        {
            // TODO - Copy zone properties work into here
            current.m_ResidentialProperties *= 2;
            return current;
        }
    }
}
