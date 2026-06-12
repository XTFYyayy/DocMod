using Doc.DocCode;
using Doc.DocCode.Attributes;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Doc.DocCode.Cards.Doctor;

public sealed class UndeclaredRage(): CardModel(0, CardType.Skill, CardRarity.Token, TargetType.Self)
{
    // 抽牌数量（升级前为0，升级后为2）
    protected override IEnumerable<DynamicVar> CanonicalVars => new[]
    {
        new CardsVar(0)
    };

    public override IEnumerable<CardKeyword> CanonicalKeywords => new[]
    {
        CardKeyword.Exhaust,
        CardKeyword.Retain
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 进入愤怒姿态
        await StanceHelper.EnterStance(choiceContext, Owner.Creature, Stance.Wrath, this);

        // 抽牌（升级后才有）
        int drawCount = DynamicVars.Cards.IntValue;
        if (drawCount > 0)
        {
            await CardPileCmd.Draw(choiceContext, drawCount, Owner);
        }
    }

    protected override void OnUpgrade()
    {
        // 升级：抽牌数量从0变为2
        DynamicVars.Cards.UpgradeValueBy(2m);
    }


}