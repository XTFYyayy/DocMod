using Godot;
using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Doc.DocCode.Monsters;

public sealed class BlazingSun : MonsterModel
{
    public const string MonsterId = "BlazingSun";

    public override int MinInitialHp => 1;
    public override int MaxInitialHp => 1;

    public override bool IsHealthBarVisible => base.Creature.IsAlive;

    protected override MonsterMoveStateMachine GenerateMoveStateMachine()
    {
        // 耀阳没有主动移动，只使用空状态
        MoveState moveState = new MoveState("NOTHING_MOVE", (IReadOnlyList<Creature> _) => Task.CompletedTask);
        moveState.FollowUpState = moveState;
        return new MonsterMoveStateMachine(new[] { moveState }, moveState);
    }

    public override CreatureAnimator GenerateAnimator(MegaSprite controller)
    {
        // 待机状态（循环）
        AnimState idleState = new AnimState("idle_loop", isLooping: true);

        // 受击状态
        AnimState hurtState = new AnimState("hurt");
        hurtState.NextState = idleState;

        // 死亡状态
        AnimState dieState = new AnimState("die");
        AnimState deadState = new AnimState("dead_loop", isLooping: true);
        dieState.NextState = deadState;

        // 复活状态
        AnimState reviveState = new AnimState("revive");
        reviveState.NextState = idleState;

        // 创建动画控制器
        CreatureAnimator animator = new CreatureAnimator(idleState, controller);

        // 添加状态转换
        animator.AddAnyState("Hit", hurtState);
        animator.AddAnyState("Dead", dieState);
        animator.AddAnyState("Revive", reviveState);

        return animator;
    }
}