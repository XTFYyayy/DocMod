using Doc.DocCode.CardPools;
using Doc.DocCode.Extensions;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Doc.DocCode.Relics;

public sealed class HrBronzeSeal : RelicModel
{
    public override RelicRarity Rarity => RelicRarity.Starter;

    public override string PackedIconPath => "hr_bronze_seal.png".RelicImagePath();

    protected override string PackedIconOutlinePath => "hr_bronze_seal.png".RelicImagePath();

    protected override string BigIconPath => "hr_bronze_seal.png".RelicImagePath();

    public override async Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (side != base.Owner.Creature.Side) return;
        if (combatState.RoundNumber > 1) return;

        try
        {
            var cardPool = ModelDb.CardPool<DocCardPool>();
            if (cardPool == null) return;

            var uncommonCards = cardPool.AllCards
                .Where(c => c != null && c.Rarity == CardRarity.Uncommon && c.IsUpgradable)
                .ToList();

            if (uncommonCards.Count == 0) return;

            // 随机选择一张卡牌模板
            var selectedCardTemplate = uncommonCards.OrderBy(x => Guid.NewGuid()).First();

            // 通过 CombatState 创建卡牌实例
            var randomCard = combatState.CreateCard(selectedCardTemplate, base.Owner);

            // 升级
            randomCard.UpgradeInternal();
            randomCard.FinalizeUpgradeInternal();

            // 加入手牌
            await CardPileCmd.Add(randomCard, PileType.Hand);

            // 本回合耗能为0
            randomCard.SetToFreeThisTurn();

            Flash();
        }
        catch (Exception e)
        {
            MainFile.Logger.Error($"HrBronzeSeal error: {e.Message}");
        }
    }
}