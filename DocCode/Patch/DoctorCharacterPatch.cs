using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Characters;
using Doc.DocCode.Charaters;
using Doc;  // 添加引用以使用 Logger

namespace Doc.DocCode.Patches;

[HarmonyPatch(typeof(ModelDb))]
public static class DoctorCharacterPatch
{
    [HarmonyPatch(nameof(ModelDb.AllCharacters), MethodType.Getter)]
    [HarmonyPostfix]
    public static void AddDoctorCharacter(ref IEnumerable<CharacterModel> __result)
    {
        var list = __result.ToList();

        // 检查是否已添加，避免重复
        bool alreadyExists = list.Any(c => c is DoctorCharacter);

        if (!alreadyExists)
        {
            MainFile.Logger.Info("Adding DoctorCharacter to character list...");
            var doctorCharacter = new DoctorCharacter();
            list.Add(doctorCharacter);
            __result = list;
            MainFile.Logger.Info($"DoctorCharacter added. Total characters: {list.Count}");
        }
        else
        {
            MainFile.Logger.Info("DoctorCharacter already exists in character list.");
        }
    }
}