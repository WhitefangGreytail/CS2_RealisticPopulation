using HarmonyLib;
using UnityEngine;
using Game.Prefabs;
using Game.Simulation;
using Unity.Mathematics;
using Game.Economy;
using System.Collections.Generic;
using Unity.Entities;

/*
 * Scraps
 * 
 * This code has flow on effects across the entire simulation. Maybe best to just leave it.
*/
namespace WG_CS2_RealisticPopulation.Patches
{
    [HarmonyPatch(typeof(ZoneProperties), nameof(ZoneProperties.InitializeBuilding))]
    class WG_ZoneProperties_InitializeBuilding
    {
        [HarmonyPrefix]
        static bool InitializeBuilding_Prefix(ref ZoneProperties __instance, EntityManager entityManager, Entity entity, BuildingPrefab buildingPrefab, byte level)
        {
            if (!buildingPrefab.Has<BuildingProperties>())
            {
                BuildingPropertyData buildingPropertyData = WG_ZoneProperties_Static.GetBuildingPropertyData(__instance, buildingPrefab, level);
                entityManager.SetComponentData(entity, buildingPropertyData);
            }
            return false; // Skip original
        }
    }

    [HarmonyPatch(typeof(ZoneProperties), nameof(ZoneProperties.GetBuildingArchetypeComponents))]
    class WG_ZoneProperties_GetBuildingArchetypeComponents
    {
        [HarmonyPrefix]
        static bool GetBuildingArchetypeComponents(ref ZoneProperties __instance, HashSet<ComponentType> components, BuildingPrefab buildingPrefab, byte level)
        {
            if (!buildingPrefab.Has<BuildingProperties>())
            {
                BuildingPropertyData buildingPropertyData = WG_ZoneProperties_Static.GetBuildingPropertyData(__instance, buildingPrefab, level);
                BuildingProperties.AddArchetypeComponents(components, buildingPropertyData);
            }
            return false; // Skip original
        }
    }

    class WG_ZoneProperties_Static
    {
        static HashSet<string> uniqueCalcString = new HashSet<string>();

        // This is the function I'm actually after
        public static BuildingPropertyData GetBuildingPropertyData(ZoneProperties __instance, BuildingPrefab buildingPrefab, byte level)
        {
            // TODO - Find the Crane or Bounds object to change the residentialProperties and the spaceMultiplier

            // Buildings don't change every level, so only make it different at 1, 3 and 5.
            // And reduce the increase until I figure out how to get the building height
            float baseNum = 1.375f;
            float levelBooster = 0.125f;
            float residentialProperties = __instance.m_ResidentialProperties;
            float num = residentialProperties; // Was 1f, combine the multiplication below
            float lotSize = (float)buildingPrefab.lotSize;
            List<ComponentBase> ogd = new List<ComponentBase>();
            if (__instance.m_ScaleResidentials)
            {
                if (buildingPrefab.GetComponents(ogd))
                {
                    /*
                    Game.Prefabs.BuildingPrefab <- Values may be here
                    Game.Prefabs.SpawnableBuilding <- Values may be here
                    Game.Prefabs.ObjectSubObjects - Wonder if I can shake out the building only from here
                    Game.Prefabs.ObjectSubAreas
                    Game.Prefabs.ObjectSubNets
                     */
                    foreach (ComponentBase item in ogd)
                    {
                        //System.Console.WriteLine(item.GetType().ToString());
                        switch (item.GetType().ToString())
                        {
                            case "Game.Prefabs.BuildingPrefab":
                                Game.Prefabs.BuildingPrefab bp = (Game.Prefabs.BuildingPrefab)item;
                                foreach (var mesh in bp.m_Meshes)
                                {
                                    var components = mesh.m_Mesh.prefab.components;
                                    foreach (var component in components)
                                    {
                                        if (component.name == "LodProperties")
                                        {
                                            var lod = (LodProperties)component;
                                            var prefab = (Game.Prefabs.RenderPrefab)lod.prefab;
                                            System.Console.WriteLine(bp.name + " height: " + prefab.bounds.y.max);
                                            // prefab.bounds.y appears to be the height of the prefab
                                            
                                        }
                                    }
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }

                // m_ResidentialProperties appears to signify the type of residential
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
                        lotSize = math.min(buildingPrefab.m_LotDepth, 3);
                        break;
                    case 1.5f:
                        residentialProperties = 1f;
                        break;
                    case 2f:
                        // Mixed use should be slightly more than medium density since the buildings are usually wall to wall)
                        residentialProperties = 1.25f;
                        // TOOD - Find prefab 
                        break;
                    case 4f:
                        baseNum = 1.20f;
                        levelBooster = 0.01f; // The building height does not change
                        residentialProperties = 3f; // Shoebox
                        break;
                    case 6f:
                        // Reduce residentialProperties to lower if constrained to a short building by tweaking the multiplier by lot size
                        // Small footprint buildings are difficult to make tall and stable
                        // TODO - Really find the prefab height
                        baseNum = 2.15f;
                        levelBooster = 0.05f;
                        residentialProperties = math.min((buildingPrefab.m_LotWidth + buildingPrefab.m_LotDepth) / 2, 5f);
                        /*
                        if (lotSize == 324)
                        {
                            // Really nerf Glass Crown. This is a bad solution
                            lotSize = 25f;
                        }
                        */
                        break;
                        // No default
                }

                num = (baseNum + (levelBooster * level)) * lotSize * residentialProperties;
                // Cap signature buildings with this division. Not great, not terrible.
                if (num > 1000)
                {
                    // Gently scale down the value for very large buildings
                    num /= math.log10(num);
                }
                /*
                string value = $"GetBuildingPropertyData {buildingPrefab.m_LotWidth}x{buildingPrefab.m_LotDepth} -> {num}";
                if (!uniqueCalcString.Contains(value))
                {
                    System.Console.WriteLine(value);
                    uniqueCalcString.Add(value);
                }
                */
            }

            BuildingPropertyData result = default(BuildingPropertyData);
            result.m_ResidentialProperties = (int)num;
            result.m_AllowedSold = EconomyUtils.GetResources(__instance.m_AllowedSold);
            result.m_AllowedManufactured = EconomyUtils.GetResources(__instance.m_AllowedManufactured);
            result.m_AllowedStored = EconomyUtils.GetResources(__instance.m_AllowedStored);
            result.m_SpaceMultiplier = __instance.m_SpaceMultiplier;
            return result;
        }
    }
}