using BaseLib.Abstracts;
using Doc.DocCode.Extensions;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using System.Threading.Tasks;

namespace Doc.DocCode.Powers;

/// <summary>
/// 我见，我得 - 造成未被格挡的攻击伤害时，偷取目标1点力量
/// </summary>
public sealed class ISeeIGainPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override string? CustomPackedIconPath => "i_see_i_gain_power.png".PowerImagePath();
    public override string? CustomBigIconPath => "i_see_i_gain_power.png".PowerImagePath();


    /// <summary>
    /// 造成伤害后触发偷取力量
    /// </summary>
    public override async Task AfterDamageGiven(PlayerChoiceContext choiceContext, Creature? dealer, DamageResult result, ValueProp props, Creature target, CardModel? cardSource)
    {
        // 只处理自己造成的伤害
        if (dealer != Owner) return;
        // 只处理攻击伤害
        if (!props.IsPoweredAttack()) return;
        // 如果伤害被完全格挡，不触发溅射
        if (result.WasFullyBlocked) return;
        // 如果伤害为 0 或没有造成实际伤害，不触发
        if (result.UnblockedDamage <= 0 && result.OverkillDamage <= 0) return;
        // 不能偷取自己的力量（对自己造成伤害时不触发）
        if (target == Owner) return;

        // 偷取 1 点力量（根据层数决定偷取数量）
        int stealAmount = Amount;

        // 目标减少力量
        await PowerCmd.Apply<StrengthPower>(
            choiceContext,
            target,
            -stealAmount, // 负数 = 减少
            Owner,
            cardSource,
            false
        );

        // 自己增加力量
        await PowerCmd.Apply<StrengthPower>(
            choiceContext,
            Owner,
            stealAmount,
            Owner,
            cardSource,
            false
        );
    }

}