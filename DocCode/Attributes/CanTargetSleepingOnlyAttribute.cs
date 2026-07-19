using System;

namespace Doc.DocCode.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class CanTargetSleepingOnlyAttribute : Attribute
    {
    }
}