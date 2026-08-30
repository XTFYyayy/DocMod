using Doc.DocCode.Attributes;
using Doc.DocCode.Cards;
using Doc.DocCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Doc.DocCode.Cards.Doctor;
[CardTags(isRhodeIsland: true)]

public sealed class Castle_3 () : DocCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<StrengthPower>(1m),
        new PowerVar<DexterityPower>(1m),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<StrengthPower>(),
        HoverTipFactory.FromPower<DexterityPower>()
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);
        decimal baseValue = base.DynamicVars.Strength.BaseValue;
        await PowerCmd.Apply<StrengthPower>(choiceContext, Owner.Creature, base.DynamicVars.Strength.BaseValue, Owner.Creature, this);
        await PowerCmd.Apply<Castle_3StrengthPower>(choiceContext, Owner.Creature, baseValue, Owner.Creature, this);
        baseValue = base.DynamicVars.Dexterity.BaseValue;
        await PowerCmd.Apply<DexterityPower>(choiceContext,Owner.Creature, base.DynamicVars.Dexterity.BaseValue, base.Owner.Creature, this);
        await PowerCmd.Apply<Castle_3DexterityPower>(choiceContext, Owner.Creature, baseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Strength.UpgradeValueBy(1m);
        base.DynamicVars.Dexterity.UpgradeValueBy(1m);
    }
}