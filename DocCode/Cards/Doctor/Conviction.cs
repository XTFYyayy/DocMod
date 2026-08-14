// Conviction.cs - 断罪者
using Doc.DocCode.Attributes;
using Doc.DocCode.Cards.Doctor;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Doc.DocCode.Cards;

[CardTags(isMinos: true)]
public sealed class Conviction() : DocCard(3, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Sly];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<Atk>(),
        HoverTipFactory.FromCard<Dfc>(),
        HoverTipFactory.FromCard<Ctrl>(),
        HoverTipFactory.FromCard<Bst>()
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 获取4种衍生牌的CardModel类型
        List<System.Type> cardTypes = [typeof(Atk), typeof(Dfc), typeof(Ctrl), typeof(Bst)];

        // 从4张中随机选3张类型
        List<System.Type> shuffledTypes = new List<System.Type>();
        var rng = base.Owner.RunState.Rng.CombatCardGeneration;
        while (cardTypes.Count > 0)
        {
            int index = rng.NextInt(0, cardTypes.Count);
            shuffledTypes.Add(cardTypes[index]);
            cardTypes.RemoveAt(index);
        }
        List<System.Type> selectedTypes = shuffledTypes.Take(3).ToList();

        // 使用ModelDb获取Canonical实例
        List<CardModel> canonicalCards = new List<CardModel>();
        foreach (var type in selectedTypes)
        {
            // 使用ModelDb获取Canonical实例
            var method = typeof(ModelDb).GetMethod("Card").MakeGenericMethod(type);
            CardModel canonical = method.Invoke(null, null) as CardModel;
            canonicalCards.Add(canonical);
        }

        // 使用GetDistinctForCombat创建卡牌实例
        IEnumerable<CardModel> cards = CardFactory.GetDistinctForCombat(base.Owner, canonicalCards, 3, rng);

        CardModel selected = await CardSelectCmd.FromChooseACardScreen(choiceContext, cards.ToList(), base.Owner, canSkip: false);
        if (selected != null)
        {
            await CardPileCmd.AddGeneratedCardToCombat(selected, PileType.Hand, base.Owner);
        }
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
    }
}