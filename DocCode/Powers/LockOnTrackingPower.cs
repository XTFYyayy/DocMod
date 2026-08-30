using BaseLib.Abstracts;
using Doc.DocCode.Extensions;
using Doc.DocCode.Orbs;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Orbs;

namespace Doc.DocCode.Powers;

/// <summary>
/// 跟踪锁定（LockOnTrackingPower）：敌人身上的 Counter 类 debuff。
/// 效果文案："从闪电充能球中受到的伤害增加50%"。
/// 注意：+50% 伤害放大逻辑不在本类实现，而是在 DocLightningOrb 任务中
/// 通过检查目标是否持有本 Power 来实现（见 DocLightningOrb 相关文件）。
/// 本类仅定义类型（含类名、图标路径、描述、层数语义），确保该类型可被其他任务引用。
/// 描述本地化文案需写入 Doc/localization/zhs/powers.json 的
/// "DOC-LOCK_ON_TRACKING_POWER.description" 字段（图标素材由用户后续提供，代码先占位）。
/// </summary>
public sealed class LockOnTrackingPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override string? CustomPackedIconPath => "lock_on_tracking_power.png".PowerImagePath();
    public override string? CustomBigIconPath => "lock_on_tracking_power.png".PowerImagePath();

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromOrb<LightningOrb>()
    
    ];
}
