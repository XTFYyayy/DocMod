using BaseLib.Abstracts;
using Doc.DocCode.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using System;
using System.Threading.Tasks;

namespace Doc.DocCode.Powers;

public sealed class SparkOfLifebeingsPower : CustomPowerModel
{
    // 下个回合起，攻击造成双倍伤害
    private bool _attacksDoubled;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override string? CustomPackedIconPath => "spark_of_lifebeings.png".PowerImagePath();
    public override string? CustomBigIconPath => "spark_of_lifebeings.png".PowerImagePath();

    // 受到未被格挡的非攻击伤害时：伤害减半，并获得1层生灵火花（苇草被动）
    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target != Owner) return;
        if (result.UnblockedDamage <= 0) return;
        if (props.HasFlag(ValueProp.Move)) return; // 攻击伤害不触发

        await PowerCmd.Apply<SparkOfLifebeingsPower>(choiceContext, Owner, 1, dealer, cardSource);
        Flash();
    }

    // 攻击伤害：下个回合起双倍；非攻击伤害：每层减半
    public override decimal ModifyDamageMultiplicative(Creature? target, decimal damage, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target != Owner) return 1m;
        if (props.HasFlag(ValueProp.Move))
        {
            return _attacksDoubled ? 2m : 1m;
        }
        return (decimal)Math.Pow(0.5, (int)Amount);
    }

    // 生命流失：减半，并获得1层生灵火花（同步 hook，若已有则直接加层）
    public override decimal ModifyHpLostAfterOstyLate(Creature target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target != Owner || amount <= 0) return amount;

        var spark = Owner.GetPower<SparkOfLifebeingsPower>();
        if (spark != null)
        {
            spark.SetAmount(spark.Amount + 1);
        }
        return amount * (decimal)Math.Pow(0.5, (int)Amount);
    }

    // 回合开始：开启攻击双倍，层数减1
    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player.Creature != Owner) return;
        _attacksDoubled = true;
        await PowerCmd.TickDownDuration(this);
    }
}
