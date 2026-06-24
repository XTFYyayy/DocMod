using BaseLib.Utils;
using Doc.DocCode.Attributes;
using Doc.DocCode.Extensions;
using Doc.DocCode.Monsters;
using Doc.DocCode.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Doc.DocCode.Cards.Doctor;

[CardTags(isKazimierz: true, isKnight: true)]
public sealed class RadiantKnightNearl() : DocCard(3, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("SummonAmount", 30m)
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 检查上一张牌是否为卡西米尔势力
        var lastCard = GetLastPlayedCard();

        if (lastCard != null && lastCard.IsKazimierz())
        {
            // 上一张是卡西米尔势力：费用减1，不消耗
            EnergyCost.SetThisTurnOrUntilPlayed(CanonicalEnergyCost - 1);
            ExhaustOnNextPlay = false;
        }
        else
        {
            // 否则：消耗
            ExhaustOnNextPlay = true;
        }

        // 召唤或升级耀阳
        decimal summonAmount = DynamicVars["SummonAmount"].BaseValue;
        await OstyCmd.Summon(choiceContext, Owner, summonAmount, this);

        // 确保耀阳拥有【颔首】能力
        if (Owner.IsOstyAlive)
        {
            var nodPower = Owner.Osty.GetPower<NodPower>();
            if (nodPower == null)
            {
                await PowerCmd.Apply<NodPower>(choiceContext, Owner.Osty, 1m, Owner.Creature, this);
            }
        }
    }

    private CardModel? GetLastPlayedCard()
    {
        var currentRound = Owner.Creature.CombatState.RoundNumber;
        var history = CombatManager.Instance.History;

        // 查找本回合自己打出的最后一张牌
        var entry = history.CardPlaysFinished
            .LastOrDefault(e => e.CardPlay.Card.Owner == Owner
                && e.CardPlay.Card.CombatState?.RoundNumber == currentRound);

        if (entry == null)
        {
            // 查找上回合自己打出的最后一张牌
            entry = history.CardPlaysFinished
                .LastOrDefault(e => e.CardPlay.Card.Owner == Owner
                    && e.CardPlay.Card.CombatState?.RoundNumber == currentRound - 1);
        }

        return entry?.CardPlay.Card;
    }

    protected override void OnUpgrade()
    {
        DynamicVars["SummonAmount"].UpgradeValueBy(10m);
    }
}