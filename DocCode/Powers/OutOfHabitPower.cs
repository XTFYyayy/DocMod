using BaseLib.Abstracts;
using Doc.DocCode.Extensions;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Combat;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;

namespace Doc.DocCode.Powers;

public sealed class OutOfHabitPower : CustomPowerModel
{
    // 存储上一回合结束时的手牌数量
    private int _handCountAtTurnEnd;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override string? CustomPackedIconPath => "out_of_habit_power.png".PowerImagePath();
    public override string? CustomBigIconPath => "out_of_habit_power.png".PowerImagePath();

    // ---------- 玩家侧回合结束时：记录手牌数量 ----------
    public override async Task BeforeSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        // 只在玩家侧回合结束时触发
        if (side != CombatSide.Player) return;

        var player = base.Owner?.Player;
        if (player == null) return;

        // 获取当前手牌数量
        var handPile = PileType.Hand.GetPile(player).Cards;
        _handCountAtTurnEnd = handPile.Count();
    }

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player == base.Owner.Player)
        {
            if (_handCountAtTurnEnd == 0) return;
            await CardPileCmd.Draw(choiceContext, _handCountAtTurnEnd, player);
            await CardCmd.Discard(choiceContext, await CardSelectCmd.FromHandForDiscard(choiceContext, player, new CardSelectorPrefs(CardSelectorPrefs.DiscardSelectionPrompt, PileType.Hand.GetPile(player).Cards.Count < _handCountAtTurnEnd? PileType.Hand.GetPile(player).Cards.Count:_handCountAtTurnEnd), null, this));
        }
        _handCountAtTurnEnd = 0;
    }
    

}