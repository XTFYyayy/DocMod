using Godot;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using System.Threading.Tasks;
using BaseLib.Abstracts;

namespace Doc.DocCode.Powers;

/// <summary>
/// 姿态系统的抽象基类。每个具体姿态（愤怒/平静/神格）继承此类，
/// 独立管理自身的伤害倍率、图标和进出逻辑。
/// </summary>
public abstract class BaseStancePower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool AllowNegative => false;

    /// <summary>
    /// 当前姿态类型。
    /// </summary>
    public abstract Stance StanceType { get; }

    public override decimal ModifyDamageAdditive(Creature? target, decimal damage, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        // 只处理自己造成的伤害
        if (dealer != Owner) return 0;
        if (!props.HasFlag(ValueProp.Move)) return 0;
        // 排除反伤伤害（反伤带有 Unpowered 标志）
        if (props.HasFlag(ValueProp.Unpowered)) return 0;

        return ModifyDamageDealt(damage);
    }

    public override decimal ModifyDamageMultiplicative(Creature? target, decimal damage, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        // 只处理针对自己的伤害
        if (target != Owner) return 1m;
        if (!props.HasFlag(ValueProp.Move)) return 1m;

        return ModifyDamageReceived();
    }

    /// <summary>
    /// 修改造成的伤害（additive方式）。
    /// 返回 0 = 无修改，返回 damage = 双倍，返回 damage * 2 = 三倍。
    /// </summary>
    protected virtual decimal ModifyDamageDealt(decimal damage) => 0;

    /// <summary>
    /// 修改受到伤害的倍率。返回乘数，默认 1m（无修改）。
    /// </summary>
    protected virtual decimal ModifyDamageReceived() => 1m;
}
