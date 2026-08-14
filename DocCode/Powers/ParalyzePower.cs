using BaseLib.Abstracts;
using Doc.DocCode.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using System.Threading.Tasks;

namespace Doc.DocCode.Powers;

public sealed class ParalyzePower : CustomPowerModel
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override string? CustomPackedIconPath => "paralyze_power.png".PowerImagePath();
    public override string? CustomBigIconPath => "paralyze_power.png".PowerImagePath();

    public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (base.Owner != dealer)
        {
            return 0m;
        }
        if (props.HasFlag(ValueProp.Unblockable))
        {
            return 0m;
        }
        if (base.Amount <= 0)
        {
            return 0m;
        }

        // 返回负的amount，将伤害降至0
        return -amount+1;
    }

    public override async Task AfterDamageGiven(PlayerChoiceContext choiceContext, Creature? dealer, DamageResult result, ValueProp props, Creature target, CardModel? cardSource)
    {
        if (base.Owner != dealer)
        {
            return;
        }
        if (props.HasFlag(ValueProp.Unblockable))
        {
            return;
        }
        if (base.Amount <= 0)
        {
            return;
        }

        // 消耗1层麻痹
        await PowerCmd.ModifyAmount(choiceContext, this, -1m, null, null);
        if (base.Amount <= 0)
        {
            await PowerCmd.Remove(this);
        }
    }
}