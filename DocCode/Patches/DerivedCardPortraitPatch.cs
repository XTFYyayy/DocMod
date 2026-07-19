using Doc.DocCode.Cards.Doctor;
using Doc.DocCode.Extensions;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace Doc.DocCode.Patches;

[HarmonyPatch]
public static class DerivedCardPortraitPatch
{
    [HarmonyPatch(typeof(CardModel), nameof(CardModel.PortraitPath), MethodType.Getter)]
    [HarmonyPostfix]
    public static void OverrideDerivedCardPortraitPath(CardModel __instance, ref string __result)
    {
        // 检查是否是衍生牌
        if (__instance is UndeclaredRage or UnexoneratedSorrow or UngloriousGlory)
        {
            var cardName = __instance.GetType().Name.ToLowerInvariant();
            __result = $"{cardName}.png".CardImagePath();
        }
    }
}