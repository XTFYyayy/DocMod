using BaseLib.Extensions;
using Doc.DocCode.Attributes;
using Doc.DocCode.Orbs;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Orbs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Doc.DocCode.Cards.Doctor;

/// <summary>
/// 澄闪（Goldenglow）：维多利亚，2 费技能稀有，消耗（Exhaust）。。
/// TODO 无升级标记，故不做升级。
/// [Pool] 特性由基类 DocCard 继承，子类不再显式声明（项目惯例）。
/// </summary>
[CardTags(isVictoria: true)]
public sealed class Goldenglow() : DocCard(2, CardType.Skill, CardRarity.Rare, TargetType.None)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
[
        HoverTipFactory.Static(StaticHoverTip.Channeling),
        HoverTipFactory.FromOrb<LightningOrb>()
];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 快照当前所有已存在的闪电充能球（避免循环引用新生成的球导致无限扩张）。
        var existingOrbs = Owner.PlayerCombatState.OrbQueue.Orbs.OfType<DocLightningOrb>().ToList();

        int orbCount = existingOrbs.Count;

        // 每个球生成3个新球
        for (int i = 0; i < orbCount * 3; i++)
        {
            await OrbCmd.Channel<DocLightningOrb>(choiceContext, base.Owner);
        }
    }

    protected override void OnUpgrade()
    {
        RemoveKeyword(CardKeyword.Exhaust);
    }
}
