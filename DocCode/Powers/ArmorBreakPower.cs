using BaseLib.Abstracts;
using Doc.DocCode.Extensions;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.HoverTips;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Doc.DocCode.Powers;

public sealed class ArmorBreakPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override string? CustomPackedIconPath => "armor_break_power.png".PowerImagePath();
    public override string? CustomBigIconPath => "armor_break_power.png".PowerImagePath();

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<Shiv>()
    ];


    // 在 Power 被施加后立即检查
    public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        // 检查目标是否有易伤
        var vulnerablePower = Owner.GetPower<VulnerablePower>();
        if (vulnerablePower == null || vulnerablePower.Amount <= 0)
        {
            // 没有易伤，立即移除自身
            RemoveInternal();
        }
    }

    public override async Task BeforeDamageReceived(PlayerChoiceContext choiceContext, Creature target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        // 1. 只处理碎甲所在的敌人受到伤害的情况
        if (target != Owner) return;

        // 2. 检查敌人是否还有易伤
        var vulnerablePower = Owner.GetPower<VulnerablePower>();
        if (vulnerablePower == null || vulnerablePower.Amount <= 0)
        {
            // 没有易伤，碎甲效果结束，移除自身
            RemoveInternal();
            return;
        }

        // 3. 只处理攻击伤害
        if (props.HasFlag(ValueProp.Unpowered)) return;
        if (!props.HasFlag(ValueProp.Move)) return;

        // 4. 检查伤害来源，如果是小刀则跳过，避免无限循环
        if (cardSource != null)
        {
            if (cardSource is Shiv || cardSource.GetType().Name == "Shiv")
            {
                return;
            }
        }

        // 5. 检查是否在战斗中
        if (CombatManager.Instance == null || !CombatManager.Instance.IsInProgress) return;

        // 6. 攻击者应该是玩家
        if (dealer == null || dealer.Player == null || CombatState == null) return;

        int shivCount = Amount;
        if (shivCount <= 0) return;

        for (int i = 0; i < shivCount; i++)
        {
            await Shiv.CreateInHand(dealer.Player, CombatState);
        }
    }
}