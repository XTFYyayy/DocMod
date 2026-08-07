using BaseLib.Abstracts;
using Doc.DocCode.Extensions;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using System;

namespace Doc.DocCode.Powers;

/// <summary>
/// 火神之力：君王之剑耗能 -1（可叠加）
/// </summary>
public sealed class ForceModePower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override string? CustomPackedIconPath => "force_mode_power.png".PowerImagePath();
    public override string? CustomBigIconPath => "force_mode_power.png".PowerImagePath();

    public override bool TryModifyEnergyCostInCombat(CardModel card, decimal originalCost, out decimal modifiedCost)
    {
        modifiedCost = originalCost;
        if (card.Owner?.Creature != Owner) return false;
        if (card is not SovereignBlade) return false;

        modifiedCost = Math.Max(0m, originalCost - Amount);
        return true;
    }
}
