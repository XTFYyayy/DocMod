using Doc.DocCode.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using System.Threading.Tasks;

namespace Doc.DocCode.Powers;

/// <summary>
/// 神格姿态：进入时获得3点能量，攻击造成三倍伤害，下回合开始时自动退出。
/// </summary>
public sealed class DivinityStancePower : BaseStancePower
{
    public override Stance StanceType => Stance.Divinity;

    public override string? CustomPackedIconPath => "divinity_stance_power.png".PowerImagePath();
    public override string? CustomBigIconPath => "divinity_stance_power.png".PowerImagePath();

    protected override decimal ModifyDamageDealt(decimal damage) => damage * 2m;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player.Creature != Owner) return;
        await PowerCmd.Remove(this);
    }
}
