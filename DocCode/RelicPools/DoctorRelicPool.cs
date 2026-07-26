using Doc.DocCode.Relics;
using MegaCrit.Sts2.Core.Models;
using Godot;

namespace Doc.DocCode.RelicsPools;
public sealed class DoctorRelicPool : RelicPoolModel
{
    public override string EnergyColorName => "doc";
    public override Color LabOutlineColor => new Color("c4278a");

    protected override IEnumerable<RelicModel> GenerateAllRelics()
    {
        return new RelicModel[]
        {
            ModelDb.Relic<HrBronzeSeal>(),
            ModelDb.Relic<DoctorSilverSeal>(),
        };
    }
}