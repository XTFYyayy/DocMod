using Doc.DocCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using System.Threading.Tasks;

namespace Doc.DocCode;

public static class StanceHelper
{
    // 获取角色当前姿态
    public static Stance GetCurrentStance(Creature creature)
    {
        var stancePower = creature.GetPower<StancePower>();
        return stancePower?.CurrentStance ?? Stance.None;
    }

    // 进入新姿态
    public static async Task EnterStance(PlayerChoiceContext choiceContext, Creature creature, Stance newStance, CardModel? source = null)
    {
        var currentStance = GetCurrentStance(creature);
        if (currentStance == newStance) return;

        if (currentStance != Stance.None)
        {
            await ExitStance(choiceContext, creature);
        }

        if (newStance == Stance.None) return;

        var stancePower = await PowerCmd.Apply<StancePower>(creature, 1m, creature, source);
        stancePower.SetStance(newStance);

        if (newStance == Stance.Divinity)
        {
            await PlayerCmd.GainEnergy(3, creature.Player);
        }
    }

    // 退出当前姿态
    public static async Task ExitStance(PlayerChoiceContext choiceContext, Creature creature)
    {
        var stancePower = creature.GetPower<StancePower>();
        stancePower?.RemoveInternal();
    }
}