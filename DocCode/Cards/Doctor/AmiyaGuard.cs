using Doc.DocCode.Attributes;
using Doc.DocCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Doc.DocCode.Cards.Doctor;

/// <summary>
/// 阿米娅·影霄（AmiyaGuard）：罗德岛，1 费攻击稀有，消耗（Exhaust）。
/// 打出：对生命最低的敌人造成 12 点伤害，获得 1 层调弦。
/// 斩杀时：伤害次数永久 +1，获得的调弦层数永久 +1（跨战斗持久）。
/// </summary>
[CardTags(isRhodeIsland: true,isLeithania:true,isBabel:true)]
public sealed class AmiyaGuard() : DocCard(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    private int _damageHits = 1;
    private int _increasedDamageHits;

    private int _tuneLayers = 1;
    private int _increasedTuneLayers;

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [
            HoverTipFactory.Static(StaticHoverTip.Fatal),
            HoverTipFactory.FromPower<TuneTheStringsPower>()
        ];

    [SavedProperty]
    public int DamageHits
    {
        get => _damageHits;
        set
        {
            AssertMutable();
            _damageHits = value;
            DynamicVars["DamageHits"].BaseValue = _damageHits;
        }
    }

    [SavedProperty]
    public int TuneLayers
    {
        get => _tuneLayers;
        set
        {
            AssertMutable();
            _tuneLayers = value;
            DynamicVars["TuneLayers"].BaseValue = _tuneLayers;
        }
    }

    [SavedProperty]
    public int IncreasedDamageHits
    {
        get => _increasedDamageHits;
        set
        {
            AssertMutable();
            _increasedDamageHits = value;
        }
    }

    [SavedProperty]
    public int IncreasedTuneLayers
    {
        get => _increasedTuneLayers;
        set
        {
            AssertMutable();
            _increasedTuneLayers = value;
        }
    }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(12m, ValueProp.Move),
        new IntVar("DamageHits", 1m),
        new IntVar("TuneLayers", 1m)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        bool anyKilled = false;

        // 持续攻击，直到没有可攻击的敌人或达到伤害次数
        for (int i = 0; i < DamageHits; i++)
        {
            if(CombatState.HittableEnemies==null || !CombatState.HittableEnemies.Any(e => e.IsAlive))
            {
                break; // 没有可攻击的敌人，退出循环
            }

            // 每次重新选择生命最低的可命中敌人
            var target = CombatState.HittableEnemies
                .Where(e => e.IsAlive)
                .OrderBy(e => e.CurrentHp)
                .FirstOrDefault();

            // 没有活着的敌人，停止攻击
            if (target == null)
                break;

            // 造成一次伤害
            var attack = await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
                .FromCard(this)
                .Targeting(target)
                .Execute(choiceContext);

            // 检查是否击杀
            if (attack.Results.SelectMany(r => r).Any(r => r.WasTargetKilled))
            {
                anyKilled = true;
            }
        }

        // 如果有任何一次击杀，永久增加伤害次数和调弦层数
        if (anyKilled)
        {
            IncreaseDamageHits(1);
            IncreaseTuneLayers(1);
            anyKilled = false; // 重置标志
        }

        // 获得调弦（层数 = TuneLayers）
        await PowerCmd.Apply<TuneTheStringsPower>(choiceContext, Owner.Creature, TuneLayers, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
    }

    protected override void AfterDowngraded()
    {
        UpdateDamageHits();
        UpdateTuneLayers();
    }

    private void IncreaseDamageHits(int amount)
    {
        IncreasedDamageHits += amount;
        UpdateDamageHits();
    }

    private void IncreaseTuneLayers(int amount)
    {
        IncreasedTuneLayers += amount;
        UpdateTuneLayers();
    }

    private void UpdateDamageHits()
    {
        DamageHits = 1 + IncreasedDamageHits;
    }

    private void UpdateTuneLayers()
    {
        TuneLayers = 1 + IncreasedTuneLayers;
    }

}