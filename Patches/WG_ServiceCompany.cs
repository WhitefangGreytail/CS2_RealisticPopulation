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

namespace WG_CS2_RealisticPopulation.Patches
{
    [HarmonyPatch(typeof(ZoneProperties), nameof(ZoneProperties.InitializeBuilding))]
    class WG_ServiceCompany_Initialize
    {
        [HarmonyPrefix]
        static bool Initialize_Prefix((EntityManager entityManager, Entity entity) {
            ServiceCompanyData componentData = default(ServiceCompanyData);
            componentData.m_MaxService = m_MaxService;
            componentData.m_WorkPerUnit = 0;
            componentData.m_MaxWorkersPerCell = m_MaxWorkersPerCell;
            componentData.m_ServiceConsuming = m_ServiceConsuming;
            entityManager.SetComponentData(entity, componentData);
        }
    }
}
*/