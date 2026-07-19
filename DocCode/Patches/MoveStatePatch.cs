using HarmonyLib;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Doc.DocCode.Patches;

    [HarmonyPatch(typeof(MoveState))]
    public static class MoveStatePatch
    {
        private static readonly ConditionalWeakTable<MoveState, PatchedMoveData> _patchedData = new();

        public static void SetPatchedData(this MoveState state, string id, IEnumerable<AbstractIntent> intents)
        {
            _patchedData.Remove(state);
            _patchedData.Add(state, new PatchedMoveData { Id = id, Intents = intents });
        }

        private static PatchedMoveData GetPatchedData(MoveState state)
        {
            _patchedData.TryGetValue(state, out var data);
            return data;
        }

        // ---------- 修补 Id getter ----------
        [HarmonyPatch(nameof(MoveState.Id), MethodType.Getter)]
        [HarmonyPrefix]
        private static bool IdPrefix(MoveState __instance, ref string __result)
        {
            var data = GetPatchedData(__instance);
            if (data != null)
            {
                __result = data.Id;
                return false; // 跳过原始 getter
            }
            return true; // 使用原始 getter
        }

        // ---------- 修补 Intents getter ----------
        [HarmonyPatch(nameof(MoveState.Intents), MethodType.Getter)]
        [HarmonyPrefix]
        private static bool IntentsPrefix(MoveState __instance, ref IEnumerable<AbstractIntent> __result)
        {
            var data = GetPatchedData(__instance);
            if (data != null)
            {
                __result = data.Intents;
                return false;
            }
            return true;
        }

        private class PatchedMoveData
        {
            public string Id { get; set; }
            public IEnumerable<AbstractIntent> Intents { get; set; }
        }
    }
