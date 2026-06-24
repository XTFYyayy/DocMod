using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Doc.DocCode.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Doc.DocCode.Cards.Doctor;

[CardTags(isKazimierz: true, isKnight: true)]
public sealed class WildMane() : DocCard(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(6m, ValueProp.Move),
        new EnergyVar(1)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var target = cardPlay.Target;

        var result = await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);

        // 检查目标是否被击杀
        // Results 是 IEnumerable<List<DamageResult>>，每个内层列表对应一次命中
        bool targetKilled = false;
        if (result.Results != null)
        {
            foreach (var hitList in result.Results)
            {
                // 遍历本次命中的所有伤害结果
                if (hitList.Any(r => r.WasTargetKilled))
                {
                    targetKilled = true;
                    break;
                }
            }
        }

        if (targetKilled)
        {
            // 抽一张牌
            await CardPileCmd.Draw(choiceContext, 1, Owner);

            // 回复 1 点能量
            await PlayerCmd.GainEnergy(DynamicVars.Energy.IntValue, Owner);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
    }
}