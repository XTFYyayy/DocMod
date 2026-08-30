using Doc.DocCode.Attributes;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.ValueProps;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Doc.DocCode.Cards.Doctor;

[CardTags(isRhodeIsland: true,isRimBilliton:true,isBabel:true)]
public sealed class Amiya() : DocCard(3, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Ethereal, CardKeyword.Sly];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(6m, ValueProp.Move),         
        new RepeatVar(4),
    ];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).WithHitCount(base.DynamicVars.Repeat.IntValue).FromCard(this)
             .Targeting(cardPlay.Target)
             .WithHitFx("vfx/vfx_bloody_impact")
             .Execute(choiceContext);
    }

    public override bool HasTurnEndInHandEffect => true;

    protected override void AddExtraArgsToDescription(LocString description)
    {
        decimal DamageInHand = 3* DynamicVars.Damage.BaseValue;
        description.Add("DamageInHand", DamageInHand);
        decimal HitCountInHand = DynamicVars.Repeat.BaseValue-1;
        description.Add("HitCountInHand", HitCountInHand);
    }
    protected override async Task OnTurnEndInHand(PlayerChoiceContext choiceContext)
    {
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue*3).WithHitCount((int)DynamicVars.Repeat.BaseValue - 1).FromCard(this)
        .TargetingAllOpponents(base.CombatState)
        .WithHitFx("vfx/vfx_bloody_impact")
        .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Repeat.UpgradeValueBy(1);
    }
}
