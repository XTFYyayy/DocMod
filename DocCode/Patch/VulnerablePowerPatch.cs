using Doc.DocCode.Powers;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Doc.DocCode.Patches;

[HarmonyPatch]
public static class VulnerablePowerPatch
{
    [HarmonyPatch(typeof(PowerModel), nameof(PowerModel.RemoveInternal))]
    [HarmonyPostfix]
    public static void OnPowerRemoved(PowerModel __instance)
    {
        // 添加空值检查
        if (__instance == null) return;
        if (__instance.Owner == null) return;

        if (__instance is VulnerablePower)
        {
            var armorBreak = __instance.Owner.GetPower<ArmorBreakPower>();
            armorBreak?.RemoveInternal();
        }
    }

    [HarmonyPatch(typeof(PowerModel), nameof(PowerModel.SetAmount))]
    [HarmonyPostfix]
    public static void OnPowerAmountChanged(PowerModel __instance, int amount)
    {
        // 添加空值检查
        if (__instance == null) return;
        if (__instance.Owner == null) return;

        if (__instance is VulnerablePower && amount <= 0)
        {
            var armorBreak = __instance.Owner.GetPower<ArmorBreakPower>();
            armorBreak?.RemoveInternal();
        }
    }
}