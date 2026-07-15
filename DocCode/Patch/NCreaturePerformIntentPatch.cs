using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Combat;
using Doc.DocCode.Powers;

namespace Doc.DocCode.Patches;

[HarmonyPatch(typeof(NCreature))]
public static class NCreaturePerformIntentPatch
{
    // 明确指定方法名
    [HarmonyPatch("PerformIntent")]
    [HarmonyPrefix]
    public static bool Prefix(NCreature __instance, ref Task __result)
    {
        var creature = __instance.Entity;
        MainFile.Logger.Info($"[NCreaturePerformIntentPatch] Entered for {creature?.LogName ?? "null"}");

        if (creature != null && SleepWellPower.IsSleeping(creature))
        {
            MainFile.Logger.Info($"[NCreaturePerformIntentPatch] {creature.LogName} is sleeping, skipping PerformIntent");
            __result = Task.CompletedTask;
            return false; // 跳过原方法
        }

        return true;
    }
}