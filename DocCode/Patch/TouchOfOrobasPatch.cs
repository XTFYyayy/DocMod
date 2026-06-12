using Doc.DocCode.Relics;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using System.Collections.Generic;

namespace Doc.DocCode.Patches;

[HarmonyPatch(typeof(TouchOfOrobas))]
public static class TouchOfOrobasPatch
{
    [HarmonyPatch("RefinementUpgrades", MethodType.Getter)]
    [HarmonyPostfix]
    public static void AddUpgradeMapping(ref Dictionary<ModelId, RelicModel> __result)
    {
        // 添加博士的遗物升级映射
        var bronzeSealId = ModelDb.Relic<HrBronzeSeal>().Id;
        var silverSealId = ModelDb.Relic<DoctorSilverSeal>().Id;

        if (!__result.ContainsKey(bronzeSealId))
        {
            __result[bronzeSealId] = ModelDb.GetById<RelicModel>(silverSealId);
        }
    }
}