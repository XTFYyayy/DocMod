using BaseLib.Abstracts;
using Doc.DocCode.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using System.Linq;
using System.Threading.Tasks;

namespace Doc.DocCode.Powers;

/// <summary>
/// 沙之碑的攻击追加能力：每打出一张攻击牌，对所有敌人造成自身生命值一半的伤害
/// </summary>
public sealed class DesertObeliskAttackAppendPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override string? CustomPackedIconPath => "desert_obelisk_attack_append_power.png".PowerImagePath();
    public override string? CustomBigIconPath => "desert_obelisk_attack_append_power.png".PowerImagePath();
    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        // 检查是否是该玩家打出的攻击牌
        if (cardPlay.Card.Owner != base.Owner.PetOwner) return;
        if (cardPlay.Card.Type != CardType.Attack) return;
        if (base.Owner.IsDead) return;

        // 计算伤害：自身当前生命值的一半
        decimal damage = base.Owner.CurrentHp / 2m;
        if (damage <= 0) return;

        // 对所有敌人造成伤害
        var enemies = base.Owner.CombatState?.Enemies;
        if (enemies == null || enemies.Count == 0) return;

        var targets = enemies.Where(e => e.IsHittable).ToList();
        if (targets.Count == 0) return;

        // 参考电球：使用 CreatureCmd.Damage，ValueProp.Unpowered，来源为沙之碑(其实伤害来源不重要）
        await CreatureCmd.Damage(context, targets, damage, ValueProp.Unpowered, base.Owner);
    }
}