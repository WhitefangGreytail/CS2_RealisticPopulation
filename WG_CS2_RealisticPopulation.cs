using Colossal.IO.AssetDatabase;
using Game;
using Game.Audio;
using Game.Modding;
using Game.Prefabs;
using Game.Settings;
using Game.Simulation;
using System.Collections.Generic;
using System.Xml.Serialization;
using Unity.Entities;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Scripting;

namespace WG_CS2_RealisticPopulation
{
    public class WG_CS2_RealisticPopulation : IMod
    {
        //internal ModSettings currentSettings { get; private set; }

        public void OnCreateWorld(UpdateSystem updateSystem)
        {
            // Register mod settings to game options UI.
            //currentSettings = new(this);
            //currentSettings.RegisterInOptionsUI();

            // Load saved settings.
            //AssetDatabase.global.LoadSettings("WorkerCapacityBooster", currentSettings, new ModSettings(this));
        }

        public void OnDispose()
        {
        }

        public void OnLoad()
        {
            throw new System.NotImplementedException();
        }
    }

    /*
    [FileLocation(Mod.ModName)]
    public class WorkerModSettings : ModSetting
    {
        private bool _unlockAll = true;
        private bool _extraAtStart = false;
        private bool _milestones = false;

        public ModSettings(IMod mod)
            : base(mod)
        {
        }

        [SettingsUISection("UnlockMode")]
        public bool UnlockAll
        {
            get => _unlockAll;

            set
            {
                _unlockAll = value;

                // Assign contra value to ensure that JSON contains at least one non-default value.
                Contra = value;

                // Clear conflicting settings.
                if (value)
                {
                    _extraAtStart = false;
                    _milestones = false;
                }

                // Ensure state.
                EnsureState();
            }
        }

        [SettingsUISection("UnlockMode")]
        public bool ExtraTilesAtStart
        {
            get => _extraAtStart;

            set
            {
                _extraAtStart = value;

                // Clear conflicting settings.
                if (value)
                {
                    _unlockAll = false;
                    _milestones = false;
                }

                // Ensure state.
                EnsureState();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the entire map should be unlocked on load.
        /// </summary>
        [SettingsUISection("UnlockMode")]
        public bool AssignToMilestones
        {
            get => _milestones;

            set
            {
                _milestones = value;

                // Clear conflicting settings.
                if (value)
                {
                    _unlockAll = false;
                    _extraAtStart = false;
                }

                // Ensure state.
                EnsureState();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether there should be no unlocked starting tiles when starting a new map.
        /// </summary>
        [SettingsUIHideByCondition(typeof(ModSettings), nameof(StartingTilesHidden))]
        [SettingsUISection("StartingOptions")]
        public bool NoStartingTiles { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether, well, nothing really.
        /// This is just the inverse of <see cref="UnlockAll"/>, to ensure the the JSON contains at least one non-default value.
        /// This is to workaround a bug where the settings file isn't overwritten when there are no non-default settings.
        /// </summary>
        [SettingsUIHidden]
        public bool Contra { get; set; } = false;

        /// <summary>
        /// Sets a value indicating whether the mod's settings should be reset.
        /// </summary>
        [XmlIgnore]
        [SettingsUIButton]
        [SettingsUISection("ResetModSettings")]
        [SettingsUIConfirmation]
        public bool ResetModSettings
        {
            set
            {
                // Apply defaults.
                SetDefaults();

                // Ensure contra is set correctly.
                Contra = UnlockAll;

                // Save.
                ApplyAndSave();
            }
        }

        /// <summary>
        /// Restores mod settings to default.
        /// </summary>
        public override void SetDefaults()
        {
            _unlockAll = true;
            _extraAtStart = false;
            _milestones = false;

            NoStartingTiles = false;
        }

        /// <summary>
        /// Returns a value indicating whether the no starting tiles option should be hidden.
        /// </summary>
        /// <returns><c>true</c> (hide starting tiles option) if 'Unlock all tiles' is selected, <c>false</c> (don't hide) otherwise.</returns>
        public bool StartingTilesHidden() => UnlockAll;

        /// <summary>
        /// Enables Unlock All as the default option and that no options are duplicated.
        /// </summary>
        private void EnsureState()
        {
            if (!_unlockAll && !_extraAtStart && !_milestones)
            {
                UnlockAll = true;
            }
        }
    }*/
}