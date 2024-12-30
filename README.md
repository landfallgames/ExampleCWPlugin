Example Content Warning Steam Workshop Mod
===
This is an example mod for content warning to show how to use the modding API!

---

## This project

- ExampleCWPlugin.csproj: This specifies the location of Content Warning and its DLLs. If it's not installed in the default Steam location of `C:\Program Files (x86)\Steam\steamapps\common\Content Warning`, you can edit that here, or set the environment variable `CWDir` before opening the project.
    - There's also a build step that copies the built DLL and preview.png to the Content Warning Plugins directory when building, letting you easily test your code by just building then launching Content Warning.
- preview.png: This is the image used for your mod's steam workshop page when uploading the mod.
- ExampleCWPlugin.cs: The code for the mod. Check out the comments in the code for details!

---

## Mod load locations

Content Warning can load mods from 3 places:

- The Steam Workshop, which automatically handles downloading and installing mods you are subscribed to. (Mods are installed to a directory outside the game folder)
- Locally installed Steam Workshop mods. This is intended purely for development of mods, and is not expected to be used by regular players. Any mods put in the directory `C:\Program Files (x86)\Steam\steamapps\common\Content Warning\Plugins` will be loaded as if they're a Steam Workshop mod. If you're subscribed to a mod with the same GUID as a mod in the Plugins folder, the subscribed mod will not be loaded (the locally installed mod takes priority).
- Mods managed by BepInEx, MelonLoader, etc. - these are mostly ignored by the vanilla Content Warning modloader (as loading them is handled by BepInEx/etc.), except to show them in the list of mods installed.

---

## Uploading

To upload a mod to the Steam Workshop, first install the mod locally to the Content Warning local plugins directory (`C:\Program Files (x86)\Steam\steamapps\common\Content Warning\Plugins\YourPluginName`). An option to upload the mod (or update the published mod if you have already published it) will show up in the Mod Manager ingame. Make sure to include your preview.png in the plugin directory!

---

## Advanced topic: Preloaded mods

If something you're doing requires code to be run before Assembly-CSharp or another game assembly is loaded, consider using a preload assembly.

Any assemblies in the top level of your plugin folder whose filenames end with `.preload.dll` (case-sensitive) will have the static method `Preload.PreloadInit()` called (in any namespace). No assemblies will have been loaded into the mono runtime yet (except `mscorlib`) when the method is called. If you reference Assembly-CSharp types in your code, then it will load - it's on you to not load something if you need it not to be loaded. Keep in mind other mods' preloaders may have loaded the assembly you care about before you.

This is an advanced topic, and the debugging experience is subpar (view Player.log). Any breakage can easily cascade to the whole preload system failing. Only use the preload system if you know what you're doing, and be responsible.

(The preloader is implemented via a [native unity plugin](https://docs.unity3d.com/Manual/plug-ins-native.html), `preloader.dll`, that scans for and loads assemblies when `UnityPluginLoad` is called)
