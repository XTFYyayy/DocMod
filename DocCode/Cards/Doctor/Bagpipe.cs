using Doc.DocCode.Attributes;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Doc.DocCode.Cards.Doctor;

[CardTags(isVictoria: true)]
public sealed class Bagpipe() : DocCard(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(9m, ValueProp.Move),
        new EnergyVar(1)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var target = cardPlay.Target;
        bool targetKilled = false;

        // 造成9点伤害2次
        for (int i = 0; i < 2; i++)
        {
            var result = await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
                .FromCard(this)
                .Targeting(target)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);

            if (!targetKilled && result.Results != null)
            {
                foreach (var hitList in result.Results)
                {
                    if (hitList.Any(r => r.WasTargetKilled))
                    {
                        targetKilled = true;
                        break;
                    }
                }
            }
        }

        // 若击杀了敌人，获得1费
        if (targetKilled)
        {
            await PlayerCmd.GainEnergy(DynamicVars.Energy.IntValue, Owner);
        }
    }

    protected override void OnUpgrade()
    {
        // 费用 2 → 1
        EnergyCost.UpgradeBy(-1);
    }
}
