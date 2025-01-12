using HarmonyLib;
using Unity.Mathematics;
using UnityEngine;
using Zorro.Settings;

// ExampleCWPlugin is mostly an example of how to use the modding API, not an actual serious mod.
// It adds a setting to the Mods settings page, which is a slider from 0 to 100.
// It then edits the Flashlight.Update method to prevent the battery of the flashlight
// from falling below that setting value.

namespace ExampleCWPlugin;

// The first argument is the GUID for this mod. This must be globally unique across all mods.
// Consider prefixing your name/etc. to the GUID. (or generate an actual GUID)

// vanillaCompatible: False means that this mod affects anything related to the multiplayer aspect of the game - for
// example, adjusting sprint regen, battery life, changing monster behavior, or anything similar or more major. Most
// mods leave this as false. 
// True means that this mod only affects this client. For example, a mod that changes the folder your clips are saved at.
// A good rule of thumb is: If someone else can tell you're using this mod, you must set vanillaCompatible to false.
// If you set this to true, and it should be false, your mod may be removed/banned from the workshop.
[ContentWarningPlugin("ExampleCWPlugin", "0.1", vanillaCompatible: false)]
public class ExampleCWPlugin
{
    static ExampleCWPlugin()
    {
        // Static constructors of types marked with ContentWarningPluginAttribute are automatically invoked on load.
        // Register callbacks, construct stuff, etc. here.
        Debug.Log("Hello from ExampleCWPlugin! This is called on plugin load");
        // Adding the [ContentWarningSetting] attribute to a setting class is basically the same as:
        // GameHandler.Instance.SettingsHandler.AddSetting(new ExampleSetting());
    }
}

// Harmony patches are automatically applied by the vanilla modloader.
// If 0Harmony.dll is already loaded by BepInEx, then BepInEx's harmony will be used instead.
[HarmonyPatch(typeof(Flashlight))]
public class FlashlightPatches
{
    private static ExampleSetting? exampleSetting;

    // Make sure the name matches when using a string in HarmonyPatch(). A good way to do so is by using nameof().
    [HarmonyPatch(nameof(Flashlight.Update))]
    [HarmonyPrefix]
    private static bool UpdatePrefix(Flashlight __instance)
    {
        exampleSetting ??= GameHandler.Instance.SettingsHandler.GetSetting<ExampleSetting>();
        // m_batteryEntry is public due to use of a publicizer
        var bat = __instance.m_batteryEntry;
        bat.m_charge = Mathf.Max(bat.m_charge, bat.m_maxCharge * (exampleSetting.Value / 100));

        return true;
    }
}

// Don't forget to inherit from IExposedSetting too!
[ContentWarningSetting]
public class ExampleSetting : FloatSetting, IExposedSetting
{
    public override void ApplyValue() => Debug.Log($"omg, mod setting changed to {Value}");

    protected override float GetDefaultValue() => 100;
    protected override float2 GetMinMaxValue() => new(0, 100);

    // Prefer using the Mods category
    public SettingCategory GetSettingCategory() => SettingCategory.Mods;

    public string GetDisplayName() => "Example mod setting";
}
