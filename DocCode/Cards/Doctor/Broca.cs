// Broca.cs - 布洛卡
using Doc.DocCode.Attributes;
using Doc.DocCode.Orbs;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Orbs;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Orbs;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.ValueProps;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Doc.DocCode.Cards.Doctor;

[CardTags(isSiracusa: true,isChiaveTeam: true)]
public sealed class Broca() : DocCard(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(8m, ValueProp.Move)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.Static(StaticHoverTip.Evoke),
        HoverTipFactory.FromOrb<LightningOrb>()
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 去除目标敌人的全部格挡
        await CreatureCmd.LoseBlock(cardPlay.Target, cardPlay.Target.Block);

        Player player = base.Owner;
        var orbQueue = player.PlayerCombatState.OrbQueue;

        // 获取所有闪电球（从后往前遍历，避免移除时索引变化）
        var lightningOrbs = orbQueue.Orbs.OfType<DocLightningOrb>().ToList();

        foreach (var orb in lightningOrbs)
        {
            // 从队列中移除该球
            orbQueue.Remove(orb);

            // 手动激发该球
            choiceContext.PushModel(orb);
            await orb.Evoke(choiceContext);
            choiceContext.PopModel(orb);

            // 播放特效和造成伤害
            VfxCmd.PlayOnCreature(cardPlay.Target, "vfx/vfx_attack_lightning");
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
                .FromCard(this)
                .Targeting(cardPlay.Target)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);
        }

    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
    }
}