using Doc.DocCode.Attributes;
using Doc.DocCode.Orbs;
using Doc.DocCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Orbs;

namespace Doc.DocCode.Cards.Doctor;

/// <summary>
/// 司霆惊蛰（LeiziTheThunderbringer）：炎，1 费技能普通。
/// 打出：获得 1 层起飞，生成 1 个闪电充能球（升级后起飞 2 层）。
/// [Pool] 特性由基类 DocCard 继承，子类不再显式声明（项目惯例）。
/// </summary>
[CardTags(isYan: true)]
public sealed class LeiziTheThunderbringer() : DocCard(3, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("FlyingPower", 2m),
        new RepeatVar(2)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.Static(StaticHoverTip.Channeling),
        HoverTipFactory.FromOrb<LightningOrb>(),
        HoverTipFactory.FromPower<FlyingPower>()
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<FlyingPower>(choiceContext, Owner.Creature, DynamicVars["FlyingPower"].BaseValue, Owner.Creature, this);
        for (int i=0;i< base.DynamicVars.Repeat.IntValue;i++) await OrbCmd.Channel(choiceContext, new DocLightningOrb().ToMutable(), Owner);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["FlyingPower"].UpgradeValueBy(1m);

    }
}
