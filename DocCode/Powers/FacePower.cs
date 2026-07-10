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

public sealed class FacePower : CustomPowerModel
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override string? CustomPackedIconPath => "face_power.png".PowerImagePath();
    public override string? CustomBigIconPath => "face_power.png".PowerImagePath();

    // 这名敌人对你造成的攻击伤害减半
    public override decimal ModifyDamageMultiplicative(Creature? target, decimal damage, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        // 只处理自己造成的伤害
        if (dealer != Owner) return 1;
        if (!props.HasFlag(ValueProp.Move)) return 1;

        // 排除反伤伤害（反伤带有 Unpowered 标志）
        if (props.HasFlag(ValueProp.Unpowered)) return 1;

        return (decimal)Math.Pow(0.5, (int)Amount);
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side == CombatSide.Enemy)
        {
            SetAmount(0);
            RemoveInternal();
        }
    }
}