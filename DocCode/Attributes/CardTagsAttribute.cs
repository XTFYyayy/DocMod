using System;

namespace Doc.DocCode.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class CardTagsAttribute : Attribute
{
    public bool IsKnight { get; set; }
    public bool IsKazimierz { get; set; }
    public bool IsSargon { get; set; }
    public bool IsRhodeIsland { get; set; }
    public bool IsLeithania { get; set; }
    public bool IsColumbia { get; set; }

    public CardTagsAttribute()
    {
        IsKnight = false;
        IsKazimierz = false;
        IsSargon= false;
        IsRhodeIsland = false;
        IsLeithania = false;
        IsColumbia = false;
    }

    // 便捷构造函数
    public CardTagsAttribute(bool isKazimierz = false, bool isKnight = false, bool isSargon = false, bool isRhodeIsland = false, bool isLeithania=false, bool isColumbia = false)
    {
        IsKazimierz = isKazimierz;
        IsKnight = isKnight;
        IsSargon = isSargon;
        IsRhodeIsland = isRhodeIsland;
        IsLeithania = isLeithania;
        IsColumbia = isColumbia;
    }
}