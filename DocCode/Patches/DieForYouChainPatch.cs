using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Doc.DocCode.Patches;

/// <summary>
/// 补丁：在 Creature.LoseHpInternal 中拦截，实现链式承伤
/// 这是最终的伤害入口，所有修正（力量、格挡等）都已应用
/// </summary>
[HarmonyPatch(typeof(Creature), nameof(Creature.LoseHpInternal))]
public static class DieForYouChainPatch
{
    private static bool _isProcessing = false;

    [HarmonyPrefix]
    public static bool Prefix(Creature __instance, ref decimal amount, ValueProp props)
    {
        // 防止重入
        if (_isProcessing) return true;

        // 只处理玩家受到的攻击伤害
        if (__instance.Player == null) return true;
        if (!props.IsPoweredAttack()) return true;
        if (amount <= 0) return true;

        var player = __instance.Player;
        if (player == null || !player.Creature.IsAlive) return true;

        // 获取所有带有 DieForYouPower 的召唤物（最后召唤的优先）
        var dieForYouPets = GetDieForYouPets(player);
        if (dieForYouPets.Count == 0) return true;

        _isProcessing = true;
        try
        {
            decimal remaining = amount;
            Creature currentTarget = __instance;

            MainFile.Logger.Info($"DieForYouChain: Starting chain for {remaining} damage, {dieForYouPets.Count} pets available");

            // 遍历所有 DieForYouPower 召唤物
            foreach (var pet in dieForYouPets)
            {
                if (pet == null || pet.IsDead) continue;
                if (remaining <= 0) break;

                // 找到这个宠物的 DieForYouPower
                PowerModel dieForYouPower = null;
                foreach (var power in pet.Powers)
                {
                    if (IsDieForYouPower(power))
                    {
                        dieForYouPower = power;
                        break;
                    }
                }

                if (dieForYouPower == null) continue;

                // 调用 Power 的 ModifyUnblockedDamageTarget 判断是否应该转移
                Creature target = dieForYouPower.ModifyUnblockedDamageTarget(
                    currentTarget,
                    remaining,
                    props,
                    null);

                // 如果返回的是宠物自己，说明这个宠物要承受伤害
                if (target == pet && pet != currentTarget)
                {
                    MainFile.Logger.Info($"DieForYouChain: {pet.Name} (hp: {pet.CurrentHp}) absorbing {remaining}");

                    if (pet.CurrentHp >= (int)remaining)
                    {
                        // 能承受全部伤害
                        ApplyDamageToPet(pet, remaining);
                        remaining = 0;
                        MainFile.Logger.Info($"DieForYouChain: {pet.Name} absorbed all damage");
                        break;
                    }
                    else
                    {
                        // 只能承受部分伤害
                        decimal absorbed = pet.CurrentHp;
                        ApplyDamageToPet(pet, absorbed);
                        remaining -= absorbed;
                        MainFile.Logger.Info($"DieForYouChain: {pet.Name} died, absorbed {absorbed}, remaining {remaining}");
                        // 继续下一个宠物
                    }
                }
                else
                {
                    MainFile.Logger.Info($"DieForYouChain: {pet.Name} cannot absorb, checking next");
                }
            }

            // 更新 amount 为剩余伤害
            amount = remaining;
            MainFile.Logger.Info($"DieForYouChain: Final remaining damage to player: {amount}");

            if (amount <= 0)
            {
                // 伤害被完全吸收，跳过原方法
                return false;
            }

            // 剩余伤害继续执行原方法
            return true;
        }
        finally
        {
            _isProcessing = false;
        }
    }

    /// <summary>
    /// 判断是否是"为你而死"类型的 Power
    /// </summary>
    private static bool IsDieForYouPower(PowerModel power)
    {
        if (power == null) return false;
        var type = power.GetType();
        return type == typeof(DieForYouPower) ||
               type.Name == "BlazingSunDieForYouPower" ||
               type.Name == "DesertObeliskDieForYouPower";
    }

    /// <summary>
    /// 对宠物施加伤害
    /// </summary>
    private static void ApplyDamageToPet(Creature pet, decimal damage)
    {
        if (pet == null || pet.IsDead || damage <= 0) return;

        int newHp = pet.CurrentHp - (int)damage;
        if (newHp < 0) newHp = 0;

        var hpField = typeof(Creature).GetField("_currentHp",
            BindingFlags.NonPublic | BindingFlags.Instance);
        if (hpField != null)
        {
            hpField.SetValue(pet, newHp);
            MainFile.Logger.Info($"DieForYouChain: Set {pet.Name} HP to {newHp}");
        }

        if (pet.IsDead)
        {
            MainFile.Logger.Info($"DieForYouChain: {pet.Name} is dead, triggering Died event");
            var invokeDiedMethod = typeof(Creature).GetMethod("InvokeDiedEvent",
                BindingFlags.Public | BindingFlags.Instance);
            if (invokeDiedMethod != null)
            {
                invokeDiedMethod.Invoke(pet, new object[] { });
            }
        }
    }

    /// <summary>
    /// 获取所有带有 DieForYouPower 的召唤物（最后召唤的优先）
    /// </summary>
    private static List<Creature> GetDieForYouPets(Player player)
    {
        var result = new List<Creature>();
        var combatState = player.Creature.CombatState;
        if (combatState == null) return result;

        foreach (var c in combatState.Allies)
        {
            if (c.PetOwner != player) continue;
            if (!c.IsAlive) continue;

            foreach (var power in c.Powers)
            {
                if (IsDieForYouPower(power))
                {
                    result.Add(c);
                    break;
                }
            }
        }

        // 反转列表：最后召唤的优先
        result.Reverse();
        return result;
    }
}