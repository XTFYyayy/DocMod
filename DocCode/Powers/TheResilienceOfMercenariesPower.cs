using BaseLib.Abstracts;
using Doc.DocCode.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using System.Linq;
using System.Threading.Tasks;

namespace Doc.DocCode.Powers;

/// <summary>
/// 佣兵的坚韧：打出君王之剑时，所有debuff减1层
/// </summary>
public sealed class TheResilienceOfMercenariesPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override string? CustomPackedIconPath => "the_resilience_of_mercenaries_power.png".PowerImagePath();
    public override string? CustomBigIconPath => "the_resilience_of_mercenaries_power.png".PowerImagePath();

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromCard<SovereignBlade>()];

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 只对自己打出的牌生效
        if (cardPlay.Card.Owner?.Creature != Owner) return;

        // 检查是否是君王之剑(SovereignBlade)
        if (cardPlay.Card is not SovereignBlade) return;

        // 遍历所有debuff，每层减1
        var debuffs = Owner.Powers.Where(p => p.Type == PowerType.Debuff).ToList();
        foreach (var debuff in debuffs)
        {
            int newAmount = debuff.Amount - 1;
            if (newAmount <= 0)
            {
                await PowerCmd.Remove(debuff);
            }
            else
            {
                debuff.SetAmount(newAmount);
            }
        }
    }
}
