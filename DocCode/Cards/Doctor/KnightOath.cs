using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Doc.DocCode.Attributes;
using Doc.DocCode.Extensions;
using Doc.DocCode.Powers;
using MegaCrit.Sts2.Core.Models;

namespace Doc.DocCode.Cards.Doctor;

[CardTags(isKazimierz: true)]
public sealed class KnightOath() : DocCard(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(2)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int drawCount = DynamicVars.Cards.IntValue;
        var drawnCards = await CardPileCmd.Draw(choiceContext, drawCount, Owner);

        bool hasKnight = drawnCards.Any(c => c.IsKnight());

        if (hasKnight)
        {
            await PowerCmd.Apply<NextKnightCardFreePower>(Owner.Creature, 1m, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1m);
    }
}