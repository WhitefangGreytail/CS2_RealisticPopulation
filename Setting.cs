using Colossal;
using Colossal.IO.AssetDatabase;
using Game.Modding;
using Game.Settings;
using Game.UI;
using Game.UI.InGame;
using Game.UI.Widgets;
using System.Collections.Generic;

namespace RealisticPopulation
{
    [FileLocation(nameof(RealisticPopulation))]
    [SettingsUIGroupOrder(kButtonGroup, kToggleGroup, householdSliderGroup)]
    [SettingsUIShowGroupName(kButtonGroup, kToggleGroup, householdSliderGroup)]
    public class Setting : ModSetting
    {
        public const string kSection = "Main";

        public const string kButtonGroup = "Button";
        public const string kToggleGroup = "Toggle";
        public const string householdSliderGroup = "Households"; // Slider for households
        public const string workSliderGroup = "Workplaces"; // Slider for households


        public Setting(IMod mod) : base(mod)
        {
            SetDefaults();
        }

        [SettingsUISection(kSection, kButtonGroup)]
        public bool Button { set { Mod.log.Info("Button clicked"); } }

        [SettingsUIButton]
        [SettingsUIConfirmation]
        [SettingsUISection(kSection, kButtonGroup)]
        public bool ButtonWithConfirmation { set { Mod.log.Info("ButtonWithConfirmation clicked"); } }

        [SettingsUISection(kSection, kToggleGroup)]
        public bool Toggle { get; set; }
        /*
        [SettingsUISlider(min = 100, max = 300, step = 2, scalarMultiplier = 1, unit = "sq. m")]
        [SettingsUISection(kSection, householdSliderGroup)]
        public int EUSlider { get; set; }

        [SettingsUISlider(min = 150, max = 350, step = 2, scalarMultiplier = 1, unit = "sq. m")]
        [SettingsUISection(kSection, householdSliderGroup)]
        public int NASlider { get; set; }
        */
        [SettingsUISlider(min = 50, max = 500, step = 1, scalarMultiplier = 1, unit = Unit.kPercentage)]
        [SettingsUISection(kSection, householdSliderGroup)]
        public int IndustrySlider { get; set; }

        [SettingsUISlider(min = 50, max = 500, step = 1, scalarMultiplier = 1, unit = Unit.kPercentage)]
        [SettingsUISection(kSection, workSliderGroup)]
        public int LD_CommercialSlider { get; set; }

        [SettingsUISlider(min = 50, max = 500, step = 1, scalarMultiplier = 1, unit = Unit.kPercentage)]
        [SettingsUISection(kSection, workSliderGroup)]
        public int HD_CommercialSlider { get; set; }

        [SettingsUISlider(min = 50, max = 500, step = 1, scalarMultiplier = 1, unit = Unit.kPercentage)]
        [SettingsUISection(kSection, workSliderGroup)]
        public int LD_OfficeSlider { get; set; }

        [SettingsUISlider(min = 50, max = 500, step = 1, scalarMultiplier = 1, unit = Unit.kPercentage)]
        [SettingsUISection(kSection, workSliderGroup)]
        public int HD_OfficSlider { get; set; }

        public override void SetDefaults()
        {
            // Defaults come from a 4 person household using
            // https://shrinkthatfootprint.com/how-big-is-a-house/
            // Looks like automatic saving of settings with this class
            /*
            if (EUSlider == 0)
            {
                EUSlider = DefaultValues.DEFAULT_EU_HOUSEHOLD_SPACE;
            }

            if (NASlider == 0)
            {
                NASlider = DefaultValues.DEFAULT_NA_HOUSEHOLD_SPACE;
            }
            */
        }
    }

    public class LocaleEN : IDictionarySource
    {
        private readonly Setting m_Setting;
        public LocaleEN(Setting setting)
        {
            m_Setting = setting;
        }
        public IEnumerable<KeyValuePair<string, string>> ReadEntries(IList<IDictionaryEntryError> errors, Dictionary<string, int> indexCounts)
        {
            return new Dictionary<string, string>
            {
                { m_Setting.GetSettingsLocaleID(), "Realistic Population" },
                { m_Setting.GetOptionTabLocaleID(Setting.kSection), "Main" },

                { m_Setting.GetOptionGroupLocaleID(Setting.kButtonGroup), "Buttons" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kToggleGroup), "Toggle" },
                { m_Setting.GetOptionGroupLocaleID(Setting.householdSliderGroup), "Space per household" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.Button)), "Button" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.Button)), $"Simple single button. It should be bool property with only setter or use [{nameof(SettingsUIButtonAttribute)}] to make button from bool property with setter and getter" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ButtonWithConfirmation)), "Button with confirmation" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ButtonWithConfirmation)), $"Button can show confirmation message. Use [{nameof(SettingsUIConfirmationAttribute)}]" },
                { m_Setting.GetOptionWarningLocaleID(nameof(Setting.ButtonWithConfirmation)), "is it confirmation text which you want to show here?" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.Toggle)), "Toggle" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.Toggle)), $"Use bool property with setter and getter to get toggable option" },
                /*
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.EUSlider)), "EU" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.EUSlider)), $"Space her household (in sq m.) for EU region" },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.NASlider)), "NA" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.NASlider)), $"Space her household (in sq m.) for NA region" },
                */
            };
        }

        public void Unload()
        {

        }
    }

    class DefaultValues
    {
        public static int DEFAULT_EU_HOUSEHOLD_SPACE = 160;
        public static int DEFAULT_NA_HOUSEHOLD_SPACE = 220;
    }
}
