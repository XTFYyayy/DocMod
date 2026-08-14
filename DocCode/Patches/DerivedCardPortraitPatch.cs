using Doc.DocCode.Cards.Doctor;
using Doc.DocCode.Extensions;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using System;
using System.Collections.Concurrent;
using System.Reflection;

namespace Doc.DocCode.Patches;

public enum DerivedCardType
{
    UndeclaredRage,
    UnexoneratedSorrow,
    UngloriousGlory,
    Recon,
    RockyChomper,
    HadiyaII,
    Atk,
    Bst,
    Ctrl,
    Dfc
}

[HarmonyPatch]
public static class DerivedCardPortraitPatch
{
    private static readonly ConcurrentDictionary<Type, bool> _matchCache;

    static DerivedCardPortraitPatch()
    {
        // 启动时一次性加载所有枚举名称
        var enumNames = Enum.GetNames(typeof(DerivedCardType));
        _matchCache = new ConcurrentDictionary<Type, bool>();

        // 预加载可选，不预加载也能运行，运行时动态判断
    }

    [HarmonyPatch(typeof(CardModel), nameof(CardModel.PortraitPath), MethodType.Getter)]
    [HarmonyPostfix]
    public static void OverrideDerivedCardPortraitPath(CardModel __instance, ref string __result)
    {
        Type cardType = __instance.GetType();
        bool matched = _matchCache.GetOrAdd(cardType, t =>
        {
            string className = t.Name;
            return Enum.IsDefined(typeof(DerivedCardType), className);
        });

        if (matched)
        {
            var cardName = cardType.Name.ToLowerInvariant();
            __result = $"{cardName}.png".CardImagePath();
        }
    }
}