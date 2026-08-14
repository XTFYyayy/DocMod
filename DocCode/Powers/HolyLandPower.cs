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

public sealed class HolyLandPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override string? CustomPackedIconPath => "holy_land_power.png".PowerImagePath();
    public override string? CustomBigIconPath => "holy_land_power.png".PowerImagePath();

    public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        // 只对拥有此Power的自身生效
        if (base.Owner != target)
        {
            return 0m;
        }

        // 攻击伤害：带 Move，不带 Unpowered → 不触发圣域
        // 非攻击伤害/生命流失：带 Move + Unpowered → 触发圣域，降为1点
        if (props.HasFlag(ValueProp.Move) && props.HasFlag(ValueProp.Unpowered))
        {
            if (amount > 1m)
            {
                return -(amount - 1m);
            }
        }

        return 0m;
    }
}