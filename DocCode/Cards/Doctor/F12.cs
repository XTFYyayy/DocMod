using Doc.DocCode.Attributes;
using Doc.DocCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Orbs;
using System;
using System.Threading.Tasks;

namespace Doc.DocCode.Cards.Doctor;

/// <summary>
/// F12：罗德岛，1 费技能普通，对目标敌人给予 2 层跟踪锁定（LockOnTrackingPower）。
/// 升级后费用 1→0。
/// </summary>
[CardTags(isRhodeIsland: true)]
public sealed class F12() : DocCard(1, CardType.Skill, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("LockOnTrackingPower", 2m)
    ];
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<LockOnTrackingPower>()
    ];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        await PowerCmd.Apply<LockOnTrackingPower>(choiceContext, cardPlay.Target, DynamicVars["GravelThreshold"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
