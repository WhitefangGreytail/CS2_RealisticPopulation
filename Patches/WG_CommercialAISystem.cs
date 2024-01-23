using HarmonyLib;
using UnityEngine;
using Game.Prefabs;
using Game.Companies;
using Game.Simulation;

namespace WG_CS2_RealisticPopulation.Patches
{
	[HarmonyPatch(typeof(CommercialAISystem), nameof(CommercialAISystem.GetFittingWorkers))]
	class CommercialPatch
	{
		[HarmonyPrefix]
		static bool GetFittingWorkers_Prefix(ref int __result, BuildingData building, BuildingPropertyData properties, int level, ServiceCompanyData serviceData)
		{
			// This result for a new building results in a company with 2/3 of the capacity. The rest of the capacity will be filled as the company grows larger
			// properties.m_ResidentialProperties is set is the building is mixed used. Current pre mod values were fine for me, so we restore it here
			float baseMultiplier = 2.375f;
			float levelStep = 0.125f;
			if (properties.m_ResidentialProperties > 0) // Mixed
			{
				baseMultiplier = .75f;
			}
			__result = Mathf.CeilToInt(serviceData.m_MaxWorkersPerCell * (float)building.m_LotSize.x * (float)building.m_LotSize.y * (baseMultiplier + levelStep * (float)level) * properties.m_SpaceMultiplier);
			//System.Console.WriteLine("C(" + level + ") - " + __result);
			return false; // Skip original
		}
	}
}