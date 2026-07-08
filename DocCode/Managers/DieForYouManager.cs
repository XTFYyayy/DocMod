using Doc.DocCode.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.ValueProps;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Doc.DocCode.Managers;

public static class DieForYouManager
{
    private static readonly HashSet<Type> _dieForYouPowerTypes = new HashSet<Type>
    {
        typeof(BlazingSunDieForYouPower),
        typeof(DesertObeliskDieForYouPower),
    };

    public static bool IsDieForYouPower(PowerModel power)
    {
        if (power == null) return false;
        var powerType = power.GetType();
        foreach (var type in _dieForYouPowerTypes)
        {
            if (powerType == type || powerType.IsSubclassOf(type))
                return true;
        }
        return false;
    }

    public static List<Creature> GetPetsWithDieForYou(Player player)
    {
        if (player == null) return new List<Creature>();

        CombatState combatState = (CombatState)player.Creature.CombatState;
        if (combatState == null) return new List<Creature>();

        var result = new List<Creature>();
        foreach (var c in combatState.Allies)
        {
            if (c.PetOwner != player) continue;
            if (!c.IsAlive) continue;

            bool hasDieForYou = false;
            foreach (var power in c.Powers)
            {
                if (IsDieForYouPower(power))
                {
                    hasDieForYou = true;
                    MainFile.Logger.Info($"GetPetsWithDieForYou: {c.Name} has DieForYou Power: {power.GetType().Name}");
                    break;
                }
            }
            if (hasDieForYou)
            {
                result.Add(c);
            }
        }
        MainFile.Logger.Info($"GetPetsWithDieForYou: Found {result.Count} pets with DieForYou");
        return result;
    }

    public static List<Creature> GetPetsWithDieForYouReversed(Player player)
    {
        var pets = GetPetsWithDieForYou(player);
        pets.Reverse();
        return pets;
    }

    public static void DealDamageToPet(Creature pet, decimal damage)
    {
        if (pet == null || pet.IsDead || damage <= 0) return;

        int newHp = pet.CurrentHp - (int)damage;
        if (newHp < 0) newHp = 0;

        var hpField = typeof(Creature).GetField("_currentHp",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (hpField != null)
        {
            hpField.SetValue(pet, newHp);
        }

        if (pet.IsDead)
        {
            MainFile.Logger.Info($"DealDamageToPet: {pet.Name} died");

            // 触发死亡事件，让游戏引擎处理
            pet.InvokeDiedEvent();

            // 不要手动 QueueFree！
            // 让 NCombatRoom 的 CreatureRemoved 事件处理器来处理节点移除
            // 或者让 CombatState 的自然死亡流程来处理

            // 只隐藏节点，不删除
            var node = NCombatRoom.Instance?.GetCreatureNode(pet);
            if (node != null)
            {
                node.Visible = false;
                node.ToggleIsInteractable(on: false);
                MainFile.Logger.Info($"DealDamageToPet: {pet.Name} hidden");
            }
        }
    }

    public static decimal ProcessDamageChain(Player targetPlayer, decimal originalDamage, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (targetPlayer == null || originalDamage <= 0) return 0;

        var pets = GetPetsWithDieForYouReversed(targetPlayer);
        if (pets.Count == 0) return 0;

        decimal remainingDamage = originalDamage;
        decimal totalAbsorbed = 0;

        foreach (var pet in pets)
        {
            if (pet == null || pet.IsDead) continue;
            if (remainingDamage <= 0) break;

            MainFile.Logger.Info($"DieForYouChain: {pet.Name} (hp: {pet.CurrentHp}) absorbing {remainingDamage}");

            if (pet.CurrentHp >= remainingDamage)
            {
                DealDamageToPet(pet, remainingDamage);
                totalAbsorbed += remainingDamage;
                remainingDamage = 0;
                break;
            }
            else
            {
                decimal damageToPet = pet.CurrentHp;
                DealDamageToPet(pet, damageToPet);
                totalAbsorbed += damageToPet;
                remainingDamage -= damageToPet;
                MainFile.Logger.Info($"DieForYouChain: {pet.Name} died, remaining {remainingDamage}");
            }
        }

        MainFile.Logger.Info($"DieForYouChain: Total absorbed {totalAbsorbed}, remaining {remainingDamage}");
        return totalAbsorbed;
    }
}