using BaseLib.Utils;
using Doc.DocCode.Attributes;
using Doc.DocCode.Powers;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Doc.DocCode.Cards.Doctor;

[CardTags(isKazimierz: true, isKnight: true)]
public sealed class Gravel() : DocCard(3, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust, CardKeyword.Sly];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("GravelThreshold", 2m)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<RatSwarmPower>()
    ];

    protected override void AddExtraArgsToDescription(LocString description)
    {
        decimal finalAmount = DynamicVars["GravelThreshold"].BaseValue;
        description.Add("FinalAmount", finalAmount);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var dexterityPower = base.Owner.Creature.GetPower<DexterityPower>();
        decimal dexterity = dexterityPower?.Amount ?? 0m;

        decimal amount = DynamicVars["GravelThreshold"].BaseValue + dexterity;

        var ratSwarmPower = await PowerCmd.Apply<RatSwarmPower>(base.Owner.Creature, amount, base.Owner.Creature, this);
        ratSwarmPower?.SetSourceCard(this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["GravelThreshold"].UpgradeValueBy(1m);
    }
}