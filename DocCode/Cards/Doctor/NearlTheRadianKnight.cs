using BaseLib.Utils;
using Doc.DocCode.Attributes;
using Doc.DocCode.Extensions;
using Doc.DocCode.Monsters;
using Doc.DocCode.Powers;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.ValueProps;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Doc.DocCode.Cards.Doctor;

[CardTags(isKazimierz: true, isKnight: true)]
public sealed class NearlTheRadianKnight() : DocCard(3, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("SummonAmount", 30m)
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var lastCard = GetLastPlayedCard();

        if (lastCard != null && lastCard.IsKazimierz())
        {
            // 费用减1
            EnergyCost.SetThisTurnOrUntilPlayed(CanonicalEnergyCost - 1);

            // 将一张复制品加入抽牌堆（带特效）
            var copy = CreateClone();
            var cards = new List<CardModel> { copy };
            var result = await CardPileCmd.AddGeneratedCardsToCombat(cards, PileType.Draw, Owner);
            CardCmd.PreviewCardPileAdd(result);
        }

        // 召唤耀阳
        await SummonBlazingSun(choiceContext);
    }

    private async Task SummonBlazingSun(PlayerChoiceContext choiceContext)
    {
        var summoner = Owner;
        var combatState = summoner.Creature.CombatState;
        decimal amount = DynamicVars["SummonAmount"].BaseValue;

        var existingSun = combatState.Allies.FirstOrDefault(c =>
            c.Monster is BlazingSun && c.PetOwner == summoner);

        if (existingSun != null && existingSun.IsAlive)
        {
            await CreatureCmd.GainMaxHp(existingSun, amount);
            await CreatureCmd.Heal(existingSun, amount);
        }
        else
        {
            var sun = await PlayerCmd.AddPet<BlazingSun>(summoner);
            await PowerCmd.Apply<BlazingSunDieForYouPower>(choiceContext, sun, 1m, null, null);
            await CreatureCmd.SetMaxHp(sun, amount);
            await CreatureCmd.Heal(sun, amount);
            await PowerCmd.Apply<NodPower>(choiceContext, sun, 1m, summoner.Creature, this);

            // ========== 替换视觉 ==========
            // 获取耀阳的 Creature 节点
            var creatureNode = NCombatRoom.Instance?.GetCreatureNode(sun);
            if (creatureNode != null)
            {
                // 移除默认的 NCreatureVisuals
                foreach (Node child in creatureNode.GetChildren())
                {
                    if (child is MegaCrit.Sts2.Core.Nodes.Combat.NCreatureVisuals)
                    {
                        child.QueueFree();
                    }
                }

                // 加载耀阳的场景
                var sunScene = GD.Load<PackedScene>("res://Doc/Scenes/create_visuals/BlazingSun.tscn");
                var sunVisual = sunScene.Instantiate<Node2D>();
                creatureNode.AddChild(sunVisual);
            }
            // =============================
        }
    }

    private CardModel? GetLastPlayedCard()
    {
        var currentRound = Owner.Creature.CombatState.RoundNumber;
        var history = CombatManager.Instance.History;

        var entry = history.CardPlaysFinished
            .LastOrDefault(e => e.CardPlay.Card.Owner == Owner
                && e.CardPlay.Card.CombatState?.RoundNumber == currentRound);

        if (entry == null)
        {
            entry = history.CardPlaysFinished
                .LastOrDefault(e => e.CardPlay.Card.Owner == Owner
                    && e.CardPlay.Card.CombatState?.RoundNumber == currentRound - 1);
        }

        return entry?.CardPlay.Card;
    }

    protected override void OnUpgrade()
    {
        DynamicVars["SummonAmount"].UpgradeValueBy(10m);
    }
}