using System;
using Doc.DocCode.Attributes;
using MegaCrit.Sts2.Core.Models;

namespace Doc.DocCode.Extensions;

public static class CardExtensions
{
    private static CardTagsAttribute? GetCardTagsAttribute(this CardModel card)//获取卡牌的CardTagsAttribute
    {
        var type = card.GetType();
        return Attribute.GetCustomAttribute(type, typeof(CardTagsAttribute)) as CardTagsAttribute;
    }

    public static bool IsKnight(this CardModel card)//骑士
    {
        var attr = card.GetCardTagsAttribute();
        return attr != null && attr.IsKnight;
    }

    public static bool IsKazimierz(this CardModel card)//卡西米尔
    {
        var attr = card.GetCardTagsAttribute();
        return attr != null && attr.IsKazimierz;
    }

    public static bool IsSargon(this CardModel card)//萨尔贡
    {
        var attr = card.GetCardTagsAttribute();
        return attr != null && attr.IsSargon;
    }

    public static bool IsRhodeIsland(this CardModel card)//罗德岛
    {
        var attr = card.GetCardTagsAttribute();
        return attr != null && attr.IsRhodeIsland;
    }

    public static bool IsLeithania(this CardModel card)//莱塔尼亚
    {
        var attr = card.GetCardTagsAttribute();
        return attr != null && attr.IsLeithania;
    }

    public static bool IsMinos(this CardModel card)//米诺斯
    {
        var attr = card.GetCardTagsAttribute();
        return attr != null && attr.IsMinos;
    }
    public static bool IsColumbia(this CardModel card)//哥伦比亚
    {
        var attr = card.GetCardTagsAttribute();
        return attr != null && attr.IsColumbia;
    }

    public static bool IsBolivar(this CardModel card)//玻利瓦尔
    {
        var attr = card.GetCardTagsAttribute();
        return attr != null && attr.IsBolivar;
    }

    public static bool IsApostle(this CardModel card)//使徒
    {
        var attr = card.GetCardTagsAttribute();
        return attr != null && attr.IsApostle;
    }

    public static bool IsEliteOperator(this CardModel card)//精英干员
    {
        var attr = card.GetCardTagsAttribute();
        return attr != null && attr.IsEliteOperator;
    }

    public static bool IsYan(this CardModel card)//炎
    {
        var attr = card.GetCardTagsAttribute();
        return attr != null && attr.IsYan;
    }

    public static bool IsVictoria(this CardModel card)//维多利亚
    {
        var attr = card.GetCardTagsAttribute();
        return attr != null && attr.IsVictoria;
    }

    public static bool IsTara(this CardModel card)//塔拉
    {
        var attr = card.GetCardTagsAttribute();
        return attr != null && attr.IsTara;
    }

    public static bool IsSweep(this CardModel card)//S.W.E.E.P
    {
        var attr = card.GetCardTagsAttribute();
        return attr != null && attr.IsSweep;
    }

    public static bool IsSiracusa(this CardModel card)//叙拉古
    {
        var attr = card.GetCardTagsAttribute();
        return attr != null && attr.IsSiracusa;
    }
    public static bool IsChiaveTeam(this CardModel card)//贾维团伙
    {
        var attr = card.GetCardTagsAttribute();
        return attr != null && attr.IsChiaveTeam;
    }
    public static bool IsRimBilliton(this CardModel card)//雷姆必拓
    {
        var attr = card.GetCardTagsAttribute();
        return attr != null && attr.IsRimBilliton;
    }
    public static bool IsBabel(this CardModel card)//巴别塔
    {
        var attr = card.GetCardTagsAttribute();
        return attr != null && attr.IsBabel;
    }
}