using BaseLib.Abstracts;
using Doc.DocCode.Extensions;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace Doc.DocCode.Powers;

/// <summary>
/// 闪电充能球全命中（LightningStrikesAllPower）：玩家身上的纯标记类 Buff。
/// 效果文案："闪电充能球现在会击中所有敌人。"。
/// 注意：本类不包含任何伤害逻辑，仅作为标记存在。
/// AOE 切换逻辑在 DocLightningOrb 任务中，通过检查玩家是否持有本 Power
/// （如 PowerModel.GetPowers&lt;LightningStrikesAllPower&gt;(owner) 或 owner.Powers）来决定单体/AOE 行为。
/// 层数语义：固定 1 层标记（PowerStackType.Single）。
/// 描述本地化文案需写入 Doc/localization/zhs/powers.json 的
/// "DOC-LIGHTNING_STRIKES_ALL_POWER.description" 字段（图标素材由用户后续提供，代码先占位）。
/// </summary>
public sealed class LightningStrikesAllPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override string? CustomPackedIconPath => "lightning_strikes_all.png".PowerImagePath();
    public override string? CustomBigIconPath => "lightning_strikes_all.png".PowerImagePath();
}
