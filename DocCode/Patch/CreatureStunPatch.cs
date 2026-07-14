using Doc.DocCode.Extensions;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Doc.DocCode.Patches;

[HarmonyPatch(typeof(Creature))]
public static class CreatureStunPatch
{
    [HarmonyPrefix]
    [HarmonyPatch("StunInternal")]
    public static bool StunInternalPrefix(
        Creature __instance,
        Func<IReadOnlyList<Creature>, Task> stunMove,
        string? nextMoveId)
    {
        // 只处理普通怪物（非乐嘉族母）
        if (__instance.Monster == null) return true;
        if (__instance.Monster is LagavulinMatriarch) return true;

        // 普通怪物：直接设置 STUNNED 状态
        if (__instance.CombatState != null && !__instance.IsDead)
        {
            if (string.IsNullOrEmpty(nextMoveId))
            {
                var stateLog = __instance.Monster.MoveStateMachine.StateLog;
                nextMoveId = stateLog.Count > 0 ? stateLog[stateLog.Count - 1].Id : null;
            }

            var state = new MoveState("STUNNED", stunMove, new StunIntent())
            {
                FollowUpStateId = nextMoveId,
                MustPerformOnceBeforeTransitioning = true
            };
            __instance.Monster.SetMoveImmediate(state);

            // 刷新意图
            var nCreature = __instance.GetCreatureNode();
            if (nCreature != null)
            {
                _ = nCreature.RefreshIntents();
            }
        }

        // 跳过原方法
        return false;
    }
}