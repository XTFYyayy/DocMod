using Doc.DocCode.Extensions;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using System.Threading.Tasks;
using BaseLib.Abstracts;  

namespace Doc.DocCode.Powers;

public sealed class RatSwarmPower : CustomPowerModel
{
    private CardModel _sourceCard;
    private bool _shouldRemove = false;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override bool AllowNegative => false;
    public override string? CustomPackedIconPath => "rat_swarm_power.png".PowerImagePath();
    public override string? CustomBigIconPath => "rat_swarm_power.png".PowerImagePath();

    public void SetSourceCard(CardModel sourceCard)
    {
        _sourceCard = sourceCard;
    }

    public override decimal ModifyHpLostAfterOstyLate(Creature target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target != Owner) return amount;
        if (props.HasFlag(ValueProp.Unpowered)) return amount;

        decimal currentAmount = Amount;

        if (amount > currentAmount)
        {
            _shouldRemove = true;
        }

        return 0m;
    }

    public override async Task AfterModifyingHpLostAfterOsty()
    {
        if (_shouldRemove)
        {
            SetAmount(0);
            RemoveInternal();
        }
        _shouldRemove = false;
    }
}