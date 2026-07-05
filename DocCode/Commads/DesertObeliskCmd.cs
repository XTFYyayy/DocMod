using Doc.DocCode.Managers;
using Doc.DocCode.Monsters;
using Doc.DocCode.Powers;
using Doc.Nodes;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using System;
using System.Threading.Tasks;

namespace Doc.DocCode.Commands;

public static class DesertObeliskCmd
{
    public static async Task<SummonResult> Summon(PlayerChoiceContext choiceContext, Player summoner, decimal amount, CardModel? source)
    {
        if (summoner == null || summoner.Creature == null || summoner.Creature.IsDead)
        {
            MainFile.Logger.Warn("DesertObeliskCmd: summoner is null or dead");
            return new SummonResult(null, 0m);
        }

        CombatState combatState = (CombatState)summoner.Creature.CombatState;
        if (combatState == null)
        {
            MainFile.Logger.Warn("DesertObeliskCmd: combatState is null");
            return new SummonResult(null, 0m);
        }

        amount = Hook.ModifySummonAmount(combatState, summoner, amount, source);

        if (amount == 0m)
        {
            return new SummonResult(null, 0m);
        }

        // 注意：不再检查是否已存在沙之碑，每次召唤都创建新的

        // 创建新的沙之碑
        var obelisk = await PlayerCmd.AddPet<DesertObelisk>(summoner);

        // 等待一帧，确保 Creature 节点完全初始化
        await Cmd.CustomScaledWait(0.1f, 0.1f);

        // 检查创建的宠物是否还活着
        if (obelisk == null || obelisk.IsDead)
        {
            MainFile.Logger.Warn("DesertObeliskCmd: created obelisk is null or dead");
            return new SummonResult(null, 0m);
        }

        // 获取 Creature 节点并播放入场动画
        NCreature? obeliskNode = NCombatRoom.Instance?.GetCreatureNode(obelisk);
        if (obeliskNode != null)
        {
            obeliskNode.Modulate = Colors.Transparent;
            Tween tween = obeliskNode.CreateTween();
            tween.TweenProperty(obeliskNode, "modulate:a", 1, 0.35f).From(0);
            ShowHealthBar(obeliskNode);
            MainFile.Logger.Info("DesertObelisk spawned");
        }

        // 设置生命值
        await CreatureCmd.SetMaxHp(obelisk, amount);
        await CreatureCmd.Heal(obelisk, amount, false);

        // 施加"为你而死" Power
        await PowerCmd.Apply<DesertObeliskDieForYouPower>(choiceContext, obelisk, 1m, summoner.Creature, source);

        // 施加攻击追加 Power
        await PowerCmd.Apply<DesertObeliskAttackAppendPower>(choiceContext, obelisk, 1m, summoner.Creature, source);

        // 更新所有召唤物站位
        MainFile.Logger.Info($"=== About to call UpdateAllSummonPositions ===");
        await SummonPositionManager.UpdateAllSummonPositions(summoner);
        MainFile.Logger.Info($"=== UpdateAllSummonPositions completed ===");

        CombatManager.Instance.History.Summoned(combatState, (int)amount, summoner);
        await Hook.AfterSummon(combatState, choiceContext, summoner, amount);
        return new SummonResult(obelisk, amount);
    }

    private static void ShowHealthBar(NCreature creatureNode)
    {
        try
        {
            var stateDisplay = creatureNode.GetNodeOrNull<NCreatureStateDisplay>("%HealthBar");
            if (stateDisplay != null)
            {
                stateDisplay.Visible = true;
                stateDisplay.Modulate = Colors.White;

                var method = typeof(NCreatureStateDisplay).GetMethod("AnimateIn",
                    System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance,
                    null, new[] { typeof(int) }, null);

                if (method != null)
                {
                    method.Invoke(stateDisplay, new object[] { 2 });
                }
                else
                {
                    stateDisplay.Visible = true;
                }
            }
        }
        catch (Exception e)
        {
            MainFile.Logger.Warn($"Failed to show health bar: {e.Message}");
        }
    }
}