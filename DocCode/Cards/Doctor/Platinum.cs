using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;
using Doc.DocCode.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Doc.DocCode.Cards.Doctor;

[CardTags(isKazimierz: true)]
public sealed class Platinum() : DocCard(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    private int _timesPlayedThisCombat = 0;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(7m, ValueProp.Move),
        new EnergyVar(1)  // 添加能量变量，用于描述显示
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        decimal damage = DynamicVars.Damage.BaseValue;

        // 如果这是本场战斗中第一次打出，造成双倍伤害
        if (_timesPlayedThisCombat == 0)
        {
            damage *= 2m;
        }

        // 造成伤害
        await DamageCmd.Attack(damage)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);

        // 增加本场战斗中的打出次数
        _timesPlayedThisCombat++;

        // 增加本场战斗中的能量消耗（仅这张牌实例）
        // 使用 AddThisCombat 而不是 SetThisCombat，类似 BansheesCry 的方式
        EnergyCost.AddThisCombat(_timesPlayedThisCombat);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
    }

    // 战斗结束时重置
    public override async Task AfterCombatEnd(CombatRoom room)
    {
        _timesPlayedThisCombat = 0;
        await base.AfterCombatEnd(room);
    }
}