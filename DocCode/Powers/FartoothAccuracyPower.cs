using BaseLib.Abstracts;
using Doc.DocCode.Extensions;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using System.Threading.Tasks;

namespace Doc.DocCode.Powers;

public sealed class FartoothAccuracyPower : CustomPowerModel
{
    private CardModel _sourceCard;
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override bool AllowNegative => false;
    public override string? CustomPackedIconPath => "fartooth_accuracy_power.png".PowerImagePath();
    public override string? CustomBigIconPath => "fartooth_accuracy_power.png".PowerImagePath();
    public void SetSourceCard(CardModel sourceCard)
    {
        _sourceCard = sourceCard;
    }

    // 每打出一张牌，获得 Amount 层精准
    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 只对自己打出的牌生效
        if (cardPlay.Card.Owner.Creature != Owner) return;

        // 排除小刀
        if (cardPlay.Card.Tags.Contains(CardTag.Shiv)) return;

        // 排除自身（当前这张能力牌本身）
        if (cardPlay.Card == _sourceCard) return;

        int accuracyGain = Amount;

        // 如果是骑士牌，获得双倍
        if (cardPlay.Card.IsKnight())
        {
            accuracyGain *= 2;
        }

        if (accuracyGain > 0)
        {
            await PowerCmd.Apply<AccuracyPower>(Owner, accuracyGain, Owner, null);
        }
    }
}