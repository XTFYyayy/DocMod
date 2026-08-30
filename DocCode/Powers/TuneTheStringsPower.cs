using BaseLib.Abstracts;
using Doc.DocCode.Extensions;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.ValueProps;
using System.Reflection;

namespace Doc.DocCode.Powers;

/// <summary>
/// 调弦（TuneTheStringsPower）：玩家 buff，攻击无视格挡，回合结束时层数减 1。
/// </summary>
public sealed class TuneTheStringsPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override string? CustomPackedIconPath => "tune_the_strings_power.png".PowerImagePath();
    public override string? CustomBigIconPath => "tune_the_strings_power.png".PowerImagePath();


    private static readonly FieldInfo DamagePropsField =
        typeof(AttackCommand).GetField("<DamageProps>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance)!;

    /// <summary>
    /// 攻击无视格挡：将 ValueProp.Unblockable 注入本次攻击的后台伤害属性。
    /// 照抄 NodPower 的反射注入写法，注入目标为 AttackCommand 中保存伤害属性的私有字段。
    /// </summary>
    public override async Task BeforeAttack(AttackCommand attackCommand)
    {
        // 只让主人（玩家）的攻击无视格挡
        if (attackCommand.Attacker != Owner) return;

        if (DamagePropsField != null)
        {
            var currentProps = (ValueProp)DamagePropsField.GetValue(attackCommand)!;
            DamagePropsField.SetValue(attackCommand, currentProps | ValueProp.Unblockable);
        }

        await Task.CompletedTask;
    }

    /// <summary>
    /// 回合结束时层数减 1；层数小于等于 0 时移除本 power。
    /// </summary>
    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (participants.Contains(Owner))
        {
            await PowerCmd.TickDownDuration(this);
            if (Amount <= 0)
            {
                await PowerCmd.Remove(this);
            }
        }
    }
}
