using Game;
using Game.Audio;
using Game.Prefabs;
using Game.Simulation;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Scripting;

namespace WG_CS2_RealisticPopulation.Systems
{
    public partial class ChangeSpawnable : GameSystemBase
    {
        private EntityQuery m_SpawnableQuery;

        protected override void OnCreate()
        {
            base.OnCreate();

            m_SpawnableQuery = GetEntityQuery(ComponentType.ReadWrite<SpawnableBuildingData>());
        }

        protected override void OnUpdate()
        {
            // Nothing to do here
            //throw new System.NotImplementedException();
        }
    }
}
