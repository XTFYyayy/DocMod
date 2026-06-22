using BaseLib.Abstracts;
using Doc.DocCode.Extensions;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using System.Threading.Tasks;

namespace Doc.DocCode.Powers;

public sealed class SleepPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override bool AllowNegative => false;
    public override string? CustomPackedIconPath => "sleep_power.png".PowerImagePath();
    public override string? CustomBigIconPath => "sleep_power.png".PowerImagePath();

    // 回合开始时层数减1
    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        // 只对敌人有效
        if (Owner.Side == player.Creature.Side) return;

        int newAmount = Amount - 1;
        if (newAmount <= 0)
        {
            RemoveInternal();
        }
        else
        {
            SetAmount(newAmount);
        }
    }

    // 受到伤害时触发击晕
    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target != Owner) return;

        int damageAmount = result.TotalDamage;
        if (damageAmount <= 0) return;

        // 施加击晕
        await CreatureCmd.Stun(Owner);

        // 击晕后移除安眠
        RemoveInternal();
    }
}