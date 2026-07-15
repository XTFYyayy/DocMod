using System;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using Doc.DocCode.Powers;

namespace Doc.DocCode.Commands;

public static class CreatureCmdExtensions
{
    /// <summary>
    /// 让目标沉睡（施加一个新实例）
    /// </summary>
    public static async Task SleepWell(
        Creature target,
        decimal duration,
        Creature source,
        CardModel? cardSource = null)
    {
        if (target == null) throw new ArgumentNullException(nameof(target));
        if (source == null) throw new ArgumentNullException(nameof(source));

        // 施加新的沉睡实例
        await PowerCmd.Apply<SleepWellPower>(
            new ThrowingPlayerChoiceContext(),
            target,
            duration,
            source,
            cardSource
        );
    }

    /// <summary>
    /// 让目标沉睡（默认持续1回合）
    /// </summary>
    public static async Task SleepWell(
        Creature target,
        Creature source,
        CardModel? cardSource = null)
    {
        await SleepWell(target, 1m, source, cardSource);
    }
}