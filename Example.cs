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
[ContentWarningPlugin("ExampleCWPlugin", "0.1", false)]
public class Plugin
{
    static Plugin()
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
    
    // Be careful when patching methods, as, if you don't have the correct method name, then Harmony will fail to find it.
    // This can be done in two ways:
    // a) Use nameof() to get the method name.
    //  This is safer than using strings as, if the method name changes in a game update, the compiler
    //  will catch it and your IDE will display it in red.
    //
    // b) Use strings, but make sure to update them if the method name changes.
    //  Harmony, as said before, will fail to find the method if you do not!
    //  This is why it's recommended to use nameof() instead.
    [HarmonyPatch(nameof(Flashlight.Update))]
    [HarmonyPrefix]
    private static bool UpdatePrefix(Flashlight __instance)
    {
        exampleSetting ??= GameHandler.Instance.SettingsHandler.GetSetting<ExampleSetting>();

        // This is where BepInEx's AssemblyPublicizer comes in handy!
        // m_batteryEntry would usually be private, but we can access it.
        var bat = __instance.m_batteryEntry;
        bat.m_charge = Mathf.Max(bat.m_charge, bat.m_maxCharge * (exampleSetting.Value / 100));
        
        return true;
    }
}

// Don't forget to inherit from IExposedSetting too!
[ContentWarningSetting]
public class ExampleSetting : FloatSetting, IExposedSetting {
    public override void ApplyValue() => Debug.Log($"omg, mod setting changed to {Value}");
    
    public override float GetDefaultValue() => 100;
    public override float2 GetMinMaxValue() => new(0, 100);
    
    // Prefer using the Mods category
    public SettingCategory GetSettingCategory() => SettingCategory.Mods;
    
    public string GetDisplayName() => "Example mod setting";
}
