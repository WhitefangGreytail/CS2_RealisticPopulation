using Game;
using Game.Audio;
using Game.Prefabs;
using Game.Simulation;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Scripting;

namespace WG_CS2_RealisticPopulation
{
    public class DataStore
    {
        public static float officeBooster = 2.5f;
        public static int maxOfficeBooster = 6;

        public static Dictionary<float, float> averageBaseGameHeight = new Dictionary<float, float>(){
            {1f, 2f},
        };


        // TODO - Change to multiplier?
        public static Dictionary<string, int> householdCache = new Dictionary<string, int>();


        // Required when we move to writing out 
        public static Dictionary<string, int> defaultHousehold = new Dictionary<string, int>()
        {
            { "EU_ResidentialHighSignature01", 600 },
            { "EU_ResidentialHighSignature02", 600 },
            { "EU_ResidentialHighSignature03", 300 },
            { "NA_ResidentialHighSignature01", 400 },
            { "NA_ResidentialHighSignature02", 700 },
            { "NA_ResidentialHighSignature03", 600 },
        };
    }
}