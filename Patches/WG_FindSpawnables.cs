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
using System.Xml;
using System.Runtime.InteropServices;
using System.Drawing;

namespace CS2_RealisticPopulation.Patches
{
    /*

     */

    // Referencing from https://docs.unity3d.com/Manual/JobSystemCreatingJobs.html
    // Not sure what version CS2 is but this seems ok
    // https://docs.unity3d.com/Packages/com.unity.entities@1.0/manual/iterating-data-ijobchunk-implement.html
    //[BurstCompile]
    public struct ChangeSpawnablesJob : IJobChunk
    {
        public ComponentTypeHandle<BuildingPropertyData> buildingPropertyDataHandle; // To change the data
        public ComponentTypeHandle<SpawnableBuildingData> spawnableDataHandle; // To get the level out
        public ComponentTypeHandle<BuildingData> bdHandle;
        public ComponentTypeHandle<ZonePropertiesData> zpDataHandle;

        public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
        {
            // Game.Zones.BlockSystem.UpdateBlocksJob - I do not get it. These arrays are 1 for 1? Maybe spread out for performance

            // Referencing Game.Simulation.ResidentialDemandSystem.UpdateResidentialDemandJob for the best part
            NativeArray<BuildingPropertyData> bpdArray = chunk.GetNativeArray(ref buildingPropertyDataHandle);
            NativeArray<SpawnableBuildingData> spdArray = chunk.GetNativeArray(ref spawnableDataHandle);
            NativeArray<BuildingData> bArray = chunk.GetNativeArray(ref bdHandle);
            //NativeArray<ZonePropertiesData> zpArray = chunk.GetNativeArray(ref zpDataHandle);

            // Too bad these are structs. Can't extend </3
            for (int i = 0; i < bpdArray.Length; i++)
            {
                // Need ZoneProperties, BuildingPrefab buildingPrefab, level
                BuildingPropertyData buildingPropertyData = bpdArray[i];
                SpawnableBuildingData spawnableBuildingData = spdArray[i];
                BuildingData bbData = bArray[i];
                //ZonePropertiesData zpData = zpArray[i];

                if (buildingPropertyData.m_ResidentialProperties > 1) // Target anything that is probably scaled
                {
                    // TODO - Building type is in the space multiplier
                    bpdArray[i] = changeHouseholds(buildingPropertyData, spawnableBuildingData.m_Level, bbData.m_LotSize); ; // See if this works. Maybe get nativeArray for faster access
                }
            }
        }

        /**
        * Changes the household count using the mesh size, lot size and the building type
        */
        private BuildingPropertyData changeHouseholds(BuildingPropertyData current, int level, int2 lotSize)
        {
            int oldHouseholds = current.m_ResidentialProperties;
            float approxOldResidentialProperties = (float)current.m_ResidentialProperties / ((1f + 0.25f * (float)(level - 1)) * (float)(lotSize.x * lotSize.y));
            // If not matching with a number near the cases, then it's a signature building.
            approxOldResidentialProperties = math.round(approxOldResidentialProperties * 2.0f) / 2.0f;
            float baseNum = 1.375f;
            float levelBooster = 0.125f;
            float totalLotSize = (float)(lotSize.x * lotSize.y);

            // m_ResidentialProperties appears to signify the type of residential
            // 1 - Row housing
            // 1.5 - Medium
            // 2 - Mixed
            // 4 - Low rent
            // 6 - Tower
            switch (approxOldResidentialProperties)
            {
                case 1f:
                    // Cap to 3 tiles since it appears the row houses only get built up 3 deep
                    baseNum = 0.75f;
                    levelBooster = 0.25f; // To get it to double the cell size by the end
                    totalLotSize = math.min(lotSize.y, 3);
                    break;
                case 1.5f:
                    approxOldResidentialProperties = 1f;
                    break;
                case 2f:
                    // Mixed use should be slightly more than medium density since the buildings are usually wall to wall)
                    approxOldResidentialProperties = 1.25f;
                    break;
                case 4f:
                    baseNum = 1.25f;
                    levelBooster = 0.05f;
                    approxOldResidentialProperties = 3f;
                    break;
                case 6f:
                    // Reduce residentialProperties to lower if constrained to a short building by tweaking the multiplier by lot size
                    // Small footprint buildings are difficult to make tall and stable
                    baseNum = 1.875f;
                    levelBooster = 0.025f;
                    approxOldResidentialProperties = math.min((lotSize.x + lotSize.y) / 2, 6f);
                    /*
                    if (lotSize == 324)
                    {
                        // Really nerf Glass Crown. This is a bad solution
                        lotSize = 25f;
                    }
                    */
                    break;
                default:
                    Mod.log.Info($"Abnormal approxOldResidentialProperties : {approxOldResidentialProperties}. Possibly non scaled");
                    return current; // Abort. Previous changes would have aborted before as well
            }

            Mod.log.Info($"{level} - {current.m_ResidentialProperties}: RP: {approxOldResidentialProperties}, SM: {current.m_SpaceMultiplier}, {lotSize.x}x{lotSize.y}");
            current.m_ResidentialProperties = (int)((baseNum + (levelBooster * level)) * totalLotSize * approxOldResidentialProperties);
            /*
            string value = $"GetBuildingPropertyData {buildingPrefab.m_LotWidth}x{buildingPrefab.m_LotDepth} -> {num}";
            if (!uniqueCalcString.Contains(value))
            {
                System.Console.WriteLine(value);
                uniqueCalcString.Add(value);
            }
            */
            return current;
        }
    }
}
