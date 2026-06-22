using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using System.Linq;
using System.Threading.Tasks;

namespace Doc.DocCode.Powers;

public sealed class NodPower : PowerModel
{
    private bool _isFirstAttackThisTurn = true;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool AllowNegative => false;

    // 每回合开始时重置标志
    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player.Creature != Owner) return;
        _isFirstAttackThisTurn = true;
    }

    // 在伤害计算前修改 ValueProp
    public override bool TryModifyDamageValueProp(Creature? target, decimal damage, ValueProp originalProps, Creature? dealer, CardModel? cardSource, out ValueProp modifiedProps)
    {
        modifiedProps = originalProps;

        // 只修改攻击牌
        if (!originalProps.IsPoweredAttack()) return false;
        if (dealer != Owner) return false;

        // 检查是否是本回合第一张攻击牌
        int attacksThisTurn = CombatManager.Instance.History.CardPlaysStarted.Count(e =>
            e.HappenedThisTurn(CombatState) &&
            e.CardPlay.Card.Type == CardType.Attack &&
            e.CardPlay.Card.Owner.Creature == Owner);

        int isCurrentCard = (cardSource?.Pile?.Type == PileType.Play) ? 1 : 0;

        if (attacksThisTurn > isCurrentCard) return false;
        if (!_isFirstAttackThisTurn) return false;

        // 添加不可格挡标志
        modifiedProps = originalProps | ValueProp.Unblockable;
        _isFirstAttackThisTurn = false;

        return true;
    }
}