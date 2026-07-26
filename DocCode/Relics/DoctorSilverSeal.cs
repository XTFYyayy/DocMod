using BaseLib.Utils;
using Doc.DocCode.CardPools;
using Doc.DocCode.Extensions;
using Doc.DocCode.RelicsPools;
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
[Pool(typeof(DoctorRelicPool))]
public sealed class DoctorSilverSeal : RelicModel
{
    public override RelicRarity Rarity => RelicRarity.Starter;

    public override string PackedIconPath => "doctor_silver_seal.png".RelicImagePath();

    protected override string PackedIconOutlinePath => "doctor_silver_seal_OL.png".RelicImagePath();

    protected override string BigIconPath => "doctor_silver_seal_BG.png".RelicImagePath();

    public override async Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        // 只在自己回合开始且是第一回合时触发
        if (side != base.Owner.Creature.Side) return;
        if (combatState.RoundNumber > 1) return;

        try
        {
            // 从博士卡池中获取所有稀有牌
            var cardPool = ModelDb.CardPool<DocCardPool>();
            if (cardPool == null) return;

            var rareCards = cardPool.AllCards
                .Where(c => c != null && c.Rarity == CardRarity.Rare && c.IsUpgradable)
                .Select(c => c.ToMutable())
                .ToList();

            if (rareCards.Count == 0) return;

            // 随机选择一张
            var randomCard = rareCards.OrderBy(x => Guid.NewGuid()).First();

            randomCard.Owner = base.Owner;

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
            MainFile.Logger.Error($"DoctorSilverSeal error: {e.Message}");
        }
    }
}