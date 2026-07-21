using Doc.DocCode.Managers;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.ValueProps;
using System.Collections.Generic;

namespace Doc.DocCode.Patches;

[HarmonyPatch(typeof(Creature))]
public static class CreatureLoseHpPatch
{
    private static bool _isProcessing = false;

    [HarmonyPrefix]
    [HarmonyPatch(nameof(Creature.LoseHpInternal))]
    public static bool Prefix(Creature __instance, ref decimal amount, ValueProp props)
    {
        if (_isProcessing) return true;
        if (__instance.Player == null) return true;
        if (amount <= 0) return true;
        if (!props.HasFlag(ValueProp.Move)) return true; // 非攻击伤害，不处理

        Player targetPlayer = __instance.Player;

        var combatState = targetPlayer.Creature.CombatState as MegaCrit.Sts2.Core.Combat.CombatState;
        if (combatState == null) return true;

        // 收集所有带有 DieForYou Power 的宠物
        var petsWithDieForYou = new List<Creature>();
        foreach (var c in combatState.Allies)
        {
            if (c.PetOwner != targetPlayer) continue;
            if (!c.IsAlive) continue;

            bool hasDieForYou = false;
            foreach (var power in c.Powers)
            {
                if (DieForYouManager.IsDieForYouPower(power))
                {
                    hasDieForYou = true;
                    break;
                }
            }
            if (hasDieForYou)
            {
                petsWithDieForYou.Add(c);
            }
        }

        if (petsWithDieForYou.Count == 0) return true;

        // 反转顺序（最后召唤的优先）
        petsWithDieForYou.Reverse();

        _isProcessing = true;
        try
        {
            decimal remainingDamage = amount;

            foreach (var pet in petsWithDieForYou)
            {
                if (pet == null || pet.IsDead) continue;
                if (remainingDamage <= 0) break;

                if (pet.CurrentHp >= remainingDamage)
                {
                    DieForYouManager.DealDamageToPet(pet, remainingDamage);
                    remainingDamage = 0;
                    break;
                }
                else
                {
                    decimal damageToPet = pet.CurrentHp;
                    DieForYouManager.DealDamageToPet(pet, damageToPet);
                    remainingDamage -= damageToPet;
                }
            }

            if (remainingDamage <= 0)
            {
                amount = 0;
                MainFile.Logger.Info($"LoseHpPatch: Damage fully absorbed, amount set to 0");
                return true;
            }

            amount = remainingDamage;
            MainFile.Logger.Info($"LoseHpPatch: Remaining damage {remainingDamage} to player");
            return true;
        }
        finally
        {
            _isProcessing = false;
        }
    }
}