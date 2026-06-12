using BaseLib.Abstracts;
using Doc.DocCode.CardPools;
using Doc.DocCode.Cards.Doctor.Basic;
using Doc.DocCode.Extensions;
using Doc.DocCode.RelicPools;
using Doc.DocCode.Relics;
using Doc.DocCode.PotionPools;
using Godot;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.PotionPools;
using MegaCrit.Sts2.Core.Models.RelicPools;
using System;
using System.Collections.Generic;

namespace Doc.DocCode.Characters;

public class DoctorCharacter : PlaceholderCharacterModel
{
    public const string CharacterId = "DoctorCharacter";

    // 保留 PlaceholderID，但通过重写其他属性来完全自定义
    public override string PlaceholderID => "necrobinder";

    public static readonly Color Color = new Color("c4278a");

    public override Color NameColor => Color;
    public override CharacterGender Gender => CharacterGender.Feminine;
    public override int StartingHp => 70;

    public override IEnumerable<CardModel> StartingDeck =>
    [
        ModelDb.Card<DocAttack>(),
        ModelDb.Card<DocAttack>(),
        ModelDb.Card<DocAttack>(),
        ModelDb.Card<DocAttack>(),
        ModelDb.Card<DocDefence>(),
        ModelDb.Card<DocDefence>(),
        ModelDb.Card<DocDefence>(),
        ModelDb.Card<DocDefence>(),
        ModelDb.Card<Orchestrate>(),
        ModelDb.Card<Plan>()
    ];

    // 初始遗物：人事部铜印
    public override IReadOnlyList<RelicModel> StartingRelics =>
        new List<RelicModel> { ModelDb.GetById<RelicModel>(new ModelId("RELIC", "HR_BRONZE_SEAL")) }.AsReadOnly();

    public override RelicPoolModel RelicPool => ModelDb.RelicPool<DoctorRelicPool>();
    public override CardPoolModel CardPool => ModelDb.CardPool<DocCardPool>();
    public override PotionPoolModel PotionPool => ModelDb.PotionPool<DocPotionPool>();

    // ========== 完全自定义以下所有属性 ==========
    
    // 战斗模型
    public override string CustomVisualPath => "res://doctor_character.tscn";

    // 足迹/拖尾效果
    //public override string CustomTrailPath => "res://doctor_trail.tscn";

    // 地图标记
    public override string? CustomMapMarkerPath => "map_marker.png".CharacterUiPath();


    public override string CustomIconPath => "doctor_love.tscn".CharacterUiPath();


    // 能量计数器
    public override string CustomEnergyCounterPath => "res://doctor_energy_counter.tscn";

    // 休息处动画
    public override string CustomRestSiteAnimPath => "res://doctor_rest_site.tscn";

    // 商店动画
    public override string CustomMerchantAnimPath => "res://doctor_merchant.tscn";

    // 选人界面背景
    public override string CustomCharacterSelectBg => "res://Doctor_bg.tscn";

    // 选人界面过渡材质
    //public override string CustomCharacterSelectTransitionPath => "res://doctor_transition_mat.tres";

    // 选人界面头像（已解锁）
    public override string? CustomCharacterSelectIconPath => "char_select_icon.png".CharacterUiPath();

    // 选人界面头像（锁定）
    public override string? CustomCharacterSelectLockedIconPath => "char_select_locked.png".CharacterUiPath();
    // 游戏内左上角小头像
    public override string CustomIconTexturePath => "character_icon.png".CharacterUiPath();

    // 音效（如果不想自定义，可以留空或使用默认）
    public override string CharacterSelectSfx => "event:/sfx/ui/char_select_generic";
    public override string CustomAttackSfx => "event:/sfx/characters/generic_attack";
    public override string CustomCastSfx => "event:/sfx/characters/generic_cast";
    public override string CustomDeathSfx => "event:/sfx/characters/generic_die";
}