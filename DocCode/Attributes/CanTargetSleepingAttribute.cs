using System;

namespace Doc.DocCode.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public sealed class CanTargetSleepingAttribute : Attribute
{
}