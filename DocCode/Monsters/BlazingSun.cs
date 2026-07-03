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

public sealed class BlazingSun : MonsterModel, ICustomSummon
{
    public const string MonsterId = "BlazingSun";

    public override int MinInitialHp => 1;
    public override int MaxInitialHp => 1;
    public override bool IsHealthBarVisible => true;

    // 实现接口属性
    public string VisualsScenePath => "res://scenes/creature_visuals/blazing_sun.tscn";
    public string FallbackTexturePath => "res://Images/summon_blazing_sun.png";

    protected override MonsterMoveStateMachine GenerateMoveStateMachine()
    {
        MoveState idleState = new MoveState("IDLE", (IReadOnlyList<Creature> _) => Task.CompletedTask);
        idleState.FollowUpState = idleState;
        return new MonsterMoveStateMachine(new[] { idleState }, idleState);
    }

    public override CreatureAnimator GenerateAnimator(MegaSprite controller)
    {
        AnimState idleState = new AnimState("idle", isLooping: true);
        return new CreatureAnimator(idleState, null);
    }
}