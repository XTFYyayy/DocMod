// TemporaryDexterityPowerIconPatch.cs
using Doc.DocCode.Extensions;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using System.Reflection;

namespace Doc.DocCode.Patches;

[HarmonyPatch]
public static class TemporaryDexterityPowerIconPatch
{
    private static MethodBase TargetMethod()
    {
        var type = typeof(PowerModel);
        return type.GetProperty("Icon")?.GetMethod;
    }

    [HarmonyPrefix]
    public static bool OverrideIcon(PowerModel __instance, ref Texture2D __result)
    {
        var typeName = __instance.GetType().Name;

        if (typeName == "Castle_3DexterityPower")
        {
            var path = "dexterity_loss_power.png".PowerImagePath();
            if (ResourceLoader.Exists(path))
            {
                __result = ResourceLoader.Load<Texture2D>(path);
                return false;
            }
        }

        return true;
    }
}