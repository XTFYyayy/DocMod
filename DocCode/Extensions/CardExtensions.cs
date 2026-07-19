using System;
using Doc.DocCode.Attributes;
using MegaCrit.Sts2.Core.Models;

namespace Doc.DocCode.Extensions;

public static class CardExtensions
{
    private static CardTagsAttribute? GetCardTagsAttribute(this CardModel card)
    {
        var type = card.GetType();
        return Attribute.GetCustomAttribute(type, typeof(CardTagsAttribute)) as CardTagsAttribute;
    }

    public static bool IsKnight(this CardModel card)
    {
        var attr = card.GetCardTagsAttribute();
        return attr != null && attr.IsKnight;
    }

    public static bool IsKazimierz(this CardModel card)
    {
        var attr = card.GetCardTagsAttribute();
        return attr != null && attr.IsKazimierz;
    }

    public static bool IsSargon(this CardModel card)
    {
        var attr = card.GetCardTagsAttribute();
        return attr != null && attr.IsSargon;
    }

    public static bool IsRhodeIsland(this CardModel card)
    {
        var attr = card.GetCardTagsAttribute();
        return attr != null && attr.IsRhodeIsland;
    }

    public static bool IsLeithania(this CardModel card)
    {
        var attr = card.GetCardTagsAttribute();
        return attr != null && attr.IsLeithania;
    }
}