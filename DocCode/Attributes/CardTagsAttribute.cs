using System;

namespace Doc.DocCode.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class CardTagsAttribute : Attribute
{
    public bool IsKnight { get; set; }//骑士
    public bool IsKazimierz { get; set; }//卡西米尔
    public bool IsSargon { get; set; }//萨尔贡
    public bool IsRhodeIsland { get; set; }//罗德岛
    public bool IsLeithania { get; set; }//莱塔尼亚
    public bool IsColumbia { get; set; }//哥伦比亚
    public bool IsMinos { get; set; }//米诺斯
    public bool IsBolivar { get; set; }//玻利瓦尔
    public bool IsApostle { get; set; }//使徒
    public bool IsEliteOperator { get; set; }//精英干员
    public bool IsYan { get; set; }//炎
    public bool IsVictoria { get; set; }//维多利亚
    public bool IsTara { get; set; }//塔拉
    public bool IsSweep { get; set; }//S.W.E.E.P
    public bool IsSiracusa { get; set; }//叙拉古
    public CardTagsAttribute()
    {
        IsKnight = false;
        IsKazimierz = false;
        IsSargon= false;
        IsRhodeIsland = false;
        IsLeithania = false;
        IsColumbia = false;
        IsMinos = false;
        IsBolivar = false;
        IsApostle = false;
        IsEliteOperator = false;
        IsYan = false;
        IsVictoria = false;
        IsTara = false;
        IsSweep = false;
        IsSiracusa = false;
    }

    // 便捷构造函数
    public CardTagsAttribute(bool isKazimierz = false, bool isKnight = false, bool isSargon = false, bool isRhodeIsland = false, bool isLeithania=false, bool isColumbia = false, bool isMinos = false, bool isBolivar = false, bool isApostle = false, bool isEliteOperator = false, bool isYan = false, bool isVictoria = false, bool isTara = false, bool isSweep = false, bool isSiracusa = false)
    {
        IsKazimierz = isKazimierz;
        IsKnight = isKnight;
        IsSargon = isSargon;
        IsRhodeIsland = isRhodeIsland;
        IsLeithania = isLeithania;
        IsColumbia = isColumbia;
        IsMinos = isMinos;
        IsBolivar = isBolivar;
        IsApostle = isApostle;
        IsEliteOperator = isEliteOperator;
        IsYan = isYan;
        IsVictoria = isVictoria;
        IsTara = isTara;
        IsSweep = isSweep;
        IsSiracusa = isSiracusa;
    }
}