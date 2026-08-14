using Doc.DocCode.Attributes;
using Doc.DocCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Doc.DocCode.Cards.Doctor;

[CardTags(isVictoria: true, isTara: true)]
public sealed class Reed() : DocCard(2, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("BloodOfBlightedMagicPower", 1m)
    ];
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<BloodOfBlightedMagicPower>(),
        HoverTipFactory.FromPower<SparkOfLifebeingsPower>()
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 获得1层枯萎魔血：受到未被格挡的非攻击伤害或生命流失时获得1层生灵火花
        // 叠层后每次触发获得对应层数的生灵火花
        await PowerCmd.Apply<BloodOfBlightedMagicPower>(choiceContext, Owner.Creature, DynamicVars["BloodOfBlightedMagicPower"].BaseValue, Owner.Creature, this);
    }
}