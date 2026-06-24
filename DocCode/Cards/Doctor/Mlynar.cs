using Doc.DocCode.Attributes;
using Doc.DocCode.Extensions;
using Doc.DocCode.Powers;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.HoverTips;
using System.Collections.Generic;
using MegaCrit.Sts2.Core.ValueProps;
using System.Linq;
using System.Threading.Tasks;

namespace Doc.DocCode.Cards.Doctor;

[CardTags(isKazimierz: true)]
public sealed class Mlynar() : DocCard(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<UndeclaredRage>(),
        HoverTipFactory.FromCard<UnexoneratedSorrow>(),
        HoverTipFactory.FromCard<UngloriousGlory>()
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 1. 获取上一张打出的牌
        var lastCard = GetLastPlayedCard();

        if (lastCard != null)
        {
            // 上一张是攻击牌 -> 未声张的怒火
            if (lastCard.Type == CardType.Attack)
            {
                await AddDerivedCardToHand<UndeclaredRage>(choiceContext);
            }
            // 上一张不是攻击牌 -> 未宽解的悲哀
            else
            {
                await AddDerivedCardToHand<UnexoneratedSorrow>(choiceContext);
            }

            // 上一张是卡西米尔势力 -> 未照耀的荣光
            if (lastCard.IsKazimierz())
            {
                await AddDerivedCardToHand<UngloriousGlory>(choiceContext);
            }
        }

        // 2. 施加无动于衷 Buff（每张玛恩纳增加3层）
        var existingPower = Owner.Creature.GetPower<ApatheticPower>();
        if (existingPower != null)
        {
            existingPower.SetAmount(existingPower.Amount + 3);
        }
        else
        {
            var newPower = await PowerCmd.Apply<ApatheticPower>(choiceContext, Owner.Creature, 3m, Owner.Creature, this);
            newPower?.SetSourceCard(this);
        }
    }

    private CardModel? GetLastPlayedCard()
    {
        var combatState = Owner.Creature.CombatState;
        if (combatState == null) return null;

        // 先找本回合的
        var entry = CombatManager.Instance.History.CardPlaysFinished
            .LastOrDefault(e => e.CardPlay.Card.Owner == Owner
                && e.HappenedThisTurn(combatState));

        if (entry == null)
        {
            // 如果本回合没有，找上回合的
            entry = CombatManager.Instance.History.CardPlaysFinished
                .LastOrDefault(e => e.CardPlay.Card.Owner == Owner
                    && e.HappenedLastPlayerTurn(Owner));
        }

        return entry?.CardPlay.Card;
    }

    private async Task AddDerivedCardToHand<T>(PlayerChoiceContext choiceContext) where T : CardModel
    {
        // 通过 CombatState 创建卡牌实例，确保卡牌被正确添加到战斗状态
        var canonicalCard = ModelDb.GetById<T>(ModelDb.GetId(typeof(T)));
        var card = Owner.Creature.CombatState.CreateCard(canonicalCard, Owner);
        await CardPileCmd.Add(card, PileType.Hand);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}