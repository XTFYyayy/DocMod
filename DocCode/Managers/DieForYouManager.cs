using Doc.DocCode.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.ValueProps;
using System.Collections.Generic;
using System.Linq;

namespace Doc.DocCode.Managers;

public static class DieForYouManager
{
    private static readonly System.Type[] _dieForYouPowerTypes = new System.Type[]
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
                    break;
                }
            }
            if (hasDieForYou)
            {
                result.Add(c);
            }
        }
        return result;
    }

    public static List<Creature> GetPetsWithDieForYouReversed(Player player)
    {
        var pets = GetPetsWithDieForYou(player);
        pets.Reverse();
        return pets;
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

            MainFile.Logger.Info($"DieForYouChain: {pet.Name} (hp: {pet.CurrentHp}) is absorbing {remainingDamage}");

            if (pet.CurrentHp >= remainingDamage)
            {
                DealDamageToPet(pet, remainingDamage);
                totalAbsorbed += remainingDamage;
                remainingDamage = 0;
                MainFile.Logger.Info($"DieForYouChain: {pet.Name} fully absorbed {totalAbsorbed}");
                break;
            }
            else
            {
                decimal damageToPet = pet.CurrentHp;
                DealDamageToPet(pet, damageToPet);
                totalAbsorbed += damageToPet;
                remainingDamage -= damageToPet;
                MainFile.Logger.Info($"DieForYouChain: {pet.Name} died, absorbed {damageToPet}, remaining {remainingDamage}");
            }
        }

        MainFile.Logger.Info($"DieForYouChain: Total absorbed {totalAbsorbed}, remaining to player {remainingDamage}");
        return totalAbsorbed;
    }

    private static void DealDamageToPet(Creature pet, decimal damage)
    {
        if (pet == null || pet.IsDead || damage <= 0) return;

        // 使用反射设置 _currentHp
        var hpField = typeof(Creature).GetField("_currentHp",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (hpField != null)
        {
            int newHp = pet.CurrentHp - (int)damage;
            if (newHp < 0) newHp = 0;
            hpField.SetValue(pet, newHp);
            MainFile.Logger.Info($"DieForYouChain: Set {pet.Name} HP to {newHp}");
        }

        // 如果宠物死亡，触发官方事件
        if (pet.IsDead)
        {
            MainFile.Logger.Info($"DieForYouChain: {pet.Name} is dead, triggering Died event");

            // 调用官方的事件触发方法
            var invokeDiedMethod = typeof(Creature).GetMethod("InvokeDiedEvent",
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            if (invokeDiedMethod != null)
            {
                invokeDiedMethod.Invoke(pet, new object[] { });
                MainFile.Logger.Info($"DieForYouChain: Invoked Died event for {pet.Name}");
            }

            // 视觉反馈：隐藏节点（但不移除）
            var creatureNode = NCombatRoom.Instance?.GetCreatureNode(pet);
            if (creatureNode != null)
            {
                creatureNode.ToggleIsInteractable(on: false);
                creatureNode.Visible = false;
            }
        }
    }
}