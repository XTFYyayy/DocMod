using Doc.DocCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using System;
using System.Threading.Tasks;

namespace Doc.DocCode;

public static class StanceHelper
{
    /// <summary>
    /// 获取角色当前姿态。
    /// </summary>
    public static Stance GetCurrentStance(Creature creature)
    {
        var stancePower = creature.GetPower<BaseStancePower>();
        return stancePower?.StanceType ?? Stance.None;
    }

    /// <summary>
    /// 进入新姿态。若已在其他姿态则先退出，再进入目标姿态。
    /// </summary>
    public static async Task EnterStance(PlayerChoiceContext choiceContext, Creature creature, Stance newStance, CardModel? source = null)
    {
        var currentStance = GetCurrentStance(creature);
        if (currentStance == newStance) return;

        if (currentStance != Stance.None)
        {
            await ExitStance(choiceContext, creature);
        }

        if (newStance == Stance.None) return;

        BaseStancePower stancePower = newStance switch
        {
            Stance.Wrath => await PowerCmd.Apply<WrathStancePower>(choiceContext, creature, 1m, creature, source),
            Stance.Calm => await PowerCmd.Apply<CalmStancePower>(choiceContext, creature, 1m, creature, source),
            Stance.Divinity => await PowerCmd.Apply<DivinityStancePower>(choiceContext, creature, 1m, creature, source),
            _ => throw new ArgumentOutOfRangeException(nameof(newStance), newStance, null)
        };

        // 神格入场：获得3点能量
        if (newStance == Stance.Divinity)
        {
            await PlayerCmd.GainEnergy(3, creature.Player);
        }
    }

    /// <summary>
    /// 退出当前姿态。
    /// </summary>
    public static async Task ExitStance(PlayerChoiceContext choiceContext, Creature creature)
    {
        var stancePower = creature.GetPower<BaseStancePower>();
        if (stancePower != null)
        {
            await PowerCmd.Remove(stancePower);
        }
    }
}
