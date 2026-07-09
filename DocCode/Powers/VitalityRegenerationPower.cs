using BaseLib.Abstracts;
using Doc.DocCode.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Doc.DocCode.Powers;

public sealed class VitalityRegenerationPower : CustomPowerModel
{
    private CardModel _sourceCard;
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    private decimal HealAmount => base.Amount;

    public override string? CustomPackedIconPath => "vitality_regeneration_power.png".PowerImagePath();
    public override string? CustomBigIconPath => "vitality_regeneration_power.png".PowerImagePath();

    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? source, CardModel? card)
    {
        if (target == base.Owner && result.UnblockedDamage > 0)
        {
            await CreatureCmd.Heal(base.Owner, HealAmount);
            Flash();
        }
    }
}