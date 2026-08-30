using BaseLib.Abstracts;
using Doc.DocCode.Extensions;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;

namespace Doc.DocCode.Powers;

public sealed class DesertObeliskDieForYouPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override string? CustomPackedIconPath => "die_for_you_power.png".PowerImagePath();
    public override string? CustomBigIconPath => "die_for_you_power.png".PowerImagePath();
    public override bool ShouldPlayVfx => false;

    // 移除所有 ModifyUnblockedDamageTarget 和其他覆盖方法
    // 这个类现在只是一个标记，由 DieForYouManager 和 LoseHpPatch 处理
}