using BaseLib.Abstracts;
using Doc.DocCode.Extensions;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Doc.DocCode.Powers;

public sealed class SleepWellPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;
    public override string? CustomPackedIconPath => "sleep_well_power.png".PowerImagePath();
    public override string? CustomBigIconPath => "sleep_well_power.png".PowerImagePath();

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("Duration", 1m)
    ];

    //判断是否“好好睡”
    public static bool IsSleeping(Creature creature) => creature.HasPower<SleepWellPower>();


    //将所有生命损失降为0
    public override decimal ModifyHpLostAfterOsty(Creature target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        return 0;
    }

    public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        if (Owner.IsMonster)
        {
            await Owner.SleepWell();
            await RefreshIntent(Owner);
        }
    }


    //回合结束时处理下回合的“好好睡”意图
    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        // 只对怪物侧生效，且持有者必须参与本次结束
        if (side != CombatSide.Enemy) return;
        if (!participants.Contains(Owner)) return;
        if (!Owner.IsMonster || Owner.Monster == null) return;

        await base.Owner.SleepWell();
        await RefreshIntent(Owner);
    }

    //处理回合开始时层数减少和消失
    public override async Task BeforeSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (!participants.Contains(Owner)) return;

        if (Amount > 1)
        {
            await PowerCmd.TickDownDuration(this);
        }
        else
        {
            await PowerCmd.Remove(this);
        }
    }

    // ---------- 能力移除时清除意图 ----------
    public override async Task AfterRemoved(Creature oldOwner)
    {
        if (oldOwner.IsMonster && oldOwner.Monster != null)
        {
            // 重置状态机，让系统在下一回合重新生成正常意图
            oldOwner.Monster.ResetStateMachine();
            // 重新初始化状态机（这会清除我们设置的睡眠状态，恢复原始行为）
            oldOwner.Monster.SetUpForCombat();

            // 刷新意图图标（会显示新生成的意图）
            await RefreshIntent(oldOwner);
        }
    }

    private async Task RefreshIntent(Creature creature)
    {
        var creatureNode = creature.GetCreatureNode();
        if (creatureNode != null)
        {
            // 使用 TaskHelper.RunSafely 确保安全执行
            await TaskHelper.RunSafely(creatureNode.RefreshIntents());
        }
    }
}