using BaseLib.Utils;
using Doc.DocCode.Attributes;
using Doc.DocCode.Commands;
using Doc.DocCode.Extensions;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
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
        new DynamicVar("SummonAmount", 30m),
        new EnergyVar(1)
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var lastCard = GetLastPlayedCard();

        // 若上一张牌为卡西米尔势力，费用减1，复制一张加入抽牌堆
        if (lastCard != null && lastCard.IsKazimierz())
        {
            await PlayerCmd.GainEnergy(DynamicVars.Energy.IntValue, Owner);

            var copy = CreateClone();
            var cards = new List<CardModel> { copy };
            var result = await CardPileCmd.AddGeneratedCardsToCombat(cards, PileType.Draw, Owner);
            CardCmd.PreviewCardPileAdd(result);
        }

        // 召唤耀阳
        decimal amount = DynamicVars["SummonAmount"].BaseValue;
        await BlazingSunCmd.Summon(choiceContext, Owner, amount, this);
    }

    private CardModel? GetLastPlayedCard()
    {
        // 获取最近一张自己打出的牌（不区分回合）
        var entry = CombatManager.Instance.History.CardPlaysFinished
            .LastOrDefault(e => e.CardPlay.Card.Owner == Owner);

        return entry?.CardPlay.Card;
    }

    protected override void OnUpgrade()
    {
        DynamicVars["SummonAmount"].UpgradeValueBy(10m);
    }
}