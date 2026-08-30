using Doc.DocCode.Attributes;
using Doc.DocCode.Orbs;
using Doc.DocCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Orbs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Doc.DocCode.Cards.Doctor;

/// <summary>
/// 布丁（Pudding）：哥伦比亚，1 费技能普通。
/// 打出：生成 1 个闪电充能球，抽 1 张牌（升级后抽 2 张）。
/// 闪电球统一使用模组自定义类 DocLightningOrb（继承官方 LightningOrb）。
/// [Pool] 特性由基类 DocCard 继承，子类不再显式声明（项目惯例）。
/// </summary>
[CardTags(isColumbia: true)]
public sealed class Pudding() : DocCard(1, CardType.Skill, CardRarity.Common, TargetType.None)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(1)
    ];
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.Static(StaticHoverTip.Channeling),
        HoverTipFactory.FromOrb<LightningOrb>()
    ];
    
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {

        await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);
        await OrbCmd.Channel<DocLightningOrb>(choiceContext, base.Owner);
        await CardPileCmd.Draw(choiceContext, base.DynamicVars.Cards.BaseValue, base.Owner);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1m);
    }
}
