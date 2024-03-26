using Game;
using Game.Audio;
using Game.Prefabs;
using Game.Simulation;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Scripting;

namespace WG_CS2_RealisticPopulation
{
    public struct ResidentialType
    {
        float sqmPerHousehold;
        float floorHeight;
        float commonAreaPerFloor; // Should have max of 0.2
    }

    

    public class DataStore
    {
        public static bool fullCapacity = false;
        public static int maxOfficeBooster = 5;
        public static Dictionary<int, ResidentialType> residentialInfo = new Dictionary<int, ResidentialType>();
        //spacePerHousehold
        //percentage of building used for common area

        // Settings
        /* Residential
         * Low density houses (1, 2)
         * Row houses (1x -> 2x)
         * Medium density
         * Mixed 
         * 
         */

        /* 
         * Commercial
         * 
         */


        /* 
         * Industrial
         * 
         */

        /*
         * Office
         * Enable Psuedo height checkbox
         * Slider for multiplers - 0.1 (cheating for signature building), 0.5, 1 (original) -> 4 (matching reality, but not really playable)
         * 
         * 
         */
    }
}