using Doc.DocCode.Cards.Doctor;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using System.Collections.Generic;

namespace Doc.DocCode.Patches;

[HarmonyPatch(typeof(CardModel))]
public static class CardModelTargetTypePatch
{
    [HarmonyPatch("TargetType", MethodType.Getter)]
    [HarmonyPostfix]
    public static void OverrideTargetType(CardModel __instance, ref TargetType __result)
    {
        // 只对 Estelle 生效
        if (__instance is Estelle estelle && estelle.IsUpgraded)
        {
            __result = TargetType.AllEnemies;
        }
    }
}