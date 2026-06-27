using Godot;
using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Monsters;
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
        MoveState moveState = new MoveState("NOTHING_MOVE", (IReadOnlyList<Creature> _) => Task.CompletedTask);
        moveState.FollowUpState = moveState;
        return new MonsterMoveStateMachine(new[] { moveState }, moveState);
    }

    public override CreatureAnimator GenerateAnimator(MegaSprite controller)
    {
        // 创建一个空的动画状态，让 Spine 不报错
        // 虽然耀阳不用 Spine，但基类要求实现这个方法
        AnimState idleState = new AnimState("idle_loop", isLooping: true);
        CreatureAnimator animator = new CreatureAnimator(idleState, controller);
        return animator;
    }
}