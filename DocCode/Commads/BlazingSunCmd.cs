using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using Doc.DocCode.Monsters;
using Doc.DocCode.Powers;
using Doc.Nodes;

namespace Doc.DocCode.Commands;

public static class BlazingSunCmd
{
    public static async Task<SummonResult> Summon(PlayerChoiceContext choiceContext, Player summoner, decimal amount, CardModel? source)
    {
        CombatState combatState = (CombatState)summoner.Creature.CombatState;
        amount = Hook.ModifySummonAmount(combatState, summoner, amount, source);

        if (amount == 0m)
        {
            return new SummonResult(null, 0m);
        }

        // 检查是否已有耀阳
        var existingSun = combatState.Allies.FirstOrDefault(c =>
            c.Monster is BlazingSun && c.PetOwner == summoner);

        Creature sun;

        if (existingSun != null && existingSun.IsAlive)
        {
            sun = existingSun;
            await CreatureCmd.GainMaxHp(sun, amount);
            await CreatureCmd.Heal(sun, amount);
        }
        else
        {
            bool isReviving = existingSun != null;
            if (isReviving)
            {
                if (existingSun.IsAlive)
                {
                    throw new InvalidOperationException("BlazingSun is already alive!");
                }
                sun = existingSun;
                summoner.PlayerCombatState.AddPetInternal(sun);
            }
            else
            {
                // 创建新的耀阳 - 视觉已在 MonsterModelPatch 中创建
                sun = await PlayerCmd.AddPet<BlazingSun>(summoner);

                // 等待一帧，确保 Creature 节点完全初始化
                await Cmd.CustomScaledWait(0.1f, 0.1f);

                // 获取 Creature 节点并播放入场动画
                NCreature? sunNode = NCombatRoom.Instance?.GetCreatureNode(sun);
                if (sunNode != null)
                {
                    // 耀阳从透明出现
                    sunNode.Modulate = Colors.Transparent;
                    Tween tween = sunNode.CreateTween();
                    tween.TweenProperty(sunNode, "modulate:a", 1, 0.35f).From(0);

                    // 直接显示血条 - 通过 GetNode 获取血条控件
                    ShowHealthBar(sunNode);

                    MainFile.Logger.Info("BlazingSun spawned with visuals");
                }
                else
                {
                    MainFile.Logger.Warn("Could not get NCreature node for BlazingSun");
                }
            }

            // 设置生命值
            await CreatureCmd.SetMaxHp(sun, amount);
            await CreatureCmd.Heal(sun, amount, isReviving);

            // 施加 Power
            await PowerCmd.Apply<BlazingSunDieForYouPower>(choiceContext, sun, 1m, summoner.Creature, source);
            await PowerCmd.Apply<NodPower>(choiceContext, sun, 1m, summoner.Creature, source);
        }

        CombatManager.Instance.History.Summoned(combatState, (int)amount, summoner);
        await Hook.AfterSummon(combatState, choiceContext, summoner, amount);
        return new SummonResult(sun, amount);
    }

    /// <summary>
    /// 直接显示血条（不使用反射）
    /// </summary>
    private static void ShowHealthBar(NCreature creatureNode)
    {
        try
        {
            // NCreature 中血条控件的路径是 "%HealthBar"
            // 这是一个 NCreatureStateDisplay 节点
            var stateDisplay = creatureNode.GetNodeOrNull<NCreatureStateDisplay>("%HealthBar");
            if (stateDisplay != null)
            {
                // 强制显示
                stateDisplay.Visible = true;
                stateDisplay.Modulate = Colors.White;

                // 调用 AnimateIn 播放淡入动画
                // HealthBarAnimMode 枚举: SpawnedAtCombatStart, SpawnedDuringCombat, FromHidden
                // 使用 FromHidden 会立即显示
                var method = typeof(NCreatureStateDisplay).GetMethod("AnimateIn",
                    System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance,
                    null, new[] { typeof(int) }, null);

                if (method != null)
                {
                    // HealthBarAnimMode.FromHidden = 2 (根据枚举值)
                    method.Invoke(stateDisplay, new object[] { 2 });
                    MainFile.Logger.Info("Health bar shown via AnimateIn(FromHidden)");
                }
                else
                {
                    // 如果找不到 AnimateIn 方法，直接设置可见
                    stateDisplay.Visible = true;
                    MainFile.Logger.Info("Health bar set visible directly");
                }
            }
            else
            {
                MainFile.Logger.Warn("Could not find %HealthBar node");
            }
        }
        catch (Exception e)
        {
            MainFile.Logger.Warn($"Failed to show health bar: {e.Message}");
        }
    }
}