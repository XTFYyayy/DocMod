using Doc.DocCode.Attributes;
using Doc.DocCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Doc.DocCode.Cards.Doctor;

[CardTags(isRhodeIsland: true, isSweep: true, isSiracusa: true)]
public sealed class ProjektRed() : DocCard(3, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<StunPower>(),
        HoverTipFactory.FromPower<BoneCuttingPower>()
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 击晕敌人
        await CreatureCmd.Stun(cardPlay.Target);

        // 施加刺骨：这名敌人每次从攻击中受到的伤害至少为5
        await PowerCmd.Apply<BoneCuttingPower>(choiceContext, cardPlay.Target, 1, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
