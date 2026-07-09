using Doc.DocCode.Extensions;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;

namespace Doc.DocCode.Powers;

public sealed class StancePower : CustomPowerModel
{
    private Stance _stance;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool AllowNegative => false;

    public override string? CustomPackedIconPath => "stance_power.png".PowerImagePath();
    public override string? CustomBigIconPath => "stance_power.png".PowerImagePath();

    public Stance CurrentStance => _stance;

    protected override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get
        {
            var description = GetStanceDescription();
            yield return new HoverTip(this, description, isSmart: false);
        }
    }

    public void SetStance(Stance stance)
    {
        _stance = stance;
        InvokeDisplayAmountChanged();
    }

    private string GetStanceDescription()//用于显示详细的姿态和该姿态的信息
    {
        switch (_stance)
        {
            case Stance.Wrath:
                return "愤怒：攻击造成双倍伤害，受到攻击伤害翻倍。";
            case Stance.Calm:
                return "平静：离开此姿态时获得2点能量。";
            case Stance.Divinity:
                return "神格：进入时获得3点能量，攻击造成三倍伤害，下回合开始时退出。";
            default:
                return "无姿态";
        }
    }

    public override decimal ModifyDamageAdditive(Creature? target, decimal damage, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        // 只处理自己造成的伤害
        if (dealer != Owner) return 0;
        if (!props.HasFlag(ValueProp.Move)) return 0;

        // 排除反伤伤害（反伤带有 Unpowered 标志）
        if (props.HasFlag(ValueProp.Unpowered)) return 0;

        switch (_stance)
        {
            case Stance.Wrath:
                return damage;  // 翻倍
            case Stance.Divinity:
                return damage * 2m;  // 三倍
            default:
                return 0;
        }
    }

    // 修改受到攻击伤害的方法（怪物攻击玩家时）
    public override decimal ModifyDamageMultiplicative(Creature? target, decimal damage, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        // 检查是否是针对自己的伤害
        if (target != Owner) return 1m;
        if (!props.HasFlag(ValueProp.Move)) return 1m;
        if (_stance != Stance.Wrath) return 1m;//不在愤怒不受到双倍伤害

        // 伤害翻倍（返回 2 表示乘以 2）
        return 2m;
    }

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)//神格的回合开始自动退出
    {
        if (player.Creature != Owner) return;
        if (_stance == Stance.Divinity)
        {
            RemoveInternal();
        }
    }

    public override async Task AfterRemoved(Creature oldOwner)//平静的退出返还费用
    {
        if (_stance == Stance.Calm)
        {
            await PlayerCmd.GainEnergy(2, oldOwner.Player);
        }
    }
}