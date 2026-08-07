using Doc.DocCode.Attributes;
using Doc.DocCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Doc.DocCode.Cards.Doctor;

[CardTags(isMinos: true)]
public sealed class Sideroca() : DocCard(4, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    // 关键词：固有(Innate) + 奇巧(Sly)
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Innate, CardKeyword.Sly];

    // 固有 + 奇巧 + 能力 悬浮提示
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<TheResilienceOfMercenariesPower>(),
        HoverTipFactory.FromCard<SovereignBlade>()
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);
        await PowerCmd.Apply<TheResilienceOfMercenariesPower>(choiceContext, Owner.Creature, 1, Owner.Creature, this);
    }

    /// <summary>
    /// 升级：获得保留(Retain)关键词
    /// </summary>
    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
    }
}
