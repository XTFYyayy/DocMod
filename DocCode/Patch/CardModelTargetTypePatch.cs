using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;

namespace Doc.DocCode.Patches;

[HarmonyPatch(typeof(CardModel))]
public static class CardModelTargetTypePatch
{
    private static TargetType? _overrideTargetType;

    public static void SetTargetType(CardModel card, TargetType targetType)
    {
        _overrideTargetType = targetType;
    }

    public static void ClearTargetType(CardModel card)
    {
        _overrideTargetType = null;
    }

    [HarmonyPatch("TargetType", MethodType.Getter)]
    [HarmonyPrefix]
    public static bool OverrideTargetType(CardModel __instance, ref TargetType __result)
    {
        if (_overrideTargetType.HasValue)
        {
            __result = _overrideTargetType.Value;
            return false;
        }
        return true;
    }
}