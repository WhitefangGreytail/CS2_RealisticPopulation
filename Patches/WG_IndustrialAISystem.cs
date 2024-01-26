using HarmonyLib;
using UnityEngine;
using Game.Prefabs;
using Game.Simulation;
using Unity.Mathematics;
using Game.Economy;

namespace WG_CS2_RealisticPopulation.Patches
{
	[HarmonyPatch(typeof(IndustrialAISystem), nameof(IndustrialAISystem.GetFittingWorkers))]
	class IndustrialPatch
	{	
		// Could expand these to include different types of industry combinations
		const Resource OFFICE_INDUSTRY = Resource.Software | Resource.Telecom | Resource.Financial | Resource.Media;

		[HarmonyPrefix]
		static bool GetFittingWorkers_Prefix(ref int __result, BuildingData building, BuildingPropertyData properties, int level, IndustrialProcessData processData)
		{
			float baseMultiplier = 1.875f;
			float levelMultiplier = 0.125f;
			float spaceMultiplier = properties.m_SpaceMultiplier;
			float lotArea = building.m_LotSize.x * building.m_LotSize.y;

			// properties.m_AllowedManufactured gives a bit array of flags of what the building can be used for
			// m_AllowedManufactured represents what is allowed in the building and can span multiple industries
			if (((ulong)properties.m_AllowedManufactured & unchecked((ulong)OFFICE_INDUSTRY)) > 0)
			{
				// Accounting for the taller buildings. CS2's 'height' and boosting it for high density
				// TODO - If we can change the space multipler when loading the prefab (if it actually works this way), then we can remove most of this
				if (spaceMultiplier >= 4) // High density
                {
					baseMultiplier = 23.5f; // Absorbing the 10 previously multiplied in spaceMultiplier
					levelMultiplier = -1f; // Any change must also aborbing the 10 previously multiplied in spaceMultiplier.
					// Implicit floor in the divide 2
					// Overwrite the space multiplier entirely with pseudo height calc
					spaceMultiplier = (math.min((building.m_LotSize.x + building.m_LotSize.y) / 2, DataStore.maxOfficeBooster) - 1);
                }
                else
				{
					baseMultiplier = 4f;
				}
			}
			else if (lotArea > 36) {
				// Signature Industrial buildings
				baseMultiplier = 2.25f; // Won't give the same multipler as vanilla, feels okay for this number
				levelMultiplier = 0f;
			}

			// This result for a new building results in a company with 2/3 of the capacity in an established city. The rest of the capacity will be filled as the company grows larger
			__result = Mathf.CeilToInt(processData.m_MaxWorkersPerCell * lotArea * (baseMultiplier + levelMultiplier * (float)level) * spaceMultiplier);
			//System.Console.WriteLine($"I({level}) - {__result}: {processData.m_MaxWorkersPerCell},{building.m_LotSize.x},{building.m_LotSize.y},{level},{properties.m_SpaceMultiplier}");
			return false; // Skip original
		}
	}
}