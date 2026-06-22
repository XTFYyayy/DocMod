using Doc.DocCode;
using Doc.DocCode.Attributes;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Doc.DocCode.Cards.Doctor;

public sealed class UnexoneratedSorrow() : CardModel(0, CardType.Skill, CardRarity.Token, TargetType.Self)
{
    // 虚弱层数（升级前为0，升级后为1）
    protected override IEnumerable<DynamicVar> CanonicalVars => new[]
    {
        new PowerVar<WeakPower>(0m)  // 使用 PowerVar
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new[]
    {
        HoverTipFactory.FromPower<WeakPower>()
    };

    public override IEnumerable<CardKeyword> CanonicalKeywords => new[]
    {
        CardKeyword.Exhaust,
        CardKeyword.Retain
    };


    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 进入平静姿态
        await StanceHelper.EnterStance(choiceContext, Owner.Creature, Stance.Calm, this);

        // 给予所有敌人虚弱（升级后才有）
        decimal weakAmount = DynamicVars.Weak.BaseValue;
        if (weakAmount > 0)
        {
            await PowerCmd.Apply<WeakPower>(CombatState.HittableEnemies, weakAmount, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        // 升级：虚弱层数从0变为1
        DynamicVars.Weak.UpgradeValueBy(1m);
    }
}