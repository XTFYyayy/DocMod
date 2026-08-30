using Doc.DocCode.Attributes;
using Doc.DocCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Doc.DocCode.Cards.Doctor;

/// <summary>
/// 云迹（Contrail）：哥伦比亚，1 费技能普通。
/// 打出：对自身获得 1 层起飞（升级后 2 层）。
/// [Pool] 特性由基类 DocCard 继承，子类不再显式声明（项目惯例）。
/// </summary>
[CardTags(isColumbia: true)]
public sealed class Contrail() : DocCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("FlyingPower", 1m)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<FlyingPower>()
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 对自身获得起飞（1 层，升级后 2 层）
        await PowerCmd.Apply<FlyingPower>(choiceContext, Owner.Creature, DynamicVars["FlyingPower"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["FlyingPower"].UpgradeValueBy(1m);
    }
}
