using HarmonyLib;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace Doc.DocCode.Patches;

[HarmonyPatch(typeof(NCreature))]
public static class MonsterSleepIntentPatch
{
    private static FieldInfo? _intentsField;

    static MonsterSleepIntentPatch()
    {
        _intentsField = typeof(NCreature).GetField("_intents", BindingFlags.NonPublic | BindingFlags.Instance);
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(NCreature.UpdateIntent))]
    public static void UpdateIntentPostfix(NCreature __instance)
    {
        try
        {
            var creature = __instance.Entity;
            if (creature == null || !creature.IsAlive) return;

            if (creature.HasPower<AsleepPower>())
            {
                var intents = new List<AbstractIntent> { new SleepIntent() };
                if (_intentsField != null)
                {
                    _intentsField.SetValue(__instance, intents);
                    _ = __instance.RefreshIntents();
                }
            }
        }
        catch (System.Exception ex)
        {
            MainFile.Logger.Warn($"MonsterSleepIntentPatch error: {ex.Message}");
        }
    }
}