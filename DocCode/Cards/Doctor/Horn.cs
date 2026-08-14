using Doc.DocCode.Attributes;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Doc.DocCode.Cards.Doctor;

[CardTags(isVictoria: true)]
public sealed class Horn() : DocCard(3, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(27m, ValueProp.Move),
        new EnergyVar(2)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var target = cardPlay.Target;
        decimal damage = DynamicVars.Damage.BaseValue;

        // 若这张牌是最后一张手牌（打出后手牌已空）
        bool isLastCardInHand = PileType.Hand.GetPile(Owner).IsEmpty;
        if (isLastCardInHand)
        {
            damage *= 2m;
            await PlayerCmd.GainEnergy(DynamicVars.Energy.IntValue, Owner);
        }

        await DamageCmd.Attack(damage)
            .FromCard(this)
            .Targeting(target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        // 升级后获得保留
        AddKeyword(CardKeyword.Retain);
    }
}
