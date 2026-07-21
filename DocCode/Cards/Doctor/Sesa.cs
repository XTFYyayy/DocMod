using BaseLib.Utils;
using Doc.DocCode.Attributes;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Doc.DocCode.Cards.Doctor;

[CardTags(isSargon:true)]
public sealed class Sesa() : DocCard(4, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(52m, ValueProp.Move)
    ];

    // 基础版本有消耗
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust,];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var target = cardPlay.Target;
        if (target == null) return;
        var combatState = target.CombatState;

        // 造成伤害
        var attackCommand = await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);

        var allResults = attackCommand.Results.FirstOrDefault();
        if (allResults == null) return;

        var mainResult = allResults.FirstOrDefault(r => r.Receiver == target);
        if (mainResult == null) return;

        int unblockedDamage = mainResult.UnblockedDamage + mainResult.OverkillDamage;
        Log.Info($"[Sesa] totalDamageDealt: {unblockedDamage}");

        if (unblockedDamage <= 0)
        {
            Log.Info("[Sesa] No damage dealt, skipping splash");
            return;
        }

        int splashDamage = unblockedDamage / 2; // 一半

       
        if (combatState == null)
        {
            Log.Info($"[Sesa] 所以呢");
            return;
        }

        var otherEnemies = combatState.Enemies
            .Where(e => e != target && e.IsAlive)
            .ToList();

        if (!otherEnemies.Any())
        {
            Log.Info($"[Sesa] 我没招了");
            return;
        }
        Log.Info($"[Sesa] Applying splash damage {splashDamage} to {otherEnemies.Count} enemies");


        // 对所有其他敌人造成溅射伤害（可格挡且能触发能力）
        foreach (var otherEnemy in otherEnemies)
        { 
            await DamageCmd.Attack(splashDamage)
            .FromCard(this)
            .Targeting(otherEnemy)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        }
        
    }

    protected override void OnUpgrade()
    {
        // 升级：移除虚无关键词
        RemoveKeyword(CardKeyword.Exhaust);
    }
}