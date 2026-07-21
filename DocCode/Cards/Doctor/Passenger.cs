using BaseLib;
using BaseLib.Abstracts;
using Doc.DocCode.Attributes;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Doc.DocCode.Cards.Doctor;

[CardTags(isSargon: true)]
public sealed class Passenger() : DocCard(3, CardType.Attack, CardRarity.Rare, TargetType.RandomEnemy)
{
    private const string _calculatedHitsKey = "HitsCount";
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        
        new DamageVar(6m, ValueProp.Move),
        new CalculationBaseVar(0m),
        new CalculationExtraVar(1m),
        new CalculatedVar("HitsCount").WithMultiplier(
           (CardModel card, Creature? target) =>
               CombatManager.Instance.History.Entries
                   .OfType<DamageReceivedEntry>()
                   .Count(e => 
                       // 伤害来源是卡牌
                       e.CardSource != null &&
                       // 伤害来源卡牌是攻击类型
                       e.CardSource.Type == CardType.Attack &&
                       // 伤害来源是这张卡的拥有者
                       e.Dealer == card.Owner?.Creature &&
                       // 伤害未被完全格挡
                       !e.Result.WasFullyBlocked &&
                       // 伤害不为0（未被格挡的伤害 + 溢出伤害 > 0）
                       (e.Result.UnblockedDamage > 0 || e.Result.OverkillDamage > 0)
                   )
           )
        
    ];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var owner = Owner.Creature;
        if (owner == null) return;

        // 获取当前已经造成未被格挡攻击伤害的次数
        int hitCount = (int)((CalculatedVar)DynamicVars["HitsCount"]).Calculate(null);

        if (hitCount <= 0) return;

        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).WithHitCount(hitCount).FromCard(this)
            .TargetingRandomOpponents(base.CombatState)
            .WithHitFx("vfx/vfx_attack_lightning")
            .Execute(choiceContext);

    }

    protected override void OnUpgrade()
    {
        // 升级：每 hit 伤害从 7 提升到 9
        base.DynamicVars.Damage.UpgradeValueBy(2m);
    }
}