using Doc.DocCode.Extensions;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Combat;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Doc.DocCode.Patches;

[HarmonyPatch(typeof(AsleepPower))]
public static class AsleepPowerPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(AsleepPower.AfterDamageReceived))]
    public static bool AfterDamageReceivedPrefix(
        AsleepPower __instance,
        PlayerChoiceContext choiceContext,
        Creature target,
        DamageResult result,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        if (target != __instance.Owner) return true;
        if (result.UnblockedDamage == 0) return true;

        var owner = __instance.Owner;
        if (owner == null || !owner.IsAlive) return true;

        bool isLagavulinMatriarch = owner.Monster is LagavulinMatriarch;

        if (isLagavulinMatriarch)
        {
            return true;
        }

        if (__instance.Amount > 1)
        {
            _ = PowerCmd.Decrement(__instance);
        }
        else
        {
            _ = PowerCmd.Remove(__instance);
            _ = CreatureCmd.TriggerAnim(owner, "Wake", 0.6f);
            owner.Monster.SetAwake(true);
            _ = CreatureCmd.Stun(owner);
        }

        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(AsleepPower.BeforeSideTurnEndVeryEarly))]
    public static bool BeforeSideTurnEndVeryEarlyPrefix(
        AsleepPower __instance,
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (!participants.Contains(__instance.Owner)) return true;

        var owner = __instance.Owner;
        if (owner == null || !owner.IsAlive) return true;

        if (owner.Monster is LagavulinMatriarch)
        {
            return true;
        }

        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(AsleepPower.AfterSideTurnEnd))]
    public static bool AfterSideTurnEndPrefix(
        AsleepPower __instance,
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (!participants.Contains(__instance.Owner)) return true;

        var owner = __instance.Owner;
        if (owner == null || !owner.IsAlive) return true;

        if (owner.Monster is LagavulinMatriarch)
        {
            return true;
        }

        if (__instance.Amount > 1)
        {
            _ = PowerCmd.Decrement(__instance);
        }
        else
        {
            _ = PowerCmd.Remove(__instance);
            if (!owner.Monster.IsAwake())
            {
                _ = CreatureCmd.TriggerAnim(owner, "Wake", 0.6f);
                owner.Monster.SetAwake(true);
            }
        }

        return false;
    }
}