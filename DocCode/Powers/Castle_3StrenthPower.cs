using Doc.DocCode.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using Doc.DocCode.Cards.Doctor;

namespace Doc.DocCode.Powers;

public sealed class Castle_3StrengthPower : TemporaryStrengthPower
{
    public override AbstractModel OriginModel => ModelDb.Card<Castle_3>();

    protected override bool IsPositive => false;
}