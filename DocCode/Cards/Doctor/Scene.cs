using BaseLib;
using BaseLib.Abstracts;
using Doc.DocCode.Attributes;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Doc.DocCode.Cards.Doctor;

[CardTags(isSargon:true)]
public sealed class Scene(): DocCard(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [

    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<Recon>(),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var owner = Owner;
        if (owner == null) return;

        int num = CardPile.MaxCardsInHand - CardPile.GetCards(base.Owner, PileType.Hand).Count();

        if (num == 0) return;

        List<CardModel> list = new List<CardModel>();

        // 直接循环创建并加入手牌
        for (int i = 0; i < num; i++)
        {
            var recon = CombatState.CreateCard<Recon>(owner);
            await CardPileCmd.AddGeneratedCardsToCombat(new List<CardModel> { recon }, PileType.Hand, owner);
        }
    }

    protected override void OnUpgrade()
    {
        // 升级：移除消耗关键词
        RemoveKeyword(CardKeyword.Exhaust);
    }
}