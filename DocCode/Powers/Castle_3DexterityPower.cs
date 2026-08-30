using Doc.DocCode.Cards;
using Doc.DocCode.Cards.Doctor;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Doc.DocCode.Powers;

public sealed class Castle_3DexterityPower : TemporaryDexterityPower
{
    public override AbstractModel OriginModel => ModelDb.Card<Castle_3>();

    protected override bool IsPositive => false;
}