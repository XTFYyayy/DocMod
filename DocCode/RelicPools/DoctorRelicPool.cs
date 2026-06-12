using Doc.DocCode.Relics;
using Godot;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Models.Relics;
using System.Collections.Generic;

namespace Doc.DocCode.RelicPools;

public sealed class DoctorRelicPool : RelicPoolModel
{
    public override string EnergyColorName => "doc";
    public override Color LabOutlineColor => new Color("c4278a");

    protected override IEnumerable<RelicModel> GenerateAllRelics()
    {
        return new RelicModel[]
        {
            ModelDb.Relic<HrBronzeSeal>(),
        };
    }
}