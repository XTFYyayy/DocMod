using Doc.DocCode.Managers;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using System.Collections.Generic;

namespace Doc.DocCode.Patches;

[HarmonyPatch]
public static class DieForYouDamageInterceptor
{
    private static bool _isProcessing = false;

    [HarmonyTargetMethod]
    public static System.Reflection.MethodBase TargetMethod()
    {
        return AccessTools.Method(
            typeof(CreatureCmd),
            nameof(CreatureCmd.Damage),
            new System.Type[] {
                typeof(PlayerChoiceContext),
                typeof(IEnumerable<Creature>),
                typeof(decimal),
                typeof(ValueProp),
                typeof(Creature),
                typeof(CardModel)
            }
        );
    }

    [HarmonyPrefix]
    public static bool Prefix(
        PlayerChoiceContext choiceContext,
        IEnumerable<Creature> targets,
        ref decimal amount,
        ValueProp props,
        Creature dealer,
        CardModel cardSource)
    {
        if (_isProcessing) return true;
        if (targets == null) return true;

        var targetList = new List<Creature>(targets);
        if (targetList.Count != 1) return true;

        Creature target = targetList[0];
        if (target == null || target.Player == null) return true;

        // 只处理攻击伤害
        if (!props.IsPoweredAttack()) return true;

        Player targetPlayer = target.Player;

        // 检查目标玩家是否还活着
        if (targetPlayer == null || targetPlayer.Creature == null || !targetPlayer.Creature.IsAlive) return true;

        // 检查是否有"为你而死"的召唤物
        var pets = DieForYouManager.GetPetsWithDieForYou(targetPlayer);
        if (pets.Count == 0) return true;

        // 计算目标格挡对伤害的影响
        decimal remainingDamage = amount;
        bool blockAbsorbedAll = false;

        // 如果目标是玩家，计算玩家的格挡
        if (targetPlayer.Creature != null && targetPlayer.Creature == target)
        {
            int block = targetPlayer.Creature.Block;
            if (block > 0 && !props.HasFlag(ValueProp.Unblockable))
            {
                if (block >= remainingDamage)
                {
                    // 格挡完全吸收伤害
                    MainFile.Logger.Info($"DieForYouChain: Player block ({block}) absorbed all {remainingDamage} damage");
                    var blockField = typeof(Creature).GetField("_block",
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    if (blockField != null)
                    {
                        blockField.SetValue(targetPlayer.Creature, block - (int)remainingDamage);
                    }
                    // 伤害被完全抵消，但不要跳过原方法，让后续流程正常执行
                    amount = 0;
                    blockAbsorbedAll = true;
                    return true; // 让原方法继续执行，但 amount 为 0
                }
                else
                {
                    MainFile.Logger.Info($"DieForYouChain: Player block ({block}) absorbed part of {remainingDamage}");
                    var blockField = typeof(Creature).GetField("_block",
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    if (blockField != null)
                    {
                        blockField.SetValue(targetPlayer.Creature, 0);
                    }
                    remainingDamage -= block;
                }
            }
        }

        // 如果剩余伤害为0，让原方法继续执行（amount 已为 0）
        if (remainingDamage <= 0)
        {
            amount = 0;
            return true;
        }

        _isProcessing = true;
        try
        {
            // 只有未被格挡的伤害才触发"为你而死"链
            decimal absorbed = DieForYouManager.ProcessDamageChain(targetPlayer, remainingDamage, props, dealer, cardSource);
            remainingDamage -= absorbed;
            if (remainingDamage < 0) remainingDamage = 0;

            amount = remainingDamage;
            MainFile.Logger.Info($"DieForYouChain: After chain, remaining damage to player: {amount}");

            // 即使 amount 为 0，也返回 true 让原方法继续执行
            // 这样 AttackHitHook 等后续钩子能正常工作
            return true;
        }
        finally
        {
            _isProcessing = false;
        }
    }
}