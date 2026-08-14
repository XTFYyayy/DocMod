using Doc.DocCode.Attributes;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using System.Collections.Generic;
using System.Threading.Tasks;
using Doc.DocCode.Powers;

namespace Doc.DocCode.Cards.Doctor;

[CardTags(isApostle: true)]
public sealed class Nightingale() : DocCard(3, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("BufferPower", 2m),
        new DynamicVar("HolyLandPower", 2m),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<BufferPower>(),
        HoverTipFactory.FromPower<HolyLandPower>()
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 获得2/3层缓冲
        await PowerCmd.Apply<BufferPower>(choiceContext, Owner.Creature, DynamicVars["BufferPower"].BaseValue, Owner.Creature, this);
        await PowerCmd.Apply<HolyLandPower>(choiceContext, Owner.Creature, DynamicVars["HolyLandPower"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        // 缓冲 2 → 3 层
        DynamicVars["BufferPower"].UpgradeValueBy(1m);
    }
}
