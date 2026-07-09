using BaseLib.Utils;
using Doc.DocCode.Attributes;
using Doc.DocCode.Patches;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.ValueProps;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Doc.DocCode.Cards.Doctor;

[CardTags(isSargon: true)]

public sealed class Estelle() : DocCard(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(7, ValueProp.Move)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (IsUpgraded)
        {
            // 升级后：AOE，对所有敌人造成7点伤害
            await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
                .FromCard(this)
                .TargetingAllOpponents(base.CombatState)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);
        }
        else
        {
            // 未升级：单体目标
            if (cardPlay.Target != null)
            {
                await CommonActions.CardAttack(this, cardPlay.Target).Execute(choiceContext);
            }
        }
    }

    protected override void OnUpgrade()
    {
        // 升级后伤害值不变，保持7点
        // TargetType 的变更由补丁处理
        CardModelTargetTypePatch.SetTargetType(this, TargetType.AllEnemies);
    }
}