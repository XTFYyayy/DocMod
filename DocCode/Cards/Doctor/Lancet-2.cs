using BaseLib.Utils;
using Doc.DocCode.Attributes;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.ValueProps;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Doc.DocCode.Cards;

[CardTags(isSargon: true)]
public sealed class Lancet_2() : DocCard(6, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Sly, CardKeyword.Ethereal];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new HealVar(6m)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);
        await CreatureCmd.Heal(base.Owner.Creature, DynamicVars.Heal.BaseValue);
    }

    protected override void OnUpgrade()
    {
        RemoveKeyword(CardKeyword.Ethereal);
    }
}