using BaseLib.Extensions;
using Doc.DocCode.Attributes;
using Doc.DocCode.Commands;
using Doc.DocCode.Extensions;
using Doc.DocCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Doc.DocCode.Cards.Doctor;

[CardTags(isSargon: true)]
public sealed class Beeswax() : DocCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("SummonAmount", 5m).WithTooltip("DOC-DESERT_OBELISK_SUMMON"),
        new DynamicVar("DesertObelisk",0).WithTooltip("DOC-DESERT_OBELISK")
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<DesertObeliskAttackAppendPower>(),
        HoverTipFactory.FromPower<DesertObeliskDieForYouPower>(),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        decimal amount = DynamicVars["SummonAmount"].BaseValue;
        await DesertObeliskCmd.Summon(choiceContext, Owner, amount, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["SummonAmount"].UpgradeValueBy(2m);
    }
}
