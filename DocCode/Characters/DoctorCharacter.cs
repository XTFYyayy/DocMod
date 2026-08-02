using BaseLib.Abstracts;
using Doc.DocCode.CardPools;
using Doc.DocCode.Cards.Doctor.Basic;
using Doc.DocCode.Extensions;
using Doc.DocCode.RelicsPools;
using Doc.DocCode.Relics;
using Doc.DocCode.PotionPools;
using Godot;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.PotionPools;
using MegaCrit.Sts2.Core.Models.RelicPools;
using System.Collections.Generic;

namespace Doc.DocCode.Characters;

public class DoctorCharacter : PlaceholderCharacterModel
{
    public const string CharacterId = "DoctorCharacter";

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

    // 修正：使用 ModelDb.Relic<T>() 而不是 ModelDb.GetById
    public override IReadOnlyList<RelicModel> StartingRelics =>
        new List<RelicModel> { ModelDb.Relic<HrBronzeSeal>() }.AsReadOnly();

    public override RelicPoolModel RelicPool => ModelDb.RelicPool<DoctorRelicPool>();
    public override CardPoolModel CardPool => ModelDb.CardPool<DocCardPool>();
    public override PotionPoolModel PotionPool => ModelDb.PotionPool<DocPotionPool>();

    // ========== 自定义属性 ==========
    public override string CustomVisualPath => "res://doctor_character.tscn";
    public override string? CustomMapMarkerPath => "map_marker.png".CharacterUiPath();
    public override string CustomIconPath => "doctor_love.tscn".CharacterUiPath();
    public override string CustomEnergyCounterPath => "res://doctor_energy_counter.tscn";
    public override string CustomRestSiteAnimPath => "res://doctor_rest_site.tscn";
    public override string CustomMerchantAnimPath => "res://doctor_merchant.tscn";
    public override string CustomCharacterSelectBg => "res://Doctor_bg.tscn";
    public override string? CustomCharacterSelectIconPath => "char_select_icon.png".CharacterUiPath();
    public override string? CustomCharacterSelectLockedIconPath => "char_select_locked.png".CharacterUiPath();
    public override string CustomIconTexturePath => "character_icon.png".CharacterUiPath();

    public override string CharacterSelectSfx => "event:/sfx/ui/char_select_generic";
    public override string CustomAttackSfx => "event:/sfx/characters/generic_attack";
    public override string CustomCastSfx => "event:/sfx/characters/generic_cast";
    public override string CustomDeathSfx => "event:/sfx/characters/generic_die";

    public override List<string> GetArchitectAttackVfx() => [
       "vfx/vfx_attack_blunt",
        "vfx/vfx_heavy_blunt",
        "vfx/vfx_attack_slash",
        "vfx/vfx_bloody_impact",
        "vfx/vfx_rock_shatter"
   ];
}