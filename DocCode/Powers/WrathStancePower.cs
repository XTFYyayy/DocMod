using Doc.DocCode.Extensions;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Doc.DocCode.Powers;

/// <summary>
/// 愤怒姿态：攻击造成双倍伤害，受到攻击伤害翻倍。
/// </summary>
public sealed class WrathStancePower : BaseStancePower
{
    public override Stance StanceType => Stance.Wrath;

    public override string? CustomPackedIconPath => "wrath_stance_power.png".PowerImagePath();
    public override string? CustomBigIconPath => "wrath_stance_power.png".PowerImagePath();

    protected override decimal ModifyDamageDealt(decimal damage) => damage;

    protected override decimal ModifyDamageReceived() => 2m;
}
