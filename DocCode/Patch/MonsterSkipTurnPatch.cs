using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Creatures;
using Doc.DocCode.Powers;

namespace Doc.DocCode.Patches;

[HarmonyPatch(typeof(Creature))]
public static class MonsterSkipTurnPatch
{
    // 明确指定方法名，避免 nameof 在异步方法上可能的问题
    [HarmonyPatch("TakeTurn")]
    [HarmonyPrefix]
    public static bool SkipTakeTurnIfSleeping(Creature __instance)
    {
        // 日志验证
        MainFile.Logger.Info($"[MonsterSkipTurnPatch] Entered TakeTurn for {__instance.LogName}");

        if (!__instance.IsMonster) return true;

        if (SleepWellPower.IsSleeping(__instance))
        {
            MainFile.Logger.Info($"[MonsterSkipTurnPatch] {__instance.LogName} is sleeping, skipping TakeTurn");
            // 重置 SpawnedThisTurn，让 CombatManager 认为已处理
            __instance.Monster?.OnSideSwitch();
            return false; // 跳过原方法
        }

        return true;
    }
}