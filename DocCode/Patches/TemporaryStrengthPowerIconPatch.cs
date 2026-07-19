using Doc.DocCode.Extensions;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using System.Reflection;

namespace Doc.DocCode.Patches;

[HarmonyPatch]
public static class TemporaryStrengthPowerIconPatch
{
    // 找到 PowerModel 的 Icon 属性的 getter 方法
    private static MethodBase TargetMethod()
    {
        var type = typeof(PowerModel);
        return type.GetProperty("Icon")?.GetMethod;
    }

    [HarmonyPrefix]
    public static bool OverrideIcon(PowerModel __instance, ref Texture2D __result)
    {
        // 只处理特定的 Power 类型
        if (__instance.GetType().Name == "ProvisoStrengthLossPower")
        {
            var path = "proviso_strength_loss_power.png".PowerImagePath();
            if (ResourceLoader.Exists(path))
            {
                __result = ResourceLoader.Load<Texture2D>(path);
                return false; // 跳过原方法
            }
        }

        return true; // 继续执行原方法
    }
}