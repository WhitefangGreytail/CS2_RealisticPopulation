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

    // Referencing from https://docs.unity3d.com/Manual/JobSystemCreatingJobs.html
    // Not sure what version CS2 is but this seems ok
    // https://docs.unity3d.com/Packages/com.unity.entities@1.0/manual/iterating-data-ijobchunk-implement.html
    //[BurstCompile]
    public struct ChangeSpawnablesJob : IJobChunk
    {
        public ComponentTypeHandle<SpawnableBuildingData> spawnableDataHandle; // TODO - Try to replace
        public ComponentTypeHandle<BuildingPropertyData> buildingPropertyDataHandle;
        public ComponentTypeHandle<PrefabRef> m_PrefabType;

        public EntityCommandBuffer.ParallelWriter Ecb;
        public EntityTypeHandle EntityHandle;
        public ComponentLookup<BuildingPropertyData> BuildingPropertyDataLookup;

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
