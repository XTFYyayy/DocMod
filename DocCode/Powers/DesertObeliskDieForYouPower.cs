using BaseLib.Abstracts;
using Doc.DocCode.Extensions;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Doc.DocCode.Powers;

public sealed class DesertObeliskDieForYouPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override string? CustomPackedIconPath => "desert_obelisk_die_for_you_power.png".PowerImagePath();
    public override string? CustomBigIconPath => "desert_obelisk_die_for_you_power.png".PowerImagePath();
    public override bool ShouldPlayVfx => false;

    // 只保留标记功能，伤害分配由 DieForYouManager 统一处理
    // 移除 ModifyUnblockedDamageTarget 方法
}