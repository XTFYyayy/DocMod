// DocDfc.cs - 生存
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Doc.DocCode.Cards.Doctor;

public sealed class Dfc() : CardModel(1, CardType.Skill, CardRarity.Token, TargetType.Self)
{
    public override bool GainsBlock => true;

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(8m, ValueProp.Move),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(base.Owner.Creature, DynamicVars.Block, cardPlay);

        CardModel toDiscard = (await CardSelectCmd.FromHandForDiscard(choiceContext, base.Owner,
            new CardSelectorPrefs(CardSelectorPrefs.DiscardSelectionPrompt, 1), null, this)).FirstOrDefault();

        if (toDiscard != null)
            await CardCmd.Discard(choiceContext, toDiscard);

        if (PileType.Hand.GetPile(base.Owner).IsEmpty)
            await CardPileCmd.Draw(choiceContext, 1, base.Owner);

    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
    }
}