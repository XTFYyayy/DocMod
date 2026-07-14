using MegaCrit.Sts2.Core.Models;
using System.Collections.Generic;

namespace Doc.DocCode.Extensions;

public static class MonsterAwakeExtension
{
    private static readonly Dictionary<MonsterModel, bool> _awakeStates = new();

    public static bool IsAwake(this MonsterModel monster)
    {
        if (monster == null) return true;
        if (_awakeStates.TryGetValue(monster, out bool value))
        {
            return value;
        }
        return true;
    }

    public static void SetAwake(this MonsterModel monster, bool awake)
    {
        if (monster == null) return;
        _awakeStates[monster] = awake;
    }
}