using BaseLib.Abstracts;
using Doc.DocCode.Extensions;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Doc.DocCode.Powers;

public sealed class BlazingSunDieForYouPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override string? CustomPackedIconPath => "blazing_sun_die_for_you_power.png".PowerImagePath();
    public override string? CustomBigIconPath => "blazing_sun_die_for_you_power.png".PowerImagePath();

    // 关键：禁用 VFX 播放
    public override bool ShouldPlayVfx => false;

    public override Creature ModifyUnblockedDamageTarget(Creature target, decimal _, ValueProp props, Creature? __)
    {
        if (target != base.Owner.PetOwner?.Creature) return target;
        if (base.Owner.IsDead) return target;
        if (!props.IsPoweredAttack()) return target;
        return base.Owner;
    }

    public override bool ShouldAllowHitting(Creature creature)
    {
        return creature.IsAlive;
    }

    public override bool ShouldCreatureBeRemovedFromCombatAfterDeath(Creature creature)
    {
        if (creature != base.Owner) return true;
        return false;
    }

    public override bool ShouldPowerBeRemovedAfterOwnerDeath()
    {
        return false;
    }
}