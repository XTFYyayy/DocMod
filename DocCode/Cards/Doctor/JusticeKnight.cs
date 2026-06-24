using Doc.DocCode.Attributes;
using Doc.DocCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Doc.DocCode.Cards.Doctor;

[CardTags(isKazimierz: true, isKnight: true)]
public sealed class JusticeKnight() : DocCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.AllEnemies)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<WeakPower>(1m),
        new PowerVar<VulnerablePower>(1m)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<WeakPower>(),
        HoverTipFactory.FromPower<VulnerablePower>()
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 给予所有敌人虚弱
        decimal weakAmount = DynamicVars.Weak.BaseValue;
        if (weakAmount > 0)
        {
            await PowerCmd.Apply<WeakPower>(choiceContext, CombatState.HittableEnemies, weakAmount, Owner.Creature, this);
        }

        // 给予所有敌人易伤
        decimal vulnerableAmount = DynamicVars.Vulnerable.BaseValue;
        if (vulnerableAmount > 0)
        {
            await PowerCmd.Apply<VulnerablePower>(choiceContext, CombatState.HittableEnemies, vulnerableAmount, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        // 升级：虚弱和易伤各增加1层
        DynamicVars.Weak.UpgradeValueBy(1m);
        DynamicVars.Vulnerable.UpgradeValueBy(1m);
    }
}