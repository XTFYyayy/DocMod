using Doc.DocCode.Attributes;
using Doc.DocCode.Orbs;
using Doc.DocCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Orbs;
using MegaCrit.Sts2.Core.ValueProps;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Doc.DocCode.Cards.Doctor;

/// <summary>
/// 格雷伊（Greyy）：玻利瓦尔，2 费能力稀有，消耗（Exhaust）。
/// 打出后获得一层获得 1 层"闪电充能球现在会击中所有敌人"（LightningStrikesAllPower），
/// 升级后并生成 3 个闪电充能球（DocLightningOrb）。
/// </summary>
[CardTags(isBolivar: true, isRhodeIsland: true)]
public sealed class Greyy() : DocCard(2, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new RepeatVar(1)
    ];
 
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<LightningStrikesAllPower>(),
        HoverTipFactory.Static(StaticHoverTip.Channeling),
        HoverTipFactory.FromOrb<LightningOrb>()
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {

        await PowerCmd.Apply<LightningStrikesAllPower>(choiceContext, Owner.Creature, 1m, Owner.Creature, this);
        await OrbCmd.Channel<DocLightningOrb>(choiceContext, base.Owner);
        await OrbCmd.Channel<DocLightningOrb>(choiceContext, base.Owner);
        if (IsUpgraded)
            await OrbCmd.Channel<DocLightningOrb>(choiceContext, base.Owner);

    }
    protected override void OnUpgrade()
    {
        DynamicVars.Repeat.UpgradeValueBy(1m);
    }
}
