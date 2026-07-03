using BaseLib.Abstracts;
using Doc.DocCode.Extensions;
using Godot;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using System.Reflection;
using System.Threading.Tasks;

namespace Doc.DocCode.Powers;

public sealed class NodPower : CustomPowerModel
{
    private bool _hasBeenUsedThisTurn = false;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool AllowNegative => false;

    public override string? CustomPackedIconPath => "nod_power.png".PowerImagePath();
    public override string? CustomBigIconPath => "nod_power.png".PowerImagePath();

    // 缓存反射字段以提高性能
    private static FieldInfo? _damagePropsField;

    static NodPower()
    {
        _damagePropsField = typeof(AttackCommand).GetField("<DamageProps>k__BackingField",
            BindingFlags.NonPublic | BindingFlags.Instance);
    }

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player.Creature != Owner.PetOwner?.Creature) return;
        _hasBeenUsedThisTurn = false;
    }

    public override async Task BeforeAttack(AttackCommand command)
    {
        MainFile.Logger.Info($"NodPower.BeforeAttack called! Attacker={command.Attacker?.Name}, OwnerPetOwner={Owner.PetOwner?.Creature?.Name}");


        // 只对主人的攻击生效
        if (command.Attacker != Owner.PetOwner?.Creature) return;

        // 如果本回合已经使用过，不再生效
        if (_hasBeenUsedThisTurn) return;

        // 标记已使用
        _hasBeenUsedThisTurn = true;

        // 通过反射添加无视格挡属性
        if (_damagePropsField != null)
        {
            var currentProps = (ValueProp)_damagePropsField.GetValue(command);
            _damagePropsField.SetValue(command, currentProps | ValueProp.Unblockable);
        }
    }
}