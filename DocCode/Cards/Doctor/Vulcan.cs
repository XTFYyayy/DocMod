using Doc.DocCode.Attributes;
using Doc.DocCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Doc.DocCode.Cards.Doctor;

/// <summary>
/// 【火神Vulcan】：奇巧。铸剑5/8。君王之剑的耗能减1。技能，普通。
/// </summary>
[CardTags(isMinos: true)]
public sealed class Vulcan() : DocCard(3, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    // 关键词：奇巧(Sly)
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Sly];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => HoverTipFactory.FromForge();

    // 动态变量：铸剑 5（升级后 +3 → 8）
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new ForgeVar(5)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 铸造动画 + 锻造君王之剑
        await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);
        await ForgeCmd.Forge(base.DynamicVars.Forge.IntValue, base.Owner, this);

        // 施加火神之力：君王之剑耗能 -1
        await PowerCmd.Apply<ForceModePower>(
            choiceContext,
            base.Owner.Creature,
            1,
            base.Owner.Creature,
            this
        );
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Forge.UpgradeValueBy(3m);
    }
}
