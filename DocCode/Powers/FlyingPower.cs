using BaseLib.Abstracts;
using Doc.DocCode.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Doc.DocCode.Powers;

/// <summary>
/// 起飞（FlyingPower）：玩家身上的 Buff。
/// 效果：从攻击牌中受到的伤害减少 50%，且每受到一次不为 0 的攻击伤害时层数减 1。
/// </summary>
public sealed class FlyingPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override string? CustomPackedIconPath => "flying_power.png".PowerImagePath();
    public override string? CustomBigIconPath => "flying_power.png".PowerImagePath();

    /// <summary>
    /// 从攻击牌中受到的伤害减少 50%。
    /// 仅当自身（Owner）为目标、伤害为攻击伤害（带 Move 属性）时返回 0.5 倍，否则不修改。
    /// 该钩子返回的是伤害倍率：1 = 不变，0.5 = 减半。
    /// </summary>
    public override decimal ModifyDamageMultiplicative(Creature? target, decimal damage, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        // 只对拥有此 Power 的自身生效
        if (target != Owner) return 1m;
        if (!props.HasFlag(ValueProp.Move)) return 1m;
        if (cardSource?.Type != CardType.Attack) return 1m;

        return 0.5m;
    }

    /// <summary>
    /// 每受到一次不为 0 的攻击伤害时层数减 1；层数归 0 后移除。
    /// </summary>
    public override async Task AfterDamageGiven(PlayerChoiceContext choiceContext, Creature? dealer, DamageResult result, ValueProp props, Creature target, CardModel? cardSource)
    {

        if (target != Owner) return;
        if (!props.HasFlag(ValueProp.Move)) return;
        if (cardSource?.Type != CardType.Attack) return;
        if (result.UnblockedDamage <= 0) return;

        // 消耗 1 层起飞
        await PowerCmd.TickDownDuration(this);
        if (Amount <= 0)
        {
            await PowerCmd.Remove(this);
        }
    }
}
