using Doc.DocCode.Attributes;
using Doc.DocCode.Extensions;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using System.Threading.Tasks;

namespace Doc.DocCode.Cards.Doctor;

[CardTags(isMinos: true)]
public sealed class Varkaris() : DocCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int num = CardPile.MaxCardsInHand - base.Owner.PlayerCombatState.Hand.Cards.Count;
        var drawnCards = await CardPileCmd.Draw(choiceContext, num, base.Owner);

        if (num > 0)
        {
            await CardCmd.Discard(choiceContext,
                await CardSelectCmd.FromHandForDiscard(choiceContext, base.Owner,
                    new CardSelectorPrefs(CardSelectorPrefs.DiscardSelectionPrompt, num), null, this));
        }
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
