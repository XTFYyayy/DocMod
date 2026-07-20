using BaseLib.Abstracts;
using Doc.DocCode.Extensions;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Doc.DocCode.Powers;

public sealed class PepePower : CustomPowerModel
{

    // 溅射伤害比例（50%）
    private const decimal SPLASH_RATIO = 0.5m;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override string? CustomPackedIconPath => "pepe_power.png".PowerImagePath();
    public override string? CustomBigIconPath => "pepe_power.png".PowerImagePath();


    // ---------- 关键：在造成伤害后触发溅射 ----------
    public override async Task AfterDamageGiven(PlayerChoiceContext choiceContext, Creature? dealer, DamageResult result, ValueProp props, Creature target, CardModel? cardSource)
    {
        // 只处理自己造成的伤害
        if (dealer != Owner) return;
        // 只处理攻击伤害
        if (!props.IsPoweredAttack()) return;
        // 如果伤害被完全格挡，不触发溅射
        if (result.WasFullyBlocked) return;

        // 如果伤害为 0 或没有造成实际伤害，不触发
        if (result.UnblockedDamage <= 0 && result.OverkillDamage <= 0) return;

        // 计算溅射伤害（未被格挡伤害的一半*层数）
        int splashDamage = (int)((result.UnblockedDamage + result.OverkillDamage) * SPLASH_RATIO*Amount);

        if (splashDamage <= 0) return;

        // 获取所有其他敌人
        var otherEnemies = CombatState.Enemies
            .Where(e => e != target && e.IsAlive)
            .ToList();

        if (!otherEnemies.Any()) return;

        // 对所有其他敌人造成溅射伤害
        await CreatureCmd.Damage(choiceContext, otherEnemies, splashDamage, ValueProp.Unpowered, base.Owner);
    }

}