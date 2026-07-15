using BaseLib.Abstracts;
using Doc.DocCode.Extensions;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Doc.DocCode.Powers;

/// <summary>
/// 好好睡 - 沉睡能力（多实例共存）
/// </summary>
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

    /// <summary>
    /// 判断目标是否处于沉睡状态（只要存在任意实例即返回true）
    /// </summary>
    public static bool IsSleeping(Creature creature)
    {
        return creature.HasPower<SleepWellPower>();
    }

    /// <summary>
    /// 回合结束时层数减1（每个实例独立计时）
    /// </summary>
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
}