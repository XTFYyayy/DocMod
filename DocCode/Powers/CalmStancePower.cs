using Doc.DocCode.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using System.Threading.Tasks;

namespace Doc.DocCode.Powers;

/// <summary>
/// 平静姿态：离开此姿态时获得2点能量。
/// </summary>
public sealed class CalmStancePower : BaseStancePower
{
    public override Stance StanceType => Stance.Calm;

    public override string? CustomPackedIconPath => "calm_stance_power.png".PowerImagePath();
    public override string? CustomBigIconPath => "calm_stance_power.png".PowerImagePath();

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new EnergyVar(2)
    ];

    public override async Task AfterRemoved(Creature oldOwner)
    {
        await PlayerCmd.GainEnergy(DynamicVars.Energy.IntValue, oldOwner.Player);
    }
}
