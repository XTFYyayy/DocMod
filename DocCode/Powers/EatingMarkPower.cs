using BaseLib.Abstracts;
using Doc.DocCode.Extensions;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Doc.DocCode.Powers;

public sealed class EatingMarkPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override string? CustomPackedIconPath => "eating_mark_power.png".PowerImagePath();
    public override string? CustomBigIconPath => "eating_mark_power.png".PowerImagePath();

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(0m, ValueProp.Unpowered)
    ];
    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (participants.Contains(base.Owner))
        {
            return;
        }

        foreach (int _ in Enumerable.Range(0, Amount))
        {

            foreach (Creature hittableEnemy in base.CombatState.HittableEnemies)
            {
                base.DynamicVars.Damage.BaseValue = base.Owner.Block;

                ((Node)(object)NCombatRoom.Instance?.CombatVfxContainer).AddChildSafely((Node?)(object)NFireSmokePuffVfx.Create(hittableEnemy));

            }
            await Cmd.CustomScaledWait(0.2f, 0.4f);
            
            await CreatureCmd.Damage(choiceContext, base.CombatState.HittableEnemies, base.DynamicVars.Damage, base.Owner);

            await PowerCmd.TickDownDuration(this);

            if (base.CombatState == null || !base.CombatState.HittableEnemies.Any())
            {
                await PowerCmd.Remove(this);
                return;
            }
        }

    }

        
}