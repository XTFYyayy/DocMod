using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.Nodes.Combat;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Doc.DocCode.Extensions
{
    public static class CreatureCmdExtensions
    {
        public static async Task SleepWell(this Creature creature)
        {
            if (!creature.IsMonster || creature.Monster == null)
                return;

            var monster = creature.Monster;

            // 创建一个"什么都不做"的执行方法
            async Task DoNothing(IReadOnlyList<Creature> targets)
            {
                // 可以添加睡眠动画/特效
                await Task.CompletedTask;
            }

            // 使用正确的构造函数创建 MoveState
            var sleepState = new MoveState(
                stateId: "SLEEP_WELL_MOVE",
                onPerform: DoNothing,
                intents: new SleepIntent()  // 官方睡眠意图
            );

            // 强制切换状态
            monster.SetMoveImmediate(sleepState, forceTransition: true);

            // 刷新意图图标
            var creatureNode = creature.GetCreatureNode();
            if (creatureNode != null)
            {
                await TaskHelper.RunSafely(creatureNode.RefreshIntents());
            }
            await Task.CompletedTask;
        }
    }
}