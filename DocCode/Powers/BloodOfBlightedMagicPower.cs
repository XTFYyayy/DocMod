using BaseLib.Abstracts;
using Doc.DocCode.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using System.Threading.Tasks;

namespace Doc.DocCode.Powers;

public sealed class BloodOfBlightedMagicPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override string? CustomPackedIconPath => "blood_of_blighted_magic_power.png".PowerImagePath();
    public override string? CustomBigIconPath => "blood_of_blighted_magic_power.png".PowerImagePath();

    // 监听未被格挡的非攻击伤害
    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (base.Owner != target) return;
        if (result.UnblockedDamage <= 0) return;
        if (props.HasFlag(ValueProp.Move)) return; // 攻击伤害不触发

        // 使用 Owner 而不是 base.Owner，并显式传入 cardSource
        await PowerCmd.Apply<SparkOfLifebeingsPower>(choiceContext, Owner, Amount, Owner, cardSource);
        Flash();
    }

    // 监听生命流失（带 Move + Unpowered）
    public override decimal ModifyHpLostAfterOstyLate(Creature target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (base.Owner != target) return amount;
        if (amount <= 0) return amount;

        // 生命流失触发获得 Amount 层生灵火花
        // 异步执行，使用 Owner 和 cardSource
        Task.Run(async () =>
        {
            await PowerCmd.Apply<SparkOfLifebeingsPower>(null, Owner, Amount, Owner, cardSource);
        });

        return amount;
    }
}