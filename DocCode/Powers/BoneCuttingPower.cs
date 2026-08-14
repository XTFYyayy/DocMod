using BaseLib.Abstracts;
using Doc.DocCode.Extensions;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace Doc.DocCode.Powers;

public sealed class BoneCuttingPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Single;

    public override string? CustomPackedIconPath => "bone_cutting_power.png".PowerImagePath();
    public override string? CustomBigIconPath => "bone_cutting_power.png".PowerImagePath();

    // 从攻击中受到的伤害至少为5
    public override decimal ModifyHpLostAfterOstyLate(Creature target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target != Owner) return amount;
        if (!props.HasFlag(ValueProp.Move)) return amount; // 只处理攻击伤害
        if (amount < 5m) return 5m;
        return amount;
    }
}
