using Doc.DocCode.Attributes;
using Doc.DocCode.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Doc.DocCode.Cards.Doctor;

[CardTags(isKazimierz: true, isKnight: true)]
public sealed class Blemishine() : DocCard(2, CardType.Skill, CardRarity.Rare, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<VulnerablePower>(1m),
        new PowerVar<AsleepPower>(1m)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<VulnerablePower>(),
        HoverTipFactory.FromPower<AsleepPower>()
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var target = cardPlay.Target;
        if (target == null) return;

        // 施加易伤
        await PowerCmd.Apply<VulnerablePower>(choiceContext, target, DynamicVars.Vulnerable.BaseValue, Owner.Creature, this);

        // 施加沉睡（1层）
        await PowerCmd.Apply<AsleepPower>(choiceContext, target, DynamicVars["AsleepPower"].BaseValue, Owner.Creature, this);

        // 标记为未醒来
        target.Monster.SetAwake(false);

        // 刷新意图，显示 SleepIntent
        var nCreature = target.GetCreatureNode();
        if (nCreature != null)
        {
            var targets = target.CombatState?.Players.Select(p => p.Creature).ToList() ?? new List<Creature>();
            await nCreature.UpdateIntent(targets);
        }
    }

    protected override void OnUpgrade()
    {
        // 升级：易伤从1变为2
        DynamicVars.Vulnerable.UpgradeValueBy(1m);
        // 沉睡层数不变（仍为1）
    }
}