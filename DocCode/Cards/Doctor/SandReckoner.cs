using BaseLib;
using BaseLib.Abstracts;
using Doc.DocCode.Attributes;
using Godot;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.ValueProps;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Doc.DocCode.Cards.Doctor;

[CardTags(isSargon: true)]

public sealed class SandReckoner(): DocCard(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
    ];
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(CardKeyword.Sly)
    ];


    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var owner = Owner;
        if (owner == null) return;

        // 获取当前手牌
        var handPile = PileType.Hand.GetPile(owner);
        var cardsInHand = handPile.Cards.ToList();

        // 给每张手牌添加"奇巧"关键词
        foreach (var card in cardsInHand)
        {
            CardCmd.ApplyKeyword(card, CardKeyword.Sly);
        }
    }

    protected override void OnUpgrade()
    {
        // 升级时减少费用
        EnergyCost.UpgradeBy(-1);
    }
}