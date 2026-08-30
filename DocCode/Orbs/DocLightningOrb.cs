using Doc.DocCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Orbs;
using MegaCrit.Sts2.Core.ValueProps;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Doc.DocCode.Orbs;

public sealed class DocLightningOrb : LightningOrb
{
    public override async Task Passive(PlayerChoiceContext choiceContext, Creature? target)
    {
        Trigger();
        await ApplyLightningDamage(PassiveVal, choiceContext);
    }

    public override async Task<IEnumerable<Creature>> Evoke(PlayerChoiceContext playerChoiceContext)
    {
        return await ApplyLightningDamage(EvokeVal, playerChoiceContext);
    }

    private async Task<IEnumerable<Creature>> ApplyLightningDamage(decimal value, PlayerChoiceContext choiceContext)
    {
        List<Creature> hittable = CombatState.HittableEnemies.ToList();
        if (hittable.Count == 0)
        {
            return System.Array.Empty<Creature>();
        }

        List<Creature> targets;
        if (Owner.Creature.GetPower<LightningStrikesAllPower>() != null)
        {
            targets = hittable;
        }
        else
        {
            targets = new List<Creature> { Owner.RunState.Rng.CombatTargets.NextItem(hittable) };
        }

        foreach (Creature target in targets)
        {
            decimal v = value;
            if (target.HasPower<LockOnTrackingPower>())
            {
                v *= 1.5m;
            }

            VfxCmd.PlayOnCreature(target, "vfx/vfx_attack_lightning");
            await CreatureCmd.Damage(choiceContext, new List<Creature> { target }, v, ValueProp.Unpowered, Owner.Creature);
        }

        return targets;
    }
}