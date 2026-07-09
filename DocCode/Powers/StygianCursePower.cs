using BaseLib.Abstracts;
using Doc.DocCode.Extensions;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using System.Threading.Tasks;

namespace Doc.DocCode.Powers;

public sealed class StygianCursePower : CustomPowerModel
{
    private CardModel _sourceCard;

    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override bool AllowNegative => false;

    public override string? CustomPackedIconPath => "stygian_curse_power.png".PowerImagePath();
    public override string? CustomBigIconPath => "stygian_curse_power.png".PowerImagePath();

    public void SetSourceCard(CardModel sourceCard)
    {
        _sourceCard = sourceCard;
    }

    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target != Owner) return;
        if (!props.HasFlag(ValueProp.Move)) return;
        if (dealer == null) return;

        await PowerCmd.Apply<DoomPower>(choiceContext, dealer, 8m, Owner, _sourceCard);
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side == CombatSide.Enemy)
        {
            await PowerCmd.TickDownDuration(this);
        }
    }
}