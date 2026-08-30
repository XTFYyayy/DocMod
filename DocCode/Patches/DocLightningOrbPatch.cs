using Doc.DocCode.Orbs;
using HarmonyLib;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Orbs;
using System.Collections.Generic;

namespace Doc.DocCode.Patches;

[HarmonyPatch]
public static class DocLightningOrbPatch
{
    // DocLightningOrb is a LightningOrb patch - reuses LightningOrb's icon/sprite/hover tip
    // OrbModel.Icon/CreateSprite/HoverTips are not virtual, so we patch them

    // 1. CreateSprite PREFIX: return LightningOrb's sprite for DocLightningOrb, skip original
    [HarmonyPatch(typeof(OrbModel), nameof(OrbModel.CreateSprite))]
    [HarmonyPrefix]
    public static bool CreateSprite(OrbModel __instance, ref Godot.Node2D __result)
    {
        if (__instance is DocLightningOrb)
        {
            __result = ModelDb.Orb<LightningOrb>().CreateSprite();
            return false; // skip original
        }
        return true;
    }

    // 2. Icon getter POSTFIX: redirect to LightningOrb
    [HarmonyPatch(typeof(OrbModel), nameof(OrbModel.Icon), MethodType.Getter)]
    [HarmonyPostfix]
    public static void RedirectIcon(OrbModel __instance, ref Godot.CompressedTexture2D __result)
    {
        if (__instance is DocLightningOrb)
        {
            __result = ModelDb.Orb<LightningOrb>().Icon;
        }
    }

    // 3. HoverTips getter POSTFIX: redirect to LightningOrb
    [HarmonyPatch(typeof(OrbModel), nameof(OrbModel.HoverTips), MethodType.Getter)]
    [HarmonyPostfix]
    public static void RedirectHoverTips(OrbModel __instance, ref IEnumerable<IHoverTip> __result)
    {
        if (__instance is DocLightningOrb)
        {
            __result = ModelDb.Orb<LightningOrb>().HoverTips;
        }
    }
}
