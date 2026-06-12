using BaseLib.Utils;
using Doc.DocCode.Attributes;
using Doc.DocCode.Extensions;
using Doc.DocCode.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.ValueProps;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Doc.DocCode.Cards.Doctor;

[CardTags(isKazimierz: true, isKnight: true)]
public sealed class NearlTheRadianKnight() : DocCard(3, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("SummonAmount", 30m)  // 召唤/升级数值
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 获取上一张打出的牌
        var lastCard = GetLastPlayedCard();

        // 检查是否需要消耗
        bool shouldExhaust = true;
        if (lastCard != null && lastCard.IsKazimierz())
        {
            // 上一张是卡西米尔势力，费用减1且不消耗
            EnergyCost.SetThisTurnOrUntilPlayed(EnergyCost.CanonicalCost - 1);
            shouldExhaust = false;
        }

        // 设置消耗
        if (shouldExhaust)
        {
            ExhaustOnNextPlay = true;
        }

        // 召唤或升级耀阳
        decimal summonAmount = DynamicVars["SummonAmount"].BaseValue;
        await OstyCmd.Summon(choiceContext, Owner, summonAmount, this);

        // 施加【颔首】能力（每回合第一张攻击牌无视格挡）
        await PowerCmd.Apply<NodPower>(Owner.Creature, 1m, Owner.Creature, this);
    }

    private CardModel? GetLastPlayedCard()
    {
        var currentRound = Owner.Creature.CombatState.RoundNumber;

        var entry = CombatManager.Instance.History.CardPlaysFinished
            .LastOrDefault(e => e.CardPlay.Card.Owner == Owner
                && e.RoundNumber == currentRound);

        if (entry == null)
        {
            entry = CombatManager.Instance.History.CardPlaysFinished
                .LastOrDefault(e => e.CardPlay.Card.Owner == Owner
                    && e.RoundNumber == currentRound - 1);
        }

        return entry?.CardPlay.Card;
    }

    protected override void OnUpgrade()
    {
        // 升级：召唤数值从30变为40
        DynamicVars["SummonAmount"].UpgradeValueBy(10m);
    }
}