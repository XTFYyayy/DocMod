using System;

namespace Doc.DocCode.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class CardTagsAttribute : Attribute
{
    public bool IsKnight { get; set; }
    public bool IsKazimierz { get; set; }

    public CardTagsAttribute()
    {
        IsKnight = false;
        IsKazimierz = false;
    }

    // 便捷构造函数
    public CardTagsAttribute(bool isKazimierz, bool isKnight = false)
    {
        IsKazimierz = isKazimierz;
        IsKnight = isKnight;
    }
}