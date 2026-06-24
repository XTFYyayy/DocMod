using BaseLib.Utils;
using Doc.DocCode.Attributes;
using Doc.DocCode.Extensions;
using Doc.DocCode.Powers;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Doc.DocCode.Cards.Doctor;

[CardTags(isKazimierz: true, isKnight: true)]
public sealed class Fartooth() : DocCard(3, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("BaseAmount", 1m)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<FartoothAccuracyPower>()
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var existingPower = Owner.Creature.GetPower<FartoothAccuracyPower>();
        if (existingPower != null)
        {
            int addAmount = (int)DynamicVars["BaseAmount"].BaseValue;
            existingPower.SetAmount(existingPower.Amount + addAmount);
        }
        else
        {
            // 没有能力，施加新能力并设置来源卡牌
            int initialAmount = (int)DynamicVars["BaseAmount"].BaseValue;
            var newPower = await PowerCmd.Apply<FartoothAccuracyPower>(choiceContext, Owner.Creature, initialAmount, Owner.Creature, this);
            newPower?.SetSourceCard(this);
        }
    }

    protected override void OnUpgrade()
    {
        // 升级：费用从3变为2
        EnergyCost.UpgradeBy(-1);
    }
}