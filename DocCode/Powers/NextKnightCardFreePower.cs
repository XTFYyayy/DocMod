using Doc.DocCode.Extensions;
using Godot;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using System.Threading.Tasks;
using BaseLib.Abstracts;

namespace Doc.DocCode.Powers;

public sealed class NextKnightCardFreePower : CustomPowerModel
{
    private bool _hasBeenUsed = false;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override bool AllowNegative => false;

    public override string? CustomPackedIconPath => "next_knight_card_free_power.png".PowerImagePath();
    public override string? CustomBigIconPath => "next_knight_card_free_power.png".PowerImagePath();

    public override bool TryModifyEnergyCostInCombat(CardModel card, decimal originalCost, out decimal modifiedCost)
    {
        modifiedCost = originalCost;

        if (_hasBeenUsed) return false;
        if (!card.IsKnight()) return false;
        if (card.Owner.Creature != Owner) return false;

        modifiedCost = 0;
        _hasBeenUsed = true;
        RemoveInternal();

        return true;
    }
}