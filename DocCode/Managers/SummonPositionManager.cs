using Doc.DocCode.Powers;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Doc.DocCode.Managers;

/// <summary>
/// 召唤物站位管理器 - 负责所有召唤物的位置计算和更新
/// </summary>
public static class SummonPositionManager
{
    // 召唤物之间的固定间距
    private const float SUMMON_SPACING = 100f;

    // 第一个召唤物相对于玩家的偏移量
    private static readonly Vector2 BASE_OFFSET = new Vector2(80f, -30f);

    /// <summary>
    /// 更新指定玩家的所有召唤物站位
    /// </summary>
    public static async Task UpdateAllSummonPositions(Player summoner)
    {
        MainFile.Logger.Info($"=== SummonPositionManager.UpdateAllSummonPositions CALLED ===");

        if (summoner == null || summoner.Creature == null || summoner.Creature.IsDead)
        {
            MainFile.Logger.Warn("=== SummonPositionManager: summoner is null or dead ===");
            return;
        }

        CombatState combatState = (CombatState)summoner.Creature.CombatState;
        if (combatState == null) return;

        // 只获取存活的宠物
        var pets = combatState.Allies
            .Where(c => c.PetOwner == summoner && c.IsAlive)
            .ToList();

        if (pets.Count == 0) return;

        NCreature? playerNode = NCombatRoom.Instance?.GetCreatureNode(summoner.Creature);
        if (playerNode == null) return;

        MainFile.Logger.Info($"=== Player GlobalPosition: {playerNode.GlobalPosition} ===");

        Vector2 playerVisualPos = playerNode.GlobalPosition;

        for (int i = 0; i < pets.Count; i++)
        {
            Creature pet = pets[i];
            if (pet == null || pet.IsDead) continue;

            NCreature? petNode = NCombatRoom.Instance?.GetCreatureNode(pet);
            if (petNode == null)
            {
                MainFile.Logger.Warn($"=== Pet {i} node is null, skipping ===");
                continue;
            }

            Vector2 targetGlobal = playerVisualPos + new Vector2(
                BASE_OFFSET.X + i * SUMMON_SPACING,
                BASE_OFFSET.Y
            );

            MainFile.Logger.Info($"=== Pet {i} moving to {targetGlobal} (was {petNode.GlobalPosition}) ===");

            petNode.GlobalPosition = targetGlobal;
            RefreshHealthBar(petNode);
        }

        await Cmd.CustomScaledWait(0.1f, 0.1f);
    }

    private static void RefreshHealthBar(NCreature creatureNode)
    {
        try
        {
            if (creatureNode == null || creatureNode.Entity == null || creatureNode.Entity.IsDead) return;

            var stateDisplay = creatureNode.GetNodeOrNull<NCreatureStateDisplay>("%HealthBar");
            if (stateDisplay != null)
            {
                stateDisplay.Visible = true;
                stateDisplay.Modulate = Colors.White;

                var visuals = creatureNode.Visuals;
                if (visuals != null)
                {
                    var bounds = visuals.Bounds;
                    if (bounds != null)
                    {
                        stateDisplay.SetCreatureBounds(bounds);
                        return;
                    }
                }

                stateDisplay.SetCreatureBounds(creatureNode.Hitbox);
            }
        }
        catch (System.Exception e)
        {
            MainFile.Logger.Warn($"Failed to refresh health bar: {e.Message}");
        }
    }

    public static List<Creature> GetSummonedPets(Player summoner)
    {
        if (summoner == null || summoner.Creature == null || summoner.Creature.IsDead)
            return new List<Creature>();

        CombatState combatState = (CombatState)summoner.Creature.CombatState;
        if (combatState == null) return new List<Creature>();

        return combatState.Allies
            .Where(c => c.PetOwner == summoner && c.IsAlive)
            .ToList();
    }

    public static Creature? GetLastSummonedPet(Player summoner)
    {
        var pets = GetSummonedPets(summoner);
        return pets.LastOrDefault();
    }

    public static List<Creature> GetPetsWithDieForYouPower(Player summoner)
    {
        var pets = GetSummonedPets(summoner);
        return pets.Where(p => p.IsAlive && p.Powers.Any(power =>
            power is BlazingSunDieForYouPower ||
            power is DesertObeliskDieForYouPower))
            .ToList();
    }
}