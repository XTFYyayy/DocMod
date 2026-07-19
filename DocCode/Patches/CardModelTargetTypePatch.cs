using Doc.DocCode.Attributes;
using Doc.DocCode.Cards.Doctor;
using Doc.DocCode.Powers;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Multiplayer;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using System.Reflection;

namespace Doc.DocCode.Patches;

// ---------- CardModel 补丁 ----------
[HarmonyPatch(typeof(CardModel))]
public static class CardModelTargetTypePatch
{
    [HarmonyPatch("TargetType", MethodType.Getter)]
    [HarmonyPostfix]
    public static void OverrideTargetType(CardModel __instance, ref TargetType __result)
    {
        if (__instance is Estelle estelle && estelle.IsUpgraded)
        {
            __result = TargetType.AllEnemies;
        }
    }
}

// ---------- NTargetManager 补丁：控制目标是否可选 ----------
[HarmonyPatch(typeof(NTargetManager))]
public static class NTargetManagerPatch
{
    [HarmonyPatch(nameof(NTargetManager.AllowedToTargetNode))]
    [HarmonyPostfix]
    public static void FilterSleepingTargets(NTargetManager __instance, ref bool __result, Node node)
    {
        // 如果已经被其他逻辑判定为不可选，直接返回
        if (!__result) return;

        // 获取当前手牌
        var hand = NCombatRoom.Instance?.Ui?.Hand;
        if (hand == null) return;

        // 检查是否在卡牌选择模式
        if (!hand.InCardPlay) return;

        // 获取当前正在选择的卡牌
        // 通过反射获取 _currentCardPlay 私有字段
        var cardPlayField = typeof(NPlayerHand).GetField("_currentCardPlay",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (cardPlayField == null) return;

        var cardPlay = cardPlayField.GetValue(hand) as NCardPlay;
        if (cardPlay == null) return;

        var card = cardPlay.Holder?.CardModel;
        if (card == null) return;

        // 检查卡牌属性
        var cardType = card.GetType();
        var canTargetSleepingOnly = cardType.GetCustomAttribute<CanTargetSleepingOnlyAttribute>(inherit: true) != null;
        var canTargetSleeping = cardType.GetCustomAttribute<CanTargetSleepingAttribute>(inherit: true) != null;

        // 如果卡牌可以选所有目标，不过滤
        if (canTargetSleeping) return;

        // 获取目标的 Creature
        Creature creature = null;
        if (node is NCreature nCreature)
        {
            creature = nCreature.Entity;
        }
        else if (node is NMultiplayerPlayerState playerState)
        {
            creature = playerState.Player?.Creature;
        }

        if (creature == null) return;

        bool isSleeping = SleepWellPower.IsSleeping(creature);

        // 如果是 CanTargetSleepingOnly，只能选"好好睡"目标
        if (canTargetSleepingOnly)
        {
            __result = isSleeping;
            return;
        }

        // 默认：不能选"好好睡"目标
        if (isSleeping)
        {
            __result = false;
        }
    }
}