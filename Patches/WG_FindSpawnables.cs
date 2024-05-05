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
using System.ComponentModel;
using Game.UI.InGame;
using System.Collections;

namespace CS2_RealisticPopulation.Patches
{
    // Referencing from https://docs.unity3d.com/Manual/JobSystemCreatingJobs.html
    // Not sure what version CS2 is but this seems ok
    // https://docs.unity3d.com/Packages/com.unity.entities@1.0/manual/iterating-data-ijobchunk-implement.html
    //[BurstCompile]
    public struct ChangeSpawnablesJob : IJobChunk
    {
        public ComponentTypeHandle<BuildingPropertyData> buildingPropertyDataHandle; // To change the data
        public ComponentTypeHandle<SpawnableBuildingData> spawnableDataHandle; // To get the level out
        public ComponentTypeHandle<BuildingData> bdHandle;
        public ComponentTypeHandle<ZoneData> zdHandle;
        public ComponentLookup<MeshData> meshDataLookup;
        //public BufferTypeHandle<SubMesh> subMeshBuffer;
        //public ComponentTypeHandle<ZonePropertiesData> zpDataHandle;
        //public ComponentTypeHandle<PrefabData> prefabDataHandle;
        //public ComponentTypeHandle<PrefabRef> prefabRefHandle;
        //public ComponentTypeHandle<MeshData> meshDataHandle;

        public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
        {
            // Game.Zones.BlockSystem.UpdateBlocksJob - I do not get it. These arrays are 1 for 1? Maybe spread out for performance
            
            // Referencing Game.Simulation.ResidentialDemandSystem.UpdateResidentialDemandJob for the best part
            NativeArray<BuildingPropertyData> bpdArray = chunk.GetNativeArray(ref buildingPropertyDataHandle);
            NativeArray<SpawnableBuildingData> spdArray = chunk.GetNativeArray(ref spawnableDataHandle);
            NativeArray<BuildingData> bArray = chunk.GetNativeArray(ref bdHandle);
            //NativeArray<PrefabData> pfArray = chunk.GetNativeArray(ref prefabDataHandle);
            //NativeArray<ZoneData> zdArray = chunk.GetNativeArray(ref zdHandle);
            //BufferAccessor<SubMesh> smBuffer = chunk.GetBufferAccessor(ref subMeshBuffer); // Thanks IntelliCode

            // Too bad these are structs. Can't extend </3
            Mod.log.Info($"Count: {bpdArray.Length}");
            for (int i = 0; i < bpdArray.Length; i++)
            {
                // Need ZoneProperties, BuildingPrefab buildingPrefab, level
                BuildingPropertyData buildingPropertyData = bpdArray[i];
                SpawnableBuildingData spawnableBuildingData = spdArray[i];
                BuildingData bbData = bArray[i];
                //PrefabData pfData = pfArray[i];
                //ZoneData zData = zdArray[i];
                //DynamicBuffer<SubMesh> subMeshes = smBuffer[i];
                //ZonePropertiesData zpData = zpArray[i];

                if (buildingPropertyData.m_ResidentialProperties > 1) // Target anything that is probably scaled
                {
                    //Mod.log.Info($"ObjectGeometryData : {ogData.m_Bounds.x},{ogData.m_Bounds.y},{ogData.m_Bounds.z}");
                    // TODO - Building type is in the space multiplier
                    bpdArray[i] = changeHouseholdsPlotSize(buildingPropertyData, spawnableBuildingData.m_Level, bbData.m_LotSize); ; // See if this works. Maybe get nativeArray for faster access
                }
            }
        }

        /**
        * Changes the household count using the mesh size, lot size and the building type
        */
        private BuildingPropertyData changeHouseholdsPlotSize(BuildingPropertyData current, int level, int2 lotSize)
        {
            int oldHouseholds = current.m_ResidentialProperties;
            float baseNum = 1.375f;
            float levelBooster = 0.125f;
            float totalLotSize = (float)(lotSize.x * lotSize.y);
            float residentialProperties = 1f;

            // Space multiplier is used in the prefab data to signify the type of building
            // 2 - Row housing
            // 2 - Medium (how the heck can this be the same? Ugh. There's no 1 wide medium density though, so can get around it)
            // 2.5 - Mixed
            // 1 - Low rent
            // 3 - Tower
            switch (current.m_SpaceMultiplier)
            {
                case 2f:
                    if (lotSize.x == 1)
                    {
                        // Row houses are the only buildings which are 1 tile wide
                        residentialProperties = 1f;
                    }
                    else
                    {
                        residentialProperties = 1.5f;
                    }
                    break;
                case 2.5f:
                    residentialProperties = 2f;
                    break;
                case 1f:
                    residentialProperties = 4f;
                    break;
                case 3f:
                    residentialProperties = 6f;
                    break;
            }

            // The old m_ResidentialProperties (from ZoneProperties) appears to signify the type of residential
            // 1 - Row housing
            // 1.5 - Medium
            // 2 - Mixed
            // 4 - Low rent
            // 6 - Tower
            switch (residentialProperties)
            {
                case 1f:
                    // Cap to 3 tiles since it appears the row houses only get built up 3 deep
                    baseNum = 0.75f;
                    levelBooster = 0.25f; // To get it to double the cell size by the end
                    totalLotSize = math.min(lotSize.y, 3);
                    break;
                case 1.5f:
                    residentialProperties = 1f;
                    break;
                case 2f:
                    // Mixed use should be slightly more than medium density since the buildings are usually wall to wall)
                    residentialProperties = 1.25f;
                    break;
                case 4f:
                    baseNum = 1.25f;
                    levelBooster = 0.05f;
                    residentialProperties = 3f;
                    break;
                case 6f:
                    // Reduce residentialProperties to lower if constrained to a short building by tweaking the multiplier by lot size
                    // Small footprint buildings are difficult to make tall and stable
                    baseNum = 2f;
                    levelBooster = 0.05f;
                    residentialProperties = math.min((lotSize.x + lotSize.y) / 2, 6f);
                    /*
                    if (lotSize == 324)
                    {
                        // Really nerf Glass Crown. This is a bad solution
                        lotSize = 25f;
                    }
                    */
                    break;
                default:
                    return current; // Abort. Previous changes would have aborted before as well
            }

            current.m_ResidentialProperties = (int)((baseNum + (levelBooster * level)) * totalLotSize * residentialProperties);
            Mod.log.Info($"{level} - {current.m_ResidentialProperties}, SM: {current.m_SpaceMultiplier}, {lotSize.x}x{lotSize.y}");
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
