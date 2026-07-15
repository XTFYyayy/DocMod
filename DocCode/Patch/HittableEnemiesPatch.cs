using Doc.DocCode.Attributes;
using Doc.DocCode.Powers;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Combat;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Doc.DocCode.Patches;

[HarmonyPatch(typeof(CombatState))]
public static class HittableEnemiesPatch
{
    private static readonly System.Threading.AsyncLocal<CardModel?> _currentCard = new();
    private static PropertyInfo? _cardProperty;

    private static PropertyInfo GetCardProperty()
    {
        if (_cardProperty == null)
        {
            _cardProperty = typeof(NCardPlay).GetProperty("Card",
                BindingFlags.NonPublic | BindingFlags.Instance);
        }
        return _cardProperty;
    }

    public static CardModel? CurrentCard
    {
        get => _currentCard.Value;
        set => _currentCard.Value = value;
    }

    [HarmonyPatch(typeof(NCardPlay), "TryPlayCard", new System.Type[] { typeof(Creature) })]
    [HarmonyPrefix]
    public static void SetCurrentCard(NCardPlay __instance)
    {
        var prop = GetCardProperty();
        var card = prop?.GetValue(__instance) as CardModel;
        CurrentCard = card;
    }

    [HarmonyPatch(typeof(NCardPlay), "TryPlayCard", new System.Type[] { typeof(Creature) })]
    [HarmonyPostfix]
    public static void ClearCurrentCard()
    {
        CurrentCard = null;
    }

    [HarmonyPatch(nameof(CombatState.HittableEnemies), MethodType.Getter)]
    [HarmonyPostfix]
    public static void FilterSleepingEnemies(CombatState __instance, ref IReadOnlyList<Creature> __result)
    {
        var card = CurrentCard;
        var canTargetSleeping = card != null &&
            card.GetType().GetCustomAttributes(typeof(CanTargetSleepingAttribute), false).Any();

        if (canTargetSleeping)
        {
            __result = __instance.Enemies.Where(e => e.IsHittable).ToList();
        }
        else
        {
            __result = __instance.Enemies
                .Where(e => e.IsHittable && !SleepWellPower.IsSleeping(e))
                .ToList();
        }
    }
}