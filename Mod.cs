using Colossal.IO.AssetDatabase;
using Colossal.Logging;
using Game;
using Game.Modding;
using Game.SceneFlow;
using HarmonyLib;
using System;
using System.Reflection;
using Unity.Entities;
using UnityEngine;
using WG_CS2_RealisticPopulation;

namespace RealisticPopulation
{
    public class Mod : IMod
    {
        public static ILog log = LogManager.GetLogger($"{nameof(RealisticPopulation)}.{nameof(Mod)}").SetShowsErrorsInUI(false);
        private Setting m_Setting;

        public void OnLoad(UpdateSystem updateSystem)
        {

            var harmony = new Harmony("WG_RealisticPopulation");
            Harmony.DEBUG = true;
            var assembly = Assembly.GetExecutingAssembly();
            harmony.PatchAll(assembly);

            if (GameManager.instance.modManager.TryGetExecutableAsset(this, out var asset))
                log.Info($"Current mod asset at {asset.path}");

            m_Setting = new Setting(this);
            m_Setting.RegisterInOptionsUI();
            GameManager.instance.localizationManager.AddSource("en-US", new LocaleEN(m_Setting));

            foreach (var item in DataStore.defaultHousehold)
            {
                DataStore.householdCache.Add(item.Key, item.Value);
            }

            //Game.Prefabs.ZoneProperties.

            AssetDatabase.global.LoadSettings(nameof(RealisticPopulation), m_Setting, new Setting(this));
            log.Info(nameof(OnLoad) + " loaded.");
        }

        public void OnDispose()
        {
            log.Info(nameof(OnDispose));
            if (m_Setting != null)
            {
                m_Setting.UnregisterInOptionsUI();
                m_Setting = null;
            }
        }
    }
}
