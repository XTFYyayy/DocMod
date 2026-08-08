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
    }

    // 便捷构造函数
    public CardTagsAttribute(bool isKazimierz = false, bool isKnight = false, bool isSargon = false, bool isRhodeIsland = false, bool isLeithania=false, bool isColumbia = false, bool isMinos = false, bool isBolivar = false, bool isApostle = false)
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
    }
}