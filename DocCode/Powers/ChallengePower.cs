using BaseLib.Abstracts;
using Doc.DocCode.Extensions;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using System.Threading.Tasks;

namespace Doc.DocCode.Powers;

public sealed class ChallengePower : CustomPowerModel
{
    public override PowerType Type => PowerType.None;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override string? CustomPackedIconPath => "challenge_power.png".PowerImagePath();
    public override string? CustomBigIconPath => "challenge_power.png".PowerImagePath();

    //攻击造成双倍伤害
    public override decimal ModifyDamageAdditive(Creature? target, decimal damage, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        // 只处理自己造成的伤害
        if (dealer != Owner) return 0;
        if (!props.HasFlag(ValueProp.Move)) return 0;

        // 排除反伤伤害（反伤带有 Unpowered 标志）
        if (props.HasFlag(ValueProp.Unpowered)) return 0;

        return damage;
    }
    //受到双倍攻击伤害
    public override decimal ModifyDamageMultiplicative(Creature? target, decimal damage, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        return 2m;
    }
    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side == CombatSide.Enemy)
        {
            await PowerCmd.TickDownDuration(this);
        }
    }
}